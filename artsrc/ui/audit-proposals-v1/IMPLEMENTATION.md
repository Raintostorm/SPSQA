# Approved UI implementation

Generated with the built-in imagegen tool. `production-sheet.png` is the text-free transparent derivative of the approved `buttons-icons-proposal.png`. Individual measured alpha cuts are in `cut/`; runtime PNG bytes live in `Assets/_Project/Resources/UiRefresh/`. They are local project assets, independent of temporary/generated-image paths.

Production prompt: export-ready transparent 6-column × 4-row cartoon UI sheet; navy/cyan/gold rounded blank button backgrounds; close/back/previous/next/menu/info; recipe/inventory/dice/crossed swords/coin/key; HP/MP/attack/speed/shield/range. No text or prices. Thick black outlines, flat shading, consistent lighting, isolated sprites. Generated grid spacing was measured manually rather than assumed exact.

Runtime text stays separate. Gold buttons use normal/pressed/disabled sprite variants; other buttons retain short color-tint feedback. Backgrounds use nine-slicing; pictograms preserve aspect ratio and never intercept input. The two critical-stat icons remain the existing set because this sheet does not contain replacements for them.

Relics: currency backing, new control icons and gold buttons, enlarged navigation hit areas, explicit topmost modal raycaster, full-width shard meter, current buff and next-level delta in separate vertical areas. Three-column collection and existing relic illustrations/data remain intact.

Battle: icon-only Recipe/Item controls, same-width cyan Roll and gold Start, separate live coin price, new menu icon. Recipe retains three distinct ingredients and horizontal Mythic selection; the sheet height is reduced from 1020 to 880 reference units. Item inventory still has ten slots. Item recipe footer moved clear of selection labels.

Validation: C# compile passed. Native Unity iPhone X inspection verified new assets on Upgrade, Relics and Battle; Relics next-arrow changed selection successfully; Recipe opened and Item tab switched. No new sprites were substituted for existing character art, and no currency transaction was needed. Physical-device touch and every gameplay/result flow have not been re-certified in this pass.
