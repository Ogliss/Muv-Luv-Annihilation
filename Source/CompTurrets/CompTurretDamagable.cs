using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using CompTurret;

namespace CompTurret
{
    public class CompProperties_TurretDamagable : CompProperties_Turret
    {
        public float breakWhenHitpointsBelow;
        public CompProperties_TurretDamagable()
        {
            this.compClass = typeof(CompTurretDamagable);
        }
    }

    public class CompTurretDamagable : CompTurretGun
    {
        public new CompProperties_TurretDamagable Props => this.props as CompProperties_TurretDamagable;
        public bool IsBroken
        {
            get
            {
                return (float)this.gun.HitPoints / (float)this.gun.MaxHitPoints * 100f <= this.Props.breakWhenHitpointsBelow;
            }
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
    }
}
