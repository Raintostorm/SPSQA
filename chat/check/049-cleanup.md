# D1 — chỉ báo cáo, không dời/xóa

2026-11-05. Số chỗ mã là số dòng ứng viên (bao gồm helper/dynamic-prefix/comment), KHÔNG phải số runtime uses. Mã>0 cần Claude đọc callsite; không tự kết luận asset chết. Serialized GUID quét toàn Assets .unity/.prefab/.asset. Cả hai bằng0 chỉ là ứng viên, còn tools/ và Resources load ghép tên cần kiểm riêng trước dời.

| Đường dẫn | Số chỗ mã (bảo thủ) | Số file chứa GUID |
|---|---:|---:|
| `Assets/_Project/Resources/ButtonFamilies/atlas.png` | 1 | 0 |
| `Assets/_Project/Resources/Buttons/blue-pressed.png` | 0 | 0 |
| `Assets/_Project/Resources/Buttons/blue.png` | 0 | 0 |
| `Assets/_Project/Resources/Buttons/disabled.png` | 0 | 0 |
| `Assets/_Project/Resources/Buttons/gold-pressed.png` | 0 | 0 |
| `Assets/_Project/Resources/Buttons/gold.png` | 0 | 0 |
| `Assets/_Project/Resources/Buttons/red-pressed.png` | 0 | 0 |
| `Assets/_Project/Resources/Buttons/red.png` | 0 | 0 |
| `Assets/_Project/Resources/Buttons/slate-pressed.png` | 0 | 0 |
| `Assets/_Project/Resources/Buttons/slate.png` | 0 | 0 |
| `Assets/_Project/Resources/SharedUI/btn_blue.png` | 5 | 0 |
| `Assets/_Project/Resources/SharedUI/btn_primary.png` | 5 | 0 |
| `Assets/_Project/Resources/SharedUI/buff_speed.png` | 4 | 0 |
| `Assets/_Project/Resources/SharedUI/gold.png` | 9 | 0 |
| `Assets/_Project/Resources/SharedUI/key.png` | 6 | 0 |
| `Assets/_Project/Resources/SummonKit/card-common.png` | 0 | 0 |
| `Assets/_Project/Resources/SummonKit/card-epic.png` | 0 | 0 |
| `Assets/_Project/Resources/SummonKit/card-legendary.png` | 0 | 0 |
| `Assets/_Project/Resources/SummonKit/card-rare.png` | 0 | 0 |
| `Assets/_Project/Resources/SummonKit/card-stone.png` | 0 | 0 |
| `Assets/_Project/Resources/SummonKit/price-pill.png` | 3 | 0 |
| `Assets/_Project/Resources/SummonKit/summon-x1.png` | 0 | 0 |
| `Assets/_Project/Resources/SummonKit/summon-x10.png` | 0 | 0 |
| `Assets/_Project/Resources/UiRefresh/atk.bytes` | 5 | 0 |
| `Assets/_Project/Resources/UiRefresh/back.bytes` | 5 | 0 |
| `Assets/_Project/Resources/UiRefresh/battle.bytes` | 6 | 0 |
| `Assets/_Project/Resources/UiRefresh/close.bytes` | 7 | 0 |
| `Assets/_Project/Resources/UiRefresh/coin.bytes` | 8 | 0 |
| `Assets/_Project/Resources/UiRefresh/cyan.bytes` | 3 | 0 |
| `Assets/_Project/Resources/UiRefresh/defense.bytes` | 4 | 0 |
| `Assets/_Project/Resources/UiRefresh/dice.bytes` | 2 | 0 |
| `Assets/_Project/Resources/UiRefresh/disabled.bytes` | 3 | 0 |
| `Assets/_Project/Resources/UiRefresh/gold.bytes` | 7 | 0 |
| `Assets/_Project/Resources/UiRefresh/hp.bytes` | 6 | 0 |
| `Assets/_Project/Resources/UiRefresh/info.bytes` | 3 | 0 |
| `Assets/_Project/Resources/UiRefresh/inventory.bytes` | 3 | 0 |
| `Assets/_Project/Resources/UiRefresh/key.bytes` | 4 | 0 |
| `Assets/_Project/Resources/UiRefresh/menu.bytes` | 2 | 0 |
| `Assets/_Project/Resources/UiRefresh/mp.bytes` | 3 | 0 |
| `Assets/_Project/Resources/UiRefresh/navy.bytes` | 3 | 0 |
| `Assets/_Project/Resources/UiRefresh/next.bytes` | 4 | 0 |
| `Assets/_Project/Resources/UiRefresh/panel.bytes` | 2 | 0 |
| `Assets/_Project/Resources/UiRefresh/pressed.bytes` | 3 | 0 |
| `Assets/_Project/Resources/UiRefresh/previous.bytes` | 2 | 0 |
| `Assets/_Project/Resources/UiRefresh/range.bytes` | 4 | 0 |
| `Assets/_Project/Resources/UiRefresh/recipe.bytes` | 4 | 0 |
| `Assets/_Project/Resources/UiRefresh/speed.bytes` | 5 | 0 |

