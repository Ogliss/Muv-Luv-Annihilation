using System;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace CrashedShipsExtension
{
	// Token: 0x02000008 RID: 8
	public class LordJob_PawnsDefendShip : LordJob
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00003CBE File Offset: 0x00001EBE
		public LordJob_PawnsDefendShip()
		{
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003CC8 File Offset: 0x00001EC8
		public LordJob_PawnsDefendShip(SpawnedPawnParams parms)
		{
			this.defSpot = parms.defSpot;
			this.defendRadius = parms.defendRadius;
			this.shipPart = parms.spawnerThing;
			this.faction = parms.spawnerThing.Faction;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003D07 File Offset: 0x00001F07
		public LordJob_PawnsDefendShip(Thing shipPart, Faction faction, float defendRadius, IntVec3 defSpot)
		{
			this.shipPart = shipPart;
			this.faction = faction;
			this.defendRadius = defendRadius;
			this.defSpot = defSpot;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00003D30 File Offset: 0x00001F30
		public override bool CanBlockHostileVisitors
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00003D44 File Offset: 0x00001F44
		public override bool AddFleeToil
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003D58 File Offset: 0x00001F58
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			bool flag = !this.defSpot.IsValid;
			StateGraph stateGraph2;
			if (flag)
			{
				Log.Warning("LordJob_PawnsDefendShip defSpot is invalid. Returning graph for LordJob_AssaultColony.", false);
				stateGraph.AttachSubgraph(new LordJob_AssaultColony(this.faction, true, true, false, false, true).CreateGraph());
				stateGraph2 = stateGraph;
			}
			else
			{
				LordToil_DefendPoint lordToil_DefendPoint = new LordToil_DefendPoint(this.defSpot, this.defendRadius, null);
				stateGraph.StartingToil = lordToil_DefendPoint;
				LordToil_AssaultColony lordToil_AssaultColony = new LordToil_AssaultColony(false);
				stateGraph.AddToil(lordToil_AssaultColony);
				LordToil_AssaultColony lordToil_AssaultColony2 = new LordToil_AssaultColony(false);
				stateGraph.AddToil(lordToil_AssaultColony2);
				Transition transition = new Transition(lordToil_DefendPoint, lordToil_AssaultColony2, false, true);
				transition.AddSource(lordToil_AssaultColony);
				transition.AddTrigger(new Trigger_PawnCannotReachMapEdge());
				stateGraph.AddTransition(transition, false);
				Transition transition2 = new Transition(lordToil_DefendPoint, lordToil_AssaultColony, false, true);
				transition2.AddTrigger(new Trigger_PawnHarmed(0.5f, true, null));
				transition2.AddTrigger(new Trigger_PawnLostViolently(true));
				transition2.AddTrigger(new Trigger_Memo(CompSpawnerOnDamaged.MemoDamaged));
				transition2.AddPostAction(new TransitionAction_EndAllJobs());
				stateGraph.AddTransition(transition2, false);
				Transition transition3 = new Transition(lordToil_AssaultColony, lordToil_DefendPoint, false, true);
				transition3.AddTrigger(new Trigger_TicksPassedWithoutHarmOrMemos(1380, new string[] { CompSpawnerOnDamaged.MemoDamaged }));
				transition3.AddPostAction(new TransitionAction_EndAttackBuildingJobs());
				stateGraph.AddTransition(transition3, false);
				Transition transition4 = new Transition(lordToil_DefendPoint, lordToil_AssaultColony2, false, true);
				transition4.AddSource(lordToil_AssaultColony);
				transition4.AddTrigger(new Trigger_ThingDamageTaken(this.shipPart, 0.5f));
				transition4.AddTrigger(new Trigger_Memo(HediffGiver_Heat.MemoPawnBurnedByAir));
				stateGraph.AddTransition(transition4, false);
				stateGraph2 = stateGraph;
			}
			return stateGraph2;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003F08 File Offset: 0x00002108
		public override void ExposeData()
		{
			Scribe_References.Look<Thing>(ref this.shipPart, "shipPart", false);
			Scribe_References.Look<Faction>(ref this.faction, "faction", false);
			Scribe_Values.Look<float>(ref this.defendRadius, "defendRadius", 0f, false);
			Scribe_Values.Look<IntVec3>(ref this.defSpot, "defSpot", default(IntVec3), false);
		}

		// Token: 0x04000038 RID: 56
		private Thing shipPart;

		// Token: 0x04000039 RID: 57
		private Faction faction;

		// Token: 0x0400003A RID: 58
		private float defendRadius;

		// Token: 0x0400003B RID: 59
		private IntVec3 defSpot;
	}
}
