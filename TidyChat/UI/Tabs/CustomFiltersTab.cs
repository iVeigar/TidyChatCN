using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace TidyChat.Settings.Tabs;

internal static class CustomFiltersTab
{
    private static CustomFilter m_placeholder = new();

    public static void Draw(Configuration configuration)
    {
        if (ImGui.BeginTabItem("自定义"))
        {
            var allowCustomFilter = configuration.AllowCustomFilter;
            if (ImGui.Checkbox("使用自定义过滤器（优先级更高）", ref allowCustomFilter))
            {
                configuration.AllowCustomFilter = allowCustomFilter;
                configuration.Save();
            }

            var outer_height = new Vector2(640f, 400f);
            if (ImGui.BeginTable("##whitelistTable", 4, ImGuiTableFlags.ScrollY | ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg, outer_height))
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableSetupColumn("选择频道", ImGuiTableColumnFlags.WidthFixed, 120f);
                ImGui.TableSetupColumn("过滤器（两侧有\"/\"视为正则表达式）", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("##AllowColumn", ImGuiTableColumnFlags.WidthFixed, 120f);
                ImGui.TableSetupColumn("##DeleteColumn", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableHeadersRow();
                var list = configuration.CustomFilters.ToList();
                for (var i = -1; i < list.Count; i++)
                {
                    var alias = i < 0 ? m_placeholder : list[i];

                    ImGui.TableNextColumn();
                    ImGui.Spacing();
                    if (ImGui.CollapsingHeader($"频道##whitelist{i}ChannelsHeader"))
                    {
                        if (ImGui.CheckboxFlags(
                                $"系统##whitelist{i}OverrideSystemFilters",
                                ref alias.whitelistedChannels,
                                1 << 3)) configuration.Save();
                        if (ImGui.CheckboxFlags(
                                $"情感动作##whitelist{i}OverrideEmoteFilters",
                                ref alias.whitelistedChannels, 1 << 1))
                            configuration.Save();
                        if (ImGui.CheckboxFlags($"战利品##whitelist{i}OverrideLootFilters",
                                ref alias.whitelistedChannels, 1 << 5))
                            configuration.Save();
                        if (ImGui.CheckboxFlags(
                                $"制作##whitelist{i}OverrideCraftingFilters",
                                ref alias.whitelistedChannels,
                                1 << 8)) configuration.Save();
                        if (ImGui.CheckboxFlags(
                                $"采集##whitelist{i}OverrideGatheringFilters",
                                ref alias.whitelistedChannels,
                                1 << 9)) configuration.Save();
                        if (ImGui.CheckboxFlags(
                                $"部队成员上下线##whitelist{i}OverrideFreeCompanyFilters",
                                ref alias.whitelistedChannels, 1 << 7)) configuration.Save();
                        if (ImGui.CheckboxFlags(
                                $"进度/成长##whitelist{i}OverrideProgressFilters",
                                ref alias.whitelistedChannels,
                                1 << 4)) configuration.Save();
                    }

                    ImGui.Spacing();

                    ImGui.TableNextColumn();
                    ImGui.Spacing();
                    if (i == -1) ImGui.TextUnformatted("消息内容包含：");

                    ImGui.SetNextItemWidth(-1);
                    if (ImGui.InputText($"##whitelist{i}FirstNameInput", ref alias.MessageOrPlayerName, 120,
                            ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        if (i == -1)
                        {
                            configuration.CustomFilters.Insert(0, alias);
                            m_placeholder = new CustomFilter();
                        }

                        configuration.Save();
                    }

                    ImGuiHelpers.ScaledDummy(10f);

                    ImGui.TableNextColumn();
                    ImGui.Spacing();
                    if (i != -1)
                    {
                        var previewValue = "";
                        if (alias.IsAllowed)
                            previewValue = "允许";
                        else
                            previewValue = "阻拦";
                        ImGui.SetNextItemWidth(-1);
                        using (ImRaii.Combo($"##whitelist{i}AllowSetting", previewValue))
                        {
                            if (ImGui.Selectable($"允许##{i}", alias.IsAllowed))
                            {
                                alias.IsAllowed = true;
                                configuration.Save();
                            }

                            if (ImGui.Selectable($"阻拦##{i}", !alias.IsAllowed))
                            {
                                alias.IsAllowed = false;
                                configuration.Save();
                            }
                        }
                    }

                    ImGui.TableNextColumn();
                    ImGui.Spacing();
                    using var id = ImRaii.PushId($"Delete{i}");
                    if (i != -1 && ImGuiComponents.IconButton(FontAwesomeIcon.Trash))
                    {
                        configuration.CustomFilters.Remove(list[i]);
                        configuration.Save();
                    }
                }
                ImGui.EndTable();
            }
            ImGui.EndTabItem();
        }
    }
}