using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation
{
    [HarmonyPatch(typeof(Pawn_JobTracker), "EndCurrentJob")]
    public class EndCurrentJobPatch
    {
        private static void Prefix(Pawn_JobTracker __instance, Pawn ___pawn, out Pawn __state, JobCondition condition, ref bool startNewJob, bool canReturnToPool = true)
        {
            if (___pawn is BetaEater)
            {
                if (___pawn.jobs.curDriver is JobDriver_AttackMelee || ___pawn.jobs.curDriver is JobDriver_AttackStatic)
                {
                    __state = ___pawn.jobs.curJob.targetA.Pawn;
                    startNewJob = false;
                }
                else
                {
                    __state = null;
                }
            }
            else
            {
                __state = null;
            }
        }
        private static void Postfix(Pawn_JobTracker __instance, Pawn ___pawn, Pawn __state, JobCondition condition, ref bool startNewJob, bool canReturnToPool = true)
        {
            if (___pawn.needs?.food != null && ___pawn.needs.food.CurLevel < 1f && __state != null && __state.RaceProps.IsFlesh && (__state.Corpse != null || __state.Downed))
            {
                Thing target = __state.Corpse != null ? __state.Corpse as Thing : __state;
                if (___pawn.CanReserveAndReach(target, PathEndMode.ClosestTouch, Danger.Deadly))
                {
                    var job = JobMaker.MakeJob(BETADefOf.BETA_Ingest, target);
                    job.count = 1;
                    ___pawn.jobs.StartJob(job);
                }
            }
        }
    }
}
