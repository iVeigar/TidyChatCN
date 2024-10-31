using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using TidyChat.Attributes;
using TidyChat.Rules;
using TidyChat.Utility;

namespace TidyChat;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] public static IPluginLog Log { get; private set; } = null!;

    private Configuration Configuration { get; }
    private MainUI PluginUi { get; }

    private const string SettingsCommand = TidyStrings.SettingsCommand;
    private const string ShorthandCommand = TidyStrings.ShorthandCommand;
    private ulong SessionBlockedMessages;
    private readonly List<(Rule, List<FilterAttribute>)> AllRules;
    private readonly List<(string, long)> ChatHistory = [];

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        ChatGui.CheckMessageHandled += OnChat;
        ClientState.TerritoryChanged += OnTerritoryChanged;
        ClientState.Login += OnLogin;
        ClientState.Logout += OnLogout;

        PluginUi = new MainUI(Configuration);

        CommandManager.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = TidyStrings.SettingsHelper,
        });

        CommandManager.AddHandler(ShorthandCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = TidyStrings.ShorthandHelper,
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenMainUi += DrawConfigUI;
        PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

        AllRules = Enum.GetValues<Rule>().Select(r => (r, r.GetAttributes<FilterAttribute>().ToList())).Where(r => r.Item2.Count != 0).ToList();
        Log.Information($"Loaded {AllRules.Count} rules");
    }

    public void Dispose()
    {
        PluginUi.Dispose();
        CommandManager.RemoveHandler(SettingsCommand);
        CommandManager.RemoveHandler(ShorthandCommand);
        ChatGui.CheckMessageHandled -= OnChat;
        ClientState.TerritoryChanged -= OnTerritoryChanged;
        ClientState.Login -= OnLogin;
        ClientState.Logout -= OnLogout;
    }
    private void OnLogin()
    {
        if (Configuration.BetterCommendationMessage)
            BetterCommendationsUpdate();
        SetPlayerName();
    }

    private void OnLogout()
    {
        BlockedCountUpdate();
    }

    private void OnTerritoryChanged(ushort e)
    {
        BlockedCountUpdate();
        if (Configuration.BetterCommendationMessage)
            BetterCommendationsUpdate();
        if (Configuration.IncludeDutyNameInComms)
        {
            try
            {
                var territory =
                    DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.TerritoryType>()!.GetRow(e); // built in sheets will never be null
                var exclusiveType = territory!.ExclusiveType;
                var isPvp = territory.IsPvpZone;

                var placeName = territory.PlaceName.Value?.Name.ToString();
                var dutyName = territory.ContentFinderCondition.Value?.Name.ToString();

                TidyStrings.LastDuty = exclusiveType switch
                {
                    2 when dutyName?.Length >= 1 => dutyName,
                    2 when dutyName?.Length == 0 && placeName?.Length > 0 => placeName,
                    2 when dutyName?.Length == 0 && isPvp => TidyStrings.PvPDuty,
                    _ => TidyStrings.LastDuty // Keep previous value if we don't care about the new value
                };
            }
            catch (KeyNotFoundException)
            {
                Log.Warning(
                    "Something somehow somewhere went wrong but we don't want to crash on territory change");
            }
        }
    }

    private void OnChat(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (!Configuration.Enabled)
            return;

        var chatType = FromDalamud(type);
        var normalizedText = message.TextValue;

        // If we have the player's name, normalize any messages containing the player's name or initials to read "you" instead of the player's name
        if (Configuration.PlayerName != "")
            normalizedText = TidyStrings.ReplaceName(normalizedText, Configuration);

        // Certain messages are improved with better messaging - these will change those messages to be Better Messages
        // Better Messages must always Early Return to avoid being filtered and because if we bettered the message we aren't filtering it anyway
        if (Configuration.BetterInstanceMessage && Rule.InstanceMessage.GetAttribute<FilterAttribute>()!.IsMatch(chatType, normalizedText))
        {
            message = TidyStrings.Instances(message, Configuration);
            Log.Debug("Better.Instances handled.");
            return;
        }

        if (Configuration.BetterSayReminder && Rule.QuestReminder.GetAttribute<FilterAttribute>()!.IsMatch(chatType, normalizedText))
        {
            message = TidyStrings.SayReminder(message, Configuration);
            Log.Debug("Better.SayReminder handled.");
            return;
        }

        if (Configuration.BetterNoviceNetworkMessage && TidyStrings.NoviceNetworkJoinRegex.IsMatch(normalizedText))
        {
            message = TidyStrings.NoviceNetworkJoin(Configuration);
            Log.Debug("Better.NoviceNetworkJoin handled.");
            return;
        }

        Rule ruleMatched = Rule.None;
        foreach (var (rule, filters) in AllRules)
        {
            if (!Configuration.RuleActive.TryGetValue(rule.ToString(), out var enabled))
            {
                Configuration.RuleActive[rule.ToString()] = enabled = false;
            }
            if (filters.All(f => f.IsMatch(chatType, normalizedText)))
            {
                ruleMatched = rule;
                isHandled = enabled;
                break;
            }
        }

        if (Configuration.CustomFilters.Count > 0)
            foreach (var playerOrMessage in Configuration.CustomFilters)
                CustomFilterCheck(sender, message, ref ruleMatched, ref isHandled, playerOrMessage, chatType);

        if (Configuration.ChatHistoryFilter && !isHandled)
        {
            /* Disable Chat History for self-sent messages by default */
            if (!Configuration.DisableSelfChatHistory || !string.Equals(sender.TextValue, Configuration.PlayerName, StringComparison.Ordinal))
            {
                var historyChannels = (ChatFlags.Channels)Configuration.ChatHistoryChannels;
                if (!historyChannels.Equals(ChatFlags.Channels.None) && ChatFlags.CheckFlags(Configuration, chatType))
                {
                    if (Configuration.ChatHistoryTimer > 0)
                    {
                        ChatHistory.RemoveAll(t => Environment.TickCount64 - t.Item2 >= Configuration.ChatHistoryTimer * 1000);
                    }
                    var currentMessage = $"{sender.TextValue}: {message.TextValue}";
                    if (ChatHistory.Any(t => t.Item1 == currentMessage))
                    {
                        ruleMatched = Rule.Repeated;
                        isHandled = true;
                    }
                    else if (ChatHistory.Count > 0 && ChatHistory.Count >= Configuration.ChatHistoryLength)
                    {
                        ChatHistory.RemoveAt(0);
                        ChatHistory.Add((currentMessage, Environment.TickCount64));
                    }
                    else
                    {
                        ChatHistory.Add((currentMessage, Environment.TickCount64));
                    }
                }
            }
        }

        if (Configuration.IncludeChannel
            || Configuration.EnableDebugMode)
        {
            SeStringBuilder tags = new();
            if (Configuration.IncludeChannel)
                TidyStrings.AddChannelTag(tags, chatType);
            if (Configuration.IncludeChatTag)
                TidyStrings.AddTidyChatTag(tags);
            if (Configuration.EnableDebugMode && ruleMatched != Rule.None)
            {
                TidyStrings.AddRuleTag(tags, ruleMatched);
                if (isHandled)
                    TidyStrings.AddBlockedTag(tags);
                else
                    TidyStrings.AddAllowedTag(tags);
            }
            if (sender.Payloads.Count > 0)
            {
                sender.Payloads.Insert(0, new TextPayload(tags.BuiltString.TextValue));
            }
            else
            {
                message = tags.Append(message).BuiltString;
            }
        }
        if (!Configuration.EnableDebugMode && isHandled)
        {
            Log.Debug($"Filtered message: {message} | Matched rule: {ruleMatched}");
            SessionBlockedMessages += 1;
        }
    }

    private void CustomFilterCheck(SeString sender, SeString message, ref Rule ruleMatched, ref bool isHandled,
        CustomFilter customFilter,
        ChatType chatType)
    {
        if (isHandled == customFilter.IsAllowed)
        {
            var e = (ChatFlags.Channels)customFilter.whitelistedChannels;
            var isRegex = false;
            Regex? msgPattern = null;
            if (customFilter.MessageOrPlayerName.StartsWith('/') && customFilter.MessageOrPlayerName.EndsWith('/'))
            {
                isRegex = true;
                msgPattern =
                    new Regex(customFilter.MessageOrPlayerName[1..^1], RegexOptions.None, TimeSpan.FromSeconds(1));
            }

            var channelSelectedToFilter = false;
            if (!e.Equals(ChatFlags.Channels.None))
                channelSelectedToFilter = ChatFlags.CheckFlags(customFilter, chatType);
            if (channelSelectedToFilter)
            {
                if (string.Equals(sender.TextValue, customFilter.MessageOrPlayerName, StringComparison.Ordinal))
                {
                    isHandled = !isHandled;
                    ruleMatched = isHandled ? Rule.CustomFilterBlocked : Rule.CustomFilterAllowed;
                    Log.Verbose($"A message from {customFilter.MessageOrPlayerName} has been {(isHandled ? "blocked" : "allowed")}.");
                }
                else if (!isRegex && message.TextValue.Contains(customFilter.MessageOrPlayerName, StringComparison.Ordinal))
                {
                    isHandled = !isHandled;
                    ruleMatched = isHandled ? Rule.CustomFilterBlocked : Rule.CustomFilterAllowed;
                    Log.Verbose($"A message matching \"{customFilter.MessageOrPlayerName}\" has been {(isHandled ? "blocked" : "allowed")}.");
                }
                else if (isRegex && msgPattern!.IsMatch(message.ToString()))
                {
                    isHandled = !isHandled;
                    ruleMatched = isHandled ? Rule.CustomFilterBlocked : Rule.CustomFilterAllowed;
                    Log.Verbose($"A message matching the regex \"{customFilter.MessageOrPlayerName}\" has been {(isHandled ? "blocked" : "allowed")}.");
                }
            }
        }
    }

    private void SetPlayerName()
    {
        try
        {
            if (ClientState.LocalPlayer == null) return;

            Configuration.PlayerName = $"{ClientState.LocalPlayer.Name}";
            Log.Information($"Player name saved as {ClientState.LocalPlayer.Name}");
            Configuration.Save();
        }
        catch (Exception ex)
        {
            Log.Error("Error: Failed to capture player's name - trying again in 30 seconds" + ex);
            var t = new Timer
            {
                Interval = 30000,
                AutoReset = false,
            };
            t.Elapsed += delegate
            {
                t.Enabled = false;
                t.Dispose();
                SetPlayerName();
            };
            t.Enabled = true;
        }
    }

    private void BlockedCountUpdate()
    {
        Configuration.TtlMessagesBlocked += SessionBlockedMessages;
        SessionBlockedMessages = 0;
        Configuration.Save();
    }

    private unsafe void BetterCommendationsUpdate()
    {
        try
        {
            var player = PlayerState.Instance();
            if (player == null)
            {
                Log.Error("PlayerState was null, something went wrong");
                return;
            }

            TidyStrings.CommendationsEarned = player->PlayerCommendations;
        }
        catch (Exception ex)
        {
            Log.Error("Failed to improve Commendations message", ex);
        }

        var commendationChange = TidyStrings.CommendationsEarned - TidyStrings.LastCommendations;
        TidyStrings.LastCommendations = TidyStrings.CommendationsEarned;

        if (commendationChange is >= 1 and <= 7)
        {
            var stringBuilder = new SeStringBuilder();
            if (Configuration.IncludeChatTag) TidyStrings.AddTidyChatTag(stringBuilder);
            var dutyName = $"{(Configuration.IncludeDutyNameInComms && TidyStrings.LastDuty.Length > 0 ? " - " + TidyStrings.LastDuty : "")}";
            stringBuilder.AddText($"你得到了{commendationChange}个赞！{dutyName}");
            ChatGui.Print(stringBuilder.BuiltString);
        }
    }

    private void OnCommand(string command, string argument)
    {
        var args = argument.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 0)
        {
            PluginUi.SettingsVisible ^= true;
        }
        else if (args.Length == 2)
        {
            var rule = Rule.None;
            if (!Enum.TryParse(args[1], out rule))
            {
                ChatGui.PrintError($"[TidyChat] Invalid RuleName {args[1]}");
                return;
            }
            var filters = rule.GetAttributes<FilterAttribute>().Select(f => f.RegexString).ToList();
            if (filters.Count == 0)
            {
                ChatGui.PrintError($"[TidyChat] Invalid RuleName {args[1]}");
            }
            else if (args[0] == "regex")
            {
                ChatGui.Print($"[TidyChat] Regex of Rule {rule}:");
                foreach(var reg in filters.Where(r => !string.IsNullOrWhiteSpace(r)))
                {
                    ChatGui.Print(reg);
                }
            }
            else if (args[0] == "enable")
            {
                Configuration.RuleActive[rule.ToString()] = true;
            }
            else if (args[0] == "disable")
            {
                Configuration.RuleActive[rule.ToString()] = false;
            }
            else if (args[0] == "toggle")
            {
                Configuration.RuleActive[rule.ToString()] = !Configuration.RuleActive.GetValueOrDefault(rule.ToString(), false);
            }
            else
            {
                ChatGui.PrintError($"[TidyChat] Invalid subcommand: {args[0]}");
            }
        }
        else
        {
            ChatGui.PrintError($"[TidyChat] Invalid command");
        }
    }

    private void DrawUI() => PluginUi.Draw();

    private void DrawConfigUI() => PluginUi.SettingsVisible = true;

    // Stole this region from Anna's Chat2: https://git.annaclemens.io/ascclemens/ChatTwo/src/branch/main/ChatTwo
    private static ChatType FromDalamud(XivChatType type)
    {
        return (ChatType)((ushort)type & 0x7f);
    }
}
