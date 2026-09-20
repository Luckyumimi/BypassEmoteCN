using FFXIVClientStructs.FFXIV.Client.System.String;
using System.Runtime.InteropServices;

namespace BypassEmote.Helpers;

internal static unsafe class SafeText
{
    private const int MaxBytes = 4096;
    private const uint MemCommit = 0x1000;
    private const uint PageNoAccess = 0x01;
    private const uint PageGuard = 0x100;
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
            if (info.State != MemCommit || (info.Protect & (PageNoAccess | PageGuard)) != 0
                || (info.Protect & ReadableProtection) == 0)
                return false;
            var regionEnd = (byte*)info.BaseAddress + info.RegionSize;
            if (regionEnd <= cursor)
                return false;
            cursor = regionEnd;
        }
        return true;
    }

    internal static string Utf8(Utf8String value)
    {
        var span = value.AsSpan();
        if (span.Length <= 0 || span.Length > MaxBytes)
            return string.Empty;
        fixed (byte* pointer = span)
            return IsReadable(pointer, span.Length) ? value.ToString().Trim() : string.Empty;
    }
}
