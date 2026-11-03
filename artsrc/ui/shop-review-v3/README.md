# Shop review and melee VFX audit

Current comparison uses the actual prior Shop screenshot live-top.png. It is not a newly captured runtime screen; a few later wallet/portrait changes may differ. index.html displays its viewport using CSS crop, never AI-regenerates the old side. Proposed art was generated with built-in imagegen, preserving merchant,9 daily offers/3 columns and sold state. Improvement targets: separate quantities, muted sold-out controls and navy page/footer. Generated portraits are illustrative; runtime should keep original assets.

Audit:20 ordinary ally animation assets all have empty AttackVfx clips. This does NOT mean no VFX: importer bakes effects directly into body frames. Inspected frame6(contact) and frame8(trail):10/12 melee allies visibly have baked cues. Machine Common and Epic lack clear contact slash/smash. Preview exporter excludes profile'machine' attack effects as if ranged even though machine_common definition is melee: this explains missing saw sparks and should be addressed in a future fix. Machine Epic is also flagged melee despite cannon-like art; use appropriate impact rather than sword trail.

ChapterOneSlimes creates fresh animation sets without AttackVfx: amber,stone,elite,boss use bite sheets; green,violet are ranged with projectile overrides. Bite motion exists; no separate impact VFX assigned. Guardian uses approved baked slash frames. Mythic skill/item effects are separate and should not be mistaken for basic-attack effects.

Preview only: no gameplay/UI code changed this turn.
