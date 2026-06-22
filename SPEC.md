# SPEC — 墩墩 & 嘟嘟 co-op mod

Design doc. Drafted in Phase 0 discovery from real reference source (decompiled
BaseLib v3.3.2, the game's own `sts2.xml`, the official template output, and the
installed reference mods). Every API claim below is cited to where it was read.
This is a **draft for review** — nothing here is locked until Yemin approves.

---

## 1. Confirmed environment (this machine, 2026-06-23)

| Thing | Value | Source |
|---|---|---|
| Game | **Slay the Spire 2 v0.107.1** (commit 59260271, 2026-06-18) | `…/Slay the Spire 2/release_info.json` |
| Runtime | **.NET 9.0** (`tfm net9.0`, framework 9.0.7) | `data_…/sts2.runtimeconfig.json` |
| Engine | **Godot 4.5.1 .NET** (GodotSharp 4.5.1) | `data_…/GodotSharp.dll` version |
| Harmony | **2.4.2** shipped *with the game* (`0Harmony.dll`) | `data_…/0Harmony.dll` |
| Game assembly | `sts2.dll` + `sts2.xml` (5.3 MB API doc) — namespace `MegaCrit.Sts2.Core.*` | `data_…/` |
| dotnet SDK | **9.0.303** (installed) | `dotnet --list-sdks` |
| Git | 2.52.0 (installed) | `git --version` |
| IDE | **VS Code** installed; **no Rider, no Visual Studio, no Godot/MegaDot editor** | path scan |
| Templates | **`Alchyr.Sts2.Templates 2.5.0`** installed (`alchyrsts2mod`, `alchyrsts2charmod`, `alchyrsts2contentmod`) | `dotnet new install` |
| Decompiler | `ilspycmd 8.2.0.7535` installed (global tool) | used to read BaseLib/YuWanCard |

**Installed reference mods** (`…/steamapps/workshop/content/2868840/`) — on-disk source:

| Id | Mod | Ver | Notes |
|---|---|---|---|
| 3737335127 | **BaseLib** (Alchyr) | **v3.3.2** | our core dependency |
| 3747566068 | **YuWanCard 猪猪** (一条鱼丸_) | v0.5.8 | full custom character — primary reference |
| 3747492505 | **Watcher** (lamali) | v1.4.20 | character port, multi-lang incl. Chinese localization |
| 3747497501 | **RegentFX 万象辉星** (Vitech) | v0.4.1 | card VFX; its changelog documents co-op (联机) VFX/desync fixes |
| 3747602295 | **RitsuLib** (OLC) | v0.4.34 | a *second*, competing framework lib (we use BaseLib, not this) |
| 3737322022 | QuickRestart (erasels) | v1.0.1 | small utility; clean manifest reference |

**Setup gap:** the only missing prerequisite is **MegaDot v4.5.1** (Mega Crit's Godot
fork) — needed to export the `.pck` (assets). Code (`.dll`) builds with dotnet alone.
Rider is *recommended but not required*; we can drive the whole build from the dotnet CLI.

---

## 2. What a mod physically is

A mod is a folder in `…/Slay the Spire 2/mods/<ModId>/` containing up to three files
(confirmed from every installed mod + the template):

- `<ModId>.dll` — your compiled C# (Harmony patches + BaseLib content subclasses)
- `<ModId>.pck` — Godot-packed assets (art, scenes, localization JSON) — *optional* if `has_pck:false`
- `<ModId>.json` — the manifest the loader scans at startup

**Manifest schema** (from the 6 installed manifests + template output). Real fields:

```json
{
  "id": "DundunDudu",
  "name": "墩墩 & 嘟嘟",
  "author": "Yemin",
  "description": "...",
  "version": "v0.1.0",
  "min_game_version": "0.107.1",
  "has_pck": true,
  "has_dll": true,
  "dependencies": [ { "id": "BaseLib", "min_version": "3.3.2" } ],
  "affects_gameplay": true
}
```

The template auto-writes the live BaseLib version into `dependencies[].min_version`
on build (csproj `UpdateDependencyVersions` target). `affects_gameplay:true`
**disables Steam achievements** while enabled — expected, not a bug.

---

## 3. Project shape (recommended: ONE mod hosting both characters)

Confirmed shape from the `alchyrsts2mod` template and Alchyr's `Oddmelt`: a mod is a
**single Godot project + single C# project, co-located** (the template requires
"solution and project in same directory" so Godot and MSBuild agree on paths).

