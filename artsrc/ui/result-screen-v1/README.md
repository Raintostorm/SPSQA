# Result screen v1
Approved board: proposal.png. Built-in ImageGen extracted four reusable sprites; original RGBA copied unmodified to Resources/ResultScreen/atlas.png. Alpha analysis confirms four separate components. Exact runtime cuts in ResultScreenArt.cs, top-left bounds in slices.json. No raster edits/alpha reconstruction.

## Prompt
From approved preview: genuine transparent 1536x1024 atlas, four groups in 2x2 grid: empty gold victory ribbon, empty purple defeat ribbon, three faceless chibi allies celebrating on small grass patch, faceless skeleton resting with sword/shield on grass. No text, numbers, UI panels, confetti, rays, aura, backdrop or disconnected sparkles. Flat clean black outline style, blank faces, isolated groups and transparent gutters.

## Runtime
Opaque navy full-screen result with safe-area fitted content, decorative low-contrast diamonds, separate generated ribbon/illustration. Runtime labels include chapter, cleared/total waves, game-time duration, three currency reward cards on win and one empty reward panel on defeat. Currency icons reuse HomeChrome/Summon assets. Gold primary Next Level or Retry (final level), purple Home below. Result actions retain existing routes and Time.timeScale reset. GameProgress.Clear settlement remains unchanged; renderer never grants resources. Generated illustration is decorative, not a claim about the actual deployed squad.

## Verification
106 source files compile clean. Live Unity previewed both outcomes using editor diagnostics with no reward grants/save calls; preview victory amounts come from configured rewards/current level. Actual iPhone X Game view: no content clipping, banner/illustration alpha clean, reward icons correct, final-level Retry label correct. Retry reloads battle preparation; Home navigates Home. Screenshots live-victory.png, live-defeat.png are layout previews, not completed battles. No fake outcome used to award progress. Narrow-device touch and actual level-end settlement remain separate coverage; settlement code was not changed.
