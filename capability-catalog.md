# capability-catalog.md — card-building vocabulary (StS2 v0.107.1)

Ready-made power / command / keyword pieces for building cards, pulled from the **0.107.1 decompile**
(`D:\MyProject\sts2-modding-mcp\decompiled`). Each entry is tagged by cost-to-implement and cited to a real
class + namespace so CC can build from existing parts instead of reinventing.

> Tooling note: the `sts2-modding` MCP is configured but **was not live in this session** (its tools didn't
> load; needs a Claude Code restart to attach). This catalog was built by direct decompile reads, not via the
> MCP query tools. Once the MCP is live, its `search_*` tools can refresh/extend this faster.

Scope: pieces only. **No character/synergy logic here**; 墩墩/嘟嘟 skeletons untouched. Cross-player synergy
(双人连携) is still Phase 5, gated on the live co-op spike PASS — see SPEC §6/§12.

---

## 1. Cost tiers

| Tier | Meaning | What you write |
|---|---|---|
| **Cheap** | Reuse an existing power and/or an existing `*Cmd` command. | Just card `OnPlay` code — a few lines. No new types. |
| **Medium** | Need a power that doesn't exist among the 260 built-ins. | A `CustomPowerModel : PowerModel` (BaseLib) with lifecycle hooks. No Harmony. |
| **Expensive** | Need to change game behavior not exposed by commands or power hooks. | A **Harmony** prefix/postfix/transpiler on a game method. |

Source assemblies / namespaces referenced below:
- Commands → `MegaCrit.Sts2.Core.Commands`
- Powers → `MegaCrit.Sts2.Core.Models.Powers`
- Cards/targeting/keywords → `MegaCrit.Sts2.Core.Entities.Cards`
- Vars → `MegaCrit.Sts2.Core.Localization.DynamicVars`; `ValueProp` → `MegaCrit.Sts2.Core.ValueProps`
- BaseLib bases → `BaseLib.Abstracts` (`CustomCardModel`, `CustomPowerModel`, `CustomModifierModel`, …)

All command methods are `async Task`; call them from `OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)`.

---

## 2. Commands — the verbs (all **Cheap**)

`MegaCrit.Sts2.Core.Commands` — 21 static command classes. The ones you actually build cards with:

### DamageCmd — deal damage
```csharp
await DamageCmd.Attack(DynamicVars.Damage.BaseValue)   // or .Attack(CalculatedDamageVar)
    .FromCard(this).Targeting(cardPlay.Target)          // AnyEnemy: guard target with ArgumentNullException.ThrowIfNull
    .WithHitFx("vfx/vfx_attack_slash")                  // optional
    .Execute(ctx);
```
Also `CreatureCmd.Damage(ctx, target(s), DamageVar|amount+ValueProp, dealer, cardSource)` for non-card / AoE damage.

### CreatureCmd — block, heal, hp, damage, kill (`MegaCrit.Sts2.Core.Commands.CreatureCmd`)
| Effect | Signature |
|---|---|
| **Gain block** | `GainBlock(Creature, BlockVar, CardPlay?, bool fast=false)` / `GainBlock(Creature, decimal, ValueProp, CardPlay?, bool)` |
| Lose block | `LoseBlock(Creature, decimal)` |
| **Heal** | `Heal(Creature, decimal, bool playAnim=true)` |
| Max HP | `GainMaxHp / LoseMaxHp / SetMaxHp(Creature, decimal, …)` |
| AoE/targeted damage | `Damage(ctx, Creature|IEnumerable<Creature>, DamageVar|decimal+ValueProp, dealer, cardSource)` |
| Kill / escape / stun | `Kill(Creature, force)`, `Escape(Creature)`, `Stun(Creature, …)` |
| Animation | `TriggerAnim(Creature, string trigger, float wait)` |

### PowerCmd — apply/modify any power (`MegaCrit.Sts2.Core.Commands.PowerCmd`)
```csharp
await PowerCmd.Apply<VulnerablePower>(ctx, target, amount, Owner.Creature, this);  // generic
await PowerCmd.Apply(ctx, powerInstance, target, amount, applier, cardSource);     // non-generic
await PowerCmd.ModifyAmount(ctx, power, offset, applier, cardSource);
await PowerCmd.Remove(power);  await PowerCmd.Decrement(power);
```

