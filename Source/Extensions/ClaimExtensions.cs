using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MuvLuvAnnihilation
{
    public static class ClaimExtensions
    {
        public static bool UnclaimMech(this Pawn pawn, MechSuit AssignedMech)
        {
            if (AssignedMech == null)
            {
                return false;
            }
            AssignedMech.CompAssignableToPawn.ForceRemovePawn(pawn);
            var comp = Current.Game.GetComponent<MechOwnership>();
            if (comp != null)
            {
                comp.mechOwnerships.Remove(pawn);
            }
            return true;
        }

        public static bool ClaimMech(this Pawn pawn, MechSuit newSuit)
        {
            if (newSuit.AssignedPawn == pawn)
            {
                return false;
            }
            UnclaimMech(pawn, newSuit);
            if (newSuit.AssignedPawn != null)
            {
                newSuit.AssignedPawn.UnclaimMech(newSuit);
            }
            newSuit.CompAssignableToPawn.ForceAddPawn(pawn);
            var comp = Current.Game.GetComponent<MechOwnership>();
            if (comp.mechOwnerships == null)
                comp.mechOwnerships = new Dictionary<Pawn, MechSuit>();
            comp.mechOwnerships[pawn] = newSuit;
            return true;
        }
    }
}
