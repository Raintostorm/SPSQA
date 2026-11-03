# Gear Gunner / machine_rare — pending review

Built-in imagegen using approved Bandit as style reference and existing machine_rare idle for identity. Source poses.png, final crop PNGs, rig.json and idle/walk/attack/dead/full demos are saved beside this document. Full prompts and any targeted fixes are saved in PROMPT.md / FIX_HAND_PROMPT.md.

Red/grey boiler, cyan power core, one wheel, rear-left stack, front-right gun. Same attachments throughout. Charge → recoil shot → settle. Fast straight tracer and brief muzzle flash, no heavy gravity or melee slash. Chassis collapses on wheel.

Six drawn poses plus procedural motion, not eight newly drawn frames. One common scale; grounded pivot except spirit levitation. True alpha values retained: connected silhouettes drive variable bounding boxes, while nearest-silhouette ownership excludes neighbours inside an overlapping weapon bbox without thresholding antialias values. No fixed square animation cells.

Reproduce: python3 tools/preview_simple_bandit.py --directory artsrc/units_simple/machine_rare_trial_v1 --profile gunner --effects --expressive

Original source/edit outputs remain non-destructively in generated_images. This trial is NOT imported; preserve all current Unity assets/IDs/stats. Integrate only after full replacement roster is finished and approved. No skill included for this Rare pass.
