# Shop v2 — installed 2026-10-25

Built-in ImageGen. Sources and candidate UI kit are saved here pending approval, not imported into Resources or used to change live layout. merchant-header.png is RGB; shop-kit.png / shop-kit-clean.png retain generated RGBA unmodified. The cleanup candidate still needs final edge/alpha QA and slice validation before production integration.

## Prompt set

Header: horizontal3:1 merchant stall illustration for simple flat2D faceless chibi medieval mobile game. Friendly faceless human with orange bandana, teal tunic, brown apron behind counter; shelves of parchment/potions, one lantern, crates; muted purple tent; top15% quiet wallet area. Thick outlines, broad colors, no UI/text/counters, no raccoon/panda, no weapon clutter or light rays.

Kit: transparent2x2 sheet: ivory tall product card with integrated blue bottom22% price strip; matching gold strip card; blank charcoal clipped-end section plaque with2gold rivets; blank purple footer. Simple black outlines, modest rounded corners, minimal bevel,15%padding, no text/icons/filigree. Cleanup prompt preserves objects and requests removal of exterior shadows/haze/glow.

Demo: portrait9:19.5 screenshot using header and kit as references. Fixed merchant/wallet top, Daily Shop plaque/countdown,9cards3columns3rows, all neutral ivory outer cards; names above portraits, quantities over portrait corner, purple-gem costs on integrated blue strip. Gold section begins below. Fixed Back/Shop footer, scroll indicator. No ads, discounts, newbie packs, paid reset, extra currencies or Skin tab. Initial demo slime ally corrected to purple Spider (slimes are enemies); rarity colors corrected beige/blue/purple, footer glow removed. Portraits/names/countdown are illustrative; production should use existing actual catalog art and live data.

## Scroll check / scoped fix

DailyShopScreen previously left default ScrollRect scrollSensitivity at1; now80, vertical explicitly enabled, clamped movement/inertia. Added transparent content hit surface so gaps receive drag input. This is a code-side input fix, not evidence of a successful live gesture. Unity CUA connection timed out during attempted visual verification. Compile and daily-shop logic tests pass. New UI assets/layout remain preview-only.

## Installation and live verification (2026-10-25)

Installed merchant-header.png and shop-kit-clean.png into Resources/ShopV2, preserving generated pixels and alpha. ShopUiSkin slices the atlas; DailyShopScreen uses actual catalog portraits, live wallet/countdown and existing purchase logic. Daily Shop precedes Gold, with a fixed merchant header and footer.

The main layout bug was center-anchored product children inside a top-pivoted content rect. Anchoring direct content children to the top removes the large blank area and makes the measured scroll bounds include all products. Content is 2380 units and viewport 1740 units (640 units travel). Live Unity wheel input reached position Y=640 and visibly exposed Gold and Test Gems at the bottom. Screenshots: live-top.png and live-bottom.png. Native automated drag did not visibly move the list; touch-device dragging remains unverified. Earlier preview-only notes above describe the previous review state.
