using RimWorld;
using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MuvLuvBeta
{
    // Token: 0x0200016C RID: 364 MuvLuvBeta.LordJob_AssaultColony_CutPower
    public class LordJob_AssaultColony_CutPower : LordJob_AssaultColony
    {
        // Token: 0x06000787 RID: 1927 RVA: 0x00042796 File Offset: 0x00040B96
        public LordJob_AssaultColony_CutPower()
        {
        }

        // Token: 0x06000788 RID: 1928 RVA: 0x000427B4 File Offset: 0x00040BB4
        public LordJob_AssaultColony_CutPower(Faction assaulterFaction, bool canKidnap = true, bool canTimeoutOrFlee = true, bool sappers = false, bool useAvoidGridSmart = false, bool canSteal = true)
        {
            this.assaulterFaction = assaulterFaction;
            this.canKidnap = canKidnap;
            this.canTimeoutOrFlee = canTimeoutOrFlee;
            this.sappers = sappers;
            this.useAvoidGridSmart = useAvoidGridSmart;
            this.canSteal = canSteal;
        }

        public LordJob_AssaultColony_CutPower(SpawnedPawnParams parms)
        {
            this.assaulterFaction = parms.spawnerThing.Faction;
            this.canKidnap = false;
            this.canTimeoutOrFlee = false;
            this.canSteal = false;
        }
        // Token: 0x1700012F RID: 303
        // (get) Token: 0x06000789 RID: 1929 RVA: 0x00042809 File Offset: 0x00040C09
        public override bool GuiltyOnDowned
        {
            get
            {
                return true;
            }
        }

        // Token: 0x0600078A RID: 1930 RVA: 0x0004280C File Offset: 0x00040C0C
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil lordToil = null;
            LordToil lordToil2 = new LordToil_AssaultColony_CutPower(false);
            if (this.useAvoidGridSmart)
            {
                lordToil2.useAvoidGrid = true;
            }
            stateGraph.AddToil(lordToil2);
            return stateGraph;
        }

        // Token: 0x0600078B RID: 1931 RVA: 0x00042C00 File Offset: 0x00041000
        public override void ExposeData()
        {
            Scribe_References.Look<Faction>(ref this.assaulterFaction, "assaulterFaction", false);
            Scribe_Values.Look<bool>(ref this.canKidnap, "canKidnap", true, false);
            Scribe_Values.Look<bool>(ref this.canTimeoutOrFlee, "canTimeoutOrFlee", true, false);
            Scribe_Values.Look<bool>(ref this.sappers, "sappers", false, false);
            Scribe_Values.Look<bool>(ref this.useAvoidGridSmart, "useAvoidGridSmart", false, false);
            Scribe_Values.Look<bool>(ref this.canSteal, "canSteal", true, false);
        }

        // Token: 0x0400033C RID: 828
        private Faction assaulterFaction;

        // Token: 0x0400033D RID: 829
        private bool canKidnap = true;

        // Token: 0x0400033E RID: 830
        private bool canTimeoutOrFlee = true;

        // Token: 0x0400033F RID: 831
        private bool sappers;

        // Token: 0x04000340 RID: 832
        private bool useAvoidGridSmart;

        // Token: 0x04000341 RID: 833
        private bool canSteal = true;

        // Token: 0x04000342 RID: 834
        private static readonly IntRange AssaultTimeBeforeGiveUp = new IntRange(26000, 38000);

        // Token: 0x04000343 RID: 835
        private static readonly IntRange SapTimeBeforeGiveUp = new IntRange(33000, 38000);
    }
}
