using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace AnimatedProjectile
{
	// Token: 0x02000006 RID: 6
	public class Projectile_Anim : Projectile
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002A66 File Offset: 0x00000C66
		public AnimatedProjectileProperties Props
		{
			get
			{
				return this.def.projectile as AnimatedProjectileProperties;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000018 RID: 24 RVA: 0x00002A78 File Offset: 0x00000C78
		public float Drawsize
		{
			get
			{
				return (this.Props.growerStartSize == this.Props.growerEndSize) ? (this.Props.growerEndSize * (this.Props.growerDistance ? this.Traveled : 1f)) : Mathf.Lerp(this.Props.growerStartSize, this.Props.growerEndSize, this.Props.growerDistance ? this.Traveled : 1f);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002AFA File Offset: 0x00000CFA
		public float Distancetotravel
		{
			get
			{
				return this.launcher.Position.DistanceTo(this.usedTarget.Cell);
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002B17 File Offset: 0x00000D17
		public float Distancetraveled
		{
			get
			{
				return this.launcher.Position.DistanceTo(base.Position);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002B2F File Offset: 0x00000D2F
		public float Traveled
		{
			get
			{
				return this.Distancetraveled / this.Distancetotravel;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002B3E File Offset: 0x00000D3E
		public int FrameTicks
		{
			get
			{
				AnimatedProjectileProperties props = this.Props;
				return (props != null) ? props.ticksPerFrame : (this.def.HasModExtension<AnimatedProjectileExtension>() ? this.def.GetModExtension<AnimatedProjectileExtension>().ticksPerFrame : 5);
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002B74 File Offset: 0x00000D74
		public override void Tick()
		{
			this.age++;
			base.Tick();
			bool flag = this.def.graphicData.graphicClass == typeof(Graphic_Flicker);
			checked
			{
				if (flag)
				{
					bool flag2 = this.TicksforFrame == 0 && base.Map != null;
					bool flag3 = flag2;
					if (flag3)
					{
						this.gfxint++;
						bool flag4 = this.gfxint >= this.subGraphics.Length;
						if (flag4)
						{
							this.gfxint = 0;
						}
						this.TicksforFrame = this.FrameTicks;
					}
				}
				bool flag5 = this.def.graphicData.graphicClass == typeof(Graphic_Random);
				if (flag5)
				{
					bool flag2 = this.TicksforFrame == 0 && base.Map != null;
					bool flag6 = flag2;
					if (flag6)
					{
						Rand.PushState();
						this.gfxint = Rand.RangeInclusive(0, this.subGraphics.Length - 1);
						Rand.PopState();
						bool flag7 = this.gfxint >= this.subGraphics.Length;
						if (flag7)
						{
							this.gfxint = 0;
						}
						this.TicksforFrame = this.FrameTicks;
					}
				}
				this.TicksforFrame--;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002CB8 File Offset: 0x00000EB8
		public override Graphic Graphic
		{
			get
			{
				bool flag = this.subGraphics != null;
				Graphic graphic;
				if (flag)
				{
					graphic = this.subGraphics[this.gfxint];
				}
				else
				{
					graphic = base.Graphic;
				}
				return graphic;
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002CF0 File Offset: 0x00000EF0
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			bool flag = this.def.graphicData.graphicClass == typeof(Graphic_Flicker);
			if (flag)
			{
				this.traverse = Traverse.Create(base.Graphic);
				this.subGraphics = (Graphic[])Projectile_Anim.subgraphicsFlicker.GetValue(base.Graphic);
			}
			bool flag2 = this.def.graphicData.graphicClass == typeof(Graphic_Random);
			if (flag2)
			{
				this.traverse = Traverse.Create(base.Graphic);
				this.subGraphics = (Graphic[])Projectile_Anim.subgraphicsRandom.GetValue(base.Graphic);
			}
		}

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Mesh mesh = MeshPool.GridPlane(this.def.graphicData.drawSize * this.Drawsize);
            Graphics.DrawMesh(mesh, this.DrawPos, this.ExactRotation, this.Graphic.MatSingle, 0);
            base.Comps_PostDraw();
        }

		// Token: 0x06000021 RID: 33 RVA: 0x00002E04 File Offset: 0x00001004
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.gfxint, "gfxint", -1, false);
			Scribe_Values.Look<int>(ref this.TicksforFrame, "TicksforFrame", 0, false);
			Scribe_Values.Look<int>(ref this.age, "age", 0, false);
		}

		// Token: 0x04000015 RID: 21
		private Traverse traverse;

		// Token: 0x04000016 RID: 22
		private Graphic[] subGraphics;

		// Token: 0x04000017 RID: 23
		public static FieldInfo subgraphicsFlicker = typeof(Graphic_Flicker).GetField("subGraphics", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

		// Token: 0x04000018 RID: 24
		public static FieldInfo subgraphicsRandom = typeof(Graphic_Random).GetField("subGraphics", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

		// Token: 0x04000019 RID: 25
		private int gfxint = 0;

		// Token: 0x0400001A RID: 26
		private int TicksforFrame = 5;

		// Token: 0x0400001B RID: 27
		private int age = 0;
	}
}
