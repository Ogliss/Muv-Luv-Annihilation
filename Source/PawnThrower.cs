using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MuvLuvBeta
{
	// Token: 0x02000D25 RID: 3365
	public class PawnThrower : PawnThrown
	{
		// Token: 0x17000E54 RID: 3668
		// (get) Token: 0x060051E7 RID: 20967 RVA: 0x001B989C File Offset: 0x001B7A9C
		private Material ShadowMaterial
		{
			get
			{
				if (this.cachedShadowMaterial == null && !this.def.pawnFlyer.shadow.NullOrEmpty())
				{
					this.cachedShadowMaterial = MaterialPool.MatFrom(this.def.pawnFlyer.shadow, ShaderDatabase.Transparent);
				}
				return this.cachedShadowMaterial;
			}
		}

		// Token: 0x060051E8 RID: 20968 RVA: 0x001B98F4 File Offset: 0x001B7AF4
		static PawnThrower()
		{
			AnimationCurve animationCurve = new AnimationCurve();
			animationCurve.AddKey(0f, 0f);
			animationCurve.AddKey(0.1f, 0.15f);
			animationCurve.AddKey(1f, 1f);
			PawnThrower.FlightSpeed = new Func<float, float>(animationCurve.Evaluate);
		}

		// Token: 0x17000E55 RID: 3669
		// (get) Token: 0x060051E9 RID: 20969 RVA: 0x001B995E File Offset: 0x001B7B5E
		public override Vector3 DrawPos
		{
			get
			{
				this.RecomputePosition();
				return this.effectivePos;
			}
		}

		// Token: 0x060051EA RID: 20970 RVA: 0x001B996C File Offset: 0x001B7B6C
		protected override bool ValidateFlyer()
		{
			if (!ModLister.RoyaltyInstalled)
			{
				Log.ErrorOnce("Items with jump capability are a Royalty-specific game system. If you want to use this code please check ModLister.RoyaltyInstalled before calling it. See rules on the Ludeon forum for more info.", 550136797, false);
				return false;
			}
			return true;
		}

		// Token: 0x060051EB RID: 20971 RVA: 0x001B9988 File Offset: 0x001B7B88
		private void RecomputePosition()
		{
			if (this.positionLastComputedTick == this.ticksThrown)
			{
				return;
			}
			this.positionLastComputedTick = this.ticksThrown;
			float arg = (float)this.ticksThrown / (float)this.ticksThrownTime;
			float num = PawnThrower.FlightSpeed(arg);
			this.effectiveHeight = PawnThrower.FlightCurveHeight(num);
			this.groundPos = Vector3.Lerp(this.startVec, base.DestinationPos, num);
			Vector3 a = new Vector3(0f, 0f, 2f);
			Vector3 b = Altitudes.AltIncVect * this.effectiveHeight;
			Vector3 b2 = a * this.effectiveHeight;
			this.effectivePos = this.groundPos + b + b2;
		}

		// Token: 0x060051EC RID: 20972 RVA: 0x001B9A3E File Offset: 0x001B7C3E
		public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			this.RecomputePosition();
			this.DrawShadow(this.groundPos, this.effectiveHeight);
			base.FlyingPawn.DrawAt(this.effectivePos, flip);
		}

		// Token: 0x060051ED RID: 20973 RVA: 0x001B9A6C File Offset: 0x001B7C6C
		private void DrawShadow(Vector3 drawLoc, float height)
		{
			Material shadowMaterial = this.ShadowMaterial;
			if (shadowMaterial == null)
			{
				return;
			}
			float num = Mathf.Lerp(1f, 0.6f, height);
			Vector3 s = new Vector3(num, 1f, num);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(drawLoc, Quaternion.identity, s);
			Graphics.DrawMesh(MeshPool.plane10, matrix, shadowMaterial, 0);
		}

		// Token: 0x060051EE RID: 20974 RVA: 0x001B9ACC File Offset: 0x001B7CCC
		protected override void RespawnPawn()
		{
			this.LandingEffects();
			base.RespawnPawn();
		}

		// Token: 0x060051EF RID: 20975 RVA: 0x001B9ADC File Offset: 0x001B7CDC
		public override void Tick()
		{
			if (this.flightEffecter == null && this.def.pawnFlyer.flightEffecterDef != null)
			{
				this.flightEffecter = this.def.pawnFlyer.flightEffecterDef.Spawn();
				this.flightEffecter.Trigger(this, TargetInfo.Invalid);
			}
			else
			{
				Effecter effecter = this.flightEffecter;
				if (effecter != null)
				{
					effecter.EffectTick(this, TargetInfo.Invalid);
				}
			}
			base.Tick();
		}

		// Token: 0x060051F0 RID: 20976 RVA: 0x001B9B58 File Offset: 0x001B7D58
		private void LandingEffects()
		{
			if (this.def.pawnFlyer.soundLanding != null)
			{
				this.def.pawnFlyer.soundLanding.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
			}
			MoteMaker.ThrowDustPuff(base.DestinationPos + Gen.RandomHorizontalVector(0.5f), base.Map, 2f);
		}

		// Token: 0x060051F1 RID: 20977 RVA: 0x001B9BC8 File Offset: 0x001B7DC8
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			Effecter effecter = this.flightEffecter;
			if (effecter != null)
			{
				effecter.Cleanup();
			}
			base.Destroy(mode);
		}

		// Token: 0x04002E39 RID: 11833
		private static readonly Func<float, float> FlightSpeed;

		// Token: 0x04002E3A RID: 11834
		private static readonly Func<float, float> FlightCurveHeight = (float x) => -4f * x * (x - 1f);

		// Token: 0x04002E3B RID: 11835
		private Material cachedShadowMaterial;

		// Token: 0x04002E3C RID: 11836
		private Effecter flightEffecter;

		// Token: 0x04002E3D RID: 11837
		private int positionLastComputedTick = -1;

		// Token: 0x04002E3E RID: 11838
		private Vector3 groundPos;

		// Token: 0x04002E3F RID: 11839
		private Vector3 effectivePos;

		// Token: 0x04002E40 RID: 11840
		private float effectiveHeight;
	}
}
