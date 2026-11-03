# Three new Mythic candidates

Preview only, built-in imagegen using existing human_legendary idle as art simplicity/proportions reference. Prompt: horizontal three-character lineup, flat faceless chibi, complete integrated equipment, transparent; Clock Mage with simple pocket-watch staff, squat friendly-dangerous Mimic Chest, Masked Swordsman with one curved saber. No text, scenery, ornate textures or floating particles.

Fusion rule confirmed by user: exact-character recipe, mandatory 1 Legendary + 1 Epic + (2 Rare OR 2 Common). Four unit ingredients total. Individual ingredient identities and character skill values not assigned. Mythic characters are not restricted to the five existing families. No Unity/gameplay integration.

## Animation/effect preview
- clock.png, mimic.png, swordsman.png: generated source sheets, 20 poses each (Idle/Walk/Attack/Skill/Dead). All source bytes preserved.
- clock-skill.png replaces original Clock skill poses that swapped staff hands. clock-final-clips.json assembles final20 with fixed far-hand staff cast and per-source scale.
- effects.png: generated16 VFX frames, 4 each time field, devour vortex, healing swirl, dash-slash.
- Runtime alpha-component masks remove background glow and exclude adjacent body fragments; original full images not rewritten. Disconnected ornaments are assigned to closest intended body. Effects use hand-checked gutter boundaries.
- index.html presents all3 characters, five animation states, frame stepping, half speed, source gallery, effect gallery and frozen skill/effect inspection.
- Demo mechanics and tests share rules.mjs. Boss is damaged but never pulled/swallowed; actual damage determines healing with HP cap. Dash has enemy-only corridor and per-cast hit set. Time-stop modifiers are derived from position/team/time, never multiplied permanently into stats.
- Browser verification: Elite devour damage250/heal125; Boss damage300/heal150 with no pull. All numbers provisional; no Unity integration.

Run: python3 -m http.server 8795 --bind 127.0.0.1 --directory artsrc/units/mythic-newcomers-v1
Preview http://127.0.0.1:8795/

Final inspection features: frozen skill/effect view labeled visual-only (does not execute damage), normal playback restored on pause toggle/replay/slow-speed switch. Clock source handedness repair and artifact-free Mimic render visually checked. Generated effect frames are used directly, not substituted with CSS-only effects. Source sprites and VFX are separate layers; weapons stay integrated with character source frames.

## Updated rules — time stop and delayed Mimic coins
Clock field stops movement and attack (multipliers0), exactly5 seconds while in range; outside/expired returns1. Mimic earns5 per unique valid resolved cast (including Boss-target casts that deal damage), pending until next-wave button, then pending clears. Frozen inspection never earns coins. Replay creates a new test cast without clearing pending coins; changing character preserves demo ledger. No real Gold/coin save modified. Tests verify 3 casts→15 next wave, duplicate cast ignored, subsequent empty wave pays0.
