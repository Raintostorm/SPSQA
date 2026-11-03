# Review v2 — handedness and readable effects

- Mythic identity is independent of five existing families. UI now uses individual character names, not a fixed race taxonomy. No Unity enum/save migration in this preview.
- Rejected elf-rejected-hands.png after inspection: it reproduced hand switches.
- Accepted new elf-actions-v2.png (8 Attack/Skill poses) and elf-base-v2.png (12 Idle/Walk/Dead poses). All inspected source poses keep staff in far brown-gloved hand; near bare hand performs magic and never becomes gripping hand. Staff remains baked in full character sprite.
- elf-v2-clips.json assembles 20 poses from two original images. Per-source scale keeps body size stable. Alpha components retain floating crescent and exclude neighboring sprites; no source pixels rewritten.
- Bear lightning now has a thick blue outer stroke, bright cyan middle, white core, seeded jagged branches, sequential 75ms jumps, per-target hit rings/sparks and actual demo damage labels/HP reduction. Effect survives recovery pose for readability. Distinct-target chain selection, dead-primary guard.
- Added half-speed playback and frozen lightning inspection button. Frozen inspection is visual only, does not repeatedly grant damage.
- Elf attack visual originates from free casting hand; staff remains held by other hand.
- node test-rules.mjs passes; JavaScript module syntax checked. UI frame gallery for all states remains available.
- Not installed in Unity. Provisional balance values remain explicitly marked in demo.

Preview: http://127.0.0.1:8794/v2.html
