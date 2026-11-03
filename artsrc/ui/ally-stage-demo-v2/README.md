# Ally stage approval demo v2

Preview only. No runtime Unity assets, save data or economy were changed.

Five backgrounds generated with built-in ImageGen, using the existing
`Assets/_Project/Resources/HomeWalk/trees.png` as the style reference.

Shared prompt: landscape 3:2 character preview stage; very thick black outlines,
large flat shapes, at most one broad shadow patch per object; sparse scenery;
empty standing oval at x50% y75%; horizon approximately 48%; no units/text/UI;
no painterly texture, intricate architecture, tiny decorations or cinematic lighting.

- Human: blue sky, two clouds, blue mountain shapes, lime trees, tiny castle, dirt oval.
- Elf: turquoise sky, round trees, large leaves, one rune stone, mint grass oval.
- Dead: purple sky, moon, two bare trees, three edge gravestones, gray-purple oval.
- Monster: orange sky, rocky hills, shrubs, short wooden fence, ochre dirt oval.
- Machine: blue-gray sky, simple factory silhouettes, one pipe and cyan lamp, metal oval.

`index.html` is a standalone approval preview. It references original game Idle
frames directly and uses clip durations and sprite pivots from Unity metadata.
It does not use Walk, generated soldiers, or an added CSS bob animation.
HP/ATK/range/interval come from Common unit definitions. Wallet, level and shards
are illustrative. Upgrade does not spend currency. The level 30 display is a
design preview, not a claim that runtime balance was modified.

Rebuild: `node tools/build_ally_stage_demo.mjs` from repository root.
