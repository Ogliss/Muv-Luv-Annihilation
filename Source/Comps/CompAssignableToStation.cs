using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MuvLuvAnnihilation
{
	public class CompProperties_MechSuitAssignableToStation : CompProperties
	{
		public int maxAssignedPawnsCount = 1;

		public bool drawAssignmentOverlay = true;

		public bool drawUnownedAssignmentOverlay = true;

		public string singleton;
		public CompProperties_MechSuitAssignableToStation()
		{
			compClass = typeof(CompMechSuitAssignableToStation);
		}
	}

	public class CompMechSuitAssignableToStation : ThingComp
	{
		public MechStation assignedStation;
		public CompProperties_AssignableToPawn Props => (CompProperties_AssignableToPawn)props;

		public int MaxAssignedPawnsCount => Props.maxAssignedPawnsCount;

		public bool PlayerCanSeeAssignments
		{
			get
			{
				if (parent.Faction == Faction.OfPlayer)
				{
					return true;
				}
				if (assignedStation.Faction == Faction.OfPlayer)
				{
					return true;
				}
				return false;
			}
		}

		public virtual AcceptanceReport CanAssignTo(MechStation station)
		{
			return AcceptanceReport.WasAccepted;
		}
		public IEnumerable<Thing> AssigningCandidates
		{
			get
			{
				if (!parent.Spawned)
				{
					return Enumerable.Empty<Pawn>();
				}
				return MechUtils.MechStations(this.parent.Map);
			}
		}

		public bool HasFreeSlot => assignedStation == null;

		protected virtual bool CanDrawOverlayForStation(MechStation station)
		{
			return true;
		}

		public override void DrawGUIOverlay()
		{
			if (Props.drawAssignmentOverlay && (Props.drawUnownedAssignmentOverlay || assignedStation != null) && Find.CameraDriver.CurrentZoom == CameraZoomRange.Closest && PlayerCanSeeAssignments)
			{
				Color defaultThingLabelColor = GenMapUI.DefaultThingLabelColor;
				if (assignedStation == null)
				{
					GenMapUI.DrawThingLabel(parent, "Unowned".Translate(), defaultThingLabelColor);
				}
				if (assignedStation != null && CanDrawOverlayForStation(assignedStation))
				{
					GenMapUI.DrawThingLabel(parent, assignedStation.LabelShort, defaultThingLabelColor);
				}
			}
		}


		public virtual void ForceAddStation(MechStation station)
		{
			assignedStation = station;
		}

		public virtual void ForceRemoveStation(MechStation station)
		{
			assignedStation = null;
		}


		public virtual void TryAssignStation(MechStation station)
		{
			assignedStation = station;
		}

		public virtual void TryUnassignStation(MechStation station)
		{
			assignedStation = null;
		}

		public virtual bool AssignedAnything(MechStation station)
		{
			return assignedStation == station;
		}

		protected virtual bool ShouldShowAssignmentGizmo()
		{
			return parent.Faction == Faction.OfPlayer;
		}

		protected virtual string GetAssignmentGizmoLabel()
		{
			return "CommandThingSetOwnerLabel".Translate();
		}

		protected virtual string GetAssignmentGizmoDesc()
		{
			return "";
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (ShouldShowAssignmentGizmo())
			{
				Command_Action command_Action = new Command_Action();
				command_Action.defaultLabel = GetAssignmentGizmoLabel();
				command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner");
				command_Action.defaultDesc = GetAssignmentGizmoDesc();
				command_Action.action = delegate
				{
					Find.WindowStack.Add(new Dialog_AssignBuildingOwnerStation(this));
				};
				command_Action.hotKey = KeyBindingDefOf.Misc4;
				yield return command_Action;
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_References.Look(ref assignedStation, "assignedStation");
		}

		public override void PostDeSpawn(Map map)
		{
			TryUnassignStation(assignedStation);
		}
	}
}
