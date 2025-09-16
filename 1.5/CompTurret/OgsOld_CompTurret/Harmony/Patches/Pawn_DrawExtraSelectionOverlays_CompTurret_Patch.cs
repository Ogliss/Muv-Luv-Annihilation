using OgsOld_CompTurret.ExtensionMethods;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace OgsOld_CompTurret.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn), "DrawExtraSelectionOverlays")]
    public static class Pawn_DrawExtraSelectionOverlays_OgsOld_CompTurret_Patch
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
                        OgsOld_CompTurret turret = item.TryGetCompFast<OgsOld_CompTurret>();
                        if (turret!=null)
                        {
                            foreach (OgsOld_CompTurret comp in item.GetComps<OgsOld_CompTurret>())
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
