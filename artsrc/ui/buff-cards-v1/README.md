# Buff choice cards — 2026-11-03

Installed: generated purple Epic and gold Legendary frames via EffectBuffCardArt. Removed empty scope panels; related Mythic portraits appear only when relevant units exist on the field. Verified both rarities in Unity at iPhone X aspect ratio. Runtime compile and effect-buff regression checks passed.

Proposal images are mockups, not runtime captures. Approved and installed on 2026-11-03. Persistent buff picks stack by level:
- Epic Ember Touch: every fourth basic attack burns for 20% ATK per second for 2 seconds.
- Legendary Mana Flow: +20% mana received, respecting mana-generation locks.
- Legendary Wave Dividend: 3 × completed wave × buff level battle coins after each won wave.
- Legendary Mythic Might: +25% ATK for Mythics.

Generation prompts and source paths: generation.json. Existing instant rewards remain unchanged.

Soul Harvest added: each enemy death grants 1 percentage point per buff level of global allied HP/ATK, committed only at next Prep. No stack cap; resets each run. HUD inspection shows active and pending percentages. Tests cover deferred application, 10,000 kills without cap, restart reset and wave 5 dividend = 15.
