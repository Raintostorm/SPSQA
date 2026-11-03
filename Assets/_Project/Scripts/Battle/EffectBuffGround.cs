using System.Collections.Generic;
using Project.Data;
using UnityEngine;
namespace Project.Battle
{
    public sealed partial class EffectBuffSystem
    {
        sealed class GroundTile
        {public EffectBuffKind Kind;public Vector3 Position;public Vector2 Size=Vector2.one;public int Cell=-1;public GameObject Art;public SpriteRenderer Fill;public LineRenderer Edge;}
        readonly List<GroundTile> tiles=new();readonly Dictionary<Unit,EffectBuffKind> waveOccupants=new();float healTimer;
        static Material zoneMaterial;static Sprite square;
        // Fixed prep cell for the run. Healing runs only in combat; occupant assignments persist through each combat wave.
        void CreateTile(EffectBuffKind kind)
        {
            var tile=new GroundTile{Kind=kind};tiles.Add(tile);PlaceTile(tile);
        }
        void PlaceTile(GroundTile tile)
        {
            var bg=FindFirstObjectByType<Project.Visual.BattleBackground>();var camera=Camera.main;if(!bg||!camera)return;
            if(!board)return;
            var bounds=bg.CombatBounds;var grid=board.Grid;var boardView=FindFirstObjectByType<BoardView>();
            float bottom=bounds.max.y-bounds.size.y*.625f+.08f,top=bounds.max.y-bounds.size.y*.365f-.08f;
            float half=camera.orthographicSize*camera.aspect;
            var candidates=new List<int>();var visible=new List<int>();var used=new HashSet<int>();
            foreach(var existing in tiles)if(existing!=tile&&existing.Cell>=0)used.Add(existing.Cell);
            for(int r=0;r<grid.Rows;r++)for(int c=0;c<grid.Cols;c++)
            {
                var centre=boardView?boardView.HomeCell(board,r,c):grid.CellToWorld(r,c);
                bool onScreen=Mathf.Abs(centre.x-camera.transform.position.x)+grid.CellWidth*.5f<=half
                    &&Mathf.Abs(centre.y-camera.transform.position.y)+grid.CellHeight*.5f<=camera.orthographicSize;
                if(!onScreen)continue;
                int cell=r*grid.Cols+c;visible.Add(cell);
                if(centre.y-grid.CellHeight*.5f>=bottom&&centre.y+grid.CellHeight*.5f<=top)candidates.Add(cell);
            }
            // Align to actual prep cells, prefer cells fully on the combat road. Never overlap another zone.
            if(!GroundBuffCells.TryChoose(candidates,used,random,out int chosen)
                &&!GroundBuffCells.TryChoose(visible,used,random,out chosen))return;
            tile.Cell=chosen;
            tile.Position=boardView?boardView.HomeCell(board,chosen/grid.Cols,chosen%grid.Cols):grid.CellToWorld(chosen/grid.Cols,chosen%grid.Cols);
            tile.Size=new Vector2(grid.CellWidth,grid.CellHeight);
            if(!tile.Art)
            {
                tile.Art=new GameObject(tile.Kind.ToString());
                if(!square){var texture=new Texture2D(1,1);texture.SetPixel(0,0,Color.white);texture.Apply();square=Sprite.Create(texture,new Rect(0,0,1,1),Vector2.one*.5f,1);}
                var fill=new GameObject("Surface");fill.transform.SetParent(tile.Art.transform,false);tile.Fill=fill.AddComponent<SpriteRenderer>();tile.Fill.sprite=square;tile.Fill.sortingOrder=BattleGrid.TileOrder+100;
                var edge=new GameObject("Edge");edge.transform.SetParent(tile.Art.transform,false);tile.Edge=edge.AddComponent<LineRenderer>();
                if(!zoneMaterial)zoneMaterial=new Material(Shader.Find("Sprites/Default"));tile.Edge.sharedMaterial=zoneMaterial;tile.Edge.positionCount=4;tile.Edge.loop=true;tile.Edge.useWorldSpace=false;tile.Edge.startWidth=tile.Edge.endWidth=.045f;tile.Edge.sortingOrder=BattleGrid.TileOrder+101;
                var icon=new GameObject("Emblem");icon.transform.SetParent(tile.Art.transform,false);var sprite=icon.AddComponent<SpriteRenderer>();
                sprite.sprite=Project.UI.EffectBuffArt.Get(tile.Kind);
                sprite.color=Color.white;
                sprite.sortingOrder=BattleGrid.TileOrder+102;
                if(sprite.sprite)icon.transform.localScale=new Vector3(1f/sprite.sprite.bounds.size.x,1f/sprite.sprite.bounds.size.y,1);
            }
            tile.Art.transform.position=tile.Position;tile.Art.transform.localScale=new Vector3(tile.Size.x,tile.Size.y,1);
            tile.Fill.enabled=false;tile.Edge.enabled=false;
            var rise=tile.Art.GetComponent<GroundTileRise>();if(!rise)rise=tile.Art.AddComponent<GroundTileRise>();
            rise.Configure(tile.Position,tile.Size);
            if(tile.Kind==EffectBuffKind.BloodTile&&!tile.Art.GetComponent<BloodRainGroundVisual>())tile.Art.AddComponent<BloodRainGroundVisual>();

            tile.Edge.SetPositions(new[]{new Vector3(-.5f,-.5f,0),new Vector3(-.5f,.5f,0),new Vector3(.5f,.5f,0),new Vector3(.5f,-.5f,0)});
        }
        bool Fighting=>flow&&(flow.Current==BattleFlow.Phase.Marching||flow.Current==BattleFlow.Phase.Fighting);
        bool Inside(Unit unit,EffectBuffKind kind)
        {
            if(!unit||!unit.IsAlive||unit.Team!=UnitTeam.Ally||!flow)return false;
            if(Fighting)return unit.Mythic.Deployed&&waveOccupants.TryGetValue(unit,out var assigned)&&assigned==kind;
            if(flow.Current!=BattleFlow.Phase.Prep||!board)return false;
            foreach(var tile in tiles)
            {
                if(tile.Kind!=kind||tile.Cell<0)continue;
                foreach(var member in board.UnitsAt(tile.Cell/board.Grid.Cols,tile.Cell%board.Grid.Cols))if(member==unit)return true;
            }
            return false;
        }
        public float TileLifesteal(Unit source)=>Fighting&&Inside(source,EffectBuffKind.BloodTile)?EffectBuffMath.Lifesteal(Level(EffectBuffKind.BloodTile)):0;
        public void OnDamage(Unit source,Unit victim,bool basic,bool killed)
        {
            if(!Fighting||!source||!victim||source.Team!=UnitTeam.Ally||victim.Team!=UnitTeam.Enemy)return;
            if(killed&&Inside(source,EffectBuffKind.GoldTile)){int coins=EffectBuffMath.Coins(Level(EffectBuffKind.GoldTile));roll.Run.AddCoins(coins);FloatingCoin(source.transform.position,coins);}
            if(basic)source.Effects?.BasicHit(victim);
        }
        static void FloatingCoin(Vector3 position,int coins)=>Project.Visual.DamagePopup.ShowCoins(position+Vector3.up*.5f,coins);
        void TickTiles()
        {
            foreach(var tile in tiles)
            {
                if(!tile.Art)PlaceTile(tile); // Retry safely if camera/background was not ready at selection.
                if(!tile.Art)continue;
                // Ground markers are formation guides only; captured occupants keep their combat buffs.
                tile.Art.SetActive(flow&&flow.Current==BattleFlow.Phase.Prep);
                var color=tile.Kind==EffectBuffKind.BloodTile?new Color(1,.12f,.25f):new Color(1,.8f,.06f);
                color.a=.075f;tile.Fill.color=color;
            }
            if(!flow||flow.Current!=BattleFlow.Phase.Fighting){healTimer=0;return;}
            healTimer+=Time.deltaTime;
            if(healTimer<1)return;healTimer-=1;
            foreach(var u in UnitRegistry.All)if(Inside(u,EffectBuffKind.BloodTile)){u.Heal(u.MaxHp*EffectBuffMath.HealFraction(Level(EffectBuffKind.BloodTile)),true);Project.Visual.SkillEffects.Ring(u.transform.position,.23f,new Color(.2f,1,.4f),.35f);}
        }
        void TilePhase(BattleFlow.Phase phase)
        {
            if(phase==BattleFlow.Phase.Prep){waveOccupants.Clear();healTimer=0;}
            if(phase!=BattleFlow.Phase.Marching)return;
            waveOccupants.Clear();healTimer=0;
            foreach(var tile in tiles)
            {
                if(!tile.Art)PlaceTile(tile);
                if(!board||tile.Cell<0)continue;
                foreach(var member in board.UnitsAt(tile.Cell/board.Grid.Cols,tile.Cell%board.Grid.Cols))
                    if(member&&member.IsAlive&&member.Team==UnitTeam.Ally)waveOccupants[member]=tile.Kind;
            }
        }
        void ClearTiles(){foreach(var tile in tiles)if(tile.Art)Destroy(tile.Art);tiles.Clear();waveOccupants.Clear();healTimer=0;}
    }
}
