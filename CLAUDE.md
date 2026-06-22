# Project: 墩墩 & 嘟嘟 — Slay the Spire 2 Co-op Mod

A private Slay the Spire 2 mod adding two custom playable characters, 墩墩 (Dundun)
and 嘟嘟 (Dudu) — the user and his partner — built around a "couple synergy"
mechanic (在一起 buff / 双人连携 combo) designed for two-player co-op. All in-game
text is Chinese. Placeholder art for now; polished art and any public Steam
Workshop release come later.

This file is your operating manual. Read it first. **SPEC.md (design) and
TASKS.md (plan) do not exist yet** — you draft them in the discovery phase (see the
discovery kickoff message). Do not write implementation code before that plan is
reviewed and approved.

---

## Critical context: a brand-new, under-documented modding surface

- StS2 runs on the **Godot engine** — NOT the original game's Java/LibGDX. None of
  ModTheSpire/BaseMod/`mts-launcher.jar` applies. If you find Java/Badlogic
  references in troubleshooting threads, those are StS1 and irrelevant here.
- Official **Steam Workshop support landed only in v0.107.1 "Major Update 2" on
  2026-06-19**. The built-in mod loader existed since launch but was unpublicized,
  and Mega Crit calls the current implementation barebones — expect it to change
  patch to patch.
- The loader **scans each mod's `manifest.json` in the game's `/mods` folder at
  startup**; a malformed or missing manifest fails silently.
- **Modding is confirmed C#/.NET.** Mods are C# assemblies built on **BaseLib**
  (the dominant community base library, by Alchyr) and patched via **Harmony**.
  A mod compiles to three files placed in `Slay the Spire 2/mods`: `<name>.dll`
  (your code), `<name>.pck` (Godot-packed assets/scenes/localization), and
  `<name>.json` (the manifest). Sources: github.com/Alchyr/BaseLib-StS2,
  github.com/Alchyr/ModTemplate-StS2.

Feasibility of the full goal is **confirmed**, not assumed: fully-custom NEW
playable characters with their own card/relic pools, custom art, events, and even
custom achievement badges already ship on the Workshop today (e.g. the
`YuWanCard（猪猪）` mod), there is a first-class `Slay the Spire 2 Character` project
template, and a community generator (`generate_character`) exists for exactly this.
What still needs to be pinned from real reference source is the **exact**
character/card registration API and the **co-op cross-player sync** design (see Hard
constraints) — confirm those from a working mod's source, don't guess. The API is
still days old and shifts with game patches, so re-verify against the current
BaseLib release rather than stale docs.

---

## User profile

- **Yemin** — Treasury Officer who writes code daily (Python, JS, VBScript, VBA,
  Office Scripts). Comfortable with Git/GitHub. Works on **Windows**. He and his
  partner **CC** both play StS2; this mod is for the two of them to play together.
- Treats AI as a **peer collaborator**. Wants you to push back with logic when he's
  wrong, take his pushback seriously (assume it reflects something you missed), and
  skip ethics/values commentary.
- Communicate **respectfully and precisely**. Never condescend, never talk down. He
  is the decision-maker on this project — surface tradeoffs and let him decide.

---

## How the user wants you to behave

These override your defaults. Follow them every response.

- **Output entire files, never snippets** with "patch here". When you change a file,
  give the whole file.
- **No emotional support or consoling** unless he is clearly venting about a
  personal situation.
- **Concise, information-dense, straight to the point** — after thorough
  investigation, not before it.
- Plain everyday words, short phrases.
- **No situational recap** at the top of a response. Start with substance. No
  meta-prefaces ("direct answer", "no preamble", "let me cut to it").
- **No apologies** for mistakes — explain how the mistake happened instead.
- **No absolute language** ("final", "most complete"). Use "v1", "current
  iteration".
- **When he pushes back, treat it as likely reflecting something you missed** — redo
  and re-evaluate deeply (assume he might be wrong, but take it seriously).
- For any **time-sensitive factual claim** about the real world (API behavior, game
  version, framework capability), **web-search and cite a real source**. Never
  invent URLs.
- When you **save content to a file, do not also paste it inline** in chat — give
  the path and character count.
- He writes in Chinese; **reply in Chinese when he does**. All in-game/user-facing
  mod text is **Chinese only** for v1.
- Mermaid diagrams: **wrap text inside every box.**
- **Never suggest deleting files/dirs inside an install path** without him first
  verifying the full structure (`Get-ChildItem -Recurse`). A path loaded by a
  running process is live code, not residue.

---

## Tech stack

- **Engine / target:** Slay the Spire 2 (Godot .NET), mod loaded via the built-in
  `/mods` loader and Steam Workshop.
- **Mod language:** **C# / .NET** (Godot Mono). Confirmed.
- **Core dependency:** **BaseLib** by Alchyr — NuGet `Alchyr.Sts2.BaseLib`
  (`<PackageReference Include="Alchyr.Sts2.BaseLib" Version="*" />`). Standardizes
  content additions AND provides save persistence + **multiplayer state sync** (the
  thing the co-op mechanic depends on). It tracks game patches closely — update it
  after each StS2 patch; a stale BaseLib is the #1 cause of mod breakage. Pair with
  `Alchyr.Sts2.ModAnalyzers`.
- **Patching:** Harmony (prefix / postfix / transpiler) for hooking game behavior.
- **Scaffold:** `dotnet new install Alchyr.Sts2.Templates`, then the
  `Slay the Spire 2 Character` template (one per character, or one mod hosting both).
