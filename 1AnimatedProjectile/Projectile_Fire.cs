using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimatedProjectile
{
	// Token: 0x02000004 RID: 4
	public class Projectile_Fire : Projectile_Anim
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x000020E5 File Offset: 0x000002E5
		public bool SetsFires
		{
			get
			{
				return base.Props != null && base.Props.setsFire;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000020FD File Offset: 0x000002FD
		public bool Ignites
		{
			get
			{
				return base.Props != null && base.Props.ignites;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002115 File Offset: 0x00000315
		protected virtual void Impact(Thing hitThing)
		{
			this.Ignite();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002120 File Offset: 0x00000320
		public override void Tick()
		{
			base.Tick();
			this.pos = base.Position;
			bool flag2;
			checked
			{
				this.TicksforAppearence--;
				bool flag = this.TicksforAppearence == 0 && base.Map != null;
				flag2 = flag;
			}
			if (flag2)
			{
				bool flag3 = this.pos != IntVec3.Invalid;
				if (flag3)
				{
					bool flag4 = this.pos != base.Position && this.SetsFires;
					if (flag4)
					{
						Rand.PushState();
						bool flag5 = Rand.Chance(base.Props.setFireChance * base.Traveled);
						if (flag5)
						{
							this.TrySpread();
						}
						Rand.PopState();
					}
				}
				Rand.PushState();
				bool flag6 = Rand.Chance(0.75f * base.Traveled);
				if (flag6)
				{
					Projectile_Fire.ThrowSmoke(this.DrawPos, base.Map, 0.5f * base.Traveled);
					bool flag7 = base.Traveled > 0.5f && this.Ignites;
					if (flag7)
					{
						bool flag8 = Rand.Chance(base.Props.igniteChance * base.Traveled);
						if (flag8)
						{
							this.Ignite();
						}
					}
				}
				Rand.PopState();
				this.TicksforAppearence = 6;
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002268 File Offset: 0x00000468
		protected virtual void Ignite()
		{
			Map map = base.Map;
			this.Destroy(DestroyMode.Vanish);
			float explosionChanceToStartFire = this.def.projectile.explosionChanceToStartFire;
			float explosionRadius = this.def.projectile.explosionRadius;
			List<IntVec3> list = SimplePool<List<IntVec3>>.Get();
			list.Clear();
			list.AddRange(this.def.projectile.damageDef.Worker.ExplosionCellsToHit(base.Position, map, explosionRadius, null, null));
			FleckMaker.Static(base.Position, map, FleckDefOf.ExplosionFlash, explosionRadius * 4f);
			for (int i = 0; i < 4; i++)
			{
                FleckMaker.ThrowSmoke(base.Position.ToVector3Shifted() + Gen.RandomHorizontalVector(explosionRadius * 0.7f), map, explosionRadius * 0.6f);
			}
			Rand.PushState();
			bool flag = Rand.Chance(explosionChanceToStartFire);
			if (flag)
			{
				foreach (IntVec3 intVec in list)
				{
					float num = explosionRadius - intVec.DistanceTo(base.Position);
					bool flag2 = num > 0.1f;
					if (flag2)
					{
						FireUtility.TryStartFireIn(intVec, map, num);
					}
				}
			}
			Rand.PopState();
			bool flag3 = this.def.projectile.explosionEffect != null;
			if (flag3)
			{
				Effecter effecter = this.def.projectile.explosionEffect.Spawn();
				effecter.Trigger(new TargetInfo(base.Position, map, false), new TargetInfo(base.Position, map, false));
				effecter.Cleanup();
			}
			GenExplosion.DoExplosion(base.Position, map, this.def.projectile.explosionRadius, this.def.projectile.damageDef, this.launcher, this.def.projectile.GetDamageAmount(1f, null), this.def.projectile.GetArmorPenetration(1f, null), this.def.projectile.soundExplode, this.equipmentDef, this.def, null, this.def.projectile.postExplosionSpawnThingDef, this.def.projectile.postExplosionSpawnChance, this.def.projectile.postExplosionSpawnThingCount, this.def.projectile.postExplosionGasType, this.def.projectile.applyDamageToExplosionCellsNeighbors, this.def.projectile.preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff, null, null);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002548 File Offset: 0x00000748
		protected void TrySpread()
		{
			IntVec3 intVec = base.Position;
			Rand.PushState();
			bool flag = Rand.Chance(0.8f);
			bool flag2;
			if (flag)
			{
				intVec = base.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(1, 8)];
				flag2 = true;
			}
			else
			{
				intVec = base.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(10, 20)];
				flag2 = false;
			}
			Rand.PopState();
			bool flag3 = !intVec.InBounds(base.Map);
			if (!flag3)
			{
				Rand.PushState();
				bool flag4 = Rand.Chance(FireUtility.ChanceToStartFireIn(intVec, base.Map));
				if (flag4)
				{
					bool flag5 = !flag2;
					if (flag5)
					{
						CellRect cellRect = CellRect.SingleCell(base.Position);
						CellRect cellRect2 = CellRect.SingleCell(intVec);
						bool flag6 = !GenSight.LineOfSight(base.Position, intVec, base.Map, cellRect, cellRect2, null);
						if (flag6)
						{
							return;
						}
						Spark spark = (Spark)GenSpawn.Spawn(ThingDefOf.Spark, base.Position, base.Map, WipeMode.Vanish);
						spark.Launch(this, intVec, intVec, ProjectileHitFlags.All);
					}
					else
					{
						FireUtility.TryStartFireIn(intVec, base.Map, 0.1f);
					}
				}
				Rand.PopState();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002690 File Offset: 0x00000890
		public override Quaternion ExactRotation
		{
			get
			{
				Vector3 vector = this.destination - this.origin;
				vector.y = 0f;
				return Quaternion.LookRotation(vector);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000026C8 File Offset: 0x000008C8
		public static void ThrowSmoke(Vector3 loc, Map map, float size)
		{
			bool flag = !GenView.ShouldSpawnMotesAt(loc, map) || map.moteCounter.SaturatedLowPriority;
			if (!flag)
			{
				MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Mote_Smoke", true), null);
				Rand.PushState();
				moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
				moteThrown.rotationRate = Rand.Range(-30f, 30f);
				moteThrown.exactPosition = loc;
				moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
				Rand.PopState();
				GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002780 File Offset: 0x00000980
		public override void Draw()
		{
			bool drawGlow = base.Props.drawGlow;
			if (drawGlow)
			{
				string text = ((!base.Props.drawGlowMote.NullOrEmpty()) ? base.Props.drawGlowMote : "Mote_FireGlow");
				Mesh mesh = MeshPool.GridPlane(DefDatabase<ThingDef>.GetNamed(text, true).graphicData.drawSize * (base.Drawsize * base.Props.drawGlowSizeFactor));
				Graphics.DrawMesh(mesh, this.DrawPos, this.ExactRotation, DefDatabase<ThingDef>.GetNamed(text, true).graphic.MatSingle, 0);
			}
			base.Draw();
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000281E File Offset: 0x00000A1E
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000282A File Offset: 0x00000A2A
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.TicksforAppearence, "TicksforAppearence", 0, false);
		}

		// Token: 0x0400000E RID: 14
		private int TicksforAppearence = 15;

		// Token: 0x0400000F RID: 15
		private IntVec3 pos = IntVec3.Invalid;
	}
}
