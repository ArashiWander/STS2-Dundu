# 墩墩 BUILD PLAN — full adoption of CC's design

Decision: 墩墩 is rebuilt around CC's complete design (incl. all three tribute
archetypes 残影 / 蓄力 / 熟悉). This **supersedes** the earlier Thorns-retaliation
墩墩. Source of truth for cards / flavor / numbers: `docs/dundun-design.md` (CC's
doc — commit it to the repo). This file is the engineering layer on top: feasibility,
dependency order, MP-safety, build phases. **CC validates every power mapping against
the 0.107.1 decompile before building** — names/effects below are intent, not verified
API.

## Fate of the 18 already-built Cheap 墩墩 cards

The earlier Thorns/heal pool is mostly superseded. **Do NOT delete blindly.** Triage:
- **Salvage (reskin, keep):** generic block / heal / damage / draw cards with NO
  Thorns and NO 闷气 dependency can be re-flavored into 猪饲料 / 麦霸 / 兴趣 entries
  (e.g. 滋补汤 / 对冲 / 养生 / 稳扎一拳 / 二段蹬腿). Map them onto the new design,
  don't re-invent.
- **Out:** the Thorns engine (反手回击 / 见招拆招 / 倒刺护甲) and the planned 以彼之道.
- Produce a salvage/cut list for Yemin **before** removing anything.

## Custom systems & feasibility (the expensive core)

Five custom powers + a card-tag system underpin everything. Confirm exact tier vs the
decompile; rough estimate:

- **闷气值 Sulking (0–10)** — the core. Stack power + tiered passive: 分层改伤
  (ModifyDamage by tier = Medium); 6–8 免疫负面 (hook status application ≈ Harmony);
  9 回合开始穿透自伤 / 10 回合末穿透 + 失去技能牌打出权 (turn hooks + card-play
  restriction ≈ Harmony). **Medium→Expensive. Biggest single piece; may split.**
- **熬大夜 debuff** — −1 to next block card per stack (ModifyBlock + consume),
  −1/turn decay, ≥3 → +1 闷气 at turn start. **Medium.**
- **残影 (太空步)** — consume 1 stack to fully negate an attack's damage (any size),
  clears at turn end. Needs pre-damage intercept hook. **Medium→Expensive.**
- **蓄力 (巨剑)** — +1 at turn end if no attack card played this turn; persists across
  turns; spent by spender cards. Needs "attack-played-this-turn" tracking + turn-end
  hook. **Medium.**
- **熟悉 (格斗)** — +1 on unblocked damage taken, permanent (no decay), +1 attack
  dmg per stack. Needs unblocked-damage hook + ModifyDamage. **Medium.**
- **Card-tag system** — relics/cards key off 「猪饲料 系」and 「麦霸 系」(小白菜 / 香烟
  key off 猪饲料; 安可 checks 麦霸 series). Need a custom card-category tag cards carry
  and effects query. **Medium, foundational.**

## MP-safety (墩墩 is one of two co-op characters)

Every custom power above holds per-player state in co-op. Build each one **MP-safe from
day one** on BaseLib's synced/persisted state — no client-local counters, no
client-local RNG. Random-target cards (机械键盘 / 麦霸附体) must use the game's synced
RNG. Full co-op validation of all this is **gated on the Phase 2 co-op proof PASS** (P8
below). Flag any client-local assumption as you go.

## Determinism guardrail (cross-character 情侣 cards)

Section D (墩嘟假嘟 / 嘟嘟的反击 / 一起打游戏) interacts across 墩墩 ↔ 嘟嘟 → these are
synergy cards: **deterministic only, MP-safe, gated on the co-op proof.** They join the
连携 track, not the single-player phases.

## Proposed build order (CC validates/adjusts vs real API)

Foundation first; each phase single-player-testable except P8.

- **P1 — 闷气值 core** (stacks + tiered damage scaling + tier states 0–10). Verify via
  a test card: gain 闷气, see damage scale at 3–5 / 6–8, see 9/10 penalties fire. May
  split P1a (stacks + scaling) / P1b (immunity + restrictions via Harmony).
- **P2 — 熬大夜 debuff** + the 闷气 link (≥3 → +1/turn-start).
- **P3 — card-tag system + starters (大屁墩/墩坚强) + 猪饲料 (B) + 麦霸 (E) + 兴趣 (C).**
  The bulk of a playable 墩墩; most effects Cheap once 闷气 + tags exist. Reskin
  salvaged cards here.
- **P4 — 残影 power + 太空步 (F).**
- **P5 — 蓄力 power + 巨剑 (G).**
- **P6 — 熟悉 power + 格斗 (H).**
- **P7 — relics (玩偶遗物).** After the powers/tags they key off (闷气/猪饲料/熬大夜).
  Respect the档位 rule in CC's doc (初始保底 / 普通平稳 / 罕见中等条件 / Boss 才碰资源
  类). 香烟 is curse/event-only, not the regular pool.
- **P8 (GATED on co-op proof) — 情侣 cards (D) + MP-safety audit of all custom powers +
  RNG-sync check + co-op playtest.**

## What CC does now (re-plan, DON'T build yet)

1. Read `docs/dundun-design.md` + this file.
2. Map EVERY effect / keyword / relic to real 0.107.1 powers/commands against the
   decompile (穿透 → HP-loss; 人工制品 / 迟钝 / 毒; ModifyDamage / ModifyBlock hooks;
   status-application hook; card-play restriction; card tags). Cite class + namespace.
3. Confirm the real feasibility tier of each custom system; flag anything that turns out
   Expensive or blocked.
4. Produce the 18-card salvage/cut list.
5. Draft 墩墩 SPEC (custom-power designs + MP-safe approach) + phased TASKS from the
   order above (adjust to reality). Update the repo's Cards.md 墩墩 section to the new
   design.
6. Commit `墩墩 redesign: plan`, push, **STOP for approval. Build nothing yet.**