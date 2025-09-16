using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MuvLuvAnnihilation
{
	public static class MechUtils
	{
		public static IEnumerable<Thing> MechSuits(Map map)
		{
			foreach (Thing thing in map.listerThings.AllThings.Where(x => x is MechSuit))
			{
				yield return thing;
			}
			yield break;
		}

		public static IEnumerable<Thing> MechStations(Map map)
		{
			foreach (Thing thing in map.listerThings.AllThings.Where(x => x is MechStation))
			{
				yield return thing;
			}
			yield break;
		}
	}
}
