# INPUT063 animation art
13 independent sprites; PREVIEW ONLY pending user approval. No C# or runtime Resources changed.
Generated sources by built-in image_gen: source/objects.png (gold4-point star, green double upward chevron, gold5-point star), source/confetti.png (goldrectangle/redcurl/bluedisk/greentriangle); truealpha, flat cartoon black outlines with onehighlight/shadow. No animation poses generated.
Reuse same4-point silhouette for smallstar; slateempty rating star derived from goldstar to preserve alpha exactly. Analytic lights: lu-pillar soft column, lu-ground constantwidth ellipse, shine-sweep30degree stripe, glint4point supersampled polygon. All visibleRGBwhite.
Rebuild: python3 tools/build_anim_art_063.py. Import only after actualuserapproval via tools/import_anim_art_063.py. Existing metas preserved, new border0; backup at retired-2026-11-06/before-063/ if overwriting.
Timing: chat/check/063-timing.md. All3GIFs25fps finite sampleslooped for review: levelup.88s,relic.72s,victory1.76s. Character and relic icons are actualgame assets resolved in manifest; use bounds-based runtime positioning rather than preview coordinates.
No Unity/device/performance QA performed.
