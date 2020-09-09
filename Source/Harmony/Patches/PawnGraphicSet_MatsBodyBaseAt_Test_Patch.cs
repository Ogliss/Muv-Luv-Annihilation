using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;

namespace MuvLuvBeta.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGraphicSet), "MatsBodyBaseAt")]
    public static class PawnGraphicSet_MatsBodyBaseAt_Test_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        public static void Postfix(PawnGraphicSet __instance, ref List<Material> __result)
        {
            Pawn pawn = __instance.pawn;
            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x => x.TryGetComp<CompExosuitDrawer>() != null))
                {
                    foreach (var item in pawn.apparel.WornApparel)
                    {
                        CompExosuitDrawer extraDrawer = item.TryGetComp<CompExosuitDrawer>();
                        if (extraDrawer != null && extraDrawer.hidesBody)
                        {
                            for (int i = 0; i < __result.Count; i++)
                            {
                                __result[i] = Constants.InvisibleGraphics(pawn).nakedGraphic.MatSingle;
                            }
                            return;
                        }
                    }
                }
            }
        }
    }

}