### CardPileCmd — draw / add / shuffle / exhaust-pile (`MegaCrit.Sts2.Core.Commands.CardPileCmd`)
| Effect | Signature |
|---|---|
| **Draw** | `Draw(ctx, decimal count, Player, bool fromHandDraw=false)` / `Draw(ctx, Player)` |
| Add generated card | `AddGeneratedCardToCombat(card, PileType, creator, position)` |
| Add to pile | `Add(card, PileType|CardPile, position, …)` |
| Shuffle | `Shuffle(ctx, Player)` |
| Auto-play from draw | `AutoPlayFromDrawPile(ctx, Player, count, position, forceExhaust)` |

### CardCmd — manipulate specific cards (`MegaCrit.Sts2.Core.Commands.CardCmd`)
`Discard`, `DiscardAndDraw(ctx, toDiscard, drawCount)`, `Exhaust`, `Upgrade`, `Transform/TransformToRandom`,
`AutoPlay(ctx, card, target)`, `ApplyKeyword(card, params CardKeyword[])`, `Enchant`, `Afflict`.

### PlayerCmd — energy / gold / stars / turn (`MegaCrit.Sts2.Core.Commands.PlayerCmd`)
| Effect | Signature |
|---|---|
| **Gain energy** | `GainEnergy(decimal, Player)` (also `LoseEnergy` / `SetEnergy`) |
| Stars (Regent) | `GainStars / LoseStars / SetStars(decimal, Player)` |
| Gold | `GainGold / LoseGold / SetGold(decimal, Player, …)` |
| End turn | `EndTurn(Player, canBackOut, actionDuringEnemyTurn)` |
| Pet | `AddPet(Creature, Player)` |

Other Cmd classes (situational, Cheap): `OrbCmd` (Defect orbs), `PotionCmd`, `RelicCmd`, `SfxCmd`, `VfxCmd`,
`MapCmd`, `RewardsCmd`, `ForgeCmd`, `OstyCmd`, `TalkCmd`, `ThinkCmd`, `CardSelectCmd`, `RelicSelectCmd`.

---

## 3. Powers — the nouns (260 built-ins, all **Cheap** to apply via `PowerCmd.Apply<T>`)

All in `MegaCrit.Sts2.Core.Models.Powers`. Apply a fixed amount → deterministic, single-player safe.
The high-use ones by category (not exhaustive — 260 total):

| Category | Ready-made powers (class names) |
|---|---|
| **Strength / Dexterity / Focus** | `StrengthPower`, `TemporaryStrengthPower`, `DexterityPower`, `TemporaryDexterityPower`, `FocusPower`, `TemporaryFocusPower`, `VigorPower`, `FlexPotionPower` |
| **Core debuffs** (apply to enemy) | `VulnerablePower` (more dmg taken), `WeakPower` (less dmg dealt), `FrailPower` (less block), `PoisonPower`, `ConstrictPower`, `ShrinkPower`, `SlowPower`, `HexPower`, `MindRotPower`, `NoBlockPower`, `NoDrawPower` |
| **Poison / damage-over-time** | `PoisonPower`, `NoxiousFumesPower`, `EnvenomPower`, `RupturePower`, `CorrosiveWavePower` |
| **Block / defense buffs** (self) | `BarricadePower` (block persists), `BlurPower`, `BufferPower` (negate next HP loss), `IntangiblePower`, `PlatingPower`, `RegenPower`, `HardenedShellPower`, `FeelNoPainPower`, `GuardedPower`, `RampartPower` |
| **Retaliate / reactive** | `ThornsPower`, `FlameBarrierPower`, `ParryPower`, `ReflectPower`, `ReboundPower`, `RupturePower` |
| **Scaling / form buffs** | `RitualPower` (str/turn), `DemonFormPower`, `EchoFormPower`, `JuggernautPower`, `MayhemPower`, `RagePower`, `DarkEmbracePower`, `Brutalityish→`(see list) |
| **Energy / draw (next turn)** | `EnergyNextTurnPower`, `NoEnergyGainPower`, `DrawCardsNextTurnPower`, `RetainHandPower`, `NoDrawPower` |
| **Multi-hit / "setup" / combo-flavored** | `OneTwoPunchPower`, `SetupStrikePower`, `FocusedStrikePower`, `SwipePower`, `BurstPower`, `AccelerantPower`, `PanachePower`, `AccuracyPower`, `InfiniteBladesPower`, `PhantomBladesPower`, `DuplicationPower` |
| **Misc utility** | `ArtifactPower` (block a debuff), `MetallicizishPlatingPower`, `HardToKillPower`, `DoomPower`, `LethalityPower`, `GravityPower` |

