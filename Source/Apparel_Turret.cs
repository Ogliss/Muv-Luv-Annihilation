using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace MuvLuvBeta
{
	public class CompProperties_Apparel_Turret : CompProperties
	{
		public CompProperties_Apparel_Turret()
		{
			this.compClass = typeof(CompApparel_TurretGun);
		}
		public bool OnByDefault = true;
		public bool DisableInMelee = true;
		public ThingDef TurretDef = null;
		public string iconPath;
		public int gizmoID = 0;
	}
	// Token: 0x02000CD3 RID: 3283
	public abstract class CompApparel_Turret : ThingComp, IAttackTargetSearcher
	{
		public CompProperties_Apparel_Turret Props => this.props as CompProperties_Apparel_Turret;
		public abstract LocalTargetInfo CurrentTarget { get; }

		public abstract Verb AttackVerb { get; }

		public LocalTargetInfo TargetCurrentlyAimingAt
		{
			get
			{
				return this.CurrentTarget;
			}
		}
		public Apparel apparel => this.parent as Apparel;
		public Pawn Wearer
		{
			get
			{
				if (apparel != null)
				{
					return apparel.Wearer;
				}
				return null;
			}
		}
		public bool isWorn => Wearer != null;
		Thing IAttackTargetSearcher.Thing
		{
			get
			{
				return Wearer;
			}
		}

		public Verb CurrentEffectiveVerb
		{
			get
			{
				return this.AttackVerb;
			}
		}

		public LocalTargetInfo LastAttackedTarget
		{
			get
			{
				return this.lastAttackedTarget;
			}
		}

		public int LastAttackTargetTick
		{
			get
			{
				return this.lastAttackTargetTick;
			}
		}

		public float TargetPriorityFactor
		{
			get
			{
				return 1f;
			}
		}

		public CompApparel_Turret()
		{
			this.stunner = new StunHandler(Wearer);
		}

		public override void CompTick()
		{
			base.CompTick();
			if (Wearer == null)
			{
				return;
			}
			if (this.forcedTarget.HasThing && (!this.forcedTarget.Thing.Spawned || !Wearer.Spawned || this.forcedTarget.Thing.Map != Wearer.Map))
			{
				this.forcedTarget = LocalTargetInfo.Invalid;
			}
			if (this.stunner!=null)
			{
				this.stunner.StunHandlerTick();
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_TargetInfo.Look(ref this.forcedTarget, "forcedTarget"+ Props.gizmoID);
			Scribe_TargetInfo.Look(ref this.lastAttackedTarget, "lastAttackedTarget" + Props.gizmoID);
			Scribe_Deep.Look<StunHandler>(ref this.stunner, "stunner" + Props.gizmoID, new object[]
			{
				this
			});
			Scribe_Values.Look<int>(ref this.lastAttackTargetTick, "lastAttackTargetTick" + Props.gizmoID, 0, false);
		}

		public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			base.PostPostApplyDamage(dinfo, totalDamageDealt);
		}
		/*
		public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			base.PostPreApplyDamage(dinfo, out absorbed);
			if (absorbed)
			{
				return;
			}
			this.stunner.Notify_DamageApplied(dinfo, true);
			absorbed = false;
		}
		*/
		public abstract void OrderAttack(LocalTargetInfo targ);

		/*
		public bool ThreatDisabled(IAttackTargetSearcher disabledFor)
		{
			return comp4 != null && !comp4.Initiated;
		}
		*/

		protected void OnAttackedTarget(LocalTargetInfo target)
		{
			this.lastAttackTargetTick = Find.TickManager.TicksGame;
			this.lastAttackedTarget = target;
		}

		protected StunHandler stunner;
		protected LocalTargetInfo forcedTarget = LocalTargetInfo.Invalid;
		private LocalTargetInfo lastAttackedTarget;
		private int lastAttackTargetTick;
		private const float SightRadiusTurret = 13.4f;
	}
}
