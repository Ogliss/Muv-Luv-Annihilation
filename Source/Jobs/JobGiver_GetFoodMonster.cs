using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MuvLuvAnnihilation
{
    public class JobGiver_GetFoodMonster : ThinkNode_JobGiver
    {
        private HungerCategory minCategory;

        private float maxLevelPercentage = 1f;

        public bool forceScanWholeMap;
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_GetFoodMonster obj = (JobGiver_GetFoodMonster)base.DeepCopy(resolve);
            obj.minCategory = minCategory;
            obj.maxLevelPercentage = maxLevelPercentage;
            obj.forceScanWholeMap = forceScanWholeMap;
            return obj;
        }
        protected override Job TryGiveJob(Pawn pawn)
        {
            bool allowCorpse = true;
            bool desperate = pawn.needs.food.CurCategory == HungerCategory.Starving;

            var foodSource = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999,
                (Thing x) => x is Pawn victim && victim.RaceProps.Humanlike && victim.Downed && pawn.CanReserve(victim));
            ThingDef foodDef = (foodSource as Pawn)?.Corpse?.def;

            if (foodSource is null && !FoodUtility.TryFindBestFoodSourceFor_NewTemp(pawn, pawn, desperate, out foodSource, out foodDef, canRefillDispenser: true, canUseInventory: true, allowCorpse, pawn.IsWildMan(), forceScanWholeMap))
            {
                return null;
            }

            Pawn pawn2 = foodSource as Pawn;
            if (pawn2 != null && !pawn2.Downed)
            {
                Job job = JobMaker.MakeJob(JobDefOf.PredatorHunt, pawn2);
                job.killIncappedTarget = true;
                return job;
            }

            float nutrition = FoodUtility.GetNutrition(pawn, foodSource, foodDef);
            Job job3 = JobMaker.MakeJob(BETADefOf.BETA_Ingest, foodSource);
            if (pawn2 != null)
            {
                job3.count = 1;
            }
            else
            {
                job3.count = FoodUtility.WillIngestStackCountOf(pawn, foodDef, nutrition);
            }
            return job3;
        }
    }
}