# VERIFY-QUEUE — 待上机人工验证清单

边建边记。每条 = 要做的操作（含游戏内控制台命令）+ 预期结果。机器只能编译+加载冒烟，**实战触发/数值/UI 必须上机验**。验过的打勾并写结果（或贴异常）。

## 用法
- 开发者控制台：游戏内呼出（默认反引号 `` ` `` 或波浪键；若无，查 DevConsole 绑定）。
- 加牌：`card <Entry>`，移除：`remove_card <Entry>`，升级：`upgrade <Entry>`。
- **我们的卡 Entry 带全前缀 `DUNDUNDUDU-`**（如 `card DUNDUNDUDU-STEADY_PUNCH`）。打 `card ` 后按 **Tab** 可补全/列出。
- 诊断日志在 `%APPDATA%\SlayTheSpire2\logs\godot.log`，我们的行带 `[闷气]` / `[DundunDudu]` 前缀，可回贴给我读。

---

## A. 进入 run / 崩溃回归（P1a 崩溃修复的欠债）
- [ ] **A1 墩墩进 run**：选墩墩 → Neow 选「给遗物」祝福（含七咒/LargeCapsule 那类）→ 不崩、正常进战斗。起手 = 5 猛击 + 4 防御 + 2 测试卡。
- [ ] **A2 嘟嘟进 run**：选嘟嘟同样选给遗物 → 不崩、进战斗。起手 = 4 轻击 + 4 轻避 + 1 灵巧。
- [ ] **A3 标签隐患抽查**：静态审计已确认全反编译里「按 CardTag 在角色池 First/Single 取牌」**只有 LargeCapsule**（Strike+Defend 已补标签）。上机随机吃几个会动起始牌的远古/遗物，确认不再 `Sequence contains no matching element`。

## B. 闷气 P1a — 分层改伤
- [ ] **B1**：战斗内 `card DUNDUNDUDU-STEADY_PUNCH`（加 1 张稳扎一拳，7 伤）。打 1 张测试卡（+3 闷气）→ 稳扎一拳应显示 **10**（×1.5）；再打 1 张（共 6）→ 显示 **14**（×2）。读 log `[闷气] 改伤 stacks=.. ×..`。
- [ ] **B2 只放大自己**：确认敌人攻击、嘟嘟（若在场）攻击的伤害**不**被墩墩闷气放大（钩子门控 `dealer==Owner`）。

## C. 闷气 P1b — 罚则
- [ ] **C1 免疫负面(6–9)**：闷气推到 6–9，让敌人对你施虚弱/易伤 → 应被挡下（debuff 不上身）。log `[闷气] 免疫负面 stacks=.. 挡下 WeakPower`。
- [ ] **C2 不免疫边界**：闷气 3–5 或 = 10 时，敌人 debuff **正常上身**（免疫只在 6–9）。
- [ ] **C3 临界穿透(9)**：闷气=9，**回合开始**受 1 点穿透（无视格挡，扣血）。log `[闷气] 临界自伤 stacks=9 ... 回合始`。
- [ ] **C4 自闭穿透(10)**：闷气=10，**回合结束**受 3 点穿透。log `[闷气] 自闭自伤 stacks=10 ... 回合末`。
- [ ] **C5 层数封顶(10)**：连打 4 张测试卡（请求 +12）→ 闷气停在 **10**，不超。log `[闷气] gain ...` 末次显示 `+0`（或 `[闷气] 封顶 ..→10`）。
- [ ] （待定，未做）**10 封技能**：本阶段未接 ShouldPlay；10 层暂不锁技能牌。要做再说。

## D. 卡池 / 控制台 / 卡面
- [ ] **D1**：`card DUNDUNDUDU-DOUBLE_KICK` / `GRAND_FEAST` / `WHIRLWIND` / `FEAST` 等，看中文卡名/数值/效果是否对。
- [ ] **D2 奖励池**：正常通关战斗，卡牌奖励里应能出现墩墩 12 张 / 嘟嘟 17 张 cheap（Common/Uncommon/Rare）。Basic（猛击/防御）与测试卡不进奖励池（正常）。

## E. 清理项（与崩溃分开）
- [ ] **E1 反斜杠路径**：进战斗看能量数字图标（text_energy）正常显示；log 不再有 `Asset not cached: res://DundunDudu\images\...`（反斜杠）。
- [ ] **E2 选角 bg**：选角界面不再报 `char_select_bg_dundundudu-*.tscn` 加载 [ERROR]（现继承占位 ironclad/silent bg）。

