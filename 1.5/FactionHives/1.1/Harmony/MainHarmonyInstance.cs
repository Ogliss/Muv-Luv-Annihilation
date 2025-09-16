using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OgsOld_ExtraHives.HarmonyInstance
{
    [StaticConstructorOnStartup]
    public class MainHarmonyInstance : Mod
    {
        public MainHarmonyInstance(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("com.ogliss.rimworld.mod.OgsOld_ExtraHives");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

}
