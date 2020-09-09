using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;

namespace MuvLuvBeta.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGraphicSet), "HairMatAt_NewTemp")]
    public static class PawnGraphicSet_HairMatAt_NewTemp_Test_Patch
    {
        public static void Postfix(PawnGraphicSet __instance, bool portrait, ref Material __result)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.apparel.AnyApparel && !portrait)
            {
                if (pawn.apparel.WornApparel.Any(x => x.TryGetComp<CompExosuitDrawer>() != null))
                {
                    foreach (var item in pawn.apparel.WornApparel)
                    {
                        CompExosuitDrawer extraDrawer = item.TryGetComp<CompExosuitDrawer>();
                        if (extraDrawer != null && extraDrawer.hidesHair)
                        {
                            __result = Constants.InvisibleGraphics(pawn).hairGraphic.MatSingle;
                            return;
                        }
                    }
                }
            }
        }
    }

}
