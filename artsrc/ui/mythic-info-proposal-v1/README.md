# Mythic information and inventory approval mockups

Current screenshots were captured in Unity Play Mode on the existing iPhone X preset. The stage 1-2 starter ingredients were fused only in the current battle to open the real owned-Mythic screen. The current inventory is empty at wave 1. No runtime code was changed for these proposals.

Generated using built-in imagegen. Prompt set:

1. Use the real current Mythic screenshot as reference; preserve simple faceless knight and navy/cyan/gold cartoon style. Owned Mythic information only: header/close, idle display, mana bar, eight stats, compact Dawn Aura block with 20% ATK per second, 50% aura-damage healing, 15 MP/s and no mana gain while active, then equipped-item effects. No fusion or upgrade controls. iPhone X portrait. Mana Heart/Twin Fang are illustrative equipment; the displayed stats are sample layout values, not a verified calculation for those items.
2. Use current inventory screenshot as reference; preserve battle layout and small allies. Exactly ten inventory slots, two rows of five; sample Mana Heart, Kunai, Mana Stone. Selected-item effect tooltip below the ally, valid-target ring, equipped badge/check and short success message. Maintain icon-only Recipe, Roll-price and Item dock. iPhone X portrait. Populated inventory and equip feedback are proposed states, not actual grants.

The generated redrawn character/item art is reference-only and does not replace existing runtime sprites. Final implementation should reuse actual sprites and compute all stats from the unit being inspected. Empty inventory should have a concise empty-state message; successfully equipped items should leave the unequipped inventory unless another copy remains.
