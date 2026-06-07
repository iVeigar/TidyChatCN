using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Animation;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using TidyChat.Rules;
using static System.Net.Mime.MediaTypeNames;
using static FFXIVClientStructs.FFXIV.Client.UI.Agent.AgentAlarm;
namespace TidyChat.Utility;

internal static partial class TidyStrings
{
    public static readonly RegexOptions RegexOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline;

    public const string SettingsCommand = "/tidychat";

    public const string ShorthandCommand = "/tidy";

    public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

    public static string PluginName { get; } = "Tidy Chat (CN)";

    public static string SettingsHelper { get; } = "打开设置";

    public static string ShorthandHelper { get; } = "打开设置";

    // The space at the end is intentional
    public static string TagTidyChat { get; } = "[TidyChat] ";
    public static string TagAllowed { get; } = "[Allowed] ";
    public static string TagBlocked { get; } = "[Blocked] ";

    public static string InstanceText { get; } = "当前副本区：";

    public static string CopiedToClipboard { get; } = "已被复制到剪贴板";

    public static string PvPDuty { get; } = "PvP任务";

    public static string StartQuotation { get; } = "“";

    public static string EndQuotation { get; } = "”";
    public static string ObtainText { get; } = "获得了";
    public static string YouText { get; } = "【你】";

    public static string LastDuty { get; set; } = "";

    public static short CommendationsEarned { get; set; }

    public static short LastCommendations { get; set; }

    public static readonly Regex NoviceNetworkJoinRegex = new(@"^加入了新人频道", RegexOptions);
    public static readonly string[] SayQuestReminderStrings = ["使用键盘或软键盘输入"];
    
    public static string ReplaceLocalPlayerName(string text, string name)
        => text.Replace(name, YouText);

    public static SeString SayReminder(SeString message, Configuration configuration)
    {
        // With the chat mode in Say, enter a phrase containing "Capture this"

        var containingPhraseStart = message.TextValue.LastIndexOf(StartQuotation, StringComparison.Ordinal);
        var containingPhraseEnd = message.TextValue.LastIndexOf(EndQuotation, StringComparison.Ordinal);
        var lengthOfPhrase = containingPhraseEnd - containingPhraseStart - 1;
        var containingPhrase = message.TextValue.Substring(containingPhraseStart + 1, lengthOfPhrase);
        var command = $"/say {containingPhrase}";
        if (configuration.CopyBetterSayReminder)
        {
            var stringBuilder = new SeStringBuilder();
            if (configuration.IncludeChatTag) AddTidyChatTag(stringBuilder);
            stringBuilder.AddText($"\"{command}\" {CopiedToClipboard}");
            ImGui.SetClipboardText(command);
            return stringBuilder.BuiltString;
        }
        return command;
    }

    public static SeString EmoteReminder(SeString message, Configuration configuration)
    {
        var containingPhraseStart = message.TextValue.LastIndexOf(StartQuotation, StringComparison.Ordinal);
        var containingPhraseEnd = message.TextValue.LastIndexOf(EndQuotation, StringComparison.Ordinal);
        var lengthOfPhrase = containingPhraseEnd - containingPhraseStart - 1;
        var containingPhrase = message.TextValue.Substring(containingPhraseStart + 1, lengthOfPhrase);
        var command = $"/{containingPhrase}";
        if (configuration.CopyBetterEmoteReminder)
        {
            var stringBuilder = new SeStringBuilder();
            if (configuration.IncludeChatTag) AddTidyChatTag(stringBuilder);
            stringBuilder.AddText($"\"{command}\" {CopiedToClipboard}");
            ImGui.SetClipboardText(command);
            return stringBuilder.BuiltString;
        }
        return command;
    }

    unsafe public static SeString Instances(Configuration configuration)
    {
        try
        {
            // This will return the instance value: 0,1,2,3,4,5,6
            int InstanceNumberFromSignature = (int)UIState.Instance()->PublicInstance.InstanceId;
            var instanceCharacter = ((char)(SeIconChar.Instance1 + (byte)(InstanceNumberFromSignature - 1))).ToString();
            var stringBuilder = new SeStringBuilder();
            if (configuration.IncludeChatTag) AddTidyChatTag(stringBuilder);
            stringBuilder.AddText($"{InstanceText} {instanceCharacter}");
            return stringBuilder.BuiltString;
        }
        catch
        {
            // Nah
        }
        return "";
    }

    public static SeString NoviceNetworkJoin(Configuration configuration)
    {
        var stringBuilder = new SeStringBuilder();
        if (configuration.IncludeChatTag) AddTidyChatTag(stringBuilder);
        stringBuilder.AddText("你加入了新频。");
        return stringBuilder.BuiltString;
    }

