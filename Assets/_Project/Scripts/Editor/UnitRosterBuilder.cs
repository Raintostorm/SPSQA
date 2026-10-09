using System.Collections.Generic;
using System.IO;
using Project.Data;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools
{
    /// <summary>
    /// Dựng cả roster từ bản kê do tools/import_units.py sinh ra.
    ///
    /// Ba việc, một lệnh:
    ///   1. sửa prefab lính: bỏ hết node của hệ ghép thân/đầu/vũ khí, thay
    ///      bằng MỘT SpriteRenderer do FrameUnitVisual điều khiển
    ///   2. tạo 20 UnitAnimationSet, mỗi bộ trỏ tới các sprite đã nhập
    ///   3. tạo 20 UnitDefinition, chỉ số suy ra từ bậc
    ///
    /// Chạy lại được: asset đã có thì ghi đè nội dung, GIỮ NGUYÊN GUID nên
    /// mọi tham chiếu đang trỏ tới nó không đứt.
    /// </summary>
    public static class UnitRosterBuilder
    {
        const string Ledger = "tools/units_manifest.json";
        const string ArtDir = "Assets/_Project/Art/Units";
        const string VfxDir = "Assets/_Project/Art/VFX";
        const string OutDir = "Assets/_Project/Data/Units";
        const string Prefab = "Assets/_Project/Prefabs/Units/Unit.prefab";
        const string FlashMat = "Assets/_Project/Art/Shaders/SpriteFlash.mat";

        const string ProjectileDir = "Assets/_Project/Prefabs/Units";

        // ── chỉ số theo bậc ───────────────────────────────────────────
        //
        // Gõ thẳng bốn bậc thay vì nhân luỹ thừa. Một hệ số chung ép mọi
        // bậc cách nhau đúng một tỉ lệ, mà cân bằng thật thì không như vậy —
        // khoảng Rare→Epic cố ý rộng hơn Common→Rare. Bốn cặp số, sửa ở
        // đúng một chỗ, vẫn không phải gõ 20 lần.
        //
        // Common không được chỉ định, nên suy ngược từ Rare theo đúng bước
        // Rare→Epic (3.33×): 600/3.33 ≈ 180, 90/3.33 ≈ 27.
        //
        // Thứ tự PHẢI khớp enum Rank: Common=0, Rare, Epic, Legendary.
        static readonly float[] HpByRank  = { 180f,  600f, 2000f, 7500f };
        static readonly float[] AtkByRank = {  27f,   90f,  300f, 1000f };

        [System.Serializable] class Clip
        {
            public string[] files; public float[] durations; public int[] sequence; public bool loop;
            public string playback;
        }
        [System.Serializable] class Anims
        {
            public Clip idle, walk, attack, dead, skill;
        }
        [System.Serializable] class Vfx
        {
            public Clip walk, attack, dead;
            public Vector2 walk_offset, attack_offset, dead_offset;
            public float foot_spacing = 0.1f;
            public float walk_scale = 1f, attack_scale = 1f, dead_scale = 1f;
            public float walk_alpha = 1f, attack_alpha = 1f, dead_alpha = 1f;
        }
        [System.Serializable] class Row
        {
            public string key, race, rank, name, combat, weapon;
            public float ppu, world_h;
            // 0 nghia la manifest chua co so — chay tools/fit_cell_scale.py.
            public float cell_scale;
            public Vector2 cell_offset;
            public int impact_step = -1;
            public Anims anims;
            public Vfx vfx;
        }

        [MenuItem("SpinSquad/Nhập 20 nhân vật")]
        public static void Run()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), Ledger);
            if (!File.Exists(path))
            {
                Debug.LogError($"[UnitRosterBuilder] Không thấy {Ledger}. " +
                               $"Chạy: python3 tools/import_units.py --write");
                return;
            }

            // JsonUtility khong doc duoc mang o goc, nen boc vao mot object.
            var rows = JsonUtility.FromJson<Wrap>("{\"items\":" + File.ReadAllText(path) + "}").items;
            if (rows == null || rows.Length == 0)
            {
                Debug.LogError("[UnitRosterBuilder] Bản kê rỗng.");
                return;
            }

            Directory.CreateDirectory(OutDir);
            RebuildPrefab();

            var made = 0;
            foreach (var r in rows)
            {
                var set = MakeSet(r);
                if (set == null) continue;
                MakeDef(r, set);
                made++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[UnitRosterBuilder] Xong {made}/{rows.Length} nhân vật vào {OutDir}.");
        }

        [System.Serializable] class Wrap { public Row[] items; }

        // ── prefab ────────────────────────────────────────────────────

        /// <summary>
        /// Bỏ toàn bộ node của hệ ghép, thay bằng một SpriteRenderer duy nhất.
        ///
        /// Sửa prefab chứ không dựng mới: thanh máu đã nối sẵn vào đây, dựng
        /// lại từ đầu là phải nối tay lại và chắc chắn sẽ nối thiếu.
        /// </summary>
        static void RebuildPrefab()
        {
            var root = PrefabUtility.LoadPrefabContents(Prefab);
            if (root == null) { Debug.LogError($"[UnitRosterBuilder] Không mở được {Prefab}"); return; }

            try
            {
                foreach (var n in new[] { "Body", "Head", "Arm", "Weapon", "Hand", "Ammo", "Muzzle" })
                {
                    var t = Find(root.transform, n);
                    if (t != null && t != root.transform) Object.DestroyImmediate(t.gameObject);
                }

                // Animator khong con viec gi: khung hinh do FrameUnitVisual chay.
                var anim = root.GetComponent<Animator>();
                if (anim != null) Object.DestroyImmediate(anim);

                // UnitEquipment va SpriteUnitVisual da bi xoa khoi du an, nen
                // o day chung la "missing script" — phai don, khong thi Unity
                // keu moi lan mo prefab.
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(root);

                var sprite = Find(root.transform, "Sprite");
                if (sprite == null)
                {
                    var go = new GameObject("Sprite");
                    go.transform.SetParent(root.transform, false);
                    sprite = go.transform;
                }

                var sr = sprite.GetComponent<SpriteRenderer>();
                if (sr == null) sr = sprite.gameObject.AddComponent<SpriteRenderer>();
                sr.sortingOrder = 0;
                var mat = AssetDatabase.LoadAssetAtPath<Material>(FlashMat);
                if (mat != null) sr.sharedMaterial = mat;
                else Debug.LogWarning($"[UnitRosterBuilder] Không thấy {FlashMat} — " +
                                      $"lính sẽ không nháy trắng khi trúng đòn.");

                var vis = root.GetComponent<Visual.FrameUnitVisual>();
                if (vis == null) vis = root.AddComponent<Visual.FrameUnitVisual>();
                var so = new SerializedObject(vis);
                so.FindProperty("target").objectReferenceValue = sr;
                so.ApplyModifiedPropertiesWithoutUndo();

                PrefabUtility.SaveAsPrefabAsset(root, Prefab);
                Debug.Log("[UnitRosterBuilder] Prefab lính: 1 SpriteRenderer, không Animator.");
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        static Transform Find(Transform root, string name)
        {
            if (root.name == name) return root;
            foreach (Transform c in root)
            {
                var hit = Find(c, name);
                if (hit != null) return hit;
            }
            return null;
        }

        // ── asset ─────────────────────────────────────────────────────

        static UnitAnimationSet MakeSet(Row r)
        {
            var path = $"{OutDir}/Anim_{r.key}.asset";
            var set = AssetDatabase.LoadAssetAtPath<UnitAnimationSet>(path);
            var isNew = set == null;
            if (isNew) set = ScriptableObject.CreateInstance<UnitAnimationSet>();

            set.Idle = Conv(r.anims.idle, r.key, "idle");
            set.Walk = Conv(r.anims.walk, r.key, "walk");
            set.Attack = Conv(r.anims.attack, r.key, "attack");
            set.Dead = Conv(r.anims.dead, r.key, "dead");
            set.Skill = Conv(r.anims.skill, r.key, "skill");

            set.WalkVfx = r.vfx == null ? null : Conv(r.vfx.walk, r.key, "vfx/walk", VfxDir);
            set.AttackVfx = r.vfx == null ? null : Conv(r.vfx.attack, r.key, "vfx/attack", VfxDir);
            set.DeathVfx = r.vfx == null ? null : Conv(r.vfx.dead, r.key, "vfx/dead", VfxDir);
            set.WalkVfxOffset = r.vfx == null ? Vector2.zero : r.vfx.walk_offset;
            set.WalkVfxFootSpacing = r.vfx == null ? 0.1f : Mathf.Max(0f, r.vfx.foot_spacing);
            set.AttackVfxOffset = r.vfx == null ? Vector2.zero : r.vfx.attack_offset;
            set.DeathVfxOffset = r.vfx == null ? Vector2.zero : r.vfx.dead_offset;
            set.WalkVfxScale = Scale(r.vfx == null ? 1f : r.vfx.walk_scale);
            set.AttackVfxScale = Scale(r.vfx == null ? 1f : r.vfx.attack_scale);
            set.DeathVfxScale = Scale(r.vfx == null ? 1f : r.vfx.dead_scale);
            set.WalkVfxAlpha = Alpha(r.vfx == null ? 1f : r.vfx.walk_alpha);
            set.AttackVfxAlpha = Alpha(r.vfx == null ? 1f : r.vfx.attack_alpha);
            set.DeathVfxAlpha = Alpha(r.vfx == null ? 1f : r.vfx.dead_alpha);

            if (set.Idle == null || set.Attack == null)
            {
                Debug.LogError($"[UnitRosterBuilder] '{r.key}' thiếu clip đứng hoặc đánh — bỏ qua.");
                return null;
            }

            // Doi cham o buoc ap chot: 4 khung thi buoc 2 la luc vu khi toi
            // diem xa nhat. Doc mot so, khong doi animation event.
            set.ImpactStep = Mathf.Clamp(r.impact_step >= 0
                ? r.impact_step : set.Attack.Count / 2, 0, set.Attack.Count - 1);

            if (isNew) AssetDatabase.CreateAsset(set, path);
            else EditorUtility.SetDirty(set);
            return set;
        }

        static UnitAnimationSet.Clip Conv(Clip c, string key, string name, string root = ArtDir)
        {
            if (c == null || c.files == null || c.files.Length == 0) return null;
            var frames = new List<Sprite>();
            foreach (var f in c.files)
            {
                var s = AssetDatabase.LoadAssetAtPath<Sprite>($"{root}/{f}");
                if (s == null)
                {
                    Debug.LogWarning($"[UnitRosterBuilder] {key}/{name}: không thấy {f}");
                    continue;
                }
                frames.Add(s);
            }
            if (frames.Count == 0) return null;
            return new UnitAnimationSet.Clip
            {
                Frames = frames.ToArray(),
                Durations = c.durations,
                Sequence = c.sequence,
                Loop = c.loop,
                UseExplicitPlayback = !string.IsNullOrEmpty(c.playback),
                PingPong = c.playback == "ping_pong",
            };
        }

        static float Scale(float value) => value <= 0f ? 1f : value;
        static float Alpha(float value) => Mathf.Clamp01(value);

        static void MakeDef(Row r, UnitAnimationSet set)
        {
            var path = $"{OutDir}/Unit_{r.key}.asset";
            var def = AssetDatabase.LoadAssetAtPath<UnitDefinition>(path);
            var isNew = def == null;
            if (isNew) def = ScriptableObject.CreateInstance<UnitDefinition>();

            var race = (Race)System.Enum.Parse(typeof(Race), Cap(r.race), true);
            var rank = (Rank)System.Enum.Parse(typeof(Rank), Cap(r.rank), true);
            var ranged = r.combat == "ranged";

            def.UnitId = r.key;
            def.DisplayName = string.IsNullOrEmpty(r.name) ? r.key : r.name;
            def.Race = race;
            def.Rank = rank;
            def.BaseHp = HpByRank[(int)rank];
            def.BaseAttack = AtkByRank[(int)rank];
            def.IsRanged = ranged;
            def.AttackRange = ranged ? 3.2f : 0.9f;
            def.AttackCooldown = ranged ? 0.95f : 0.7f;
            def.MoveSpeed = 2.0f;
            def.Animations = set;
            // Scale RIENG tung con, do tu chinh khung hinh cua no. Luc nhap
            // art duoc chuan hoa theo CHIEU CAO nen be ngang tha noi: than
            // human_legendary rong gap ba lan elf_common. Mot so chung phuc
            // vu duoc con nay thi hong con kia — 18/20 con tran khoi o.
            //
            // tools/fit_cell_scale.py do va ghi cell_scale vao manifest.
            // Thieu so thi lui ve hang cu, chu khong de lính scale 0.
            def.Scale = r.cell_scale > 0f
                ? r.cell_scale
                : BattleSceneSetup.UnitScale;
            def.CellOffset = r.cell_offset;

            if (ranged)
            {
                var projectile = ProjectileFor(r.key);
                def.ProjectilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(projectile.path);
                def.ProjectileSpeed = projectile.speed;
                def.ProjectileArcHeight = projectile.arc;
                if (def.ProjectilePrefab == null)
                    Debug.LogWarning($"[UnitRosterBuilder] '{r.key}' đánh xa mà không " +
                                     $"thấy {projectile.path}. Cú bắn sẽ mất trắng.");
            }
            else def.ProjectilePrefab = null;

            if (isNew) AssetDatabase.CreateAsset(def, path);
            else EditorUtility.SetDirty(def);
        }

        static string Cap(string s) =>
            string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);

        static (string path, float speed, float arc) ProjectileFor(string key)
        {
            switch (key)
            {
                case "dead_rare":      return ($"{ProjectileDir}/Projectile_psychic_orb.prefab", 18f, 0f);
                case "dead_epic":      return ($"{ProjectileDir}/Projectile_ghost_skull.prefab", 17f, 0.02f);
                case "elf_common":     return ($"{ProjectileDir}/Projectile_wood_arrow.prefab", 22f, 0.03f);
                case "elf_epic":       return ($"{ProjectileDir}/Projectile_crescent_bolt.prefab", 24f, 0f);
                case "elf_legendary":  return ($"{ProjectileDir}/Projectile_royal_arrow.prefab", 24f, 0.02f);
                case "human_epic":     return ($"{ProjectileDir}/Projectile_crystal_bolt.prefab", 21f, 0f);
                case "machine_rare":   return ($"{ProjectileDir}/Projectile_brass_bullet.prefab", 32f, 0f);
                case "monster_common": return ($"{ProjectileDir}/Projectile_web_glob.prefab", 16f, 0.04f);
                default:                return ($"{ProjectileDir}/Projectile_modern_1.prefab", 9f, 0f);
            }
        }
    }
}
