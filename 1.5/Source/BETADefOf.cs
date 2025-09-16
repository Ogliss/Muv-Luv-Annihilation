using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation
{
	// Token: 0x02001012 RID: 4114
	[DefOf]
	public static class BETADefOf
	{
		// Token: 0x060064B9 RID: 25785 RVA: 0x0022FF4D File Offset: 0x0022E14D
		static BETADefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(BETADefOf));
		}

		public static DutyDef MLB_BETA_AssaultColony;
		public static DutyDef MLB_BETA_AssaultColony_CutPower;

		public static ThingDef BETA_SoldierClass;
		public static ThingDef BETA_WarriorClass;
		public static ThingDef BETA_TankClass;
		public static ThingDef BETA_GrapplerClass;
		public static ThingDef BETA_DestroyerClass;
		public static ThingDef BETA_FortClass;
		public static ThingDef BETA_LaserClass;
		public static ThingDef BETA_HeavyLaserClass;

		public static FactionDef MuvLuv_BETA;

		public static JobDef BETA_RefuelChargingStation;

		public static JobDef BETA_RefuelAtomicChargingStation;

		public static ThingDef MLB_ChargingStation;

		public static JobDef BETA_Ingest;

		public static EffecterDef JumpFlightEffect;
		public static SoundDef JumpPackLand, EnergyShield_Broken;



    }
}
