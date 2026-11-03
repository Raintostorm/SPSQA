# Relic Open All results v1
Approved preview: proposal.png. Installed UI uses existing generated gold ribbon (ResultScreenArt), current production relic icons, native rounded cards and gold primary Continue. Text, counts and NEW labels are runtime data, not baked into an image.

## Runtime behavior
OpenEveryKey calls ShowRelicResults after existing grant/persist flow. Groups by item ID; rarity descending then name; total/quantities sum actual ShardsGranted using long. MAX grants remain zero and show MAX, first acquisition uses any IsNew result in group. Three columns; vertical clipped scroll with indicator, fixed summary/Continue; safe-area scale. No auto-upgrade, rates, extra prices, second purchase button or claim transaction added. Relics grant one shard per roll; ally summon unchanged at20. Prior saved shard counts not rewritten.

## Icon layout
Original30 item PNGs are unchanged. Read-only alpha measurement produced Resources/RelicResults/icon-bounds.json. RelicResultIconFit scales and centers visible bounds into260x220 logical icon area, preserving aspect. This avoids both tiny icons due to transparent padding and cropping tall staffs. Missing metadata falls back to regular aspect fit. No raster edits or alpha reconstruction.

## Verification
108 source files compile clean; relic manual-upgrade/grant regression passes (1 new/duplicate, actual total, MAX0, gold+shard spend). Unity diagnostic fixtures call renderer directly without Roll/SpendKeys/Save. Nine-type25-shard preview verified totals, duplicate counts and NEW.30-type preview verified MAX0 exclusion (29 total), rarity colors, clipped vertical scrolling and visible scroll hint. Continue returns to collection; wallet remains167,000 gold/6 keys. live-nine.png and live-scrolled.png are fixture UI screenshots, not real rolls. Actual Open All purchase was not used for layout verification; device touch remains pending.
