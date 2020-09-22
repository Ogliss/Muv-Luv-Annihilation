using RimWorld;
using System.Linq;
using Verse;

namespace MuvLuvAnnihilation
{
	public class MechSuit : Apparel
	{
		public bool broken = false;
		public CompAssignableToPawn_Mech CompAssignableToPawn => GetComp<CompAssignableToPawn_Mech>();
		public Pawn AssignedPawn
		{
			get
			{
				if (CompAssignableToPawn == null || !CompAssignableToPawn.AssignedPawnsForReading.Any())
				{
					return null;
				}
				return CompAssignableToPawn.AssignedPawnsForReading[0];
			}
		}

		public CompMechSuitAssignableToStation CompAssignableToStation => GetComp<CompMechSuitAssignableToStation>();
		public MechStation AssignedStation
		{
			get
			{
				if (CompAssignableToStation == null || CompAssignableToStation.assignedStation == null)
				{
					return null;
				}
				return CompAssignableToStation.assignedStation;
			}
		}


		public void TakeArmorDamage(float armorDamage)
        {
			this.HitPoints -= (int)armorDamage;
			if ((float)this.HitPoints / (float)this.MaxHitPoints * 100f <= 25f)
            {
				this.broken = true;
				Pawn pawn = this.Wearer;
				this.Wearer.apparel.TryDrop(this);
				DoCatapult(pawn);
            }
        }

		public void DoCatapult(Pawn pawn)
        {
			MLB_ThrownObject flyingObject = (MLB_ThrownObject)GenSpawn.Spawn(ThingDef.Named("BETA_GrapplerClassThrown"), pawn.PositionHeld, pawn.MapHeld, 0);
			var cellToCatapult = pawn.Map.AllCells.Where(x => IntVec3Utility.DistanceTo(pawn.Position, x) == 20).FirstOrDefault();
			flyingObject.Launch(null, new LocalTargetInfo(cellToCatapult), pawn);
		}
		public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look<bool>(ref broken, "broken");
        }
    }
}
