# Discovery kickoff — paste the block below into Claude Code

Send this once the repo is cloned, these Phase 0 files (CLAUDE.md, .gitignore, this
file) are in the repo root, and the folder is open in your IDE with CC attached.
CC should NOT write the real card/character content yet — only explore, stand up the
toolchain, prove the riskiest piece, and draft the plan.

The feasibility question is already settled: fully-custom characters, custom cards,
events, relics, and co-op cross-player effects all exist on the StS2 Workshop today,
built on C#/.NET + BaseLib + Harmony. So discovery is NOT "can we?" — it is "nail the
exact API from real source, stand up the build, and de-risk co-op sync early."

---

```
# Discovery + toolchain + co-op spike, not full content

Read CLAUDE.md first. Then do the steps below and draft the plan. Do NOT build the
real card/character content yet.

Goal: a private, Chinese-language, co-op-focused Slay the Spire 2 mod adding two
custom playable characters — 墩墩 and 嘟嘟 — with a "couple synergy" mechanic
(在一起 buff / 双人连携) where a card one player plays buffs the OTHER player in
two-player co-op.

Out of scope for v1: polished/final art (placeholder is fine), public Steam Workshop
release, English text, single-player as the primary mode.

# Size of this task

Multi-module / multi-session build, so a phased plan is warranted. The bottleneck is
NOT code volume — it is (a) exact API details and (b) co-op cross-player sync. Spend
effort there. Do NOT wrap this in a sub-agent swarm or multi-pass "assessment". The
toolchain is known (see CLAUDE.md Tech stack); confirm it, prove the risky bit,
report.

# What to do (in priority order)

1. THIS REPO. Report what's actually in `sts2-mod-uploader` — empty, an uploader
   tool/scaffold, or something else. Don't assume.

2. STAND UP THE TOOLCHAIN + SMOKE TEST. The StS2 install is at
   `D:\SteamLibrary\steamapps\common\Slay the Spire 2\`. First inventory what's
   ALREADY installed: list `D:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\`
   and `D:\SteamLibrary\steamapps\workshop\content\2868840\` and report the mods
   present (BaseLib id 3737335127, plus whatever else) with their versions — these
   are on-disk reference source. Confirm on this machine: dotnet SDK, Rider or VS,
   Godot .NET. Install the templates (`dotnet new install Alchyr.Sts2.Templates`).
   Create a trivial mod (empty or the plain Mod template), Build/Publish it, and
   confirm it LOADS via "Load with Mods" without breaking the game. This is the first
   runnable milestone — get it green before anything else. Report exact versions
   (game, BaseLib, Godot, dotnet).

3. LEARN THE REAL API FROM REFERENCE SOURCE. Read the `Slay the Spire 2 Character`
   template output, the BaseLib wiki (alchyr.github.io/BaseLib-Wiki), and at least
   one real character mod's source — Alchyr's `Oddmelt` (github.com/Alchyr/Oddmelt,
   C# WIP character) and/or the local copy of `YuWanCard（猪猪）` and any other
   installed character mod under
   `D:\SteamLibrary\steamapps\workshop\content\2868840\`. Extract, from real code:
   how a character registers (id, starting HP, energy, card color/pool, relic pool,
   portraits/art, site art), how cards/powers/relics register and hook, how
   localization works (Chinese strings), and the real manifest fields. Cite what you
   read.

4. CO-OP CROSS-PLAYER SPIKE — the de-risking centerpiece. Confirm how BaseLib does
   multiplayer state sync and how custom net messages are sent/received. Reference
   the campfire card-trading co-op mod (it already mutates another player's state on
   BaseLib) and, if it's current with v0.107.1, the "STS2 Modding Assistant MCP"
   generator `generate_net_message` and its multiplayer-networking guide. Build the
   smallest possible proof: in a co-op lobby, a trivial action by player A applies a
   visible, correct effect to player B's state, with NO desync across a few turns.
   Report whether this works, the exact mechanism, and the desync failure modes to
   avoid (cf. known softlock patterns where cross-player kills/effects diverge). If
   it can't be made MP-safe with the current API, flag it now — it reshapes the
   mechanic.

5. EVALUATE THE MCP TOOL. Is "STS2 Modding Assistant MCP" (Nexus mods/345) current
   with v0.107.1, and can it attach to this CC workflow? If yes, it gives generators
   (generate_character, generate_card/relic/power, generate_net_message), in-game
   Harmony hot-reload, and AUTOSLAY stress-testing — say whether to use it or skip
   it, with a one-line reason.

# Then draft the plan

- SPEC.md — real architecture from what you found: the C#/BaseLib/Harmony project
  shape, the two-character structure (one mod hosting both, or two mods — recommend
  one), how each character + its card pool registers, the Chinese localization setup,
  the **MP-safe design for the 双人连携 mechanic** (BaseLib sync + named net
  messages), the build/publish workflow, the genuine known-unknowns that remain, and
  explicit non-goals. Use templates/SPEC.md.template if present.
- TASKS.md — phases from the real scope, each ending runnable/testable. Order:
  (1) toolchain + empty-mod smoke test, (2) co-op cross-player spike, (3) one
  character skeleton loads & is playable solo, (4) second character, (5) the synergy
  cards, (6) Chinese localization + placeholder art pass, (7) two-player co-op
  playtest. Riskiest unknowns (smoke test, co-op spike) come first. Use
  templates/TASKS.md.template if present.
- Note anything that surprised you versus what the goal implied.

# Then stop

Commit the plan: `git add` + commit `Phase 0: discovery + plan draft`, push to
origin/main. Report:
- What's in the repo (briefly)
- Toolchain versions + whether the empty-mod smoke test passed
- Result of the co-op cross-player spike (#4) — the single most important finding
- Whether to use the MCP tool
- Your proposed SPEC + TASKS in outline, and any decisions you need from me

STOP. The plan gets reviewed and approved before you build the real content.

# Decide vs ask

See CLAUDE.md "Decision authority". During discovery, lean toward reporting findings
and proposing — don't lock in irreversible design choices alone. The co-op mechanism
and any new dependency are ASK, not DECIDE.
```

---

## Notes (for Yemin, not part of the paste)

- After CC reports, relay to me just: did the smoke test pass, the co-op spike
  result (#4), CC's proposed phase outline, and any open questions. No file diffs.
- Hard prerequisite CC will surface: this needs Rider (or VS) + Godot .NET + dotnet
  SDK installed locally. That's the one setup cost before code can build.
- If CC starts hand-writing card content during discovery, stop it — this phase is
  toolchain + co-op proof + plan only.
- Verbs to send me: `go` (approve plan/phase → build), `stop` (fix the prompt first),
  `rollback` (last phase failed, revert + redesign), `diagnose <thing>` (investigate
  before next move).
