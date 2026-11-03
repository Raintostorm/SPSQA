# In-battle unit information implementation

Applied shared BattleUnitInfo to the preparation cell Info button and owned Mythic portraits. Ordinary allies show their own basic/Legendary skill and cannot equip items. Includes idle preview, eight live combat stats, MP state, skill description, equipped item names/effects and a scrollable equipment list.

Inventory keeps ten slots. Selecting an item highlights it and displays its actual effect. Successful drag equipment adds a short green acknowledgement following the unit.

Validation: C# compilation passed. The actual Common Skeleton Warrior screen was inspected in the iPhone X Game viewport (implemented-common.png). This caught and fixed the preparation-phase speed getter accessing an uninitialized cached definition. Mythic detail and the new equipped acknowledgement still need an interactive runtime pass; native Unity automation became unresponsive during that pass. No physical-device validation was performed.
