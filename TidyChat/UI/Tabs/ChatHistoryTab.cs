using System.Globalization;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace TidyChat.Settings.Tabs;

internal static class ChatHistoryTab
{
    public static void Draw(Configuration configuration)
    {
        if (ImGui.BeginTabItem("消息记录"))
        {
            var chatHistoryFilter = configuration.ChatHistoryFilter;
            if (ImGui.Checkbox("过滤重复消息", ref chatHistoryFilter))
            {
                configuration.ChatHistoryFilter = chatHistoryFilter;
                configuration.Save();
            }

            ImGuiComponents.HelpMarker(string.Format(CultureInfo.CurrentCulture, "如果某条消息在最近{0}条消息中重复出现则将被过滤。发送者不同，内容相同的消息视为不重复的",
                configuration.ChatHistoryLength.ToString(CultureInfo.CurrentCulture), System.StringComparison.Ordinal));

            var disableSelfChatHistory = configuration.DisableSelfChatHistory;
            if (ImGui.Checkbox("忽略自己发送的消息", ref disableSelfChatHistory))
            {
                configuration.DisableSelfChatHistory = disableSelfChatHistory;
                configuration.Save();
            }

            ImGuiComponents.HelpMarker("勾选后若一条消息是自己发送的则不过滤（也不会应用自定义过滤器）");

            var chatHistoryLength = configuration.ChatHistoryLength;
            if (chatHistoryLength < 1)
            {
                chatHistoryLength = 1;
                configuration.ChatHistoryLength = chatHistoryLength;
                configuration.Save();

            }
            ImGui.SetNextItemWidth(120f);
            if (ImGui.InputInt("缓存的消息数量", ref chatHistoryLength))
            {
                if (chatHistoryLength < 1)
                    chatHistoryLength = 1;
                if (configuration.ChatHistoryLength != chatHistoryLength)
                {
                    configuration.ChatHistoryLength = chatHistoryLength;
                    configuration.Save();
                }
            }

            ImGui.TextUnformatted("警告: 缓存数量过大可能会影响游戏性能，建议保持在50以下");

            var chatHistoryTimer = configuration.ChatHistoryTimer;
            ImGui.SetNextItemWidth(120f);
            if (ImGui.InputInt("每条消息的缓存时长", ref chatHistoryTimer))
            {
                configuration.ChatHistoryTimer = chatHistoryTimer;
                configuration.Save();
            }

            ImGuiComponents.HelpMarker("设置为0以禁用过期检查");

            ImGui.NewLine();

            var chatHistoryChannels = configuration.ChatHistoryChannels;
            ImGui.TextUnformatted("选择要过滤的消息类型");
            if (ImGui.CheckboxFlags("情感动作", ref chatHistoryChannels, 1 << 1))
            {
                configuration.ChatHistoryChannels = chatHistoryChannels;
                configuration.Save();
            }

            ImGui.SameLine(90f);
            if (ImGui.CheckboxFlags("战利品/掉落品", ref chatHistoryChannels, 1 << 5))
            {
                configuration.ChatHistoryChannels = chatHistoryChannels;
                configuration.Save();
            }

            if (ImGui.CheckboxFlags("制作", ref chatHistoryChannels, 1 << 8))
            {
                configuration.ChatHistoryChannels = chatHistoryChannels;
                configuration.Save();
            }

            ImGui.SameLine(90f);
            if (ImGui.CheckboxFlags("采集", ref chatHistoryChannels, 1 << 9))
            {
                configuration.ChatHistoryChannels = chatHistoryChannels;
                configuration.Save();
            }

            if (ImGui.CheckboxFlags("对话", ref chatHistoryChannels, 1 << 2))
            {
                configuration.ChatHistoryChannels = chatHistoryChannels;
                configuration.Save();
            }

            ImGui.SameLine(90f);
            if (ImGui.CheckboxFlags("部队成员上下线", ref chatHistoryChannels, 1 << 7))
                configuration.ChatHistoryChannels = chatHistoryChannels;
            {
                configuration.Save();
            }
            if (ImGui.CheckboxFlags("进度/成长", ref chatHistoryChannels, 1 << 4))
            {
                configuration.ChatHistoryChannels = chatHistoryChannels;
                configuration.Save();
            }

            ImGui.SameLine(90f);
            if (ImGui.CheckboxFlags("系统", ref chatHistoryChannels, 1 << 3))
            {
                configuration.ChatHistoryChannels = chatHistoryChannels;
                configuration.Save();
            }

            ImGui.EndTabItem();
        }
    }
}