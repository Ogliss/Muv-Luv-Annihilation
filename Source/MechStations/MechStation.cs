using OgsOld_CompTurret;
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
        public OgsOld_CompTurretGunDamagable OgsOld_CompTurretGunFirst;
        public OgsOld_CompTurretGunDamagable OgsOld_CompTurretGunSecond;
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
                if (OgsOld_CompTurretGunFirst != null && OgsOld_CompTurretGunFirst.gun.HitPoints != OgsOld_CompTurretGunFirst.gun.MaxHitPoints 
                    && compRefuelableSteel != null && compRefuelableSteel.HasFuel)
                {
                    Log.Message(OgsOld_CompTurretGunFirst.gun.HitPoints + " - " + OgsOld_CompTurretGunFirst.gun.MaxHitPoints + " - compRefuelableSteel.ConsumeFuel(1f) - 2");
                    compRefuelableSteel.ConsumeFuel(1f);
                    OgsOld_CompTurretGunFirst.gun.HitPoints += 1;
                }
                if (OgsOld_CompTurretGunSecond != null && OgsOld_CompTurretGunSecond.gun.HitPoints != OgsOld_CompTurretGunSecond.gun.MaxHitPoints
                    && compRefuelableSteel != null && compRefuelableSteel.HasFuel)
                {
                    Log.Message(OgsOld_CompTurretGunSecond.gun.HitPoints + " - " + OgsOld_CompTurretGunSecond.gun.MaxHitPoints + " - compRefuelableSteel.ConsumeFuel(1f) - 3");
                    compRefuelableSteel.ConsumeFuel(1f);
                    OgsOld_CompTurretGunSecond.gun.HitPoints += 1;
                }
                if (OgsOld_CompTurretGunFirst != null && OgsOld_CompTurretGunFirst.RemainingCharges != OgsOld_CompTurretGunFirst.MaxCharges
                    && compRefuelableAmmoFirst != null && compRefuelableAmmoFirst.FuelFilter != null 
                    && compRefuelableAmmoFirst.FuelFilter.Allows(OgsOld_CompTurretGunFirst.AmmoDef) && compRefuelableAmmoFirst.HasFuel)
                {
                    Log.Message("compRefuelableAmmoFirst.ConsumeFuel(1f)");
                    compRefuelableAmmoFirst.ConsumeFuel(1f);
                    OgsOld_CompTurretGunFirst.remainingCharges += 1;
                }
                if (OgsOld_CompTurretGunSecond != null && OgsOld_CompTurretGunSecond.RemainingCharges != OgsOld_CompTurretGunSecond.MaxCharges
                    && compRefuelableAmmoSecond != null && compRefuelableAmmoSecond.FuelFilter != null 
                    && compRefuelableAmmoSecond.FuelFilter.Allows(OgsOld_CompTurretGunSecond.AmmoDef) && compRefuelableAmmoSecond.HasFuel)
                {
                    Log.Message("compRefuelableAmmoSecond.ConsumeFuel(1f)");
                    compRefuelableAmmoSecond.ConsumeFuel(1f);
                    OgsOld_CompTurretGunSecond.remainingCharges += 1;
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
            var turrets = assignedSuit.AllComps.Where(x => x is OgsOld_CompTurretGun);
            if (turrets.Count() > 0)
            {
                this.OgsOld_CompTurretGunFirst = turrets.ElementAt(0) as OgsOld_CompTurretGunDamagable;
            }
            if (turrets.Count() > 1)
            {
                this.OgsOld_CompTurretGunSecond = turrets.ElementAt(1) as OgsOld_CompTurretGunDamagable;
            }
            this.compReloadableDual = assignedSuit.GetComp<CompReloadableDual>();
        }

        public void EraseSuitsData()
        {
            this.assignedSuit = null;
            this.OgsOld_CompTurretGunFirst = null;
            this.OgsOld_CompTurretGunSecond = null;
            this.compReloadableDual = null;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<MechSuit>(ref assignedSuit, "assignedSuit");
        }
    }
}
