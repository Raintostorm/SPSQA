# Relic skin v1

Built-in ImageGen, transparent output, original PNG alpha unchanged. atlas.png is the source copy; runtime copy is Assets/_Project/Resources/RelicSkin/atlas.png. Sprite.Create measures original opaque bounds (read-only image analysis); no pixel cleanup/repainting. Nine-sliced sprites retain rounded corners for different button sizes. Atlas is imported at original NPOT resolution.

Prompt: Production mobile cartoon fantasy game UI sprite atlas, exactly two columns/four rows. Gold horizontal button, blue horizontal button; red square button, neutral light-gray square panel; left-chevron, right-chevron; white X, neutral horizontal progress-track. Flat simple vivid colors, thick black outlines, strongly rounded corners, modest top highlight and bottom dark edge. Transparent padding; no words, numbers, ornate filigree, gems, wood grain or exterior glow/shadows.

Applied to Relics buttons/cards/detail panel/progress. Navigation and close use generated atlas icons. Stat icons reuse the existing generated Allies StatIcons set for consistency. Wallet coin/key assets remain the shared HomeChrome icons. This does not replace unrelated battle/home UI.

Detail panel height now follows stat count, eliminating the large empty gap above its price button. No economy changes in this skin pass.

Verification: production C# compile and isolated relic upgrade economy tests pass. Live visual Play Mode verification remains pending in the user's open Unity editor.
