# Local APK attack stretch study / 2026-10-10

Read AnimationClip streamed Transform scale key values from the supplied SpinSquad APK, not a live run. Values below are local transform keys, not guaranteed final screen-space scale after the full hierarchy.

- `1005_bandit_attack.json`: path hash 898039781 scale at 0s = (1,1), at .50s = (.95,1.155), at 1.033s = (1.0875,.95), at 1.5s = (1,1). Separate path 1473615233 rotates Z to 63.444 degrees at 1.033s. This confirms vertical anticipation stretch and a separately rotated slash element; do not rotate the whole model by 63 degrees.
- APK bundle `9b018511a6cf34d45829b765a575d860`, clip `2001_elfarcher_attack`: path 898039781 scale at 0s = (1,1), .417s = (1.15,.8431), .60s = (.95,1.155), 1.10s = (1.0875,.95), 1.5s = (1,1). Confirms horizontal compression/stretch and subsequent recoil.

New original-art trials use gentler authored values and shorter existing timing, not identical source curves:

- Bandit V5: windup X -4%, Y +10%; strike X +6%, Y -4%, clockwise tilt 7–15 degrees (image coordinates: blade moves right/down). Slash endpoint follows the same transformed blade.
- Elf V2: draw X +9%, Y -4.5%; release recoil X -3.5%, Y +6%, then returns to normal. Straight fast arrow, no gravity.
- Feet remain the transform pivot; cropping and base scale unchanged. Walk effect unchanged. Originals preserved in V4/V1.

Reproduce with `tools/preview_simple_bandit.py`, respective `--directory`, `--effects --expressive`, and `--profile elf` for elf.

Pending user review; not imported into Unity. Keep existing approved baseline until stretch trials are accepted.
