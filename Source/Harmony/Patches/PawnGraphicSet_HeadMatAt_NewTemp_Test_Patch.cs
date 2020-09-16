using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using UnityEngine;

namespace MuvLuvAnnihilation.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGraphicSet), "HeadMatAt_NewTemp")]
    public static class PawnGraphicSet_HeadMatAt_NewTemp_Test_Patch
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
                        if (extraDrawer != null && extraDrawer.hidesHead)
                        {
                            __result = Constants.InvisibleGraphics(pawn).headGraphic.MatSingle;
                            return;
                        }
                    }
                }
            }
        }
    }

}
