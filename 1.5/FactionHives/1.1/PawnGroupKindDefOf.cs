using RimWorld;
using System;
using Verse;

namespace OgsOld_ExtraHives
{
	[DefOf]
	public static class PawnGroupKindDefOf
	{
		static PawnGroupKindDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(PawnGroupKindDefOf));
		}
		
		public static PawnGroupKindDef Hive_OgsOld_ExtraHives;
		public static PawnGroupKindDef Tunneler_OgsOld_ExtraHives;
		
	}
}
