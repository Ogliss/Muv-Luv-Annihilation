using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace MuvLuvBeta
{
	public class CompProperties_Apparel_Turret : CompProperties
	{
		public CompProperties_Apparel_Turret()
		{
			this.compClass = typeof(CompApparel_TurretGun);
		}

		public NamedArgument ChargeNounArgument
		{
			get
			{
				return this.chargeNoun.Named("CHARGENOUN");
			}
		}


		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (string text in base.ConfigErrors(parentDef))
			{
				yield return text;
			}
			IEnumerator<string> enumerator = null;
			if (this.ammoDef != null && this.ammoCountToRefill == 0 && this.ammoCountPerCharge == 0)
			{
				yield return "TurretGun component has ammoDef but one of ammoCountToRefill or ammoCountPerCharge must be set";
			}
			if (this.ammoCountToRefill != 0 && this.ammoCountPerCharge != 0)
			{
				yield return "TurretGun component: specify only one of ammoCountToRefill and ammoCountPerCharge";
			}
			yield break;
			yield break;
		}

		// Token: 0x0600564E RID: 22094 RVA: 0x001CE077 File Offset: 0x001CC277
		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry statDrawEntry in base.SpecialDisplayStats(req))
			{
				yield return statDrawEntry;
			}
			IEnumerator<StatDrawEntry> enumerator = null;
			if (!req.HasThing)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Apparel, "Stat_Thing_ReloadMaxCharges_Name".Translate(this.ChargeNounArgument), this.maxCharges.ToString(), "Stat_Thing_ReloadMaxCharges_Desc".Translate(this.ChargeNounArgument), 2749, null, null, false);
			}
			if (this.ammoDef != null)
			{
				if (this.ammoCountToRefill != 0)
				{
					yield return new StatDrawEntry(StatCategoryDefOf.Apparel, "Stat_Thing_ReloadRefill_Name".Translate(this.ChargeNounArgument), string.Format("{0} {1}", this.ammoCountToRefill, this.ammoDef.label), "Stat_Thing_ReloadRefill_Desc".Translate(this.ChargeNounArgument), 2749, null, null, false);
				}
				else
				{
					yield return new StatDrawEntry(StatCategoryDefOf.Apparel, "Stat_Thing_ReloadPerCharge_Name".Translate(this.ChargeNounArgument), string.Format("{0} {1}", this.ammoCountPerCharge, this.ammoDef.label), "Stat_Thing_ReloadPerCharge_Desc".Translate(this.ChargeNounArgument), 2749, null, null, false);
				}
			}
			if (this.destroyOnEmpty)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Apparel, "Stat_Thing_ReloadDestroyOnEmpty_Name".Translate(this.ChargeNounArgument), "Yes".Translate(), "Stat_Thing_ReloadDestroyOnEmpty_Desc".Translate(this.ChargeNounArgument), 2749, null, null, false);
			}
			yield break;
			yield break;
		}

		public float projectileOffset = 0f;
		public int maxCharges = 1;

		public ThingDef ammoDef;

		public int ammoCountToRefill;

		public float ammoCountPerCharge = 1f;

		public bool destroyOnEmpty;

		public int baseReloadTicks = 60;

		public bool displayGizmoWhileUndrafted = true;

		public bool displayGizmoWhileDrafted = true;

		public KeyBindingDef hotKey;

		public SoundDef soundReload;
		public SoundDef soundEmptyWarning;
		public string messageEmptyWarning;
		public SoundDef soundHalfRemaningWarning;
		public string messageHalfRemaningWarning;
		public SoundDef soundQuaterRemaningWarning;
		public string messageQuaterRemaningWarning;

		public bool OnByDefault = true;
		public bool DisableInMelee = true;
		public ThingDef TurretDef = null;
		public string iconPath;
		public int gizmoID = 0;

		[MustTranslate]
		public string chargeNoun = "charge";
	}
	// Token: 0x02000CD3 RID: 3283
	public abstract class CompApparel_Turret : ThingComp, IAttackTargetSearcher, IThingHolder
	{
		public CompProperties_Apparel_Turret Props => this.props as CompProperties_Apparel_Turret;
		public bool UseAmmo => Props.ammoDef != null;
		public ThingDef AmmoDef => Props.ammoDef;
		public int MaxCharges => Props.maxCharges;
		public float ChargesPerUnit => 1 / Props.ammoCountPerCharge; 

		public string LabelRemaining
		{
			get
			{
				return string.Format("{0} / {1}", this.RemainingCharges, this.MaxCharges);
			}
		}

		public int RemainingCharges
		{
			get
			{
				return this.remainingCharges;
			}
		}


		// Token: 0x17000F31 RID: 3889
		// (get) Token: 0x06005655 RID: 22101 RVA: 0x001CE0BD File Offset: 0x001CC2BD
		public bool CanBeUsed
		{
			get
			{
				return this.remainingCharges > 0;
			}
		}
		// Token: 0x17000F2F RID: 3887
		// (get) Token: 0x06005653 RID: 22099 RVA: 0x001CE0A3 File Offset: 0x001CC2A3

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
			this.innerContainer = new ThingOwner<Thing>(this);
		}

		public override void CompTick()
		{
			base.CompTick();
			if (Wearer == null)
			{
				return;
			}
			if (UseAmmo && RemainingCharges == 0)
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

		// Token: 0x06005664 RID: 22116 RVA: 0x001CE200 File Offset: 0x001CC400
		public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
			{
				yield return gizmo;
			}
			IEnumerator<Gizmo> enumerator = null;
			bool drafted = this.Wearer.Drafted;
			if ((drafted && !this.Props.displayGizmoWhileDrafted) || (!drafted && !this.Props.displayGizmoWhileUndrafted))
			{
				yield break;
			}
			ThingWithComps gear = this.parent;
			/*
			foreach (Verb verb in this.VerbTracker.AllVerbs)
			{
				if (verb.verbProps.hasStandardCommand)
				{
					yield return this.CreateVerbTargetCommand(gear, verb);
				}
			}
			*/
			List<Verb>.Enumerator enumerator2 = default(List<Verb>.Enumerator);
			if (Prefs.DevMode)
			{
				yield return new Command_Action
				{
					defaultLabel = "Debug: Reload to full",
					action = delegate ()
					{
						this.remainingCharges = this.MaxCharges;
					}
				};
			}
			yield break;
		}
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (UseAmmo)
			{
				if (RemainingCharges < MaxCharges)
				{

				}
			}
			foreach (var item in base.CompFloatMenuOptions(selPawn))
			{
				yield return item;
			}
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


		// Token: 0x0600529A RID: 21146 RVA: 0x001BDEC4 File Offset: 0x001BC0C4
		public ThingOwner GetDirectlyHeldThings()
		{
			return this.innerContainer;
		}

		// Token: 0x0600529B RID: 21147 RVA: 0x001BDECC File Offset: 0x001BC0CC
		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		// Token: 0x06005665 RID: 22117 RVA: 0x001CE210 File Offset: 0x001CC410
		public Command_TSFReloadable CreateVerbTargetCommand(Thing gear, Verb verb)
		{
			Command_TSFReloadable command_Reloadable = new Command_TSFReloadable(this);
			command_Reloadable.defaultDesc = gear.def.description;
			command_Reloadable.hotKey = this.Props.hotKey;
			command_Reloadable.defaultLabel = verb.verbProps.label;
			command_Reloadable.verb = verb;
			if (verb.verbProps.defaultProjectile != null && verb.verbProps.commandIcon == null)
			{
				command_Reloadable.icon = verb.verbProps.defaultProjectile.uiIcon;
				command_Reloadable.iconAngle = verb.verbProps.defaultProjectile.uiIconAngle;
				command_Reloadable.iconOffset = verb.verbProps.defaultProjectile.uiIconOffset;
				command_Reloadable.overrideColor = new Color?(verb.verbProps.defaultProjectile.graphicData.color);
			}
			else
			{
				command_Reloadable.icon = ((verb.UIIcon != BaseContent.BadTex) ? verb.UIIcon : gear.def.uiIcon);
				command_Reloadable.iconAngle = gear.def.uiIconAngle;
				command_Reloadable.iconOffset = gear.def.uiIconOffset;
				command_Reloadable.defaultIconColor = gear.DrawColor;
			}
			if (!this.Wearer.IsColonistPlayerControlled)
			{
				command_Reloadable.Disable(null);
			}
			else if (verb.verbProps.violent && this.Wearer.WorkTagIsDisabled(WorkTags.Violent))
			{
				command_Reloadable.Disable("IsIncapableOfViolenceLower".Translate(this.Wearer.LabelShort, this.Wearer).CapitalizeFirst() + ".");
			}
			else if (!this.CanBeUsed)
			{
				command_Reloadable.Disable(this.DisabledReason(this.MinAmmoNeeded(false), this.MaxAmmoNeeded(false)));
			}
			return command_Reloadable;
		}

		// Token: 0x06005666 RID: 22118 RVA: 0x001CE3D0 File Offset: 0x001CC5D0
		public string DisabledReason(int minNeeded, int maxNeeded)
		{
			string result;
			if (this.AmmoDef == null)
			{
				result = "CommandReload_NoCharges".Translate(this.Props.ChargeNounArgument);
			}
			else
			{
				string arg;
				if (this.Props.ammoCountToRefill != 0)
				{
					arg = this.Props.ammoCountToRefill.ToString();
				}
				else
				{
					arg = ((minNeeded == maxNeeded) ? minNeeded.ToString() : string.Format("{0}-{1}", minNeeded, maxNeeded));
				}
				result = "CommandReload_NoAmmo".Translate(this.Props.ChargeNounArgument, this.AmmoDef.Named("AMMO"), arg.Named("COUNT"));
			}
			return result;
		}

		// Token: 0x06005667 RID: 22119 RVA: 0x001CE480 File Offset: 0x001CC680
		public bool NeedsReload(bool allowForcedReload)
		{
			if (this.AmmoDef == null)
			{
				return false;
			}
			if (this.Props.ammoCountToRefill == 0)
			{
				return this.RemainingCharges != this.MaxCharges;
			}
			if (!allowForcedReload)
			{
				return this.remainingCharges == 0;
			}
			return this.RemainingCharges != this.MaxCharges;
		}

		// Token: 0x06005668 RID: 22120 RVA: 0x001CE4D4 File Offset: 0x001CC6D4
		public void ReloadFrom(Thing ammo)
		{
			if (!this.NeedsReload(true))
			{
				return;
			}
			if (this.Props.ammoCountToRefill != 0)
			{
				if (ammo.stackCount < this.Props.ammoCountToRefill)
				{
					return;
				}
				ammo.SplitOff(this.Props.ammoCountToRefill).Destroy(DestroyMode.Vanish);
				this.remainingCharges = this.MaxCharges;
			}
			else
			{
				if (ammo.stackCount < this.Props.ammoCountPerCharge && this.Props.ammoCountPerCharge > 1f)
				{
					return;
				}
				int num = (int)(Mathf.Clamp(MaxAmmoNeeded(true), 1, ammo.stackCount));
			//	Log.Message("Ammo Needed: " + num + " RoundsPerUnit: " + ChargesPerUnit + " total rounds: " + num * ChargesPerUnit);
				ammo.SplitOff(num).Destroy(DestroyMode.Vanish);
				this.remainingCharges += (int)(num * ChargesPerUnit);
			}
			if (this.Props.soundReload != null)
			{
				this.Props.soundReload.PlayOneShot(new TargetInfo(this.Wearer.Position, this.Wearer.Map, false));
			}
		}

		// Token: 0x06005669 RID: 22121 RVA: 0x001CE5D0 File Offset: 0x001CC7D0
		public int MinAmmoNeeded(bool allowForcedReload)
		{
			if (!this.NeedsReload(allowForcedReload))
			{
				return 0;
			}
			if (this.Props.ammoCountToRefill != 0)
			{
				return this.Props.ammoCountToRefill;
			}
			if (this.Props.ammoCountPerCharge<1f)
			{
				return 1;
			}
			return (int)this.Props.ammoCountPerCharge;
		}

		// Token: 0x0600566A RID: 22122 RVA: 0x001CE601 File Offset: 0x001CC801
		public int MaxAmmoNeeded(bool allowForcedReload)
		{
			if (!this.NeedsReload(allowForcedReload))
			{
				return 0;
			}
			if (this.Props.ammoCountToRefill != 0)
			{
				return this.Props.ammoCountToRefill;
			}
			return (int)(this.Props.ammoCountPerCharge * (this.MaxCharges - this.RemainingCharges));
		}

		// Token: 0x0600566B RID: 22123 RVA: 0x001CE640 File Offset: 0x001CC840
		public int MaxAmmoAmount()
		{
			if (this.AmmoDef == null)
			{
				return 0;
			}
			if (this.Props.ammoCountToRefill == 0)
			{
				return (int)(this.Props.ammoCountPerCharge * this.MaxCharges);
			}
			return this.Props.ammoCountToRefill;
		}

		// Token: 0x0600566C RID: 22124 RVA: 0x001CE677 File Offset: 0x001CC877
		public void UsedOnce()
		{
			if (this.remainingCharges > 0)
			{
				this.remainingCharges--;
			}
			if (this.Props.destroyOnEmpty && this.remainingCharges == 0)
			{
				this.parent.Destroy(DestroyMode.Vanish);
			}
		}

		// Token: 0x04003005 RID: 12293
		public int remainingCharges;

		// Token: 0x04003006 RID: 12294
		private VerbTracker verbTracker;
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_TargetInfo.Look(ref this.forcedTarget, "forcedTarget" + Props.gizmoID);
			Scribe_TargetInfo.Look(ref this.lastAttackedTarget, "lastAttackedTarget" + Props.gizmoID);
			Scribe_Deep.Look<StunHandler>(ref this.stunner, "stunner" + Props.gizmoID, new object[]
			{
				this
			});
			Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer" + Props.gizmoID, new object[]
			{
				this
			});
			Scribe_Values.Look<int>(ref this.lastAttackTargetTick, "lastAttackTargetTick" + Props.gizmoID, 0, false);
			Scribe_Values.Look<int>(ref this.remainingCharges, "remainingCharges" + Props.gizmoID, -999, false);
			Scribe_Deep.Look<VerbTracker>(ref this.verbTracker, "verbTracker" + Props.gizmoID, new object[]
			{
				this
			});
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.remainingCharges == -999)
			{
				this.remainingCharges = this.MaxCharges;
			}
		}
		public ThingOwner innerContainer;
		protected StunHandler stunner;
		protected LocalTargetInfo forcedTarget = LocalTargetInfo.Invalid;
		private LocalTargetInfo lastAttackedTarget;
		private int lastAttackTargetTick;
		private const float SightRadiusTurret = 13.4f;
	}
}
