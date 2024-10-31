using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ImGuiNET;
using TidyChat.Attributes;
using TidyChat.Rules;
using TidyChat.Settings.Tabs;
namespace TidyChat;

internal class MainUI(Configuration configuration) : IDisposable
{
    private readonly Configuration configuration = configuration;
    private bool settingsVisible;
    public bool SettingsVisible { get => settingsVisible; set => settingsVisible = value; }
    internal static readonly Dictionary<Cate, Dictionary<SubCate, List<Rule>>> DrawableRules = GetGroupedDrawableRules();
    private static Dictionary<Cate, Dictionary<SubCate, List<Rule>>> GetGroupedDrawableRules()
    {
        return Enum.GetValues<Rule>()
        .Select(rule => (Cate: rule.GetAttribute<CateAttribute>(), Info: rule.GetAttribute<RuleInfoAttribute>(), Rule: rule))
        .Where(t => t.Cate != null && t.Info != null)
        .OrderBy(t => t.Cate!.Tab)
        .GroupBy(t => t.Cate!.Tab)
        .ToDictionary(
            group => group.Key,
            group => group.OrderBy(t => t.Cate!.CollapsingHeader)
                    .GroupBy(t => t.Cate!.CollapsingHeader)
                    .ToDictionary(
                        group => group.Key,
                        group => group.OrderBy(t => t.Rule)
                        .Select(t => t.Rule)
                        .ToList()));
    }
    public void Dispose()
    {
        // Have around in case we need it
    }
    public void Draw()
    {
        if (!SettingsVisible) return;
        ImGui.SetNextWindowSize(new Vector2(600, 450), ImGuiCond.FirstUseEver | ImGuiCond.Appearing);
        if (ImGui.Begin("Tidy Chat##CN", ref settingsVisible))
        {
            DrawTabs();
            ImGui.End();
        }
    }
    public void DrawTabs()
    {
        if (ImGui.BeginTabBar("##tidychatConfigTabs"))
        {
            GeneralTab.Draw(configuration);
            DrawRulesTabs();
            CustomFiltersTab.Draw(configuration);
            ChatHistoryTab.Draw(configuration);
            ImGui.EndTabBar();
        }
    }

    private void DrawRulesTabs()
    {
        foreach (var (tab, group) in MainUI.DrawableRules)
        {
            if (ImGui.BeginTabItem(tab.GetAttribute<DescriptionAttribute>()?.Description ?? tab.ToString()))
            {
                foreach (var (header, rules) in group)
                {
                    var text = header.GetAttribute<DescriptionAttribute>()?.Description ?? "";
                    if (string.IsNullOrWhiteSpace(text) || ImGui.CollapsingHeader(text))
                    {
                        foreach (var (ruleKey, info) in rules.Select(r => (r.ToString(), r.GetAttribute<RuleInfoAttribute>())).Where(r => r.Item2 != null))
                        {
                            if (!configuration.RuleActive.TryGetValue(ruleKey!, out var val))
                            {
                                val = false;
                            };
                            if (ImGui.Checkbox(info!.Description, ref val))
                            {
                                configuration.RuleActive[ruleKey] = val;
                                configuration.Save();
                            }
                            if (!string.IsNullOrWhiteSpace(info.HelpMessage))
                            {
                                ImGuiComponents.HelpMarker(info.HelpMessage);
                            }
                        }
                    }
                }
                ImGui.EndTabItem();
            }
        }
    }
}