## Bằng chứng file:dòng

### Assets/_Project/Resources/ButtonFamilies/atlas.png
- `Assets/_Project/Scripts/UI/ButtonFamilySkin.cs:15: var t=Resources.Load<Texture2D>("ButtonFamilies/atlas");if(!t)return null;`
- 0 serialized GUID hits.

### Assets/_Project/Resources/Buttons/blue-pressed.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/Buttons/blue.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/Buttons/disabled.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/Buttons/gold-pressed.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/Buttons/gold.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/Buttons/red-pressed.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/Buttons/red.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/Buttons/slate-pressed.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/Buttons/slate.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/SharedUI/btn_blue.png
- `Assets/_Project/Scripts/UI/BattleModalView.cs:75: else {var art=Rect(row,"Icon",new Vector2(-325,0),new Vector2(85,85)).gameObject.AddComponent<Image>();art.sprite=Resources.Load<Sprite>("SharedUI/"+icon);art.preserveAspect=true;art.raycastTarget=false;}`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:58: var sprite=Resources.Load<Sprite>(name.Contains("/")?name:"SharedUI/"+name);`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:54: buttonSprite = Resources.Load<Sprite>("SharedUI/btn_blue");`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:153: icon == "close" ? "Glyphs/close" : "SharedUI/" + icon;`
- `Assets/_Project/Scripts/Editor/TreasureSceneSetup.cs:59: var btn = S("btn_blue");`
- 0 serialized GUID hits.

### Assets/_Project/Resources/SharedUI/btn_primary.png
- `Assets/_Project/Scripts/UI/BattleModalView.cs:75: else {var art=Rect(row,"Icon",new Vector2(-325,0),new Vector2(85,85)).gameObject.AddComponent<Image>();art.sprite=Resources.Load<Sprite>("SharedUI/"+icon);art.preserveAspect=true;art.raycastTarget=false;}`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:58: var sprite=Resources.Load<Sprite>(name.Contains("/")?name:"SharedUI/"+name);`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:153: icon == "close" ? "Glyphs/close" : "SharedUI/" + icon;`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:159: if (label == "Resume") image.sprite = Resources.Load<Sprite>("SharedUI/btn_primary");`
- `Assets/_Project/Scripts/Editor/UpgradeSceneSetup.cs:81: var btn = S("btn_primary");`
- 0 serialized GUID hits.

