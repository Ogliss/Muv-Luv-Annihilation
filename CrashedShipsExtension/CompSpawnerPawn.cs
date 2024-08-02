

// CrashedShipsExtension.CompSpawnerPawn
using System;
using System.Collections.Generic;
using System.Linq;
using CrashedShipsExtension;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

public class CompSpawnerPawn : ThingComp
{
    public int nextPawnSpawnTick = -1;

    public int initialSpawnDelay = -1;

    public int pawnsLeftToSpawn = -1;

    public List<Pawn> spawnedPawns = new List<Pawn>();

    public List<PawnGenOption> spawnablePawnKinds = new List<PawnGenOption>();

    public bool aggressive = true;

    public bool canSpawnPawns = true;

    private PawnKindDef chosenKind;

    private CompCanBeDormant dormancyCompCached;

    private CrashedShipsExtension.CompProperties_SpawnerPawn Props => (CrashedShipsExtension.CompProperties_SpawnerPawn)props;

    public Lord Lord => FindLordToJoin(parent, Props.lordJob, Props.shouldJoinParentLord);

    private float SpawnedPawnsPoints
    {
        get
        {
            FilterOutUnspawnedPawns();
            float num = 0f;
            for (int i = 0; i < spawnedPawns.Count; i++)
            {
                num += spawnedPawns[i].kindDef.combatPower;
            }
            return num;
        }
    }

    public bool Active => pawnsLeftToSpawn != 0 && !Dormant;

    public CompCanBeDormant DormancyComp
    {
        get
        {
            CompCanBeDormant result;
            if ((result = dormancyCompCached) == null)
            {
                result = (dormancyCompCached = parent.TryGetComp<CompCanBeDormant>());
            }
            return result;
        }
    }

