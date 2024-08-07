﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MuvLuvAnnihilation
{
	// MuvLuvBeta.CompProperties_BETAProjectileInterceptor

	// Token: 0x02000DBD RID: 3517
	public class CompProperties_BETAProjectileInterceptor : CompProperties_ProjectileInterceptor
	{
		// Token: 0x060055DE RID: 21982 RVA: 0x001CB9E8 File Offset: 0x001C9BE8
		public CompProperties_BETAProjectileInterceptor()
		{
			this.compClass = typeof(CompBETAProjectileInterceptor);
		}
		public HediffDef Hediff = null;

	}

	[StaticConstructorOnStartup]
	public class CompBETAProjectileInterceptor : CompProjectileInterceptor
	{
		private int lastInterceptTicks = -999999;

		private int nextChargeTick = -1;

		private bool shutDown;

		private StunHandler stunner;

		private float lastInterceptAngle;

		private bool debugInterceptNonHostileProjectiles;

		private static readonly Material ForceFieldMat = MaterialPool.MatFrom("Other/ForceField", ShaderDatabase.MoteGlow);

		private static readonly Material ForceFieldConeMat = MaterialPool.MatFrom("Other/ForceFieldCone", ShaderDatabase.MoteGlow);

		private static readonly MaterialPropertyBlock MatPropertyBlock = new MaterialPropertyBlock();

		private const float TextureActualRingSizeFactor = 1.16015625f;

		private static readonly Color InactiveColor = new Color(0.2f, 0.2f, 0.2f);

		public new CompProperties_BETAProjectileInterceptor Props => (CompProperties_BETAProjectileInterceptor)props;

		public new bool Active
		{
			get
			{
				if (Props.Hediff!=null)
				{
					Pawn pawn = this.parent as Pawn;
					if (pawn != null)
					{
						if (!pawn.health.hediffSet.HasHediff(Props.Hediff))
						{
							return false;
						}
					}
				}
				if (!OnCooldown && !stunner.Stunned && !shutDown)
				{
					return !Charging;
				}
				return false;
			}
		}

		public new bool OnCooldown => Find.TickManager.TicksGame < lastInterceptTicks + Props.cooldownTicks;

		public new bool Charging
		{
			get
			{
				if (nextChargeTick >= 0)
				{
					return Find.TickManager.TicksGame > nextChargeTick;
				}
				return false;
			}
		}

		public new int ChargeCycleStartTick
		{
			get
			{
				if (nextChargeTick < 0)
				{
					return 0;
				}
				return nextChargeTick;
			}
		}

		public new int ChargingTicksLeft
		{
			get
			{
				if (nextChargeTick < 0)
				{
					return 0;
				}
				return nextChargeTick + Props.chargeDurationTicks - Find.TickManager.TicksGame;
			}
		}

		public new int CooldownTicksLeft
		{
			get
			{
				if (!OnCooldown)
				{
					return 0;
				}
				return Props.cooldownTicks - (Find.TickManager.TicksGame - lastInterceptTicks);
			}
		}

		public new bool ReactivatedThisTick => Find.TickManager.TicksGame - lastInterceptTicks == Props.cooldownTicks;

		public override void PostPostMake()
		{
			base.PostPostMake();
			if (Props.chargeIntervalTicks > 0)
			{
				nextChargeTick = Find.TickManager.TicksGame + Rand.Range(0, Props.chargeIntervalTicks);
			}
			stunner = new StunHandler(parent);
		}

		public new bool CheckIntercept(Projectile projectile, Vector3 lastExactPos, Vector3 newExactPos)
		{
		//	Log.Message("CheckIntercept");
			if (!ModLister.RoyaltyInstalled)
			{
				Log.ErrorOnce("Shields are a Royalty-specific game system. If you want to use this code please check ModLister.RoyaltyInstalled before calling it.", 657212);
				return false;
			}
			Vector3 vector = parent.Position.ToVector3Shifted();
			float num = Props.radius + projectile.def.projectile.SpeedTilesPerTick + 0.1f;
			if ((newExactPos.x - vector.x) * (newExactPos.x - vector.x) + (newExactPos.z - vector.z) * (newExactPos.z - vector.z) > num * num)
			{
				return false;
			}
			if (!Active)
			{
				return false;
			}
			if (!(Props.interceptGroundProjectiles ? (!projectile.def.projectile.flyOverhead) : (Props.interceptAirProjectiles && projectile.def.projectile.flyOverhead)))
			{
				return false;
			}
			if ((projectile.Launcher == null || (projectile.Launcher.Faction !=null && projectile.Launcher.Faction == parent.Faction)) && !debugInterceptNonHostileProjectiles && !Props.interceptNonHostileProjectiles)
			{
				return false;
			}
			if (!Props.interceptOutgoingProjectiles && (new Vector2(vector.x, vector.z) - new Vector2(lastExactPos.x, lastExactPos.z)).sqrMagnitude <= Props.radius * Props.radius)
			{
				return false;
			}
			if (!GenGeo.IntersectLineCircleOutline(new Vector2(vector.x, vector.z), Props.radius, new Vector2(lastExactPos.x, lastExactPos.z), new Vector2(newExactPos.x, newExactPos.z)))
			{
				return false;
			}
			lastInterceptAngle = lastExactPos.AngleToFlat(parent.TrueCenter());
			lastInterceptTicks = Find.TickManager.TicksGame;
			if (projectile.def.projectile.damageDef == DamageDefOf.EMP)
			{
				BreakShield(new DamageInfo(projectile.def.projectile.damageDef, projectile.def.projectile.damageDef.defaultDamage));
			}
			Effecter effecter = new Effecter(Props.interceptEffect ?? EffecterDefOf.Interceptor_BlockedProjectile);
			effecter.Trigger(new TargetInfo(newExactPos.ToIntVec3(), parent.Map), TargetInfo.Invalid);
			effecter.Cleanup();
			return true;
		}

		public override void CompTick()
		{
			if (ReactivatedThisTick && Props.reactivateEffect != null)
			{
				Effecter effecter = new Effecter(Props.reactivateEffect);
				effecter.Trigger(parent, TargetInfo.Invalid);
				effecter.Cleanup();
			}
			if (Find.TickManager.TicksGame >= nextChargeTick + Props.chargeDurationTicks)
			{
				nextChargeTick += Props.chargeIntervalTicks;
			}
			stunner.StunHandlerTick();
		}

		public override void Notify_LordDestroyed()
		{
			base.Notify_LordDestroyed();
			shutDown = true;
		}

		public override void PostDraw()
		{
			if (!Active)
			{
				return;
			}
			base.PostDraw();
		}

		private float GetCurrentAlpha()
		{
			return Mathf.Max(Mathf.Max(Mathf.Max(Mathf.Max(GetCurrentAlpha_Idle(), GetCurrentAlpha_Selected()), GetCurrentAlpha_RecentlyIntercepted()), GetCurrentAlpha_RecentlyActivated()), Props.minAlpha);
		}

		private float GetCurrentAlpha_Idle()
		{
			if (!Active)
			{
				return 0f;
			}
			if (parent.Faction == Faction.OfPlayer && !debugInterceptNonHostileProjectiles)
			{
				return 0f;
			}
			if (Find.Selector.IsSelected(parent))
			{
				return 0f;
			}
			return Mathf.Lerp(-1.7f, 0.11f, (Mathf.Sin((float)(Gen.HashCombineInt(parent.thingIDNumber, 96804938) % 100) + Time.realtimeSinceStartup * 0.7f) + 1f) / 2f);
		}

		private float GetCurrentAlpha_Selected()
		{
			if (!Find.Selector.IsSelected(parent) || stunner.Stunned || shutDown)
			{
				return 0f;
			}
			if (!Active)
			{
				return 0.41f;
			}
			return Mathf.Lerp(0.2f, 0.62f, (Mathf.Sin((float)(Gen.HashCombineInt(parent.thingIDNumber, 35990913) % 100) + Time.realtimeSinceStartup * 2f) + 1f) / 2f);
		}

		private float GetCurrentAlpha_RecentlyIntercepted()
		{
			int num = Find.TickManager.TicksGame - lastInterceptTicks;
			return Mathf.Clamp01(1f - (float)num / 40f) * 0.09f;
		}

		private float GetCurrentAlpha_RecentlyActivated()
		{
			if (!Active)
			{
				return 0f;
			}
			int num = Find.TickManager.TicksGame - (lastInterceptTicks + Props.cooldownTicks);
			return Mathf.Clamp01(1f - (float)num / 50f) * 0.09f;
		}

		private float GetCurrentConeAlpha_RecentlyIntercepted()
		{
			int num = Find.TickManager.TicksGame - lastInterceptTicks;
			return Mathf.Clamp01(1f - (float)num / 40f) * 0.82f;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Prefs.DevMode)
			{
				if (OnCooldown)
				{
					Command_Action command_Action = new Command_Action();
					command_Action.defaultLabel = "Dev: Reset cooldown";
					command_Action.action = delegate
					{
						lastInterceptTicks = Find.TickManager.TicksGame - Props.cooldownTicks;
					};
					yield return command_Action;
				}
				Command_Toggle command_Toggle = new Command_Toggle();
				command_Toggle.defaultLabel = "Dev: Intercept non-hostile";
				command_Toggle.isActive = (() => debugInterceptNonHostileProjectiles);
				command_Toggle.toggleAction = delegate
				{
					debugInterceptNonHostileProjectiles = !debugInterceptNonHostileProjectiles;
				};
				yield return command_Toggle;
			}
		}

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!Active)
			{
				return stringBuilder.ToString();
			}
			if (Props.interceptGroundProjectiles || Props.interceptAirProjectiles)
			{
				string value = (!Props.interceptGroundProjectiles) ? ((string)"InterceptsProjectiles_AerialProjectiles".Translate()) : ((string)"InterceptsProjectiles_GroundProjectiles".Translate());
				if (Props.cooldownTicks > 0)
				{
					stringBuilder.Append("InterceptsProjectilesEvery".Translate(value, Props.cooldownTicks.ToStringTicksToPeriod()));
				}
				else
				{
					stringBuilder.Append("InterceptsProjectiles".Translate(value));
				}
			}
			if (OnCooldown)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append("CooldownTime".Translate() + ": " + CooldownTicksLeft.ToStringTicksToPeriod());
			}
			if (stunner.Stunned)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append("DisarmedTime".Translate() + ": " + stunner.StunTicksLeft.ToStringTicksToPeriod());
			}
			if (shutDown)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append("ShutDown".Translate());
			}
			else if (Props.chargeIntervalTicks > 0)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendLine();
				}
				if (Charging)
				{
					stringBuilder.Append("ChargingTime".Translate() + ": " + ChargingTicksLeft.ToStringTicksToPeriod());
				}
				else
				{
					stringBuilder.Append("ChargingNext".Translate((ChargeCycleStartTick - Find.TickManager.TicksGame).ToStringTicksToPeriod(), Props.chargeDurationTicks.ToStringTicksToPeriod(), Props.chargeIntervalTicks.ToStringTicksToPeriod()));
				}
			}
			return stringBuilder.ToString();
		}

		private void BreakShield(DamageInfo dinfo)
		{
			float fTheta;
			if (Active)
			{
				BETADefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(parent));
				int num = Mathf.CeilToInt(Props.radius * 2f);
				fTheta = (float)Math.PI * 2f / (float)num;
				for (int i = 0; i < num; i++)
				{
					FleckMaker.ConnectingLine(PosAtIndex(i), PosAtIndex((i + 1) % num), FleckDefOf.LineEMP, parent.Map, 1.5f);
				}
			}
			dinfo.SetAmount((float)Props.disarmedByEmpForTicks / 30f);
			stunner.Notify_DamageApplied(dinfo);
			Vector3 PosAtIndex(int index)
			{
				return new Vector3(Props.radius * Mathf.Cos(fTheta * (float)index) + (float)parent.Position.x, 0f, Props.radius * Mathf.Sin(fTheta * (float)index) + (float)parent.Position.z);
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref lastInterceptTicks, "lastInterceptTicks", -999999);
			Scribe_Values.Look(ref shutDown, "shutDown", defaultValue: false);
			Scribe_Values.Look(ref nextChargeTick, "nextChargeTick", -1);
			Scribe_Deep.Look(ref stunner, "stunner", parent);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (Props.chargeIntervalTicks > 0 && nextChargeTick <= 0)
				{
					nextChargeTick = Find.TickManager.TicksGame + Rand.Range(0, Props.chargeIntervalTicks);
				}
				if (stunner == null)
				{
					stunner = new StunHandler(parent);
				}
			}
		}
	}

}
