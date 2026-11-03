# Roll reward art and integration plan

Scope: prepare art and plan only. Runtime changes require approval after demo. Preserve existing payout probabilities, cost, rank/race grants and buffs in phase one. Keep backups/versioned assets and revert only this feature's changes.

## Art package

`artsrc/ui/roll-rewards-v1/`: coin rotation (6 poses), six buff/pulse icons (HP, attack, attack speed, crit, armor, pulse), six recruit VFX components (head, burst, arrival ellipse, motes, impact, halo), six slot-highlight frames. Generated through built-in imagegen with the actual human_legendary idle sprite as style reference. Flat cartoon, bold outlines, real alpha, equal 3x2 cell layout, no text or character redraw. Existing character sprites and gold wallet icon retained. Neutral recruit elements tinted by rank in code, not four independent drifting sheets.

## 1. Validate / demo

- Inspect transparent alpha, cell boundaries and dark/light/grass readability.
- Split fixed-size cells preserving canvas/pivot; don't crop each animation frame independently.
- Coin 6-frame loop; buff six entries are independent icons, NOT a sequential animation; recruit entries are components, NOT a clip.
- Show normal game-sized coin, buff, recruit and reward-pair demos on the actual preparation background before import.

## 2. Shared presentation pipeline

- RollPanel retains reel and winning-cell settle. Resolve once; give presentation controller the immutable payout with stable source anchors before panel hides.
- Separate gameplay commit from visual delivery; current CommitPayout is exactly-once and must remain so. Never award again on effect arrival.
- Lock conflicting roll/start/drag actions until delivery completes; handle pause, speed changes, restart, disable and scene exit. Decide cancellation as flush committed rewards + stop all visual tasks, not lose or duplicate payout.
- Use scaled deltaTime for all reward motion, green counters, hold and fades. Menus may retain unscaled animation independently.
- Queue simultaneous coin/buff results with short stagger; no modal tutorial-style interruption.

## 3. Coin

- Winning slot pulse about .15-.20 game seconds, then 3-8 spinning coins curve from slot to wallet over .35-.55 seconds, stagger .03-.05 seconds. Timing is design proposal, not a claim of APK timings.
- Wallet briefly pulses and shows +amount; count increases green, holds briefly, returns to normal.
- Trigger on reward event, not the label prefix. Selling/wave rewards may use their own source and shorter feedback, preventing fake roll celebrations.

## 4. Buff

- After result settle, reveal actual awarded stat icon and a short +value label; UI must use the resolved BuffAdds, never another random pick.
- Show concise nonblocking toast, then icon/energy travels to matching HUD stat over .4-.6 game seconds. Destination icon pulses; existing green count animates to committed value.
- Each stat gets a stable icon mapping; clarify points vs percentages. Preserve four current stat types at first.
- Armor addition is a separate gameplay change: extend BuffStatKind, RollResolver, balance tables, RunState, RunBuffs and HUD together; approve weights and armor units/formula first. Treasure baseline must not be overwritten.

## 5. Ally

- Reserve a valid board cell and final formation slot before starting flight. Compute final SlotToWorld with CellOffset/pivot; centre-of-cell is insufficient.
- Stable neutral comet head with code-driven curved trail/easing; tint Common white, Rare blue, Epic purple, Legendary gold. Increase particles/halo modestly by rarity, not character size.
- Arrival burst/ring then .12-.18 second character reveal using real sprite and feet anchor. Avoid cumulative scale/pivot changes.
- Full board must use an explicit approved refund/conversion or retry policy; currently it can drop a grant. Do not invent compensation during an art-only change.

## 6. Equipment (separate milestone)

- No gameplay implementation until persistence (run-only vs permanent), automatic vs manual target, slot limit, duplicate handling, stats and roll odds are approved.
- Then add reward model/resolver support, inventory/equip/unequip logic, stat composition with buffs, receive/equip UI and icon/shine art. Do not reinterpret Relics as equipment.

## 7. Verification and acceptance

- Forced deterministic payouts: coin only, ally each rarity, each buff, coin+buff, coin+ally, jackpot, no usable board cell.
- Verify totals exactly once before/after pause, x1/x2/x3, speed switch midflight, restart, scene exit and interrupted UI.
- Check source/destination alignment for grouped units, each board edge and mobile safe areas; particles never intercept taps.
- Asset count/reference checks, runtime compilation, Play Mode assertions and actual on-screen captures. Headless HTML demo checks do not replace Unity runtime verification.
- Final acceptance: distinguish all reward types without reading long text, no overlapping toast, no double awards, no sprite redesign and no balance changes in visual phase.

Implementation order: art/demo approval -> shared presenter -> coin -> buff -> ally -> runtime tests -> armor decisions -> equipment decisions and implementation.
