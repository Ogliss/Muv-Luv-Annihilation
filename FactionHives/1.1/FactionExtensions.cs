﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace OgsOld_ExtraHives.ExtensionMethods
{
    public static class FactionExtensions
    {
        public static HiveFactionExtension HiveExt(this Faction faction)
        {
            return faction.def.HiveExt();
        }
        public static HiveFactionExtension HiveExt(this FactionDef faction)
        {
            if (faction.HasModExtension<OgsOld_ExtraHives.HiveFactionExtension>())
            {
                return faction.GetModExtension<OgsOld_ExtraHives.HiveFactionExtension>();
            }
            return null;
        }
        public static List<ThingDef> HivedefsFor(this Faction faction)
        {
            return faction.def.HivedefsFor();
        }
        public static List<ThingDef> HivedefsFor(this FactionDef factionDef)
        {
            List<ThingDef> defs = new List<ThingDef>();
            if (Main.HiveDefs.Any(x => x.GetModExtension<OgsOld_ExtraHives.HiveDefExtension>().Faction == factionDef && x.thingClass == typeof(Hive)))
            {
                defs = Main.HiveDefs.FindAll(x => x.GetModExtension<OgsOld_ExtraHives.HiveDefExtension>().Faction == factionDef);
            }
        //    Log.Message(defs.Count + " Hives loaded for " + factionDef);
            return defs;

        }
    }
}
