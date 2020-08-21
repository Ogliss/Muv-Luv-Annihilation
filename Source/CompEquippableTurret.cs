using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MuvLuvBeta
{
    public class CompProperties_EquippableTurret : CompProperties
    {
        public CompProperties_EquippableTurret()
        {
            this.compClass = typeof(CompEquippableTurret);
        }
        public bool OnByDefault = true;
        public bool DisableInMelee = true;
        public ThingDef TurretDef = null;
        public string iconPath;
    }

    // Token: 0x02000E58 RID: 3672
    public class CompEquippableTurret : ThingComp
    {
        public CompProperties_EquippableTurret Props => (CompProperties_EquippableTurret)props;

        public Apparel Apparel => this.parent as Apparel;

        public bool IsWorn => Apparel != null ? Apparel.Wearer != null : false;

        public Pawn Wearer => IsWorn ? Apparel.Wearer : null;

        public Map Map
        {
            get
            {
                Map map = null;
                if (IsWorn)
                {
                    map = Wearer.Map ?? Wearer.MapHeld;
                }
                else
                {
                    map = Apparel.Map ?? Apparel.MapHeld;
                }

                return map;
            }
        }

        public IntVec3 Loc
        {
            get
            {
                IntVec3 vec3 = IntVec3.Invalid;
                if (IsWorn)
                {
                    vec3 = Wearer.Position != null ? Wearer.Position : Wearer.PositionHeld;
                }
                else
                {
                    vec3 = Apparel.Position != null ? Apparel.Position : Apparel.PositionHeld;
                }

                return vec3;
            }
        }
        public void ThrowMote(string str, float time = 5f)
        {
            if (!str.NullOrEmpty())
            {
                if (Map != null && Loc != IntVec3.Invalid)
                {
                    IntVec3 vec3 = Loc;
                    //    vec3.x -= 1;
                    vec3.z += 1;
                    MoteMaker.ThrowText(vec3.ToVector3ShiftedWithAltitude(AltitudeLayer.MoteOverhead), Map, str, time);
                }
            }
        }

        protected virtual Pawn GetWearer
        {
            get
            {
                if (ParentHolder != null && ParentHolder is Pawn_ApparelTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else if (ParentHolder != null && ParentHolder is Pawn_EquipmentTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else
                {
                    return null;
                }
            }
        }


        public bool turretIsOn
        {
            get
            {
                if (!IsWorn)
                {
                    return false;
                }
                if (GetWearer.Dead || GetWearer.Downed || GetWearer.IsPrisoner)
                {
                    return false;
                }

                if (GetWearer.mindState.MeleeThreatStillThreat && DisableInMelee)
                {
                    return false;
                }
                return GetWearer.Awake() && Toggled;
            }
        }

        public bool Toggled;
        public bool DisableInMelee = true;

        public bool onDefault
        {
            get
            {
                return Props.OnByDefault;
            }
        }
        public bool MeleeDisable
        {
            get
            {
                return Props.DisableInMelee;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            //    Scribe_Deep.Look<Thing>(ref this.turret, "TurretThing", false);

            Scribe_Values.Look(ref Toggled, "ToggledMode", onDefault, true);
            Scribe_Values.Look(ref DisableInMelee, "DisableInMelee", MeleeDisable, true);
            Scribe_Values.Look(ref id, "id", 0, true);
            //    Scribe_Values.Look<bool>(ref this.turretIsOn, "TurretIsOn", IsTurnedOn);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (IsWorn && GetWearer.Map != null)
            {
                if (this.turretIsOn || Find.TickManager.TicksGame >= this.nextUpdateTick)
                {
                    this.nextUpdateTick = Find.TickManager.TicksGame + 60;
                    this.RefreshTurretState();
                }
            }
        }

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            parent.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                id = Rand.RangeInclusive(0, 99999);
                Rand.PushState();
                this.nextUpdateTick = Find.TickManager.TicksGame + Rand.Range(0, 60);
                Rand.PopState();
            }
        }

        // Token: 0x06000004 RID: 4 RVA: 0x000020EE File Offset: 0x000002EE
        public void RefreshTurretState()
        {
            if (this.ComputeTurretState())
            {
                this.SwitchOnTurret();
                return;
            }
            this.SwitchOffTurret();
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002108 File Offset: 0x00000308
        public bool ComputeTurretState()
        {
            return GetWearer != null && !GetWearer.Dead && !GetWearer.Downed && GetWearer.Awake() && (this.turretIsOn);
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000021F0 File Offset: 0x000003F0
        public void SwitchOnTurret()
        {
            IntVec3 intVec = GetWearer.DrawPos.ToIntVec3();
            if (!this.turret.DestroyedOrNull() && intVec != this.turret.Position)
            {
                this.MoveTurret(intVec);
            }
            if ((this.turret.DestroyedOrNull() || !this.turret.Spawned) && intVec.GetFirstThing(GetWearer.Map, Props.TurretDef) == null)
            {
                this.turret = GenSpawn.Spawn(Props.TurretDef, intVec, GetWearer.Map, WipeMode.Vanish);
                this.turret.SetFactionDirect(this.GetWearer.Faction);
                ((Building_Turret_Shoulder)this.turret).Parental = GetWearer;
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
        public void SwitchOffTurret()
        {
            if (!this.turret.DestroyedOrNull() && this.turret.Spawned)
            {
                this.turret.DeSpawn();
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
        public void MoveTurret(IntVec3 intVec)
        {
            if (!this.turret.DestroyedOrNull())
            {
                this.turret.Position = intVec;
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x000022A8 File Offset: 0x000004A8
        public virtual IEnumerable<Gizmo> CompGetGizmosWorn()
        {
            ThingWithComps owner = IsWorn ? GetWearer : parent;
            bool flag = Find.Selector.SelectedObjects.Contains(GetWearer);
            if (flag && GetWearer.IsColonist)
            {
                Texture2D CommandTex;
                int num = 700000101;
                CommandTex = ContentFinder<Texture2D>.Get(Props.iconPath, true);
                yield return new Command_Toggle
                {

                    icon = CommandTex,
                    defaultLabel = Toggled ? "Turret: on." : "Turret: off.",
                    defaultDesc = "Switch mode.",
                    isActive = (() => Toggled),
                    toggleAction = delegate ()
                    {
                        Toggled = !Toggled;
                        this.SwitchTurretMode();
                    },
                    activateSound = SoundDef.Named("Click"),
                    groupKey = num + id,
                    /*
                    disabled = GetWearer.stances.curStance.StanceBusy,
                    disabledReason = "Busy"
                    */
                };
            }
            yield break;
        }


        // Token: 0x0600000A RID: 10 RVA: 0x000023A4 File Offset: 0x000005A4
        public void SwitchTurretMode()
        {
            switch (this.turretIsOn)
            {
                case true:
                    SwitchOffTurret();
                    break;
                case false:
                    SwitchOnTurret();
                    break;
            }
            this.RefreshTurretState();
        }

        // Token: 0x04000001 RID: 1
        public const int updatePeriodInTicks = 60;

        // Token: 0x04000002 RID: 2
        public int nextUpdateTick;

        // Token: 0x04000003 RID: 3 Building_Turret_Shoulder
        public Thing turret;
        public Building_Turret_Shoulder turret_Shoulder;

        private int id;
        // Token: 0x04000004 RID: 4

    }
}
