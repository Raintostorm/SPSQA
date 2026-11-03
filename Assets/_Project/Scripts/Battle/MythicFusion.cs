using System;
using System.Collections.Generic;
using Project.Data;
using Project.Meta;
using UnityEngine;
namespace Project.Battle
{
    public static class MythicFusion
    {
        public const string CapacityRelic="astral_contract";
        [Serializable] public class Recipe {public string mythic;public string[] ingredients;}
        [Serializable] class Book {public Recipe[] recipes;}
        static Recipe[] recipes;
        public static int Limit=>2+Mathf.Max(0,SaveSystem.Current.FindItem(CapacityRelic)?.Level??0)/2;
        public static int Count {get{int n=0;foreach(var u in UnitRegistry.All)if(u&&u.Team==UnitTeam.Ally&&u.Definition&&u.Definition.Rank==Rank.Mythic)n++;return n;}}
        public static Recipe For(UnitDefinition output)
        {
            if(recipes==null){var json=Resources.Load<TextAsset>("Mythics/recipes");recipes=json?JsonUtility.FromJson<Book>(json.text).recipes:Array.Empty<Recipe>();}
            foreach(var r in recipes)if(r.mythic==output.UnitId)return r;return null;
        }
        public static bool CanFuse(BattleBoard board,UnitDefinition output,out List<Unit> chosen,out string reason)
        {
            chosen=new List<Unit>();if(!board||!output||output.Rank!=Rank.Mythic){reason="Invalid fusion request";return false;}var flow=UnityEngine.Object.FindFirstObjectByType<BattleFlow>();
            if(!flow||flow.Current!=BattleFlow.Phase.Prep){reason="Preparation only";return false;}
            if(Count>=Limit){reason="Mythic limit reached";return false;}
            if(!MatchIngredients(board,output,out chosen,out reason))return false;
            if(!FindDestination(board,chosen,out _,out _)){reason="No free cell after fusion";return false;}
            reason="Ready to fuse";return true;
        }
        public static bool MatchIngredients(BattleBoard board,UnitDefinition output,out List<Unit> chosen,out string reason)
        {
            chosen=new List<Unit>();
            if(!board||!output||output.Rank!=Rank.Mythic){reason="Invalid fusion request";return false;}
            var recipe=For(output);
            if(recipe?.ingredients==null||recipe.ingredients.Length!=3){reason="Recipe awaiting configuration";return false;}
            for(int i=0;i<recipe.ingredients.Length;i++)
            {
                Unit found=null;
                for(int r=0;r<board.Grid.Rows;r++)for(int c=0;c<board.Grid.Cols;c++)foreach(var u in board.UnitsAt(r,c))if(u&&u.IsAlive&&u.Team==UnitTeam.Ally&&u.Definition.UnitId==recipe.ingredients[i]&&!chosen.Contains(u)){found=u;break;}
                if(!found){reason="Missing ingredients";return false;}chosen.Add(found);
            }
            int legends=0,epics=0,rares=0,commons=0;
            foreach(var u in chosen)switch(u.Definition.Rank){case Rank.Legendary:legends++;break;case Rank.Epic:epics++;break;case Rank.Rare:rares++;break;case Rank.Common:commons++;break;}
            if(legends+epics<1||legends+epics+rares+commons!=3||
                (recipe.ingredients[0]==recipe.ingredients[1]&&recipe.ingredients[1]==recipe.ingredients[2])){reason="Invalid recipe";return false;}
            reason="Ingredients complete";return true;
        }
        static bool FindDestination(BattleBoard board,List<Unit> consumed,out int row,out int col)
        {
            for(int r=0;r<board.Grid.Rows;r++)for(int c=0;c<board.Grid.Cols;c++)
            {bool empty=true;foreach(var u in board.UnitsAt(r,c))if(!consumed.Contains(u)){empty=false;break;}if(empty){row=r;col=c;return true;}}
            row=col=-1;return false;
        }
        public static bool Fuse(BattleBoard board,UnitDefinition output,UnitSpawner spawner)
        {
            if(!spawner||!CanFuse(board,output,out var ingredients,out _)||!FindDestination(board,ingredients,out int r,out int c))return false;
            // Create first, consume only after all validation and successful spawning.
            var result=spawner.Spawn(output,UnitTeam.Ally,board.Grid.CellToWorld(r,c),board.transform);if(!result)return false;
            var brain=result.GetComponent<UnitBrain>();if(brain)brain.enabled=false;result.Visual?.SetFacing(1);
            var homes=new Dictionary<Unit,Vector2Int>();
            for(int row=0;row<board.Grid.Rows;row++)for(int col=0;col<board.Grid.Cols;col++)foreach(var u in board.UnitsAt(row,col))if(ingredients.Contains(u))homes[u]=new Vector2Int(row,col);
            foreach(var u in ingredients)board.Remove(u);
            if(!board.Place(result,r,c))
            {UnityEngine.Object.Destroy(result.gameObject);foreach(var u in ingredients)board.Place(u,homes[u].x,homes[u].y);return false;}
            foreach(var u in ingredients){u.gameObject.SetActive(false);UnityEngine.Object.Destroy(u.gameObject);}
            Project.Visual.MergeReveal.Play(result);return true;
        }
    }
}
