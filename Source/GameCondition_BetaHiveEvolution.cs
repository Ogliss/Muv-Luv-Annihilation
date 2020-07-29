using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MuvLuvBeta
{
	// Token: 0x02000A09 RID: 2569
	public class GameCondition_BetaHiveEvolution : GameCondition
	{
		/*
		public override string TooltipString
		{
			get
			{
				Vector2 location;
				if (Find.CurrentMap != null)
				{
					location = Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile);
				}
				else
				{
					location = default(Vector2);
				}
				return this.def.LabelCap + "\n" + "\n" + this.Description + ("\n" + "ImpactDate".Translate().CapitalizeFirst() + ": " + GenDate.DateFullStringAt((long)GenDate.TickGameToAbs(this.startTick + base.Duration), location)) + ("\n" + "TimeLeft".Translate().CapitalizeFirst() + ": " + base.TicksLeft.ToStringTicksToPeriod(true, false, true, true));
			}
		}
		*/
		// Token: 0x06003D54 RID: 15700 RVA: 0x00143D9C File Offset: 0x00141F9C
		public override void GameConditionTick()
		{
			base.GameConditionTick();

		}

		// Token: 0x06003D55 RID: 15701 RVA: 0x00143DF7 File Offset: 0x00141FF7
		public override void End()
		{
			base.End();
		//	this.Impact();
		}

		// Token: 0x06003D56 RID: 15702 RVA: 0x00143E05 File Offset: 0x00142005
		private void Impact()
		{
			ScreenFader.SetColor(Color.clear);
			
			GenGameEnd.EndGameDialogMessage("GameOverPlanetkillerImpact".Translate(Find.World.info.name), false, GameCondition_BetaHiveEvolution.FadeColor);
		}

		// Token: 0x040024AB RID: 9387
		private const int SoundDuration = 179;

		// Token: 0x040024AC RID: 9388
		private const int FadeDuration = 90;

		// Token: 0x040024AD RID: 9389
		private static readonly Color FadeColor = Color.white;
	}

}
