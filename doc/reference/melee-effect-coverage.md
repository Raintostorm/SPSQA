> Trạng thái 2026-11-03: các family đã gắn; người dùng đã xác nhận effect ở x1/x2/x3. Phần audit và đề xuất ban đầu bên dưới là lịch sử, không phải yêu cầu generate lại. Căn weapon socket từng pose và hiệu năng trên thiết bị vẫn cần QA.

# Melee effect coverage audit — 2026-11-02

Read-only gameplay audit. No combat/VFX changes in this pass. Scope: 20 ordinary allies, 8 Mythics, 6 Chapter One enemies = 34 definitions. 12 ranged mappings are installed; 22 definitions are melee.

Evidence: serialized Unit/Anim assets, UnitBrain.OnImpact, FrameUnitVisual attack VFX path, MythicCatalog runtime clips, MythicCombat.AfterBasicAttack, ChapterOneSlimes runtime layouts. Contact sheets extracted from the actual referenced art live in artsrc/audits/melee-effects. Ordinary sheets sample 5 frames per attack; Mythic sheets show all 4 basic frames and Bear empowered frames; slime sheet shows all 6 bite frames. This is source-art/code review, not full x1/x3 visual validation in battle.

No generic melee contact VFX is spawned by UnitBrain.OnImpact. Its melee path applies damage and invokes Mythic.AfterBasicAttack. All 12 ordinary melee animation assets have empty AttackVfx clips. Ordinary sampled art has weapon motion and occasional faint sweep lines, so empty AttackVfx alone must NOT be described as no visual motion/effect at all.

## Ordinary melee: no dedicated clear impact; improve readability

- Sickle Militia / human_common: short sickle sweep.
- Lion Spear Knight / human_rare: thrust/contact spark, not a broad sword slash.
- Royal Hero / human_legendary: sword sweep; existing shockwave skill is separate.
- Leaf Blade / elf_rare: light sword sweep.
- Skeleton Warrior / dead_common: light sword sweep.
- Skeleton King / dead_legendary: heavier sword sweep; summon skill is separate.
- Scrap Crawler / machine_common: saw contact sparks.
- Furnace Crawler / machine_epic: close-range barrel/fire/metal contact; definition is MELEE despite gun-like silhouette. Do not silently convert to ranged.
- Titan Pugilist / machine_legendary: punch/smash contact; shield skill is separate.
- Armored Bear / monster_rare: claw/short swipe contact.
- Lava Scorpion / monster_epic: pincer/stinger contact.
- Lion Warlord / monster_legendary: mace smash; roar skill is separate.

## Mythics

- Dawn Guardian: clear golden slash baked into basic frame 3; no duplicate slash needed.
- Grave Warden: scythe swing, lacks distinct basic slash/contact effect. Revival VFX exists and is not an attack effect.
- Aegis Engine: punch motion, lacks distinct basic smash/contact effect. Shield VFX exists.
- Mimic: chest closing/bite motion, lacks distinct basic contact effect. Devour/heal skill VFX exists.
- Masked Ronin: katana swing, lacks distinct basic slash/contact effect. Dash skill has a separate slash VFX.
- Thunder Bear: base hammer swing lacks distinct smash contact. Empowered attack DOES have a glowing/electrified hammer plus electric impact/chain VFX; preserve these, do not duplicate.
- Moonwood Oracle and Chronomancer are ranged; covered by the approved ranged families.

## Enemy melee

Amber, Stone, Bramble (Elite), Slime Overlord (Boss): all have explicit toothless open-mouth bite frames, but no dedicated bite/contact VFX. Their attacks are readable by pose; a small compression/contact puff is a proposal, not a requirement to add teeth or large slash arcs. Boss double attack uses the same attack clip and has no separate large contact effect.

## Suggested next scope

Prioritize ordinary sword/sickle + spear + punch/hammer/saw families. Then fill 5 Mythic base-hit gaps. Preview a very small bite puff last and decide whether it improves readability or merely adds noise. Never duplicate Guardian's baked slash or Bear's empowered electric effects. Do not change damage timing, attack count, status effects or melee/ranged definitions during art integration.

## Preview review status

User approved first four prototypes (Sickle Militia, Lion Spear Knight, Titan Pugilist, Scrap Crawler) at melee-effects-review-v2: larger effects, 0.40s default and 0.48s blunt contact. Approval recorded; integration not yet performed in this preview task. Remaining 17 definitions now have source-art side-by-side animated proposals under artsrc/ui/melee-effects-rest-v1; pending user review. No combat code changed by these previews.


## Integration — approved 2026-11-02

Installed all approved melee families and Royal/Leaf/Skeleton palette variants. MeleeAttackEffect maps 21 melee definitions; Guardian excluded because its slash is baked, Bear empowered attack excluded because its electric contact already exists. Effects run once at the existing attack impact callback, independently of damage: a committed harmless swing still displays its weapon effect if its victim died. Damage logic is untouched. SourceFacesLeft is accounted for separately from transform reflection. Shared bounded sprite pool, cached four-frame clips, .40s normal and .48s blunt timing, held contact frame and fade-out. Ranged timing remains .24s.

Validation: 161 runtime C# sources compile, target-lock and actual Projectile regressions pass. Full in-game per-weapon alignment, shield presentation and dense x3 combat visuals remain to be checked; offline comparison GIFs do not establish these.
