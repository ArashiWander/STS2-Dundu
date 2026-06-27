# CARDS.md — content design & build status

Card design + feasibility triage. CC builds from the triaged tables here. **Build status is live**
(updated as cards land). Numbers are 粗平衡, tunable after solo playtest.

## 构建状态 (snapshot)

- **Cheap single-player cards BUILT & smoke-clean: 35** — 墩墩 18 + 嘟嘟 17. All compile 0/0, load in-game
  (pck), zero runtime errors. Plus the 5 starters (墩墩猛击/防御, 嘟嘟轻击/轻避/灵巧) and the spike cards.
- **Medium (custom power, no Harmony) — NOT built, pending:** 8 total.
  - 墩墩 ×3 signatures: 老火靓汤, 以彼之道, 现金垫.
  - 嘟嘟 ×5: 嘟嘟逼人, 炫饭, 光盘行动 (signatures) + **越吃越快, 停不下嘴 (bumped from Cheap** — both need a
    turn-start / on-Nth-card hook that no existing power provides; see notes).
- **双人连携 (synergy) — GATED:** design only; build waits for the live co-op spike PASS (Phase 2).
- **2 conditional cards reinterpreted to Cheap-readable triggers** (no ambient "cards-this-turn" counter
  exists): 防守垫步 → "若你已有格挡", 边吃边抢 → "若你有敏捷". See notes under each table.

## Conventions (confirmed from the build)

- **Deterministic only for any cross-player (synergy) card** — no RNG / shuffle-draw / random buff on a card
  that targets a partner (desyncs lockstep co-op). Single-character cards may use draw/RNG freely. (SPEC §12)
- **Target types:** `Self`, `AnyAlly` (partner, excl. self), `AnyEnemy`, `AllEnemies`, `AnyPlayer`. Synergy = `AnyAlly`.
- **Every card needs `[Pool]`** (colorless or the character pool) or it throws at runtime (compile = warning only).
- **Loc keys:** `DUNDUNDUDU-<SLUG>.title`/`.description`, flat, in `cards.json` under `eng`+`zhs`, packed into the
  pck (`pck_name == DundunDudu`, case-sensitive). `zhs` Chinese is the real text. `<SLUG>` = Slugify(className).
- **Characters:** 墩墩 = tanky / Thorns-retaliation / heavy sustain (warm orange, HP 75). 嘟嘟 = agile / combo /
  cheap-card setup (teal, HP 65). The synergy bridges the two.

## Feasibility legend (vs capability-catalog.md)

- **Cheap** — existing power/command (GainBlock, Apply Thorns/Regen/Weak/Vulnerable/Buffer/Intangible/Barricade/
  Dexterity, DamageCmd, Draw, GainEnergy). Just OnPlay a few lines. ✅ = built & smoke-clean.
- **Medium** — needs a small custom power (`CustomPowerModel`, no Harmony).
- **Expensive** — needs a Harmony patch / new system. Flag before committing.

## 墩墩 cards — ⚠ REDESIGNED (情绪天平 / 残影·蓄力·熟悉), plan pending approval

**墩墩 is being rebuilt around CC's full design** (`墩墩角色Mod设计文档1.md`): 情绪天平【闷气 0–10】+ 熬大夜 debuff
+ three tribute archetypes 残影(太空步) / 蓄力(巨剑) / 熟悉(格斗), 猪饲料·麦霸·兴趣 card families, 情侣(连携) cards,
and 玩偶遗物. This **supersedes** the old Thorns-retaliation 墩墩 below. Engineering plan, API mapping, feasibility,
and the salvage/cut list for the 18 built cards: **`Dundun SPEC.md`** (build order `Dundun Build Plan.md`).

Status: **plan only — no code yet** (awaiting approval). Validated vs the 0.107.1 decompile: the whole design is
**Cheap/Medium, zero Expensive** (5 custom powers via PowerModel hooks; 穿透=`ValueProp.Unblockable`; 迟钝=`SlowPower`;
猪饲料/麦霸 = `ModCardTagRegistry` custom tags). 18 built cards: **hard-cut 3 Thorns (反手回击/见招拆招/倒刺护甲),
salvage 12 (reskin in P3), optional-cut 3** — delete nothing until Yemin confirms (list in Dundun SPEC §4).

