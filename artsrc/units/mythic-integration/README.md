# Mythic integration

Eight approved identities. Fusion uses units already on the preparation board, not collection shards. Each consumes exactly one Legendary, one Epic and two identical Rare or Common units. No Mythics in basic rolls or gem summons.

- mythic_guardian: human_legendary, human_epic, human_common, human_common
- mythic_undead: dead_legendary, dead_epic, dead_common, dead_common
- mythic_elf: elf_legendary, elf_epic, elf_rare, elf_rare
- mythic_bear: monster_legendary, monster_epic, monster_rare, monster_rare
- mythic_robot: machine_legendary, machine_epic, machine_common, machine_common
- mythic_clock: elf_legendary, human_epic, dead_rare, dead_rare
- mythic_mimic: dead_legendary, machine_epic, monster_common, monster_common
- mythic_swordsman: human_legendary, elf_epic, human_rare, human_rare

Capacity: base 2, plus floor(Astral Contract relic level / 2); relic unowned contributes zero. One Mythic per board cell. Astral Contract is Legendary, max level 10, manual shard/gold upgrades.

Balance values carried from the approved demos are provisional: Oracle +20% ATK and attack speed for 8s; Bear charged attacks 6s, two additional targets at 50% ATK; Robot refresh shield 25% max HP for 6s within radius 2; Mimic 300% ATK and heal 50% actual damage; Ronin dash 200% ATK, one hit per enemy. Guardian aura radius 1.2, 20% ATK each second, heals half actual aura damage, consumes 15 mana/s with all recharge blocked while active. Clock field radius 1.5 freezes movement and attack for 5s. Mimic resolved cast earns 5 battle coins, paid once at the next wave even if caster dies.

Built-in imagegen generated support VFX, combat VFX and capacity relic icon. Source PNG bytes preserved unchanged in Assets/_Project/Resources/Mythics/*.bytes; runtime cuts use audited component masks and stable foot pivots.

Prompt set: 4x4 transparent support atlas (aura, resurrection, team buff, shield); 4x4 transparent combat atlas (lightning hit, green bolt, time bolt, skull seal); one simple gold/purple summoning medallion. No characters or weapons added to VFX.

Validation: 53 runtime Unity checks passed (audit.txt); C# compile passed. Regression checks passed for summon, relic upgrades, daily shop and contribution. Deferred-wave coin ledger tests passed. All 160 pose regions validated against source bounds. Fusion list was checked at top and bottom in Unity Game View.

## Mythic readiness and details
During preparation, a horizontal tray automatically shows Mythics whose exact board ingredients are complete. Tap a card to inspect the animated idle preview, eight combat stats, skill, mana rules and ingredient counts, then explicitly Fuse. Reaching the cap keeps the matching card visible with a blocked reason. Fusion consumes three distinct live board units and plays the existing arrival reveal. The runtime audit passes 54 checks, including ingredient readiness at a full Mythic cap.

Recipe rules revised to exactly three allies (2026-10-28); see RECIPES.md. The previous 54-check runtime report predates this rule revision. Current revision passes the C# compile check; runtime audit must be rerun after Unity refresh.
