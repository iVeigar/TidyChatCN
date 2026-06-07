using System;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;

namespace TidyChat.Settings.Tabs;

internal static class CustomFiltersTab
{
    private static CustomFilter m_placeholder = new();

    public static void Draw(Configuration configuration)
    {
        if (ImGui.BeginTabItem("自定义"))
        {
            var allowCustomFilter = configuration.AllowCustomFilter;
            if (ImGui.Checkbox("使用自定义过滤器（优先级高于内置过滤器，可能会放行原本会被内置过滤器阻拦的消息）", ref allowCustomFilter))
            {
                configuration.AllowCustomFilter = allowCustomFilter;
                configuration.Save();
            }

            var outer_height = new Vector2(600f, 400f);
            if (ImGui.BeginTable("##CustomFilterTable", 4, ImGuiTableFlags.ScrollY | ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg, outer_height))
            {
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableSetupColumn("选择频道", ImGuiTableColumnFlags.WidthFixed, 100f);
                ImGui.TableSetupColumn("发送者/包含的消息内容", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("模式", ImGuiTableColumnFlags.WidthFixed, 80f);
                ImGui.TableSetupColumn("操作", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableHeadersRow();
                var list = configuration.CustomFilters.ToList();
                for (var i = -1; i < list.Count; i++)
                {
                    var alias = i < 0 ? m_placeholder : list[i];

                    ImGui.TableNextColumn();
                    ImGui.Spacing();
                    ImGui.SetNextItemWidth(100f);
                    var xivChatTypeNames = Enum.GetNames<XivChatType>();
                    var xivChatTypeValues = Enum.GetValues<XivChatType>();
                    var enumCurrent = Array.IndexOf(xivChatTypeValues, alias.SelectedChatType);
                    if (ImGui.Combo($"##CustomFilter_{i}_LogKind", ref enumCurrent, xivChatTypeNames, xivChatTypeNames.Length))
                    {
                        alias.SelectedChatType = xivChatTypeValues[enumCurrent];
                        configuration.Save();
                    }
                    ImGui.Spacing();

                    ImGui.TableNextColumn();
                    ImGui.Spacing();
                    ImGui.SetNextItemWidth(-1);
                    var content = alias.MessageOrPlayerName;
                    if (ImGui.InputText($"##CustomFilter_{i}_MessageOrPlayerName", ref content, 120,
                            ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        if (alias.MessageOrPlayerName != content && !content.IsNullOrEmpty())
                        {
                            alias.MessageOrPlayerName = content;
                            if (i != -1)
                                configuration.Save();
                        }
                    }
                    ImGui.Spacing();

                    ImGui.TableNextColumn();
                    ImGui.Spacing();
                    var previewValue = alias.IsAllowed ? "允许" : "阻拦";
                    ImGui.SetNextItemWidth(-1);
                    if (ImGui.BeginCombo($"##CustomFilter_{i}_AllowSetting", previewValue))
                    {
                        if (ImGui.Selectable($"允许##{i}", alias.IsAllowed))
                        {
                            alias.IsAllowed = true;
                            if (i != -1)
                                configuration.Save();
                        }

                        if (ImGui.Selectable($"阻拦##{i}", !alias.IsAllowed))
                        {
                            alias.IsAllowed = false;
                            if (i != -1)
                                configuration.Save();
                        }
                        ImGui.EndCombo();
                    }
                    ImGui.Spacing();

                    ImGui.TableNextColumn();
                    ImGui.Spacing();
                    if (i == -1)
                    {
                        using (ImRaii.Disabled(alias.MessageOrPlayerName.IsNullOrEmpty()))
                        {
                            if (ImGuiComponents.IconButton($"New{i}", FontAwesomeIcon.Plus))
                            {
                                configuration.CustomFilters.Add(alias);
                                m_placeholder = new CustomFilter();
                                configuration.Save();
                            }
                        }
                    }
                    else
                    {
                        if (ImGuiComponents.IconButton($"Delete{i}", FontAwesomeIcon.Trash))
                        {
                            configuration.CustomFilters.Remove(list[i]);
                            configuration.Save();
                        }
                    }
                    ImGui.Spacing();
                }
                ImGui.EndTable();
            }
            ImGui.EndTabItem();
        }
    }
}