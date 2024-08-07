﻿using OgsOld_ExtraHives.GenStuff;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OgsOld_ExtraHives.HarmonyInstance
{
	// Token: 0x02000019 RID: 25
	[HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve", null)]
	public class GenerateImpliedDefs_PreResolve_Patch
	{
		// Token: 0x06000088 RID: 136 RVA: 0x00004224 File Offset: 0x00002424
		public static void Postfix()
		{
		//	Log.Message("GenerateImpliedDefs_PreResolve");

			PawnGroupKindDef pawnGroupKindDef = new PawnGroupKindDef();
			pawnGroupKindDef.defName = "Hive_OgsOld_ExtraHives";
			pawnGroupKindDef.workerClass = typeof(PawnGroupKindWorker_Normal);
			DefGenerator.AddImpliedDef<PawnGroupKindDef>(pawnGroupKindDef);

			pawnGroupKindDef = new PawnGroupKindDef();
			pawnGroupKindDef.defName = "Tunneler_OgsOld_ExtraHives";
			pawnGroupKindDef.workerClass = typeof(PawnGroupKindWorker_Normal);
			DefGenerator.AddImpliedDef<PawnGroupKindDef>(pawnGroupKindDef);

			PawnsArrivalModeDef pawnsArrivalModeDef = new PawnsArrivalModeDef();
			pawnsArrivalModeDef.defName = "EdgeTunnelIn_OgsOld_ExtraHives";
			pawnsArrivalModeDef.textEnemy = "A group of {0} from {1} have tunneled in nearby.";
			pawnsArrivalModeDef.textFriendly = "A group of friendly {0} from {1} have tunneled in nearby.";
			pawnsArrivalModeDef.textWillArrive = "{0_pawnsPluralDef} will tunnel in.";
			pawnsArrivalModeDef.workerClass = typeof(OgsOld_ExtraHives.PawnsArrivalModeWorker_EdgeTunnel);
			/*
			pawnsArrivalModeDef.selectionWeightCurve = new SimpleCurve();
			pawnsArrivalModeDef.selectionWeightCurve.Add(new CurvePoint(300f,0f));
			pawnsArrivalModeDef.selectionWeightCurve.Add(new CurvePoint(700f,0.3f));
			*/
			DefGenerator.AddImpliedDef<PawnsArrivalModeDef>(pawnsArrivalModeDef);

			pawnsArrivalModeDef = new PawnsArrivalModeDef();
			pawnsArrivalModeDef.defName = "EdgeTunnelInGroups_OgsOld_ExtraHives";
			pawnsArrivalModeDef.textEnemy = "Several separate groups of {0} from {1} have tunneled in nearby.";
			pawnsArrivalModeDef.textFriendly = "Several separate groups of friendly {0} from {1} have tunneled in nearby.";
			pawnsArrivalModeDef.textWillArrive = "Several separate groups of {0_pawnsPluralDef} will tunnel in.";
			pawnsArrivalModeDef.workerClass = typeof(OgsOld_ExtraHives.PawnsArrivalModeWorker_EdgeTunnelGroups);
			/*
			pawnsArrivalModeDef.selectionWeightCurve = new SimpleCurve();
			pawnsArrivalModeDef.selectionWeightCurve.Add(new CurvePoint(100f, 0f));
			pawnsArrivalModeDef.selectionWeightCurve.Add(new CurvePoint(300f, 0.2f));
			pawnsArrivalModeDef.selectionWeightCurve.Add(new CurvePoint(700f, 0.5f));
			pawnsArrivalModeDef.pointsFactorCurve = new SimpleCurve();
			pawnsArrivalModeDef.pointsFactorCurve.Add(new CurvePoint(0f, 0.9f));
			*/
			DefGenerator.AddImpliedDef<PawnsArrivalModeDef>(pawnsArrivalModeDef);

			pawnsArrivalModeDef = new PawnsArrivalModeDef();
			pawnsArrivalModeDef.defName = "CenterTunnelIn_OgsOld_ExtraHives";
			pawnsArrivalModeDef.textEnemy = "A group of {0} from {1} have tunneled in right on top of you!";
			pawnsArrivalModeDef.textFriendly = "A group of friendly {0} from {1} have tunneled in right on top of you!";
			pawnsArrivalModeDef.textWillArrive = "{0_pawnsPluralDef} will tunnel in right on top of you.";
			pawnsArrivalModeDef.workerClass = typeof(OgsOld_ExtraHives.PawnsArrivalModeWorker_CenterTunnel);
			/*
			pawnsArrivalModeDef.selectionWeightCurve = new SimpleCurve();
			pawnsArrivalModeDef.selectionWeightCurve.Add(new CurvePoint(300f, 0.0f));
			pawnsArrivalModeDef.selectionWeightCurve.Add(new CurvePoint(1000f, 3.5f));
			pawnsArrivalModeDef.pointsFactorCurve = new SimpleCurve();
			pawnsArrivalModeDef.pointsFactorCurve.Add(new CurvePoint(0f, 0.75f));
			pawnsArrivalModeDef.pointsFactorCurve.Add(new CurvePoint(5000f, 0.5f));
			*/
			DefGenerator.AddImpliedDef<PawnsArrivalModeDef>(pawnsArrivalModeDef);

			pawnsArrivalModeDef = new PawnsArrivalModeDef();
			pawnsArrivalModeDef.defName = "RandomTunnelIn_OgsOld_ExtraHives";
			pawnsArrivalModeDef.textEnemy = "A group of {0} from {1} have tunneled in. They are scattered all over the area!";
			pawnsArrivalModeDef.textFriendly = "A group of friendly {0} from {1} have tunneled in. They are scattered all over the area!";
			pawnsArrivalModeDef.textWillArrive = "{0_pawnsPluralDef} will tunnel in.";
			pawnsArrivalModeDef.workerClass = typeof(OgsOld_ExtraHives.PawnsArrivalModeWorker_RandomTunnel);
			/*
			pawnsArrivalModeDef.selectionWeightCurve = new SimpleCurve();
			pawnsArrivalModeDef.selectionWeightCurve.Add(new CurvePoint(300f, 0f));
			pawnsArrivalModeDef.selectionWeightCurve.Add(new CurvePoint(1000f, 1.9f));
			pawnsArrivalModeDef.pointsFactorCurve = new SimpleCurve();
			pawnsArrivalModeDef.pointsFactorCurve.Add(new CurvePoint(0f, 0.7f));
			pawnsArrivalModeDef.pointsFactorCurve.Add(new CurvePoint(5000f, 0.45f));
			*/
			DefGenerator.AddImpliedDef<PawnsArrivalModeDef>(pawnsArrivalModeDef);

			ThingDef thingDef = new ThingDef();
			thingDef.defName = "InfestedMeteoriteIncoming_OgsOld_ExtraHives";
			thingDef.category = ThingCategory.Ethereal;
			thingDef.thingClass = typeof(Skyfaller);
			thingDef.useHitPoints = false;
			thingDef.drawOffscreen = true;
			thingDef.tickerType = TickerType.Normal;
			thingDef.altitudeLayer = AltitudeLayer.Skyfaller;
			thingDef.drawerType = DrawerType.RealtimeOnly;
			thingDef.skyfaller = new SkyfallerProperties();
			thingDef.label = "meteorite (incoming)";
			thingDef.size = new IntVec2(3,3);
			thingDef.graphicData = new GraphicData();
			thingDef.graphicData.texPath = "Things/Skyfaller/Meteorite";
			thingDef.graphicData.graphicClass = typeof(Graphic_Single);
			thingDef.graphicData.shaderType = ShaderTypeDefOf.Transparent;
			thingDef.graphicData.drawSize = new Vector2(10, 10);
			thingDef.skyfaller.shadowSize = new Vector2(3, 3);
			thingDef.skyfaller.explosionRadius = 4;
			thingDef.skyfaller.explosionDamage = DamageDefOf.Bomb;
			thingDef.skyfaller.rotateGraphicTowardsDirection = true;
			thingDef.skyfaller.speed = 1.2f;
			DefGenerator.AddImpliedDef<ThingDef>(thingDef);

			thingDef = new ThingDef();
			thingDef.defName = "Tunneler_OgsOld_ExtraHives";
			thingDef.category = ThingCategory.Ethereal;
			thingDef.thingClass = typeof(OgsOld_ExtraHives.TunnelRaidSpawner);
			thingDef.useHitPoints = false;
			thingDef.drawOffscreen = true;
			thingDef.alwaysFlee = true;
			thingDef.tickerType = TickerType.Normal;
			thingDef.altitudeLayer = AltitudeLayer.Skyfaller;
			thingDef.drawerType = DrawerType.RealtimeOnly;
			thingDef.label = "tunnel (incoming)";
			thingDef.size = new IntVec2(1,1);
			DefGenerator.AddImpliedDef<ThingDef>(thingDef);



			//PawnsArrivalModeDef

			/*
			thingDef = new ThingDef();
			thingDef.defName = "Hive_OgsOld_ExtraHives";
			thingDef.category = ThingCategory.Ethereal;
			thingDef.thingClass = typeof(OgsOld_ExtraHives.TunnelRaidSpawner);
			thingDef.useHitPoints = false;
			thingDef.drawOffscreen = true;
			thingDef.alwaysFlee = true;
			thingDef.tickerType = TickerType.Normal;
			thingDef.altitudeLayer = AltitudeLayer.Skyfaller;
			thingDef.drawerType = DrawerType.RealtimeOnly;
			thingDef.label = "tunnel (incoming)";
			thingDef.size = new IntVec2(3,3);
			DefGenerator.AddImpliedDef<ThingDef>(thingDef);

			*/

			//RuleDef
			
			RuleDef ruleDef;
			ruleDef = new RuleDef();
			ruleDef.defName = "OgsOld_ExtraHives_HiveBaseMaker";
			ruleDef.symbol = "OgsOld_ExtraHives_HiveBaseMaker";
			ruleDef.resolvers = new List<SymbolResolver>();
			ruleDef.resolvers.Add(new OgsOld_ExtraHives.GenStuff.SymbolResolver_Hivebase());
			DefGenerator.AddImpliedDef<RuleDef>(ruleDef);

			ruleDef = new RuleDef();
			ruleDef.defName = "OgsOld_ExtraHives_HiveMoundMaker";
			ruleDef.symbol = "OgsOld_ExtraHives_HiveMoundMaker";
			ruleDef.resolvers = new List<SymbolResolver>();
			ruleDef.resolvers.Add(new OgsOld_ExtraHives.GenStuff.SymbolResolver_HiveBaseMoundMaker());
			DefGenerator.AddImpliedDef<RuleDef>(ruleDef);
			
			ruleDef = new RuleDef();
			ruleDef.defName = "OgsOld_ExtraHives_HiveClearChamber";
			ruleDef.symbol = "OgsOld_ExtraHives_HiveClearChamber";
			ruleDef.resolvers = new List<SymbolResolver>();
			ruleDef.resolvers.Add(new OgsOld_ExtraHives.GenStuff.SymbolResolver_ClearChamber());
			DefGenerator.AddImpliedDef<RuleDef>(ruleDef);
			
			ruleDef = new RuleDef();
			ruleDef.defName = "OgsOld_ExtraHives_HiveInterals";
			ruleDef.symbol = "OgsOld_ExtraHives_HiveInterals";
			ruleDef.resolvers = new List<SymbolResolver>();
			ruleDef.resolvers.Add(new OgsOld_ExtraHives.GenStuff.SymbolResolver_HiveInternals());
			DefGenerator.AddImpliedDef<RuleDef>(ruleDef);

			ruleDef = new RuleDef();
			ruleDef.defName = "OgsOld_ExtraHives_HiveOutdoorLighting";
			ruleDef.symbol = "OgsOld_ExtraHives_HiveOutdoorLighting";
			ruleDef.resolvers = new List<SymbolResolver>();
			ruleDef.resolvers.Add(new OgsOld_ExtraHives.GenStuff.SymbolResolver_OutdoorLightingHivebase());
			DefGenerator.AddImpliedDef<RuleDef>(ruleDef);
			
			ruleDef = new RuleDef();
			ruleDef.defName = "OgsOld_ExtraHives_HiveRandomCorpse";
			ruleDef.symbol = "OgsOld_ExtraHives_HiveRandomCorpse";
			ruleDef.resolvers = new List<SymbolResolver>();
			ruleDef.resolvers.Add(new OgsOld_ExtraHives.GenStuff.SymbolResolver_RandomCorpse());
			DefGenerator.AddImpliedDef<RuleDef>(ruleDef);
			
			ruleDef = new RuleDef();
			ruleDef.defName = "OgsOld_ExtraHives_HiveRandomDamage";
			ruleDef.symbol = "OgsOld_ExtraHives_HiveRandomDamage";
			ruleDef.resolvers = new List<SymbolResolver>();
			ruleDef.resolvers.Add(new OgsOld_ExtraHives.GenStuff.SymbolResolver_RandomDamage());
			DefGenerator.AddImpliedDef<RuleDef>(ruleDef);
			
			ruleDef = new RuleDef();
			ruleDef.defName = "OgsOld_ExtraHives_HiveRandomHives";
			ruleDef.symbol = "OgsOld_ExtraHives_HiveRandomHives";
			ruleDef.resolvers = new List<SymbolResolver>();
			ruleDef.resolvers.Add(new OgsOld_ExtraHives.GenStuff.SymbolResolver_RandomHives());
			DefGenerator.AddImpliedDef<RuleDef>(ruleDef);
			
			ruleDef = new RuleDef();
			ruleDef.defName = "OgsOld_ExtraHives_PawnGroup";
			ruleDef.symbol = "OgsOld_ExtraHives_PawnGroup";
			ruleDef.resolvers = new List<SymbolResolver>();
			ruleDef.resolvers.Add(new OgsOld_ExtraHives.GenStuff.SymbolResolver_PawnHiveGroup());
			DefGenerator.AddImpliedDef<RuleDef>(ruleDef);
			
			ruleDef = new RuleDef();
			ruleDef.defName = "OgsOld_ExtraHives_Pawn";
			ruleDef.symbol = "OgsOld_ExtraHives_Pawn";
			ruleDef.resolvers = new List<SymbolResolver>();
			ruleDef.resolvers.Add(new OgsOld_ExtraHives.GenStuff.SymbolResolver_SingleHivePawn());
			DefGenerator.AddImpliedDef<RuleDef>(ruleDef);

		}
	}
}
