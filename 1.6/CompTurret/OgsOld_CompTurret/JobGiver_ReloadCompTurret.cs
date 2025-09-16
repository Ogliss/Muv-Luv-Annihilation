using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace OgsOld_CompTurret
{
	// OgsOld_CompTurret.JobGiver_ReloadOgsOld_CompTurret
	public class JobGiver_ReloadOgsOld_CompTurret : ThinkNode_JobGiver
	{
		// Token: 0x0600313A RID: 12602 RVA: 0x00114108 File Offset: 0x00112308
		public override float GetPriority(Pawn pawn)
		{
			return 5.9f;
		}

		// Token: 0x0600313B RID: 12603 RVA: 0x00114110 File Offset: 0x00112310
		public override Job TryGiveJob(Pawn pawn)
		{

			foreach (OgsOld_CompTurretGun compReloadable in OgsOld_CompTurretReloadableUtility.FindSomeReloadableComponents(pawn, false))
			{
				if (compReloadable == null)
				{
					continue;
				}
				List<Thing> list = OgsOld_CompTurretReloadableUtility.FindEnoughAmmo(pawn, pawn.Position, compReloadable, false);
				if (list == null)
				{
					continue;
				}
				return JobGiver_ReloadOgsOld_CompTurret.MakeReloadJob(compReloadable, list);
			}
			return null;
		}

		// Token: 0x0600313C RID: 12604 RVA: 0x00114148 File Offset: 0x00112348
		public static Job MakeReloadJob(OgsOld_CompTurretGun comp, List<Thing> chosenAmmo)
		{
			Job job = JobMaker.MakeJob(JobDefOf.OgsOld_CompTurretReload, comp.parent, null, targetC: comp.gun);
			job.targetQueueB = (from t in chosenAmmo
								select new LocalTargetInfo(t)).ToList<LocalTargetInfo>();
			job.count = chosenAmmo.Sum((Thing t) => t.stackCount);
			
			job.count = Math.Min(job.count, comp.MaxAmmoNeeded(true));
			return job;
		}

		// Token: 0x04001B62 RID: 7010
		private const bool forceReloadWhenLookingForWork = false;
	}
}
