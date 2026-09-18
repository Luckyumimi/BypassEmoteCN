using FFXIVClientStructs.FFXIV.Client.System.String;
using System.Runtime.InteropServices;

namespace BypassEmote.Helpers;

/// <summary>
/// Reads a game <see cref="Utf8String"/> without ever following a pointer that cannot be proved readable.
/// <para/>
/// <c>Utf8String.ToString()</c> builds its span straight out of <c>StringPtr</c> (it does not fall back to the
/// inline buffer), so the decoder reads whatever address happens to sit in that field. When the struct is not
/// really a string - because a node was identified by a raw type number rather than by the struct the game
/// actually put there, or because the addon is still being set up and the field has not been filled in yet -
/// that address is garbage and the read is an <see cref="AccessViolationException"/>. The runtime cannot catch
/// those, so one bad read takes the whole game down instead of failing this single lookup.
/// <para/>
/// Everything here therefore stays on the safe side of that line: the length is sanity checked, the address range
/// is probed with <c>VirtualQuery</c>, and anything that does not check out simply reads as an empty string.
/// </summary>
internal static unsafe class SafeText
{
    /// <summary> Longest string we are willing to decode. Anything bigger is a misread struct, not real text. </summary>
    private const int MaxBytes = 4096;

    private const uint MemCommit = 0x1000;

    private const uint PageNoAccess = 0x01;

    private const uint PageGuard = 0x100;

    /// <summary> Read, write, copy on write, or any of the executable-but-readable protection flags. </summary>
    private const uint ReadableProtection = 0x02 | 0x04 | 0x08 | 0x20 | 0x40 | 0x80;

    [StructLayout(LayoutKind.Sequential)]
    private struct MemoryBasicInformation
    {
        public void* BaseAddress;
        public void* AllocationBase;
        public uint AllocationProtect;
        public ushort PartitionId;
        public nuint RegionSize;
        public uint State;
        public uint Protect;
        public uint Type;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nuint VirtualQuery(void* address, MemoryBasicInformation* buffer, nuint length);

    /// <summary> Whether every byte in the range can be read without faulting. </summary>
    internal static bool IsReadable(void* address, int length)
    {
        if (address == null || length <= 0)
            return false;

        var start = (byte*)address;
        var end = start + length;
        var cursor = start;

        while (cursor < end)
        {
            MemoryBasicInformation info;

            if (VirtualQuery(cursor, &info, (nuint)sizeof(MemoryBasicInformation)) == 0)
                return false;

            if (info.State != MemCommit
                || (info.Protect & (PageNoAccess | PageGuard)) != 0
                || (info.Protect & ReadableProtection) == 0)
            {
                return false;
            }

            var regionEnd = (byte*)info.BaseAddress + info.RegionSize;

            // A region that does not move forward would spin forever.
            if (regionEnd <= cursor)
                return false;

            cursor = regionEnd;
        }

        return true;
    }

    /// <summary> Trimmed contents of a game string, or an empty string when it cannot be read safely. </summary>
    internal static string Utf8(Utf8String value)
    {
        var span = value.AsSpan();

        if (span.Length <= 0 || span.Length > MaxBytes)
            return string.Empty;

        fixed (byte* pointer = span)
        {
            // Building the span above does not touch the memory it points at; only ToString() does, so the probe
            // has to happen here, before it.
            return IsReadable(pointer, span.Length) ? value.ToString().Trim() : string.Empty;
        }
    }
}