    public bool Dormant => DormancyComp != null && !DormancyComp.Awake;

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        if (chosenKind == null)
        {
            chosenKind = RandomPawnKindDef();
        }
        if (Props.maxPawnsToSpawn != IntRange.zero)
        {
            pawnsLeftToSpawn = Props.maxPawnsToSpawn.RandomInRange;
        }
    }

    public static Lord FindLordToJoin(Thing spawner, Type lordJobType, bool shouldTryJoinParentLord, Func<Thing, List<Pawn>> spawnedPawnSelector = null)
    {
        if (spawner.Spawned)
        {
            if (shouldTryJoinParentLord)
            {
                Lord lord = (spawner as Building)?.GetLord();
                if (lord != null)
                {
                    return lord;
                }
            }
            if (spawnedPawnSelector == null)
            {
                spawnedPawnSelector = (Thing s) => s.TryGetComp<CompSpawnerPawn>()?.spawnedPawns;
            }
            Predicate<Pawn> hasJob = delegate (Pawn x)
            {
                Lord lord2 = x.GetLord();
                return lord2 != null && lord2.LordJob.GetType() == lordJobType;
            };
            Pawn foundPawn = null;
            RegionTraverser.BreadthFirstTraverse(spawner.OccupiedRect().AdjacentCells.RandomElement().GetRegion(spawner.Map, RegionType.Normal | RegionType.Portal), (Region from, Region to) => true, delegate (Region r)
            {
                List<Thing> list = r.ListerThings.ThingsOfDef(spawner.def);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Faction == spawner.Faction)
                    {
                        List<Pawn> list2 = spawnedPawnSelector(list[i]);
                        if (list2 != null)
                        {
                            foundPawn = list2.Find(hasJob);
                        }
                        if (foundPawn != null)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }, 40, RegionType.Normal | RegionType.Portal);
            if (foundPawn != null)
            {
                return foundPawn.GetLord();
            }
        }
        return null;
    }

    public static Lord CreateNewLord(Thing byThing, bool aggressive, float defendRadius, Type lordJobType)
    {
        if (!CellFinder.TryFindRandomCellNear(byThing.Position, byThing.Map, 5, (IntVec3 c) => c.Standable(byThing.Map) && byThing.Map.reachability.CanReach(c, byThing, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)), out var result))
        {
            Log.Error("Found no place for pawns to defend " + byThing);
            result = IntVec3.Invalid;
        }
        return LordMaker.MakeNewLord(byThing.Faction, Activator.CreateInstance(lordJobType, new SpawnedPawnParams
        {
            aggressive = aggressive,
            defendRadius = defendRadius,
            defSpot = result,
            spawnerThing = byThing
        }) as LordJob, byThing.Map);
    }

    private void SpawnInitialPawns()
    {
        Pawn pawn;
        for (int i = 0; i < Props.initialPawnsCount; i++)
        {
            if (!TrySpawnPawn(out pawn))
            {
                break;
            }
        }
        SpawnPawnsUntilPoints(Props.initialPawnsPoints);
        if (!Props.AlwaysSpawnWith.NullOrEmpty())
        {
            foreach (PawnKindDef item in Props.AlwaysSpawnWith)
            {
                TrySpawnPawn(out pawn);
            }
        }
        CalculateNextPawnSpawnTick();
    }

    public void SpawnPawnsUntilPoints(float points)
    {
        int num = 0;
        while (SpawnedPawnsPoints < points)
        {
            num++;
            if (num > 1000)
            {
                Log.Error("Too many iterations.");
                break;
            }
            if (!TrySpawnPawn(out var _))
            {
                break;
            }
        }
        CalculateNextPawnSpawnTick();
    }

    private void CalculateNextPawnSpawnTick()
    {
        CalculateNextPawnSpawnTick(Props.pawnSpawnIntervalDays.RandomInRange * 60000f);
    }

    public void CalculateNextPawnSpawnTick(float delayTicks)
    {
        float num = GenMath.LerpDouble(0f, 5f, 1f, 0.5f, spawnedPawns.Count);
        nextPawnSpawnTick = Find.TickManager.TicksGame + (int)(delayTicks / (num * ((DifficultyDef)(object)Find.Storyteller.difficulty).enemyReproductionRateFactor));
    }

    private void FilterOutUnspawnedPawns()
    {
        for (int num = spawnedPawns.Count - 1; num >= 0; num--)
        {
            if (!spawnedPawns[num].Spawned)
            {
                spawnedPawns.RemoveAt(num);
            }
        }
    }

    private PawnKindDef RandomPawnKindDef()
    {
        float curPoints = SpawnedPawnsPoints;
        if (spawnablePawnKinds.NullOrEmpty())
        {
            if (!Props.spawnablePawnKinds.NullOrEmpty())
            {
                spawnablePawnKinds = Props.spawnablePawnKinds;
            }
            else if (parent.Faction != null)
            {
                if (parent.Faction.def.pawnGroupMakers.Any((PawnGroupMaker x) => x.kindDef == Props.factionGroupKindDef))
                {
                    spawnablePawnKinds = parent.Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == Props.factionGroupKindDef).RandomElementByWeight((PawnGroupMaker x) => x.commonality).options;
                }
                else
                {
                    spawnablePawnKinds = parent.Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == PawnGroupKindDefOf.Combat || x.kindDef == PawnGroupKindDefOf.Settlement).RandomElementByWeight((PawnGroupMaker x) => x.commonality).options;
                }
            }
        }
        IEnumerable<PawnGenOption> source = spawnablePawnKinds;
        if (Props.maxSpawnedPawnsPoints > -1f)
        {
            source = source.Where((PawnGenOption x) => curPoints + x.kind.combatPower <= Props.maxSpawnedPawnsPoints);
        }
        if (source.TryRandomElementByWeight((PawnGenOption x) => x.selectionWeight, out var result))
        {
            return result.kind;
        }
        return null;
    }

    private bool TrySpawnPawn(out Pawn pawn)
    {
        if (!canSpawnPawns)
        {
            pawn = null;
            return false;
        }
        if (!Props.chooseSingleTypeToSpawn)
        {
            chosenKind = RandomPawnKindDef();
        }
        if (chosenKind == null)
        {
            pawn = null;
            return false;
        }
        int index = chosenKind.lifeStages.Count - 1;
        pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(chosenKind, parent.Faction, PawnGenerationContext.NonPlayer, -1));
        spawnedPawns.Add(pawn);
        GenSpawn.Spawn(pawn, parent.OccupiedRect().AdjacentCells.RandomElement(), parent.Map);
        Lord lord = Lord;
        if (lord == null)
        {
            lord = CreateNewLord(parent, aggressive, Props.defendRadius, Props.lordJob);
        }
        lord.AddPawn(pawn);
        if (Props.spawnSound != null)
        {
            Props.spawnSound.PlayOneShot(parent);
        }
        if (pawnsLeftToSpawn > 0)
        {
            pawnsLeftToSpawn--;
        }
        SendMessage();
        return true;
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (respawningAfterLoad || !Active || nextPawnSpawnTick != -1)
        {
        }
    }

    public override void PostPostMake()
    {
        base.PostPostMake();
        initialSpawnDelay = Props.initialSpawnDelay;
    }

    public override void CompTick()
    {
        if (parent.Map == null)
        {
            return;
        }
        if (initialSpawnDelay > -1)
        {
            initialSpawnDelay--;
        }
        if (Active && parent.Spawned && initialSpawnDelay == 0)
        {
            SpawnInitialPawns();
        }
        if (!parent.Spawned || initialSpawnDelay != -1)
        {
            return;
        }
        FilterOutUnspawnedPawns();
        if (Active && Find.TickManager.TicksGame >= nextPawnSpawnTick)
        {
            if ((Props.maxSpawnedPawnsPoints < 0f || SpawnedPawnsPoints < Props.maxSpawnedPawnsPoints) && TrySpawnPawn(out var pawn) && pawn.caller != null)
            {
                pawn.caller.DoCall();
            }
            CalculateNextPawnSpawnTick();
        }
    }

    public void SendMessage()
    {
        if (!Props.spawnMessageKey.NullOrEmpty() && MessagesRepeatAvoider.MessageShowAllowed(Props.spawnMessageKey, 0.1f))
        {
            Messages.Message(Props.spawnMessageKey.Translate(), parent, MessageTypeDefOf.NegativeEvent);
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (Prefs.DevMode)
        {
            yield return new Command_Action
            {
                defaultLabel = "DEBUG: Spawn pawn",
                icon = TexCommand.ReleaseAnimals,
                action = delegate
                {
                    TrySpawnPawn(out var _);
                }
            };
        }
    }

    public override string CompInspectStringExtra()
    {
        if (!Props.showNextSpawnInInspect || nextPawnSpawnTick <= 0 || chosenKind == null)
        {
            return null;
        }
        if (pawnsLeftToSpawn == 0 && !Props.noPawnsLeftToSpawnKey.NullOrEmpty())
        {
            return Props.noPawnsLeftToSpawnKey.Translate();
        }
        string text;
        if (!Dormant)
        {
            text = (Props.nextSpawnInspectStringKey ?? "SpawningNextPawnIn").Translate(chosenKind.LabelCap, (nextPawnSpawnTick - Find.TickManager.TicksGame).ToStringTicksToDays());
        }
        else
        {
            if (Props.nextSpawnInspectStringKeyDormant == null)
            {
                return null;
            }
            text = Props.nextSpawnInspectStringKeyDormant.Translate() + ": " + chosenKind.LabelCap;
        }
        if (pawnsLeftToSpawn > 0 && !Props.pawnsLeftToSpawnKey.NullOrEmpty())
        {
            text = string.Concat(text + ("\n" + Props.pawnsLeftToSpawnKey.Translate() + ": "), pawnsLeftToSpawn.ToString());
        }
        return text;
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref nextPawnSpawnTick, "nextPawnSpawnTick", 0);
        Scribe_Values.Look(ref initialSpawnDelay, "initialSpawnDelay", 0);
        Scribe_Values.Look(ref pawnsLeftToSpawn, "pawnsLeftToSpawn", -1);
        Scribe_Collections.Look(ref spawnedPawns, "spawnedPawns", LookMode.Reference);
        Scribe_Values.Look(ref aggressive, "aggressive", defaultValue: false);
        Scribe_Values.Look(ref canSpawnPawns, "canSpawnPawns", defaultValue: true);
        Scribe_Defs.Look(ref chosenKind, "chosenKind");
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            spawnedPawns.RemoveAll((Pawn x) => x == null);
            if (pawnsLeftToSpawn == -1 && Props.maxPawnsToSpawn != IntRange.zero)
            {
                pawnsLeftToSpawn = Props.maxPawnsToSpawn.RandomInRange;
            }
        }
    }
}
