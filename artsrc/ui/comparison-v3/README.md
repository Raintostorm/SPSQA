# UI comparison preview

Current images are genuine Unity Game View screenshots captured at 2734x1642. The comparison displays only the game viewport through CSS clipping; it does not redraw the current design. Proposed screens are concept art only, not implemented gameplay. Contribution figures are illustrative. Character illustrations are layout placeholders; keep actual runtime sprites when implementing.

Built-in imagegen was used, not the API/CLI fallback. Reference images: each corresponding current screenshot and the approved BattleChrome v2 atlas.

## Saved images and prompt set

- recipe-proposal.png: portrait iPhone X Mythic Recipe redesign; navy/steel-blue/cyan palette; exact three ingredients (one Crescent Mage, two Sickle Militia), readable Summon and 0/2 cap, horizontal Mythic list, visible battlefield above sheet and approved book/dice/bag dock. No Unity editor chrome, purple buttons or unnecessary paragraphs.
- item-proposal.png: portrait Item Recipe redesign; Kunai + Mana Stone = Runic Kunai; consistent isolated icons without opaque squares; on-kill slash 20% ATK, secondary attack speed +10% and starting mana +20; three completed items and clear selected state; battlefield above panel, approved bottom dock.
- settings-proposal.png: portrait Settings overlay; coherent navy/steel family; three stacked rows Sound/Damage Numbers/Effects with readable ON/OFF switches; no empty desktop Vibration position; gold Resume and red close X.
- contribution-proposal.png: portrait Contribution overlay; damage dealt/taken tabs, All/Legendary/Mythic filters, proportional ranking bars with explicitly marked sample values, collapsed secondary roll and skills summaries, coherent navy/cyan family.

Old screenshots: recipe-current-full.png, item-current-full.png, settings-current-full.png, contribution-current-full.png.

Editable comparison template and builder are saved in this directory. Inline comparison fragments are in the thread-scoped visualization directory, under ui-compare-recipe/settings/item/contribution.html. The primary viewing control switches between side-by-side and enlarged stacked images. Display JPEG encoding only reduces payload; original PNG files remain intact.