```
DundunDudu/                       # repo + mod root
├── DundunDudu.csproj             # SDK = Godot.NET.Sdk/4.5.1, TFM net9.0
├── DundunDudu.sln
├── Directory.Build.props         # GodotPath (MegaDot) lives here
├── Sts2PathDiscovery.props       # auto-finds the game install
├── project.godot                 # Godot project (drives .pck export)
├── export_presets.cfg            # "BasicExport" preset, architecture=msil
├── DundunDudu.json               # manifest
├── DundunDuduCode/               # ALL C# here
│   ├── MainFile.cs               # [ModInitializer] entry; Harmony.PatchAll()
│   ├── Dundun/ …                 # 墩墩 character + cards + powers
│   ├── Dudu/  …                  # 嘟嘟 character + cards + powers
│   └── Couple/ …                 # 在一起 / 双人连携 shared synergy + net messages
└── DundunDudu/                   # ALL assets here (packed into .pck)
    ├── localization/**/*.json    # Chinese strings (analyzer-checked)
    ├── images/ …                 # placeholder portraits/art
    └── mod_image.png             # mod icon
```

**Why one mod, not two:** the 双人连携 synergy couples the two characters — shared
buff types, shared custom net-message types (auto-discovered per *assembly*), shared
keywords/localization. Co-op also requires *identical mod versions on both clients*;
one artifact = one version to keep in lockstep. Splitting into two mods buys nothing
and multiplies the version-sync surface. **Decision needed: confirm one-mod.**

**Entry point** (verified, `SmokeMod/SmokeModCode/MainFile.cs`):

```csharp
using MegaCrit.Sts2.Core.Modding;
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node {
    public const string ModId = "DundunDudu";
    public static void Initialize() {
        new Harmony(ModId).PatchAll();   // BaseLib content auto-registers via reflection
    }
}
```

---

## 4. Content registration API (read from decompiled BaseLib v3.3.2)

You **subclass an abstract `Custom*Model`**; the constructor self-registers and BaseLib
reflection-scans the mod assembly for `ICustomModel` subtypes. No manual register call.

- `CustomCharacterModel : CharacterModel, ICustomModel, ILocalizationProvider`
  (BaseLib `BaseLib.decompiled.cs:35680`). Its ctor calls
  `CustomContentDictionary.AddCharacter(this)` (`:35748`). Surface includes:
  - From base `CharacterModel`: `StartingHp`, `MaxEnergy`, `StartingDeck`, card pool,
    `StartingGold` (default 99, `:35736`), `MultiplayerStartingRelics` (`:35582`).
  - Art paths (all `virtual`, default null): `CustomVisualPath`, `CustomIconPath`,
    `CustomCharacterSelectBg`, `CustomRestSiteAnimPath`, `CustomMerchantAnimPath`,
    rock/paper/scissors arm textures, `CustomEnergyCounter` (color), SFX paths
    (`:35690`–`:35734`).
  - `Localization => List<(string,string)>?` inline provider (`:35682`).
  - Animation helper `SetupAnimationState(...)` for spine/sprite idle/hit/attack/cast
    states (`:35761`).
- `CustomCardModel : CardModel, ICustomModel, ILocalizationProvider` (`:35148`)
- `CustomCardPoolModel : CardPoolModel` — **card color** via `ShaderColor` +
  `CardFrameMaterialPath` (`:35569`). Each character gets its own pool/color.
- `CustomModifierModel : ModifierModel` — **powers** (`ModifierAlignment`, `:37503`)
- `CustomRelicModel`, `CustomPotionModel`, `CustomOrbModel`, `CustomEventModel`,
  `CustomEncounterModel`, `CustomMonsterModel`/`CustomPetModel`, `CustomActModel`, etc.

Cards add keywords via `CardModel.AddKeyword(CardKeyword)` (seen in YuWanCard
`:29187`). Models are looked up via `ModelDb.Get<T>()` / `ModelDb.GetById<T>(ModelId)`.

---

## 5. Localization (Chinese-only v1)

Two routes, both confirmed; the game uses `LocString(table, key)` pairs
(`MegaCrit.Sts2.Core.Localization`, seen in YuWanCard `:42817`).

- **Route L1 — inline in C#:** override `Localization => List<(key,value)>` on each
  `Custom*Model`. Pure dll, **no `.pck` needed**. Best for early dll-only iteration.
- **Route L2 — JSON files:** `<mod>/localization/**/*.json`, declared as
  `AdditionalFiles` in the csproj and validated by `Alchyr.Sts2.ModAnalyzers` (the
  empty-mod build emitted `STS002: Localization files must be added as additional
  files`). These pack into the `.pck`. This is the analyzer-blessed, scalable route.

