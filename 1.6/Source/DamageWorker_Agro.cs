using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000230 RID: 560
	public class DamageWorker_Agro : DamageWorker
	{
		// Token: 0x06000FA0 RID: 4000 RVA: 0x0005A8ED File Offset: 0x00058AED
		public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
		{
			DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
			Pawn v = victim as Pawn;
			if (v!=null && v.RaceProps.FleshType.defName == "BETAFlesh")
			{
				v.jobs.StopAll();
			//	v.jobs.StartJob(new AI.Job(JobDefOf.AttackMelee,dinfo.Instigator));
				v.mindState.enemyTarget = dinfo.Instigator;
			}
			return damageResult;
		}

		public DamageInfo damageInfo;

		// Token: 0x04000244 RID: 580
		private static List<Thing> thingsToAffect = new List<Thing>();

		// Token: 0x04000245 RID: 581
		private static List<IntVec3> openCells = new List<IntVec3>();

		// Token: 0x04000246 RID: 582
		private static List<IntVec3> adjWallCells = new List<IntVec3>();


		// Token: 0x060004D7 RID: 1239 RVA: 0x00018398 File Offset: 0x00016598
		public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
		{

		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00018524 File Offset: 0x00016724
		public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
		{
			DamageWorker_Agro.thingsToAffect.Clear();
			float num = float.MinValue;
			bool flag = false;
			List<Thing> list = explosion.Map.thingGrid.ThingsListAt(c);
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (thing.def.category != ThingCategory.Mote && thing.def.category != ThingCategory.Ethereal)
				{
					DamageWorker_Agro.thingsToAffect.Add(thing);
					if (thing.def.Fillage == FillCategory.Full && thing.def.Altitude > num)
					{
						flag = true;
						num = thing.def.Altitude;
					}
				}
			}
			for (int j = 0; j < DamageWorker_Agro.thingsToAffect.Count; j++)
			{
				if (DamageWorker_Agro.thingsToAffect[j].def.Altitude >= num)
				{
					this.ExplosionDamageThing(explosion, DamageWorker_Agro.thingsToAffect[j], damagedThings, ignoredThings, c);
				}
			}
		}

        // Token: 0x060004DA RID: 1242 RVA: 0x000187A4 File Offset: 0x000169A4
        public override void ExplosionDamageThing(Explosion explosion, Thing t, List<Thing> damagedThings, List<Thing> ignoredThings, IntVec3 cell)
		{
			if (t.def.category == ThingCategory.Mote || t.def.category == ThingCategory.Ethereal)
			{
				return;
			}
			if (damagedThings.Contains(t))
			{
				return;
			}
			damagedThings.Add(t);
			if (ignoredThings != null && ignoredThings.Contains(t))
			{
				return;
			}
			if (this.def == DamageDefOf.Bomb && t.def == ThingDefOf.Fire && !t.Destroyed)
			{
				t.Destroy(DestroyMode.Vanish);
				return;
			}
			float angle;
			if (t.Position == explosion.Position)
			{
				angle = (float)Rand.RangeInclusive(0, 359);
			}
			else
			{
				angle = (t.Position - explosion.Position).AngleFlat;
			}
			DamageInfo dinfo = new DamageInfo(this.def, (float)explosion.GetDamageAmountAt(cell), explosion.GetArmorPenetrationAt(cell), angle, explosion.instigator, null, explosion.weapon, DamageInfo.SourceCategory.ThingOrUnknown, explosion.intendedTarget);
			if (this.def.explosionAffectOutsidePartsOnly)
			{
				dinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
			}
			BattleLogEntry_ExplosionImpact battleLogEntry_ExplosionImpact = null;
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				battleLogEntry_ExplosionImpact = new BattleLogEntry_ExplosionImpact(explosion.instigator, t, explosion.weapon, explosion.projectile, this.def);
				Find.BattleLog.Add(battleLogEntry_ExplosionImpact);
			}
			DamageWorker.DamageResult damageResult = t.TakeDamage(dinfo);
			damageResult.AssociateWithLog(battleLogEntry_ExplosionImpact);
			if (pawn != null && damageResult.wounded && pawn.stances != null)
			{
				pawn.stances.StaggerFor(95);
			}
		}

	}
}
