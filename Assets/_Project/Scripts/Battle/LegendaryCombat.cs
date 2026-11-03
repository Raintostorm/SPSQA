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
        void GainBuffedMana(float amount)
        {
            float before=Mana;Mana=Mathf.Min(MaxMana,Mana+amount*(EffectBuffSystem.Current?EffectBuffSystem.Current.ManaMultiplier(unit):1));
            if(Mana>before&&unit.Team==UnitTeam.Ally)Project.Visual.BuffProcVisual.Play(unit,EffectBuffKind.ManaFlow);
        }
        public const float ManaPerHit = 20f, MaxMana = 100f;
        float mana;
        public float Mana { get => unit && unit.Mythic && unit.Mythic.Active ? unit.Mythic.Mana : mana; private set => mana=value; }
        public float Shield => Time.time < shieldUntil ? shield : 0f;
        public float AttackMultiplier => Time.time < roarUntil ? 1.3f : 1f;
        public float SpeedMultiplier => AttackMultiplier;
        public bool Stunned => Time.time < frozenUntil || Time.time < knockUntil || unit && unit.Mythic && unit.Mythic.Stopped;
        bool casting;
        public bool Casting {get=>casting || unit && unit.Mythic && unit.Mythic.Casting;private set=>casting=value;}
        public bool IsLegend => unit && unit.Definition && unit.Definition.Rank == Rank.Legendary;
        public bool IsBoss => unit && unit.Team == UnitTeam.Enemy && unit.Definition && unit.Definition.EnemyTier == UnitDefinition.EnemyClass.Boss;
        public bool UsesMana => IsLegend || IsBoss || unit && unit.Mythic && unit.Mythic.Active && unit.Mythic.Kind != MythicKind.Undead;
        public bool Temporary { get; private set; }
        [SerializeField] float areaRadius = 3.5f;
        [SerializeField] float humanShockwaveRadius = 1.6f;
        // Keep gameplay area and its art tied to Human's radius, including future tuning.
        float ElfSkillRadius => humanShockwaveRadius * .75f;
        [SerializeField] float skillDamageMultiplier = 2f;
        Unit unit;
        BattleFlow flow;
        float shield, shieldUntil, roarUntil, frozenUntil, knockUntil;
        SpriteRenderer body;
        Vector3 bodyBase;
        bool lifted;
        Coroutine fallback;
        MonsterBuffVisual roarVisual;

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
                // One owner composes status RGB from a stable base, never last frame's tint.
                // Sprite alpha remains owned by death/reveal animation; hit flash uses its property block.
                bool frozen=Time.time < frozenUntil || unit.Mythic && unit.Mythic.Stopped;
                var hue=StatusTint.Compose(color.a,frozen,unit.Effects&&unit.Effects.Slowed&&(!body.sharedMaterial||!body.sharedMaterial.HasProperty("_SlowAmount")),Project.UI.SharedSettings.EffectsEnabled,unit.Effects&&unit.Effects.Burning&&(!body.sharedMaterial||!body.sharedMaterial.HasProperty("_BurnAmount")));
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
        bool CanFinishCast => InWave || flow && flow.Current == BattleFlow.Phase.Finished;
        public void BasicHit()
        {
            if(unit.Mythic && unit.Mythic.Active){unit.Mythic.BasicHit();return;}
            if (UsesMana && unit.IsAlive && InWave) GainBuffedMana(ManaPerHit);
        }
        public void ReceiveBasicHit()
        {
            if(unit.Mythic && unit.Mythic.Active){unit.Mythic.ReceiveBasicHit();return;}
            if (IsLegend && !IsBoss && unit.IsAlive && InWave)
                GainBuffedMana(4f);
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
            LegendSkillArt.Play("machine-shield", unit, unit.HitPoint,
                Mathf.Max(.65f, Mathf.Abs(unit.transform.lossyScale.x) * 1.6f), 8f, true);
        }
        public void RoarBuff()
        {
            roarUntil = Time.time + 5f;
            if (IsLegend && unit.Definition.Race == Race.Monster)
            {
                if (!roarVisual) roarVisual = gameObject.AddComponent<MonsterBuffVisual>();
                roarVisual.Refresh(roarUntil);
            }
        }
        public void Freeze() { frozenUntil = Mathf.Max(frozenUntil, Time.time + 2f); SkillEffects.Ring(unit.transform.position, .7f, Color.cyan, 2f); }
        public void KnockUp() { knockUntil = Time.time + .6f; }
        public void MarkTemporary() { Temporary = true; }
        void PhaseChanged(BattleFlow.Phase phase)
        {
            if (phase == BattleFlow.Phase.Finished)
            {
                if (Temporary) unit.EndSummonedLife();
                return; // Let already-playing cast animations/effects finish.
            }
            if (phase == BattleFlow.Phase.Marching || phase == BattleFlow.Phase.Fighting) return;
            Mana = 0f; shield = shieldUntil = roarUntil = frozenUntil = knockUntil = 0f;
            Casting = false;
            if (fallback != null) StopCoroutine(fallback);
            if (Temporary) { gameObject.SetActive(false); Destroy(gameObject); }
        }
        public bool TryCast(Unit target)
        {
            if(unit.Mythic && unit.Mythic.Active)return unit.Mythic.TryCast(target);
            if (!UsesMana || !unit.IsAlive || !InWave || Stunned || Casting || Mana < MaxMana || !target || !target.IsAlive || target.Team == unit.Team) return false;
            if (IsBoss && !BossTargetInRange(target)) return false;
            Mana = 0f;
            Casting = true;
            unit.Visual?.SetMoving(false);
            if (IsBoss)
            {
                fallback = StartCoroutine(BossDoubleAttack(target));
                return true;
            }
            var centre = target.transform.position;
            var frame = GetComponent<FrameUnitVisual>();
            if (frame) frame.PlaySkill(() => ApplySkill(centre), () => Casting = false);
            else fallback = StartCoroutine(FallbackCast(centre));
            return true;
        }
        bool BossTargetInRange(Unit target) => target && target.IsAlive && target.Team != unit.Team &&
            Vector2.Distance(unit.transform.position, target.transform.position) <= Mathf.Min(unit.Definition.AttackRange, .55f) + .05f;

        IEnumerator BossDoubleAttack(Unit target)
        {
            var frame = GetComponent<FrameUnitVisual>();
            for (int hit = 0; hit < 2; hit++)
            {
                if (!Casting || !unit.IsAlive || !InWave) break;
                if (!BossTargetInRange(target))
                {
                    target = null;
                    float nearest = float.PositiveInfinity;
                    foreach (var other in UnitRegistry.All)
                        if (BossTargetInRange(other))
                        {
                            float distance = Vector2.Distance(unit.transform.position, other.transform.position);
                            if (distance < nearest) { nearest = distance; target = other; }
                        }
                }
                if (!target) break;
                Unit victim = target;
                bool finished = false;
                System.Action impact = () =>
                {
                    // Skill hits do not grant mana to either combatant.
                    if (Casting && unit.IsAlive && InWave && BossTargetInRange(victim))
                        victim.TakeDamage(unit.AttackPower, unit, false);
                };
                if (frame)
                {
                    frame.SetFacing(victim.transform.position.x - unit.transform.position.x);
                    frame.PlaySkill(impact, () => finished = true);
                    while (!finished && Casting && unit.IsAlive && InWave) yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(.35f);
                    impact();
                    yield return new WaitForSeconds(.35f);
                }
            }
            Casting = false;
            fallback = null;
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
            if (!Casting || !unit.IsAlive || !CanFinishCast) return;
            if(unit.Team==UnitTeam.Ally)BattleContribution.SkillCasts++;
            switch (unit.Definition.Race)
            {
                case Race.Elf:
                    LegendSkillArt.Play("elf-frost-rain", unit, centre, ElfSkillRadius * 2.2f, 1.2f);
                    fallback = StartCoroutine(ArrowRain(centre));
                    break;
                case Race.Human:
                    LegendSkillArt.Play("human-shockwave", unit, transform.position, humanShockwaveRadius * 2.2f, .8f);
                    foreach (var enemy in UnitRegistry.All)
                        if (EnemyInArea(enemy, transform.position, humanShockwaveRadius)) { enemy.Combat.KnockUp(); enemy.TakeDamage(unit.AttackPower * skillDamageMultiplier, unit, false); }
                    break;
                case Race.Machine:
                    foreach (var ally in UnitRegistry.All)
                        if (ally && ally.IsAlive && ally.Team == unit.Team && ally.Definition && !ally.Definition.IsRanged) ally.Combat.GiveShield();
                    break;
                case Race.Dead: Summon(); break;
                case Race.Monster:
                    LegendSkillArt.Play("monster-roar", unit, unit.HitPoint + Vector3.up * .45f, 1.4f, .9f);
                    foreach (var ally in UnitRegistry.All)
                        if (ally && ally.IsAlive && ally.Team == unit.Team) { ally.Combat.RoarBuff(); SkillEffects.Ring(ally.transform.position, .55f, Color.red, .8f); }
                    break;
            }
        }
        bool EnemyInArea(Unit enemy, Vector3 centre) => EnemyInArea(enemy, centre, areaRadius);
        bool EnemyInArea(Unit enemy, Vector3 centre, float radius) => enemy && enemy.IsAlive && enemy.Mythic.Deployed && enemy.Team != unit.Team && (enemy.transform.position - centre).sqrMagnitude <= radius * radius;
        IEnumerator ArrowRain(Vector3 centre)
        {
            yield return new WaitForSeconds(.3f);
            if (!unit.IsAlive || !CanFinishCast) yield break;
            yield return new WaitForSeconds(.35f);
            if (!unit.IsAlive || !CanFinishCast) yield break;
            foreach (var enemy in UnitRegistry.All)
                if (EnemyInArea(enemy, centre, ElfSkillRadius)) { enemy.Combat.Freeze(); enemy.TakeDamage(unit.AttackPower * skillDamageMultiplier, unit, false); }
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
                LegendSkillArt.Play("dead-summon", unit, pos, 1.15f, 1.1f);
                var summoned = spawner.Spawn(common, unit.Team, pos, transform.parent);
                if (!summoned) continue;
                if(unit.Team==UnitTeam.Ally)BattleContribution.Summons++;
                summoned.SetSummonedStats(hp, attack);
                summoned.Combat.MarkTemporary();
                if (flow && flow.Current == BattleFlow.Phase.Finished)
                    summoned.EndSummonedLife();
                var brain = summoned.GetComponent<UnitBrain>();
                if (brain) { brain.SummonedCooldown = unit.Definition.AttackCooldown * 3f; brain.SummonedMoveSpeed = unit.Definition.MoveSpeed / 3f; brain.enabled = true; }
            }
        }
    }
}
