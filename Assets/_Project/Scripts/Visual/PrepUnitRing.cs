using System.Collections.Generic;
using Project.Battle;
using Project.Data;
using UnityEngine;

namespace Project.Visual
{
    /// <summary>World-space ground marker, visible only on the arrangement board.</summary>
    [RequireComponent(typeof(Unit))]
    public sealed class PrepUnitRing : MonoBehaviour
    {
        static readonly HashSet<Unit> selected = new HashSet<Unit>();
        Unit unit;
        BattleFlow flow;
        SpriteRenderer body;
        LineRenderer ring;
        LineRenderer outline;
        Material material;
        float radius;
        public bool Dragging { get; set; }
        public bool EquipmentTarget { get; set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetSelection() => selected.Clear();

        public static void Select(IEnumerable<Unit> units)
        {
            selected.Clear();
            if (units != null) foreach (var u in units) if (u != null) selected.Add(u);
        }

        void Start()
        {
            unit = GetComponent<Unit>();
            body = GetComponent<FrameUnitVisual>()?.BodyRenderer;
            flow = FindAnyObjectByType<BattleFlow>();
            radius = Mathf.Clamp(body != null ? body.bounds.size.y * .30f : .25f, .16f, .42f);
            var marker = new GameObject("PrepGroundRing");
            marker.transform.SetParent(transform, false);
            ring = marker.AddComponent<LineRenderer>();
            material = new Material(Shader.Find("Sprites/Default"));
            ring.sharedMaterial = material;
            ring.useWorldSpace = true;
            ring.loop = true;
            ring.positionCount = 48;
            ring.numCornerVertices = 2;
            ring.numCapVertices = 2;
            ring.enabled = false;
            var backing = new GameObject("RingOutline");
            backing.transform.SetParent(marker.transform, false);
            outline = backing.AddComponent<LineRenderer>();
            outline.sharedMaterial = material;
            outline.useWorldSpace = true;
            outline.loop = true;
            outline.positionCount = ring.positionCount;
            outline.numCornerVertices = outline.numCapVertices = 2;
            outline.startColor = outline.endColor = new Color(.04f,.06f,.09f,1);
            outline.enabled = false;
        }

        void LateUpdate()
        {
            if (ring == null) return;
            ring.enabled = flow != null && flow.Current == BattleFlow.Phase.Prep &&
                unit.IsAlive && (body == null || body.enabled);
            outline.enabled = ring.enabled;
            if (!ring.enabled) return;
            bool highlight = Dragging || EquipmentTarget || selected.Contains(unit);
            var color = EquipmentTarget ? new Color(.25f,1f,.45f) : ColorForRank(unit.Definition != null ? unit.Definition.Rank : Rank.Common);
            ring.startColor = ring.endColor = color;
            ring.startWidth = ring.endWidth = radius * (highlight ? .22f : .16f);
            outline.startWidth = outline.endWidth = ring.startWidth + radius * .09f;
            if (body != null)
            {
                ring.sortingLayerID = body.sortingLayerID;
                ring.sortingOrder = body.sortingOrder - 1;
                outline.sortingLayerID = ring.sortingLayerID;
                outline.sortingOrder = ring.sortingOrder - 1;
            }
            DrawRing();
        }

        public static Color ColorForRank(Rank rank) => rank switch
            {
                Rank.Rare => new Color(.16f,.58f,1f,1),
                Rank.Epic => new Color(.72f,.34f,1f,1),
                Rank.Legendary => new Color(1f,.8f,.12f,1),
                Rank.Mythic => new Color(1f,.35f,.12f,1),
                _ => Color.white,
            };

        void DrawRing()
        {
            // Root is the ground anchor. Do not follow the sprite's breathing,
            // animation bounds, rotation or flipped local scale.
            var centre = transform.position;
            for (int i = 0; i < ring.positionCount; i++)
            {
                float angle = i * Mathf.PI * 2 / ring.positionCount;
                ring.SetPosition(i, centre + new Vector3(Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius * .28f, 0));
                outline.SetPosition(i,ring.GetPosition(i));
            }
        }

        void OnDisable()
        {
            selected.Remove(GetComponent<Unit>());
            Dragging = false;
            if (ring != null) ring.enabled = false;
            if (outline != null) outline.enabled = false;
        }

        void OnDestroy() { if (material != null) Destroy(material); }
    }
}
