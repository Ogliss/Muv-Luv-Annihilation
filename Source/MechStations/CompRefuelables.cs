using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MuvLuvAnnihilation
{
	[StaticConstructorOnStartup]
	public class CompRefuelableSteel : CompRefuelableMulti
	{
		private static readonly Texture2D SetTargetFuelLevelCommand = ContentFinder<Texture2D>.Get("UI/Commands/SetTargetFuelLevel");
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Props.targetFuelLevelConfigurable)
			{
				Command_SetTargetFuelLevelMulti command_SetTargetFuelLevel = new Command_SetTargetFuelLevelMulti();
				command_SetTargetFuelLevel.refuelable = this;
				command_SetTargetFuelLevel.defaultLabel = "2 CommandSetTargetFuelLevel".Translate();
				command_SetTargetFuelLevel.defaultDesc = "2 CommandSetTargetFuelLevelDesc".Translate();
				command_SetTargetFuelLevel.icon = SetTargetFuelLevelCommand;
				yield return command_SetTargetFuelLevel;
			}
			if (Props.showFuelGizmo && Find.Selector.SingleSelectedThing == parent)
			{
				Gizmo_RefuelableFuelStatusMulti gizmo_RefuelableFuelStatus = new Gizmo_RefuelableFuelStatusMulti();
				gizmo_RefuelableFuelStatus.refuelable = this;
				yield return gizmo_RefuelableFuelStatus;
			}
			if (Props.showAllowAutoRefuelToggle)
			{
				Command_Toggle command_Toggle = new Command_Toggle();
				command_Toggle.defaultLabel = "2 CommandToggleAllowAutoRefuel".Translate();
				command_Toggle.defaultDesc = "2 CommandToggleAllowAutoRefuelDesc".Translate();
				command_Toggle.hotKey = KeyBindingDefOf.Command_ItemForbid;
				command_Toggle.icon = (allowAutoRefuel ? TexCommand.ForbidOff : TexCommand.ForbidOn);
				command_Toggle.isActive = (() => allowAutoRefuel);
				command_Toggle.toggleAction = delegate
				{
					allowAutoRefuel = !allowAutoRefuel;
				};
				yield return command_Toggle;
			}
		}
	}
	[StaticConstructorOnStartup]

	public class CompRefuelableChemfuel : CompRefuelableMulti
	{
		private static readonly Texture2D SetTargetFuelLevelCommand = ContentFinder<Texture2D>.Get("UI/Commands/SetTargetFuelLevel");
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Props.targetFuelLevelConfigurable)
			{
				Command_SetTargetFuelLevelMulti command_SetTargetFuelLevel = new Command_SetTargetFuelLevelMulti();
				command_SetTargetFuelLevel.refuelable = this;
				command_SetTargetFuelLevel.defaultLabel = "3 CommandSetTargetFuelLevel".Translate();
				command_SetTargetFuelLevel.defaultDesc = "3 CommandSetTargetFuelLevelDesc".Translate();
				command_SetTargetFuelLevel.icon = SetTargetFuelLevelCommand;
				yield return command_SetTargetFuelLevel;
			}
			if (Props.showFuelGizmo && Find.Selector.SingleSelectedThing == parent)
			{
				Gizmo_RefuelableFuelStatusMulti gizmo_RefuelableFuelStatus = new Gizmo_RefuelableFuelStatusMulti();
				gizmo_RefuelableFuelStatus.refuelable = this;
				yield return gizmo_RefuelableFuelStatus;
			}
			if (Props.showAllowAutoRefuelToggle)
			{
				Command_Toggle command_Toggle = new Command_Toggle();
				command_Toggle.defaultLabel = "3 CommandToggleAllowAutoRefuel".Translate();
				command_Toggle.defaultDesc = "3 CommandToggleAllowAutoRefuelDesc".Translate();
				command_Toggle.hotKey = KeyBindingDefOf.Command_ItemForbid;
				command_Toggle.icon = (allowAutoRefuel ? TexCommand.ForbidOff : TexCommand.ForbidOn);
				command_Toggle.isActive = (() => allowAutoRefuel);
				command_Toggle.toggleAction = delegate
				{
					allowAutoRefuel = !allowAutoRefuel;
				};
				yield return command_Toggle;
			}
		}
	}

	[StaticConstructorOnStartup]
	public class CompRefuelableAmmoFirst : CompRefuelableMulti
	{
		public override ThingFilter FuelFilter
		{
			get
			{
				if (base.fuelFilter != null)
                {
					return base.fuelFilter;
                }
				return Props.fuelFilter;
			}
		}

        private static readonly Texture2D SetTargetFuelLevelCommand = ContentFinder<Texture2D>.Get("UI/Commands/SetTargetFuelLevel");
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Props.targetFuelLevelConfigurable)
			{
				Command_SetTargetFuelLevelMulti command_SetTargetFuelLevel = new Command_SetTargetFuelLevelMulti();
				command_SetTargetFuelLevel.refuelable = this;
				command_SetTargetFuelLevel.defaultLabel = "4 CommandSetTargetFuelLevel".Translate();
				command_SetTargetFuelLevel.defaultDesc = "4 CommandSetTargetFuelLevelDesc".Translate();
				command_SetTargetFuelLevel.icon = SetTargetFuelLevelCommand;
				yield return command_SetTargetFuelLevel;
			}
			if (Props.showFuelGizmo && Find.Selector.SingleSelectedThing == parent)
			{
				Gizmo_RefuelableFuelStatusMulti gizmo_RefuelableFuelStatus = new Gizmo_RefuelableFuelStatusMulti();
				gizmo_RefuelableFuelStatus.refuelable = this;
				yield return gizmo_RefuelableFuelStatus;
			}
			if (Props.showAllowAutoRefuelToggle)
			{
				Command_Toggle command_Toggle = new Command_Toggle();
				command_Toggle.defaultLabel = "4 CommandToggleAllowAutoRefuel".Translate();
				command_Toggle.defaultDesc = "4 CommandToggleAllowAutoRefuelDesc".Translate();
				command_Toggle.hotKey = KeyBindingDefOf.Command_ItemForbid;
				command_Toggle.icon = (allowAutoRefuel ? TexCommand.ForbidOff : TexCommand.ForbidOn);
				command_Toggle.isActive = (() => allowAutoRefuel);
				command_Toggle.toggleAction = delegate
				{
					allowAutoRefuel = !allowAutoRefuel;
				};
				yield return command_Toggle;
			}
			var command_Action = new Command_SetAmmoType(this, this.parent.Map);
			command_Action.defaultLabel = "SetAmmoTypeFirst".Translate();
			command_Action.defaultDesc = "SetAmmoTypeFirstDesc".Translate();
			command_Action.hotKey = KeyBindingDefOf.Misc8;
			command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/LoadTransporter");
			yield return command_Action;
		}
    }

	[StaticConstructorOnStartup]
	public class CompRefuelableAmmoSecond : CompRefuelableMulti
	{
		public override ThingFilter FuelFilter
		{
			get
			{
				if (base.fuelFilter != null)
				{
					return base.fuelFilter;
				}
				return Props.fuelFilter;
			}
		}

		private static readonly Texture2D SetTargetFuelLevelCommand = ContentFinder<Texture2D>.Get("UI/Commands/SetTargetFuelLevel");
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Props.targetFuelLevelConfigurable)
			{
				Command_SetTargetFuelLevelMulti command_SetTargetFuelLevel = new Command_SetTargetFuelLevelMulti();
				command_SetTargetFuelLevel.refuelable = this;
				command_SetTargetFuelLevel.defaultLabel = "4 CommandSetTargetFuelLevel".Translate();
				command_SetTargetFuelLevel.defaultDesc = "4 CommandSetTargetFuelLevelDesc".Translate();
				command_SetTargetFuelLevel.icon = SetTargetFuelLevelCommand;
				yield return command_SetTargetFuelLevel;
			}
			if (Props.showFuelGizmo && Find.Selector.SingleSelectedThing == parent)
			{
				Gizmo_RefuelableFuelStatusMulti gizmo_RefuelableFuelStatus = new Gizmo_RefuelableFuelStatusMulti();
				gizmo_RefuelableFuelStatus.refuelable = this;
				yield return gizmo_RefuelableFuelStatus;
			}
			if (Props.showAllowAutoRefuelToggle)
			{
				Command_Toggle command_Toggle = new Command_Toggle();
				command_Toggle.defaultLabel = "4 CommandToggleAllowAutoRefuel".Translate();
				command_Toggle.defaultDesc = "4 CommandToggleAllowAutoRefuelDesc".Translate();
				command_Toggle.hotKey = KeyBindingDefOf.Command_ItemForbid;
				command_Toggle.icon = (allowAutoRefuel ? TexCommand.ForbidOff : TexCommand.ForbidOn);
				command_Toggle.isActive = (() => allowAutoRefuel);
				command_Toggle.toggleAction = delegate
				{
					allowAutoRefuel = !allowAutoRefuel;
				};
				yield return command_Toggle;
			}
			var command_Action = new Command_SetAmmoType(this, this.parent.Map);
			command_Action.defaultLabel = "SetAmmoTypeSecond".Translate();
			command_Action.defaultDesc = "SetAmmoTypeSecondDesc".Translate();
			command_Action.hotKey = KeyBindingDefOf.Misc8;
			command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/LoadTransporter");
			yield return command_Action;
		}
	}
}
