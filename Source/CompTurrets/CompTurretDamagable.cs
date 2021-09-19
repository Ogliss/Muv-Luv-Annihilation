using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using OgsOld_CompTurret;

namespace OgsOld_CompTurret
{
    public class CompProperties_TurretDamagable : CompProperties_Turret
    {
        public float breakWhenHitpointsBelow;
        public CompProperties_TurretDamagable()
        {
            this.compClass = typeof(OgsOld_CompTurretGunDamagable);
        }
    }

    public class OgsOld_CompTurretGunDamagable : OgsOld_CompTurretGun
    {
        public int currentHP;
        public new CompProperties_TurretDamagable Props => this.props as CompProperties_TurretDamagable;
        public bool IsBroken
        {
            get
            {
                return (float)this.gun.HitPoints / (float)this.gun.MaxHitPoints * 100f <= this.Props.breakWhenHitpointsBelow;
            }
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            this.gun.HitPoints = this.gun.MaxHitPoints;
        }
        public void TakeDamage(int damage)
        {
            this.gun.HitPoints -= damage;
        }
        public override bool Active => !IsBroken && base.Active;
        public override bool CanBeUsed => !IsBroken && base.CanBeUsed;
        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            Log.Message(this + " - TEST - PostPreApplyDamage", true);
            base.PostPreApplyDamage(dinfo, out absorbed);
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            Log.Message(this + " - TEST - PostPostApplyDamage", true);
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                this.currentHP = this.gun.HitPoints;
            }
            else if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.gun.HitPoints = this.currentHP;
            }
            Scribe_Values.Look<int>(ref currentHP, "gunHitPoints");
        }
    }
}
