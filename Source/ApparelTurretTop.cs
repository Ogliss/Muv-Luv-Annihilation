using System;
using UnityEngine;
using Verse;

namespace MuvLuvBeta
{
	// Token: 0x02000CD6 RID: 3286
	public class ApparelTurretTop
	{
		// Token: 0x17000DD4 RID: 3540
		// (get) Token: 0x06004F19 RID: 20249 RVA: 0x001AACC8 File Offset: 0x001A8EC8
		// (set) Token: 0x06004F1A RID: 20250 RVA: 0x001AACD0 File Offset: 0x001A8ED0
		private float CurRotation
		{
			get
			{
				return this.curRotationInt;
			}
			set
			{
				this.curRotationInt = value;
				if (this.curRotationInt > 360f)
				{
					this.curRotationInt -= 360f;
				}
				if (this.curRotationInt < 0f)
				{
					this.curRotationInt += 360f;
				}
			}
		}

		// Token: 0x06004F1B RID: 20251 RVA: 0x001AAD24 File Offset: 0x001A8F24
		public void SetRotationFromOrientation()
		{
			this.CurRotation = this.parentTurret.Wearer.Rotation.AsAngle;
		}

		// Token: 0x06004F1C RID: 20252 RVA: 0x001AAD4A File Offset: 0x001A8F4A
		public ApparelTurretTop(CompApparel_Turret ParentTurret)
		{
			this.parentTurret = ParentTurret;
		}

		// Token: 0x06004F1D RID: 20253 RVA: 0x001AAD5C File Offset: 0x001A8F5C
		public void TurretTopTick()
		{
			LocalTargetInfo currentTarget = this.parentTurret.CurrentTarget;
			if (currentTarget.IsValid)
			{
				float curRotation = (currentTarget.Cell.ToVector3Shifted() - this.parentTurret.Wearer.DrawPos).AngleFlat();
				this.CurRotation = curRotation;
				this.ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
				return;
			}
			if (this.ticksUntilIdleTurn > 0)
			{
				this.ticksUntilIdleTurn--;
				if (this.ticksUntilIdleTurn == 0)
				{
					if (Rand.Value < 0.5f)
					{
						this.idleTurnClockwise = true;
					}
					else
					{
						this.idleTurnClockwise = false;
					}
					this.idleTurnTicksLeft = 140;
					return;
				}
			}
			else
			{
				if (this.idleTurnClockwise)
				{
					this.CurRotation += 0.26f;
				}
				else
				{
					this.CurRotation -= 0.26f;
				}
				this.idleTurnTicksLeft--;
				if (this.idleTurnTicksLeft <= 0)
				{
					this.ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
				}
			}
		}

		// Token: 0x06004F1E RID: 20254 RVA: 0x001AAE64 File Offset: 0x001A9064
		public void DrawTurret()
		{
			Vector3 b = new Vector3(this.parentTurret.Props.TurretDef.building.turretTopOffset.x, 1f, this.parentTurret.Props.TurretDef.building.turretTopOffset.y).RotatedBy(this.CurRotation);
		//	Log.Message("b = " + b);
			float turretTopDrawSize = this.parentTurret.Props.TurretDef.building.turretTopDrawSize;
		//	Log.Message("turretTopDrawSize = " + turretTopDrawSize);
			Matrix4x4 matrix = default(Matrix4x4);
			Vector3 drawPos = this.parentTurret.Wearer.DrawPos + Altitudes.AltIncVect + b;
		//	Log.Message("drawPos = " + drawPos);
			Quaternion quart = (this.CurRotation + (float)ApparelTurretTop.ArtworkRotation).ToQuat();
		//	Log.Message("quart = " + quart);
			matrix.SetTRS(drawPos, quart, new Vector3(turretTopDrawSize, 1f, turretTopDrawSize));
			Graphics.DrawMesh(MeshPool.plane10, matrix, this.parentTurret.Props.TurretDef.building.turretTopMat, 0);
		}

		// Token: 0x04002CE1 RID: 11489
		private CompApparel_Turret parentTurret;

		// Token: 0x04002CE2 RID: 11490
		private float curRotationInt;

		// Token: 0x04002CE3 RID: 11491
		private int ticksUntilIdleTurn;

		// Token: 0x04002CE4 RID: 11492
		private int idleTurnTicksLeft;

		// Token: 0x04002CE5 RID: 11493
		private bool idleTurnClockwise;

		// Token: 0x04002CE6 RID: 11494
		private const float IdleTurnDegreesPerTick = 0.26f;

		// Token: 0x04002CE7 RID: 11495
		private const int IdleTurnDuration = 140;

		// Token: 0x04002CE8 RID: 11496
		private const int IdleTurnIntervalMin = 150;

		// Token: 0x04002CE9 RID: 11497
		private const int IdleTurnIntervalMax = 350;

		// Token: 0x04002CEA RID: 11498
		public static readonly int ArtworkRotation = -90;
	}
}
