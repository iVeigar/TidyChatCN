using TidyChat.Attributes;
namespace TidyChat.Rules;

public enum Rule
{
    None,
    Repeated, // used by chatHistory
    CustomFilterAllowed,
    CustomFilterBlocked,

    [Cate(Cate.System, SubCate.Zone)]
    [RuleInfo("隐藏当前所在副本区为“xxxx ”...")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1350)]
    InstanceMessage,

    [Cate(Cate.System, SubCate.Hunting)]
    [RuleInfo("隐藏怪物通缉令完成进度提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4411)]
    HuntSlain,

    [Cate(Cate.System, SubCate.Hunting)]
    [RuleInfo("隐藏S怪出现提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 9331)]
    SRankMark,

    [Cate(Cate.System, SubCate.Hunting)]
    [RuleInfo("隐藏SS怪出现提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 9332)]
    SSRankMark,

    [Cate(Cate.System, SubCate.Hunting)]
    [RuleInfo("隐藏恶名精英讨伐成功，获得了相应的报酬")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4442)]
    SlayedTheMark,
    
    [Cate(Cate.System, SubCate.Retainer)]
    [RuleInfo("隐藏雇员<名字>成功完成了探险")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4333, 4341)]
    CompletedVenture,

    [Cate(Cate.System, SubCate.Retainer)]
    [RuleInfo("隐藏开始/停止在xx市场出售物品")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 398, 399)]
    StartStopSelling,

    [Cate(Cate.System, SubCate.Retainer)]
    [RuleInfo("隐藏交给雇员x枚探险币")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4334)]
    PaidToRetainer,
    
    [Cate(Cate.System, SubCate.Retainer)]
    [RuleInfo("隐藏市场中出售物品的货款已经汇入雇员的账户", "汇入金额超过雇员所持金额上限时消息不会被过滤")]
    [Filter([ChatType.System], "市场中出售物品的货款已经汇入雇员的账户。$")]
    GilEarnedEntrusted,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏获得了最优队员推荐")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 926)]
    Commendations,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏将聊天频道设置为“说话”频道后，使用键盘或软键盘输入暗号“xxxx”。")]
    [Filter([ChatType.System], "聊天频道.*?说话.*?键盘.*?输入")]
    QuestReminder,

    [Cate(Cate.System)]
    [RuleInfo("隐藏风脉仪提示信息")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 3712, 3713, 3714, 3715)]
    AetherCompass,

    [Cate(Cate.System)]
    [RuleInfo("隐藏<你>的职业转换成了“xx”")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 561)]
    YourJobChanged,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏发起了准备确认")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 3790)]
    ReadyChecks,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏准备确认结果")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 3794)]
    ReadyChecksComplete,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏战斗开始倒计时")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 5260, 5264)]
    CountdownTime,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏向<玩家>发送了入队/团队邀请")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1, 4616)]
    InviteSent,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏收到了<玩家>发来的入队/团队邀请")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 3, 4618)]
    InvitedBy,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏你加入了<玩家>组建的“xxx”队伍")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7455, 7456, 7466, 7467)]
    YouJoinPartyFinder,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏你加入了小队/团队", "含跨服")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 61, 4619, 7446, 7465, 7473)]
    YouJoinParty,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏<玩家>加入了队伍")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 60)]
    [Filter(PlayerFilterKind.NotContainYou)]
    OtherPlayerJoins,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏你或其他玩家离开了队伍")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4, 68, 69, 70, 71)]
    LeftParty,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏队伍解散了")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 5, 72, 73, 7447, 7448, 7468, 7469)]
    PartyDisbandAndDissolved,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏xx任务开始/结束", "包括距离“xx”结束还有x分钟")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1531, 1532, 1533, 1534)]
    DutyStartEnd,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏全体队员都从特务队长那里领取到报酬或经过一段时间之后，行会令会结束并且全员返回")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1530)]
    GuildhestEnded,

    [Cate(Cate.System, SubCate.Zone)]
    [RuleInfo("隐藏进入/离开了休息区")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 732, 733)]
    EnterLeftSanctuary,

    [Cate(Cate.System, SubCate.Zone)]
    [RuleInfo("隐藏进入/离开了幻卡游戏区")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4763, 4764)]
    TripleTriadArea,

    [Cate(Cate.System)]
    [RuleInfo("隐藏在线状态更新信息")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 97, 98)]
    OnlineStatus,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏有种不安的感觉", "常见于巡逻类理符任务")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1831)]
    UnsettlingPresence,

    [Cate(Cate.System)]
    [RuleInfo("隐藏成功将金币/物品附加到了邮件中")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 672, 673)]
    AttachToMail,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏获得了<魔陶器>")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7221)]
    ObtainedPomander,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏无法获得更多的<魔陶器>，放回到了宝箱中")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7222)]
    ReturnedPomander,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏<物体>开始散发光辉")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7242)]
    CairnGlows,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏倒下的冒险者重新恢复了活力")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7243)]
    RestoresLifeToFallen,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏<物体>启动了")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7245)]
    CairnActivates,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏传送倒计时/中断/成功")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7246, 7247, 7248)]
    Transference,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏强化值提升成功")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7250, 7252, 7253)]
    AetherpoolIncrease,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏强化值提升失败")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7251)]
    AetherpoolUnchanged,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏咒印解除提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7255)]
    PomanderOfSafety,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏全景提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7256)]
    PomanderOfSight,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏宝箱增加提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7259)]
    PomanderOfAffluence,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏减少敌人提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7260)]
    PomanderOfFlight,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏改变敌人提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7261)]
    PomanderOfAlteration,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏形态变化提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7264)]
    PomanderOfWitching,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏魔法效果解除提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7265)]
    PomanderOfSerenity,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏楼层信息", "地下1层")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7270, 9218)]
    FloorNumber,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏这一层似乎有宝藏")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7272)]
    SenseAccursedHoard,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏这一层似乎没有宝藏")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7273)]
    DoNotSenseAccursedHoard,

    [Cate(Cate.System, SubCate.DeepDungeon)]
    [RuleInfo("隐藏发现了埋藏的宝藏")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 7274)]
    DiscoverAccursedHoard,

    [Cate(Cate.CraftingGathering, SubCate.Materia)]
    [RuleInfo("隐藏<装备>的精炼度升到了100%")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 744)]
    SpiritboundGear,

    [Cate(Cate.System, SubCate.Zone)]
    [RuleInfo("隐藏到达/离开了探索笔记的目的地")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1272, 1273)]
    VistaMessages,

    [Cate(Cate.System, SubCate.Glamour)]
    [RuleInfo("隐藏对<装备>进行了试穿")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 3911)]
    TryOnGlamour,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏宝箱的数量将变为x个")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4233, 4238, 4246, 4516)]
    EligibleForCoffers,

    [Cate(Cate.System)]
    [RuleInfo("隐藏退出了新人频道")]
    [Filter([ChatType.NoviceNetworkSystem], MessageFilterKind.LogMessage, 7030)]
    NoviceNetworkLeft,

    [Cate(Cate.System)]
    [RuleInfo("隐藏达到人数上限，无法加入新人频道")]
    [Filter([ChatType.NoviceNetworkSystem], MessageFilterKind.LogMessage, 7005)]
    NoviceNetworkFull,

    [Cate(Cate.System)]
    [RuleInfo("隐藏个人房屋/共享房屋/个人房间/公寓的交流簿有新的留言")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 6066, 6067, 6068, 6069)]
    PersonalMessageBook,

    [Cate(Cate.System, SubCate.Trading)]
    [RuleInfo("隐藏向<玩家>发送了交易申请")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 32)]
    TradeSent,

    [Cate(Cate.System, SubCate.Trading)]
    [RuleInfo("隐藏等待<玩家>确认交易内容")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 33)]
    AwaitingTradeConfirmation,

    [Cate(Cate.System, SubCate.Trading)]
    [RuleInfo("隐藏交易取消")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 36)]
    TradeCanceled,

    [Cate(Cate.System, SubCate.Trading)]
    [RuleInfo("隐藏交易完成")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 38)]
    TradeComplete,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏队伍中有初次执行该任务的队员，在完成任务时会获得额外的/获得了额外的奖励")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4225, 4226, 7975)]
    FirstClearAward,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏收到了<玩家>去往<地点>的传送邀请")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 440)]
    OfferedTeleport,

    [Cate(Cate.System, SubCate.Party)]
    [RuleInfo("隐藏传送邀请已保留，请在准备接受传送时打开“传送邀请”信息")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 443)]
    TeleportOfferPreserved,

    [Cate(Cate.System, SubCate.Glamour)]
    [RuleInfo("隐藏套装“<套装名>”的装备配置更改完毕")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 755)]
    GearsetChanged,

    [Cate(Cate.System, SubCate.Glamour)]
    [RuleInfo("隐藏成功装备了套装，不过仍缺少必要的装备")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 761)]
    GearsetEquipped1,

    [Cate(Cate.System, SubCate.Glamour)]
    [RuleInfo("隐藏成功装备了套装，不过其中有一部分具有其他投影效果的物品")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1900)]
    GearsetEquipped2,

    [Cate(Cate.System, SubCate.Glamour)]
    [RuleInfo("隐藏成功装备了套装，不过其中有一部分染过色的物品")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1901)]
    GearsetEquipped3,

    [Cate(Cate.System, SubCate.Glamour)]
    [RuleInfo("隐藏成功装备了套装，不过其中有一部分物品无法装备")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1906)]
    GearsetEquipped4,

    [Cate(Cate.System, SubCate.Glamour)]
    [RuleInfo("隐藏成功装备了套装，不过其中混有镶有其他魔晶石或以太化的物品")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1908)]
    GearsetEquipped5,

    [Cate(Cate.System, SubCate.Glamour)]
    [RuleInfo("隐藏成功装备了套装，不过其中混有以太化的物品")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1936)]
    GearsetEquipped6,

    [Cate(Cate.System, SubCate.Glamour)]
    [RuleInfo("隐藏已使用投影模板xx进行武具投影")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4364)]
    GlamourProjected,

    [Cate(Cate.System, SubCate.Glamour)]
    [RuleInfo("隐藏将xx的外型投影到了xx上")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4309)]
    ItemProjected,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏场地封锁/封锁解除提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 2012, 2013, 2014)]
    SealedOff,

    [Cate(Cate.System)]
    [RuleInfo("隐藏确认物品持有数量的结果")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1440, 1441, 1442, 1443, 1444, 1445, 1446, 1447, 1448, 1450, 1451, 1452)]
    SearchForItemResults,

    [Cate(Cate.System)]
    [RuleInfo("隐藏当前共有xx个队伍正在招募队员，其中xx个队伍符合搜索条件")]
    [Filter(ChatType.PeriodicRecruitmentNotification)]
    PeriodicRecruitmentNotification,

    [Cate(Cate.System, SubCate.FreeCompanyAndVoyage)]
    [RuleInfo("隐藏部队成员上线")]
    [Filter([ChatType.FreeCompanyLoginLogout], MessageFilterKind.LogMessage, 3085)]
    FCMemberLogins,

    [Cate(Cate.System, SubCate.FreeCompanyAndVoyage)]
    [RuleInfo("隐藏部队成员下线")]
    [Filter([ChatType.FreeCompanyLoginLogout], MessageFilterKind.LogMessage, 3086)]
    FCMemberLogouts,

    [Cate(Cate.System, SubCate.FreeCompanyAndVoyage)]
    [RuleInfo("隐藏部队房屋的交流簿有新的留言")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 6065)]
    FreeCompanyMessageBook,

    [Cate(Cate.System, SubCate.FreeCompanyAndVoyage)]
    [RuleInfo("隐藏飞空艇、潜水艇出发了")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4158, 6057)]
    VoyageEmbarked,

    [Cate(Cate.System, SubCate.FreeCompanyAndVoyage)]
    [RuleInfo("隐藏接受了“xx”探索完成的报告")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4161)]
    VoyageComplete,

    [Cate(Cate.System, SubCate.FreeCompanyAndVoyage)]
    [RuleInfo("隐藏飞空艇、潜水艇探索发现了目的地<编号>")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4163, 6061)]
    VoyageDestinationDiscovered,

    [Cate(Cate.System, SubCate.FreeCompanyAndVoyage)]
    [RuleInfo("隐藏修理了xx部件")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4168)]
    VoyageRepaired,

    [Cate(Cate.Emote)]
    [RuleInfo("隐藏你发送的/以你为目标的情感动作信息")]
    [Filter([ChatType.StandardEmote, ChatType.CustomEmote])]
    [Filter(PlayerFilterKind.ContainsYou)]
    YourEmotes,

    [Cate(Cate.Emote)]
    [RuleInfo("隐藏其他玩家的情感动作信息")]
    [Filter([ChatType.StandardEmote, ChatType.CustomEmote])]
    [Filter(PlayerFilterKind.NotContainYou)]
    OtherPlayerEmotes,

    [Cate(Cate.CraftingGathering, SubCate.Desynthesis)]
    [RuleInfo("隐藏成功分解了<物品>")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 4321)]
    DesynthesizedItem,

    [Cate(Cate.CraftingGathering, SubCate.Desynthesis)]
    [RuleInfo("隐藏分解获得的产物")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 3554)]
    DesynthesisObtains,

    [Cate(Cate.CraftingGathering, SubCate.Desynthesis)]
    [RuleInfo("隐藏分解技能提升了x点")]
    [Filter([ChatType.Progress], MessageFilterKind.LogMessage, 4325)]
    DesynthesisLevel,

    [Cate(Cate.CraftingGathering, SubCate.Crafting)]
    [RuleInfo("隐藏你制作<物品>成功")]
    [Filter([ChatType.Crafting], MessageFilterKind.LogMessage, 1156, 1157)]
    [Filter(PlayerFilterKind.ContainsYou)]
    YouSynthesized,

    [Cate(Cate.CraftingGathering, SubCate.Crafting)]
    [RuleInfo("隐藏其他玩家制作<物品>成功")]
    [Filter([ChatType.Crafting], MessageFilterKind.LogMessage, 1156, 1157)]
    [Filter(PlayerFilterKind.NotContainYou)]
    OtherPlayerSynthesized,

    [Cate(Cate.CraftingGathering, SubCate.Materia)]
    [RuleInfo("隐藏魔晶石镶嵌成功")]
    [Filter([ChatType.Crafting], MessageFilterKind.LogMessage, 1201)]
    AttachedMateria,


    [Cate(Cate.CraftingGathering, SubCate.Materia)]
    [RuleInfo("隐藏魔晶石镶嵌失败")]
    [Filter([ChatType.Crafting], MessageFilterKind.LogMessage, 1202)]
    OvermeldFailure,

    [Cate(Cate.CraftingGathering, SubCate.Materia)]
    [RuleInfo("隐藏用<装备>进行了精制魔晶石")]
    [Filter([ChatType.Crafting], MessageFilterKind.LogMessage, 1200)]
    MateriaExtract,

    [Cate(Cate.CraftingGathering, SubCate.Materia)]
    [RuleInfo("隐藏成功回收了魔晶石")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1954)]
    MateriaRetrieved,

    [Cate(Cate.CraftingGathering, SubCate.Materia)]
    [RuleInfo("隐藏回收魔晶石失败")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 1955)]
    MateriaShatters,

    [Cate(Cate.CraftingGathering, SubCate.Crafting)]
    [RuleInfo("隐藏练习制作相关信息")]
    [Filter([ChatType.Crafting], MessageFilterKind.LogMessage, 5902, 5903, 5904, 5905, 5906, 5907, 5908)]
    TrialMessages,

    [Cate(Cate.CraftingGathering, SubCate.Gathering)]
    [RuleInfo("隐藏开始了/结束了采集作业")]
    [Filter([ChatType.GatheringSystem], MessageFilterKind.LogMessage, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070)]
    GatheringStartEnd,

    [Cate(Cate.CraftingGathering, SubCate.Gathering)]
    [RuleInfo("隐藏在<方向>感知到了x级的<采集点>")]
    [Filter([ChatType.GatheringSystem], MessageFilterKind.LogMessage, 3561)]
    GatheringSenses,

    [Cate(Cate.CraftingGathering, SubCate.Gathering)]
    [RuleInfo("隐藏采集点的特殊效果发动")]
    [Filter([ChatType.GatheringSystem], MessageFilterKind.LogMessage, 1096, 1097, 1098, 1099, 1207, 5550, 5551, 5552, 5553)]
    LocationAffects,

    [Cate(Cate.CraftingGathering, SubCate.Gathering)]
    [RuleInfo("隐藏<玩家>对<收藏品>进行了精选和获得的产物提示")]
    [Filter([ChatType.System], MessageFilterKind.LogMessage, 3553, 3554, 3555)]
    AetherialReductionSands,

    [Cate(Cate.CraftingGathering, SubCate.Fishing)]
    [RuleInfo("隐藏将<鱼>的数据记录到了鱼类图鉴中")]
    [Filter([ChatType.Gathering], MessageFilterKind.LogMessage, 1114)]
    AddedToFishGuide,

    [Cate(Cate.CraftingGathering, SubCate.Fishing)]
    [RuleInfo("隐藏开始利用上钩的<鱼>尝试以小钓大")]
    [Filter([ChatType.Gathering], MessageFilterKind.LogMessage, 1121)]
    Mooching,

    [Cate(Cate.CraftingGathering, SubCate.Fishing)]
    [RuleInfo("隐藏在<钓场>甩出了鱼线开始钓鱼")]
    [Filter([ChatType.Gathering], MessageFilterKind.LogMessage, 1110)]
    CurrentFishingLocation,

    [Cate(Cate.CraftingGathering, SubCate.Fishing)]
    [RuleInfo("隐藏将新钓场记录到了钓鱼笔记中")]
    [Filter([ChatType.Gathering], MessageFilterKind.LogMessage, 1115, 3579, 1130, 3513)]
    DiscoveredFishinglocation,

    [Cate(Cate.CraftingGathering, SubCate.Fishing)]
    [RuleInfo("隐藏成功钓上了<鱼><星寸>")]
    [Filter([ChatType.Gathering], MessageFilterKind.LogMessage, 3512)]
    MeasuringIlms,

    [Cate(Cate.LootObtain, SubCate.LootingRolling)]
    [RuleInfo("隐藏你对<物品>掷骰")]
    [Filter([ChatType.LootRoll], MessageFilterKind.LogMessage, 5180)]
    [Filter(PlayerFilterKind.ContainsYou)]
    YouCastLot,

    [Cate(Cate.LootObtain, SubCate.LootingRolling)]
    [RuleInfo("隐藏其他玩家对<物品>掷骰")]
    [Filter([ChatType.LootRoll], MessageFilterKind.LogMessage, 5180)]
    [Filter(PlayerFilterKind.NotContainYou)]
    OtherPlayerCastLot,

    [Cate(Cate.LootObtain, SubCate.LootingRolling)]
    [RuleInfo("隐藏你在<条件>下对<物品>掷出了<x>点")]
    [Filter([ChatType.LootRoll], MessageFilterKind.LogMessage, 1231)]
    [Filter(PlayerFilterKind.ContainsYou)]
    YouLootRoll,

    [Cate(Cate.LootObtain, SubCate.LootingRolling)]
    [RuleInfo("隐藏其他玩家在<条件>下对<物品>掷出了<x>点")]
    [Filter([ChatType.LootRoll], MessageFilterKind.LogMessage, 1231)]
    [Filter(PlayerFilterKind.NotContainYou)]
    OtherPlayerLootRoll,

    [Cate(Cate.LootObtain, SubCate.LootingRolling)]
    [RuleInfo("隐藏其他玩家获得了<物品>")]
    [Filter([ChatType.LootRoll])]
    [Filter(PlayerFilterKind.PlayerObtained)]
    [Filter(PlayerFilterKind.NotContainYou)]
    OtherPlayerObtained,

    [Cate(Cate.LootObtain, SubCate.Other)]
    [RuleInfo("隐藏获得了惰性水晶/晶簇")]
    [Filter([ChatType.LootRoll, ChatType.LootNotice], MessageFilterKind.Obtained, "惰性.*?(水晶|晶簇)")]
    ObtainedCrackedShards,

    [Cate(Cate.LootObtain, SubCate.Other)]
    [RuleInfo("隐藏获得了六种元素的碎晶/水晶/晶簇")]
    [Filter([ChatType.Gathering, ChatType.LootRoll, ChatType.LootNotice], MessageFilterKind.Obtained, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19)]
    ObtainedShards,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏获得了额外的经验值和金币作为随机任务的奖励")]
    [Filter([ChatType.LootNotice], MessageFilterKind.LogMessage, 2246)]
    RouletteBonus,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏以少数职能参加了随机任务，获得了额外的经验值和金币作为奖励")]
    [Filter([ChatType.LootNotice], MessageFilterKind.LogMessage, 2244)]
    AdventurerInNeedBonus,

    [Cate(Cate.LootObtain, SubCate.Currencies)]
    [RuleInfo("隐藏获得了金币")]
    [Filter([ChatType.LootNotice], MessageFilterKind.LogMessage, 1798)]
    ObtainedGil,

    [Cate(Cate.LootObtain, SubCate.Currencies)]
    [RuleInfo("隐藏获得了金碟币")]
    [Filter([ChatType.LootNotice], MessageFilterKind.LogMessage, 4765)]
    ObtainedMGP,

    [Cate(Cate.LootObtain, SubCate.Currencies)]
    [RuleInfo("隐藏获得了亚拉戈xx神典石")]
    [Filter([ChatType.LootNotice], MessageFilterKind.Obtained, "亚拉戈.{2}神典石")]
    ObtainedTomestones,

    [Cate(Cate.LootObtain, SubCate.Currencies)]
    [RuleInfo("隐藏获得了狼印战绩")]
    [Filter([ChatType.LootNotice], MessageFilterKind.Obtained, 25)]
    ObtainedWolfMarks,

    [Cate(Cate.LootObtain, SubCate.Currencies)]
    [RuleInfo("隐藏获得了军票")]
    [Filter([ChatType.LootNotice], MessageFilterKind.Obtained, "(?:双蛇党|黑涡团|恒辉队)的军票")]
    ObtainedSeals,

    [Cate(Cate.LootObtain, SubCate.Currencies)]
    [RuleInfo("隐藏获得了探险币")]
    [Filter([ChatType.LootNotice], MessageFilterKind.Obtained, 21072)]
    ObtainedVenture,

    [Cate(Cate.LootObtain, SubCate.Currencies)]
    [RuleInfo("隐藏获得了友好部族货币")]
    [Filter([ChatType.LootNotice], MessageFilterKind.Obtained, 21073, 21074, 21075, 21076, 21077, 21078, 21079, 21080, 21081, 21935, 22525, 28186, 28187, 28188, 36657, 37854, 38952)]
    ObtainedTribalCurrency,

    [Cate(Cate.LootObtain, SubCate.Currencies)]
    [RuleInfo("隐藏获得了同盟徽章")]
    [Filter([ChatType.LootNotice], MessageFilterKind.Obtained, 27)]
    ObtainedAlliedSeals,

    [Cate(Cate.LootObtain, SubCate.Currencies)]
    [RuleInfo("隐藏获得了兵团徽章")]
    [Filter([ChatType.LootNotice], MessageFilterKind.Obtained, 10307)]
    ObtainedCenturioSeals,

    [Cate(Cate.LootObtain, SubCate.Currencies)]
    [RuleInfo("隐藏获得了怪物狩猎的战利品")]
    [Filter([ChatType.LootNotice], MessageFilterKind.Obtained, 26533)]
    ObtainedNuts,

    [Cate(Cate.LootObtain, SubCate.Other)]
    [RuleInfo("隐藏获得了魔晶石")]
    [Filter([ChatType.LootNotice], MessageFilterKind.Obtained, "(?<!半)魔晶石.{1,2}型")]
    ObtainedMaterials,

    [Cate(Cate.System, SubCate.ContentsAndQuests)]
    [RuleInfo("隐藏解限任务完成时间")]
    [Filter([ChatType.Progress], MessageFilterKind.LogMessage, 4679)]
    CompletionTime,

    [Cate(Cate.Progress)]
    [RuleInfo("隐藏获得了经验值")]
    [Filter([ChatType.Progress], MessageFilterKind.LogMessage, 4761)]
    GainExperience,

    [Cate(Cate.Progress)]
    [RuleInfo("隐藏获得了对战经验值")]
    [Filter([ChatType.Progress], MessageFilterKind.LogMessage, 659)]
    GainPvpExp,

    [Cate(Cate.Progress)]
    [RuleInfo("隐藏你获得了成就")]
    [Filter([ChatType.Progress], MessageFilterKind.LogMessage, 952)]
    [Filter(PlayerFilterKind.ContainsYou)]
    YouEarnAchievement,

    [Cate(Cate.Progress)]
    [RuleInfo("隐藏其他玩家获得了成就")]
    [Filter([ChatType.Progress], MessageFilterKind.LogMessage, 952)]
    [Filter(PlayerFilterKind.NotContainYou)]
    OtherPlayerEarnedAchievement,

    [Cate(Cate.Progress)]
    [RuleInfo("隐藏你升级")]
    [Filter([ChatType.Progress], MessageFilterKind.LogMessage, @".*?升到了.*?级")]
    [Filter(PlayerFilterKind.ContainsYou)]
    YouLevelUps,

    [Cate(Cate.Progress)]
    [RuleInfo("隐藏其他人/物体/对象升级")]
    [Filter([ChatType.Progress], MessageFilterKind.LogMessage, @".*?升到了.*?级")]
    [Filter(PlayerFilterKind.NotContainYou)]
    OtherLevelUps,

    [Cate(Cate.Progress)]
    [RuleInfo("隐藏学会了<技能>/<特性>", "在深层迷宫中会经常出现这些信息")]
    [Filter([ChatType.Progress], MessageFilterKind.LogMessage, 446, 552, 553)]
    AbilityUnlocks,

    [Cate(Cate.System)]
    [RuleInfo("隐藏其他未列出的系统频道信息")]
    [Filter(ChatType.System)]
    AllOtherSystemMessages,
}
