# Four Mythics — animation and logic review

Preview only. No changes to Unity combat, rank enum, save files, loot pools, unit cap or relic progression.

## Generated assets (built-in imagegen)
- elf.png: 20 poses, Idle/Walk/Attack/Skill/Dead, 4 each.
- undead.png: 16 poses, Idle/Walk/Attack/Dead. No active skill; passive resurrection only.
- robot.png: 20 poses, Idle/Walk/Attack/Skill/Dead.
- bear-extra.png: 16 poses, Idle/Walk/Skill/Dead. Existing bear-attack.png adds 4 normal and 4 empowered Attack frames.
Total: 80 poses. References: approved other-mythics.png race sheet, existing Legendary simplicity and approved Bear attack design. Prompt direction: preserve character, full body and weapon integrated, directional motion, consistent scale, no baked full-screen VFX.

## Corrections and validation
- Original generated image files preserved unchanged.
- audit-sheets.py measures alpha components, orders bodies by row and assigns disconnected ornaments to their nearest body (notably Elf crescents). Full bodies counted: Elf20 / Undead16 / Robot20 / Bear-extra16.
- Per-frame runtime clip paths isolate body and its ornaments, excluding neighboring sprites even where generated poses overlap rectangular crops. Bear electricity belongs to the empowered strike frame.
- Fixed foot anchors for Bear attacks and Undead scythe swings, avoiding bounding-box recentering when weapon extends.
- Browser supports each animation, frame stepping and frame gallery. Undead has clickable death cases for Common/Elite/Boss/ally.
- rules.mjs is the same rule module imported by browser demo and test-rules.mjs. node test-rules.mjs passes Boss exclusion; Common/Elite conversion; one ally resurrection despite multiple Undead; duplicate death guard; reset between battles; Human aura15 MP/s/no gain while active/actual damage heal/expiry; unique chain targets; non-compounding Elf buff and shield refresh.
- UI checked: Common/Elite convert; Boss final; ally gains skull then dies permanently; repeated button does not revive corpse.

## Provisional demo balance (NOT approved gameplay values)
Elf +20% ATK/+20% speed,8sec. Bear empowered6sec,2 additional targets,50% ATK chain. Robot shields self/near allies25% maxHP,6sec. These numbers are explicitly marked provisional in UI.
Undead conversion and one ally revival rules are approved; max-HP revival amount, passive source death behavior, wave-vs-run lifetime and interaction with reserved Mythic slots still require integration decisions. Tests do not certify Unity gameplay or performance.

Serve: python3 -m http.server 8794 --bind 127.0.0.1 --directory artsrc/units/mythic-roster-v1
