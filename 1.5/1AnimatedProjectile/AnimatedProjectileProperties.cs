using System;
using Verse;

namespace AnimatedProjectile
{
	// Token: 0x02000003 RID: 3
	public class AnimatedProjectileProperties : ProjectileProperties
	{
		// Token: 0x04000002 RID: 2
		public int ticksPerFrame = 15;

		// Token: 0x04000003 RID: 3
		public bool growerDistance = false;

		// Token: 0x04000004 RID: 4
		public float growerStartSize = 1f;

		// Token: 0x04000005 RID: 5
		public float growerEndSize = 1f;

		// Token: 0x04000006 RID: 6
		public float rotation = 0f;

		// Token: 0x04000007 RID: 7
		public string drawGlowMote = string.Empty;

		// Token: 0x04000008 RID: 8
		public bool drawGlow = false;

		// Token: 0x04000009 RID: 9
		public float drawGlowSizeFactor = 6f;

		// Token: 0x0400000A RID: 10
		public bool setsFire = false;

		// Token: 0x0400000B RID: 11
		public float setFireChance = 0.75f;

		// Token: 0x0400000C RID: 12
		public bool ignites = false;

		// Token: 0x0400000D RID: 13
		public float igniteChance = 0.25f;
	}
}