### Assets/_Project/Resources/SharedUI/buff_speed.png
- `Assets/_Project/Scripts/UI/BattleModalView.cs:75: else {var art=Rect(row,"Icon",new Vector2(-325,0),new Vector2(85,85)).gameObject.AddComponent<Image>();art.sprite=Resources.Load<Sprite>("SharedUI/"+icon);art.preserveAspect=true;art.raycastTarget=false;}`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:58: var sprite=Resources.Load<Sprite>(name.Contains("/")?name:"SharedUI/"+name);`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:153: icon == "close" ? "Glyphs/close" : "SharedUI/" + icon;`
- `Assets/_Project/Scripts/Visual/MonsterBuffVisual.cs:37: MakeBadge("AttackSpeed",Resources.Load<Sprite>("SharedUI/buff_speed"),.22f);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/SharedUI/gold.png
- `Assets/_Project/Scripts/UI/BattleModalView.cs:75: else {var art=Rect(row,"Icon",new Vector2(-325,0),new Vector2(85,85)).gameObject.AddComponent<Image>();art.sprite=Resources.Load<Sprite>("SharedUI/"+icon);art.preserveAspect=true;art.raycastTarget=false;}`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:13: public static Sprite Gold => ApprovedUiArt.Get("gold",true) ?? ButtonFamilySkin.Get(2) ?? gold ?? (gold = Make(new Color32(255,219,74,255),new Color32(255,200,31,255)));`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:19: public static Sprite Orange => ApprovedUiArt.Get("gold",true) ?? ButtonFamilySkin.Get(0) ?? orange ?? (orange = Make(new Color32(255,205,38,255), new Color32(255,144,0,255)));`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:58: var sprite=Resources.Load<Sprite>(name.Contains("/")?name:"SharedUI/"+name);`
- `Assets/_Project/Scripts/UI/ScreenChrome.cs:85: _ => "gold",`
- `Assets/_Project/Scripts/UI/ResultPanel.cs:126: Reward(content,"Gold",gold,-340,"gold");Reward(content,"Keys",keys,0,"key");Reward(content,"Gems",gems,340,"gem");`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:48: Apply(button.GetComponent<Image>(), "gold", true);`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:50: button.spriteState = new SpriteState { highlightedSprite = Get("gold", true), selectedSprite = Get("gold", true), pressedSprite = Get("pressed", true), disabledSprite = Get("disabled", true) };`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:153: icon == "close" ? "Glyphs/close" : "SharedUI/" + icon;`
- 0 serialized GUID hits.

### Assets/_Project/Resources/SharedUI/key.png
- `Assets/_Project/Scripts/UI/BattleModalView.cs:75: else {var art=Rect(row,"Icon",new Vector2(-325,0),new Vector2(85,85)).gameObject.AddComponent<Image>();art.sprite=Resources.Load<Sprite>("SharedUI/"+icon);art.preserveAspect=true;art.raycastTarget=false;}`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:58: var sprite=Resources.Load<Sprite>(name.Contains("/")?name:"SharedUI/"+name);`
- `Assets/_Project/Scripts/UI/ResultPanel.cs:126: Reward(content,"Gold",gold,-340,"gold");Reward(content,"Keys",keys,0,"key");Reward(content,"Gems",gems,340,"gem");`
- `Assets/_Project/Scripts/UI/ResultPanel.cs:152: art.sprite=Resources.Load<Sprite>(name=="Gems"?"Summon/gem":"HomeChrome/"+(name=="Gold"?"coin":"key"));art.preserveAspect=true;art.raycastTarget=false;art.enabled=art.sprite;`
- `Assets/_Project/Scripts/UI/HomeChrome.cs:41: self.Wallet(keys,"key",new Vector2(-380,-160),new Vector2(250,95));`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:153: icon == "close" ? "Glyphs/close" : "SharedUI/" + icon;`
- 0 serialized GUID hits.

### Assets/_Project/Resources/SummonKit/card-common.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/SummonKit/card-epic.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/SummonKit/card-legendary.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/SummonKit/card-rare.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/SummonKit/card-stone.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/SummonKit/price-pill.png
- `Assets/_Project/Scripts/UI/SummonResultsView.cs:56: var badge=Box(card.transform,"Count",82,6,124,54,Color.white);badge.sprite=Resources.Load<Sprite>("SummonKit/price-pill");`
- `Assets/_Project/Scripts/UI/DailyShopScreen.cs:93: var badge=Box(card,"Count",72,204,150,54,Color.white);badge.raycastTarget=false;badge.sprite=Resources.Load<Sprite>("SummonKit/price-pill");`
- `Assets/_Project/Scripts/UI/GemEconomyScreen.cs:99: pillImage.sprite=Resources.Load<Sprite>("SummonKit/price-pill");pillImage.enabled=pillImage.sprite;`
- 0 serialized GUID hits.

### Assets/_Project/Resources/SummonKit/summon-x1.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/SummonKit/summon-x10.png
- Không tìm thấy chỗ mã theo phép dò bảo thủ.
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/atk.bytes
- `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:193: string statIcon=entry.Stat switch{TreasureStat.Atk=>"atk",TreasureStat.Hp=>"hp",TreasureStat.AtkSpeed=>"speed",TreasureStat.CritDamage=>"crit_damage",TreasureStat.CritChance=>"crit_chance",TreasureStat.LifeSteal=>"lifesteal",_=>"defense"};`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/ChapterMonsterViews.cs:114: string[] names={"HP","ATK","ATK SPD","RANGE"};string[] icons={"hp","atk","speed","range"};`
- `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/back.bytes
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:22: public static Sprite Back => ApprovedUiArt.Get("back") ?? ButtonFamilySkin.Get(6) ?? Blue;`
- `Assets/_Project/Scripts/UI/ScreenChrome.cs:263: /// <summary>Nút back tròn đã duyệt (Resources/UiRefresh/back), đặt ở góc trên trái.</summary>`
- `Assets/_Project/Scripts/UI/ScreenChrome.cs:265: => Round(parent, "back", "blue", position, BackSize, onClick);`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:105: var prev=ScreenChrome.Round(stage,"back","blue",Vector2.zero,120,()=>Select((selected+units.Length-1)%units.Length));`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/battle.bytes
- `Assets/_Project/Scripts/UI/BattleHud.cs:123: var swords=new GameObject("Start swords",typeof(RectTransform),typeof(Image)).GetComponent<Image>();swords.transform.SetParent(startButton.transform,false);swords.sprite=ApprovedUiArt.Get("battle");swords.preserveAspect=true;swords.raycastTarget=false;BattleUiSkin.Place(swords.rectTransform,Vector2.one*.5f,Vector2.zero,new Vector2(150,140));`
- `Assets/_Project/Scripts/UI/MythicFusionPanel.cs:155: var icon=Rect(tab.transform,"Tab icon",new Vector2(-155,0),new Vector2(60,60)).gameObject.AddComponent<Image>();icon.sprite=ApprovedUiArt.Get(tab==tabAllies?"recipe":"battle");icon.preserveAspect=true;icon.raycastTarget=false;`
- `Assets/_Project/Scripts/UI/HomeScreen.cs:77: BattleUiSkin.Icon(playButton,"battle");`
- `Assets/_Project/Scripts/UI/HomeScreen.cs:79: ScreenChrome.Action(playButton,ScreenChrome.Role.Primary,"battle");`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:199: if(Battle){var stats=Button(menu,"Contribution",new Vector2(-220,-165),BuildContribution);stats.sizeDelta=new Vector2(400,90);BattleUiSkin.Button(stats.GetComponent<Button>(),BattleUiSkin.Purple,"Contribution");ScreenChrome.Action(stats.GetComponent<Button>(),ScreenChrome.Role.Secondary);BattleUiSkin.Icon(stats.GetComponent<Button>(),"battle");}`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/close.bytes
- `Assets/_Project/Scripts/UI/BattleChrome.cs:39: var image=new GameObject("Action pictogram",typeof(RectTransform),typeof(Image)).GetComponent<Image>();image.transform.SetParent(parent,false);image.sprite=ApprovedUiArt.Get(index==1?"dice":index==4?"close":index==2?"inventory":"recipe");image.preserveAspect=true;image.raycastTarget=false;BattleUiSkin.Place(image.rectTransform,Vector2.one*.5f,pos,size);return image;`
- `Assets/_Project/Scripts/UI/MythicFusionPanel.cs:159: var close=Button(root,"",new Vector2(440,headerY),new Vector2(125,125),()=>Destroy(modal));ApprovedUiArt.Apply(close.GetComponent<Image>(),"close");`
- `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:166: ApprovedUiArt.Apply(close.GetComponent<Image>(),"close");`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:144: label == "Home" || label == "Leave" ? "home" : label == "Close" || label == "Cancel" ? "close" : null;`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:153: icon == "close" ? "Glyphs/close" : "SharedUI/" + icon;`
- `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:58: var close=Rect(panel,"Close",new Vector2(440,1010),new Vector2(125,125));ApprovedUiArt.Apply(close.gameObject.AddComponent<Image>(),"close");close.gameObject.AddComponent<Button>().onClick.AddListener(Close);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/coin.bytes
- `Assets/_Project/Scripts/UI/BattleHud.cs:127: var coin=BattleChrome.Draw(rollButton.transform,1,new Vector2(75,0),new Vector2(65,65));coin.sprite=ApprovedUiArt.Get("coin");`
- `Assets/_Project/Scripts/UI/RollRewardArt.cs:68: var fx=Create(parent,"coin");fx.space=parent;fx.target=destination;`
- `Assets/_Project/Scripts/UI/RollRewardArt.cs:91: fx.art.rectTransform.sizeDelta=Vector2.one*(sheet=="coin"?70:95);`
- `Assets/_Project/Scripts/UI/ResultPanel.cs:152: art.sprite=Resources.Load<Sprite>(name=="Gems"?"Summon/gem":"HomeChrome/"+(name=="Gold"?"coin":"key"));art.preserveAspect=true;art.raycastTarget=false;art.enabled=art.sprite;`
- `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:82: walletGold=T(reviewRoot,"",-155,25,260,85,48);Icon(reviewRoot,ApprovedUiArt.Get("coin"),-300,35,65);`
- `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:208: Icon(buy.transform,ApprovedUiArt.Get("coin"),-215,24,80);T(buy.transform,price.ToString("N0"),65,0,550,150,64);`
- `Assets/_Project/Scripts/UI/HomeChrome.cs:40: self.Wallet(gold,"coin",new Vector2(-710,-160),new Vector2(290,95));`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/cyan.bytes
- `Assets/_Project/Scripts/UI/BattleChrome.cs:22: ApprovedUiArt.Apply(button.GetComponent<Image>(),"cyan",true);`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:18: public static Sprite Blue => ApprovedUiArt.Get("cyan",true) ?? blue ?? (blue = Make(new Color32(43,174,244,255), new Color32(0,119,226,255)));`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/defense.bytes
- `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:193: string statIcon=entry.Stat switch{TreasureStat.Atk=>"atk",TreasureStat.Hp=>"hp",TreasureStat.AtkSpeed=>"speed",TreasureStat.CritDamage=>"crit_damage",TreasureStat.CritChance=>"crit_chance",TreasureStat.LifeSteal=>"lifesteal",_=>"defense"};`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/dice.bytes
- `Assets/_Project/Scripts/UI/BattleChrome.cs:39: var image=new GameObject("Action pictogram",typeof(RectTransform),typeof(Image)).GetComponent<Image>();image.transform.SetParent(parent,false);image.sprite=ApprovedUiArt.Get(index==1?"dice":index==4?"close":index==2?"inventory":"recipe");image.preserveAspect=true;image.raycastTarget=false;BattleUiSkin.Place(image.rectTransform,Vector2.one*.5f,pos,size);return image;`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/disabled.bytes
- `Assets/_Project/Scripts/UI/ScreenChrome.cs:100: // Sprite "disabled" chỉ thay mặt nút; chữ và icon bên trong vẫn sáng nguyên nên`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:50: button.spriteState = new SpriteState { highlightedSprite = Get("gold", true), selectedSprite = Get("gold", true), pressedSprite = Get("pressed", true), disabledSprite = Get("disabled", true) };`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/gold.bytes
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:13: public static Sprite Gold => ApprovedUiArt.Get("gold",true) ?? ButtonFamilySkin.Get(2) ?? gold ?? (gold = Make(new Color32(255,219,74,255),new Color32(255,200,31,255)));`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:19: public static Sprite Orange => ApprovedUiArt.Get("gold",true) ?? ButtonFamilySkin.Get(0) ?? orange ?? (orange = Make(new Color32(255,205,38,255), new Color32(255,144,0,255)));`
- `Assets/_Project/Scripts/UI/ScreenChrome.cs:85: _ => "gold",`
- `Assets/_Project/Scripts/UI/ResultPanel.cs:126: Reward(content,"Gold",gold,-340,"gold");Reward(content,"Keys",keys,0,"key");Reward(content,"Gems",gems,340,"gem");`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:48: Apply(button.GetComponent<Image>(), "gold", true);`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:50: button.spriteState = new SpriteState { highlightedSprite = Get("gold", true), selectedSprite = Get("gold", true), pressedSprite = Get("pressed", true), disabledSprite = Get("disabled", true) };`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/hp.bytes
- `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:193: string statIcon=entry.Stat switch{TreasureStat.Atk=>"atk",TreasureStat.Hp=>"hp",TreasureStat.AtkSpeed=>"speed",TreasureStat.CritDamage=>"crit_damage",TreasureStat.CritChance=>"crit_chance",TreasureStat.LifeSteal=>"lifesteal",_=>"defense"};`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/ChapterMonsterViews.cs:114: string[] names={"HP","ATK","ATK SPD","RANGE"};string[] icons={"hp","atk","speed","range"};`
- `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- `Assets/_Project/Scripts/Debug/BattleProbe.cs:54: sb.AppendLine($"{"t",6} {"ten",-10} {"phe",-5} {"x",7} {"hp",7} {"song",4} {"dan",4}");`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/info.bytes
- `Assets/_Project/Scripts/UI/CellPanel.cs:77: ApprovedUiArt.Apply(go.GetComponent<Image>(),"info");`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:23: public static Sprite Info => ApprovedUiArt.Get("info") ?? ButtonFamilySkin.Get(7) ?? Navy;`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/inventory.bytes
- `Assets/_Project/Scripts/UI/BattleChrome.cs:21: if(family==0 || family==2){ApprovedUiArt.Action(button,"navy",family==0?"recipe":"inventory");return;}`
- `Assets/_Project/Scripts/UI/BattleChrome.cs:39: var image=new GameObject("Action pictogram",typeof(RectTransform),typeof(Image)).GetComponent<Image>();image.transform.SetParent(parent,false);image.sprite=ApprovedUiArt.Get(index==1?"dice":index==4?"close":index==2?"inventory":"recipe");image.preserveAspect=true;image.raycastTarget=false;BattleUiSkin.Place(image.rectTransform,Vector2.one*.5f,pos,size);return image;`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/key.bytes
- `Assets/_Project/Scripts/UI/ResultPanel.cs:126: Reward(content,"Gold",gold,-340,"gold");Reward(content,"Keys",keys,0,"key");Reward(content,"Gems",gems,340,"gem");`
- `Assets/_Project/Scripts/UI/ResultPanel.cs:152: art.sprite=Resources.Load<Sprite>(name=="Gems"?"Summon/gem":"HomeChrome/"+(name=="Gold"?"coin":"key"));art.preserveAspect=true;art.raycastTarget=false;art.enabled=art.sprite;`
- `Assets/_Project/Scripts/UI/HomeChrome.cs:41: self.Wallet(keys,"key",new Vector2(-380,-160),new Vector2(250,95));`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/menu.bytes
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/SharedSettings.cs:97: else { BattleUiSkin.Skin(openImage,BattleUiSkin.Dark); ApprovedUiArt.Apply(openImage,"menu"); }`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/mp.bytes
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/navy.bytes
- `Assets/_Project/Scripts/UI/BattleChrome.cs:21: if(family==0 || family==2){ApprovedUiArt.Action(button,"navy",family==0?"recipe":"inventory");return;}`
- `Assets/_Project/Scripts/UI/BattleUiSkin.cs:17: public static Sprite Navy => ApprovedUiArt.Get("navy",true) ?? navy ?? (navy = Make(new Color32(15,39,57,255), new Color32(10,27,40,255)));`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/next.bytes
- `Assets/_Project/Scripts/UI/ResultPanel.cs:135: Action(content,hasNext?"NEXT LEVEL":"RETRY",hasNext?"next":"replay",1860,false,()=>{Time.timeScale=1;SceneRoutes.PlayLevel(hasNext?level+1:level);});`
- `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:216: ApprovedUiArt.Apply(previous.GetComponent<Image>(),"previous");ApprovedUiArt.Apply(nextButton.GetComponent<Image>(),"next");`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:109: var nxt=ScreenChrome.Round(stage,"next","blue",Vector2.zero,120,()=>Select((selected+1)%units.Length));`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/panel.bytes
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/Editor/BattleSceneSetup.cs:483: so.FindProperty("panel").objectReferenceValue = panel;`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/pressed.bytes
- `Assets/_Project/Scripts/UI/ScreenChrome.cs:98: // Art của bản "pressed" chỉ đậm hơn một chút, nên hạ nội dung xuống cho thấy rõ là đang bấm.`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:50: button.spriteState = new SpriteState { highlightedSprite = Get("gold", true), selectedSprite = Get("gold", true), pressedSprite = Get("pressed", true), disabledSprite = Get("disabled", true) };`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/previous.bytes
- `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:216: ApprovedUiArt.Apply(previous.GetComponent<Image>(),"previous");ApprovedUiArt.Apply(nextButton.GetComponent<Image>(),"next");`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/range.bytes
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/ChapterMonsterViews.cs:114: string[] names={"HP","ATK","ATK SPD","RANGE"};string[] icons={"hp","atk","speed","range"};`
- `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/recipe.bytes
- `Assets/_Project/Scripts/UI/BattleChrome.cs:21: if(family==0 || family==2){ApprovedUiArt.Action(button,"navy",family==0?"recipe":"inventory");return;}`
- `Assets/_Project/Scripts/UI/BattleChrome.cs:39: var image=new GameObject("Action pictogram",typeof(RectTransform),typeof(Image)).GetComponent<Image>();image.transform.SetParent(parent,false);image.sprite=ApprovedUiArt.Get(index==1?"dice":index==4?"close":index==2?"inventory":"recipe");image.preserveAspect=true;image.raycastTarget=false;BattleUiSkin.Place(image.rectTransform,Vector2.one*.5f,pos,size);return image;`
- `Assets/_Project/Scripts/UI/MythicFusionPanel.cs:155: var icon=Rect(tab.transform,"Tab icon",new Vector2(-155,0),new Vector2(60,60)).gameObject.AddComponent<Image>();icon.sprite=ApprovedUiArt.Get(tab==tabAllies?"recipe":"battle");icon.preserveAspect=true;icon.raycastTarget=false;`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- 0 serialized GUID hits.

### Assets/_Project/Resources/UiRefresh/speed.bytes
- `Assets/_Project/Scripts/UI/TreasureReviewUi.cs:193: string statIcon=entry.Stat switch{TreasureStat.Atk=>"atk",TreasureStat.Hp=>"hp",TreasureStat.AtkSpeed=>"speed",TreasureStat.CritDamage=>"crit_damage",TreasureStat.CritChance=>"crit_chance",TreasureStat.LifeSteal=>"lifesteal",_=>"defense"};`
- `Assets/_Project/Scripts/UI/ApprovedUiArt.cs:20: var data = Resources.Load<TextAsset>("UiRefresh/" + name);`
- `Assets/_Project/Scripts/UI/ChapterMonsterViews.cs:114: string[] names={"HP","ATK","ATK SPD","RANGE"};string[] icons={"hp","atk","speed","range"};`
- `Assets/_Project/Scripts/UI/AllyUpgradeView.cs:125: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- `Assets/_Project/Scripts/UI/BattleUnitInfo.cs:77: string[] icons={"hp","mp","atk","speed","crit_chance","crit_damage","defense","range"};`
- 0 serialized GUID hits.
