using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace CrashedShipsExtension
{
	// Token: 0x02000007 RID: 7
	public class CompSpawnerOnDamaged : ThingComp
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00003308 File Offset: 0x00001508
		public CompProperties_SpawnerOnDamaged Props
		{
			get
			{
				return (CompProperties_SpawnerOnDamaged)this.props;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00003315 File Offset: 0x00001515
		public Lord Lord
		{
			get
			{
				return this.lord;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00003320 File Offset: 0x00001520
		public List<Faction> AllFactions
		{
			get
			{
				List<Faction> allFactionsListForReading = Find.FactionManager.AllFactionsListForReading;
				List<Faction> allFactionsListForReading2 = Find.FactionManager.AllFactionsListForReading;
				bool flag = this.Props.disallowedFactions != null;
				if (flag)
				{
					foreach (Faction faction in allFactionsListForReading)
					{
						bool flag2 = !this.Props.disallowedFactions.Contains(faction.def);
						if (flag2)
						{
							allFactionsListForReading2.Remove(faction);
						}
					}
				}
				return allFactionsListForReading2;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000033CC File Offset: 0x000015CC
		public TechLevel techLevel
		{
			get
			{
				bool flag = this.Props.techLevel != null;
				TechLevel techLevel;
				if (flag)
				{
					bool flag2 = this.Props.techLevel == "Animal";
					if (flag2)
					{
						techLevel = TechLevel.Animal;
					}
					else
					{
						bool flag3 = this.Props.techLevel == "Archotech";
						if (flag3)
						{
							techLevel = TechLevel.Archotech;
						}
						else
						{
							bool flag4 = this.Props.techLevel == "Industrial";
							if (flag4)
							{
								techLevel = TechLevel.Industrial;
							}
							else
							{
								bool flag5 = this.Props.techLevel == "Medieval";
								if (flag5)
								{
									techLevel = TechLevel.Medieval;
								}
								else
								{
									bool flag6 = this.Props.techLevel == "Neolithic";
									if (flag6)
									{
										techLevel = TechLevel.Neolithic;
									}
									else
									{
										bool flag7 = this.Props.techLevel == "Spacer";
										if (flag7)
										{
											techLevel = TechLevel.Spacer;
										}
										else
										{
											bool flag8 = this.Props.techLevel == "Ultra";
											if (flag8)
											{
												techLevel = TechLevel.Ultra;
											}
											else
											{
												techLevel = TechLevel.Undefined;
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
					techLevel = TechLevel.Undefined;
				}
				return techLevel;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000027 RID: 39 RVA: 0x000034E0 File Offset: 0x000016E0
		// (set) Token: 0x06000028 RID: 40 RVA: 0x0000363A File Offset: 0x0000183A
		public Faction OfFaction
		{
			get
			{
				bool flag = this.faction == null;
				if (flag)
				{
					bool flag2 = this.parent.Faction != null;
					if (flag2)
					{
						this.faction = this.parent.Faction;
					}
					else
					{
						bool flag3 = this.Props.Faction != null;
						if (flag3)
						{
							this.factionDef = this.Props.Faction;
							this.faction = Find.FactionManager.FirstFactionOfDef(this.factionDef);
							this.Props.faction = this.faction;
						}
						else
						{
							bool flag4 = this.Props.Factions.Count > 0;
							if (flag4)
							{
								this.factionDef = this.Props.Factions.RandomElement<FactionDef>();
								this.Props.faction = this.faction;
								this.faction = Find.FactionManager.FirstFactionOfDef(this.factionDef);
							}
							else
							{
								this.faction = Find.FactionManager.RandomEnemyFaction(this.Props.allowHidden, this.Props.allowDefeated, this.Props.allowNonHumanlike, this.techLevel);
								this.factionDef = this.faction.def;
								this.Props.faction = this.faction;
							}
						}
					}
				}
				return this.faction;
			}
			set
			{
				this.faction = value;
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003644 File Offset: 0x00001844
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_References.Look<Lord>(ref this.lord, "defenseLord", false);
			Scribe_References.Look<Faction>(ref this.faction, "defenseFaction", false);
			Scribe_Values.Look<float>(ref this.pointsLeft, "PawnPointsLeft", 0f, false);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003694 File Offset: 0x00001894
		public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
		{
			base.PostPreApplyDamage(ref dinfo, out absorbed);
			bool flag = absorbed;
			if (!flag)
			{
				bool harmsHealth = dinfo.Def.harmsHealth;
				if (harmsHealth)
				{
					bool flag2 = this.lord != null;
					if (flag2)
					{
						this.lord.ReceiveMemo(CompSpawnerOnDamaged.MemoDamaged);
					}
					float num = (float)this.parent.HitPoints - dinfo.Amount;
					bool flag3 = (num < (float)this.parent.MaxHitPoints * 0.98f && dinfo.Instigator != null && dinfo.Instigator.Faction != null) || num < (float)this.parent.MaxHitPoints * 0.9f;
					if (flag3)
					{
						this.TrySpawnPawns();
					}
				}
				absorbed = false;
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003758 File Offset: 0x00001958
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			bool flag = !respawningAfterLoad;
			if (flag)
			{
				bool flag2 = this.parent.Faction == null;
				if (flag2)
				{
					this.parent.SetFaction(this.OfFaction, null);
				}
				bool flag3 = this.pointsLeft == 0f;
				if (flag3)
				{
					this.pointsLeft = Mathf.Max(this.Props.defaultPoints * 0.9f, this.Props.minPoints);
				}
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000037DC File Offset: 0x000019DC
		private void TrySpawnPawns()
		{
			bool flag = this.spawnablePawnKinds.NullOrEmpty<PawnGenOption>();
			if (flag)
			{
				bool flag2 = !this.Props.allowedKinddefs.NullOrEmpty<PawnGenOption>();
				if (flag2)
				{
					this.spawnablePawnKinds = this.Props.allowedKinddefs;
				}
				else
				{
					bool flag3 = this.parent.Faction != null;
					if (flag3)
					{
						bool flag4 = this.parent.Faction.def.pawnGroupMakers.Any((PawnGroupMaker x) => x.kindDef == this.Props.factionGroupKindDef);
						if (flag4)
						{
							this.spawnablePawnKinds = this.parent.Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == this.Props.factionGroupKindDef).RandomElementByWeight((PawnGroupMaker x) => x.commonality).options;
						}
						else
						{
							this.spawnablePawnKinds = this.parent.Faction.def.pawnGroupMakers.Where((PawnGroupMaker x) => x.kindDef == PawnGroupKindDefOf.Combat || x.kindDef == PawnGroupKindDefOf.Settlement).RandomElementByWeight((PawnGroupMaker x) => x.commonality).options;
						}
					}
				}
			}
			IEnumerable<PawnGenOption> enumerable = this.spawnablePawnKinds;
			bool flag5 = this.pointsLeft <= 0f;
			if (!flag5)
			{
				bool flag6 = !this.parent.Spawned;
				if (!flag6)
				{
					bool flag7 = this.lord == null;
					if (flag7)
					{
						IntVec3 invalid;
						bool flag8 = !CellFinder.TryFindRandomCellNear(this.parent.Position, this.parent.Map, 5, (IntVec3 c) => c.Standable(this.parent.Map) && this.parent.Map.reachability.CanReach(c, this.parent, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)), out invalid, -1);
						if (flag8)
						{
							Log.Error("Found no place for Pawns to defend " + ((this != null) ? this.ToString()
								: null));
							invalid = IntVec3.Invalid;
						}
						LordJob_PawnsDefendShip lordJob_PawnsDefendShip = new LordJob_PawnsDefendShip(this.parent, this.parent.Faction, 21f, invalid);
						this.lord = LordMaker.MakeNewLord(this.OfFaction, lordJob_PawnsDefendShip, this.parent.Map, null);
					}
					try
					{
						while (this.pointsLeft > 0f)
						{
							PawnGenOption pawnGenOption;
							bool flag9 = !enumerable.Select((PawnGenOption def) => def).TryRandomElementByWeight((PawnGenOption x) => x.selectionWeight, out pawnGenOption);
							if (flag9)
							{
								break;
							}
							IntVec3 intVec;
							bool flag10 = !(from cell in GenAdj.CellsAdjacent8Way(this.parent)
								where this.CanSpawnPawnAt(cell)
								select cell).TryRandomElement(out intVec);
							if (flag10)
							{
								break;
							}
							PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnGenOption.kind, this.faction, PawnGenerationContext.NonPlayer, -1);
							Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
							bool flag11 = !GenPlace.TryPlaceThing(pawn, intVec, this.parent.Map, ThingPlaceMode.Near, null, null, default(Rot4));
							if (flag11)
							{
								Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
								break;
							}
							this.lord.AddPawn(pawn);
							this.pointsLeft -= pawn.kindDef.combatPower;
						}
					}
					finally
					{
						this.pointsLeft = 0f;
					}
					SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera(this.parent.Map);
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003BE8 File Offset: 0x00001DE8
		private bool CanSpawnPawnAt(IntVec3 c)
		{
			return c.Walkable(this.parent.Map);
		}

		// Token: 0x04000030 RID: 48
		public FactionDef factionDef;

		// Token: 0x04000031 RID: 49
		public Faction faction = null;

		// Token: 0x04000032 RID: 50
		public float pointsLeft;

		// Token: 0x04000033 RID: 51
		private Lord lord;

		// Token: 0x04000034 RID: 52
		private const float PawnsDefendRadius = 21f;

		// Token: 0x04000035 RID: 53
		public static readonly string MemoDamaged = "ShipPartDamaged";

		// Token: 0x04000036 RID: 54
		private List<Faction> allFactions = new List<Faction>();

		// Token: 0x04000037 RID: 55
		public List<PawnGenOption> spawnablePawnKinds = new List<PawnGenOption>();
	}
}
