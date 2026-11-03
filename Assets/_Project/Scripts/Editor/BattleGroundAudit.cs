#if UNITY_EDITOR
using System.Reflection;
using Project.Battle;
using Project.Data;
using Project.Visual;
using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    // Disposable runtime clones: no scene, roster or save data is modified.
    public static class BattleGroundAudit
    {
        [MenuItem("SpinSquad/Diagnostics/Audit all unit ground bounds %&g")]
        public static void Run()
        {
            var flow = Object.FindFirstObjectByType<BattleFlow>();
            var spawner = Object.FindFirstObjectByType<UnitSpawner>();
            var background = Object.FindFirstObjectByType<BattleBackground>();
            if (!Application.isPlaying || !flow || !spawner || !background ||
                flow.Current == BattleFlow.Phase.Prep)
            {
                Debug.LogWarning("[GroundAudit] Run in Play Mode during/after battle.");
                return;
            }
            var field = background.GetComponent<SpriteRenderer>().bounds;
            float bottom = field.max.y - field.size.y * .625f + .08f;
            float top = field.max.y - field.size.y * .365f - .08f;
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var start = typeof(BattleGroundConstraint).GetMethod("Start", flags);
            var tick = typeof(BattleGroundConstraint).GetMethod("LateUpdate", flags);
            int units = 0, checks = 0, failures = 0;
            foreach (var guid in AssetDatabase.FindAssets("t:UnitDefinition",
                         new[] { "Assets/_Project/Data/Units" }))
            {
                var def = AssetDatabase.LoadAssetAtPath<UnitDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                var unit = spawner.Spawn(def, UnitTeam.Ally, Vector3.zero);
                if (!unit) { failures++; continue; }
                try
                {
                    var visual = unit.GetComponent<FrameUnitVisual>();
                    var body = visual.BodyRenderer;
                    var constraint = unit.GetComponent<BattleGroundConstraint>();
                    start.Invoke(constraint, null);
                    unit.GetComponent<UnitBrain>().enabled = true;
                    var scale = unit.transform.localScale;
                    int before = failures, frames = 0;
                    var set = def.Animations;
                    foreach (var clip in new[] { set.Idle, set.Walk, set.Attack, set.Dead, set.Skill })
                    {
                        if (clip == null || clip.Frames == null) continue;
                        foreach (var frame in clip.Frames)
                        {
                            if (!frame) { failures++; continue; }
                            frames++;
                            // Both facing directions, including a legacy offscreen spawn.
                            foreach (float facing in new[] { -1f, 1f })
                            foreach (float y in new[] { -500f, 0f, 500f })
                            {
                                unit.transform.localScale = scale;
                                visual.SetFacing(facing);
                                unit.transform.position = new Vector3(facing * 3.45f, y, 0);
                                body.sprite = frame;
                                tick.Invoke(constraint, null);
                                checks++;
                                var bounds = body.bounds;
                                if (bounds.min.y < bottom - .001f || bounds.max.y > top + .001f ||
                                    !body.enabled || bounds.size.y <= 0)
                                    failures++;
                            }
                        }
                    }
                    units++;
                    Debug.Log($"[GroundAudit] {def.UnitId}: {frames} poses, failures={failures-before}");
                }
                finally { Object.DestroyImmediate(unit.gameObject); }
            }
            Debug.Log($"[GroundAudit] RESULT units={units}, checks={checks}, failures={failures}");
        }
    }
}
#endif
