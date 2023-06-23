

// CrashedShipsExtension.IncidentWorker_CrashedShipPart
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

public class IncidentWorker_CrashedShipPart : IncidentWorker
{
    private const float ShipPointsFactor = 0.9f;

    private const int IncidentMinimumPoints = 300;

    private const float DefendRadius = 28f;

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (((Map)parms.target).listerThings.ThingsOfDef(def?.mechClusterBuilding).Count > 0)
        {
            return false;
        }
        return true;
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        List<TargetInfo> list = new List<TargetInfo>();
        List<Thing> list2 = ThingSetMakerDefOf.Meteorite.root.Generate();
        ThingDef shipPartDef = def?.mechClusterBuilding;
        IntVec3 intVec = FindDropPodLocation(map, (IntVec3 spot) => CanPlaceAt(spot));
        if (intVec == IntVec3.Invalid)
        {
            return false;
        }
        float points = Mathf.Max(parms.points * 0.9f, 300f);
        List<Pawn> list3 = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms
        {
            groupKind = PawnGroupKindDefOf.Combat,
            tile = map.Tile,
            faction = Faction.OfMechanoids,
            points = points
        }).ToList();
        Thing thing = ThingMaker.MakeThing(shipPartDef);
        thing.SetFaction(Faction.OfMechanoids);
        LordMaker.MakeNewLord(Faction.OfMechanoids, new LordJob_SleepThenMechanoidsDefend(new List<Thing> { thing }, Faction.OfMechanoids, 28f, intVec, canAssaultColony: false, isMechCluster: false), map, list3);
        DropPodUtility.DropThingsNear(intVec, map, list3.Cast<Thing>(), 110, false, false, true, true);
        foreach (Pawn item in list3)
        {
            item.TryGetComp<CompCanBeDormant>()?.ToSleep();
        }
        list.AddRange(list3.Select((Pawn p) => new TargetInfo(p)));
        GenSpawn.Spawn(SkyfallerMaker.MakeSkyfaller(ThingDefOf.MeteoriteIncoming, thing), intVec, map);
        list.Add(new TargetInfo(intVec, map));
        SendStandardLetter(parms, list);
        return true;
        bool CanPlaceAt(IntVec3 loc)
        {
            CellRect cellRect = GenAdj.OccupiedRect(loc, Rot4.North, shipPartDef.Size);
            if (loc.Fogged(map) || !cellRect.InBounds(map))
            {
                return false;
            }
            if (!DropCellFinder.SkyfallerCanLandAt(loc, map, shipPartDef.Size))
            {
                return false;
            }
            foreach (IntVec3 item2 in cellRect)
            {
                if (item2.GetRoof(map)?.isNatural ?? false)
                {
                    return false;
                }
            }
            return GenConstruct.CanBuildOnTerrain(shipPartDef, loc, map, Rot4.North);
        }
    }

    private static IntVec3 FindDropPodLocation(Map map, Predicate<IntVec3> validator)
    {
        for (int i = 0; i < 200; i++)
        {
            IntVec3 intVec = RCellFinder.FindSiegePositionFrom(DropCellFinder.FindRaidDropCenterDistant(map, true), map, true);
            if (validator(intVec))
            {
                return intVec;
            }
        }
        return IntVec3.Invalid;
    }
}