    /// <summary>
    ///     This method takes <paramref name="sestring" /> and adds a red "[TidyChat] " tag text to it
    /// </summary>
    /// <param name="sestring">An empty SeStringBuilder()</param>
    /// <returns>SeString with text: "[TidyChat] "</returns>
    public static SeStringBuilder AddTidyChatTag(SeStringBuilder sestring)
    {
        sestring.AddUiForeground(14);
        sestring.AddText(TagTidyChat);
        sestring.AddUiForegroundOff();
        return sestring;
    }

    /// <summary>
    ///     This method takes <paramref name="sestring" /> and adds a yellow "[Channel] " tag text to it
    /// </summary>
    /// <param name="sestring">An empty SeStringBuilder()</param>
    /// <returns>SeString with text: "[Channel] "</returns>
    public static SeStringBuilder AddLogKindTag(SeStringBuilder sestring, XivChatType channel)
    {
        sestring.AddUiForeground(8);
        sestring.AddText($"[{channel}] ");
        sestring.AddUiForegroundOff();
        return sestring;
    }

    /// <summary>
    ///     This method takes <paramref name="sestring" /> and adds a purple "[Rule] " tag text to it
    /// </summary>
    /// <param name="sestring">An empty SeStringBuilder()</param>
    /// <returns>SeString with text: "[Rule] "</returns>
    public static SeStringBuilder AddRuleTag(SeStringBuilder sestring, Rule ruleMatched)
    {
        sestring.AddUiForeground(9);
        sestring.AddText($"[{ruleMatched}] ");
        sestring.AddUiForegroundOff();
        return sestring;
    }

    /// <summary>
    ///     This method takes <paramref name="sestring" /> and adds a red "[BLOCKED] " tag text to it
    /// </summary>
    /// <param name="sestring">An empty SeStringBuilder()</param>
    /// <returns>SeString with text: "[Rule] "</returns>
    public static SeStringBuilder AddBlockedTag(SeStringBuilder sestring)
    {
        sestring.AddUiForeground(8);
        sestring.AddText(TagBlocked);
        sestring.AddUiForegroundOff();
        return sestring;
    }

    /// <summary>
    ///     This method takes <paramref name="sestring" /> and adds a purple "[ALLOWED] " tag text to it
    /// </summary>
    /// <param name="sestring">An empty SeStringBuilder()</param>
    /// <returns>SeString with text: "[Rule] "</returns>
    public static SeStringBuilder AddAllowedTag(SeStringBuilder sestring)
    {
        sestring.AddUiForeground(9);
        sestring.AddText(TagAllowed);
        sestring.AddUiForegroundOff();
        return sestring;
    }

    public static bool IsChatKind(this XivChatType chatType)
    {
        return chatType == XivChatType.Say
            || chatType == XivChatType.Shout
            || chatType == XivChatType.TellOutgoing
            || chatType == XivChatType.TellIncoming
            || chatType == XivChatType.Party
            || chatType == XivChatType.Alliance
            || chatType == XivChatType.Ls1
            || chatType == XivChatType.Ls2
            || chatType == XivChatType.Ls3
            || chatType == XivChatType.Ls4
            || chatType == XivChatType.Ls5
            || chatType == XivChatType.Ls6
            || chatType == XivChatType.Ls7
            || chatType == XivChatType.Ls8
            || chatType == XivChatType.FreeCompany
            || chatType == XivChatType.NoviceNetwork
            || chatType == XivChatType.CustomEmote
            || chatType == XivChatType.StandardEmote
            || chatType == XivChatType.Yell
            || chatType == XivChatType.CrossParty
            || chatType == XivChatType.PvPTeam
            || chatType == XivChatType.CrossLinkShell1
            || chatType == XivChatType.GmTell
            || chatType == XivChatType.GmSay
            || chatType == XivChatType.GmShout
            || chatType == XivChatType.GmYell
            || chatType == XivChatType.GmParty
            || chatType == XivChatType.GmFreeCompany
            || chatType == XivChatType.GmLinkshell1
            || chatType == XivChatType.GmLinkshell2
            || chatType == XivChatType.GmLinkshell3
            || chatType == XivChatType.GmLinkshell4
            || chatType == XivChatType.GmLinkshell5
            || chatType == XivChatType.GmLinkshell6
            || chatType == XivChatType.GmLinkshell7
            || chatType == XivChatType.GmLinkshell8
            || chatType == XivChatType.GmNoviceNetwork
            || chatType == XivChatType.CrossLinkShell2
            || chatType == XivChatType.CrossLinkShell3
            || chatType == XivChatType.CrossLinkShell4
            || chatType == XivChatType.CrossLinkShell5
            || chatType == XivChatType.CrossLinkShell6
            || chatType == XivChatType.CrossLinkShell7
            || chatType == XivChatType.CrossLinkShell8;
    }
}