**Plan:** start L1 while iterating code-only (Phases 3–5), migrate to L2 in the
localization/art pass (Phase 6). Known-unknown: confirm L1 covers *all* card/character
text (name, description, keywords) or whether some strings require L2. Verify in Phase 3.

---

## 6. The 双人连携 / 在一起 mechanic — MP-safe design (the centerpiece)

### What the engine gives us (read from `sts2.xml`)

StS2 co-op is **deterministic lockstep**, not state-replication:

- Action-queue messages: `RequestEnqueueActionMessage`, `ActionEnqueuedMessage`,
  `HookActionEnqueuedMessage`, `RequestEnqueueHookActionMessage`
  (`sts2.xml` `Multiplayer.Messages.Game.*`).
- RNG is synced: `SyncRngMessage`, `SyncPlayerDataMessage`.
- **Desync is detected by the engine**: `Multiplayer.Game.ChecksumTracker` +
  `Messages.Game.Checksums.StateDivergenceMessage`. If our effect diverges state
  between clients, the game *will* flag it.
- Per-subsystem synchronizers: `CombatStateSynchronizer`, `RewardSynchronizer`,
  `PlayerChoiceSynchronizer` (`ReserveChoiceId` / `WaitForRemoteChoice`).

### What BaseLib gives us (read from decompiled BaseLib)

- **Custom net messages are first-class.** Define `ICustomMessage` (`BaseLib:37491`):
  `Serialize/Deserialize(Packet…)`, `HandleMessage(ulong senderId)`,
  `ShouldBroadcast`, `ShouldBuffer => true` (default — buffers to dodge timing
  desyncs), `Mode = reliable`. Send with **`CustomMessageWrapper.Send(msg)`**
  (`:37483`). BaseLib auto-discovers all `ICustomMessage` subtypes in the mod,
  assigns stable hashed ids, and registers the handler — zero boilerplate wiring.
- **Player-targeted variant:** `ICustomTargetedMessage` + `CustomTargetedMessageWrapper.Send`
  (`:38193`), carries a `RunLocation`, and on receipt **buffers through
  `RewardSynchronizer` if combat is mid-reward** — exactly the timing-safety we want.
- **Any-player card targeting in MP already exists:** `AnyPlayerCardTargetingHelper.IsAnyPlayerMultiplayer`
  with Harmony patches on `NCardPlay` (`:13936`, `:14151`–`:14316`). BaseLib already
  supports a card whose target is *the other player*.

### Two routes for the synergy (recommend A; B as fallback/extension)

- **Route A — synchronized in-combat card effect (preferred, lowest risk).** A card
  played by 墩墩 enqueues a normal `GameAction` that applies a power/block to *嘟嘟's
  creature* via BaseLib's any-player targeting. Card plays are **already** run through
  the synchronized action queue on both clients, so the cross-player effect rides the
  existing lockstep + RNG sync + checksum. Minimal new netcode.
- **Route B — BaseLib custom net message.** For triggers *outside* the synchronized
  combat action flow (e.g. a persistent 在一起 buff applied at run start, or a
  campfire/out-of-combat interaction): define `CoupleBuffMessage : ICustomMessage`
  carrying `{targetPlayer, buffId, amount}`, `Send` it, and in `HandleMessage` enqueue
  a deterministic action on the recipient. `ShouldBuffer=true`. This is the pattern the
  CLAUDE.md-referenced campfire card-trading co-op mod uses.

### Desync failure modes to design against (enumerated up front)

1. **Non-determinism** (unsynced RNG, `DateTime`, local-only state) → `StateDivergenceMessage`.
   → Only use game/synced RNG; never wall-clock or `System.Random`.