## F. 太空步 / 残影（P4）
- [ ] **F1 获得残影**：`card DUNDUNDUDU-AERIAL_RETREAT` 打出 → +2 层[残影]，状态栏出现残影图标（占位图）。
- [ ] **F2 否定攻击**：有残影时挨敌人一拳 → **完全免伤**（不论数值），每次消耗 1 层。log `[残影] 否定一次攻击, 消耗 1 层 (剩 N)`。
- [ ] **F3 多段攻击（SPEC §3 watch-item）**：攒 2 层残影，挨一个**多段攻击**的敌人 → 应每段各消耗 1 层（2 层挡 2 段），不是「1 层挡整套」也不是「一次扣光」。这是唯一需要重点盯的边界。
- [ ] **F4 回合末清空**：回合内剩残影没用完 → 敌方回合结束清空，不跨回合。log `[残影] 回合末清空 N 层`。
- [ ] **F5 太空步连切**：先有残影再 `card DUNDUNDUDU-MOONWALK_SLASH` → 9 伤 + 额外 1 层残影；无残影打 → 只 9 伤不加（残影每回合清空，故「有残影」=「本回合已获得」）。
- [ ] **F6 完美落地**：攒 N 层残影 → `card DUNDUNDUDU-PERFECT_LANDING` 伤害应 = 6×N（无残影则 12 保底）；打完残影清空。注意：**卡面预览恒显 12**（保底值），实际按层数算——确认伤害数对即可。
- [ ] **F7 舞台聚光灯**：本回合先打一张攻击再 `card DUNDUNDUDU-SPOTLIGHT` → 9 格挡；没先打攻击 → 6 格挡。
- [ ] **F8 预览**：有残影时，敌人攻击意图对你的伤害预览应显示 0（ModifyDamageCap）。

## G. 巨剑 / 蓄力 + 绝境坚持（P5）
- [ ] **G1 蓄力获得**：一个回合**不打任何攻击牌**（只出技能/能力，或不出牌）→ 回合结束 +1 层[蓄力]。log `[蓄力] 本回合未出攻击 → +1`。打过攻击的回合**不**加。
- [ ] **G2 跨回合保留**：蓄力不清零、逐回合累积（与残影相反）。
- [ ] **G3 斩断一切**：攒 N 层蓄力 → `card DUNDUNDUDU-CLEAVE_ALL` 伤害应 = 8+4×N；打完蓄力清 0。（卡面预览显 8 基础值，实际按层算。）
- [ ] **G4 绝境坚持**：`card DUNDUNDUDU-LAST_STAND` 上 buff；把 HP 打到 <30%（墩墩 75 血 → <22.5 即 ≤22）→ 攻击牌伤害 ×1.3；HP≥30% 不加成。log `[绝境坚持] HP .. <30% → 攻击 ×1.3`。
- [ ] **G5 沉重步伐**：`card DUNDUNDUDU-HEAVY_STEP` → 3 格挡 + 抽 1。
- [ ] **G6 叠乘**：闷气爆发(×2) + 绝境坚持(×1.3) 同时 → 攻击应 ×2.6（两个乘法 buff 叠乘，确认数值）。

