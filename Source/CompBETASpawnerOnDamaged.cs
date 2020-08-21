using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace MuvLuvBeta
{
    // Token: 0x0200025A RID: 602 MuvLuvBeta.CompProperties_BETASpawnerOnDamaged
    public class CompProperties_BETASpawnerOnDamaged : CompProperties
    {
        // Token: 0x06000AC8 RID: 2760 RVA: 0x000562D4 File Offset: 0x000546D4
        public CompProperties_BETASpawnerOnDamaged()
        {
            this.compClass = typeof(CompBETASpawnerOnDamaged);
        }
        public List<PawnKindDef> allowedKinddefs = new List<PawnKindDef>();
        public bool oneShot = true;
    }

    // Token: 0x02000769 RID: 1897
    public class CompBETASpawnerOnDamaged : ThingComp
    {
        public CompProperties_BETASpawnerOnDamaged Props => (CompProperties_BETASpawnerOnDamaged)props;

        public Pawn pawn => this.parent as Pawn;
        public Faction Faction => pawn.Faction ?? null;

        public Lord lord => pawn.GetLord() ?? null;

        public bool Spawned = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.Spawned, "SpawnedPawns", false);
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
            if (absorbed)
            {
                return;
            }
            if (dinfo.Def.harmsHealth && !this.Spawned)
            {
                this.TrySpawnPawns();
            }
            absorbed = false;
        }

        // Token: 0x060029EE RID: 10734 RVA: 0x0013DA2C File Offset: 0x0013BE2C
        private void TrySpawnPawns()
        {
            if (!this.parent.Spawned)
            {
                return;
            }
            if (!Props.allowedKinddefs.NullOrEmpty())
            {
                foreach (PawnKindDef kd in Props.allowedKinddefs)
                {
                    if (!(from cell in GenAdj.CellsAdjacent8Way(this.parent)
                          where this.CanSpawnPawnAt(cell)
                          select cell).TryRandomElement(out IntVec3 center))
                    {
                        break;
                    }
                    //    Log.Message(string.Format("kindDef: {0}", kind));
                    PawnGenerationRequest request = new PawnGenerationRequest(kd, Faction, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, false, 1f, false, true, true, false, false, false, false);
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    if (!GenPlace.TryPlaceThing(pawn, center, this.parent.Map, ThingPlaceMode.Near, null, null))
                    {
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
                        break;
                    }
                    this.lord.AddPawn(pawn);
                }
            }
            this.Spawned = true;
            SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera(this.parent.Map);
        }

        // Token: 0x060029EF RID: 10735 RVA: 0x0013DC44 File Offset: 0x0013C044
        private bool CanSpawnPawnAt(IntVec3 c)
        {
            return c.Walkable(this.parent.Map);
        }

    }
}