<details><summary>Superseded — old Thorns-retaliation 墩墩 pool (kept for the salvage mapping)</summary>

Identity: stand and reflect — **Thorns is the primary win-con** (scales via 以彼之道), survive on Block + heavy
Regen/heal, soften with Weak / Buffer / Intangible. 3 Medium signatures carry the identity. Small attack backbone
(猛击/稳扎一拳/二段蹬腿) as backup vs high-HP low-hit bosses. Life flavor: 羽毛球=防反, 做饭=heal, 财务=mitigation, 攀岩=block.

| 中文名 | 类型 | 费 | 稀有 | 效果(确定性原语) | 目标 | 可行性 | flavor / 状态 |
|---|---|---|---|---|---|---|---|
| 墩墩猛击 | Attack | 1 | Starter | 造成 6 伤害 | AnyEnemy | Cheap | ✅ 起手 ×5 |
| 墩墩防御 | Skill | 1 | Starter | 获得 5 格挡 | Self | Cheap | ✅ 起手 ×5 |
| 抓握点 | Skill | 0 | Common | 获得 3 格挡 | Self | Cheap | ✅ 攀岩 |
| 滋补汤 | Skill | 1 | Common | 4 格挡 + 2 回复 | Self | Cheap | ✅ 做饭 |
| 稳扎一拳 | Attack | 1 | Common | 造成 7 伤害 | AnyEnemy | Cheap | ✅ 备用输出 |
| 缠斗 | Skill | 1 | Common | 施加 2 虚弱 | AnyEnemy | Cheap | ✅ 羽毛球 |
| 储备 | Skill | 1 | Common | 5 格挡 + 抽 1 | Self | Cheap | ✅ 财务 |
| 养生 | Skill | 1 | Common | 回复 4 生命 + 1 回复 | Self | Cheap | ✅ 做饭 |
| 防守垫步 | Skill | 1 | Common | 6 格挡;**若你已有格挡则 9** | Self | Cheap | ✅ 羽毛球（条件改写★） |
| 反手回击 | Power | 1 | Uncommon | 获得 3 荆棘 | Self | Cheap | ✅ 羽毛球 |
| 文火慢炖 | Power | 1 | Uncommon | 获得 3 回复 | Self | Cheap | ✅ 做饭 |
| 对冲 | Skill | 2 | Uncommon | 10 格挡 + 回复 3 生命 | Self | Cheap | ✅ 财务 |
| 风控 | Skill | 1 | Uncommon | 获得 1 屏障 | Self | Cheap | ✅ 财务 |
| 见招拆招 | Skill | 1 | Uncommon | 5 格挡;若有荆棘则荆棘 +2 | Self | Cheap | ✅ 羽毛球 |
| 倒刺护甲 | Power | 2 | Uncommon | 4 荆棘 + 4 格挡 | Self | Cheap | ✅ 攀岩 |
| 连续防守 | Skill | 2 | Uncommon | 全体 2 虚弱 + 6 格挡 | AllEnemies | Cheap | ✅ 羽毛球 AoE |
| 二段蹬腿 | Attack | 2 | Uncommon | 造成 14 伤害 | AnyEnemy | Cheap | ✅ boss 备用 |
| 铜墙铁壁 | Power | 2 | Rare | Barricade(格挡不消失) | Self | Cheap | ✅ 攀岩 |
| 心如止水 | Skill | 2 | Rare | Intangible 1 回合(伤害降为1) | Self | Cheap | ✅ |
| 满汉全席 | Skill | 3 | Rare | 15 格挡 + 回复 8 + 4 回复 | Self | Cheap | ✅ 做饭 |
| 老火靓汤 | Power | 2 | Uncommon | 每回合开始回复 4 并获得 4 格挡 | Self | **Medium** | 做饭引擎(自建 power) — 待 |
| 以彼之道 | Power | 3 | Rare | 受到攻击伤害后,荆棘 +1 | Self | **Medium** | Thorns 收尾(自建 power) — 待 |
| 现金垫 | Power | 2 | Rare | 2 屏障;每回合开始 +1 屏障 | Self | **Medium** | 财务引擎(自建 power) — 待 |

