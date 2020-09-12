using RimWorld;
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
		public ApparelTurretTop(Comp_Turret ParentTurret)
		{
			this.parentTurret = ParentTurret;
		}
		public Vector3 DrawPos
		{
			get
			{
				Vector3 b = new Vector3(this.parentTurret.Props.TurretDef.building.turretTopOffset.x, 1f, this.parentTurret.Props.TurretDef.building.turretTopOffset.y).RotatedBy(this.CurRotation);
				Vector3 vector = this.parentTurret.Wearer.DrawPos;
				if (this.parentTurret.Wearer.ParentHolder as PawnFlyer is PawnFlyer flyer)
				{
					vector = flyer.DrawPos;
				}
				Vector3 drawPos = vector + Altitudes.AltIncVect + b; 

				Rot4 rot = this.parentTurret.Wearer.Rotation;
				if (rot == Rot4.North)
				{
					drawPos += this.parentTurret.Props.offsetNorth;
				}
				if (rot == Rot4.South)
				{
					drawPos += this.parentTurret.Props.offsetSouth;
				}
				if (rot == Rot4.East)
				{
					drawPos += this.parentTurret.Props.offsetEast;
				}
				if (rot == Rot4.West)
				{
					drawPos += this.parentTurret.Props.offsetWest;
				}
				return drawPos;
			}
		}
		// Token: 0x06004F1D RID: 20253 RVA: 0x001AAD5C File Offset: 0x001A8F5C
		public void TurretTopTick()
		{
			LocalTargetInfo currentTarget = this.parentTurret.CurrentTarget;
			if (this.parentTurret.stunTicksLeft > 0)
			{
				/*
				if (this.parentTurret.showStunMote && (this.parentTurret.moteStun == null || this.parentTurret.moteStun.Destroyed))
				{
					this.parentTurret.moteStun = MakeStunOverlay(this.parentTurret.Wearer);
				}
				*/
				Pawn pawn = this.parentTurret.Wearer as Pawn;
				if (pawn != null && pawn.Downed)
				{
					this.parentTurret.stunTicksLeft = 0;
				}

				if (this.parentTurret.moteStun != null)
				{
					this.parentTurret.moteStun.Maintain();
				}

				if (this.parentTurret.AffectedByEMP && this.parentTurret.stunFromEMP)
				{
					if (this.parentTurret.empEffecter == null)
					{
						this.parentTurret.empEffecter = new EffecterComp(DefDatabase<EffecterDef>.GetNamed("CompTurretDisabledByEMP"));
					}
					EffecterComp empEffecter = this.parentTurret.empEffecter as EffecterComp;
					if (empEffecter != null)
					{
					//	Log.Message("empEffecter EffecterComp");
						empEffecter.EffectTick(this.parentTurretGun, this.parentTurret.Wearer);
					}
					else
					{
					//	Log.Message("empEffecter Effecter");
						this.parentTurret.empEffecter.EffectTick(this.parentTurret.Wearer, this.parentTurret.Wearer);
					}
					return;
				}
			}
			else if (this.parentTurret.empEffecter != null)
			{
				this.parentTurret.empEffecter.Cleanup();
				this.parentTurret.empEffecter = null;
				this.parentTurret.stunFromEMP = false;
			}
			if (currentTarget.IsValid)
			{
				float curRotation = (currentTarget.Cell.ToVector3Shifted() - DrawPos).AngleFlat();
				this.CurRotation = curRotation;
				this.ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
				return;
			}
			/*
			else
			{
				SetRotationFromOrientation();
			}
			*/

			float rot = this.parentTurret.Wearer.Rotation.AsAngle;
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
		public Mote MakeStunOverlay(Thing stunnedThing)
		{
			MoteCompTurretAttached mote = (MoteCompTurretAttached)ThingMaker.MakeThing(ThingDef.Named("Mote_CompTurretStun"), null);
			mote.Attach(stunnedThing, parentTurretGun);
			GenSpawn.Spawn(mote, stunnedThing.Position, stunnedThing.Map, WipeMode.Vanish);
			return mote;
		}

		// Token: 0x06004F1E RID: 20254 RVA: 0x001AAE64 File Offset: 0x001A9064
		public void DrawTurret()
		{
			float turretTopDrawSize = this.parentTurret.Props.TurretDef.building.turretTopDrawSize;
			Matrix4x4 matrix = default(Matrix4x4);
			Quaternion quart = (this.CurRotation + (float)ApparelTurretTop.ArtworkRotation).ToQuat();
			matrix.SetTRS(DrawPos, quart, new Vector3(turretTopDrawSize, 1f, turretTopDrawSize));
			Graphics.DrawMesh(MeshPool.plane10, matrix, this.parentTurret.Props.TurretDef.building.turretTopMat, 0);
			if (this.parentTurret.TargetCurrentlyAimingAt != null && !this.parentTurret.Stunned && parentTurretGun !=null)
			{
				GenDraw.DrawLineBetween(DrawPos, this.parentTurret.CurrentTarget.CenterVector3, Comp_TurretGun.LineMatRed);
			}

		}
		// Token: 0x04002CE1 RID: 11489
		private Comp_Turret parentTurret;
		private Comp_TurretGun parentTurretGun => parentTurret as Comp_TurretGun;

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
