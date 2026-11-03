> Trạng thái 2026-11-03: các family đã gắn; người dùng đã xác nhận effect ở x1/x2/x3. Phần audit và đề xuất ban đầu bên dưới là lịch sử, không phải yêu cầu generate lại. Căn weapon socket từng pose và hiệu năng trên thiết bị vẫn cần QA.

# G03 — Ranged projectile and impact review

Date: 2026-11-02. Status: approved base impacts integrated; remaining family art and full visual coverage pending.


## Confirmed rules — 2026-11-02

User confirmed the following. These supersede conflicting suggestions in the earlier audit below; the approved base implementation is now integrated.

1. Basic ranged projectiles home toward their ORIGINAL target while that target is alive. No switching to a different enemy.
2. Release from the correct weapon/attack pose: bow tip for archers, equivalent gun/staff/mouth anchor for other ranged units. Respect facing and animation release timing.
3. Add small impact effects to make valid hits readable. This is visual feedback, not additional damage or new status mechanics.
4. If the target dies during flight, finish travel to that enemy's last valid location and disappear WITHOUT impact. Preserve the death location before the body is hidden/destroyed/moved; do not follow a restored life or another unit. A lethal hit on a target alive at contact may show its normal contact effect once. This final distinction is an implementation interpretation to present in the demo, not an explicitly separate user answer.
5. Shield interception shows impact at the shield rather than on the character. Must inspect absorption across Epic Bodyguard, Legendary and Mythic shields. Proposed partial-absorption handling: shield contact when any shield intercepts, remaining damage still applies once; avoid duplicate shield+body effects unless separately approved.
6. Preserve existing projectile art initially; presentation adds no knockback, slow, DoT or AoE. Five impact families remain the working proposal: arrow, metal, arcane, spirit, web/slime. Keep basic projectiles visible when decorative Effects is disabled.
7. User delegates suitable timing/performance choices. Start with small4frame non-looping impact, tune duration/footprint at real combat size and x1/x3. Cache assets/materials and use bounded reused impact objects. No bulk generation before representative demos are reviewed.

Next: demo a guided shot and three representative contact types (arrow/body, shield, magic/slime), including target death in flight; then implement agreed timing/contact plumbing and complete art-family coverage. Other audit suggestions remain proposals, not user-confirmed requirements.

## Scope and observed inventory

Code/data audit, not frame-by-frame visual verification of every unit. Eight serialized ranged allies currently wired to eight projectile prefabs:

| Unit | Projectile | Speed | Arc |
|---|---|---:|---:|
| Forest Archer | wood_arrow |22|0.03|
| Moon Archer | crescent_bolt |24|0|
| Verdant Archer | royal_arrow |24|0.02|
| Gear Gunner | brass_bullet |32|0|
| Crescent Mage | crystal_bolt |21|0|
| Spirit Wisp | psychic_orb |18|0|
| Haunted Tank | ghost_skull |17|0.02|
| Cave Spider | web_glob |16|0.04|

Runtime definitions add two ranged Mythics (Elf and Clock), with elf-bolt / clock-bolt fallback projectiles at speed8, arc0. Chapter1 Green and Violet Slimes clone a ranged template and override projectile art/tint; speed/arc/prefab orientation are inherited from that template. Elite/Boss chapter1 slimes are melee and are not projectile-impact work.

## Fix / design checklist

