using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace MuvLuvAnnihilation.HarmonyInstance
{


    //Current effective verb influence target pick.
    [HarmonyPatch(typeof(Pawn), "TryGetAttackVerb")]
    public static class Pawn_TryGetAttackVerb_VerbCheckNonhumanlike_Patch
    {
        static bool Prefix(ref Pawn __instance, ref Verb __result)
        {
            //If animal don't bother
            if (__instance.RaceProps.Humanlike)
                return true;

            List<Verb> verbList = __instance.verbTracker.AllVerbs;
            for (int i = 0; i < verbList.Count; i++)
            {
                if (verbList[i].verbProps.range > 1.5f)
                {
                    //found range verb return first one in the list
                    __result = verbList[i];
                    return false;
                }
            }
            return true;

        }
    }
}
