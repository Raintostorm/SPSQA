# 20 model sprite — 5 races × 4 ranks

Source: active-roster.json, canonical character assets. Human, dead, elf, monster, machine; common, rare, epic, legendary. Each race/rank folder contains character.json and the exact original referenced PNGs under assets/. All have idle, walk, attack, dead; legendary also includes the saved skill animation. Use manifest sources, rect, parts, layers, anchors, normalization and frame timing together; do not assume every animation is a uniform sprite strip.

These are 2D sprite assets plus animation pose/timing data, not rigged 3D models or Unity .anim clips. This export does not integrate them into the Unity project. Animation review statuses are retained verbatim, including pending-review states. Legendary skill v1 motion approval is preserved; illustrative balance parameters are not gameplay approval.

Size policy: common 80, rare 84, epic 88.2, legendary 92.61 rendered idle silhouette pixels, rank multiplier 1.05. These are render targets, not identical source PNG resolutions.

roster.json records all 20 entries, source hashes and animation counts. Rejected cutout rigs, generated drafts and historical QA are excluded. Canonical sources were not modified.
