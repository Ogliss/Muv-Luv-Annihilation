using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MuvLuvBeta.HarmonyInstance
{
    //Current effective verb influence target pick.
    [HarmonyPatch(typeof(Pawn), "DrawExtraSelectionOverlays")]
    public static class MLB_Pawn_DrawExtraSelectionOverlays_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref Pawn __instance)
        {
            if (__instance.apparel!=null)
            {
                if (__instance.apparel.WornApparel.Any())
                {
                    foreach (var item in __instance.apparel.WornApparel)
                    {
                        CompApparel_Turret turret = item.TryGetComp<CompApparel_Turret>();
                        if (turret!=null)
                        {
                            foreach (CompApparel_Turret comp in item.GetComps<CompApparel_Turret>())
                            {
                                comp.PostDrawExtraSelectionOverlays();
                            }
                        }
                    }

                }
            }
        }
    }
}
