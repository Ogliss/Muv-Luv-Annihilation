using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MuvLuvAnnihilation
{
	// Token: 0x02000D28 RID: 3368
	public class PawnDasher : PawnFlyer
	{
		// Token: 0x17000E62 RID: 3682
		// (get) Token: 0x06005220 RID: 21024 RVA: 0x001BB10C File Offset: 0x001B930C
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

		// Token: 0x06005221 RID: 21025 RVA: 0x001BB164 File Offset: 0x001B9364
		static PawnDasher()
		{
			AnimationCurve animationCurve = new AnimationCurve();
			animationCurve.AddKey(0f, 0f);
			animationCurve.AddKey(0.1f, 0.15f);
			animationCurve.AddKey(1f, 1f);
			PawnDasher.FlightSpeed = new Func<float, float>(animationCurve.Evaluate);
		}

		// Token: 0x17000E63 RID: 3683
		// (get) Token: 0x06005222 RID: 21026 RVA: 0x001BB1CA File Offset: 0x001B93CA
		public override Vector3 DrawPos
		{
			get
			{
				this.RecomputePosition();
				return this.effectivePos;
			}
		}

		// Token: 0x06005223 RID: 21027 RVA: 0x001BB1D8 File Offset: 0x001B93D8
		protected override bool ValidateFlyer()
		{
			if (!ModLister.RoyaltyInstalled)
			{
				Log.ErrorOnce("Items with jump capability are a Royalty-specific game system. If you want to use this code please check ModLister.RoyaltyInstalled before calling it. See rules on the Ludeon forum for more info.", 550136797, false);
				return false;
			}
			return true;
		}

		// Token: 0x06005224 RID: 21028 RVA: 0x001BB1F4 File Offset: 0x001B93F4
		private void RecomputePosition()
		{
			if (this.positionLastComputedTick == this.ticksFlying)
			{
				return;
			}
			this.positionLastComputedTick = this.ticksFlying;
			float arg = (float)this.ticksFlying / (float)this.ticksFlightTime;
			float num = PawnDasher.FlightSpeed(arg);
			this.effectiveHeight = 0;
			this.groundPos = Vector3.Lerp(this.startVec, base.DestinationPos, num);
			Vector3 a = new Vector3(0f, 0f, 2f);
			Vector3 b = Altitudes.AltIncVect * this.effectiveHeight;
			Vector3 b2 = a * this.effectiveHeight;
			this.effectivePos = this.groundPos + b + b2;
		}

		// Token: 0x06005225 RID: 21029 RVA: 0x001BB2AA File Offset: 0x001B94AA
		public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			this.RecomputePosition();
			this.DrawShadow(this.groundPos, this.effectiveHeight);
			base.FlyingPawn.DrawAt(this.effectivePos, flip);
		}

		// Token: 0x06005226 RID: 21030 RVA: 0x001BB2D8 File Offset: 0x001B94D8
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

		// Token: 0x06005227 RID: 21031 RVA: 0x001BB338 File Offset: 0x001B9538
		protected override void RespawnPawn()
		{
			this.LandingEffects();
			base.RespawnPawn();
		}

		// Token: 0x06005228 RID: 21032 RVA: 0x001BB348 File Offset: 0x001B9548
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

		// Token: 0x06005229 RID: 21033 RVA: 0x001BB3C4 File Offset: 0x001B95C4
		private void LandingEffects()
		{
			if (this.def.pawnFlyer.soundLanding != null)
			{
				this.def.pawnFlyer.soundLanding.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
			}
			MoteMaker.ThrowDustPuff(base.DestinationPos + Gen.RandomHorizontalVector(0.5f), base.Map, 2f);
		}

		// Token: 0x0600522A RID: 21034 RVA: 0x001BB434 File Offset: 0x001B9634
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			Effecter effecter = this.flightEffecter;
			if (effecter != null)
			{
				effecter.Cleanup();
			}
			base.Destroy(mode);
		}

		// Token: 0x04002E5A RID: 11866
		private static readonly Func<float, float> FlightSpeed;

		// Token: 0x04002E5B RID: 11867
		private static readonly Func<float, float> FlightCurveHeight = new Func<float, float>(GenMath.InverseParabola);

		// Token: 0x04002E5C RID: 11868
		private Material cachedShadowMaterial;

		// Token: 0x04002E5D RID: 11869
		private Effecter flightEffecter;

		// Token: 0x04002E5E RID: 11870
		private int positionLastComputedTick = -1;

		// Token: 0x04002E5F RID: 11871
		private Vector3 groundPos;

		// Token: 0x04002E60 RID: 11872
		private Vector3 effectivePos;

		// Token: 0x04002E61 RID: 11873
		private float effectiveHeight;
	}
}
