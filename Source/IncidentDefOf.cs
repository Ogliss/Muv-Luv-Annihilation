using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuvLuvBeta
{
	// Token: 0x02001012 RID: 4114
	[DefOf]
	public static class IncidentDefOf
	{
		// Token: 0x060064B9 RID: 25785 RVA: 0x0022FF4D File Offset: 0x0022E14D
		static IncidentDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(IncidentDefOf));
		}

		public static IncidentDef BETAInfestationInterior;
		public static IncidentDef BETAInfestationExterior;
		public static IncidentDef BETAShipPartCrash;
		public static IncidentDef BETAMonumentCrash;
	}
}
