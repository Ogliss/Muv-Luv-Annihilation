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

	public class DamageWorker_Tear : DamageWorker_AddInjury
	{
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
			if (thing is Pawn pawn && dinfo.Amount >= 15)
            {
				if (Rand.Chance(0.8f))
                {
					List<BodyPartRecord> list = (from x in pawn.RaceProps.body.AllParts
												 where !pawn.health.hediffSet.PartIsMissing(x)
												&& x.depth == BodyPartDepth.Outside
												&& x.coverage > 0.1f
												 select x).ToList<BodyPartRecord>();

					if (list.TryRandomElement<BodyPartRecord>(out var bodyPartRecord))
					{
						var missingBodyPart = HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, pawn, bodyPartRecord);
						pawn.health.AddHediff(missingBodyPart);
					}
				}
			}

			return base.Apply(dinfo, thing);
        }
    }
}
