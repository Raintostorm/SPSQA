# Legendary skill art v4

Generated with built-in imagegen using each actual Legendary skill_010.png as style reference. Effect-only sheets; character sprites unchanged. All four sheets are RGBA 1536x1024, six cells in a 3x2 layout with genuine alpha. Imported via tools/import_legend_skill_art.py into Resources/SkillVfx and connected through LegendSkillArt / LegendaryCombat. Human retains radius 1.1; shields follow recipients for up to eight seconds, refreshing without stacked artwork. All new effects stop on death or leaving combat. Retain earlier assets for rollback.

## Prompt set

Shared: production 2D mobile-game VFX, transparent background; exactly six equal 512x512 cells chronological left-to-right, top-to-bottom; fixed centre, generous gutters; reference character for style only, do not redraw character. Flat saturated cartoon colours, bold dark outlines, rounded chunky shapes, minimal two-tone shading. No text, UI, environment or photorealism.

- Elf: faint cyan ellipse → rune circle lights → gold arrows with ice tips descend → arrows impact and ice shards inside circle → icy pulse → fading shards. Action bounded by one spell circle.
- Human: tiny ground flash → gold impact star → concentric elliptical rings and chunky brown rock chips → airborne dust → fading rings and dust → motes. Compact, centred at feet, no giant crater.
- Machine: cyan arcs → oval bubble forms → transparent bubble with shield emblem → bright pulse → stable bubble → dim pulse. Keep interior nearly transparent so recipient remains readable.
- Monster: faint lion silhouette → head emerges → cartoon lion roars with two sound waves → strongest roar with three waves → fading head and sparks → wisps. Orange mane, cream muzzle, round ears matching actual Monster Legendary; no body, realistic fur or demon.

Dead retains approved `../dead-current-v3/hand-magic.png`; no replacement character art.
