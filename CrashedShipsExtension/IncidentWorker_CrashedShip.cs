using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CrashedShipsExtension
{
	// Token: 0x02000009 RID: 9
	public class IncidentWorker_CrashedShip : IncidentWorker
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00003F6C File Offset: 0x0000216C
		protected virtual int CountToSpawn
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003F80 File Offset: 0x00002180
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			return map.listerThings.ThingsOfDef(this.def.mechClusterBuilding).Count <= 0;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003FC0 File Offset: 0x000021C0
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			int num = 0;
			int countToSpawn = this.CountToSpawn;
			List<TargetInfo> list = new List<TargetInfo>();
			Rand.PushState();
			float num2 = Rand.Range(0f, 360f);
			Rand.PopState();
			Faction faction = null;
			Building building = (Building)ThingMaker.MakeThing(this.def.mechClusterBuilding, null);
			bool flag = faction == null;
			if (flag)
			{
				faction = building.GetComp<CompSpawnerOnDamaged>().OfFaction;
			}
			for (int i = 0; i < countToSpawn; i++)
			{
				ThingDef thingDef = building.GetComp<CompSpawnerOnDamaged>().Props.skyFaller ?? ThingDefOf.CrashedShipPartIncoming;
				IntVec3 intVec;
				bool flag2 = !CellFinderLoose.TryFindSkyfallerCell(thingDef, map, out intVec, 14, default(IntVec3), -1, false, true, true, true, true, false, null);
				if (flag2)
				{
					break;
				}
				building.SetFaction(faction, null);
				building.GetComp<CompSpawnerOnDamaged>().pointsLeft = Mathf.Max(parms.points * 0.9f, 300f);
				Skyfaller skyfaller = SkyfallerMaker.MakeSkyfaller(thingDef, building);
				skyfaller.shrapnelDirection = num2;
				GenSpawn.Spawn(skyfaller, intVec, map, WipeMode.Vanish);
				num++;
				list.Add(new TargetInfo(intVec, map, false));
			}
			bool flag3 = num > 0;
			if (flag3)
			{
				base.SendStandardLetter(parms, list, Array.Empty<NamedArgument>());
			}
			return num > 0;
		}

		// Token: 0x0400003C RID: 60
		private const float ShipPointsFactor = 0.9f;

		// Token: 0x0400003D RID: 61
		private const int IncidentMinimumPoints = 300;
	}
}
