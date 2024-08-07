﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using UnityEngine;
using System.Reflection;

namespace MuvLuvAnnihilation.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
    public static class Verb_Shoot_TryCastShot_Scattershot_Patch
    {
        [HarmonyPrefix]
        public static bool TryCastShot_Prefix(ref Verb_Shoot __instance, LocalTargetInfo ___currentTarget, bool ___canHitNonTargetPawnsNow)
        {
            if (__instance?.verbProps?.defaultProjectile != null)
            {
                int ScattershotCount = __instance.verbProps.defaultProjectile.HasModExtension<ProjectileDefExtension>() ? __instance.verbProps.defaultProjectile.GetModExtension<ProjectileDefExtension>().extraProjectiles : 0;
                if (ScattershotCount > 0)
                {
                    //    Log.Message(string.Format("AllowMultiShot: {0} Projectile Count: {1}", AMASettings.Instance.AllowMultiShot && Multishot, ScattershotCount));
                    for (int i = 0; i < ScattershotCount; i++)
                    {
                        //    Log.Message(string.Format("Launching extra projectile {0} / {1}", i+1, ScattershotCount));
                        //    AccessTools.Method(typeof(Verb_Shoot).BaseType, "TryCastShot", null, null).Invoke(__instance, null);
                        //    Traverse.Create(__instance).Method("TryCastShot", null).GetValue();
                        TryCastExtraShot(ref __instance, ___currentTarget, ___canHitNonTargetPawnsNow);
                    }
                }
            }
            return true;
        }

        // Token: 0x0600651E RID: 25886 RVA: 0x001B8BC0 File Offset: 0x001B6FC0
        public static bool TryCastExtraShot(ref Verb_Shoot __instance, LocalTargetInfo currentTarget, bool canHitNonTargetPawnsNow)
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != __instance.caster.Map)
            {
                return false;
            }
            ThingDef projectile = __instance.Projectile;
            if (projectile == null)
            {
                return false;
            }
            ShootLine shootLine;
            bool flag = __instance.TryFindShootLineFromTo(__instance.caster.Position, currentTarget, out shootLine);
            if (__instance.verbProps.stopBurstWithoutLos && !flag)
            {
                return false;
            }
            if (__instance.EquipmentSource != null)
            {
                CompChangeableProjectile comp = __instance.EquipmentSource.GetComp<CompChangeableProjectile>();
                if (comp != null)
                {
                    comp.Notify_ProjectileLaunched();
                }
            }
            Thing launcher = __instance.caster;
            Thing equipment = __instance.EquipmentSource;
            CompMannable compMannable = __instance.caster.TryGetComp<CompMannable>();
            if (compMannable != null && compMannable.ManningPawn != null)
            {
                launcher = compMannable.ManningPawn;
                equipment = __instance.caster;
            }
            Vector3 drawPos = __instance.caster.DrawPos;
            Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, __instance.caster.Map, WipeMode.Vanish);
            if (__instance.verbProps.ForcedMissRadius > 0.5f)
            {
                float num = VerbUtility.CalculateAdjustedForcedMiss(__instance.verbProps.ForcedMissRadius, currentTarget.Cell - __instance.caster.Position);
                if (num > 0.5f)
                {
                    int max = GenRadial.NumCellsInRadius(num);
                    Rand.PushState();
                    int num2 = Rand.Range(0, max);
                    Rand.PopState();
                    if (num2 > 0)
                    {
                        IntVec3 c = currentTarget.Cell + GenRadial.RadialPattern[num2];

                        ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                        Rand.PushState();
                        if (Rand.Chance(0.5f))
                        {
                            projectileHitFlags = ProjectileHitFlags.All;
                        }
                        Rand.PopState();
                        if (!canHitNonTargetPawnsNow)
                        {
                            projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
                        }
                        projectile2.Launch(launcher, drawPos, c, currentTarget, projectileHitFlags, false, equipment, null);
                        return true;
                    }
                }
            }
            ShotReport shotReport = ShotReport.HitReportFor(__instance.caster, __instance, currentTarget);
            Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
            ThingDef targetCoverDef = (randomCoverToMissInto == null) ? null : randomCoverToMissInto.def;
            Rand.PushState();
            bool chance = Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture);
            Rand.PopState();
            if (!chance)
            {
                bool flyOverhead = projectile2?.def?.projectile != null && projectile2.def.projectile.flyOverhead;
                shootLine.ChangeDestToMissWild_NewTemp(shotReport.AimOnTargetChance_StandardTarget, flyOverhead, __instance.caster.Map);
                ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                Rand.PushState();
                if (Rand.Chance(0.5f) && canHitNonTargetPawnsNow)
                {
                    projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                }
                Rand.PopState();
                projectile2.Launch(launcher, drawPos, shootLine.Dest, currentTarget, projectileHitFlags2, false, equipment, targetCoverDef);
                return true;
            }
            Rand.PushState();
            chance = Rand.Chance(shotReport.PassCoverChance);
            Rand.PopState();
            if (currentTarget.Thing != null && currentTarget.Thing.def.category == ThingCategory.Pawn && !chance)
            {

                ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                if (canHitNonTargetPawnsNow)
                {
                    projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
                }
                projectile2.Launch(launcher, drawPos, randomCoverToMissInto, currentTarget, projectileHitFlags3, false, equipment, targetCoverDef);
                return true;
            }
            ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
            if (canHitNonTargetPawnsNow)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
            }
            if (!currentTarget.HasThing || currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
            }
            if (currentTarget.Thing != null)
            {
                projectile2.Launch(launcher, drawPos, currentTarget, currentTarget, projectileHitFlags4, false, equipment, targetCoverDef);
            }
            else
            {
                projectile2.Launch(launcher, drawPos, shootLine.Dest, currentTarget, projectileHitFlags4, false, equipment, targetCoverDef);
            }
            return true;
        }
    }

}
