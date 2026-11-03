> Audit lịch sử, không phải checklist đã kiểm lại hôm nay. Xem [việc đang chờ](../WORK_QUEUE.md); dòng mã và trạng thái test bên dưới có thể đã đổi.

# Architecture and gameplay audit — 2026-10-25

Scope: static review of runtime Battle, Meta, Gacha, Data and UI entry points. Five existing isolated model suites run; one extra isolated invalid-pool reproduction uses production TreasureRoll. No production code, player save, or game balance changed in this audit. Unity lifecycle/fault-injection and on-device profiling are not covered.

## Findings

### P1 — Save replacement has a data-loss window
SaveSystem.cs:105-108 writes .tmp, deletes save.json, then moves .tmp. Delete+Move is not one atomic replacement. A process interruption after Delete leaves no primary save; Load returns new SaveData when primary missing, ignoring the valid .tmp. Save returns void and catches errors, so callers cannot distinguish durable success. Recovery currently archives a corrupt file but does not load a last-known-good snapshot.
Remedy: same-directory atomic replacement supported on target platforms; preserve last-known-good backup, recover validated backup/temp on startup, return persistence outcome and test failures at each stage. Do not describe this as a thread race; it is a crash-consistency issue even on one thread.

### P1 — Chest debit and grants are separate transactions
TreasureReviewUi.cs:169 persists all key debit via SpendKeys before TreasureRoll.Roll applies and persists results. Termination or an exception in between loses keys without rewards. TreasureRoll.cs:49-54 updates pity before checking pool and skips a missing pool. Extra isolated test, production TreasureRoll, initial pity=80 + empty Legendary pool: rewards=0, resulting pity=0. This confirms the invalid-data failure; it does not imply today's catalog is missing a tier.
Remedy: validated TryOpenChest operation stages key debit, grants and pity on one proposed state, then commits once. Fail with no state changes on invalid catalog. Test missing tiers, invalid weights, persistence failure, large Open All counts.

### P1 — Clearing progress depends on result presentation
ResultPanel.cs:56-68 calls GameProgress.Clear from the UI phase listener. A disabled/missing ResultPanel means no listener and no reward/unlock despite BattleFlow reaching LevelCleared. ResultPanel.Awake also wires compatibility fields while building a runtime replacement later.
Remedy: a run completion service owns one idempotent settlement with a run ID and returns reward deltas. UI only displays the settlement. Test completion with no result canvas, duplicate events and retries.

### P2 — Draw falls through victory handling
BattleFlow.cs:290-292 explicitly sets Winner=null when both armies have zero living units. TickFinished only rejects Enemy winner, then advances or clears level for all other values (313-329). Thus a final-wave draw can clear the chapter. Whether a draw should lose or replay is a gameplay choice; currently it is implicit, not a separately handled rule.
Remedy: handle Ally/Enemy/Draw explicitly; test empty boards, last-unit simultaneous death, temporary summons dying during Finished, and final-wave settlement. No need to change the rest of the state machine.

### P2 — Parsed save is not necessarily a valid save
SaveSystem.Load accepts any non-null deserialized SaveData; Version is not migrated, lists/rows/IDs/numeric ranges not normalized. FindUnit/FindItem assume non-null lists and rows. Negative Level can yield a negative upgrade gold cost in AllySummon.TryUpgrade. Out-of-range levels can exceed intended caps through UnitUpgrades.Load.
Remedy: versioned migrations, validation/normalization on load, reject future versions safely, quarantine invalid data, enforce domain-level numeric bounds and checked/capped arithmetic. Public fields on a serialization DTO are not inherently an OOP defect.

### P2 — Test rules are embedded in production paths
GameProgress.Unlocked uses Max(2, savedHighest), permanently exposing chapter 1-2 to fresh saves. RollService.AwardWaveClear returns early in chapter 1-2 with a legend-testing comment. The free test gem shop is intentional per user request and should not be removed silently.
Remedy: explicit development/test configuration or balance profile, with clearly selected release profile. Preserve current authorized testing behavior until the user chooses release rules. Do not silently rebalance replay gem rewards (80/250 per win were explicitly authorized).

## Architecture observations

Good foundation: ScriptableObject definitions vs runtime Unit state; UnitBrain separated from health; IUnitVisual seam; explicit battle phases and queued deployment; target scan uses squared distance; independent summon/relic pity; upgrade preconditions; isolated tests.

Dependency leakage: BattleFlow calls Project.UI.SharedSettings.IsPaused and looks up UI/presentation-dependent classes; RollService discovers RollPanel, BattleSetup and reward placer at runtime. Unit creates visual helpers and HealthBar access alongside health rules. LegendaryCombat selects abilities by Race and discovers summon definitions among loaded Resources. These are concrete coupling points, not proof that every class must become an interface.

Suggested boundaries:
- Data/catalog and balance definitions, validated once.
- BattleContext for run state, pause, RNG and references; BattleFlow owns lifecycle, not presentation.
- Economy/completion services for domain transactions; persistence behind a small save-store contract.
- Presenters receive results/events and issue commands.
- Ability handlers selected from catalog when adding more skills; do not introduce a generic ability framework before required.

## Algorithm and performance

UnitRegistry uses List.Contains/Add and Remove: registration/removal are O(n), not O(1) as comment claims. Nearest-enemy is O(n) per query, O(n^2) when every living unit retargets. At current small roster this may be fine; profile realistic 100/200/500 units before spatial hashing. An indexed team list or grid is a later option, not a prerequisite for correctness.

A concrete List<T> foreach uses a value-type enumerator and does not inherently allocate; UnitRegistry's comment claiming foreach is universally expensive is misleading. Iterating via interface or LINQ can allocate. Do not replace all foreach loops mechanically.

TreasureDatabase.OfTier allocates and scans for every chest roll; Open All repeats this count times and retains a Result per roll before UI groups them. Build tier pools once per transaction and aggregate display results for huge counts, while preserving per-roll pity and floor semantics.

No evidence justifying ECS, multithreading, an inheritance tree per faction, or rewriting the whole project. Interface/composition and pure calculation helpers can be introduced at actual seams.

## Verification and gaps

PASS: test_summon.sh, test_relic_upgrade.sh, test_daily_shop.sh, test_ally_stats.sh, test_contribution.sh. They use minimal stubs and exercise model logic; they do not prove Unity lifecycle, file crash safety, UI-independent completion or combat timing. Invalid-pool audit ran separately in /tmp and did not touch player data.

Pity semantics: current >=80 misses check guarantees the next (81st) draw after 80 misses. Tooltip describes 'after 80 misses'; if product promise is 'within 80 draws', align both systems and add a boundary test for counters 78/79/80. Existing test only seeds counter 80. Treat this as a convention to settle, not an unqualified bug.

## Recommended order
1. Persistence recovery + atomic economy transactions, then UI-independent idempotent completion.
2. Explicit draw handling + load validation/migration + development profile isolation.
3. Unify combat/preview stat calculations and verify animation-dependent speed; add headless lifecycle/integration tests.
4. Profile actual battles and large chest batches, optimize measured hot paths only.
