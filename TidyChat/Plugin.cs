using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Dalamud.Game.Chat;
using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
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
    [PluginService] public static IObjectTable Objects { get; private set; } = null!;
    private Configuration Configuration { get; }
    private MainUI PluginUi { get; }
    private WindowSystem WindowSystem { get; }
    
    private const string SettingsCommand = TidyStrings.SettingsCommand;
    private const string ShorthandCommand = TidyStrings.ShorthandCommand;
    private ulong SessionBlockedMessages;
    private readonly List<(Rule, List<FilterAttribute>)> AllRules;
    private readonly List<(XivChatType LogKind, long Timestamp, string Message)> ChatHistory = [];
    private string LocalPlayerName;
    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        ChatGui.ChatMessage += OnChat;
        ClientState.TerritoryChanged += OnTerritoryChanged;
        ClientState.Login += OnLogin;
        ClientState.Logout += OnLogout;
        WindowSystem = new WindowSystem();
        PluginUi = new MainUI(Configuration);
        WindowSystem.AddWindow(PluginUi);
        CommandManager.AddHandler(SettingsCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = TidyStrings.SettingsHelper,
        });

        CommandManager.AddHandler(ShorthandCommand, new CommandInfo(OnCommand)
        {
            HelpMessage = TidyStrings.ShorthandHelper,
        });

        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        AllRules = [.. Enum.GetValues<Rule>().Select(r => (r, r.GetAttributes<FilterAttribute>().ToList())).Where(r => r.Item2.Count != 0)];
        Log.Information($"Loaded {AllRules.Count} rules");
    }
    private void ToggleConfigUi() => PluginUi.IsOpen ^= true;
    public void Dispose()
    {
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        CommandManager.RemoveHandler(SettingsCommand);
        CommandManager.RemoveHandler(ShorthandCommand);
        ChatGui.ChatMessage -= OnChat;
        ClientState.TerritoryChanged -= OnTerritoryChanged;
        ClientState.Login -= OnLogin;
        ClientState.Logout -= OnLogout;
    }

    private void OnLogin()
    {
        if (Configuration.BetterCommendationMessage)
            BetterCommendationsUpdate();
    }

    private void OnLogout(int type, int code)
    {
        BlockedCountUpdate();
    }

    private void OnTerritoryChanged(uint e)
    {
        BlockedCountUpdate();
        if (Configuration.BetterCommendationMessage)
            BetterCommendationsUpdate();
        if (Configuration.IncludeDutyNameInComms)
        {
            try
            {
                var territory =
                    DataManager.GetExcelSheet<Lumina.Excel.Sheets.TerritoryType>()!.GetRow(e); // built in sheets will never be null
                var exclusiveType = territory!.ExclusiveType;
                var isPvp = territory.IsPvpZone;

                var placeName = territory.PlaceName.Value.Name.ToString();
                var dutyName = territory.ContentFinderCondition.Value.Name.ToString();

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

    private void OnChat(IHandleableChatMessage message)
    {
        if (!Configuration.Enabled)
            return;

        var originalMessage = message.OriginalMessage.ToString();
        // Certain messages are improved with better messaging - these will change those messages to be Better Messages
        // Better Messages must always Early Return to avoid being filtered and because if we bettered the message we aren't filtering it anyway
        if (Configuration.BetterInstanceMessage && Rule.InstanceMessage.GetAttributes<FilterAttribute>().All(f => f.IsMatch(message.LogKind, originalMessage)))
        {
            message.Message = TidyStrings.Instances(Configuration);
            Log.Debug("Better.Instances handled.");
            return;
        }

        if (Configuration.BetterSayReminder && Rule.QuestSayReminder.GetAttributes<FilterAttribute>().All(f => f.IsMatch(message.LogKind, originalMessage)))
        {
            message.Message = TidyStrings.SayReminder(message.Message, Configuration);
            Log.Debug("Better.SayReminder handled.");
            return;
        }

        if (Configuration.BetterEmoteReminder && Rule.QuestEmoteReminder.GetAttributes<FilterAttribute>().All(f => f.IsMatch(message.LogKind, originalMessage)))
        {
            message.Message = TidyStrings.EmoteReminder(message.Message, Configuration);
            Log.Debug("Better.EmoteReminder handled.");
            return;
        }

        if (Configuration.BetterNoviceNetworkMessage && TidyStrings.NoviceNetworkJoinRegex.IsMatch(originalMessage))
        {
            message.Message = TidyStrings.NoviceNetworkJoin(Configuration);
            Log.Debug("Better.NoviceNetworkJoin handled.");
            return;
        }

        var isHandled = false;
        var ruleMatched = Rule.None;

        LocalPlayerName ??= Objects.LocalPlayer.Name.ToString();

        if (message.OriginalSender.ToString() != LocalPlayerName)
        {
            var normalizedText = TidyStrings.ReplaceLocalPlayerName(originalMessage, LocalPlayerName);
            foreach (var (rule, filters) in AllRules)
            {
                if (!Configuration.RuleActive.TryGetValue(rule.ToString(), out var enabled))
                {
                    Configuration.RuleActive[rule.ToString()] = enabled = false;
                }
                if (enabled && filters.All(f => f.IsMatch(message.LogKind, normalizedText)))
                {
                    ruleMatched = rule;
                    isHandled = true;
                    break;
                }
            }

            if (Configuration.AllowCustomFilter && Configuration.CustomFilters.Count > 0)
            {
                foreach (var filter in Configuration.CustomFilters)
                {
                    if (filter.Check(message, ref isHandled, ref ruleMatched))
                        break;
                }
            }

            if (!message.IsHandled && Configuration.ChatHistoryFilter && message.LogKind.IsChatKind())
            {
                if (Configuration.ChatHistoryTimer > 0)
                {
                    ChatHistory.RemoveAll(t => Environment.TickCount64 - t.Timestamp >= Configuration.ChatHistoryTimer * 1000);
                }
                var currentMessage = $"{message.Sender}: {message.Message}";
                if (ChatHistory.Any(t => t.LogKind == message.LogKind && t.Message == currentMessage))
                {
                    ruleMatched = Rule.Repeated;
                    isHandled = true;
                }
                else if (ChatHistory.Count > 0 && ChatHistory.Count >= Configuration.ChatHistoryLength)
                {
                    ChatHistory.RemoveAt(0);
                    ChatHistory.Add((message.LogKind, Environment.TickCount64, currentMessage));
                }
                else
                {
                    ChatHistory.Add((message.LogKind, Environment.TickCount64, currentMessage));
                }
            }
        }  

        if (isHandled && !Configuration.EnableDebugMode)
        {
            Log.Verbose($"Filtered message: {message} | Matched rule: {ruleMatched}");
            message.PreventOriginal();
            SessionBlockedMessages += 1;
            if (SessionBlockedMessages >= 100)
                BlockedCountUpdate();

            return;
        }

        if (Configuration.IncludeLogKind || Configuration.EnableDebugMode)
        {
            SeStringBuilder tags = new();
            if (Configuration.IncludeLogKind)
                TidyStrings.AddLogKindTag(tags, message.LogKind);
            if (Configuration.EnableDebugMode && ruleMatched != Rule.None)
            {
                TidyStrings.AddRuleTag(tags, ruleMatched);
                if (isHandled)
                    TidyStrings.AddBlockedTag(tags);
                else
                    TidyStrings.AddAllowedTag(tags);
            }
            if (message.Sender.Payloads.Count > 0)
            {
                message.Sender.Payloads.Insert(0, new TextPayload(tags.BuiltString.TextValue));
            }
            else
            {
                message.Message = tags.Append(message.Message).BuiltString;
            }
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

        if (commendationChange >= 1)
        {
            var stringBuilder = new SeStringBuilder();
            if (Configuration.IncludeChatTag) TidyStrings.AddTidyChatTag(stringBuilder);
            var dutyName = $"{(Configuration.IncludeDutyNameInComms && TidyStrings.LastDuty.Length > 0 ? " - " + TidyStrings.LastDuty : "")}";
            stringBuilder.AddText($"你得到了{commendationChange}个赞！-{dutyName}");
            ChatGui.Print(stringBuilder.BuiltString);
        }
    }

    private void OnCommand(string command, string argument)
    {
        var args = argument.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 0)
        {
            PluginUi.IsOpen ^= true;
        }
        else if (args.Length == 2)
        {
            var rule = Rule.None;
            if (!Enum.TryParse(args[0], out rule))
            {
                ChatGui.PrintError($"[TidyChat] Invalid RuleName {args[0]}");
                return;
            }
            var filters = rule.GetAttributes<FilterAttribute>().Select(f => f.GetRegexString()).Where(r => !string.IsNullOrEmpty(r)).ToList();
            if (filters.Count == 0)
            {
                ChatGui.Print($"[TidyChat] {args[0]} doesn't have any regex strings");
            }
            else if (args[1] == "regex")
            {
                ChatGui.Print($"[TidyChat] RegexString of Rule {rule}:");
                foreach(var reg in filters)
                {
                    ChatGui.Print(reg);
                }
            }
            else if (args[1] == "enable")
            {
                Configuration.RuleActive[rule.ToString()] = true;
            }
            else if (args[1] == "disable")
            {
                Configuration.RuleActive[rule.ToString()] = false;
            }
            else if (args[1] == "toggle")
            {
                Configuration.RuleActive[rule.ToString()] = !Configuration.RuleActive.GetValueOrDefault(rule.ToString(), false);
            }
            else
            {
                ChatGui.PrintError($"[TidyChat] Invalid subcommand: {args[1]}");
            }
        }
        else
        {
            ChatGui.PrintError($"[TidyChat] Invalid command");
        }
    }
}
