NHAN    = {"Common":3,   "Rare":6,  "Epic":10, "Legendary":16}
XACSUAT = {"Common":1.5, "Rare":3,  "Epic":10, "Legendary":14}
MAXLV   = {"Common":10,  "Rare":8,  "Epic":6,  "Legendary":5}
PROB = {"Chí mạng","Hút máu","Giáp"}

# (ten VN, bac, [chi so], mo ta hinh bang tieng Anh cho cong cu sinh anh)
ITEMS = [
 ("Chipped Wooden Cup","Common",["ATK"],"a chipped wooden drinking cup, plain and worn"),
 ("Dented Tin Mug","Common",["Máu"],"a dented tin tankard with a simple handle"),
 ("Oak Staff","Common",["ATK"],"a plain oak walking staff, bark still on it"),
 ("Burlap Cloak","Common",["Giáp"],"a coarse brown burlap cloak with a rope tie"),
 ("Terracotta Seal","Common",["Máu"],"a small terracotta clay seal stamp"),
 ("Woven Grass Charm","Common",["Tốc đánh"],"a charm braided from dry grass and twine"),
 ("Bronze Badge","Common",["Chí mạng"],"a plain round bronze badge, slightly tarnished"),
 ("Iron Ring","Common",["ATK"],"a plain iron band ring, no gem"),
 ("Small Bronze Bell","Common",["Tốc đánh"],"a small bronze handbell"),
 ("Worn Leather Book","Common",["ST chí mạng"],"a worn leather-bound book, pages frayed"),
 ("Leather Cap","Common",["Giáp"],"a simple brown leather cap"),
 ("Bone Necklace","Common",["Máu"],"a necklace of small bone beads on a cord"),

 ("Silver Goblet","Rare",["Máu"],"a polished silver goblet with an engraved rim"),
 ("Runic Staff","Rare",["ATK"],"a dark wooden staff carved with glowing blue runes"),
 ("Wolf-Fur Cloak","Rare",["Giáp"],"a grey wolf-fur cloak with a silver clasp"),
 ("Silver Seal","Rare",["ST chí mạng"],"a silver seal stamp with a scrollwork handle"),
 ("Raven Eye Charm","Rare",["Chí mạng"],"an amulet shaped like a raven's eye, black feather attached"),
 ("Knight Badge","Rare",["Tốc đánh"],"a silver winged knight's badge"),
 ("Hourglass","Rare",["Tốc đánh"],"a small hourglass in a silver frame, blue sand inside"),
 ("Ruby Ring","Rare",["ATK"],"a silver ring set with a single red ruby"),

 ("Golden Chalice","Epic",["Máu","Hút máu"],"an ornate golden chalice brimming with glowing red liquid"),
 ("Crystal Staff","Epic",["ATK","ST chí mạng"],"a gold staff topped with a large purple crystal, faint glow"),
 ("Shadow Cloak","Epic",["Giáp","Tốc đánh"],"a black cloak dissolving into purple smoke at the hem"),
 ("Dragon Seal","Epic",["ATK","Chí mạng"],"a gold seal stamp with a coiled dragon handle"),
 ("Forbidden Grimoire","Epic",["ST chí mạng","Chí mạng"],"a chained grimoire with a purple eye on the cover, glowing"),
 ("Genie Lamp","Epic",["Máu","Giáp"],"a golden genie lamp with blue smoke curling from the spout"),

 ("Immortal Crown","Legendary",["Máu","Hút máu"],"a radiant golden crown set with red gems, bright light rays"),
 ("Chaos Staff","Legendary",["ATK","ST chí mạng"],"a twisted black-and-gold staff with a swirling void orb, radiant"),
 ("Divine Seal","Legendary",["Chí mạng","Tốc đánh"],"a divine gold seal stamp with a winged handle, blazing light"),
 ("Golden Armor","Legendary",["Giáp","Máu"],"an ornate golden breastplate with a sunburst emblem, radiant"),
]
def val(s,t): return (XACSUAT if s in PROB else NHAN)[t]
