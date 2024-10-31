using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace TidyChat;

[Serializable]
public class Configuration : IPluginConfiguration
{
    // the below exist just to make saving less cumbersome
    [NonSerialized] private IDalamudPluginInterface? pluginInterface;

    public ulong TtlMessagesBlocked { get; set; } = 0;
    public bool Enabled { get; set; } = true;

    #region Improved Messaging
    public bool IncludeChatTag { get; set; } = true;
    public bool BetterInstanceMessage { get; set; } = true;
    public bool BetterCommendationMessage { get; set; } = true;
    public bool IncludeDutyNameInComms { get; set; } = true;
    public bool BetterSayReminder { get; set; } = true;
    public bool CopyBetterSayReminder { get; set; } = true;
    public bool BetterNoviceNetworkMessage { get; set; } = true;
    #endregion

    #region Debug
    public bool EnableDebugMode { get; set; } = false;
    public bool IncludeChannel { get; set; } = false;
    #endregion

    public string PlayerName { get; set; } = "";
    public IList<CustomFilter> CustomFilters { get; set; } = [];
    public bool AllowCustomFilter { get; set; } = true;
    public bool ChatHistoryFilter { get; set; } = false;
    public int ChatHistoryChannels { get; set; } = 2;
    public int ChatHistoryLength { get; set; } = 10;
    public int ChatHistoryTimer { get; set; } = 10;
    public bool DisableSelfChatHistory { get; set; } = true;
    public int Version { get; set; } = 1;

    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public T? GetPropertyValue<T>(object obj, string propName)
    {
        return (T?)obj?.GetType()?.GetProperty(propName)?.GetValue(this, index: null);
    }

    public void Save()
    {
        pluginInterface!.SavePluginConfig(this);
    }



    public Dictionary<string, bool> RuleActive { get; set; } = [];
}