- [ ] R03-01 — Flight and contact agreement. Projectile.Launch snapshots target.HitPoint; Arrive applies damage to the living target without checking distance from the old endpoint. Thus moving targets can take damage where the projectile is not. Decide homing or true fixed-point miss/collision behaviour first. Suggested baseline: guided basic projectiles, no new accuracy mechanic; trajectory and impact follow the same target. Never silently retarget a dead victim.
- [ ] R03-02 — Launch origin. Unit.prefab muzzle is null; normal FireProjectile fallback is transform.position (feet), Mythic fallback uses HitPoint. Add configurable release offsets/anchors tied to each attack pose and facing; inspect bow/staff/gun/slime-mouth separately. Release at the existing animation callback, not an extra damage event.
- [ ] R03-03 — Define hit outcome. Arrive currently calls TakeDamage then destroys itself, no impact VFX. TakeDamage returns void and can be fully absorbed, lethal, or followed by revival. Decide body-hit vs shield-hit feedback and capture a result/contact position without applying damage twice. A fully blocked hit is still a contact, not a miss.
- [ ] R03-04 — Lifetime/identity rules. Keep no damage on dead/destroyed/wrong-team targets and no old projectiles crossing into Prep. Add a policy for a victim dying and reviving while its old projectile is airborne (same Unit reference becomes alive again). Suggested: cancel that projectile, do not strike the new life. Decide what a lost-target projectile visually does. Shooter death should not automatically erase an already-fired projectile; preserve current rule and contribution behaviour.
- [ ] R03-05 — Asset-family mapping. Explicit projectile/impact family data rather than inferring from names or copying ranged templates. Suggested five base families: arrow/physical puncture; metal bullet spark; arcane burst (cyan/violet variants); spirit puff; web/slime splat (green/violet variants). Shared motion/shape where appropriate; special Clock identity can be decided separately. This is a proposal, not final art count.
- [ ] R03-06 — Footprint, direction, sorting. Agree size relative to target (suggest 25–40% target width with minimum/maximum), impact on torso HitPoint, directional physical strikes vs round magic bursts, effect in front of body but behind damage text. Do not cover HP/mana or obscure the target silhouette. Keep ATK numbers distinct from purple skill numbers: purple art alone must not classify a normal attack as skill damage.
- [ ] R03-07 — Timing and settings. Suggested basic impact4frames /0.16–0.24 game seconds; strong skill impacts separate. Check x1/x2/x3 and pause; Effects toggle must hide optional art without suppressing damage or making essential projectiles unreadable. MythicProjectileArt currently hides the projectile itself when Effects is off; decide whether that should change.
- [ ] R03-08 — Performance/cleanup. Bound and pool new impact instances; no per-hit Resources loads, new materials or registry-wide searches. Keep renderer/material/frame caches. Disable/recycle at phase exit and restart. Broader projectile/VFX pooling remains G05; avoid expanding G03 into a full optimization rewrite.
- [ ] R03-09 — Coverage validation. Check all8 serialized ranged allies,2 ranged Mythics,2 ranged slimes; static/moving target, lethal hit, shield absorption, target death/revival in flight, shooter death, close/long range, left/right facing, phase transition, Effects off and dense volleys. Compare actual HP/ledger/coin/mana outcomes before/after. Preview a few reusable families at real iPhone X combat size before bulk generation.

## Decisions needed before bulk generation

1. Basic projectiles follow their original target, or fixed-point shots can miss? Suggested follow original target; dead target cancels, no retarget.
2. Preserve existing projectile art and add launch/impact only, or redesign some projectile art? Suggested preserve first, identify proven bad art individually.
3. Approve five base impact families above or combine some? Name the Clock variant if unique.
4. Hit on lethal target still gets impact? Suggested yes, unlike suppressed finishing-basic damage number. Already-dead target gets neither damage nor hit effect.
5. Fully absorbed hit: a small shared shield flash, or the same effect as body? Suggested shared shield flash.
6. Any new knockback/stun/slow/DoT/AoE? Suggested none: impact art is presentation and grants no new mechanics.
7. Agree small4frame impacts and target-relative size; whether basic crit gets a slightly stronger same-family impact or no difference. Suggested same basic impact initially, no extra crit asset set.
8. Under Effects off, keep basic projectiles visible, suppress decorative trails/impact? Suggested yes.

## Proposed next steps

Discuss decisions1–2 first → agree hit/shield/death rules → approve family mapping and footprint → build3representative motion demos (physical, magic, splat) → validate timing/contact in-game → generate only remaining approved variants → complete coverage checks. Nothing in this audit is marked implemented.


## Implementation — 2026-11-02

Approved arrow/shield and four-family sheets are installed under Resources/RangedImpacts. Machine orange, Human Epic cyan/blue, spirit teal and Spider white/blue preserve their projectile palettes. RangedShotStyle explicitly maps all eight serialized ranged allies; no new damage/status mechanic is introduced.

Projectile now guides toward the original target, captures death position via LifeEnded before revival/hide, and finishes without damage or impact when that life ends. Contact captures shield state before applying damage exactly once. Weapon release uses facing-aware body-bounds offsets, retaining the existing animation callback and any explicitly assigned muzzle. These offsets still need per-pose visual QA; they are not authored weapon sockets.

Impacts use four frames / 0.24 game seconds, shared cached sprites and a bounded pool of 64. Phase exit and Effects-off recycle them; essential Mythic projectiles remain visible with Effects off.

Validation: runtime C# compilation (160 sources), actual Projectile deterministic regression, target-lock, contribution and effect-buff tests pass. Projectile regression covers moving target, one hit, death endpoint, instant revival, shield, lethal contact, shooter death, allegiance change and phase cleanup. Stubs do not verify renderer alignment, real shield geometry or visual density. Unity import completed; full x1/x3 and per-weapon visual review remains pending.

The four remaining dedicated contact families were approved and integrated: Mythic Elf (lime nature), Mythic Clock (cyan/gold), Green Slime and Violet Slime. Existing projectile/skill art remains. Shared shield contact still takes precedence. Source strips live in artsrc/ui/ranged-impact-remaining-v1; runtime remaining.bytes packs 4 rows × 4 cells, aspect-fit at a fixed center, 256px per cell. All 12 known ranged definitions now have an explicit body-impact mapping.

Earlier checklist is retained as the audit baseline, not a claim that its original defects still all exist. R03-01/03/04 and base art/pool plumbing are implemented; R03-02/06/07/09 still require complete in-game visual coverage.
