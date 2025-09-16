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

namespace MuvLuvAnnihilation.HarmonyInstance
{
    
    [HarmonyPatch(typeof(TrashUtility), "TrashJob")]
    public static class TrashUtility_TrashJob_BETA_Patch
    {
        public static void Postfix(Pawn pawn, Thing t, Job __result, bool allowPunchingInert = false, bool killIncappedTarget = false)
        {
            if (pawn.def == BETADefOf.BETA_SoldierClass && __result.def != RimWorld.JobDefOf.Ignite)
            {
				if (Rand.Value < 0.60f && pawn.natives.IgniteVerb != null && pawn.natives.IgniteVerb.IsStillUsableBy(pawn) && t.FlammableNow && !t.IsBurning() && !(t is Building_Door))
				{
					__result = JobMaker.MakeJob(RimWorld.JobDefOf.Ignite, t);
				}
				else
				{
					Building building = t as Building;
					if (!(building != null && building.def.building.isInert && !allowPunchingInert))
					{
						__result = JobMaker.MakeJob(RimWorld.JobDefOf.AttackMelee, t);
					}
				}
				__result.killIncappedTarget = killIncappedTarget;
				TrashUtility_TrashJob_BETA_Patch.FinalizeTrashJob(__result);
			}
		}
		// Token: 0x06002F41 RID: 12097 RVA: 0x001097C8 File Offset: 0x001079C8
		private static void FinalizeTrashJob(Job job)
		{
			job.expiryInterval = TrashUtility_TrashJob_BETA_Patch.TrashJobCheckOverrideInterval.RandomInRange;
			job.checkOverrideOnExpire = true;
			job.expireRequiresEnemiesNearby = true;
		}

		// Token: 0x04001AE6 RID: 6886
		private const float ChanceHateInertBuilding = 0.008f;

		// Token: 0x04001AE7 RID: 6887
		private static readonly IntRange TrashJobCheckOverrideInterval = new IntRange(450, 500);
	}
    
}