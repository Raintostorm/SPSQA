using System;
using Project.Battle;
using UnityEngine;
using UnityEngine.Rendering;

namespace Project.Visual
{
    /// <summary>Generated effect-only sprites. Gameplay remains in LegendaryCombat.</summary>
    public sealed class LegendSkillArt : MonoBehaviour
    {
        Sprite[] frames;
        SpriteRenderer image;
        Unit owner;
        BattleFlow flow;
        Vector3 origin;
        float elapsed, duration, width;
        bool shield;
        public static LegendSkillArt Play(string name, Unit caster, Vector3 position,
            float worldWidth, float seconds, bool isShield = false)
        {
            if(!Project.UI.SharedSettings.EffectsEnabled)return null;
            if (isShield)
                foreach (var old in FindObjectsByType<LegendSkillArt>(FindObjectsSortMode.None))
                    if (old.shield && old.owner == caster) { old.gameObject.SetActive(false); Destroy(old.gameObject); }
            var sprites = Resources.LoadAll<Sprite>("SkillVfx/" + name);
            Array.Sort(sprites, (a,b) => string.CompareOrdinal(a.name,b.name));
            if (sprites.Length != 6) { Debug.LogWarning("[LegendSkillArt] Missing six frames: " + name); return null; }
            var fx = new GameObject("SkillArt " + name).AddComponent<LegendSkillArt>();
            fx.frames = sprites; fx.owner = caster; fx.origin = position;
            fx.width = worldWidth; fx.duration = seconds; fx.shield = isShield;
            fx.image = fx.gameObject.AddComponent<SpriteRenderer>();
            fx.flow = FindFirstObjectByType<BattleFlow>();
            if (fx.flow) fx.flow.PhaseChanged += fx.OnPhase;
            fx.Draw();
            return fx;
        }
        void OnPhase(BattleFlow.Phase phase)
        {
            if (phase != BattleFlow.Phase.Fighting && phase != BattleFlow.Phase.Marching && phase != BattleFlow.Phase.Finished)
            { gameObject.SetActive(false); Destroy(gameObject); }
        }
        void OnDestroy() { if (flow) flow.PhaseChanged -= OnPhase; }
        void Update()
        {
            elapsed += Time.deltaTime;
            if (!owner || !owner.IsAlive || elapsed >= duration ||
                shield && owner.Combat.Shield <= 0f) { Destroy(gameObject); return; }
            Draw();
        }
        void Draw()
        {
            image.enabled=Project.UI.SharedSettings.EffectsEnabled;
            var step = shield ? (elapsed < .3f ? Mathf.Min(3, (int)(elapsed/.075f)) :
                4 + (int)((elapsed-.3f)/.3f)%2) : Mathf.Min(5,(int)(elapsed/duration*6));
            image.sprite = frames[step];
            transform.position = shield ? owner.HitPoint : origin;
            transform.localScale = Vector3.one * width;
            var sorting = owner.GetComponent<SortingGroup>();
            if (sorting) { image.sortingLayerID = sorting.sortingLayerID; image.sortingOrder = sorting.sortingOrder + 2; }
            image.color = new Color(1,1,1,shield ? .65f : 1f);
        }
    }
}
