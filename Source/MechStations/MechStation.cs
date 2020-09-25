using CompTurret;
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
        public CompTurretGunDamagable compTurretGunFirst;
        public CompTurretGunDamagable compTurretGunSecond;
        public CompReloadableDual compReloadableDual;
        public override void Tick()
        {
            base.Tick();
            if (assignedSuit != null && assignedSuit.Position == this.InteractionCell)
            {
                if (assignedSuit.HitPoints != assignedSuit.MaxHitPoints && compRefuelableSteel != null && compRefuelableSteel.HasFuel)
                {
                    Log.Message("compRefuelableSteel.ConsumeFuel(1f) - 1");
                    compRefuelableSteel.ConsumeFuel(1f);
                    assignedSuit.HitPoints += 1;
                }
                if (compTurretGunFirst != null && compTurretGunFirst.gun.HitPoints != compTurretGunFirst.gun.MaxHitPoints 
                    && compRefuelableSteel != null && compRefuelableSteel.HasFuel)
                {
                    Log.Message(compTurretGunFirst.gun.HitPoints + " - " + compTurretGunFirst.gun.MaxHitPoints + " - compRefuelableSteel.ConsumeFuel(1f) - 2");
                    compRefuelableSteel.ConsumeFuel(1f);
                    compTurretGunFirst.gun.HitPoints += 1;
                }
                if (compTurretGunSecond != null && compTurretGunSecond.gun.HitPoints != compTurretGunSecond.gun.MaxHitPoints
                    && compRefuelableSteel != null && compRefuelableSteel.HasFuel)
                {
                    Log.Message(compTurretGunSecond.gun.HitPoints + " - " + compTurretGunSecond.gun.MaxHitPoints + " - compRefuelableSteel.ConsumeFuel(1f) - 3");
                    compRefuelableSteel.ConsumeFuel(1f);
                    compTurretGunSecond.gun.HitPoints += 1;
                }
                if (compTurretGunFirst != null && compTurretGunFirst.RemainingCharges != compTurretGunFirst.MaxCharges
                    && compRefuelableAmmoFirst != null && compRefuelableAmmoFirst.FuelFilter != null 
                    && compRefuelableAmmoFirst.FuelFilter.Allows(compTurretGunFirst.AmmoDef) && compRefuelableAmmoFirst.HasFuel)
                {
                    Log.Message("compRefuelableAmmoFirst.ConsumeFuel(1f)");
                    compRefuelableAmmoFirst.ConsumeFuel(1f);
                    compTurretGunFirst.remainingCharges += 1;
                }
                if (compTurretGunSecond != null && compTurretGunSecond.RemainingCharges != compTurretGunSecond.MaxCharges
                    && compRefuelableAmmoSecond != null && compRefuelableAmmoSecond.FuelFilter != null 
                    && compRefuelableAmmoSecond.FuelFilter.Allows(compTurretGunSecond.AmmoDef) && compRefuelableAmmoSecond.HasFuel)
                {
                    Log.Message("compRefuelableAmmoSecond.ConsumeFuel(1f)");
                    compRefuelableAmmoSecond.ConsumeFuel(1f);
                    compTurretGunSecond.remainingCharges += 1;
                }
                if (compReloadableDual != null && compReloadableDual.RemainingCharges != compReloadableDual.MaxCharges
                    && compRefuelableChemfuel != null && compRefuelableChemfuel.FuelFilter != null
                    && compRefuelableChemfuel.FuelFilter.Allows(compReloadableDual.AmmoDef) && compRefuelableChemfuel.Fuel >= compReloadableDual.Props.ammoCountPerCharge)
                {
                    Log.Message("1 compRefuelableChemfuel.ConsumeFuel(20f)");
                    compRefuelableChemfuel.ConsumeFuel(compReloadableDual.Props.ammoCountPerCharge);
                    Thing chemfuel = ThingMaker.MakeThing(compReloadableDual.AmmoDef);
                    chemfuel.stackCount = compReloadableDual.Props.ammoCountPerCharge;
                    var soundDef = compReloadableDual.Props.soundReload;
                    compReloadableDual.Props.soundReload = null;
                    compReloadableDual.ReloadFrom(chemfuel);
                    compReloadableDual.Props.soundReload = soundDef;
                }
                if (compReloadableDual != null && compReloadableDual.RemainingChargesSecondry != compReloadableDual.MaxChargesSecondry
                        && compRefuelableChemfuel != null && compRefuelableChemfuel.FuelFilter != null
                        && compRefuelableChemfuel.FuelFilter.Allows(compReloadableDual.AmmoDefSecondry) 
                        && compRefuelableChemfuel.Fuel >= compReloadableDual.Props.ammoCountPerChargeSecondry)
                {
                    Log.Message("2 compRefuelableChemfuel.ConsumeFuel(20f)");
                    compRefuelableChemfuel.ConsumeFuel(compReloadableDual.Props.ammoCountPerChargeSecondry);
                    Thing chemfuel = ThingMaker.MakeThing(compReloadableDual.AmmoDefSecondry);
                    chemfuel.stackCount = compReloadableDual.Props.ammoCountPerChargeSecondry;
                    var soundDef = compReloadableDual.Props.soundReload;
                    compReloadableDual.Props.soundReload = null;
                    compReloadableDual.ReloadFromSecondry(chemfuel);
                    compReloadableDual.Props.soundReload = soundDef;
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
            if (assignedSuit != null)
            {
                InitSuitsData(assignedSuit);
            }
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
                this.compTurretGunFirst = turrets.ElementAt(0) as CompTurretGunDamagable;
            }
            if (turrets.Count() > 1)
            {
                this.compTurretGunSecond = turrets.ElementAt(1) as CompTurretGunDamagable;
            }
            this.compReloadableDual = assignedSuit.GetComp<CompReloadableDual>();
        }

        public void EraseSuitsData()
        {
            this.assignedSuit = null;
            this.compTurretGunFirst = null;
            this.compTurretGunSecond = null;
            this.compReloadableDual = null;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<MechSuit>(ref assignedSuit, "assignedSuit");
        }
    }
}
