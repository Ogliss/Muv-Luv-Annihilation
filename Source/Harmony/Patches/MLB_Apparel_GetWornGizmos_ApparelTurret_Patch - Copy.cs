using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;

namespace MuvLuvBeta.HarmonyInstance
{
    
    [HarmonyPatch(typeof(Apparel), "CheckPreAbsorbDamage")]
    public static class MLB_Apparel_CheckPreAbsorbDamage_ApparelTurret_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Apparel __instance, DamageInfo dinfo, ref bool __result)
        {
            if (__instance == null)
            {
                return;
            }
            Comp_Turret apparel_Turret = __instance.TryGetComp<Comp_Turret>();
            if (apparel_Turret!=null)
            {
                if (dinfo.Def == DamageDefOf.EMP)
                {
                    foreach (ThingComp item in __instance.AllComps)
                    {
                        Comp_TurretGun Turret = item as Comp_TurretGun;
                        if (Turret != null && Turret.AffectedByEMP)
                        {
                            Turret.stunTicksLeft += Mathf.RoundToInt(dinfo.Amount * 30f);
                            Turret.stunFromEMP = true;
                        //    Log.Message(Turret.gun.Label + " turret hit by EMP, disabling for " + Mathf.RoundToInt(dinfo.Amount * 30f) + " ticks, Total: "+ Turret.stunTicksLeft);
                        }
                    }
                }
            }
        }
    }
    
}