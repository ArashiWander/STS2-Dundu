# CARDS.md — content design & capture

Container for card / relic / event ideas → feasibility triage → CC implements per
phase. This is where Yemin's card ideas get captured (in the elicitation waves, or
dumped anytime in the Backlog). CC builds from the triaged tables here, not from
chat scrollback.

## Conventions (confirmed from the build so far)

- **v1 effects are deterministic only for any cross-player (synergy) card.** No
  RNG / card-draw-that-shuffles / random buffs on a card that targets a partner —
  those desync the lockstep co-op. Single-character cards may use draw/RNG; synergy
  cards (target the partner) stay deterministic: fixed block, fixed Strength/Dex,
  fixed numeric buff. (SPEC §12)
- **Target types available:** `Self`, `AnyAlly` (any player incl. partner, excl.
  self), `AnyEnemy`, `AnyPlayer`. Synergy uses `AnyAlly`.
- **Every card needs a `[Pool]`** (a colorless pool, or the character's own pool) or
  it throws at runtime (compile-time it's only a warning).
- **Localization keys:** `DUNDUNDUDU-<SLUG>.title` / `.description`, flat keys, in
  `cards.json` under the `eng` + `zhs` lang dirs, packed into the pck
  (`pck_name == DundunDudu`, case-sensitive). Chinese (`zhs`) is the real text.
- **Characters:** 墩墩 = tanky / block / sturdy (warm orange). 嘟嘟 = agile / combo /
  cheap-card setup (contrast color). The synergy bridges the two.

## Feasibility legend (filled against CC's capability catalog)

- **Cheap** — expressible with an existing game power/command (e.g. `GainBlock`,
  apply Strength). Fast.
- **Medium** — needs a small custom power/keyword.
- **Expensive** — needs a Harmony patch / new system. Flag before committing.

## 墩墩 cards — tank / block

| 中文名 | 类型 | 费 | 稀有 | 效果(确定性原语) | 目标 | 连携? | 可行性 | 备注 |
|---|---|---|---|---|---|---|---|---|
| 墩墩猛击 | Attack | 1 | Starter | 造成 6 伤害 | AnyEnemy | – | Cheap | 已建(起手 ×5) |
| 墩墩防御 | Skill | 1 | Starter | 获得 5 格挡 | Self | – | Cheap | 已建(起手 ×5) |
| 在一起·守护 | Skill | ? | – | 给队友 8 格挡 | AnyAlly | ✓ | Cheap | spike 验证卡 |
| _(待填)_ |  |  |  |  |  |  |  |  |

## 嘟嘟 cards — agile / combo / cheap setup

| 中文名 | 类型 | 费 | 稀有 | 效果(确定性原语) | 目标 | 连携? | 可行性 | 备注 |
|---|---|---|---|---|---|---|---|---|
| _(Phase 4 起手牌建好后回填)_ |  |  |  |  |  |  |  |  |

## 双人连携 / synergy — GATED on co-op proof PASS; deterministic only

The couple mechanic. Direction sketch: 墩墩 堆格挡/扛伤 ↔ 嘟嘟 多张低费铺连击.
「在一起」candidates: 嘟嘟连击数 → 墩墩格挡转化, or 墩墩格挡 → 嘟嘟加伤. Bidirectional,
deterministic. Filled in elicitation Wave 2.

| 中文名 | 触发 | 效果(确定性) | 目标 | 可行性 | 备注 |
|---|---|---|---|---|---|
| _(待 Wave 2)_ |  |  |  |  |  |

## Backlog — raw ideas (dump anytime; I triage into the tables above)

-

## Open questions

-