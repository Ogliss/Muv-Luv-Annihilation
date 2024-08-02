using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation
{
	// Token: 0x02000D23 RID: 3363
	public class PawnThrown : Thing, IThingHolder
	{
		// Token: 0x17000E52 RID: 3666
		// (get) Token: 0x060051DA RID: 20954 RVA: 0x001B94C9 File Offset: 0x001B76C9
		public Pawn FlyingPawn
		{
			get
			{
				if (this.innerContainer.InnerListForReading.Count <= 0)
				{
					return null;
				}
				return this.innerContainer.InnerListForReading[0] as Pawn;
			}
		}

		// Token: 0x060051DB RID: 20955 RVA: 0x001B94F6 File Offset: 0x001B76F6
		public ThingOwner GetDirectlyHeldThings()
		{
			return this.innerContainer;
		}

		// Token: 0x060051DC RID: 20956 RVA: 0x001B94FE File Offset: 0x001B76FE
		public PawnThrown()
		{
			this.innerContainer = new ThingOwner<Thing>(this);
		}

		// Token: 0x060051DD RID: 20957 RVA: 0x001B951A File Offset: 0x001B771A
		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		// Token: 0x17000E53 RID: 3667
		// (get) Token: 0x060051DE RID: 20958 RVA: 0x001B9528 File Offset: 0x001B7728
		public Vector3 DestinationPos
		{
			get
			{
				Pawn flyingPawn = this.FlyingPawn;
				return GenThing.TrueCenter(base.Position, flyingPawn.Rotation, flyingPawn.def.size, flyingPawn.def.Altitude);
			}
		}

		// Token: 0x060051DF RID: 20959 RVA: 0x001B9564 File Offset: 0x001B7764
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				float num = Mathf.Max(this.throwDistance, 1f) / this.def.pawnFlyer.flightSpeed;
				num = Mathf.Max(num, this.def.pawnFlyer.flightDurationMin);
				this.ticksThrownTime = num.SecondsToTicks();
				this.ticksThrown = 0;
			}
		}

		// Token: 0x060051E0 RID: 20960 RVA: 0x001B95C8 File Offset: 0x001B77C8
		protected virtual void RespawnPawn()
		{
			Pawn thrownPawn = this.FlyingPawn;
			Thing thing;
			this.innerContainer.TryDrop(thrownPawn, base.Position, thrownPawn.MapHeld, ThingPlaceMode.Direct, out thing, null, null, false);
			if (thrownPawn.drafter != null)
			{
				thrownPawn.drafter.Drafted = this.pawnWasDrafted;
			}
			if (this.pawnWasSelected && Find.CurrentMap == thrownPawn.Map)
			{
				Find.Selector.Select(thrownPawn, false, true);
			}
			if (this.jobQueue != null)
			{
				thrownPawn.jobs.RestoreCapturedJobs(this.jobQueue, true);
			}
		}

		// Token: 0x060051E1 RID: 20961 RVA: 0x001B9650 File Offset: 0x001B7850
		public override void Tick()
		{
			if (this.ticksThrown >= this.ticksThrownTime)
			{
				this.ImpactSomething();
			//	this.Destroy(DestroyMode.Vanish);
			}
			else
			{
				if (this.ticksThrown % 5 == 0)
				{
					this.CheckDestination();
				}
				this.innerContainer.ThingOwnerTick(true);
			}
			this.ticksThrown++;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004308 File Offset: 0x00002508
		private void ImpactSomething()
		{
			bool flag = this.usedTarget != null;
			if (flag)
			{
				Pawn pawn = this.usedTarget as Pawn;
				Rand.PushState();
				bool flag2 = pawn != null /* && PawnUtility.GetPosture(pawn) != null */ && GenGeo.MagnitudeHorizontalSquared(this.startVec - this.DestinationPos) >= 20.25f && Rand.Value > 0.2f;
				Rand.PopState();
				if (flag2)
				{
					this.Impact(null);
				}
				else
				{
					this.Impact(this.usedTarget);
				}
			}
			else
			{
				this.Impact(null);
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004390 File Offset: 0x00002590
		protected virtual void Impact(Thing hitThing)
		{
			if (hitThing == null)
			{
				Pawn pawn;
				if ((pawn = (GridsUtility.GetThingList(base.Position, base.Map).FirstOrDefault((Thing x) => x == this.usedTarget) as Pawn)) != null)
				{
					hitThing = pawn;
				}
			}
			if (this.impactDamage != null)
			{
				if (this.damageLaunched)
				{
					this.FlyingPawn.TakeDamage(this.impactDamage.Value);
				}
				else
				{
					hitThing.TakeDamage(this.impactDamage.Value);
				}
			}
			this.RespawnPawn();
			if (this.def.projectile.explosionRadius > 0)
			{
				GenExplosion.DoExplosion(base.Position, base.Map, this.def.projectile.explosionRadius, this.def.projectile.damageDef, FlyingPawn);
			}
			this.Destroy(0);
		}

		// Token: 0x060051E2 RID: 20962 RVA: 0x001B96A4 File Offset: 0x001B78A4
		private void CheckDestination()
		{
			if (!Verb_Jump.ValidJumpTarget(base.Map, base.Position))
			{
				int num = GenRadial.NumCellsInRadius(3.9f);
				for (int i = 0; i < num; i++)
				{
					IntVec3 intVec = base.Position + GenRadial.RadialPattern[i];
					if (Verb_Jump.ValidJumpTarget(base.Map, intVec))
					{
						base.Position = intVec;
						return;
					}
				}
			}
		}

		// Token: 0x060051E3 RID: 20963 RVA: 0x001B9708 File Offset: 0x001B7908
		public static PawnThrown MakeThrown(ThingDef thrownDef, Pawn pawn, IntVec3 destCell, DamageInfo? newDamageInfo = null)
		{
			PawnThrown pawnFlyer = (PawnThrown)ThingMaker.MakeThing(thrownDef, null);
			if (!pawnFlyer.ValidateFlyer())
			{
				return null;
			}
			pawnFlyer.impactDamage = newDamageInfo;
			pawnFlyer.startVec = pawn.TrueCenter();
			pawnFlyer.throwDistance = pawn.Position.DistanceTo(destCell);
			pawnFlyer.pawnWasDrafted = pawn.Drafted;
			pawnFlyer.pawnWasSelected = Find.Selector.IsSelected(pawn);
			if (pawnFlyer.pawnWasDrafted)
			{
				Find.Selector.Deselect(pawn);
			}
			pawnFlyer.jobQueue = pawn.jobs.CaptureAndClearJobQueue();
			pawn.DeSpawn(DestroyMode.Vanish);
			if (!pawnFlyer.innerContainer.TryAdd(pawn, true))
			{
				Log.Error("Could not add " + pawn.ToStringSafe<Pawn>() + " to a thrower.");
				pawn.Destroy(DestroyMode.Vanish);
			}
			return pawnFlyer;
		}

		// Token: 0x060051E4 RID: 20964 RVA: 0x00010439 File Offset: 0x0000E639
		protected virtual bool ValidateFlyer()
		{
			return true;
		}

		// Token: 0x060051E5 RID: 20965 RVA: 0x001B97C4 File Offset: 0x001B79C4
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look<ThingOwner<Thing>>(ref this.innerContainer, "innerContainer", new object[]
			{
				this
			});
			Scribe_Values.Look<Vector3>(ref this.startVec, "startVec", default(Vector3), false);
			Scribe_Values.Look<float>(ref this.throwDistance, "throwDistance", 0f, false);
			Scribe_Values.Look<bool>(ref this.pawnWasDrafted, "pawnWasDrafted", false, false);
			Scribe_Values.Look<bool>(ref this.pawnWasSelected, "pawnWasSelected", false, false);
			Scribe_Values.Look<int>(ref this.ticksThrownTime, "ticksThrownTime", 0, false);
			Scribe_Values.Look<int>(ref this.ticksThrown, "ticksThrown", 0, false);
			Scribe_Deep.Look<JobQueue>(ref this.jobQueue, "jobQueue", Array.Empty<object>());
			Scribe_References.Look<Thing>(ref this.usedTarget, "usedTarget", false);
		}

		public DamageInfo? impactDamage;
		protected Thing usedTarget;
		public bool damageLaunched = true;
		// Token: 0x04002E31 RID: 11825
		private ThingOwner<Thing> innerContainer;

		// Token: 0x04002E32 RID: 11826
		protected Vector3 startVec;

		// Token: 0x04002E33 RID: 11827
		private float throwDistance;

		// Token: 0x04002E34 RID: 11828
		private bool pawnWasDrafted;

		// Token: 0x04002E35 RID: 11829
		private bool pawnWasSelected;

		// Token: 0x04002E36 RID: 11830
		protected int ticksThrownTime = 120;

		// Token: 0x04002E37 RID: 11831
		protected int ticksThrown;

		// Token: 0x04002E38 RID: 11832
		private JobQueue jobQueue;
	}
}
