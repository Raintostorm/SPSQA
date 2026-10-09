NHAN    = {"Common":3,   "Rare":6,  "Epic":10, "Legendary":16}
XACSUAT = {"Common":1.5, "Rare":3,  "Epic":10, "Legendary":14}
MAXLV   = {"Common":10,  "Rare":8,  "Epic":6,  "Legendary":5}
PROB = {"Chí mạng","Hút máu","Giáp"}

# (ten VN, bac, [chi so], mo ta hinh bang tieng Anh cho cong cu sinh anh)
ITEMS = [
 ("Cốc gỗ sứt","Common",["ATK"],"a chipped wooden drinking cup, plain and worn"),
 ("Ly thiếc móp","Common",["Máu"],"a dented tin tankard with a simple handle"),
 ("Gậy gỗ sồi","Common",["ATK"],"a plain oak walking staff, bark still on it"),
 ("Áo choàng vải thô","Common",["Giáp"],"a coarse brown burlap cloak with a rope tie"),
 ("Ấn đất nung","Common",["Máu"],"a small terracotta clay seal stamp"),
 ("Bùa cỏ bện","Common",["Tốc đánh"],"a charm braided from dry grass and twine"),
 ("Huy hiệu đồng","Common",["Chí mạng"],"a plain round bronze badge, slightly tarnished"),
 ("Nhẫn sắt","Common",["ATK"],"a plain iron band ring, no gem"),
 ("Chuông đồng nhỏ","Common",["Tốc đánh"],"a small bronze handbell"),
 ("Sách da cũ","Common",["ST chí mạng"],"a worn leather-bound book, pages frayed"),
 ("Mũ da","Common",["Giáp"],"a simple brown leather cap"),
 ("Dây chuyền xương","Common",["Máu"],"a necklace of small bone beads on a cord"),

 ("Chén bạc","Rare",["Máu"],"a polished silver goblet with an engraved rim"),
 ("Trượng khắc rune","Rare",["ATK"],"a dark wooden staff carved with glowing blue runes"),
 ("Áo choàng lông sói","Rare",["Giáp"],"a grey wolf-fur cloak with a silver clasp"),
 ("Ấn triện bạc","Rare",["ST chí mạng"],"a silver seal stamp with a scrollwork handle"),
 ("Bùa mắt quạ","Rare",["Chí mạng"],"an amulet shaped like a raven's eye, black feather attached"),
 ("Huy hiệu kỵ sĩ","Rare",["Tốc đánh"],"a silver winged knight's badge"),
 ("Đồng hồ cát","Rare",["Tốc đánh"],"a small hourglass in a silver frame, blue sand inside"),
 ("Nhẫn hồng ngọc","Rare",["ATK"],"a silver ring set with a single red ruby"),

 ("Chén thánh vàng","Epic",["Máu","Hút máu"],"an ornate golden chalice brimming with glowing red liquid"),
 ("Trượng tinh thể","Epic",["ATK","ST chí mạng"],"a gold staff topped with a large purple crystal, faint glow"),
 ("Áo choàng bóng tối","Epic",["Giáp","Tốc đánh"],"a black cloak dissolving into purple smoke at the hem"),
 ("Ấn triện rồng","Epic",["ATK","Chí mạng"],"a gold seal stamp with a coiled dragon handle"),
 ("Sách cấm","Epic",["ST chí mạng","Chí mạng"],"a chained grimoire with a purple eye on the cover, glowing"),
 ("Đèn thần","Epic",["Máu","Giáp"],"a golden genie lamp with blue smoke curling from the spout"),

 ("Vương miện bất tử","Legendary",["Máu","Hút máu"],"a radiant golden crown set with red gems, bright light rays"),
 ("Trượng hỗn mang","Legendary",["ATK","ST chí mạng"],"a twisted black-and-gold staff with a swirling void orb, radiant"),
 ("Ấn triện thần","Legendary",["Chí mạng","Tốc đánh"],"a divine gold seal stamp with a winged handle, blazing light"),
 ("Giáp hoàng kim","Legendary",["Giáp","Máu"],"an ornate golden breastplate with a sunburst emblem, radiant"),
]
def val(s,t): return (XACSUAT if s in PROB else NHAN)[t]
