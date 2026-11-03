"""Mechanical translation of player-facing text; preserve identifiers and save keys."""
from pathlib import Path
import re,json,ast
root=Path(__file__).resolve().parents[1]
translations={
'Vào trận':'Battle','Gộp':'Merge','đánh xa':'Ranged','cận chiến':'Melee',
'MÁU':'HP','SÁT THƯƠNG':'ATTACK','TẦM':'RANGE','NHỊP':'INTERVAL',
'Máu':'HP','Sát thương':'Attack','Tốc đánh':'Attack Speed','Chí mạng':'Crit Chance',
'ST chí mạng':'Crit Damage','Hút máu':'Lifesteal','Giáp':'Armor',
'TỐC ĐÁNH':'ATTACK SPEED','CHÍ MẠNG':'CRIT CHANCE',
'Cấp ':'Level ','cấp ':'level ','TỐI ĐA':'MAX','CHƯA CÓ':'NOT OWNED','MỚI!':'NEW!',
'gốc            ':'Base           ','cấp nâng   ':'Upgrade    ','kho báu     ':'Relics      ','TỔNG':'TOTAL',
'Mảnh:':'Shards:','Quay rương để nhận.':'Open chests to obtain.',' điểm':' points',
'Thiếu chìa — cần ':'Not enough keys — need ', ', có {GameProgress.Keys}':', have {GameProgress.Keys}',
'Không ra gì.':'No items received.', 'LÒ RÈN':'UPGRADE','NÂNG':'UPGRADE','ĐÓNG':'CLOSE',
'XEM ĐỊCH':'ENEMIES','XUẤT QUÂN':'BATTLE','CHƠI LẠI':'RETRY','TRANG CHỦ':'HOME','BÁN':'SELL','TIẾN HOÁ':'MERGE',
}
files=list((root/'Assets/_Project/Scripts/UI').glob('*.cs'))+[root/'Assets/_Project/Scripts/Gacha/RollResolver.cs']
files += [root/f'Assets/_Project/Scripts/Editor/{name}.cs' for name in ['BattleHudBuilder','UpgradeSceneSetup','TreasureSceneSetup']]
def translate(s):
    for old,new in sorted(translations.items(),key=lambda p:-len(p[0])):s=s.replace(old,new)
    return s
for path in files:
    source=path.read_text()
    # String literals only, not comments or implementation identifiers.
    updated=re.sub(r'"(?:\\.|[^"\\])*"',lambda m:translate(m[0]),source)
    if updated!=source:path.write_text(updated)
# Names only: never translate persistent IDs or stat enum input strings.
names=dict(zip([
'Cốc gỗ sứt','Ly thiếc móp','Gậy gỗ sồi','Áo choàng vải thô','Ấn đất nung','Bùa cỏ bện','Huy hiệu đồng','Nhẫn sắt','Chuông đồng nhỏ','Sách da cũ','Mũ da','Dây chuyền xương','Chén bạc','Trượng khắc rune','Áo choàng lông sói','Ấn triện bạc','Bùa mắt quạ','Huy hiệu kỵ sĩ','Đồng hồ cát','Nhẫn hồng ngọc','Chén thánh vàng','Trượng tinh thể','Áo choàng bóng tối','Ấn triện rồng','Sách cấm','Đèn thần','Vương miện bất tử','Trượng hỗn mang','Ấn triện thần','Giáp hoàng kim','Dân quân cầm liềm'],[
'Chipped Wooden Cup','Dented Tin Mug','Oak Staff','Burlap Cloak','Terracotta Seal','Woven Grass Charm','Bronze Badge','Iron Ring','Small Bronze Bell','Worn Leather Book','Leather Cap','Bone Necklace','Silver Goblet','Runic Staff','Wolf-Fur Cloak','Silver Seal','Raven Eye Charm','Knight Badge','Hourglass','Ruby Ring','Golden Chalice','Crystal Staff','Shadow Cloak','Dragon Seal','Forbidden Grimoire','Genie Lamp','Immortal Crown','Chaos Staff','Divine Seal','Golden Armor','Sickle Militia']))
for path in [root/'tools/treasure_items.py',root/'tools/treasure_items.json',root/'tools/units_manifest.json']:
    text=path.read_text()
    for old,new in names.items():text=text.replace(old,new).replace(json.dumps(old)[1:-1],new)
    path.write_text(text)
for path in [root/'Assets/_Project/Data/TreasureDatabase.asset',*list((root/'Assets/_Project/Data/Units').glob('*.asset'))]:
    text=path.read_text()
    def display(m):
        raw=m[2];value=ast.literal_eval(raw) if raw.startswith('"') else raw
        return m[1]+json.dumps(names[value]) if value in names else m[0]
    text=re.sub(r'^(\s+DisplayName: )(.*)$',display,text,flags=re.M)
    path.write_text(text)
for path in (root/'Assets/_Project/Scenes').glob('*.unity'):
    source=path.read_text()
    def field(m):
        raw=m[2]
        try:value=ast.literal_eval(raw) if raw.startswith('"') else raw
        except ValueError:return m[0]
        updated=translate(value)
        return m[1]+json.dumps(updated,ensure_ascii=True) if updated!=value else m[0]
    updated=re.sub(r'^(  m_Text: )(.*)$',field,source,flags=re.M)
    if updated!=source:path.write_text(updated)
