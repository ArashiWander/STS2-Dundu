# Dundun SPEC — redesign plan (CC's design, validated vs 0.107.1)

Engineering plan for rebuilding 墩墩 around CC's full design (情绪天平 + 残影/蓄力/熟悉 archetypes),
**superseding** the Thorns-retaliation 墩墩. Source of truth for flavor/numbers: `墩墩角色Mod设计文档1.md`.
Build order: `Dundun Build Plan.md`. This file = the API mapping + feasibility + salvage list + custom-power
designs + adjusted TASKS. **Every mapping below was checked against the 0.107.1 decompile** (real class +
namespace + a reference power that already does it). **Plan only — no code until approved.**

Namespaces (cited once): powers = `MegaCrit.Sts2.Core.Models.Powers`; commands = `MegaCrit.Sts2.Core.Commands`;
cards/tags/targeting = `MegaCrit.Sts2.Core.Entities.Cards`; vars = `MegaCrit.Sts2.Core.Localization.DynamicVars`;
`ValueProp` = `MegaCrit.Sts2.Core.ValueProps`; BaseLib bases = `BaseLib.Abstracts` (`CustomPowerModel`,
`CustomCardModel`, `CustomRelicModel`).

---

## 1. Headline finding — the whole design is Cheap/Medium. **Zero Expensive. Nothing blocked.**

The Build Plan feared several pieces would need Harmony (Expensive). They don't: the game exposes **PowerModel
hooks** for every one of them. Confirmed by reading powers that already use each hook:

| Mechanic the plan feared was Harmony | Real mechanism (PowerModel hook) | Reference power that uses it | Tier |
|---|---|---|---|
| 闷气 6–8 **免疫负面** | `TryModifyPowerAmountReceived(...)` → zero incoming Debuff amount | **`ArtifactPower`** (exact pattern) | Medium |
| 闷气 10 **失去技能牌打出权** | `ShouldPlay(card)` → false for `CardType.Skill` (+ `BeforeCardPlayed`) | **`ChainsOfBindingPower`**, `StranglePower` | Medium |
| 残影 **完全否定一次攻击** | `ModifyHpLostAfterOsty(...)` → if stacks>0 & amount≥1, consume 1, return 0; `ModifyDamageCap` for previews | **`IntangiblePower`** (caps to 1 — we negate) | Medium |
| 闷气 **分层改伤** (+50% / ×2) | `ModifyDamageMultiplicative(float)` keyed on tier | **`KnockdownPower`**, `HangPower` | Medium |
| 蓄力 / 机械键盘 **「本回合出了N张/攻击牌」** | `AfterCardPlayed(ctx, CardPlay)` counter, reset on turn | **`DiamondDiadem`** relic (per-turn card counter); `ArtOfWar` relic reads `attacksPlayedThisTurn` | Medium |

