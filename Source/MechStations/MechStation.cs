﻿using CompTurret;
using RimWorld;
using System.Linq;
using Verse;

namespace MuvLuvAnnihilation
{
	public class MechStation : Building
	{
        public MechSuit assignedSuit;
        public CompRefuelableSteel compRefuelableSteel;
        public CompRefuelableChemfuel compRefuelableChemfuel;
        public CompRefuelableAmmoFirst compRefuelableAmmoFirst;
        public CompRefuelableAmmoSecond compRefuelableAmmoSecond;
        public CompTurretGun compTurretGunFirst;
        public CompTurretGun compTurretGunSecond;
        public override void Tick()
        {
            base.Tick();
            if (assignedSuit != null && assignedSuit.Position == this.InteractionCell)
            {
                if (assignedSuit.HitPoints != assignedSuit.MaxHitPoints && compRefuelableSteel != null && compRefuelableSteel.HasFuel)
                {
                    compRefuelableSteel.ConsumeFuel(1f);
                    assignedSuit.HitPoints += 1;
                }
                if (compTurretGunFirst != null && compTurretGunFirst.gun.HitPoints != compTurretGunFirst.gun.MaxHitPoints 
                    && compRefuelableSteel != null && compRefuelableSteel.HasFuel)
                {
                    compRefuelableSteel.ConsumeFuel(1f);
                    compTurretGunFirst.gun.HitPoints += 1;
                }
                if (compTurretGunSecond != null && compTurretGunSecond.gun.HitPoints != compTurretGunSecond.gun.MaxHitPoints
                    && compRefuelableSteel != null && compRefuelableSteel.HasFuel)
                {
                    compRefuelableSteel.ConsumeFuel(1f);
                    compTurretGunSecond.gun.HitPoints += 1;
                }
                if (compTurretGunFirst != null && compTurretGunFirst.RemainingCharges != compTurretGunFirst.MaxCharges
                    && compRefuelableAmmoFirst != null && compRefuelableAmmoFirst.FuelFilter != null 
                    && compRefuelableAmmoFirst.FuelFilter.Allows(compTurretGunFirst.AmmoDef) && compRefuelableAmmoFirst.HasFuel)
                {
                    compRefuelableAmmoFirst.ConsumeFuel(1f);
                    compTurretGunFirst.remainingCharges += 1;
                }
                if (compTurretGunSecond != null && compTurretGunSecond.RemainingCharges != compTurretGunSecond.MaxCharges
                    && compRefuelableAmmoSecond != null && compRefuelableAmmoSecond.FuelFilter != null 
                    && compRefuelableAmmoSecond.FuelFilter.Allows(compTurretGunSecond.AmmoDef) && compRefuelableAmmoSecond.HasFuel)
                {
                    compRefuelableAmmoSecond.ConsumeFuel(1f);
                    compTurretGunSecond.remainingCharges += 1;
                }
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.compRefuelableSteel = this.TryGetComp<CompRefuelableSteel>();
            this.compRefuelableChemfuel = this.TryGetComp<CompRefuelableChemfuel>();
            this.compRefuelableAmmoFirst = this.TryGetComp<CompRefuelableAmmoFirst>();
            this.compRefuelableAmmoSecond = this.TryGetComp<CompRefuelableAmmoSecond>();
            InitSuitsData(assignedSuit);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
            EraseSuitsData();
        }
        public void InitSuitsData(MechSuit suit)
        {
            this.assignedSuit = suit;
            var turrets = assignedSuit.AllComps.Where(x => x is CompTurretGun);
            if (turrets.Count() > 0)
            {
                this.compTurretGunFirst = turrets.ElementAt(0) as CompTurretGun;
            }
            if (turrets.Count() > 1)
            {
                this.compTurretGunSecond = turrets.ElementAt(1) as CompTurretGun;
            }
        }

        public void EraseSuitsData()
        {
            this.assignedSuit = null;
            this.compTurretGunFirst = null;
            this.compTurretGunSecond = null;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<MechSuit>(ref assignedSuit, "assignedSuit");
        }
    }
}
