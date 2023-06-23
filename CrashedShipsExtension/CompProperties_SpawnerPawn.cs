using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CrashedShipsExtension
{
	// Token: 0x02000004 RID: 4
	public class CompProperties_SpawnerPawn : CompProperties
	{
		// Token: 0x06000006 RID: 6 RVA: 0x0000230C File Offset: 0x0000050C
		public CompProperties_SpawnerPawn()
		{
			this.compClass = typeof(CompSpawnerPawn);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000238C File Offset: 0x0000058C
		public override void ResolveReferences(ThingDef parentDef)
		{
			base.ResolveReferences(parentDef);
			bool flag = this.factionGroupKindDef == null;
			if (flag)
			{
				this.factionGroupKindDef = PawnGroupKindDefOf.Combat;
			}
		}

		// Token: 0x04000004 RID: 4
		public List<PawnGenOption> spawnablePawnKinds = new List<PawnGenOption>();

		// Token: 0x04000005 RID: 5
		public List<PawnKindDef> AlwaysSpawnWith = new List<PawnKindDef>();

		// Token: 0x04000006 RID: 6
		public SoundDef spawnSound;

		// Token: 0x04000007 RID: 7
		public string spawnMessageKey;

		// Token: 0x04000008 RID: 8
		public string noPawnsLeftToSpawnKey;

		// Token: 0x04000009 RID: 9
		public string pawnsLeftToSpawnKey;

		// Token: 0x0400000A RID: 10
		public bool showNextSpawnInInspect;

		// Token: 0x0400000B RID: 11
		public bool shouldJoinParentLord;

		// Token: 0x0400000C RID: 12
		public Type lordJob;

		// Token: 0x0400000D RID: 13
		public float defendRadius = 21f;

		// Token: 0x0400000E RID: 14
		public int initialSpawnDelay = 120;

		// Token: 0x0400000F RID: 15
		public int initialPawnsCount;

		// Token: 0x04000010 RID: 16
		public float initialPawnsPoints;

		// Token: 0x04000011 RID: 17
		public float maxSpawnedPawnsPoints = -1f;

		// Token: 0x04000012 RID: 18
		public FloatRange pawnSpawnIntervalDays = new FloatRange(0.85f, 1.15f);

		// Token: 0x04000013 RID: 19
		public int pawnSpawnRadius = 2;

		// Token: 0x04000014 RID: 20
		public IntRange maxPawnsToSpawn = IntRange.zero;

		// Token: 0x04000015 RID: 21
		public bool chooseSingleTypeToSpawn;

		// Token: 0x04000016 RID: 22
		public string nextSpawnInspectStringKey;

		// Token: 0x04000017 RID: 23
		public string nextSpawnInspectStringKeyDormant;

		// Token: 0x04000018 RID: 24
		public PawnGroupKindDef factionGroupKindDef;
	}
}
