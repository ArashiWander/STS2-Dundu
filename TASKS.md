# TASKS — 墩墩 & 嘟嘟 co-op mod

Phased plan. Riskiest unknowns first (toolchain, then co-op sync) per CLAUDE.md and the
discovery brief. Each phase ends **runnable/testable**. "Done when" is the gate; tick the
boxes, commit `Phase N: <summary>`, push, report, **STOP for the next prompt**. See SPEC.md
for the API/design each phase implements.

Legend: **[agent]** = I can do it headless. **[user]** = needs Yemin (in-game / live co-op /
two instances). **[ask]** = a decision in SPEC §11 must be settled first.

---

## Phase 0 — Discovery + plan  ✅ (this phase; awaiting approval)

- [x] Inventory repo, toolchain, installed mods, game/BaseLib/Godot/dotnet versions
- [x] Extract real registration + net-message API from `sts2.xml` + decompiled BaseLib/YuWanCard
- [x] Confirm build/publish workflow + `.pck`/MegaDot requirement from the template
- [x] Design the MP-safe 双人连携 mechanic (Routes A/B, desync failure modes)
- [x] Evaluate the STS2 Modding Assistant MCP
- [x] Empty-mod **dll build green** (`SmokeMod.dll` compiled + staged to scratch)
- [x] Draft SPEC.md + TASKS.md

**Done when:** SPEC.md + TASKS.md committed; report delivered. **→ STOP for `go`.**

---

## Phase 1 — Toolchain + empty-mod smoke test (finish the loop in-game)

Goal: a trivial mod loads in the real game via "Load with Mods" without breaking it.

- [ ] **[ask]** Settle: one-mod (SPEC §11.1), MegaDot-now-vs-PckPacker-first (§11.4)
- [ ] **[agent]** Scaffold the real project `DundunDudu` from `alchyrsts2mod` into the repo
      (set `ModAuthor=Yemin`, `Sts2Path` in `Directory.Build.props`)
- [ ] **[agent]** Decide `has_pck`: for a *code-only* smoke mod set `has_pck:false` to avoid a
      missing-`.pck` load warning; OR try the `PckPacker` NuGet to emit a trivial `.pck`
- [ ] **[agent]** `dotnet build` → confirm `.dll`/`.json` land in `…/Slay the Spire 2/mods/DundunDudu/`
- [ ] **[user]** Launch StS2 → "Load with Mods" → confirm the mod loads, logs its init line,
      and the game reaches the main menu / character select with no crash
- [ ] **[user]** (if pursuing assets early) install **MegaDot v4.5.1**, set `GodotPath`, and
      confirm `dotnet publish` exports a `.pck`

**Done when:** the empty mod is listed/loaded in-game with no crash, and BaseLib loads as its
dependency. (dll build is already proven; this phase closes the *in-game load* gap.)
**Risks:** Steam D:-library path discovery; MegaDot availability; loader behavior on a
`has_pck:true` mod with no real `.pck`.

---

## Phase 2 — Co-op cross-player spike (THE de-risking gate)

Goal: prove a card one player plays applies a correct effect to the *other* player with **no
desync**. Nothing downstream is trustworthy until this is green.

- [ ] **[ask]** Approve Route A primary / Route B fallback (SPEC §11.2)
- [ ] **[agent]** Add one trivial test card to a temporary pool: "墩墩 plays → 嘟嘟 gains 1 Block"
- [ ] **[agent]** Implement via **Route A** (synchronized card `GameAction` + BaseLib any-player
      targeting). Keep effect fully deterministic (no `System.Random`/`DateTime`)
- [ ] **[agent]** Add a debug log + an on-screen tell so the effect is visually observable
- [ ] **[user]** Host a 2-player lobby (two instances / two machines, identical mod+BaseLib);
      play the card; confirm 嘟嘟 gains Block, and **no `StateDivergenceMessage`** over ≥3 turns
- [ ] **[agent]** If Route A can't apply to the partner cleanly: implement **Route B**
      (`CoupleBuffMessage : ICustomMessage` + `CustomMessageWrapper.Send`, `HandleMessage`
      enqueues the action, `ShouldBuffer=true`) and re-test
- [ ] **[agent]** Record the working mechanism + the desync modes actually observed in SPEC §6

**Done when:** in a live 2-player lobby, the cross-player effect is correct and reproducible
with zero divergence across several turns, on a named route (A or B). **If it can't be made
MP-safe: STOP and report — it reshapes the mechanic.** (3-strikes rule applies.)

---

## Phase 3 — 墩墩 character skeleton, playable solo

Goal: 墩墩 is selectable and a full solo run is playable with a starter deck.

