using System.Collections;
using Project.Data;
using Project.Visual;
using UnityEngine;

namespace Project.Battle
{
    /// <summary>Wave-scoped mana, shields, control and five distinct automatic skills.</summary>
    [RequireComponent(typeof(Unit))]
    public sealed class LegendaryCombat : MonoBehaviour
    {
        public const float ManaPerHit = 20f, MaxMana = 100f;
        public float Mana { get; private set; }
        public float Shield => Time.time < shieldUntil ? shield : 0f;
        public float AttackMultiplier => Time.time < roarUntil ? 1.3f : 1f;
        public float SpeedMultiplier => AttackMultiplier;
        public bool Stunned => Time.time < frozenUntil || Time.time < knockUntil;
        public bool Casting { get; private set; }
        public bool IsLegend => unit && unit.Definition && unit.Definition.Rank == Rank.Legendary;
        public bool Temporary { get; private set; }
        [SerializeField] float areaRadius = 3.5f;
        [SerializeField] float skillDamageMultiplier = 2f;
        Unit unit;
        BattleFlow flow;
        float shield, shieldUntil, roarUntil, frozenUntil, knockUntil;
        SpriteRenderer body;
        Vector3 bodyBase;
        bool lifted;
        Coroutine fallback;

        void Awake() { unit = GetComponent<Unit>(); }
        void Start()
        {
            flow = FindFirstObjectByType<BattleFlow>();
            if (flow) flow.PhaseChanged += PhaseChanged;
            var frame = GetComponent<FrameUnitVisual>();
            if (frame) body = frame.BodyRenderer;
        }
        void OnDestroy() { if (flow) flow.PhaseChanged -= PhaseChanged; }
        void Update()
        {
            if (!unit.IsAlive) { Casting = false; Mana = 0f; }
        }
        void LateUpdate()
        {
            if (body)
            {
                var color = body.color;
                var hue = Time.time < frozenUntil ? new Color(.45f,.85f,1f) : Color.white;
                hue.a = color.a;
                body.color = hue;
                if (Time.time < knockUntil)
                {
                    if (!lifted) { bodyBase = body.transform.localPosition; lifted = true; }
                    var t = 1f - Mathf.Clamp01((knockUntil - Time.time) / .6f);
                    body.transform.localPosition = bodyBase + Vector3.up * Mathf.Sin(t * Mathf.PI) * .45f;
                }
                else if (lifted) { body.transform.localPosition = bodyBase; lifted = false; }
            }
        }
        bool InWave => !flow || flow.Current == BattleFlow.Phase.Marching || flow.Current == BattleFlow.Phase.Fighting;
        public void BasicHit()
        {
            if (IsLegend && unit.IsAlive && InWave) Mana = Mathf.Min(MaxMana, Mana + ManaPerHit);
        }
        public float Absorb(float damage)
        {
            var absorbed = Mathf.Min(Shield, damage);
            shield -= absorbed;
            return damage - absorbed;
        }
        public void GiveShield()
        {
            shield = unit.MaxHp * .5f;
            shieldUntil = Time.time + 8f; // refresh; never stack
            SkillEffects.Shield(unit, 8f);
        }
        public void RoarBuff() { roarUntil = Time.time + 5f; }
        public void Freeze() { frozenUntil = Mathf.Max(frozenUntil, Time.time + 2f); SkillEffects.Ring(unit.transform.position, .7f, Color.cyan, 2f); }
        public void KnockUp() { knockUntil = Time.time + .6f; }
        public void MarkTemporary() { Temporary = true; }
        void PhaseChanged(BattleFlow.Phase phase)
        {
            if (phase == BattleFlow.Phase.Marching || phase == BattleFlow.Phase.Fighting) return;
            Mana = 0f; shield = shieldUntil = roarUntil = frozenUntil = knockUntil = 0f;
            Casting = false;
            if (fallback != null) StopCoroutine(fallback);
            if (Temporary) { gameObject.SetActive(false); Destroy(gameObject); }
        }
        public bool TryCast(Unit target)
        {
            if (!IsLegend || !unit.IsAlive || !InWave || Stunned || Casting || Mana < MaxMana || !target || !target.IsAlive) return false;
            Mana = 0f;
            Casting = true;
            unit.Visual?.SetMoving(false);
            var centre = target.transform.position;
            var frame = GetComponent<FrameUnitVisual>();
            if (frame) frame.PlaySkill(() => ApplySkill(centre), () => Casting = false);
            else fallback = StartCoroutine(FallbackCast(centre));
            return true;
        }
        IEnumerator FallbackCast(Vector3 centre)
        {
            yield return new WaitForSeconds(.35f);
            ApplySkill(centre);
            yield return new WaitForSeconds(.35f);
            Casting = false;
        }
        void ApplySkill(Vector3 centre)
        {
            if (!Casting || !unit.IsAlive || !InWave) return;
            switch (unit.Definition.Race)
            {
                case Race.Elf:
                    SkillEffects.Beam(unit.HitPoint, unit.HitPoint + Vector3.up * 2f, Color.cyan, .3f);
                    fallback = StartCoroutine(ArrowRain(centre));
                    break;
                case Race.Human:
                    SkillEffects.Ring(transform.position, areaRadius, new Color(1f,.7f,.15f), .6f);
                    foreach (var enemy in UnitRegistry.All)
                        if (EnemyInArea(enemy, transform.position)) { enemy.Combat.KnockUp(); enemy.TakeDamage(unit.AttackPower * skillDamageMultiplier, unit, false); }
                    break;
                case Race.Machine:
                    foreach (var ally in UnitRegistry.All)
                        if (ally && ally.IsAlive && ally.Team == unit.Team && ally.Definition && !ally.Definition.IsRanged) ally.Combat.GiveShield();
                    break;
                case Race.Dead: Summon(); break;
                case Race.Monster:
                    SkillEffects.Ring(transform.position, areaRadius * 1.5f, new Color(1f,.3f,.05f), .8f);
                    foreach (var ally in UnitRegistry.All)
                        if (ally && ally.IsAlive && ally.Team == unit.Team) { ally.Combat.RoarBuff(); SkillEffects.Ring(ally.transform.position, .55f, Color.red, .8f); }
                    break;
            }
        }
        bool EnemyInArea(Unit enemy, Vector3 centre) => enemy && enemy.IsAlive && enemy.Team != unit.Team && (enemy.transform.position - centre).sqrMagnitude <= areaRadius * areaRadius;
        IEnumerator ArrowRain(Vector3 centre)
        {
            yield return new WaitForSeconds(.3f);
            if (!unit.IsAlive || !InWave) yield break;
            for (var i = 0; i < 12; i++)
            {
                var offset = Random.insideUnitCircle * areaRadius;
                var end = centre + new Vector3(offset.x, offset.y * .35f, 0f);
                SkillEffects.Beam(end + Vector3.up * 2f, end, Color.cyan, .35f);
            }
            yield return new WaitForSeconds(.35f);
            if (!unit.IsAlive || !InWave) yield break;
            SkillEffects.Ring(centre, areaRadius, Color.cyan, .6f);
            foreach (var enemy in UnitRegistry.All)
                if (EnemyInArea(enemy, centre)) { enemy.Combat.Freeze(); enemy.TakeDamage(unit.AttackPower * skillDamageMultiplier, unit, false); }
        }
        void Summon()
        {
            var spawner = FindFirstObjectByType<UnitSpawner>();
            UnitDefinition common = null;
            foreach (var def in Resources.FindObjectsOfTypeAll<UnitDefinition>())
                if (def.Race == Race.Dead && def.Rank == Rank.Common) { common = def; break; }
            if (!spawner || !common) { Debug.LogWarning("[LegendaryCombat] Missing Dead Common or UnitSpawner.", this); return; }
            var hp = unit.MaxHp / 3f;
            var attack = unit.AttackPower / 3f;
            for (var i = 0; i < 3; i++)
            {
                var pos = transform.position + new Vector3((i - 1) * .75f, -.45f, 0f);
                SkillEffects.Ring(pos, .65f, new Color(.7f,.25f,1f), .7f);
                var summoned = spawner.Spawn(common, unit.Team, pos, transform.parent);
                if (!summoned) continue;
                summoned.SetSummonedStats(hp, attack);
                summoned.Combat.MarkTemporary();
                var brain = summoned.GetComponent<UnitBrain>();
                if (brain) { brain.SummonedCooldown = unit.Definition.AttackCooldown * 3f; brain.SummonedMoveSpeed = unit.Definition.MoveSpeed / 3f; brain.enabled = true; }
            }
        }
    }
}
