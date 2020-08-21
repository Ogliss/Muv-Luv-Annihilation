using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MuvLuvBeta.HarmonyInstance
{

    [HarmonyPatch(typeof(GetOrGenerateMapUtility), "GetOrGenerateMap", new[] { typeof(int), typeof(IntVec3), typeof(WorldObjectDef) })]
    public static class MLB_GetOrGenerateMapUtility_GetOrGenerateMap_BETA_Patch
    {
        public static void Prefix(int tile, ref IntVec3 size, WorldObjectDef suggestedMapParentDef)
        {
            Map map = Current.Game.FindMap(tile);
            Faction faction = null;
            if (map != null)
            {
                if (map.ParentFaction != null)
                {
                    faction = map.ParentFaction;
                }
            }
            else
            {
                MapParent mapParent = Find.WorldObjects.MapParentAt(tile);
                if (mapParent.Faction!=null)
                {
                    faction = mapParent.Faction;
                }
            }
            if (faction == null)
            {
                return;
            }

            if (faction.def.defName.Contains("MuvLuv_BETA"))
            {
            //    Log.Message("Beta hive map size original " + size);
                if (faction.def.defName.Contains("2P"))
                {
                    //    mult = 1.2f;
                }
                if (faction.def.defName.Contains("3P"))
                {
                    //    mult = 1.4f;
                }
                if (faction.def.defName.Contains("4P"))
                {
                    //    mult = 1.6f;
                }
                if (faction.def.defName.Contains("5P"))
                {
                    size.x += 25;
                    size.z += 25;
                //    Log.Message("Beta hive map size " + size);
                }
                if (faction.def.defName.Contains("6P"))
                {
                    size.x += 50;
                    size.z += 50;
                //    Log.Message("Beta hive map size " + size);
                }
            }
        }
    }
    
}