So: **all 5 custom powers + the tag system are Medium; 穿透/迟钝/标签 are Cheap.** No Harmony anywhere in the v1
design. (If a future tweak needs a hook that turns out absent, *then* it's Expensive — flagged at build, not now.)

---

## 2. API mapping — every effect / keyword / status (real class + namespace)

### 2.1 Damage / block / heal / draw / energy primitives — **Cheap** (per capability-catalog.md)
- 伤害 `DamageCmd.Attack(var).FromCard(this).Targeting(t).Execute(ctx)`; 多段 `.WithHitCount(n)`; AoE
  `.TargetingAllOpponents(base.CombatState)`. 格挡 `CreatureCmd.GainBlock`. 回复 `CreatureCmd.Heal`.
  抽牌 `CardPileCmd.Draw`. 能量 `PlayerCmd.GainEnergy`.

### 2.2 穿透伤害 (闷气 9/10 自伤；机械键盘) — **Cheap**
- **`ValueProp.Unblockable`** (`MegaCrit.Sts2.Core.ValueProps`) — confirmed used by powers/cards. Deal
  block-ignoring damage via `CreatureCmd.Damage(ctx, target, amount, ValueProp.Unblockable, dealer, card)`
  (the `decimal amount, ValueProp props` overload). Self-damage = target `Owner.Creature`.

### 2.3 Status / debuffs (existing powers — **Cheap**, apply via `PowerCmd.Apply<T>`)
| 设计里的状态 | 真实 power |
|---|---|
| 毒 (干煸肥肠) | `PoisonPower` |
| 虚弱 / 易伤 | `WeakPower` / `VulnerablePower` |
| 迟钝 (Strogonoff 下回合) | **`SlowPower`** (exists — maps directly; confirm its exact effect text in P2) |
| 人工制品 (银色山泉转化) | `ArtifactPower` (negates next debuff) |
| 牛排「下张攻击 +50%」 | `VigorPower` (next attack bonus dmg) or a 1-turn custom — Cheap via existing `VigorPower` |
| 迟钝/虚弱免疫等基础 buff | `BarricadePower` / `BufferPower` / `RegenPower` / `IntangiblePower` (all exist) |

### 2.4 Custom resources → **Medium** (one `CustomPowerModel` each; hooks cited)
| 资源 | StackType | 核心钩子 | 参照 |
|---|---|---|---|
| **闷气 Sulking 0–10** | `Counter` | `ModifyDamageMultiplicative` (分层改伤) · `TryModifyPowerAmountReceived` (6–8 免疫负面) · `ShouldPlay` (10 封技能) · `BeforeTurnStart` (9 穿透自伤 / 熬大夜≥3 入气 / 10 末穿透) · `AfterCardPlayed` (主动喂养计) | Artifact + Knockdown + ChainsOfBinding |
| **熬大夜** | `Counter` (decays) | `ModifyBlockAdditive` (下张格挡 −1/层) · `AfterSideTurnEnd`/`TickDown` (−1/回合) · `BeforeTurnStart` (≥3 → +1 闷气) | NoDrawPower (decay), Knockdown |
| **残影 Afterimage** | `Counter` | `ModifyHpLostAfterOsty` (有层→消1层返0) · `ModifyDamageCap` (预览) · `AfterSideTurnEnd` (回合末清空) | **IntangiblePower** (almost identical shape) |
| **蓄力 Charge** | `Counter` (persists) | `AfterCardPlayed` (记本回合是否出攻击) · `AfterTurnEnd` (没出攻击→+1) · spender card reads `power.Amount` | DiamondDiadem + ArtOfWar |
| **熟悉 Familiar** | `Counter` (no decay) | post-HP-loss hook (未格挡受伤→+1) · `ModifyDamageAdditive` (+1 攻击/层) | (HP-loss observe) + StrengthPower-style add |

> Note: 残影 vs the game's `AfterimagePower` — **different mechanic** (game's = gain block on card play). Ours
> is a damage-negation counter; we author our own (don't reuse the name's power).

### 2.5 卡牌标签系统 (猪饲料系 / 麦霸系) — **Cheap/Medium, foundational, native**
- **`ModCardTagRegistry.For("DUNDUNDUDU").RegisterOwned("SNACK")`** returns a `CardTag` — confirmed: YuWanCard
  does exactly this (`...For("YUWANCARD").RegisterOwned("FOOD_PIG")`). Register `SNACK` (猪饲料) + `KARAOKE`
  (麦霸). Cards carry it via `CanonicalTags`; effects/relics query `card.Tags.Contains(theTag)`. (香烟/小白菜
  key off `SNACK`; 安可 keys off `KARAOKE`.)

### 2.6 Relics — all **Cheap/Medium** (`CustomRelicModel`, hooks mirror the powers)
- 卡皮吧啦 (回合末剩余能量→格挡): relic `BeforeSideTurnEnd` + read `player.Energy` + `CreatureCmd.GainBlock`.
- 大鼠与美叽 (同费2张→抽1): `AfterCardPlayed` track cost counts. (DiamondDiadem pattern.)
- 比比拉不 (同回合攻击+格挡牌→格挡): `AfterCardPlayed` track card types this turn.
- 土豆 / 小白菜 / 香烟 / 小粉: key off 闷气 events / `SNACK` tag / 熬大夜 — all via the powers' public state +
  relic hooks (`AfterApplied` on 闷气 power, etc.). 小熊虫/Jellycat (致命免疫) → there's a fatal-prevention hook
  (`ShouldOwnerDeathTriggerFatal`, seen on PowerModel; relics have an equivalent — confirm exact name in P7).