## H. 极限格斗 / 熟悉 + 千锤百炼（P6）
- [ ] **H1 熟悉获得**：受到**未格挡**伤害（敌人打你掉血，或自伤）→ +1 层[熟悉]。log `[熟悉] 受未格挡伤害 N → +1`。被格挡吸收的伤害**不**加。
- [ ] **H2 攻击加成**：有 N 层熟悉 → 攻击牌伤害 +N（如稳扎一拳 7 → 7+N）。log `[熟悉] 攻击 +N`。
- [ ] **H3 千锤百炼**：`card DUNDUNDUDU-TEMPER_HAMMER` 后，熟悉加成翻倍（N 层 → +2N）。log 显示 `千锤百炼=True`。
- [ ] **H4 以伤换伤**：`card DUNDUNDUDU-BLOOD_FOR_BLOOD` → 对敌 6 伤 + 自身 2 伤 → +1 熟悉。**已知交互**：若此时你有[残影]，自伤会被残影否定 → 不加熟悉（跨 archetype，符合规则，非 bug）。
- [ ] **H5 野兽之力**：`card DUNDUNDUDU-BEAST_FORCE` → 16 伤。
- [ ] **H6 加法在乘法前**：熟悉 +N（加法）与 闷气×2 / 绝境×1.3（乘法）→ 伤害应 = (基础+N)×倍率，确认顺序对。

## I. 玩偶遗物（P7，结构性 3 个；加遗物：控制台 `relic DUNDUNDUDU-<SLUG>`）
- [ ] **I1 卡皮吧啦**：`relic DUNDUNDUDU-CAPYBARA`。一回合留 N 点能量不花 → 回合结束 +N 格挡。log `[卡皮吧啦] 回合末剩余能量 N → 格挡 +N`。
- [ ] **I2 大鼠与美叽**：`relic DUNDUNDUDU-MOUSE_AND_MAGPIE`。一回合打 2 张同费用牌 → 抽 1（4 张再抽 1）。log `[大鼠与美叽]`。
- [ ] **I3 比比拉不**：`relic DUNDUNDUDU-BIBILABU`。一回合内打 ≥1 攻击牌 + ≥1 提供格挡的技能牌 → +3 格挡（每回合触发一次）。log `[比比拉不] 刀盾连击`。
- [ ] **I4 图标**：3 个遗物用占位 relic.png（大图标占位；小图标可能糊/缺，纯美术，后面换真图）。
- 备注（**本轮未建，依赖缺口**）：小熊虫(首次免死)需 Osty 死亡系统研究；小粉/土豆/小白菜/香烟 依赖 P2(熬大夜)/P3(SNACK·KARAOKE 标签)——这些押后。

## J. 起手 starters（P3a，替换占位 DundunStrike/Defend）
- [ ] **J1 大屁墩！**：起手有大屁墩（基础攻击）。闷气<3 打 → 6 伤；闷气≥3 → 9 伤。注意卡面预览恒显 6（实际按闷气，打出才 +3）。
- [ ] **J2 墩坚强**：基础防御。闷气 0–2 → 5 格挡；3–5 → 7；6–8 → 9；9–10 → 11（每 3 层 +2，最多 +6）。预览恒显 5。
- [ ] **J3 起手组**：5 大屁墩 + 4 墩坚强 + 2 测试卡（共 11）。大屁墩带 Strike 标签、墩坚强带 Defend 标签（LargeCapsule 仍安全）。
- 备注：P3a 仅建标签**基础设施**（DundunCardTag 枚举 + ITaggedDundunCard 接口 + DundunCardTags 查询；因 BaseLib 3.3.2 无 ModCardTagRegistry，改用 mod 内接口标签）。猪饲料/麦霸标签在 P3b 挂到 reskin 卡上。

