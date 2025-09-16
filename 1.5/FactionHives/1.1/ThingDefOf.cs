using RimWorld;
using System;
using Verse;

namespace OgsOld_ExtraHives
{
	[DefOf]
	public static class ThingDefOf
	{
		static ThingDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));

		public static ThingDef Tunneler_OgsOld_ExtraHives;
		public static ThingDef InfestedMeteoriteIncoming_OgsOld_ExtraHives; 
		
	}
}
