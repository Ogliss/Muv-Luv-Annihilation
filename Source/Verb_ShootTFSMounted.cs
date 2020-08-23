using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace MuvLuvBeta
{
	// Token: 0x02000490 RID: 1168 MuvLuvBeta.Verb_ShootTFSMounted
	public class Verb_ShootTFSMounted : Verb_LaunchProjectile
	{
		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x060022F5 RID: 8949 RVA: 0x000D54D3 File Offset: 0x000D36D3
		protected override int ShotsPerBurst
		{
			get
			{
				return this.verbProps.burstShotCount;
			}
		}

		// Token: 0x060022F6 RID: 8950 RVA: 0x000D54E0 File Offset: 0x000D36E0
		public override void WarmupComplete()
		{
			Log.Message("WarmupComplete");
			base.WarmupComplete();
			Pawn pawn = this.currentTarget.Thing as Pawn;
			if (pawn != null && !pawn.Downed && this.CasterIsPawn && this.CasterPawn.skills != null)
			{
				float num = pawn.HostileTo(this.caster) ? 170f : 20f;
				float num2 = this.verbProps.AdjustedFullCycleTime(this, this.CasterPawn);
				this.CasterPawn.skills.Learn(SkillDefOf.Shooting, num * num2, false);
			}
		}

		// Token: 0x060022F7 RID: 8951 RVA: 0x000D556B File Offset: 0x000D376B
		protected override bool TryCastShot()
		{
			Log.Message("TryCastShot");
			if (this.currentTarget.HasThing && this.currentTarget.Thing.Map != this.caster.Map)
			{
				Log.Message("TGT Wrong Map");
				return false;
			}
			ThingDef projectile = this.Projectile;
			if (projectile == null)
			{
				Log.Message("projectile == null");
				return false;
			}
			ShootLine shootLine;
			bool flag = base.TryFindShootLineFromTo(this.caster.Position, this.currentTarget, out shootLine);
			if (this.verbProps.stopBurstWithoutLos && !flag)
			{
				Log.Message("stopBurstWithoutLos");
				return false;
			}
			if (base.EquipmentSource != null)
			{
				CompChangeableProjectile comp = base.EquipmentSource.GetComp<CompChangeableProjectile>();
				if (comp != null)
				{
					comp.Notify_ProjectileLaunched();
				}
				CompReloadable comp2 = base.EquipmentSource.GetComp<CompReloadable>();
				if (comp2 != null)
				{
					comp2.UsedOnce();
				}
			}
			Thing launcher = this.caster;
			Thing equipment = base.EquipmentSource;
			CompMannable compMannable = this.caster.TryGetComp<CompMannable>();
			if (compMannable != null && compMannable.ManningPawn != null)
			{
				launcher = compMannable.ManningPawn;
				equipment = this.caster;
			}
			Vector3 drawPos = this.caster.DrawPos;
			Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, this.caster.Map, WipeMode.Vanish);
			if (this.verbProps.forcedMissRadius > 0.5f)
			{
				float num = VerbUtility.CalculateAdjustedForcedMiss(this.verbProps.forcedMissRadius, this.currentTarget.Cell - this.caster.Position);
				if (num > 0.5f)
				{
					int max = GenRadial.NumCellsInRadius(num);
					int num2 = Rand.Range(0, max);
					if (num2 > 0)
					{
						IntVec3 c = this.currentTarget.Cell + GenRadial.RadialPattern[num2];
						this.ThrowDebugText("ToRadius");
						this.ThrowDebugText("Rad\nDest", c);
						ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
						if (Rand.Chance(0.5f))
						{
							projectileHitFlags = ProjectileHitFlags.All;
						}
						if (!this.canHitNonTargetPawnsNow)
						{
							projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
						}
						projectile2.Launch(launcher, drawPos, c, this.currentTarget, projectileHitFlags, equipment, null);
						if (this.CasterIsPawn)
						{
							this.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
						}
						Log.Message("TryCastShot 1");
						return true;
					}
				}
			}
			ShotReport shotReport = ShotReport.HitReportFor(this.caster, this, this.currentTarget);
			Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
			ThingDef targetCoverDef = (randomCoverToMissInto != null) ? randomCoverToMissInto.def : null;
			if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
			{
				shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
				this.ThrowDebugText("ToWild" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
				this.ThrowDebugText("Wild\nDest", shootLine.Dest);
				ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
				if (Rand.Chance(0.5f) && this.canHitNonTargetPawnsNow)
				{
					projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
				}
				projectile2.Launch(launcher, drawPos, shootLine.Dest, this.currentTarget, projectileHitFlags2, equipment, targetCoverDef);
				if (this.CasterIsPawn)
				{
					this.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
				}
				Log.Message("TryCastShot 2");
				return true;
			}
			if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance))
			{
				this.ThrowDebugText("ToCover" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
				this.ThrowDebugText("Cover\nDest", randomCoverToMissInto.Position);
				ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
				if (this.canHitNonTargetPawnsNow)
				{
					projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
				}
				projectile2.Launch(launcher, drawPos, randomCoverToMissInto, this.currentTarget, projectileHitFlags3, equipment, targetCoverDef);
				if (this.CasterIsPawn)
				{
					this.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
				}
				Log.Message("TryCastShot 3");
				return true;
			}
			ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
			if (this.canHitNonTargetPawnsNow)
			{
				projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
			}
			if (!this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full)
			{
				projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
			}
			this.ThrowDebugText("ToHit" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
			if (this.currentTarget.Thing != null)
			{
				projectile2.Launch(launcher, drawPos, this.currentTarget, this.currentTarget, projectileHitFlags4, equipment, targetCoverDef);
				this.ThrowDebugText("Hit\nDest", this.currentTarget.Cell);
			}
			else
			{
				projectile2.Launch(launcher, drawPos, shootLine.Dest, this.currentTarget, projectileHitFlags4, equipment, targetCoverDef);
				this.ThrowDebugText("Hit\nDest", shootLine.Dest);
			}

			if (this.CasterIsPawn)
			{
				this.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
			}
			Log.Message("TryCastShot 4");
			return true;
		}

		// Token: 0x060022CF RID: 8911 RVA: 0x000D40B8 File Offset: 0x000D22B8
		public override bool TryStartCastOn(LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true)
		{
			Log.Message("TryStartCastOn 0");
			if (this.caster == null)
			{
				Log.Error("Verb " + this.GetUniqueLoadID() + " needs caster to work (possibly lost during saving/loading).", false);
				return false;
			}
			Log.Message("TryStartCastOn 1 caster "+ this.caster);
			if (!this.caster.Spawned)
			{
				if (this.caster is Apparel app)
				{
					if (app.Wearer==null)
					{
						return false;
					}
					else
					{
						this.caster = app.Wearer;
					}

				}
				else
				{
					return false;
				}
			}
			Log.Message("TryStartCastOn Bursting: "+ (this.state == VerbState.Bursting) + " Can Hit tgt: "+ (this.CanHitTarget(castTarg)));
			if (this.state == VerbState.Bursting || !this.CanHitTarget(castTarg))
			{
				return false;
			}
			Log.Message("TryStartCastOn 3");
			if (this.CausesTimeSlowdown(castTarg))
			{
				Find.TickManager.slower.SignalForceNormalSpeed();
			}
			Log.Message("TryStartCastOn 4");
			this.surpriseAttack = surpriseAttack;
			this.canHitNonTargetPawnsNow = canHitNonTargetPawns;
			this.currentTarget = castTarg;
			this.currentDestination = destTarg;
			Log.Message("TryStartCastOn 5");
			/*
			if (this.CasterIsPawn && this.verbProps.warmupTime > 0f)
			{
				Log.Message("TryStartCastOn DoWarmup");
				ShootLine newShootLine;
				if (!this.TryFindShootLineFromTo(this.caster.Position, castTarg, out newShootLine))
				{
					Log.Message("TryStartCastOn No LOS");
					return false;
				}
				this.CasterPawn.Drawer.Notify_WarmingCastAlongLine(newShootLine, this.caster.Position);
				float statValue = this.CasterPawn.GetStatValue(StatDefOf.AimingDelayFactor, true);
				int ticks = (this.verbProps.warmupTime * statValue).SecondsToTicks();
				this.CasterPawn.stances.SetStance(new Stance_Warmup(ticks, castTarg, this));
			}
			else
			{
				Log.Message("TryStartCastOn WarmupComplete");
				this.WarmupComplete();
			}
			*/

			Log.Message("TryStartCastOn WarmupComplete");
			this.WarmupComplete();
			Log.Message("TryStartCastOn 6");
			return true;
		}

		public override bool CanHitTarget(LocalTargetInfo targ)
		{
			Log.Message("CanHitTarget ");
			if (this.caster is Apparel app)
			{
				Log.Message("CanHitTarget app");
				if (app.Wearer != null)
				{
					Log.Message("CanHitTarget Wearer spawned: "+ app.Wearer.Spawned+" Selftarget: " + (targ == app.Wearer)+ " CanHitFrom: "+(this.CanHitTargetFrom(app.Wearer.Position, targ)));
					return app.Wearer.Spawned && (targ == app.Wearer || this.CanHitTargetFrom(app.Wearer.Position, targ));
				}

			}
			return this.caster != null && this.caster.Spawned && (targ == this.caster || this.CanHitTargetFrom(this.caster.Position, targ));
		}

		public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
		{
			ShootLine shootLine;
			if (this.caster is Apparel app)
			{
				Log.Message("CanHitTargetFrom app");
				if (app.Wearer != null)
				{
					if (targ.Thing != null && targ.Thing == app.Wearer)
					{
						return this.targetParams.canTargetSelf;
					}
					Log.Message("CanHitTargetFrom ApparelPreventsShooting: "+(this.ApparelPreventsShooting(root, targ)) + " TryFindShootLineFromTo: " + (this.TryFindShootLineFromTo(root, targ, out shootLine)));
					return !this.ApparelPreventsShooting(root, targ) && this.TryFindShootLineFromTo(root, targ, out shootLine);
				}

			}
			if (targ.Thing != null && targ.Thing == this.caster)
			{
				return this.targetParams.canTargetSelf;
			}
			return !this.ApparelPreventsShooting(root, targ) && this.TryFindShootLineFromTo(root, targ, out shootLine);
		}

		private bool CausesTimeSlowdown(LocalTargetInfo castTarg)
		{
			if (!this.verbProps.CausesTimeSlowdown)
			{
				return false;
			}
			if (!castTarg.HasThing)
			{
				return false;
			}
			Thing thing = castTarg.Thing;
			if (thing.def.category != ThingCategory.Pawn && (thing.def.building == null || !thing.def.building.IsTurret))
			{
				return false;
			}
			Pawn pawn = thing as Pawn;
			bool flag = pawn != null && pawn.Downed;
			return (thing.Faction == Faction.OfPlayer && this.caster.HostileTo(Faction.OfPlayer)) || (this.caster.Faction == Faction.OfPlayer && thing.HostileTo(Faction.OfPlayer) && !flag);
		}
		private void ThrowDebugText(string text)
		{
			if (DebugViewSettings.drawShooting)
			{
				MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, text, -1f);
			}
		}

		// Token: 0x060022EC RID: 8940 RVA: 0x000D53A9 File Offset: 0x000D35A9
		private void ThrowDebugText(string text, IntVec3 c)
		{
			if (DebugViewSettings.drawShooting)
			{
				MoteMaker.ThrowText(c.ToVector3Shifted(), this.caster.Map, text, -1f);
			}
		}
	}
}