- **Optional tooling to evaluate in discovery:** the open-source **"STS2 Modding
  Assistant MCP"** (Nexus mods/345) — generators for characters / cards / powers /
  net-messages, an in-game bridge for Harmony hot-reload, autonomous stress-testing,
  plus ~15 BaseLib reference docs and 29 guides. **ModConfig** if in-game settings
  toggles are wanted. Confirm currency with v0.107.1 before relying on either.
- **Platform:** Windows. StS2 install (this machine):
  `D:\SteamLibrary\steamapps\common\Slay the Spire 2\`. Manually-placed mods go in
  `D:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\`; Workshop-subscribed mods
  (BaseLib, etc.) live under `D:\SteamLibrary\steamapps\workshop\content\2868840\<id>\`.
  Several mods are already installed — read them as on-disk reference source.
- **IDE:** Rider recommended (Godot needs a `.sln`; VS is moving away from those).

Why these choices live in SPEC.md once you write it. Don't second-guess them without
reading that file.

---

## Architecture at a glance

Unknown until discovery. You define it in SPEC.md after reading a working mod. At
minimum the mod is a folder under `/mods` with a `manifest.json` plus
character/card/event definitions and placeholder art, registering two characters and
their card pools through the framework's content-registration API, with the
couple-synergy mechanic implemented in a multiplayer-safe way.

---

## File and directory layout

Top level is tentative; the **mod-internal structure is yours to define in SPEC.md**
after you confirm C# vs GDScript:

```
sts2-mod-uploader/            # repo root (existing repo — confirm its contents first)
├── CLAUDE.md                 # this file
├── SPEC.md                   # design — you draft in discovery
├── TASKS.md                  # phased plan — you draft in discovery
├── README.md                 # human setup/usage — near release
├── .gitignore
└── <mod folder>/             # the actual mod; internal layout TBD in SPEC
    ├── manifest.json         # you generate from the REAL schema, not a guess
    └── ...                   # characters, cards, events, placeholder art, scripts
```

`manifest.json` (the `<name>.json`) is generated by the Character template and the
publish step, not hand-written from scratch. Its real fields include `name`,
`author`, `description`, `version`, `has_pck`, `has_dll`, `dependencies` (list
BaseLib here), and `affects_gameplay` (`true` for this mod). Confirm any
character-specific fields from the template output. Flag before creating files
outside this layout.

---

## Decision authority

**You decide alone:**
- Internal file layout of the mod
- Card / power / relic / event internal implementation
- Function or script signatures, helper extraction, naming
- Logging style, error-handling style
- The exact verification steps and commands

**You ask the user (via the architect):**
- The `manifest.json` schema interpretation when ambiguous
- Adding any framework dependency beyond the confirmed core
- **The co-op sync approach** for the cross-player synergy mechanic
- Any change to scope, the two-character target, Chinese-only text, or this
  file / SPEC.md
- Anything that could corrupt saves or affect the user's real game install
- When the 3-strikes rule triggers (same approach failed 3×)

---

## Hard constraints

- **Verify before you design.** No SPEC text about an API capability you have not
  seen in a real mod's source, the BaseLib wiki, or the official template output.
  Cite the source.
- **Co-op cross-player sync is the primary technical risk.** Full custom characters
  are a solved problem; the novel, risky part is the 在一起 buff / 双人连携 mechanic,
  where a card played by one player applies an effect to the *other* player's state.
  This is doable — BaseLib provides multiplayer state sync and custom net messages
  are a known pattern (a campfire card-trading co-op mod already does cross-player
  effects on BaseLib) — but mishandled cross-player state is exactly what
  desyncs/softlocks co-op. So: design the synergy around BaseLib's sync + explicit
  net messages, treat MP-safety as a hard requirement (both clients run identical mod
  + BaseLib versions), de-risk it with a minimal cross-player-effect spike **early**
  (right after a trivial "empty mod loads" smoke test, before building card content),
  and enumerate the desync failure modes before scaling up.
- **Protect the real game install.** Develop against a backup or a test setup; do
  not risk the user's saves. (v0.107.1 note: the base game no longer deletes
  progress from removed/errored mods — but still be careful.)
- Enabling gameplay mods **disables achievements** — expected, tell the user, not a
  bug to fix.
- **All in-game text Chinese only** for v1. No English in cards/UI.
- Never commit build artifacts or secrets; keep `.gitignore` current.

---

## Phase protocol

For every phase:

1. Read the phase's section in TASKS.md and matching component in SPEC.md
2. Implement
3. Run the phase's "Done when" verification
4. Tick the phase's checkboxes in TASKS.md
5. Commit `Phase N: <one-line summary>` (or `Phase N.M hotfix: <summary>`)
6. Push to origin/main
7. Report using the format below
8. **STOP.** Do not start the next phase until told to.

If "Done when" fails 3 times: **stop, report all 3 attempts, ask for direction.**

---

## Report format

```
Phase N — <done | hotfix-needed | stuck>

What changed:
- <file>: <one-line description>

Verification:
- <test>: <result>

Caveats / deviations:
- <issue or "none">

Commit: <hash> pushed to origin/main.

<one of>
- STOPPING for Phase N+1 prompt.
- STOPPING for interactive verification by user: <what to do>.
- STUCK after 3 attempts on <subproblem>: <details>. Need direction.
```

Keep reports terse. The user sometimes reads these on a phone.

---

## What "done" looks like for v1

Two playable characters, 墩墩 and 嘟嘟, in Chinese, each with a starter deck / small
card pool, with the couple-synergy (在一起 buff / 双人连携) mechanic working
**multiplayer-safe in two-player co-op** — a card one of you plays measurably buffs
the other — using **placeholder art**, loading through the StS2 mod loader **without
breaking the game or desyncing co-op**, and playable by the two of you together.

Polished art, English localization, single-player tuning, and public Workshop
release are later — out of scope for v1 unless the user approves.