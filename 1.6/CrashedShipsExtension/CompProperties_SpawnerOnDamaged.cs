using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace CrashedShipsExtension
{
	// Token: 0x02000006 RID: 6
	public class CompProperties_SpawnerOnDamaged : CompProperties
	{
		// Token: 0x06000021 RID: 33 RVA: 0x0000325C File Offset: 0x0000145C
		public CompProperties_SpawnerOnDamaged()
		{
			this.compClass = typeof(CompSpawnerOnDamaged);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000032D8 File Offset: 0x000014D8
		public override void ResolveReferences(ThingDef parentDef)
		{
			base.ResolveReferences(parentDef);
			bool flag = this.factionGroupKindDef == null;
			if (flag)
			{
				this.factionGroupKindDef = PawnGroupKindDefOf.Combat;
			}
		}

		// Token: 0x04000022 RID: 34
		public FactionDef Faction;

		// Token: 0x04000023 RID: 35
		public Faction faction;

		// Token: 0x04000024 RID: 36
		public List<PawnGenOption> allowedKinddefs = new List<PawnGenOption>();

		// Token: 0x04000025 RID: 37
		public List<PawnKindDef> disallowedKinddefs = new List<PawnKindDef>();

		// Token: 0x04000026 RID: 38
		public List<FactionDef> Factions = new List<FactionDef>();

		// Token: 0x04000027 RID: 39
		public List<FactionDef> disallowedFactions = new List<FactionDef>();

		// Token: 0x04000028 RID: 40
		public string techLevel;

		// Token: 0x04000029 RID: 41
		public bool allowHidden = true;

		// Token: 0x0400002A RID: 42
		public bool allowNonHumanlike = false;

		// Token: 0x0400002B RID: 43
		public bool allowDefeated = true;

		// Token: 0x0400002C RID: 44
		public ThingDef skyFaller;

		// Token: 0x0400002D RID: 45
		public float defaultPoints = 550f;

		// Token: 0x0400002E RID: 46
		public float minPoints = 300f;

		// Token: 0x0400002F RID: 47
		public PawnGroupKindDef factionGroupKindDef;
	}
}
