#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Project.Battle;
using Project.Data;
using UnityEditor;
using UnityEngine;

namespace Project.EditorTools
{
    public static class LegendaryCombatAudit
    {
        [MenuItem("SpinSquad/Diagnostics/Audit Legendary Combat %&m")]
        static void Run()
        {
            if (!Application.isPlaying || Object.FindFirstObjectByType<BattleFlow>())
            { Debug.LogWarning("[LegendAudit] Run in Home Play Mode, outside a live battle."); return; }
            var root = new GameObject("LegendAudit (temporary)");
            var spawner = root.AddComponent<UnitSpawner>();
            typeof(UnitSpawner).GetField("unitPrefab", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(spawner,
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Units/Unit.prefab"));
            // Make the summon definition available without changing roster assets.
            AssetDatabase.LoadAssetAtPath<UnitDefinition>("Assets/_Project/Data/Units/Unit_dead_common.asset");
            spawner.StartCoroutine(Check(spawner, root));
        }
        static IEnumerator Check(UnitSpawner spawner, GameObject root)
        {
            var lines = new List<string>();
            var definitions = new List<UnitDefinition>();
            Unit Make(string race, Rank rank, UnitTeam team, float hp, float attack)
            {
                var original = AssetDatabase.LoadAssetAtPath<UnitDefinition>($"Assets/_Project/Data/Units/Unit_{race}_{(rank == Rank.Legendary ? "legendary" : "common")}.asset");
                var def = Object.Instantiate(original); definitions.Add(def);
                def.BaseHp = hp; def.BaseAttack = attack;
                var u = spawner.Spawn(def, team, new Vector3(100f, 0f, 0f), root.transform);
                u.SetSummonedStats(hp, attack); u.GetComponent<UnitBrain>().enabled = false;
                return u;
            }
            void Assert(bool ok, string label) { lines.Add((ok ? "PASS " : "FAIL ") + label); }
            var machine = Make("machine", Rank.Legendary, UnitTeam.Ally, 5000f, 100f);
            var human = Make("human", Rank.Common, UnitTeam.Ally, 300f, 30f);
            var enemy = Make("human", Rank.Common, UnitTeam.Enemy, 10000f, 30f);
            yield return null;
            for (var i = 0; i < 5; i++) enemy.TakeDamage(1f, machine);
            Assert(machine.Combat.Mana == 100f, "5 basic hits = 100 mana");
            enemy.TakeDamage(1f, machine, false);
            Assert(machine.Combat.Mana == 100f, "skill damage does not grant mana");
            machine.Combat.GiveShield(); human.Combat.GiveShield();
            Assert(machine.Combat.Shield == 2500f && human.Combat.Shield == 150f, "shield = recipient own HP / 2 (2500, 150)");
            var hpBefore = human.CurrentHp;
            human.TakeDamage(100f, enemy);
            Assert(human.CurrentHp == hpBefore && human.Combat.Shield < 150f, "shield absorbs before HP");
            human.Combat.GiveShield(); human.Combat.GiveShield();
            Assert(human.Combat.Shield == 150f, "shield refresh never stacks");
            human.Combat.RoarBuff(); human.Combat.RoarBuff();
            Assert(Mathf.Approximately(human.Combat.AttackMultiplier, 1.3f) && Mathf.Approximately(human.Combat.SpeedMultiplier, 1.3f), "roar +30% attack/speed, non-stacking");
            foreach (var race in new[] { "elf", "human", "dead", "monster", "machine" })
            {
                var caster = Make(race, Rank.Legendary, UnitTeam.Ally, 900f, 90f);
                for (var i = 0; i < 5; i++) enemy.TakeDamage(1f, caster);
                Assert(caster.Combat.TryCast(enemy) && caster.Combat.Mana == 0f, race + " starts real skill clip and consumes mana");
                var deadline = Time.time + 15f;
                var sawHandMagic = false;
                while (caster.Combat.Casting && Time.time < deadline)
                {
                    var magic = caster.transform.Find("SkillVfx");
                    if (magic && magic.GetComponent<SpriteRenderer>().enabled) sawHandMagic = true;
                    yield return null;
                }
                yield return new WaitForSeconds(.5f);
                Assert(!caster.Combat.Casting, race + " skill clip finishes without lock");
                if (race == "dead")
                {
                    Assert(sawHandMagic, "Dead hand magic plays during casting");
                    var magic = caster.transform.Find("SkillVfx");
                    Assert(magic && !magic.GetComponent<SpriteRenderer>().enabled, "Dead hand magic hidden after casting");
                    var count = 0;
                    foreach (var u in UnitRegistry.All)
                        if (u && u.Combat.Temporary)
                        { count++; Assert(Mathf.Approximately(u.MaxHp, 300f) && Mathf.Approximately(u.AttackPower / u.Combat.AttackMultiplier, 30f), "summon HP/attack = caster / 3"); }
                    Assert(count == 3, "Dead summons exactly 3");
                }
            }
            human.Combat.GiveShield();
            yield return new WaitForSeconds(8.1f);
            Assert(human.Combat.Shield == 0f, "shield expires at 8 game seconds");
            foreach (var u in new List<Unit>(UnitRegistry.All))
                if (u && u.transform.IsChildOf(root.transform))
                    typeof(LegendaryCombat).GetMethod("PhaseChanged", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(u.Combat, new object[] { BattleFlow.Phase.Prep });
            Assert(UnitRegistry.CountAlive(UnitTeam.Ally) == 7, "wave cleanup removes three temporary summons");
            System.IO.File.WriteAllLines(System.IO.Path.Combine(Application.dataPath, "../outdata/LEGEND_COMBAT_AUDIT.txt"), lines);
            Debug.Log("[LegendAudit]\n" + string.Join("\n", lines));
            foreach (var def in definitions) Object.Destroy(def);
            Object.Destroy(root);
        }
    }
}
#endif
