using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lumina.Excel.GeneratedSheets;
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

    private ChatType[]? Channels { get; }
    private Regex? Check { get; set; }
    private MessageFilterKind? MessageFilterType { get; }
    private object[]? Args { get; }
    private bool Initialized { get; set; } = false;
    public string RegexString { get; private set; } = null!;

    internal FilterAttribute(params ChatType[] channels)
    {
        Channels = channels;
        Initialized = true;
    }
    internal FilterAttribute(ChatType[] channels, string regex)
    {
        Channels = channels;
        RegexString = regex;
    }

    internal FilterAttribute(ChatType[] channels, MessageFilterKind messageFilterType, params object[] args)
    {
        Channels = channels;
        MessageFilterType = messageFilterType;
        Args = args;
    }

    internal FilterAttribute(PlayerFilterKind playerFilterType)
    {
        RegexString = GetRegexString(playerFilterType);
    }
    internal bool IsMatch(ChatType channel, string text)
    {
        if (!Initialized)
        {
            if (string.IsNullOrWhiteSpace(RegexString))
            {
                if (MessageFilterType == MessageFilterKind.LogMessage)
                {
                    RegexString = LogMessage();
                }
                else if (MessageFilterType == MessageFilterKind.Obtained)
                {
                    RegexString = Obtains();
                }
                if (string.IsNullOrWhiteSpace(RegexString))
                {
                    throw new ArgumentException($"Regex String is required! MessageFilterKind: {MessageFilterType}, {string.Join(",",Args!.Select(a => a.ToString()))}.");
                }
            }
            Check = new(RegexString, TidyStrings.RegexOptions, TidyStrings.RegexTimeout);
            Initialized = true;
        }
        return (Channels == null || Channels.Length == 0 || Channels.Contains(channel)) && (Check?.IsMatch(text) ?? true);
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
                var text = Plugin.DataManager.GetExcelSheet<LogMessage>()?.GetRow((uint)row)?.Text;
                if (text != null)
                {
                    string reg = "^";
                    foreach (var p in text.Payloads.Where(x => x is not null))
                    {
                        if (p.PayloadType == Lumina.Text.Payloads.PayloadType.Text && !string.IsNullOrEmpty(p.RawString))
                            reg += p.RawString;
                        else if (p.PayloadType == Lumina.Text.Payloads.PayloadType.NewLine)
                            reg += "\r?\n";
                        else
                            reg += ".*?";
                    }
                    regs.Add($"(?:{reg})");
                }
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
                regs.Add(Plugin.DataManager.GetExcelSheet<Item>()?.GetRow((uint)row)?.Name ?? "");
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
