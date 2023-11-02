﻿using Dalamud.Interface.Components;

namespace SubmarineTracker.Windows.Config;

public partial class ConfigWindow
{
    private void Overlay()
    {
        if (ImGui.BeginTabItem($"{Loc.Localize("Config Tab - Overlay", "Overlay")}##Overlay"))
        {
            var changed = false;
            ImGuiHelpers.ScaledDummy(5.0f);

            ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("Config Tab Entry - Open", "Open:"));
            ImGui.Indent(10.0f);
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - On Startup", "On Startup"), ref Configuration.OverlayStartUp);
            ImGuiComponents.HelpMarker(Loc.Localize("Config Tab Tooltip - On Startup", "Opens the overlay on startup, if at least one sub is done"));
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - On Return", "On Return"), ref Configuration.OverlayAlwaysOpen);
            ImGuiComponents.HelpMarker(Loc.Localize("Config Tab Tooltip - On Return", "Always opens the overlay if one submarine returns"));
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - Automatically Close", "Automatically Close"), ref Configuration.OverlayHoldClosed);
            ImGuiComponents.HelpMarker(Loc.Localize("Config Tab Tooltip - Automatically Close", "Automatically closes the overlay if all submarines are on voyage, and holds it closed"));
            ImGui.Unindent(10.0f);

            ImGuiHelpers.ScaledDummy(5.0f);

            ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("Config Tab Entry - General", "General:"));
            ImGui.Indent(10.0f);
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - Only FC Tag", "Only FC Tag"), ref Configuration.OverlayOnlyFCTag);
            ImGuiComponents.HelpMarker(Loc.Localize("Config Tab Tooltip - Only FC Tag", "Shows only the FC tag without @World."));
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - Use Character Name", "Use Character Name"), ref Configuration.OverlayCharacterName);
            ImGuiComponents.HelpMarker(Loc.Localize("Config Tab Tooltip - Use Character Name", "Use character name instead of FC tag.\nBe aware this option can lead to cut-off text."));
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - Show Rank", "Show Rank"), ref Configuration.OverlayShowRank);
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - Show Build", "Show Build"), ref Configuration.OverlayShowBuild);
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - As Date", "Show As Date"), ref Configuration.OverlayShowDate);
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - First Return Time", "Show First Return Time"), ref Configuration.OverlayFirstReturn);
            ImGuiComponents.HelpMarker(Loc.Localize("Config Tab Tooltip - First Return Time", "Shows the first returning sub in the overlay headers for each FC."));
            if (ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - Sort Time Lowest", "Sort By Lowest Time"), ref Configuration.OverlaySort))
            {
                changed = true;
                Configuration.OverlaySortReverse = false;
            }
            ImGuiComponents.HelpMarker(Loc.Localize("Config Tab Tooltip - Sort Time Lowest", "Sorts the overlay headers by return time, this overwrites FC order."));

            if (Configuration.OverlaySort)
            {
                ImGui.Indent(10.0f);
                changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - Reverse Sort", "Reverse Sort"), ref Configuration.OverlaySortReverse);
                ImGui.Unindent(10.0f);
            }
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - Only Returned", "Only Show Returned"), ref Configuration.OverlayOnlyReturned);
            ImGui.Unindent(10.0f);

            ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("Config Tab Entry - Colors", "Colors:"));
            ImGui.Indent(10.0f);
            var spacing = 150 * ImGuiHelpers.GlobalScale;
            changed |= Helper.ColorPickerWithReset(Loc.Localize("Config Tab Colors - All Done", "All Done"), ref Configuration.OverlayAllDone, Helper.CustomFullyDone, spacing);
            changed |= Helper.ColorPickerWithReset(Loc.Localize("Config Tab Colors - Partly Done", "Partly Done"), ref Configuration.OverlayPartlyDone, Helper.CustomPartlyDone,spacing);
            changed |= Helper.ColorPickerWithReset(Loc.Localize("Config Tab Colors - None Done", "None Done"), ref Configuration.OverlayNoneDone, Helper.CustomOnRoute, spacing);
            ImGui.Unindent(10.0f);

            ImGui.TextColored(ImGuiColors.DalamudViolet, Loc.Localize("Config Tab Entry - Window", "Window:"));
            ImGui.Indent(10.0f);
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - Lock Size", "Lock Size"), ref Configuration.OverlayLockSize);
            changed |= ImGui.Checkbox(Loc.Localize("Config Tab Checkbox - Lock Position", "Lock Position"), ref Configuration.OverlayLockLocation);
            ImGui.Unindent(10.0f);

            if (changed)
                Configuration.Save();

            ImGui.EndTabItem();
        }
    }
}