> ⚠ Behavior of the multi-hit/"setup" powers is mechanic-specific — read the class before relying on it. The
> NAME is a lead, not a spec.

### Co-op / teammate-flavored powers (candidate ready-made pieces for Phase 5 — verify each)
These are real `PowerModel`s whose names imply ally/co-op behavior. **Do not use yet** (synergy is Phase 5),
but they are the first things to inspect when 双人连携 opens:
- `FriendshipPower : PowerModel`, `TagTeamPower : PowerModel`, `CoordinatePower : TemporaryStrengthPower`,
  `SynchronizePower`, `LeadershipPower`, `SignalBoostPower`, `BeaconOfHopePower`, `DieForYouPower`.
- If any does what we want, the synergy effect drops from Medium → **Cheap** (just `PowerCmd.Apply<T>` on the
  ally). Tag stays "verify behavior + MP-safety first."

---

## 4. Card keywords — all **Cheap**

`MegaCrit.Sts2.Core.Entities.Cards.CardKeyword`: `None, Exhaust, Ethereal, Innate, Unplayable, Retain, Sly, Eternal`.
Add via `CanonicalKeywords => [CardKeyword.Exhaust]` or `CardCmd.ApplyKeyword(card, …)`. (We already use these
on the spike/basic cards.) Card classification tags = `CardTag` (e.g. for "play a Strike" logic).

---

## 5. Targeting — incl. **teammate** effects

`MegaCrit.Sts2.Core.Entities.Cards.TargetType`: `None, Self, AnyEnemy, AllEnemies, RandomEnemy,
AnyPlayer, AnyAlly, AllAllies, TargetedNoCreature, Osty`.

**Deterministic effects that can target a teammate (all Cheap pieces):**
- Card targets the partner via `TargetType.AnyAlly` (one ally) or `AllAllies` (everyone) — `cardPlay.Target`
  is the selected ally creature. BaseLib's `AnyPlayerCardTargetingHelper` makes this selectable in MP.
- Then apply any `Creature`-taking command to that ally creature — **all Cheap**:
  - Block → `CreatureCmd.GainBlock(allyCreature, …)`
  - Strength/Dexterity/any buff → `PowerCmd.Apply<StrengthPower>(ctx, allyCreature, amt, Owner.Creature, this)`
  - Heal → `CreatureCmd.Heal(allyCreature, amt)`
  - Energy to the ally player → `PlayerCmd.GainEnergy(amt, allyPlayer)`
- **MP-safety is NOT extra code** for in-combat card effects: card plays ride the game's synchronized action
  queue (deterministic lockstep), proven desync-free-by-design in the Phase 2 spike (`TogetherGuardSpikeCard`,
  AnyAlly→GainBlock). The only hard rule: keep the amount deterministic (no `System.Random`, no wall-clock).
- **Out of v1 scope until separately verified** (per SPEC §12 guardrail): giving a teammate *draw*, *energy that
  reshuffles*, pile manipulation, or any RNG — these are Cheap to *write* but carry desync risk and must each
  pass the live co-op test first.

---

## 6. Medium — author a custom power (`CustomPowerModel`)

