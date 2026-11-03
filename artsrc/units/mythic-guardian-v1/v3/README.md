# Mythic v3 — Skill / Dead repair

New imagegen sheet skill-dead.png contains weaponless character poses only, with clear spacing. Original output preserved. Uses v2 character identity as reference; prompts enforce consistent fall toward screen right (helmet right, feet left) and connected limbs.

Sword is a single canvas geometry with fixed dimensions in all Skill/Dead poses, with per-pose hand anchors and rotation. No blade generated independently per frame, so length is invariant. This is a separate render layer, not an edit to the bitmap.

bounds.json uses measured alpha boxes within hand-inspected row boundaries (the generated sheet is not evenly spaced). Correct boundary is y=500, not half image height; splitting at half previously included adjacent-row fragments. Base Idle/Walk/Attack retained. All crops clear canvas every rendered frame.

Added frame stepping controls so Skill and Dead can be inspected pose-by-pose. Unity unchanged. Preview: http://127.0.0.1:8793/v3/
