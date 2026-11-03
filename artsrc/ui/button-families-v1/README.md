# Button families v1 — proposal only
Built-in ImageGen, 2026-10-26. No production changes. proposal.png is an unmodified review board, not a runtime sprite atlas. Text, icons, prices and shadows must be separate from scalable button backgrounds when implemented. Pressed state must depress a button without changing its layout/hit area; the board's differently sized examples illustrate style, not literal runtime sizing.

## Prompt specification
Use case ui-mockup. Landscape review board of 4 rows for simple flat faceless chibi medieval mobile game. Matte muted purple backdrop, thick near-black outlines, broad cheerful fills, restrained upper highlight. No ornate filigree, realistic metal, neon, excessive bevel or hexagonal end caps. Families differ in construction/silhouette rather than recoloring one capsule.
1. BATTLE: wide gold rounded rectangle, crossed swords, bold BATTLE; adjacent depressed pressed-state example.
2. SUMMON: two purple subtly arched buttons, simple orb inset left, Summon ×1 with gem10 / Summon ×10 with gem100 on separate cost lines. No chance table or shard price.
3. UPGRADE / SHOP: compact amber coin2,000 cost-only button, legible muted disabled counterpart with Not enough shards outside; ivory product-card bottom with integrated blue gem75 strip.
4. NAVIGATION / FILTERS: circular blue Back arrow, rounded square berry-red X, dark-purple Rarity/Level filters with distinct selected edge, navy circular i, leave confirmation with Stay neutral purple / Leave red and no X.
Bold rounded white game type, restrained outline. Green reserved for readiness/status. No characters, phone frames or new game systems.

## Next step after approval
Produce separate transparent background sprites for approved families and slice/validate stretch borders. Use existing currency/character assets and real labels; do not import the whole board as UI. Test normal, pressed, disabled, selected and sold states at real touch size.