## K. P3b reskin 12 卡（猪饲料/麦霸/兴趣，`card DUNDUNDUDU-<SLUG>`）
- [ ] **K1 麦霸/Karaoke**：关起门来吼两嗓子(清2闷气+3格挡) · 五音不全也要唱(7伤，闷气≥3 额外清1) · 一个人的卡拉OK(回5+清3闷气) · 副歌大合唱(6伤+全体3) · 高音炫技(16；本回合没出技能牌→22)。
- [ ] **K2 猪饲料/Snack**：糖醋排骨(回6+清2闷气) · 牛排(下张攻击+4+1闷气) · 蛋饼(0费 清1闷气+抽1) · Pizza(回7+2能量) · Strogonoff(回12+1闷气)。
- [ ] **K3 兴趣(无标签)**：限定版手办上架(5格挡；已有格挡→8) · 长款皮衣(10格挡+回3)。
- [ ] **K4 闷气涨/清**：看 log `[闷气] gain ...` / `[闷气] 清空 N 层`；清空不会变负（封在当前层数）。
- [ ] **K5 标签生效**：P3b 已给猪饲料卡挂 Snack、麦霸卡挂 Karaoke 接口标签；真正**消费**标签的卡/遗物（香烟/小白菜/安可）押后。
- 简化/偏差（已知）：牛排「+50%」→ VigorPower 固定 +4（近似，BaseLib 无现成「百分比下张攻击」）；限定版手办「本回合未受伤」→「已有格挡」(Cheap proxy)；Strogonoff 的 土豆/迟钝 条款随其遗物押后。

## L. 熬大夜（P2，Debuff；加 power：控制台 `power DUNDUNDUDU-SLEEP_DEPRIVED_POWER <层数> 0`）
- [ ] **L1 加熬大夜**：控制台 `power DUNDUNDUDU-SLEEP_DEPRIVED_POWER 3 0`（id 大写、第二个数=层数、第三个=目标 0=自己）→ 状态栏出现熬大夜（占位图）。
- [ ] **L2 格挡削减**：有 N 层熬大夜，本回合打**第一张**格挡牌 → 该牌格挡 -N（最低0）；同回合第二张格挡牌**不**受影响。log `[熬大夜] 削减本回合首张格挡牌 -N`。（卡面预览可能显基础值，削减在打出时生效。）
- [ ] **L3 衰减**：回合结束 -1 层。log `[熬大夜] 回合末衰减 -1`。
- [ ] **L4 闷气联动**：熬大夜≥3 → 回合开始额外 +1 闷气。log `[熬大夜] ≥3 → 回合始 +1 闷气`。
- [ ] **L5 免疫交互**：熬大夜是 Debuff → 闷气 6–9（免疫负面）时加熬大夜会被挡（设计如此）。测试请在闷气<6 时加。
- 备注：解除手段【银色山泉】属 C 段（未建）；其它给熬大夜的卡/遗物（香烟）押后。当前只能控制台加。

## M. 标签/事件消费遗物 + 安可（`relic DUNDUNDUDU-<SLUG>` / `card DUNDUNDUDU-ENCORE`）
- [ ] **M1 小白菜**：`relic DUNDUNDUDU-BABY_BOK_CHOY`。打任意【猪饲料】卡 → +3 生命；若当前 HP≤50% → 额外清 2 闷气。log `[小白菜]`。
- [ ] **M2 小粉**：`relic DUNDUNDUDU-XIAO_FEN`。每次清空闷气（任何方式：猪饲料/麦霸卡）→ +2 生命。log `[小粉] 清空闷气 → +2 生命`。
- [ ] **M3 香烟**：`relic DUNDUNDUDU-CONFISCATED_CIGARETTE`（Event 稀有度→**不进常规奖励池**，只能 console/事件给）。开战最大生命 -6（满血时会掉 6 现血）；每打 1 张猪饲料 → 回 2 点上限；战斗结束还满剩余。log `[香烟]`。**注意**：战斗末「还满」用 GainMaxHp（带治疗）→ 净 HP 中性，代价是战斗中更脆。
- [ ] **M4 安可**：`card DUNDUNDUDU-ENCORE`。3 格挡；若本回合已打过**其他**麦霸卡 → +4（共 7）格挡。
- [ ] **M5 续航闭环**：装香烟（开战 -6 上限）→ 打猪饲料（小白菜 +3 血 & 香烟 +2 上限 &（若清气）小粉 +2 血）→「猪饲料续航流」成立。
- 偏差（已知）：小白菜 +3 生命对**每张**猪饲料触发（含不回血的蛋饼/牛排，简化「回复效果+3」）；小粉的治疗逻辑写在 Sulking.Clear（闷气清空唯一入口）。

