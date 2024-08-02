using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MuvLuvAnnihilation
{
    // Token: 0x020000E4 RID: 228
    public class CompProperties_Charger : CompProperties
    {
        // Token: 0x17000117 RID: 279
        // (get) Token: 0x06000624 RID: 1572 RVA: 0x000588FC File Offset: 0x00056AFC
        public float GetChargeChance
        {
            get
            {
                return Mathf.Clamp01(this.Chance);
            }
        }

        // Token: 0x17000118 RID: 280
        // (get) Token: 0x06000625 RID: 1573 RVA: 0x0005891C File Offset: 0x00056B1C
        public float GetExplodingChargeChance
        {
            get
            {
                return Mathf.Clamp01(this.explodingChance);
            }
        }

        // Token: 0x06000626 RID: 1574 RVA: 0x0005893C File Offset: 0x00056B3C
        public CompProperties_Charger()
        {
            this.compClass = typeof(CompCharger);
        }

        // Token: 0x040005DB RID: 1499
        public float RangeMax = 8f;

        // Token: 0x040005DC RID: 1500
        public float RangeMin = 2f;

        // Token: 0x040005DD RID: 1501
        public float Chance = 0.5f;

        // Token: 0x040005DE RID: 1502
        public float ticksBetweenChargeChance = 100f;

        // Token: 0x040005E0 RID: 1504
        public bool targetPawns = true;
        public bool targetBuildings = false;
        public bool exploding = false;

        // Token: 0x040005E1 RID: 1505
        public float explodingChance = 0.2f;

        // Token: 0x040005E2 RID: 1506
        public float explodingRadius = 2f;

        // Token: 0x040005E3 RID: 1507
        public bool textMotes = true;
    }
    // Token: 0x020000E3 RID: 227
    public class CompCharger : ThingComp
    {
        // Token: 0x17000115 RID: 277
        // (get) Token: 0x0600061B RID: 1563 RVA: 0x00058128 File Offset: 0x00056328
        private Pawn Pawn
        {
            get
            {
                Pawn pawn = this.parent as Pawn;
                bool flag = pawn == null;
                bool flag2 = flag;
                if (flag2)
                {
                    Log.Error("pawn is null");
                }
                return pawn;
            }
        }

        // Token: 0x0600061C RID: 1564 RVA: 0x00058160 File Offset: 0x00056360
        public override void CompTick()
        {
            base.CompTick();
            bool spawned = this.Pawn.Spawned;
            if (spawned)
            {
                /*
                bool flag = Find.TickManager.TicksGame % 10 == 0;
                if (flag)
                {
                    bool flag2 = this.Pawn.Downed && !this.Pawn.Dead;
                    if (flag2)
                    {
                        GenExplosion.DoExplosion(this.Pawn.Position, this.Pawn.Map, Rand.Range(this.explosionRadius * 0.5f, this.explosionRadius * 1.5f), DamageDefOf.Stun, this.Pawn, Rand.Range(6, 10), 0f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                    }
                }
                */
                bool flag3 = Find.TickManager.TicksGame % this.nextLeap == 0 && !this.Pawn.Downed && !this.Pawn.Dead;
                if (flag3)
                {
                    LocalTargetInfo a = null;
                    bool flag4 = this.Pawn.CurJob != null && this.Pawn.CurJob.targetA != null;
                    if (flag4)
                    {
                        a = this.Pawn.jobs.curJob.targetA.Thing;
                    }
                    bool flag5 = a != null && a.Thing != null;
                    if (flag5)
                    {
                        Thing thing = a.Thing;
                        bool flag6 = thing is Pawn && thing.Spawned && Props.targetPawns;
                        if (flag6)
                        {
                            float lengthHorizontal = (thing.Position - this.Pawn.Position).LengthHorizontal;
                            bool flag7 = lengthHorizontal <= this.Props.RangeMax && lengthHorizontal > this.Props.RangeMin;
                            if (flag7)
                            {
                                bool flag8 = Rand.Chance(this.Props.GetChargeChance);
                                if (flag8)
                                {
                                    bool flag9 = this.CanHitTargetFrom(this.Pawn.Position, thing);
                                    if (flag9)
                                    {
                                        this.Attack(thing);
                                    }
                                }
                                else
                                {
                                    bool textMotes = this.Props.textMotes;
                                    if (textMotes)
                                    {
                                        bool flag10 = Rand.Chance(0.5f);
                                        if (flag10)
                                        {
                                            MoteMaker.ThrowText(this.Pawn.DrawPos, this.Pawn.Map, "grrr", -1f);
                                        }
                                        else
                                        {
                                            MoteMaker.ThrowText(this.Pawn.DrawPos, this.Pawn.Map, "hsss", -1f);
                                        }
                                    }
                                }
                            }
                        }

                        bool flag6b = thing is Building && thing.Spawned && Props.targetBuildings;
                        if (flag6b)
                        {
                            float lengthHorizontal = (thing.Position - this.Pawn.Position).LengthHorizontal;
                            bool flag7 = lengthHorizontal <= this.Props.RangeMax && lengthHorizontal > this.Props.RangeMin;
                            if (flag7)
                            {
                                bool flag8 = Rand.Chance(this.Props.GetChargeChance);
                                if (flag8)
                                {
                                    bool flag9 = this.CanHitTargetFrom(this.Pawn.Position, thing);
                                    if (flag9)
                                    {
                                        this.Attack(thing);
                                    }
                                }
                                else
                                {
                                    bool textMotes = this.Props.textMotes;
                                    if (textMotes)
                                    {
                                        bool flag10 = Rand.Chance(0.5f);
                                        if (flag10)
                                        {
                                            MoteMaker.ThrowText(this.Pawn.DrawPos, this.Pawn.Map, "grrr", -1f);
                                        }
                                        else
                                        {
                                            MoteMaker.ThrowText(this.Pawn.DrawPos, this.Pawn.Map, "hsss", -1f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x0600061D RID: 1565 RVA: 0x00058610 File Offset: 0x00056810
        public void Attack(LocalTargetInfo target)
        {
            bool flag = target != null && target.Cell != default(IntVec3);
            bool flag2 = flag;
            if (flag2)
            {
                bool flag3 = this.Pawn != null && this.Pawn.Position.IsValid && this.Pawn.Spawned && this.Pawn.Map != null && !this.Pawn.Downed && !this.Pawn.Dead && !target.Thing.DestroyedOrNull();
                if (flag3)
                {
                    this.Pawn.jobs.StopAll(false);
                    ChargingThing chargingThing = (ChargingThing)GenSpawn.Spawn(ThingDef.Named("BETA_DestroyerClassCharge"), this.Pawn.Position, this.Pawn.Map, WipeMode.Vanish);
                    if (target.Thing is Building)
                    {

                        chargingThing.Launch(this.Pawn, target.Cell, this.Pawn, new DamageInfo (DamageDefOf.Blunt, 60f,0.5f, -1f, this.Pawn));
                    }
                    else
                    {

                        chargingThing.Launch(this.Pawn, target.Cell, this.Pawn);
                    }
                }
            }
        }

        // Token: 0x0600061E RID: 1566 RVA: 0x00058718 File Offset: 0x00056918
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.initialized = true;
            Pawn pawn = this.parent as Pawn;
            this.nextLeap = Mathf.RoundToInt(Rand.Range(this.Props.ticksBetweenChargeChance * 0.75f, 1.25f * this.Props.ticksBetweenChargeChance));
            this.explosionRadius = this.Props.explodingRadius * Rand.Range(0.8f, 1.25f);
        }

        // Token: 0x17000116 RID: 278
        // (get) Token: 0x0600061F RID: 1567 RVA: 0x00058794 File Offset: 0x00056994
        public CompProperties_Charger Props
        {
            get
            {
                return (CompProperties_Charger)this.props;
            }
        }

        // Token: 0x06000620 RID: 1568 RVA: 0x000587B1 File Offset: 0x000569B1
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.initialized, "initialized", true, false);
        }

        // Token: 0x06000621 RID: 1569 RVA: 0x000587D0 File Offset: 0x000569D0
        private bool CanHitTargetFrom(IntVec3 pawn, LocalTargetInfo target)
        {
            bool flag = target.IsValid && target.CenterVector3.InBounds(this.Pawn.Map) && !target.Cell.Fogged(this.Pawn.Map) && target.Cell.Walkable(this.Pawn.Map);
            ShootLine shootLine;
            return flag && this.TryFindShootLineFromTo(pawn, target, out shootLine);
        }

        // Token: 0x06000622 RID: 1570 RVA: 0x00058854 File Offset: 0x00056A54
        public bool TryFindShootLineFromTo(IntVec3 root, LocalTargetInfo targ, out ShootLine resultingLine)
        {
            bool flag = targ.HasThing && targ.Thing.Map != this.Pawn.Map;
            bool result;
            if (flag)
            {
                resultingLine = default(ShootLine);
                result = false;
            }
            else
            {
                resultingLine = new ShootLine(root, targ.Cell);
                bool flag2 = !GenSight.LineOfSightToEdges(root, targ.Cell, this.Pawn.Map, true, null);
                result = !flag2;
            }
            return result;
        }

        // Token: 0x040005D8 RID: 1496
        private bool initialized = true;

        // Token: 0x040005D9 RID: 1497
        public float explosionRadius = 2f;

        // Token: 0x040005DA RID: 1498
        private int nextLeap = 0;
    }
}