★ 防守垫步：原设计「本回合已出技能则9」。0.107.1 无 Cheap 的「本回合出牌数」环境读取（只有遗物/power 用钩子自己计数），
故改写为等价的 Cheap 自身状态条件「若你已有格挡」。不是 power，仍 Cheap。

</details>

## 嘟嘟 cards — multi-hit combo / agile / 干饭(嘴馋) flavor

Identity: 多段连击 — play more cards → more hits. Fragile (65 HP); cheap cantrips + draw fuel the combo, Dexterity
+ block survive. Flavor = CC 嘴馋/干饭. 3 Medium signatures: 嘟嘟逼人(combo 引擎), 炫饭(随出牌数缩放), 光盘行动(收尾).

| 中文名 | 类型 | 费 | 稀有 | 效果(确定性原语) | 目标 | 可行性 | flavor / 状态 |
|---|---|---|---|---|---|---|---|
| 嘟嘟轻击 | Attack | 1 | Starter | 造成 5 伤害 | AnyEnemy | Cheap | ✅ 起手 ×5 |
| 嘟嘟轻避 | Skill | 1 | Starter | 获得 4 格挡 | Self | Cheap | ✅ 起手 ×4 |
| 嘟嘟灵巧 | Skill | 0 | Starter | 获得 1 敏捷 | Self | Cheap | ✅ 起手 ×1(签名) |
| 嘴馋 | Skill | 0 | Common | 抽 1 张牌 | Self | Cheap | ✅ |
| 戳一口 | Attack | 0 | Common | 造成 3 伤害 | AnyEnemy | Cheap | ✅ 0 费攒连击 |
| 两口吞 | Attack | 1 | Common | 造成 3 伤害 ×2 | AnyEnemy | Cheap | ✅ |
| 护食 | Skill | 1 | Common | 5 格挡 + 1 敏捷 | Self | Cheap | ✅ |
| 假装分你 | Attack | 1 | Common | 4 伤害 + 1 易伤 | AnyEnemy | Cheap | ✅ |
| 加餐 | Skill | 1 | Common | 抽 2 张牌 | Self | Cheap | ✅ |
| 抱紧饭碗 | Skill | 1 | Common | 获得 7 格挡 | Self | Cheap | ✅ 生存 |
| 开胃 | Power | 1 | Uncommon | 获得 2 敏捷 | Self | Cheap | ✅ |
| 边吃边看 | Skill | 1 | Uncommon | 6 格挡 + 抽 1 | Self | Cheap | ✅ |
| 抢菜 | Skill | 1 | Uncommon | 抽 2 + 1 能量 | Self | Cheap | ✅ |
| 风卷残云 | Attack | 2 | Uncommon | 造成 6 伤害 ×3 | AnyEnemy | Cheap | ✅ 固定多段 |
| 大快朵颐 | Attack | 2 | Uncommon | 4 伤害 ×2 + 抽 1 | AnyEnemy | Cheap | ✅ |
| 横扫自助 | Attack | 2 | Uncommon | 全体 5 伤害 + 1 易伤 | AllEnemies | Cheap | ✅ AoE |
| 边吃边抢 | Attack | 1 | Uncommon | 6 伤害;**若你有敏捷则 +1 能量** | AnyEnemy | Cheap | ✅ 条件改写★ |
| 大餐 | Skill | 2 | Uncommon | 回复 10 生命 | Self | Cheap | ✅ 吃顿好的 |
| 吃饱不怕 | Skill | 2 | Rare | Intangible 1 回合 | Self | Cheap | ✅ |
| 报复性进食 | Attack | 3 | Rare | 造成 4 伤害 ×5 | AnyEnemy | Cheap | ✅ 暴风干饭 |
| 越吃越快 | Power | 2 | Rare | 每回合开始抽 1 并获得 1 能量 | Self | **Medium** | ⬆ bump:回合开始钩子无现成 power(自建) — 待 |
| 停不下嘴 | Power | 3 | Rare | 本回合每打出第 4 张牌,+1 能量并抽 1 | Self | **Medium** | ⬆ bump:出牌计数钩子(自建) — 待 |
| 嘟嘟逼人 | Power | 1 | Uncommon | 本回合每第 3 张牌,下张攻击额外命中 1 | Self | **Medium** | combo 引擎(自建) — 待 |
| 炫饭 | Attack | 1 | Uncommon | 命中次数 = 本回合已出牌数 | AnyEnemy | **Medium** | 随出牌数缩放(自建) — 待 |
| 光盘行动 | Attack | 2 | Rare | 每本回合出过一张牌命中 1 次 | AnyEnemy | **Medium** | 干饭收尾(自建) — 待 |

