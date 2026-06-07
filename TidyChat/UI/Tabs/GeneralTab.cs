using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;

namespace TidyChat.Settings.Tabs;

internal static class GeneralTab
{
    public static void Draw(Configuration configuration)
    {
        if (ImGui.BeginTabItem("通常"))
        {
            var enabled = configuration.Enabled;
            if (ImGui.Checkbox("启用插件", ref enabled))
            {
                configuration.Enabled = enabled;
                configuration.Save();
            }
            ImGui.TextUnformatted($"TidyChat至今已经拦截 {configuration.TtlMessagesBlocked:n0} 条消息");
            ImGuiComponents.HelpMarker($"计数只在每拦截 100 条消息、切换区域或下线时更新");
            
            var enableDebugMode = configuration.EnableDebugMode;
            if (ImGui.Checkbox("启用调试模式：总是显示本应被过滤的消息，也会显示对应的过滤规则", ref enableDebugMode))
            {
                configuration.EnableDebugMode = enableDebugMode;
                configuration.Save();
            }

            var debugIncludeChannel = configuration.IncludeLogKind;
            if (ImGui.Checkbox("注明消息所属频道", ref debugIncludeChannel))
            {
                configuration.IncludeLogKind = debugIncludeChannel;
                configuration.Save();
            }

            ImGui.Separator();
            DrawImprovedMessages(configuration);
            ImGui.EndTabItem();
        }
    }

    private static void DrawImprovedMessages(Configuration configuration)
    {
        if (ImGui.CollapsingHeader("改进消息"))
        {
            var includeChatTag = configuration.IncludeChatTag;
            if (ImGui.Checkbox("给改进后的消息添加 [TidyChat] 标签", ref includeChatTag))
            {
                configuration.IncludeChatTag = includeChatTag;
                configuration.Save();
            }
            ImGuiComponents.HelpMarker("给由Tidy Chat发送或修改后的消息添加[TidyChat]前缀");
            
            ImGui.Spacing();
            ImGui.TextUnformatted($"以下改进后的消息将总是显示出来，无论原始消息是否被过滤");

            var betterInstanceMessage = configuration.BetterInstanceMessage;
            if (ImGui.Checkbox("改进副本区提示", ref betterInstanceMessage))
            {
                configuration.BetterInstanceMessage = betterInstanceMessage;
                configuration.Save();
            }
            ImGuiComponents.HelpMarker("改变副本区消息为“当前副本区: #”");

            var betterCommendationMessage = configuration.BetterCommendationMessage;
            if (ImGui.Checkbox("改进最优队员推荐消息", ref betterCommendationMessage))
            {
                configuration.BetterCommendationMessage = betterCommendationMessage;
                if (!configuration.BetterCommendationMessage && configuration.IncludeDutyNameInComms)
                    configuration.IncludeDutyNameInComms = false;
                configuration.Save();
            }
            ImGuiComponents.HelpMarker("将多条获赞的提示整合为单条并显示 (建议同时勾选隐藏获得了最优队员评价)");
            
            using (ImRaii.PushIndent())
            {
                var includeDutyNameInComms = configuration.IncludeDutyNameInComms;
                if (ImGui.Checkbox("最优队员推荐消息包含副本名称", ref includeDutyNameInComms))
                {
                    configuration.IncludeDutyNameInComms = includeDutyNameInComms;
                    if (!configuration.BetterCommendationMessage && configuration.IncludeDutyNameInComms)
                        configuration.BetterCommendationMessage = true;
                    configuration.Save();
                }
            }

            var betterSayReminder = configuration.BetterSayReminder;
            if (ImGui.Checkbox("改进任务要求在说话频道发送指定内容的提示信息", ref betterSayReminder))
            {
                configuration.BetterSayReminder = betterSayReminder;
                if (!configuration.BetterSayReminder && configuration.CopyBetterSayReminder)
                    configuration.CopyBetterSayReminder = false;
                configuration.Save();
            }
            ImGuiComponents.HelpMarker("当任务要求你在说话频道发送特定内容时，把那条提示信息改为相应的宏文本");
            
            using (ImRaii.PushIndent())
            {
                var copyBetterSayReminder = configuration.CopyBetterSayReminder;
                if (ImGui.Checkbox("自动复制改进后的说话宏命令到剪贴板", ref copyBetterSayReminder))
                {
                    configuration.CopyBetterSayReminder = copyBetterSayReminder;
                    if (!configuration.BetterSayReminder && configuration.CopyBetterSayReminder)
                        configuration.BetterSayReminder = true;
                    configuration.Save();
                }
            }

            var betterEmoteReminder = configuration.BetterEmoteReminder;
            if (ImGui.Checkbox("改进任务要求使用指定情感动作的提示信息", ref betterEmoteReminder))
            {
                configuration.BetterEmoteReminder = betterEmoteReminder;
                if (!configuration.BetterEmoteReminder && configuration.CopyBetterEmoteReminder)
                    configuration.CopyBetterEmoteReminder = false;
                configuration.Save();
            }
            ImGuiComponents.HelpMarker("当任务要求你使用特定情感动作时，把那条提示信息改为相应的宏文本");
            using (ImRaii.PushIndent())
            {
                var copyBetterEmoteReminder = configuration.CopyBetterEmoteReminder;
                if (ImGui.Checkbox("自动复制改进后的情感动作宏命令到剪贴板", ref copyBetterEmoteReminder))
                {
                    configuration.CopyBetterEmoteReminder = copyBetterEmoteReminder;
                    if (!configuration.BetterEmoteReminder && configuration.CopyBetterEmoteReminder)
                        configuration.BetterEmoteReminder = true;
                    configuration.Save();
                }
            }

            var betterNoviceNetworkMessage = configuration.BetterNoviceNetworkMessage;
            if (ImGui.Checkbox("改进加入新人频道的消息", ref betterNoviceNetworkMessage))
            {
                configuration.BetterNoviceNetworkMessage = betterNoviceNetworkMessage;
                configuration.Save();
            }
            ImGuiComponents.HelpMarker("减少显示的文本量");
        }
    }
}