using System.ComponentModel;

namespace TidyChat.Rules;
public enum SubCate
{
    None,
    [Description("雇员")]
    Retainer,
    [Description("投影/换装")]
    Glamour,
    [Description("部队/飞空艇/潜水艇")]
    FreeCompanyAndVoyage,
    [Description("区域")]
    Zone,
    [Description("副本/任务")]
    ContentsAndQuests,
    [Description("组队")]
    Party,
    [Description("交易")]
    Trading,
    [Description("深层迷宫")]
    DeepDungeon,
    [Description("战利品 & 掷骰")]
    LootingRolling,
    [Description("货币")]
    Currencies,
    [Description("分解")]
    Desynthesis,
    [Description("魔晶石")]
    Materia,
    [Description("制作")]
    Crafting,
    [Description("采集")]
    Gathering,
    [Description("钓鱼")]
    Fishing,
    [Description("狩猎")]
    Hunting,
    [Description("其他")]
    Other,
}
