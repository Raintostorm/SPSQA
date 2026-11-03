# Mythic v7 — combat direction, bear attack and skill specifications

Preview only; no Unity gameplay integration.

Guardian movement no longer oscillates backwards. Advances only during walking and stops during attack. At forward limit uses idle between attacks. Added per-pose foot anchors so changing sprite bounds (extended sword) does not recenter body backwards.

Bear has generated normal and empowered attack sequences (4 poses each). Same integrated body/hammer. Hammer lights up in empowered art. Chain-lightning presentation runs only on impact pose. Actual gameplay multipliers, target count and skill duration remain unconfirmed. Source image preserved. Runtime alpha component clip paths isolate each character from neighbor overlap in packed generated sheet; detached electricity is assigned to empowered impact. Per-pose foot anchoring avoids bounding-box recentering from extended hammer.

Skill requirements recorded:
- Human: existing mana-draining damage/healing aura.
- Undead: no active skill; enemy deaths resurrect as allies; allies get one skull-marked resurrection then true death. Confirmed: only Common and Elite enemies can convert; Boss excluded. Each ally can resurrect only once per battle, regardless of how many Undead Mythics exist. Skull mark identifies the consumed resurrection; never grant another resurrection to the same unit.
- Elf: whole-field allied ATK/attack-speed buff, values/duration/stacking pending.
- Bear: skill changes hammer visual and attack to chain lightning, values/duration/target count pending.
- Robot proposal: shield nearby allies. Values and activation pending.

Four approved other race visuals retained. URL http://127.0.0.1:8793/v7/