- **Tier rule** (CC's doc): 初始 保底 / 普通 平稳无条件 / 罕见 中等条件 / Boss 才碰资源(能量/大额). 香烟 =
  curse/event-only, never the regular pool.

---

## 3. Feasibility verdict

- **Cheap:** all damage/block/heal/draw/energy, 穿透(`Unblockable`), 毒/虚弱/易伤/迟钝/人工制品 (existing powers),
  card-tag registration + queries.
- **Medium:** the 5 custom powers (闷气/熬大夜/残影/蓄力/熟悉) and the relics — each is one `CustomPowerModel` /
  `CustomRelicModel` using documented hooks. 闷气 is the biggest (5 hooks) → **split P1a/P1b**.
- **Expensive / blocked:** **none.** (Watch-item: if 残影's `ModifyHpLostAfterOsty` interacts badly with the
  game's block-vs-HP order on *multi-hit* attacks, the negation may need per-hit care — verify in P4 with a
  test, not a blocker.)
- **MP-safety:** every power is a per-creature `PowerModel` instance whose Amount/state lives in combat state
  (synced by `CombatStateSynchronizer`); use `GetScaledAmountForMultiplayer` for any player-count scaling
  (ArtifactPower pattern); **no `static` counters, no `System.Random`** — random-target cards (机械键盘 /
  麦霸附体) use the game RNG (`RunManager...RunRngSet`/combat rng). Cross-player 情侣 cards (§5 D) → synergy
  track, deterministic, gated on the co-op proof.

---

## 4. 18-built-card salvage / cut list (DELETE NOTHING until Yemin confirms)

| Built card | 效果 | 处置 | 映射到新设计 |
|---|---|---|---|
| 反手回击 (3 Thorns) | Thorns | **CUT** | Thorns engine out |
| 见招拆招 (Thorns +2) | Thorns | **CUT** | Thorns engine out |
| 倒刺护甲 (Thorns+block) | Thorns | **CUT** | Thorns engine out |
| 稳扎一拳 (7 dmg) | attack | **SALVAGE** | → 五音不全也要唱 (E, 攻击) base |
| 二段蹬腿 (14 dmg) | big attack | **SALVAGE** | → 高音炫技 / 野兽之力 (E/H) base |
| 防守垫步 (6/9 cond block) | cond block | **SALVAGE** | → 限定版手办上架 (C, 条件格挡) |
| 对冲 (10 block + heal3) | block+heal | **SALVAGE** | → 长款皮衣 / 兴趣 sustain |
| 养生 (heal4 + Regen) | heal | **SALVAGE** | → 一个人的卡拉OK (E, 回血) base |
| 满汉全席 (15blk+heal8+regen) | big sustain | **SALVAGE** | → 猪饲料 大餐 / 满汉全席 (B) |
| 滋补汤 (block+Regen) | block+heal | **SALVAGE** | → 猪饲料 / 兴趣 |
| 储备 (block+draw) | block+draw | **SALVAGE** | → 深夜单曲循环 / 兴趣 |
| 抓握点 (0c 3 block) | cheap block | **SALVAGE** | → 关起门来吼两嗓子 (E) base / cheap block |
| 缠斗 (2 Weak) | Weak | **SALVAGE** | → 麦霸/兴趣 control utility |
| 风控 (1 Buffer) | Buffer | **SALVAGE (opt)** | Buffer kept only if we keep a survival subset |
| 连续防守 (AoE Weak+block) | AoE | **SALVAGE** | → 副歌大合唱 (E, AoE) base |
| 铜墙铁壁 (Barricade) | Barricade | **CUT (opt)** | Barricade-tank identity gone — drop unless we want a rare defensive |
| 心如止水 (Intangible) | Intangible | **CUT (opt)** | 残影 replaces the "negate damage" niche |
| 文火慢炖 (3 Regen power) | Regen | **CUT (opt)** | Regen not in new design |

**Summary:** hard-CUT 3 (Thorns). SALVAGE 12 (reskin in P3, drop their old conditions, add 闷气/tag clauses once
those exist). Optional-CUT 3 (Barricade/Intangible/Regen — keep only if we want a small generic-survival subset).
The starter pair (墩墩猛击/防御) stays as the basis for 大屁墩!/墩坚强. **All salvage = deterministic, single-player
safe — no 闷气 dependency added until P3.**

---

## 5. Custom-power SPEC (the 5 + tag system) — designs, hooks, MP-safe

All extend `CustomPowerModel : PowerModel` (`BaseLib.Abstracts`), `[ICustomPower]` for icons, registered by
reflection. State = the power's `Amount` (+ small internal fields) held in combat state → synced. Built MP-safe
from day one.

1. **`SulkingPower` (闷气, Counter 0–10).** `Amount` clamped 0–10. Tier from `Amount`:
   - `ModifyDamageMultiplicative`: tier 3–5 → ×1.5; 6–8/9 → ×2.0; else ×1.0 (only when this creature deals).
   - `TryModifyPowerAmountReceived`: if tier∈[6,9] and incoming is `Debuff` & visible → modifiedAmount=0 (免疫).
   - `ShouldPlay(card)`: if `Amount==10` and `card.Type==Skill` → false (封技能). (Reset the lock condition each
     turn per the doc's "下回合".)
   - `BeforeTurnStart`: if 熬大夜≥3 → +1 闷气; if `Amount==9` → `CreatureCmd.Damage(self, 1, Unblockable)`;
     if `Amount==10` → end-of-turn 3 穿透 (do in `AfterTurnEnd`).
   - Gain API: a static helper `Sulking.Add(player, n)` cards call; cap-at-2-per-turn for the "受伤+1" path via
     an `AfterApplied`/damage-observe counter reset each turn.
   - MP: per-creature; no player-count scaling needed (闷气 is personal).
2. **`SleepDeprivedPower` (熬大夜, Counter, decays).** `ModifyBlockAdditive` → −Amount on the *next* block card
   (track "used this stack"); `AfterSideTurnEnd` → −1; `BeforeTurnStart` → if Amount≥3, Sulking.Add(owner,1).
   `Type=Debuff`.
3. **`AfterimageStacksPower` (残影, Counter).** `ModifyHpLostAfterOsty`: if Amount>0 && amount≥1 → consume 1,
   return 0 (negate the whole hit, any size); `ModifyDamageCap` mirror for previews; `AfterSideTurnEnd` →
   clear to 0 (no carry-over).
4. **`ChargePower` (蓄力, Counter, persists).** `AfterCardPlayed` → if `cardPlay.Card.Type==Attack` set a
   per-turn "attacked" flag; `AfterTurnEnd` → if not attacked, Amount+1; flag reset on turn start. Spender cards
   read `owner.GetPower<ChargePower>()?.Amount` then `PowerCmd.Remove`/zero it. (`ArtOfWar` confirms
   `attacksPlayedThisTurn` is also readable — either source works.)
5. **`FamiliarPower` (熟悉, Counter, no decay).** observe unblocked damage (post-`ModifyHpLostAfterOsty`/an
   after-damage hook) → +1; `ModifyDamageAdditive` → +Amount when owner deals attack damage. Permanent within
   the combat.
6. **Card tags.** `static readonly CardTag Snack = ModCardTagRegistry.For("DUNDUNDUDU").RegisterOwned("SNACK");`
   (+ `Karaoke`). Cards: `CanonicalTags => [Snack]`. Queries: `card.Tags.Contains(Dundun.Snack)`.

---

## 6. TASKS — phased build (adjusted to the verified API; each P single-player-testable except P8)

- **P1a — 闷气 stacks + 分层改伤** (`SulkingPower`: Counter 0–10, `ModifyDamageMultiplicative` tiers; gain helper;
  one test card). Verify dmg scales at 3–5 / 6–8.
- **P1b — 闷气 罚则**: `TryModifyPowerAmountReceived` (6–8 免疫), `ShouldPlay` (10 封技能), `BeforeTurnStart`/
  `AfterTurnEnd` 9/10 穿透 (`Unblockable`). Verify each tier fires.
- **P2 — 熬大夜** (`SleepDeprivedPower`) + 闷气 link (≥3 → +1/turn-start).
- **P3 — 卡标签系统 (SNACK/KARAOKE) + starters (大屁墩/墩坚强) + 猪饲料(B) + 麦霸(E) + 兴趣(C).** Reskin the 12
  salvaged cards here; add their 闷气/tag clauses now that the power+tags exist. Bulk of a playable 墩墩.
- **P4 — 残影** (`AfterimageStacksPower`) + 太空步(F). Verify negate + multi-hit interaction (the §3 watch-item).
- **P5 — 蓄力** (`ChargePower`) + 巨剑(G).
- **P6 — 熟悉** (`FamiliarPower`) + 格斗(H).
- **P7 — 玩偶遗物.** After the powers/tags they key off. Respect the 档位 rule. 香烟 = curse/event-only.
- **P8 (GATED on co-op proof PASS) — 情侣牌(D) + MP-safety audit of all 5 powers + RNG-sync check (机械键盘/
  麦霸附体) + co-op playtest.**

Each P ends runnable + smoke-clean (tools/smoke.ps1) + commit `墩墩 PN: …`. Riskiest first inside the char (闷气
core P1). Build nothing until Yemin approves this plan.
