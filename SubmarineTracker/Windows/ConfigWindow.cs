﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using SubmarineTracker.Data;

using static SubmarineTracker.Utils;

namespace SubmarineTracker.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;
    private static ExcelSheet<Item> ItemSheet = null!;

    private ComboWithFilter<FakeItem> ItemFilter;
    private FakeItem SelectedItem = FakeItem.Empty();

    public ConfigWindow(Plugin plugin) : base("Configuration")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(320, 460),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Configuration = plugin.Configuration;
        ItemSheet = Plugin.Data.GetExcelSheet<Item>()!;

        var itemList = new List<FakeItem>();
        foreach (var item in ItemSheet.Where(i => ToStr(i.Name) != ""))
            itemList.Add(new FakeItem(ToStr(item.Name), item));

        IReadOnlyList<FakeItem> readOnlyItemList = itemList;
        ItemFilter = new ComboWithFilter<FakeItem>("##Add", 240.0f, 240.0f, readOnlyItemList, s => s.Name) {
            Flags = ImGuiComboFlags.NoArrowButton | ImGuiComboFlags.HeightLarge,
            ItemsAtOnce = 12,
        };
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (ImGui.BeginTabBar("##ConfigTabBar"))
        {
            if (ImGui.BeginTabItem("General"))
            {
                var changed = false;
                changed |= ImGui.Checkbox("Show Extended Parts List", ref Configuration.ShowExtendedPartsList);
                changed |= ImGui.Checkbox("Show Time in Overview", ref Configuration.ShowTimeInOverview);
                if (Configuration.ShowTimeInOverview)
                {
                    ImGui.Indent(10.0f);
                    changed |= ImGui.Checkbox("Show Return Date Instead", ref Configuration.UseDateTimeInstead);
                    ImGui.Unindent(10.0f);
                }

                changed |= ImGui.Checkbox("Show Route in Overview", ref Configuration.ShowRouteInOverview);
                changed |= ImGui.Checkbox("Use Character Name", ref Configuration.UseCharacterName);
                ImGuiComponents.HelpMarker("Use character name instead of FC tag in the overview.\n" +
                                           "If the FC tag is still shown, this means your character name has yet to be saved, this will resolve itself the next time your submarines are sent out.\n" +
                                           "Be aware this option can lead to cut-off button text.");

                if (changed)
                    Configuration.Save();

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Saves"))
            {
                if (ImGui.BeginTable("##DeleteSavesTable", 2))
                {
                    ImGui.TableSetupColumn("Saved Setup");
                    ImGui.TableSetupColumn("Del", 0, 0.2f);

                    ImGui.TableHeadersRow();

                    ulong deletion = 0;
                    foreach (var (id, fc) in Submarines.KnownSubmarines)
                    {
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{fc.Tag}@{fc.World}");

                        ImGui.TableNextColumn();
                        if (ImGuiComponents.IconButton((int)id, FontAwesomeIcon.Trash))
                            deletion = id;

                        ImGui.TableNextRow();
                    }

                    if (deletion != 0)
                        Submarines.DeleteCharacter(deletion);

                    ImGui.EndTable();
                }

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Loot"))
            {
                var change  = ItemFilter.Draw(SelectedItem.Name, out var newItem, 240.0f) && newItem.Item.RowId != SelectedItem.Item.RowId;

                if (change)
                {
                    SelectedItem = newItem;
                    var value = (int) (newItem.Item.PriceLow > 1000 ? newItem.Item.PriceLow : 0);
                    Configuration.CustomLootWithValue.Add(newItem.Item.RowId, value);
                    Configuration.Save();
                }

                ImGuiHelpers.ScaledDummy(5);
                ImGui.Separator();
                ImGuiHelpers.ScaledDummy(5);

                if (ImGui.BeginTable("##DeleteLootTable", 3))
                {
                    ImGui.TableSetupColumn("Item");
                    ImGui.TableSetupColumn("Value", 0, 0.4f);
                    ImGui.TableSetupColumn("Del", 0, 0.15f);

                    ImGui.TableHeadersRow();

                    var deletion = -1;
                    foreach (var ((item, value), idx) in Configuration.CustomLootWithValue.Select((val, i) => (val, i)))
                    {
                        var resolvedItem = ItemSheet.GetRow(item)!;
                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted($"{ToStr(resolvedItem.Name)}");

                        ImGui.TableNextColumn();
                        var val = value;
                        ImGui.SetNextItemWidth(-1);
                        if (ImGui.InputInt($"##inputValue{item}", ref val, 0))
                        {
                            val = Math.Clamp(val, 0, int.MaxValue);
                            Configuration.CustomLootWithValue[item] = val;
                            Configuration.Save();
                        }

                        ImGui.TableNextColumn();
                        if (ImGuiComponents.IconButton(idx, FontAwesomeIcon.Trash))
                            deletion = idx;

                        ImGui.TableNextRow();
                    }

                    if (deletion != -1)
                    {
                        Configuration.CustomLoot.RemoveAt(deletion);
                        Configuration.Save();
                    }

                    ImGui.EndTable();
                }

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("About"))
            {
                if (ImGui.BeginChild("AboutContent", new Vector2(0, -50)))
                {
                    ImGuiHelpers.ScaledDummy(10);

                    ImGui.TextUnformatted("Author:");
                    ImGui.SameLine();
                    ImGui.TextColored(ImGuiColors.ParsedGold, Plugin.Authors);

                    ImGui.TextUnformatted("Discord:");
                    ImGui.SameLine();
                    ImGui.TextColored(ImGuiColors.ParsedGold, "Infi#6958");

                    ImGui.TextUnformatted("Version:");
                    ImGui.SameLine();
                    ImGui.TextColored(ImGuiColors.ParsedOrange, Plugin.Version);
                }
                ImGui.EndChild();

                ImGuiHelpers.ScaledDummy(5);
                ImGui.Separator();
                ImGuiHelpers.ScaledDummy(5);

                if (ImGui.BeginChild("AboutBottomBar", new Vector2(0, 0), false, 0))
                {
                    ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.ParsedBlue);
                    if (ImGui.Button("Discord Thread"))
                        Dalamud.Utility.Util.OpenLink("https://canary.discord.com/channels/581875019861328007/1094255662860599428");
                    ImGui.PopStyleColor();

                    ImGui.SameLine();

                    ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.DPSRed);
                    if (ImGui.Button("Issues"))
                        Dalamud.Utility.Util.OpenLink("https://github.com/Infiziert90/SubmarineTracker/issues");
                    ImGui.PopStyleColor();
                }
                ImGui.EndChild();

                ImGui.EndTabItem();
            }
        }
        ImGui.EndTabBar();
    }
}
