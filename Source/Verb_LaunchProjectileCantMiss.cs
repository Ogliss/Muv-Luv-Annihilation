using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace MuvLuvAnnihilation
{
	// Token: 0x02000485 RID: 1157
	public class Verb_LaunchProjectileCantMiss : Verb
	{
		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06002250 RID: 8784 RVA: 0x000D11E4 File Offset: 0x000CF3E4
		public virtual ThingDef Projectile
		{
			get
			{
				if (base.EquipmentSource != null)
				{
					CompChangeableProjectile comp = base.EquipmentSource.GetComp<CompChangeableProjectile>();
					if (comp != null && comp.Loaded)
					{
						return comp.Projectile;
					}
				}
				return this.verbProps.defaultProjectile;
			}
		}

		// Token: 0x06002251 RID: 8785 RVA: 0x000D1224 File Offset: 0x000CF424
		public override void WarmupComplete()
		{
			base.WarmupComplete();
			Find.BattleLog.Add(new BattleLogEntry_RangedFire(this.caster, this.currentTarget.HasThing ? this.currentTarget.Thing : null, (base.EquipmentSource != null) ? base.EquipmentSource.def : null, this.Projectile, this.ShotsPerBurst > 1));
		}

		// Token: 0x06002252 RID: 8786 RVA: 0x000D128C File Offset: 0x000CF48C
		protected override bool TryCastShot()
		{
			if (this.currentTarget.HasThing && this.currentTarget.Thing.Map != this.caster.Map)
			{
				return false;
			}
			ThingDef projectile = this.Projectile;
			if (projectile == null)
			{
				return false;
			}
			ShootLine shootLine;
			bool flag = base.TryFindShootLineFromTo(this.caster.Position, this.currentTarget, out shootLine);
			if (this.verbProps.stopBurstWithoutLos && !flag)
			{
				return false;
			}
			if (base.EquipmentSource != null)
			{
				CompChangeableProjectile comp = base.EquipmentSource.GetComp<CompChangeableProjectile>();
				if (comp != null)
				{
					comp.Notify_ProjectileLaunched();
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
			ShotReport shotReport = ShotReport.HitReportFor(this.caster, this, this.currentTarget);
			Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
			ThingDef targetCoverDef = (randomCoverToMissInto != null) ? randomCoverToMissInto.def : null;
			ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
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
			return true;
		}

		// Token: 0x06002253 RID: 8787 RVA: 0x000D1693 File Offset: 0x000CF893
		private void ThrowDebugText(string text)
		{
			if (DebugViewSettings.drawShooting)
			{
				MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, text, -1f);
			}
		}

		// Token: 0x06002254 RID: 8788 RVA: 0x000D16BD File Offset: 0x000CF8BD
		private void ThrowDebugText(string text, IntVec3 c)
		{
			if (DebugViewSettings.drawShooting)
			{
				MoteMaker.ThrowText(c.ToVector3Shifted(), this.caster.Map, text, -1f);
			}
		}

		// Token: 0x06002255 RID: 8789 RVA: 0x000D16E4 File Offset: 0x000CF8E4
		public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
		{
			needLOSToCenter = true;
			ThingDef projectile = this.Projectile;
			if (projectile == null)
			{
				return 0f;
			}
			return projectile.projectile.explosionRadius;
		}

		// Token: 0x06002256 RID: 8790 RVA: 0x000D1710 File Offset: 0x000CF910
		public override bool Available()
		{
			if (!base.Available())
			{
				return false;
			}
			if (this.CasterIsPawn)
			{
				Pawn casterPawn = this.CasterPawn;
				if (casterPawn.Faction != Faction.OfPlayer && casterPawn.mindState.MeleeThreatStillThreat && casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
				{
					return false;
				}
			}
			return this.Projectile != null;
		}
	}
}
