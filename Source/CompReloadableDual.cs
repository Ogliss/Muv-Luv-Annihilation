using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MuvLuvAnnihilation
{
	// MuvLuvBeta.CompProperties_ReloadableDual
	public class CompProperties_ReloadableDual : CompProperties_ApparelReloadable
	{
		// Token: 0x17000F2C RID: 3884
		// (get) Token: 0x0600564B RID: 22091 RVA: 0x001CE00E File Offset: 0x001CC20E
		public NamedArgument ChargeNounArgumentSecondry
		{
			get
			{
				return this.chargeNounSecondry.Named("CHARGENOUN");
			}
		}

		// Token: 0x0600564C RID: 22092 RVA: 0x001CE020 File Offset: 0x001CC220
		public CompProperties_ReloadableDual()
		{
			this.compClass = typeof(CompReloadableDual);
		}

		// Token: 0x0600564D RID: 22093 RVA: 0x001CE060 File Offset: 0x001CC260
		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (string text in base.ConfigErrors(parentDef))
			{
				yield return text;
			}
			IEnumerator<string> enumerator = null;
			if (this.ammoDefSecondry != null && this.ammoCountToRefillSecondry == 0 && this.ammoCountPerChargeSecondry == 0)
			{
				yield return "Reloadable component has ammoDef but one of ammoCountToRefill or ammoCountPerCharge must be set";
			}
			if (this.ammoCountToRefillSecondry != 0 && this.ammoCountPerChargeSecondry != 0)
			{
				yield return "Reloadable component: specify only one of ammoCountToRefill and ammoCountPerCharge";
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

		// Token: 0x04002FFA RID: 12282
		public int maxChargesSecondry = 1;

		// Token: 0x04002FFB RID: 12283
		public ThingDef ammoDefSecondry;

		// Token: 0x04002FFC RID: 12284
		public int ammoCountToRefillSecondry;

		// Token: 0x04002FFD RID: 12285
		public int ammoCountPerChargeSecondry;

		// Token: 0x04002FFE RID: 12286
		public bool destroyOnEmptySecondry;

		// Token: 0x04002FFF RID: 12287
		public int baseReloadTicksSecondry = 60;

		// Token: 0x04003000 RID: 12288
		public bool displayGizmoWhileUndraftedSecondry = true;

		// Token: 0x04003001 RID: 12289
		public bool displayGizmoWhileDraftedSecondry = true;

		// Token: 0x04003002 RID: 12290
		public KeyBindingDef hotKeySecondry;

		// Token: 0x04003003 RID: 12291
		public SoundDef soundReloadSecondry;

		// Token: 0x04003004 RID: 12292
		[MustTranslate]
		public string chargeNounSecondry = "charge";
	}
	// Token: 0x02000DC7 RID: 3527
	public class CompReloadableDual : CompApparelReloadable
	{
		// Token: 0x17000F2D RID: 3885
		// (get) Token: 0x06005651 RID: 22097 RVA: 0x001CE08E File Offset: 0x001CC28E
		public new CompProperties_ReloadableDual Props
		{
			get
			{
				return this.props as CompProperties_ReloadableDual;
			}
		}

		// Token: 0x17000F2E RID: 3886
		// (get) Token: 0x06005652 RID: 22098 RVA: 0x001CE09B File Offset: 0x001CC29B
		public int RemainingChargesSecondry
		{
			get
			{
				return this.remainingChargesSecondry;
			}
		}

		// Token: 0x17000F2F RID: 3887
		// (get) Token: 0x06005653 RID: 22099 RVA: 0x001CE0A3 File Offset: 0x001CC2A3
		public int MaxChargesSecondry
		{
			get
			{
				return this.Props.maxChargesSecondry;
			}
		}

		// Token: 0x17000F30 RID: 3888
		// (get) Token: 0x06005654 RID: 22100 RVA: 0x001CE0B0 File Offset: 0x001CC2B0
		public ThingDef AmmoDefSecondry
		{
			get
			{
				return this.Props.ammoDefSecondry ?? this.Props.ammoDef;
			}
		}

		// Token: 0x17000F31 RID: 3889
		// (get) Token: 0x06005655 RID: 22101 RVA: 0x001CE0BD File Offset: 0x001CC2BD
		public bool CanBeUsedSecondry
		{
			get
			{
				return this.remainingChargesSecondry > 0;
			}
		}

		// Token: 0x17000F38 RID: 3896
		// (get) Token: 0x0600565E RID: 22110 RVA: 0x001CE116 File Offset: 0x001CC316
		public string LabelRemainingSecondry
		{
			get
			{
				return string.Format("{0} / {1}", this.RemainingChargesSecondry, this.MaxChargesSecondry);
			}
		}



		// Token: 0x06005660 RID: 22112 RVA: 0x001CE145 File Offset: 0x001CC345
		public override void PostPostMake()
		{
			base.PostPostMake();
			this.remainingChargesSecondry = this.MaxChargesSecondry;
		}

		// Token: 0x06005661 RID: 22113 RVA: 0x001CE159 File Offset: 0x001CC359
		public override string CompInspectStringExtra()
		{
			return "TSF_ChargesRemaining".Translate(this.Props.ChargeNounArgument) + ": " + this.LabelRemaining+"\n"+ "TSF_ChargesRemaining".Translate(this.Props.ChargeNounArgumentSecondry) + ": " + this.LabelRemainingSecondry;
		}

		// Token: 0x06005663 RID: 22115 RVA: 0x001CE19C File Offset: 0x001CC39C
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref this.remainingChargesSecondry, "remainingChargesSecondry", -999, false);

			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.remainingChargesSecondry == -999)
			{
				this.remainingChargesSecondry = this.MaxChargesSecondry;
			}
		}

		
		// Token: 0x06005664 RID: 22116 RVA: 0x001CE200 File Offset: 0x001CC400
		public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
		{
			
			foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
			{
				var _Reloadable = gizmo as Command_VerbOwner;
				if (_Reloadable != null)
				{
					if (_Reloadable.verb.verbProps.label != Props.chargeNoun)
					{
						continue;
					}
				}
				yield return gizmo;
			}
			
			bool drafted = this.Wearer.Drafted;
			if ((drafted && !this.Props.displayGizmoWhileDrafted) || (!drafted && !this.Props.displayGizmoWhileUndrafted))
			{
				yield break;
			}
			ThingWithComps gear = this.parent;
			foreach (Verb verb in this.VerbTracker.AllVerbs)
			{
				if (verb.verbProps.hasStandardCommand && verb.verbProps.label == Props.chargeNounSecondry)
				{
					yield return this.CreateVerbTargetCommand(gear, verb);
				}
			}
			if (Prefs.DevMode)
			{
				yield return new Command_Action
				{
					defaultLabel = "Debug: Reload to full",
					action = delegate ()
					{
						this.remainingChargesSecondry = this.MaxChargesSecondry;
					}
				};
			}
			yield break;
		}
		

		// Token: 0x06005665 RID: 22117 RVA: 0x001CE210 File Offset: 0x001CC410
		private Command_ReloadableDual CreateVerbTargetCommand(Thing gear, Verb verb)
		{
			bool Primary = verb.verbProps.label == this.Props.chargeNoun;
			Command_ReloadableDual command_Reloadable = new Command_ReloadableDual(this);
			command_Reloadable.defaultDesc = gear.def.description;
			command_Reloadable.hotKey = Primary ? this.Props.hotKey : this.Props.hotKeySecondry;
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
			else if (!this.CanBeUsed(out _))
			{
				string r = Primary ? this.DisabledReason(this.MinAmmoNeeded(false), this.MaxAmmoNeeded(false)) : this.DisabledReasonSecondry(this.MinAmmoNeededSecondry(false), this.MaxAmmoNeededSecondry(false));
				command_Reloadable.Disable(r);
			}
			return command_Reloadable;
		}

		// Token: 0x06005666 RID: 22118 RVA: 0x001CE3D0 File Offset: 0x001CC5D0
		public string DisabledReasonSecondry(int minNeeded, int maxNeeded)
		{
			string result;
			if (this.AmmoDef == null)
			{
				result = "CommandReload_NoCharges".Translate(this.Props.ChargeNounArgumentSecondry);
			}
			else
			{
				string arg;
				if (this.Props.ammoCountToRefillSecondry != 0)
				{
					arg = this.Props.ammoCountToRefillSecondry.ToString();
				}
				else
				{
					arg = ((minNeeded == maxNeeded) ? minNeeded.ToString() : string.Format("{0}-{1}", minNeeded, maxNeeded));
				}
				result = "CommandReload_NoAmmo".Translate(this.Props.ChargeNounArgumentSecondry, this.AmmoDefSecondry.Named("AMMO"), arg.Named("COUNT"));
			}
			return result;
		}

		// Token: 0x06005667 RID: 22119 RVA: 0x001CE480 File Offset: 0x001CC680
		public bool NeedsReloadSecondry(bool allowForcedReload)
		{
			if (this.AmmoDefSecondry == null)
			{
				return false;
			}
			if (this.Props.ammoCountToRefillSecondry == 0)
			{
				return this.RemainingChargesSecondry != this.MaxChargesSecondry;
			}
			if (!allowForcedReload)
			{
				return this.remainingChargesSecondry == 0;
			}
			return this.RemainingChargesSecondry != this.MaxChargesSecondry;
		}

		public void ReloadFromSecondry(Thing ammo)
		{
			if (!this.NeedsReloadSecondry(true))
			{
				return;
			}
			if (this.Props.ammoCountToRefillSecondry != 0)
			{
				if (ammo.stackCount < this.Props.ammoCountToRefillSecondry)
				{
					return;
				}
				ammo.SplitOff(this.Props.ammoCountToRefillSecondry).Destroy(DestroyMode.Vanish);
				this.remainingChargesSecondry = this.MaxChargesSecondry;
			}
			else
			{
				if (ammo.stackCount < this.Props.ammoCountPerChargeSecondry)
				{
					return;
				}
				int num = Mathf.Clamp(ammo.stackCount / this.Props.ammoCountPerChargeSecondry, 0, this.MaxChargesSecondry - this.RemainingChargesSecondry);
				ammo.SplitOff(num * this.Props.ammoCountPerChargeSecondry).Destroy(DestroyMode.Vanish);
				this.remainingChargesSecondry += num;
			}
			if (this.Props.soundReload != null)
			{
				this.Props.soundReload.PlayOneShot(new TargetInfo(this.Wearer.Position, this.Wearer.Map, false));
			}
		}


		// Token: 0x06005669 RID: 22121 RVA: 0x001CE5D0 File Offset: 0x001CC7D0
		public int MinAmmoNeededSecondry(bool allowForcedReload)
		{
			if (!this.NeedsReloadSecondry(allowForcedReload))
			{
				return 0;
			}
			if (this.Props.ammoCountToRefillSecondry != 0)
			{
				return this.Props.ammoCountToRefillSecondry;
			}
			return this.Props.ammoCountPerChargeSecondry;
		}

		// Token: 0x0600566A RID: 22122 RVA: 0x001CE601 File Offset: 0x001CC801
		public int MaxAmmoNeededSecondry(bool allowForcedReload)
		{
			if (!this.NeedsReloadSecondry(allowForcedReload))
			{
				return 0;
			}
			if (this.Props.ammoCountToRefillSecondry != 0)
			{
				return this.Props.ammoCountToRefillSecondry;
			}
			return this.Props.ammoCountPerChargeSecondry * (this.MaxChargesSecondry - this.RemainingChargesSecondry);
		}

		// Token: 0x0600566B RID: 22123 RVA: 0x001CE640 File Offset: 0x001CC840
		public int MaxAmmoAmountSecondry()
		{
			if (this.AmmoDefSecondry == null)
			{
				return 0;
			}
			if (this.Props.ammoCountToRefillSecondry == 0)
			{
				return this.Props.ammoCountPerChargeSecondry * this.MaxChargesSecondry;
			}
			return this.Props.ammoCountToRefillSecondry;
		}

		// Token: 0x0600566C RID: 22124 RVA: 0x001CE677 File Offset: 0x001CC877
		public void UsedOnceSecondry()
		{
			if (this.remainingChargesSecondry > 0)
			{
				this.remainingChargesSecondry--;
			}
			if (this.Props.destroyOnEmpty && this.remainingChargesSecondry == 0)
			{
				this.parent.Destroy(DestroyMode.Vanish);
			}
		}

		// Token: 0x04003005 RID: 12293
		private int remainingChargesSecondry;
	}
}
