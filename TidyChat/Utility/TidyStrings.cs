using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using TidyChat.Rules;
namespace TidyChat.Utility;

internal static partial class TidyStrings
{
    public static readonly RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline;
    public static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

    public const string SettingsCommand = "/tidychat";

    public const string ShorthandCommand = "/tidy";

    public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

    public static string PluginName { get; } = "Tidy Chat (CN)";

    public static string SettingsHelper { get; } = "打开设置";

    public static string ShorthandHelper { get; } = "打开设置";

    // The space at the end is intentional
    public static string Tag { get; } = "[TidyChat] ";

    public static string InstanceText { get; } = "当前副本区：";

    public static string CopiedToClipboard { get; } = "已被复制到剪贴板";

    public static string PvPDuty { get; } = "PvP任务";

    public static string StartQuotation { get; } = "“";

    public static string EndQuotation { get; } = "”";
    public static string ObtainText { get; } = "获得了";
    public static string YouText { get; } = "【你】";

    public static string LastDuty { get; set; } = "";

    public static short CommendationsEarned { get; set; } = 0;

    public static short LastCommendations { get; set; } = 0;

    public static readonly Regex NoviceNetworkJoinRegex = new(@"^加入了新人频道", RegexOptions, RegexTimeout);
    public static readonly string[] SayQuestReminderStrings = ["使用键盘或软键盘输入"];
    
    public static string ReplaceName(string normalizedInput, Configuration configuration)
    {
        var name = configuration.PlayerName.Trim();
        Regex IO = new(name.ToLower(CultureInfo.CurrentCulture), TidyStrings.RegexOptions, TidyStrings.RegexTimeout);
        normalizedInput = IO.Replace(normalizedInput, TidyStrings.YouText, (int)StringComparison.CurrentCultureIgnoreCase);
        return normalizedInput;
    }

    public static SeString SayReminder(SeString message, Configuration configuration)
    {
        // With the chat mode in Say, enter a phrase containing "Capture this"

        var containingPhraseStart = message.TextValue.LastIndexOf(TidyStrings.StartQuotation, StringComparison.Ordinal);
        var containingPhraseEnd = message.TextValue.LastIndexOf(TidyStrings.EndQuotation, StringComparison.Ordinal);
        var lengthOfPhrase = containingPhraseEnd - containingPhraseStart;
        var containingPhrase = message.TextValue.Substring(containingPhraseStart + 1, lengthOfPhrase - 1);
        if (configuration.CopyBetterSayReminder)
        {
            var stringBuilder = new SeStringBuilder();
            if (configuration.IncludeChatTag) AddTidyChatTag(stringBuilder);
            stringBuilder.AddText($"\"/say {containingPhrase}\" {TidyStrings.CopiedToClipboard}");
            ImGui.SetClipboardText($"/say {containingPhrase}");
            return stringBuilder.BuiltString;
        }
        return $"/say {containingPhrase}";
    }

    unsafe public static SeString Instances(SeString message, Configuration configuration)
    {
        try
        {
            // This will return the instance value: 0,1,2,3,4,5,6
            int InstanceNumberFromSignature = (int)UIState.Instance()->PublicInstance.InstanceId;
            var instanceCharacter = ((char)(SeIconChar.Instance1 + (byte)(InstanceNumberFromSignature - 1))).ToString();
            var stringBuilder = new SeStringBuilder();
            if (configuration.IncludeChatTag) AddTidyChatTag(stringBuilder);
            stringBuilder.AddText($"{TidyStrings.InstanceText} {instanceCharacter}");
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
        sestring.AddText(TidyStrings.Tag);
        sestring.AddUiForegroundOff();
        return sestring;
    }

    /// <summary>
    ///     This method takes <paramref name="sestring" /> and adds a yellow "[Channel] " tag text to it
    /// </summary>
    /// <param name="sestring">An empty SeStringBuilder()</param>
    /// <returns>SeString with text: "[Channel] "</returns>
    public static SeStringBuilder AddChannelTag(SeStringBuilder sestring, ChatType channel)
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
        sestring.AddText("[Blocked] ");
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
        sestring.AddText($"[Allowed] ");
        sestring.AddUiForegroundOff();
        return sestring;
    }
}