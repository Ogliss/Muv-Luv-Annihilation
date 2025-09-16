using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace AnimatedProjectile
{
	// Token: 0x02000005 RID: 5
	public class Projectile_Explosive_Anim : Projectile_Explosive
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002863 File Offset: 0x00000A63
		private int frameTicks
		{
			get
			{
				return this.def.HasModExtension<AnimatedProjectileExtension>() ? this.def.GetModExtension<AnimatedProjectileExtension>().ticksPerFrame : 5;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002888 File Offset: 0x00000A88
		protected override void Tick()
		{
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
						this.TicksforFrame = this.frameTicks;
					}
				}
				this.TicksforFrame--;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002928 File Offset: 0x00000B28
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

		// Token: 0x06000012 RID: 18 RVA: 0x00002960 File Offset: 0x00000B60
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			bool flag = this.def.graphicData.graphicClass == typeof(Graphic_Flicker);
			if (flag)
			{
				this.traverse = Traverse.Create(base.Graphic);
				this.subGraphics = (Graphic[])Projectile_Explosive_Anim.subgraphics.GetValue(base.Graphic);
			}
		}

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Mesh mesh = MeshPool.GridPlane(this.def.graphicData.drawSize);
            Graphics.DrawMesh(mesh, this.DrawPos, this.ExactRotation, this.Graphic.MatSingle, 0);
            base.Comps_PostDraw();
        }

		// Token: 0x06000014 RID: 20 RVA: 0x00002A12 File Offset: 0x00000C12
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.gfxint, "gfxint", -1, false);
		}

		// Token: 0x04000010 RID: 16
		private Traverse traverse;

		// Token: 0x04000011 RID: 17
		private Graphic[] subGraphics;

		// Token: 0x04000012 RID: 18
		public static FieldInfo subgraphics = typeof(Graphic_Flicker).GetField("subGraphics", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

		// Token: 0x04000013 RID: 19
		private int gfxint = 0;

		// Token: 0x04000014 RID: 20
		private int TicksforFrame = 5;
	}
}