## N. C 段特殊卡（`card DUNDUNDUDU-<SLUG>`）
- [ ] **N1 机械键盘**：`card DUNDUNDUDU-MECH_KEYBOARD`（能力）。之后每打出 4 张牌 → 对随机敌人 8 伤。log `[机械键盘] 清脆敲击`。**注**：穿透简化为普通伤害（builder 无 Unblockable）；无可见计数（每 4 张内部计）。
- [ ] **N2 KICK BACK**：`card DUNDUNDUDU-KICK_BACK` → 18 伤；抽牌堆里随机洗入 1 张本场耗能 0 的【大屁墩！】（抽到时 0 费打）。
- [ ] **N3 联机游戏**：`card DUNDUNDUDU-ONLINE_GAMING` → 8 伤 + 1 能量。
- [ ] **N4 芝士味Dorito**：`card DUNDUNDUDU-CHEESY_DORITO` → +2 能量；抽牌堆洗入 1 张废牌（用原版【粘液 Slimed】当「黏糊糊」）。
- [ ] **N5 银色山泉**：`card DUNDUNDUDU-SILVER_SPRING`（消耗）→ ①清你全部负面（先用 `power` 加个虚弱/熬大夜再打，确认被清）②闷气减半转等量人工制品（闷气 8 → 剩 4 + 4 人工制品）③本回合打不出攻击牌（试着出攻击牌应被禁）。
- 偏差（已知）：机械键盘穿透→普通同步随机伤害；黏糊糊→原版 Slimed。

---

### 墩墩可加卡 Entry 速查（`card DUNDUNDUDU-<SLUG>`）
大屁墩！ BIG_BUTT · 墩坚强 STEADFAST · 测试卡 SULK_TEST_CARD
麦霸：关起门来吼两嗓子 SHOUT_IT_OUT · 五音不全也要唱 OFF_KEY_SONG · 一个人的卡拉OK SOLO_KARAOKE · 副歌大合唱 CHORUS_ALL_SING · 高音炫技 HIGH_NOTE · 安可 ENCORE
猪饲料：糖醋排骨 SWEET_SOUR_RIBS · 牛排 STEAK · 蛋饼 EGG_PANCAKE · Pizza PIZZA · Strogonoff STROGONOFF
兴趣：限定版手办上架 FIGURE_DROP · 长款皮衣 LONG_COAT
C段特殊：机械键盘 MECH_KEYBOARD · KICK BACK KICK_BACK · 联机游戏 ONLINE_GAMING · 芝士味Dorito CHEESY_DORITO · 喷洒银色山泉 SILVER_SPRING
太空步系列（残影）：凌空倒退 AERIAL_RETREAT · 太空步连切 MOONWALK_SLASH · 镜花水月 MIRROR_IMAGE · 舞台聚光灯 SPOTLIGHT · 完美落地 PERFECT_LANDING · 华丽转身 GRACEFUL_TURN
巨剑系列（蓄力）：沉重步伐 HEAVY_STEP · 斩断一切 CLEAVE_ALL · 绝境坚持 LAST_STAND
格斗系列（熟悉）：以伤换伤 BLOOD_FOR_BLOOD · 千锤百炼 TEMPER_HAMMER · 野兽之力 BEAST_FORCE

### 墩墩可加遗物 Entry 速查（`relic DUNDUNDUDU-<SLUG>`）
卡皮吧啦 CAPYBARA · 大鼠与美叽 MOUSE_AND_MAGPIE · 比比拉不 BIBILABU · 小白菜 BABY_BOK_CHOY · 小粉 XIAO_FEN · 香烟 CONFISCATED_CIGARETTE(Event)
