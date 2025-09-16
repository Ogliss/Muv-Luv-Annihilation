using System.Text;
using Verse;
using RimWorld;
using HarmonyLib;
using RimWorld.Planet;

namespace MuvLuvAnnihilation.HarmonyInstance
{
    [HarmonyPatch(typeof(Settlement), nameof(Settlement.GetInspectString))]
	public static class Settlement_GetInspectString_Patch
	{
		public static bool Prefix(Settlement __instance, ref string __result)
		{
			if (__instance.Faction.def == BETADefOf.MuvLuv_BETA)
			{
				__result = GetInspectString(__instance);
                return false;
			}
			return true;
		}
        public static string GetInspectString(Settlement __settlement)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (__settlement.Faction != null && __settlement.AppendFactionToInspectString)
            {
                stringBuilder.Append("Faction".Translate() + ": " + __settlement.Faction.Name);
            }
            QuestUtility.AppendInspectStringsFromQuestParts(stringBuilder, __settlement);

            string text = stringBuilder.ToString();
            if (__settlement.Faction != Faction.OfPlayer)
            {
                if (!text.NullOrEmpty())
                {
                    text += "\n";
                }
                text += __settlement.Faction.PlayerRelationKind.GetLabelCap();
            }
            return text;
        }
    }

}
