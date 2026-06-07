using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using TidyChat.Attributes;
using TidyChat.Rules;
using TidyChat.Settings.Tabs;
namespace TidyChat;

public sealed class MainUI: Window
{
    private readonly Configuration configuration;
    public MainUI(Configuration configuration) : base("Tidy Chat")
    {
        SizeConstraints = new WindowSizeConstraints { MinimumSize = new Vector2(640, 400), MaximumSize = new Vector2(float.MaxValue, float.MaxValue) };
        this.configuration = configuration;
    }

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

    public override void Draw()
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
        foreach (var (tab, group) in DrawableRules)
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
