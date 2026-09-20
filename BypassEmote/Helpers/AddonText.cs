using FFXIVClientStructs.FFXIV.Component.GUI;
using NoireLib.Helpers;
using System.Collections.Generic;

namespace BypassEmote.Helpers;

internal static unsafe class AddonText
{
    private const int MaxNodes = 1024;
    private const int MaxDepth = 4;
    private const ushort TextInputType = 1007;

    internal static bool TryGetComponent(AtkUnitBase* addon, uint nodeId, ComponentType expected, out AtkComponentBase* component)
    {
        component = null;
        if (addon == null || !AddonHelper.TryGetNode(addon, nodeId, out var node)
            || !AddonHelper.TryGetComponentNode(node, out var componentNode) || componentNode->Component == null)
            return false;
        component = componentNode->Component;
        return true;
    }

    internal static bool TryGetComponent(AtkResNode* node, ComponentType expected, out AtkComponentBase* component)
    {
        component = null;
        if (node == null || !AddonHelper.TryGetComponentNode(node, out var componentNode) || componentNode->Component == null)
            return false;
        component = componentNode->Component;
        return true;
    }

    internal static bool TryGetComponentList(AtkUnitBase* addon, uint nodeId, out AtkComponentList* list)
    {
        list = null;
        if (!TryGetComponent(addon, nodeId, ComponentType.List, out var component))
            return false;
        list = (AtkComponentList*)component;
        return true;
    }

    internal static bool TryReadTextInput(AtkUnitBase* addon, out string text)
    {
        text = addon == null ? string.Empty : Search(&addon->UldManager, 0);
        return text.Length > 0;
    }

    internal static IEnumerable<string> ReadComponentTexts(AtkComponentBase* component)
    {
        var result = new List<string>();
        Collect(component, 0, result);
        return result;
    }

    private static void Collect(AtkComponentBase* component, int depth, List<string> result)
    {
        if (component == null || depth > MaxDepth)
            return;
        var manager = &component->UldManager;
        if (manager->NodeList == null || manager->NodeListCount > MaxNodes)
            return;
        for (var i = 0; i < manager->NodeListCount; i++)
        {
            var node = manager->NodeList[i];
            if (node == null)
                continue;
            if (AddonHelper.TryReadText(node, out var text) && !string.IsNullOrWhiteSpace(text))
                result.Add(text);
            if (AddonHelper.TryGetComponentNode(node, out var child) && child->Component != null)
                Collect(child->Component, depth + 1, result);
        }
    }

    private static string Search(AtkUldManager* manager, int depth)
    {
        if (manager == null || depth > MaxDepth || manager->NodeList == null || manager->NodeListCount > MaxNodes)
            return string.Empty;
        for (var i = 0; i < manager->NodeListCount; i++)
        {
            var node = manager->NodeList[i];
            if (node == null || !AddonHelper.TryGetComponentNode(node, out var componentNode) || componentNode->Component == null)
                continue;
            if ((ushort)node->Type == TextInputType)
            {
                var input = (AtkComponentTextInput*)componentNode->Component;
                var raw = SafeText.Utf8(input->AtkComponentInputBase.RawString);
                return raw.Length > 0 ? raw : SafeText.Utf8(input->AtkComponentInputBase.EvaluatedString);
            }
            var nested = Search(&componentNode->Component->UldManager, depth + 1);
            if (nested.Length > 0)
                return nested;
        }
        return string.Empty;
    }
}