- [ ] **[agent]** Read the `alchyrsts2charmod` Character template + YuWanCard to pin exact
      `CharacterModel` overrides (HP, `MaxEnergy`, `StartingDeck`, card pool, portraits, site art)
- [ ] **[agent]** `CustomCharacterModel` for 墩墩: `StartingHp`, `MaxEnergy`, starter deck,
      `CustomCardPoolModel` (its card color), placeholder portrait/icon paths
- [ ] **[agent]** ~5–6 starter `CustomCardModel`s (basic strike/defend analogues), Chinese text
      via inline `Localization` (L1)
- [ ] **[agent]** `dotnet build`; **[user]** select 墩墩 in a solo run, play through ≥1 combat
- [ ] **[agent]** Verify card pool/rewards show 墩墩 cards; no null-art crashes

**Done when:** 墩墩 appears in character select with Chinese name, starts with the right HP/
energy/deck, and a solo combat is winnable with rewards drawing from his pool.

---

## Phase 4 — 嘟嘟, second character

Goal: 嘟嘟 selectable and solo-playable, mirroring Phase 3, in the same mod.

- [ ] **[agent]** `CustomCharacterModel` + `CustomCardPoolModel` (distinct color) for 嘟嘟
- [ ] **[agent]** Starter deck + ~5–6 cards, inline Chinese text
- [ ] **[agent]** `dotnet build`; **[user]** solo run as 嘟嘟, ≥1 combat
- [ ] **[agent]** Confirm both characters coexist (no id/pool collisions) and both select cleanly

**Done when:** both 墩墩 and 嘟嘟 are independently selectable and solo-playable in one mod
build, with separate card pools/colors and no collisions.

---

## Phase 5 — 双人连携 / 在一起 synergy cards

Goal: the real couple-synergy content, built on the route proven in Phase 2.

- [ ] **[agent]** `在一起` buff as `CustomModifierModel` (the persistent couple buff)
- [ ] **[agent]** A small set of 双人连携 cards per character: each measurably buffs the
      partner (block/strength/energy/draw) via the Phase 2 mechanism
- [ ] **[agent]** Tune so effects no-op gracefully in solo / with a non-paired partner (§6.5)
- [ ] **[agent]** `dotnet build`; **[user]** 2-player lobby: each plays synergy cards, confirm
      the partner is buffed correctly, **no desync**, over a full combat
- [ ] **[agent]** Add the synergy cards to each character's reward pool

**Done when:** in live co-op, both characters' 双人连携 cards correctly buff the *other*
player, the 在一起 buff behaves as designed, and a full combat runs with no divergence.

---

## Phase 6 — Chinese localization + placeholder art pass

Goal: consistent Chinese text everywhere + placeholder art packed into a real `.pck`.

- [ ] **[agent]** Decide L1 inline vs L2 JSON-in-`.pck` based on Phase 3 findings; migrate text
- [ ] **[agent]** Placeholder portraits, card frames, icons, map markers, energy counter colors
- [ ] **[ask/user]** Produce the `.pck`: try `PckPacker` (no editor); if insufficient, install
      **MegaDot v4.5.1** and `dotnet publish`
- [ ] **[user]** In-game pass: every card/character/keyword shows Chinese, no missing-string keys,
      no missing-texture placeholders crash
- [ ] **[agent]** Set final manifest fields (`has_pck:true`, `min_game_version:0.107.1`, version)

**Done when:** a published build (`.dll`+`.pck`+`.json`) loads with all Chinese text resolved
and placeholder art rendering, no localization-key or missing-resource errors.

---

## Phase 7 — Two-player co-op playtest (acceptance)

Goal: the v1 "done" bar — both of you play a full co-op run together.

- [ ] **[user]** Full co-op run: 墩墩 + 嘟嘟, identical mod+BaseLib on both clients
- [ ] **[user]** Exercise 双人连携 repeatedly across acts; watch for desync/softlock
- [ ] **[agent]** Triage any divergence against the SPEC §6 failure-mode list; fix + re-test
- [ ] **[agent]** Tag the v1 build; write README (setup: subscribe BaseLib, install paths,
      "achievements disabled", "both must run identical versions")

**Done when:** the two of you complete a co-op run together with the synergy working and no
desync/softlock — the v1 acceptance bar from CLAUDE.md.

---

## Cross-phase invariants

- Both clients must run **identical** mod + BaseLib versions (co-op serialization safety).
- Effects crossing players must be **deterministic** and **routed through the synchronized
  action queue** — never poke remote state directly (SPEC §6).
- Develop without risking saves; gameplay mods **disable achievements** (expected).
- All in-game text **Chinese only** for v1.
- Don't start the next phase until told. 3 failed "Done when" attempts → stop + report.
