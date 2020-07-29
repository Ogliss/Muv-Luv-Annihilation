using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace MuvLuvBeta.HarmonyInstance
{
    [StaticConstructorOnStartup]
    class MainHarmonyInstance
    {
        static MainHarmonyInstance()
        {
            var harmony = new Harmony("com.ogliss.rimworld.mod.MuvLuvBeta");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }


        public static Vector3 ThrowResult(Thing Caster, Thing thingToPush, int pushDist, out bool collision)
        {
            Vector3 vector = GenThing.TrueCenter(thingToPush);
            Vector3 result = vector;
            bool flag = false;
            int num = pushDist;
            int num2 = pushDist;
            if (vector.x > GenThing.TrueCenter(Caster).x)
            {
                num2 = -num2;
            }
            if (vector.z > GenThing.TrueCenter(Caster).z)
            {
                num = -num;
            }
            if (vector.x == GenThing.TrueCenter(Caster).x)
            {
                num2 = 0;
            }
            if (vector.z == GenThing.TrueCenter(Caster).z)
            {
                num = 0;
            }
            
            num += Rand.RangeInclusive(-pushDist, pushDist) / (num != 0 ? 3 : 2);
            num2 += Rand.RangeInclusive(-pushDist, pushDist) / (num2 != 0 ? 3 : 2);
            
            Vector3 vector2 = new Vector3(vector.x + (float)num2, 0f, vector.z + (float)num);
            if (GenGrid.Standable(IntVec3Utility.ToIntVec3(vector2), Caster.Map))
            {
                result = vector2;
            }
            else
            {
                if (thingToPush is Pawn)
                {
                    flag = true;
                }
            }
            collision = flag;
            return result;
        }

        public static void ThrowEffect(Thing Caster, Thing target, int distance, bool damageOnCollision = false)
        {
            if (target is Building)
            {
                return;
            }
            Pawn pawn;
            if (target != null && (pawn = (target as Pawn)) != null && pawn.Spawned && !pawn.Downed && !pawn.Dead && (pawn?.MapHeld) != null)
            {
                bool drafted = pawn.Drafted;
                Vector3 vector = MainHarmonyInstance.ThrowResult(Caster, target, distance, out bool flag2);
                MLB_ThrownObject flyingObject = (MLB_ThrownObject)GenSpawn.Spawn(ThingDef.Named("BETA_GrapplerClassThrown"), pawn.PositionHeld, pawn.MapHeld, 0);
                bool flag3 = flag2 & damageOnCollision;
                if (flag3)
                {
                    Rand.PushState();
                    flyingObject.Launch(Caster, new LocalTargetInfo(IntVec3Utility.ToIntVec3(vector)), target, new DamageInfo?(new DamageInfo(DamageDefOf.Blunt, (float)Rand.Range(8, 10), 0f, -1f, null, null, null, 0, null)));
                    Rand.PopState();
                }
                else
                {
                    flyingObject.Launch(Caster, new LocalTargetInfo(IntVec3Utility.ToIntVec3(vector)), target);
                }

            }
        }
    }


}