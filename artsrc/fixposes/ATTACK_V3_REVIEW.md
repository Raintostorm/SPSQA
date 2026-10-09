# Attack repair v3 — first clip imported

human_epic is the first repaired candidate. Original reference:
`20model/human/epic/assets/sheet-proportions-v2.png`, third row only.

Generated with the built-in image tool, real transparent alpha. Identity:
red curly hair, purple crescent hat, teal cape, gold crescent/cyan staff.
Eight steps: ready, anticipation, charge, acceleration, release,
follow-through, recovery, settled ready. One cast, no neutral reset midway.
Complete staff, hat, cape and boots, generous transparent separation.
No magenta key removal, no square cells, no per-frame fit-to-box.

`human_epic_attack_v3.regions.json` contains explicit source-space regions,
polygon separators through transparent gaps, anatomical roots, a shared PPU
and timing. Shared PPU 342.8 matches the approximate hat-to-boot height of
the current idle. Art/extraction review passed; Unity runtime review is still
pending. All 598,983 alpha pixels retained, alpha sum 129,895,694 unchanged.

Run:
```
python3 tools/review_attack_clip.py artsrc/fixposes/human_epic_attack_v3.regions.json outdata/attack_review/human_epic_v3
```

The preview uses one fixed root/camera/scale. Extraction retains every alpha
pixel exactly once, including detached parts. Do not import earlier v2
assets using the old largest-component importer. Existing game assets have
been updated for human_epic attack only, with their existing GUIDs preserved.
Idle/walk/dead/skill/VFX sections were checked byte-for-byte unchanged.
Backup: `outdata/attack_import_backup_20261008_171024`.

Remaining attack repairs: human_legendary, dead_common, dead_epic, elf_rare,
elf_epic, machine_epic, monster_common, monster_epic, monster_legendary.
Then audit all other attacks against original motion and current timing.
