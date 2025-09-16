using System.Collections.Generic;
using UnityEngine;
using Verse;
using System.Linq;

namespace MuvLuvAnnihilation
{
    [StaticConstructorOnStartup]
    public class Command_SetAmmoType : Command
    {
        public CompRefuelableMulti compRefuelableMulti;
        public Map map;
        public Command_SetAmmoType(CompRefuelableMulti compRefuelableMulti, Map map)
        {
            this.compRefuelableMulti = compRefuelableMulti;
            this.map = map;
        }
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            foreach (var defName in AmmoTypes)
            {
                var ammoType = DefDatabase<ThingDef>.GetNamed(defName);
                list.Add(new FloatMenuOption(ammoType.LabelCap, delegate
                {
                    this.SetAmmoType(ammoType);
                }, MenuOptionPriority.Default, null, null, 29f, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        public List<string> AmmoTypes = new List<string>
        {
            "Ammo_TSF_Cannon120mm",
            "Ammo_TSF_Minigun",
            "Ammo_TSF_Missile",
            "Ammo_TSF_Mortar",
            "Chemfuel"
        };
        private void SetAmmoType(ThingDef ammoType)
        {
            if (this.compRefuelableMulti.fuelFilter == null) this.compRefuelableMulti.fuelFilter = new ThingFilter();
            this.compRefuelableMulti.fuelFilter.SetDisallowAll();
            this.compRefuelableMulti.fuelFilter.SetAllow(ammoType, true);
            ThingDef thingDef = this.compRefuelableMulti.FuelFilter.AllowedThingDefs.First();
            int num = GenMath.RoundRandom(1f * this.compRefuelableMulti.fuel);
            while (num > 0)
            {
                Thing thing = ThingMaker.MakeThing(thingDef);
                thing.stackCount = Mathf.Min(num, thingDef.stackLimit);
                num -= thing.stackCount;
                GenPlace.TryPlaceThing(thing, this.compRefuelableMulti.parent.Position, map, ThingPlaceMode.Near);
            }
            this.compRefuelableMulti.fuel = 0;
        }
    }
}