★ 边吃边抢：原设计「本回合≥3张牌则1能量」。同上无 Cheap 出牌数读取，改写为贴合嘟嘟主题的「若你有敏捷则+1能量」
（GetPower&lt;DexterityPower&gt;，Cheap）。

## 双人连携 / synergy — GATED on co-op proof PASS; deterministic only

墩墩 做饭 → 嘟嘟 干饭：墩墩出做饭/养生牌 → 嘟嘟被「喂」(heal / +力量·敏捷 / 退连击)；嘟嘟干饭「消耗」墩墩的格挡/荆棘
换额外连击。双向、确定性、贴主题。机制桥：嘟嘟 combo/出牌数 ↔ 墩墩 Thorns/block。先验现成协同 power
(FriendshipPower / TagTeamPower / CoordinatePower / SynchronizePower / DieForYou) — 合适则 Medium→Cheap。

两档：**互喂(Cheap, 低风险)** = AnyAlly 给队友一个确定性命令(沿用已验 spike 套路)；**「在一起」共鸣(Medium, 最依赖联机
实证)** = 读/响应队友行动。Phase 5 顺序：先互喂，再共鸣（待 Phase 2 PASS）。

| 中文名 | 类型 | 费 | 池 | 效果(确定性) | 目标 | 可行性 | 备注 |
|---|---|---|---|---|---|---|---|
| 在一起·守护 | Skill | ? | 墩墩 | 给队友 8 格挡 | AnyAlly | Cheap | ✅ spike,已编译验证(待联机实证) |
| 守护自己 | Skill | 0 | 无色 | 给自己 8 格挡 | Self | Cheap | ✅ spike(单机验卡逻辑) |
| 喂你一口 | Skill | 1 | 墩墩 | 给队友回复 6 生命 | AnyAlly | Cheap | 互喂 — 待 P5 |
| 给你加餐 | Skill | 1 | 墩墩 | 给队友 2 敏捷 | AnyAlly | Cheap | 互喂 — 待 P5 |
| 分你一块 | Skill | 1 | 嘟嘟 | 给队友 6 格挡 | AnyAlly | Cheap | 互喂 — 待 P5 |
| 喂你吃刺 | Skill | 1 | 嘟嘟 | 给队友 3 荆棘 | AnyAlly | Cheap | 互喂(超搭墩墩) — 待 P5 |
| 你也尝尝 | Attack | 1 | 嘟嘟 | 5 伤害;给队友回复 3 | AnyEnemy+AnyAlly | Cheap | 互喂 — 待 P5 |
| 在一起 | Power | 1 | 无色 | 双方本回合都出过牌→各 4 格挡 | both | Medium | 共鸣签名(gated) |
| 你做我吃 | Power | 2 | 无色 | 队友获得格挡→你下张攻击额外命中 1 | both | Medium | 共鸣核心(gated) |
| 心有灵犀 | Skill | 1 | 无色 | 队友本回合出过牌→抽 2 + 1 能量 | Self(读队友) | Medium | 共鸣(gated) |
| 双人连携 | Power | 2 | 无色 | 队友每出一张攻击牌→你获得 2 格挡 | both | Medium | 共鸣(gated) |

## Backlog — raw ideas (dump anytime; CC triages into tables above)

-

## Open questions

- 数值全是 v0 粗平衡，待单机 + playtest 校准（尤其墩墩 Thorns 对 boss 的速度、嘟嘟脆度）。
- 8 张 Medium（自建 power）+ synergy 共鸣，是下一批；synergy build 待 Phase 2 联机实证 PASS。