2. **One-sided application** (sender applies, receiver doesn't, or vice-versa) → divergence.
   → Always route through the synchronized action queue; never poke remote state directly.
3. **Timing/ordering** (message handled at different game phases per client) → softlock.
   → `ShouldBuffer=true`; prefer Route A; for B use the targeted/reward-buffered wrapper.
4. **Version mismatch** (different mod/BaseLib per client) → serialization id mismatch.
   → Hard-require identical mod + BaseLib versions on both clients (single artifact, §3).
5. **Modded + vanilla partner** (RegentFX supports "only one installs"). v1 assumes
   **both** play 墩墩/嘟嘟, so both have the mod; the synergy must no-op gracefully if
   the partner isn't the paired character. Flag, don't solve, for v1.

### The spike (Phase 2 — de-risking centerpiece)

Smallest possible proof: in a 2-player lobby, 墩墩 plays a trivial card → 嘟嘟 visibly
gains 1 Block/Strength, applied via Route A, with **no `StateDivergenceMessage`** across
a few turns. Build the card + effect; **Yemin runs the live 2-instance verification**
(an agent can't drive two game clients in a co-op lobby). If Route A can't apply to the
partner cleanly, fall back to Route B and re-test. Result reshapes Phases 5/7 if negative.

---

## 7. Build & publish workflow (verified against the template)

- **Code iteration (no Godot): `dotnet build`.** Restores `Godot.NET.Sdk/4.5.1` +
  BaseLib + analyzers, compiles the `.dll`, and (csproj `CopyToModsFolderOnBuild`)
  copies `.dll`/`.pdb`/`.json` into `…/mods/<ModId>/`. **Proven green this session.**
- **Full build (assets): `dotnet publish`** → csproj `GodotPublish` runs
  `MegaDot --headless --export-pack "BasicExport" …` to produce the `.pck`. **Requires
  `GodotPath` in `Directory.Build.props` → MegaDot v4.5.1.** Not yet installed.
- **Possible shortcut:** the template ships a commented
  `BSchneppe.StS2.PckPacker 0.1.1` PackageReference that "generates a `.pck` when
  building if only using simple assets" — i.e. a `.pck` for plain PNG/JSON **without
  the Godot editor**. For placeholder static art this may remove the MegaDot dependency
  entirely for v1. Evaluate in Phase 1/6.
- **Path note:** Steam auto-discovery (`Sts2PathDiscovery.props`) keys off the default
  Steam library; the game is on `D:\SteamLibrary`. Build worked when passed
  `/p:Sts2Path=D:/SteamLibrary/steamapps/common/Slay the Spire 2`. Set this once in
  `Directory.Build.props` (uncomment `<Sts2Path>`).

---

## 8. Tooling: STS2 Modding Assistant MCP

`elliotttate/sts2-modding-mcp` (Nexus mods/345), **MIT**, latest **v3.7.0 2026-03-25**.
151–153 tools: decompile/query the game (3,048 entities, 144 hooks), generators
(`generate_character/card/relic/power/net_message`), build/deploy, **live Godot
scene-tree inspection**, and **AUTOSLAY autonomous playtesting (seeded runs at 20×)**.
Attaches to Claude Code via `claude mcp add sts2-modding <venv-python> -- <run.py>`;
needs the game installed + local decompile (ilspycmd), Python 3.11+, .NET 9 SDK.

**Recommendation (ASK — it's a new dependency):** *worth attaching*, mainly for (a)
fast decompiled-source queries, (b) the net-message/character generators as a
cross-check against our hand-written code, and (c) **AUTOSLAY for regression/desync
stress-testing the synergy** — something we otherwise can't automate. Caveats: its
v3.7.0 release predates the v0.107.1 patch (June), so it must **re-decompile against
the installed 0.107.1** before its game-knowledge is trustworthy; and attaching it is
Yemin's call + action (config + running the server). We can also proceed entirely
without it — nothing in the plan depends on it.

---

## 9. Genuine known-unknowns (not yet proven)

1. Whether Route A any-player targeting applies a power to the partner's creature
   cleanly inside the synced action pipeline (Phase 2 spike + live test).
2. Live 2-player no-desync behavior across a full combat — only verifiable in a real
   lobby on two instances (Yemin).
3. Whether inline `Localization` (L1) covers all card/character text or L2 JSON is
   required (Phase 3).
4. Whether `PckPacker` handles our placeholder assets or MegaDot v4.5.1 is required
   (Phase 1/6) — and obtaining MegaDot.
5. Exact `CharacterModel` overrides for HP/energy/starter-deck/portraits/site-art —
   partially read; pin down from the `alchyrsts2charmod` Character template +
   YuWanCard in Phase 3.
6. The `min_game_version` we should declare (0.107.1) vs forward-compat as the game
   patches — BaseLib tracks patches; re-verify per patch.

---

## 10. Non-goals (v1)

Polished/final art (placeholder only), English or any non-Chinese text, public Steam
Workshop release, single-player balance tuning as the primary mode. All deferred.

---

## 11. Decisions needed from Yemin (surfaced, not decided)

1. **One mod hosting both characters?** (recommended, §3.)
2. **Co-op route:** approve Route A primary / Route B fallback for the Phase 2 spike (§6)?
3. **Attach the STS2 Modding Assistant MCP?** (§8 — new dependency, ASK.)
4. **MegaDot v4.5.1 install** now, or try `PckPacker` first to defer it? (§7.)
5. **Git remote:** none exists. Provide an `origin` URL (or confirm local-only commits
   for now)?