When no built-in power fits (e.g. the **在一起 couple buff**, a bespoke "combo counter that grows as you play
cards", a "first card each turn does X"): subclass `CustomPowerModel : PowerModel` (`BaseLib.Abstracts`) — no
Harmony needed. Auto-registers like other `Custom*Model`; needs power icons in the pck + a `[Pool]`-free power
loc entry (`powers.json`).

PowerModel hooks you override (from `custom_power.md` + decompiled `PowerModel`):
- `StackType` — `Intensity` (stacks amount) / `Duration` (counts down) / `NoneAndStacks` / `None`
- Damage/block math: `ModifyDamageAdditive(int)`, `ModifyDamageMultiplicative(float)`, `ModifyBlockAdditive(int)`
- Turn lifecycle: `BeforeTurnStart()`, `AfterTurnEnd()`, `TickDown()` (Duration powers)
- Stack lifecycle: `OnApply()`, `OnRemove()`, `BeforeApplied(target, amount, applier, cardSource)`,
  `AfterApplied(applier, cardSource)`, `AfterRemoved(oldOwner)`
- Custom icon: implement `ICustomPower` (`PackedIcon` 64×64, `BigIcon` 256×256).

Reference a real one: `RitualPower` (gain Strength each turn), `PoisonPower` (DoT via turn hook), or
`CoordinatePower : TemporaryStrengthPower` (subclass an existing power).

`CustomModifierModel` (also `BaseLib.Abstracts`) is the sibling base for **run-level modifiers** (the
`ModifierAlignment` buff/debuff shown on the run), vs `CustomPowerModel` for **combat powers**.

---

## 7. Expensive — Harmony patches

When the effect changes behavior **not exposed** by any command or power hook — e.g. globally intercept every
card play, alter the energy/turn structure, change targeting rules, add an "on-combo / on-partner-plays-a-card"
trigger with no existing hook, or modify reward/map generation. Use **Harmony** (bundled `0Harmony.dll`,
patched in `MainFile.Initialize()` via `harmony.PatchAll()`):
```csharp
[HarmonyPatch(typeof(SomeGameType), nameof(SomeGameType.SomeMethod))]
internal static class MyPatch { static void Postfix(...) { ... } }   // prefix / postfix / transpiler
```
Publicize is already enabled (`<Publicize Include="sts2"/>`) so private/protected game members are reachable.
BaseLib itself is almost entirely Harmony patches — read its decompiled `BaseLib.Patches.*` for patterns.
**Expensive = highest desync risk in co-op**: any patched cross-cutting behavior must be deterministic and
identical on both clients. Default to Cheap/Medium; reach for Harmony only when there's no hook.

---

## 8. Coverage checklist (the requested topics → cheapest building block)

| Want | Tier | Build with |
|---|---|---|
| **伤害** (damage) | Cheap | `DamageCmd.Attack(var).FromCard(this).Targeting(target).Execute(ctx)` |
| **格挡** (block) | Cheap | `CreatureCmd.GainBlock(creature, BlockVar, cardPlay)` |
| **力量** (strength) | Cheap | `PowerCmd.Apply<StrengthPower>` (temp: `TemporaryStrengthPower`) |
| **敏捷** (dexterity) | Cheap | `PowerCmd.Apply<DexterityPower>` (temp: `TemporaryDexterityPower`) |
| **能量** (energy) | Cheap | `PlayerCmd.GainEnergy(amt, player)` / `EnergyNextTurnPower` |
| **抽牌** (draw) | Cheap | `CardPileCmd.Draw(ctx, count, player)` |
| **中毒** (poison) | Cheap | `PowerCmd.Apply<PoisonPower>` (+`NoxiousFumesPower`/`EnvenomPower`) |
| **虚弱** (weak) | Cheap | `PowerCmd.Apply<WeakPower>` |
| **易伤** (vulnerable) | Cheap | `PowerCmd.Apply<VulnerablePower>` |
| **连击 / combo (multi-hit)** | Cheap | loop `DamageCmd.Attack` N×, or apply `OneTwoPunchPower`/`SetupStrikePower`/`BurstPower` |
| **连击 (new combo counter)** | Medium | `CustomPowerModel` with `AfterTurnEnd`/`OnApply` tracking, `StackType.Intensity` |
| **目标=队友 (deterministic)** | Cheap | `TargetType.AnyAlly` + `CreatureCmd.GainBlock`/`PowerCmd.Apply<…>` on `cardPlay.Target` (ally). MP-safe via synced action queue (Phase 2 proven). |
| **在一起 couple buff** | Medium | `CustomPowerModel` (the bespoke buff); apply cross-player in Phase 5 after live PASS |
| **on-partner-plays-a-card trigger** | Expensive | Harmony hook on card-play if no power hook fits — Phase 5, verify MP-safety |
