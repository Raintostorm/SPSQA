# Human legendary v5 — rejected by user, do not import

Correction: frames2-4 have an extra low navy fist while the navy sword hand
is raised overhead. This creates THREE arms; the earlier review missed it.
Superseded by v6, which removes the extra low navy limb. Await v6 approval.

Candidate source: `artsrc/fixposes/human_legendary_attack_v5.png`.
Generated with the built-in image tool using only the original model sheet.
Prompt: `artsrc/fixposes/human_legendary_attack_v5.prompt.txt`.

One overhead downward slash, same navy sword arm throughout. Gold arm under
lion shoulder stays low/free. Eight unique poses, no square cell clipping.
Shared PPU 269.1 estimated against current idle height (183 px at 184.81 PPU).
All 472,631 alpha pixels retained exactly once; alpha sum 105,201,420.
Explicit anatomical roots and timings stored in the regions manifest.
Complete silhouette ownership, including soft alpha and detached pieces;
large unclaimed pieces trigger failure. Eight extraction regression tests pass.

v3 and v4 were rejected: arm swap / armour recolour instead of correct arm
mechanics. They are NOT imported. v5 preview uses a fixed camera and shared
scale; no independent fit-to-box. Ignore faint alpha specks only for preview
camera extent, never in exported sprite files.

User asked for one character at a time and approval before the next.
Human-epic approved. This candidate has NOT replaced human_legendary game
assets. Next step is user review of `preview.gif`, not another character.
