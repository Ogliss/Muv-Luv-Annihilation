using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MuvLuvAnnihilation.HarmonyInstance
{
    //Current effective verb influence target pick.
//    [HarmonyPatch(typeof(Pawn), "get_VerbProperties")]
    public static class Pawn_VerbProperties_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref Pawn __instance, ref List<VerbProperties> __result)
        {
            if (__instance.equipment!=null)
            {
                if (__instance.equipment.PrimaryEq.PrimaryVerb.verbProps.range>1.5f)
                {
                    return;
                }
            }
            if (__instance.health.hediffSet.hediffs.Any(x=>x.TryGetComp<HediffComp_VerbGiver>()!=null))
            {
                foreach (HediffWithComps hdc in __instance.health.hediffSet.hediffs.Where(x=> x.def.HasComp(typeof(HediffComp_VerbGiver))))
                {
                    HediffComp_VerbGiver _VerbGiver = hdc.TryGetComp<HediffComp_VerbGiver>();
                    if (_VerbGiver.Props.verbs!=null)
                    {
                        foreach (VerbProperties verb in _VerbGiver.Props.verbs)
                        {
                            if (!__result.Contains(verb))
                            {
                                __result.Add(verb);
                            }
                        }
                    }
                }

            }
        }
    }
}
