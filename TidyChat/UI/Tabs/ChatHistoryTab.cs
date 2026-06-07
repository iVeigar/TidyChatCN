using System.Globalization;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Components;

namespace TidyChat.Settings.Tabs;

internal static class ChatHistoryTab
{
    public static void Draw(Configuration configuration)
    {
        if (ImGui.BeginTabItem("聊天历史"))
        {
            var chatHistoryFilter = configuration.ChatHistoryFilter;
            if (ImGui.Checkbox("过滤重复的聊天消息", ref chatHistoryFilter))
            {
                configuration.ChatHistoryFilter = chatHistoryFilter;
                configuration.Save();
            }

            ImGuiComponents.HelpMarker(string.Format(CultureInfo.CurrentCulture, "如果某条消息在最近{0}条消息中重复出现则将被过滤。发送者不同，内容相同的消息视为不重复的",
                configuration.ChatHistoryLength.ToString(CultureInfo.CurrentCulture), System.StringComparison.Ordinal));

            var chatHistoryLength = configuration.ChatHistoryLength;
            if (chatHistoryLength < 1)
            {
                chatHistoryLength = 1;
                configuration.ChatHistoryLength = chatHistoryLength;
                configuration.Save();

            }
            ImGui.SetNextItemWidth(120f);
            if (ImGui.InputInt("独特的消息缓存数量", ref chatHistoryLength))
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
            if (ImGui.InputInt("每条消息的缓存时长（秒）", ref chatHistoryTimer))
            {
                configuration.ChatHistoryTimer = chatHistoryTimer;
                configuration.Save();
            }

            ImGuiComponents.HelpMarker("设置为0以禁用过期检查");
        }
    }
}