using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MuvLuvAnnihilation
{
	[StaticConstructorOnStartup]
	public class Command_SetTargetFuelLevelMulti : Command
	{
		public CompRefuelableMulti refuelable;

		private List<CompRefuelableMulti> refuelables;

		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			if (refuelables == null)
			{
				refuelables = new List<CompRefuelableMulti>();
			}
			if (!refuelables.Contains(refuelable))
			{
				refuelables.Add(refuelable);
			}
			int num = int.MaxValue;
			for (int i = 0; i < refuelables.Count; i++)
			{
				if ((int)refuelables[i].Props.fuelCapacity < num)
				{
					num = (int)refuelables[i].Props.fuelCapacity;
				}
			}
			int startingValue = num / 2;
			for (int j = 0; j < refuelables.Count; j++)
			{
				if ((int)refuelables[j].TargetFuelLevel <= num)
				{
					startingValue = (int)refuelables[j].TargetFuelLevel;
					break;
				}
			}
			Func<int, string> textGetter = (!refuelable.parent.def.building.hasFuelingPort) ? ((Func<int, string>)((int x) => "SetTargetFuelLevel".Translate(x))) : ((Func<int, string>)((int x) => "SetPodLauncherTargetFuelLevel".Translate(x, MaxLaunchDistanceAtFuelLevel(x))));
			Dialog_Slider window = new Dialog_Slider(textGetter, 0, num, delegate (int value)
			{
				for (int k = 0; k < refuelables.Count; k++)
				{
					refuelables[k].TargetFuelLevel = value;
				}
			}, startingValue);
			Find.WindowStack.Add(window);
        }

        public static int MaxLaunchDistanceAtFuelLevel(float fuelLevel)
        {
            return Mathf.FloorToInt(fuelLevel / 2.25f);
        }

        public override bool InheritInteractionsFrom(Gizmo other)
		{
			if (refuelables == null)
			{
				refuelables = new List<CompRefuelableMulti>();
			}
			refuelables.Add(((Command_SetTargetFuelLevelMulti)other).refuelable);
			return false;
		}
	}
}
