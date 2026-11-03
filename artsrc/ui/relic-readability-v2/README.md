# Relic readability v2

UI changes: wallet48, card levels40/shards36, detail name52/level54/shards46, current buff60/next increment48, gold price64 at1125px reference width. Dynamic stat blocks185px each instead of115, descriptive scope/stat heading above values; Armor uses points, matching TreasureBuffs and RunBuffs rather than falsely promising percent reduction. Existing real dynamic font and light black outline retained. No raster digit atlas was created: text must remain editable and scalable, with consistent decimals and punctuation. A future dedicated TTF/OTF must carry an appropriate redistribution license.

Built-in ImageGen transparent assets, original alpha unchanged:

- lifesteal.png: installed at Resources/StatIcons/lifesteal.png and used for LifeSteal rows instead of HP.
- all_allies.png: reusable scope emblem source, not added as extra decoration to the minimal popup.

Shared prompt: game UI icon asset, HP icon is style reference only. Bold black outlines, flat saturated color, one small highlight, legible silhouette at32px, cartoon fantasy mobile game. Centered square,15% transparent padding, crisp opaque edges, no glow/cast shadow/text/numbers/frame.

Lifesteal prompt: one pink-red heart with a single bold curved red arrow entering from lower right, representing lifesteal rather than HP; no blood/fangs.

All-allies prompt: three friendly faceless chibi hero bust silhouettes shoulder-to-shoulder, center blue helmet, side teal/gold shapes; no facial detail, weapons or text.

Production C# compilation and isolated relic upgrade tests pass. Live visual inspection of the revised layout remains pending in Unity; no player gold/shards were spent by tests.
