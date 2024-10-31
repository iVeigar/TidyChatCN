using System.ComponentModel;

namespace TidyChat.Rules;

public enum Cate
{
    [Description("系统")]
    System,

    [Description("战利品/掉落品")]
    LootObtain,

    [Description("进度/成长")]
    Progress,

    [Description("制作/采集")]
    CraftingGathering,

    [Description("情感动作")]
    Emote,
}