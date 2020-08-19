using RimWorld;
using System;
using Verse.AI;
using Verse.AI.Group;

namespace MuvLuvBeta
{
    // Token: 0x0200018B RID: 395
    public class LordToil_AssaultColony_CutPower : LordToil
    {
        // Token: 0x0600084C RID: 2124 RVA: 0x00047017 File Offset: 0x00045417
        public LordToil_AssaultColony_CutPower(bool attackDownedIfStarving = false)
        {
            this.attackDownedIfStarving = attackDownedIfStarving;
        }

        // Token: 0x17000156 RID: 342
        // (get) Token: 0x0600084D RID: 2125 RVA: 0x00047026 File Offset: 0x00045426
        public override bool ForceHighStoryDanger
        {
            get
            {
                return true;
            }
        }

        // Token: 0x17000157 RID: 343
        // (get) Token: 0x0600084E RID: 2126 RVA: 0x00047029 File Offset: 0x00045429
        public override bool AllowSatisfyLongNeeds
        {
            get
            {
                return false;
            }
        }

        // Token: 0x0600084F RID: 2127 RVA: 0x0004702C File Offset: 0x0004542C
        public override void Init()
        {
            base.Init();
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.Drafting, OpportunityType.Critical);
        }

        // Token: 0x06000850 RID: 2128 RVA: 0x00047040 File Offset: 0x00045440
        public override void UpdateAllDuties()
        {
            for (int i = 0; i < this.lord.ownedPawns.Count; i++)
            {
                this.lord.ownedPawns[i].mindState.duty = new PawnDuty(BETADefOf.MLB_BETA_AssaultColony_CutPower);
                this.lord.ownedPawns[i].mindState.duty.attackDownedIfStarving = this.attackDownedIfStarving;
            }
        }

        // Token: 0x04000391 RID: 913
        private bool attackDownedIfStarving;
    }
}
