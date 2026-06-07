using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Serilog;
using TidyChat.Rules;

namespace TidyChat;

public class CustomFilter
{
    public bool IsAllowed = true;
    public string MessageOrPlayerName = string.Empty;
    //public string ServerName = string.Empty;
    public XivChatType SelectedChatType = XivChatType.None;

    public bool Check(IHandleableChatMessage message, ref bool isHandled, ref Rule ruleMatched)
    {
        if (SelectedChatType != message.LogKind)
            return false;
        var re = new Regex(MessageOrPlayerName, RegexOptions.None);
        if (string.Equals(message.OriginalSender.ToString(), MessageOrPlayerName, StringComparison.Ordinal)
            || re.IsMatch(message.OriginalMessage.ToString()))
        {
            isHandled = !IsAllowed;
            ruleMatched = IsAllowed ? Rule.CustomFilterAllowed : Rule.CustomFilterBlocked;
            Log.Verbose($"[TidyChatCN] CustomFilter {(IsAllowed ? "allowed" : "blocked")} a message, matched {MessageOrPlayerName}");
        }
        return true;
    }
}