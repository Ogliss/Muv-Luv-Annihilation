using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;
using CompTurret;

namespace MuvLuvAnnihilation.HarmonyInstance
{
    public static class Patch_ArmorUtility
    {
        [HarmonyPatch(typeof(ArmorUtility), nameof(ArmorUtility.GetPostArmorDamage))]
        public static class GetPostArmorDamage_Patch
        {
            public static bool Prefix(Pawn pawn, ref float amount, ref float armorPenetration, BodyPartRecord part, ref DamageDef damageDef, out bool deflectedByMetalArmor, out bool diminishedByMetalArmor, ref float __result)
            {
				deflectedByMetalArmor = false;
				diminishedByMetalArmor = false;
				MechSuit mechSuit = pawn.GetMechSuit();
				if (mechSuit != null && !mechSuit.broken)
                {
					__result = 0f;
					deflectedByMetalArmor = true;
					diminishedByMetalArmor = true;
					var armorDamage = GetPostArmorDamage(pawn, mechSuit, amount, armorPenetration, part, ref damageDef, out deflectedByMetalArmor, out diminishedByMetalArmor);
					if (part.IsInGroup(DefDatabase<BodyPartGroupDef>.GetNamed("Shoulders")))
                    {
						var comp = mechSuit.AllComps.Where(x => x is CompTurretDamagable).RandomElement() as CompTurretDamagable;
						if (comp != null)
                        {
							comp.TakeDamage((int)armorDamage);
                        }
                    }
					mechSuit.TakeArmorDamage(armorDamage);
					return false;
                }
				return true;
            }
        }

		public const float MaxArmorRating = 2f;

		public const float DeflectThresholdFactor = 0.5f;
		public static float GetPostArmorDamage(Pawn pawn, MechSuit mechSuit, float amount, float armorPenetration, BodyPartRecord part, ref DamageDef damageDef, out bool deflectedByMetalArmor, out bool diminishedByMetalArmor)
		{
			deflectedByMetalArmor = false;
			diminishedByMetalArmor = false;
			if (damageDef.armorCategory == null)
			{
				return amount;
			}
			StatDef armorRatingStat = damageDef.armorCategory.armorRatingStat;
			if (mechSuit.def.apparel.CoversBodyPart(part))
			{
				float num2 = amount;
				ApplyArmor(ref amount, armorPenetration, mechSuit.GetStatValue(armorRatingStat), mechSuit, ref damageDef, pawn, out bool metalArmor);
				if (amount < 0.001f)
				{
					deflectedByMetalArmor = metalArmor;
					return 0f;
				}
				if (amount < num2 && metalArmor)
				{
					diminishedByMetalArmor = true;
				}
			}
			float num3 = amount;
			ApplyArmor(ref amount, armorPenetration, pawn.GetStatValue(armorRatingStat), null, ref damageDef, pawn, out bool metalArmor2);
			if (amount < 0.001f)
			{
				deflectedByMetalArmor = metalArmor2;
				return 0f;
			}
			if (amount < num3 && metalArmor2)
			{
				diminishedByMetalArmor = true;
			}
			return amount;
		}

		private static void ApplyArmor(ref float damAmount, float armorPenetration, float armorRating, Thing armorThing, ref DamageDef damageDef, Pawn pawn, out bool metalArmor)
		{
			if (armorThing != null)
			{
				metalArmor = (armorThing.def.apparel.useDeflectMetalEffect || (armorThing.Stuff != null && armorThing.Stuff.IsMetal));
			}
			else
			{
				metalArmor = pawn.RaceProps.IsMechanoid;
			}
			if (armorThing != null)
			{
				float f = damAmount * 0.25f;
				armorThing.TakeDamage(new DamageInfo(damageDef, GenMath.RoundRandom(f)));
			}
			float num = Mathf.Max(armorRating - armorPenetration, 0f);
			float value = Rand.Value;
			float num2 = num * 0.5f;
			float num3 = num;
			if (value < num2)
			{
				damAmount = 0f;
			}
			else if (value < num3)
			{
				damAmount = GenMath.RoundRandom(damAmount / 2f);
				if (damageDef.armorCategory == DamageArmorCategoryDefOf.Sharp)
				{
					damageDef = DamageDefOf.Blunt;
				}
			}
		}
	}

}
