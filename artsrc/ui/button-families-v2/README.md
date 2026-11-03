# Button families v2 — installed
Built-in ImageGen; derived from button-families-v1/proposal.png. Source alpha/pixels copied unmodified to Resources/ButtonFamilies/atlas.png. No text/icons/costs are baked into the backgrounds. No Python image edits or alpha reconstruction used.

## Production slices
ButtonFamilySkin creates 8 separate runtime sprites from exact alpha-island bounds. slices.json records top-left source coordinates; code converts them to Unity bottom-left coordinates. Main backgrounds use 9-slice borders; circular Back/info use Simple+preserveAspect. PNG importer uses no mipmaps, no compression, original NPOT dimensions and readable texture. Bounds verified: 8 isolated regions, opaque centers, no overlap.

Families: Battle gold; arched Summon purple; compact amber cost; muted-purple secondary; dark-purple filter; red close/destructive; blue circular Back; navy circular info. Shop retains its integrated blue product price footer. Existing wallet/panel/rarity surfaces are not replaced with button art.

## Usage
BattleUiSkin exposes family sprites with existing procedural fallbacks. Home/Battle primary action uses Battle family, upgrade prices use Cost, Summon has two-line dynamic count+gem price plus separate orb icon, collection sort controls use Filter, info/Back use circles. Relic and shop Back/close use runtime glyphs instead of layering complete old button art. Leave keeps no X, neutral Stay and red Leave. State tint leaves layout and hit areas fixed.

## Prompt
Use case ui-mockup, edit/extract from reference board to reusable transparent button atlas. Exactly 8 isolated blank items in2columns×4rows,40px padding: row1 gold rounded rectangle Battle / purple arched Summon; row2 amber compact cost / muted purple secondary; row3 dark purple filter / red rounded square; row4 blue circle / navy circle. Remove all labels, icons, prices and board. Simplify gloss, shallow base, moderate black outline, minimal upper highlight; no exterior shadow, glow, runes, filigree or texture. Empty centers for runtime content; preserve family identity and stretchable corners.

## Validation
C# compile103 files clean. Relic grant/upgrade and ally summon model tests pass. Live Unity visual checks recorded separately; on-device touch QA remains pending. Actual prototype image is not a runtime atlas.

Live Unity Play verification: Home Battle, Mercenaries filters/Back, ally Upgrade cost/info, and Summon counts/prices render with intact slice edges and no content clipping. Back navigation and opening detail verified without purchases. Screenshots: live-home.png, live-mercenaries.png, live-upgrade.png, live-summon.png. Other modal families compile but were not exhaustively exercised in this pass.
