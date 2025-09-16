using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace MuvLuvAnnihilation.HarmonyInstance
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


            /*
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
            */

            //    Vector3 vector2 = new Vector3(vector.x + (float)num2, 0f, vector.z + (float)num);

            Pawn caster = Caster as Pawn;
            float num = 0f;
            if ((thingToPush.DrawPos - caster.DrawPos).MagnitudeHorizontalSquared() > 0.001f)
            {
                num = (thingToPush.DrawPos - caster.DrawPos).AngleFlat();
            }

            IntVec3 vector2 = GenerateShrapnelLocation(caster.Position, num, pushDist);
            if (RCellFinder.TryFindRandomCellNearWith(vector2, x=> GenSight.LineOfSight(caster.Position, x, Caster.Map) && GenGrid.Standable(x, Caster.Map), Caster.Map, out vector2))
            {
                result = vector2.ToVector3Shifted();
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

        // Token: 0x06004F42 RID: 20290 RVA: 0x001AB178 File Offset: 0x001A9378
        private static IntVec3 GenerateShrapnelLocation(IntVec3 center, float angleOffset, float distanceFactor)
        {
            float num = MainHarmonyInstance.ShrapnelAngleDistribution.Evaluate(Rand.Value);
            float d = MainHarmonyInstance.ShrapnelDistanceFromAngle.Evaluate(num) * Rand.Value + distanceFactor;
            return (Vector3Utility.HorizontalVectorFromAngle(num + angleOffset) * d).ToIntVec3() + center;
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

                IntVec3 cell = pawn.Position;
                IntVec3 tcell = IntVec3Utility.ToIntVec3(vector);

                if (ModLister.RoyaltyInstalled)
                {
                    PawnThrown pawnFlyer = PawnThrown.MakeThrown(ThingDef.Named("PawnThrown"), pawn, tcell, new DamageInfo?(new DamageInfo(DamageDefOf.Blunt, (float)Rand.Range(8, 10), 0f, -1f, null, null, null, 0, null)));
                    if (pawnFlyer != null)
                    {
                        GenSpawn.Spawn(pawnFlyer, tcell, Caster.Map, WipeMode.Vanish);
                    }

                }
                else
                {
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

        // Token: 0x04002C7B RID: 11387
        private static readonly SimpleCurve ShrapnelDistanceFromAngle = new SimpleCurve
        {
            {
                new CurvePoint(0f, 6f),
                true
            },
            {
                new CurvePoint(90f, 4f),
                true
            },
            {
                new CurvePoint(135f, 4f),
                true
            },
            {
                new CurvePoint(180f, 30f),
                true
            },
            {
                new CurvePoint(225f, 4f),
                true
            },
            {
                new CurvePoint(270f, 4f),
                true
            },
            {
                new CurvePoint(360f, 6f),
                true
            }
        };

        // Token: 0x04002C7C RID: 11388
        private static readonly SimpleCurve ShrapnelAngleDistribution = new SimpleCurve
        {
            {
                new CurvePoint(0f, 0f),
                true
            },
            {
                new CurvePoint(0.1f, 90f),
                true
            },
            {
                new CurvePoint(0.25f, 135f),
                true
            },
            {
                new CurvePoint(0.5f, 180f),
                true
            },
            {
                new CurvePoint(0.75f, 225f),
                true
            },
            {
                new CurvePoint(0.9f, 270f),
                true
            },
            {
                new CurvePoint(1f, 360f),
                true
            }
        };
    }


}