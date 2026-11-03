#if UNITY_EDITOR
using Project.Battle;
using Project.UI;
using Project.Visual;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Project.EditorTools
{
    public static class CleanBattleEditorPreview
    {
        [MenuItem("SpinSquad/Diagnostics/Hide Legacy Battle Editor Preview")]
        static void Clean()
        {
            if(EditorApplication.isPlaying){Debug.LogWarning("Stop Play Mode before cleaning editor-only preview.");return;}
            var scene=EditorSceneManager.GetActiveScene();
            if(scene.name!="CharacterLab"||scene.isDirty){Debug.LogWarning("Open a saved CharacterLab scene first; existing unsaved changes are preserved.");return;}
            int hidden=0;
            foreach(var root in scene.GetRootGameObjects())
            {
                foreach(var hud in root.GetComponentsInChildren<BattleHud>(true))
                {
                    var canvas=hud.GetComponent<Canvas>();if(!canvas)continue;
                    Undo.RecordObject(canvas,"Hide obsolete edit-mode HUD");canvas.enabled=false;EditorUtility.SetDirty(canvas);hidden++;
                }
                foreach(var renderer in root.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    if(!renderer.GetComponent<BattleBackground>()&&!renderer.name.StartsWith("Cell_"))continue;
                    Undo.RecordObject(renderer,"Hide obsolete edit-mode art");renderer.enabled=false;EditorUtility.SetDirty(renderer);hidden++;
                }
            }
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            Debug.Log("[BattleEditorPreview] Hidden "+hidden+" obsolete editor renderers. Runtime Awake restores gameplay presentation; logic/references retained.");
        }
    }
}
#endif
