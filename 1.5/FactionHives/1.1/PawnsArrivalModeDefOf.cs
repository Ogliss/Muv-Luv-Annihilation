using RimWorld;
using System;

namespace OgsOld_ExtraHives
{
	[DefOf]
	public static class PawnsArrivalModeDefOf
	{
		static PawnsArrivalModeDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(PawnsArrivalModeDefOf));
		}

		public static PawnsArrivalModeDef EdgeWalkInGroups;
		public static PawnsArrivalModeDef EdgeDropGroups;
		public static PawnsArrivalModeDef EdgeTunnelIn_OgsOld_ExtraHives;
		public static PawnsArrivalModeDef EdgeTunnelInGroups_OgsOld_ExtraHives;
		public static PawnsArrivalModeDef CenterTunnelIn_OgsOld_ExtraHives;
		public static PawnsArrivalModeDef RandomTunnelIn_OgsOld_ExtraHives;
	}
}
