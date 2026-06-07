using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dalamud.Game.Text;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using TidyChat.Rules;
using TidyStrings = TidyChat.Utility.TidyStrings;
namespace TidyChat.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
internal class FilterAttribute : Attribute
{
    internal static string GetRegexString(PlayerFilterKind rule)
    {
        return rule switch
        {
            PlayerFilterKind.NotContainYou => $"^(?!.*{TidyStrings.YouText})",
            PlayerFilterKind.ContainsYou => TidyStrings.YouText,
            PlayerFilterKind.PlayerObtained => @"^[^\n]+?" + TidyStrings.ObtainText,
            _ => ""
        };
    }

    private XivChatType[]? LogKinds { get; }
    private Regex? Check { get; set; }
    private MessageFilterKind? MessageFilterType { get; }
    private object[]? Args { get; }
    public string RegexString { get; private set; } = null!;

    internal FilterAttribute(params XivChatType[] logkinds)
    {
        LogKinds = logkinds;
    }

    internal FilterAttribute(string regex)
    {
        RegexString = regex;
    }

    internal FilterAttribute(MessageFilterKind messageFilterType, params object[] args)
    {
        MessageFilterType = messageFilterType;
        Args = args;
    }

    internal FilterAttribute(PlayerFilterKind playerFilterType)
    {
        RegexString = GetRegexString(playerFilterType);
    }

    internal bool IsMatch(XivChatType logkind, string normalizedText)
    {
        if (LogKinds != null)
            return LogKinds.Contains(logkind);

        if (RegexString == null)
        {
            if (MessageFilterType == MessageFilterKind.LogMessage)
            {
                RegexString = LogMessage();
            }
            else if (MessageFilterType == MessageFilterKind.Obtained)
            {
                RegexString = Obtains();
            }
            if (RegexString == null)
            {
                throw new ArgumentException($"Regex String is required! MessageFilterKind: {MessageFilterType}, {string.Join(",", Args!.Select(a => a.ToString()))}.");
            }
        }
        Check = new(RegexString, TidyStrings.RegexOptions);
        return Check.IsMatch(normalizedText);
    }

    public string GetRegexString()
    {
        if (LogKinds != null)
            return string.Empty;

        if (RegexString == null)
        {
            if (MessageFilterType == MessageFilterKind.LogMessage)
            {
                RegexString = LogMessage();
            }
            else if (MessageFilterType == MessageFilterKind.Obtained)
            {
                RegexString = Obtains();
            }
            if (RegexString == null)
            {
                throw new ArgumentException($"Regex String is required! MessageFilterKind: {MessageFilterType}, {string.Join(",", Args!.Select(a => a.ToString()))}.");
            }
        }
        return RegexString;
    }

    private string LogMessage()
    {
        if (Args == null)
            return string.Empty;
        List<string> regs = [];
        foreach (var arg in Args)
        {
            if (arg is int row)
            {
                var text = Plugin.DataManager.GetExcelSheet<LogMessage>().GetRow((uint)row).Text;
                string reg = "^";
                foreach (var p in text)
                {
                    if (p.Type == ReadOnlySePayloadType.Text)
                        reg += p.ToString();
                    else if (p.MacroCode == Lumina.Text.Payloads.MacroCode.NewLine)
                        reg += "\r?\n";
                    else
                        reg += ".*?";
                }
                regs.Add($"(?:{reg})");
            }
            else if (arg is string reg)
                regs.Add(reg);
            else
                continue;

        }
        return string.Join('|', regs);
    }

    private string Obtains()
    {
        if (Args == null)
            return string.Empty;
        List<string> regs = [];
        foreach (var arg in Args)
        {
            if (arg is int row)
                regs.Add(Plugin.DataManager.GetExcelSheet<Item>().GetRow((uint)row).Name.ToString());
            else if (arg is string reg)
                regs.Add(reg);
            else
                continue;
        }
        string itemsString = string.Join('|', regs.Where(reg => !string.IsNullOrWhiteSpace(reg)));
        if (itemsString.Contains('|', StringComparison.OrdinalIgnoreCase))
        {
            itemsString = $"(?:{itemsString})";
        }
        itemsString = string.IsNullOrWhiteSpace(itemsString) ? ".+" : $".*?{itemsString}";
        return $"^[^\n]*?{TidyStrings.ObtainText}{itemsString}";
    }
}
