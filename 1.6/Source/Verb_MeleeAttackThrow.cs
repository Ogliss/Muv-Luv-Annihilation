using MuvLuvAnnihilation.HarmonyInstance;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MuvLuvAnnihilation
{
    // Token: 0x02000A0D RID: 2573
    public class Verb_MeleeAttackThrow : Verb_MeleeAttack
    {
        // Token: 0x06005F6B RID: 24427 RVA: 0x0020F288 File Offset: 0x0020D488
        private IEnumerable<DamageInfo> DamageInfosToApply(LocalTargetInfo target)
        {
            /*
            float num = this.verbProps.AdjustedMeleeDamageAmount(this, this.CasterPawn);
            float armorPenetration = this.verbProps.AdjustedArmorPenetration(this, this.CasterPawn);
            DamageDef def = this.verbProps.meleeDamageDef;
            BodyPartGroupDef bodyPartGroupDef = null;
            HediffDef hediffDef = null;
            Rand.PushState();
            num = Rand.Range(num * 0.8f, num * 1.2f);
            Rand.PopState();
            if (this.CasterIsPawn)
            {
                bodyPartGroupDef = this.verbProps.AdjustedLinkedBodyPartsGroup(this.tool);
                if (num >= 1f)
                {
                    if (base.HediffCompSource != null)
                    {
                        hediffDef = base.HediffCompSource.Def;
                    }
                }
                else
                {
                    num = 1f;
                    def = DamageDefOf.Blunt;
                }
            }
            ThingDef source;
            if (base.EquipmentSource != null)
            {
                source = base.EquipmentSource.def;
            }
            else
            {
                source = this.CasterPawn.def;
            }
            Vector3 direction = (target.Thing.Position - this.CasterPawn.Position).ToVector3();
            DamageInfo damageInfo = new DamageInfo(def, num, armorPenetration, -1f, this.caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
            damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
            damageInfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
            damageInfo.SetWeaponHediff(hediffDef);
            damageInfo.SetAngle(direction);
            yield return damageInfo;
            if (this.tool != null && this.tool.extraMeleeDamages != null)
            {
                foreach (ExtraDamage extraDamage in this.tool.extraMeleeDamages)
                {
                    Rand.PushState();
                    if (Rand.Chance(extraDamage.chance))
                    {
                        num = extraDamage.amount;
                        num = Rand.Range(num * 0.8f, num * 1.2f);
                        damageInfo = new DamageInfo(extraDamage.def, num, extraDamage.AdjustedArmorPenetration(this, this.CasterPawn), -1f, this.caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
                        damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                        damageInfo.SetWeaponBodyPartGroup(bodyPartGroupDef);
                        damageInfo.SetWeaponHediff(hediffDef);
                        damageInfo.SetAngle(direction);
                        yield return damageInfo;
                    }
                    Rand.PopState();
                }
                //    List<ExtraDamage>.Enumerator enumerator = default(List<ExtraDamage>.Enumerator);
            }
            if (this.surpriseAttack && ((this.verbProps.surpriseAttack != null && !this.verbProps.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraDamage>()) || (this.tool != null && this.tool.surpriseAttack != null && !this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraDamage>())))
            {
                IEnumerable<ExtraDamage> enumerable = Enumerable.Empty<ExtraDamage>();
                if (this.verbProps.surpriseAttack != null && this.verbProps.surpriseAttack.extraMeleeDamages != null)
                {
                    enumerable = enumerable.Concat(this.verbProps.surpriseAttack.extraMeleeDamages);
                }
                if (this.tool != null && this.tool.surpriseAttack != null && !this.tool.surpriseAttack.extraMeleeDamages.NullOrEmpty<ExtraDamage>())
                {
                    enumerable = enumerable.Concat(this.tool.surpriseAttack.extraMeleeDamages);
                }
                foreach (ExtraDamage extraDamage2 in enumerable)
                {
                    int num2 = GenMath.RoundRandom(extraDamage2.AdjustedDamageAmount(this, this.CasterPawn));
                    float armorPenetration2 = extraDamage2.AdjustedArmorPenetration(this, this.CasterPawn);
                    DamageInfo damageInfo2 = new DamageInfo(extraDamage2.def, (float)num2, armorPenetration2, -1f, this.caster, null, source, DamageInfo.SourceCategory.ThingOrUnknown, null);
                    damageInfo2.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                    damageInfo2.SetWeaponBodyPartGroup(bodyPartGroupDef);
                    damageInfo2.SetWeaponHediff(hediffDef);
                    damageInfo2.SetAngle(direction);
                    yield return damageInfo2;
                }
                //    IEnumerator<ExtraDamage> enumerator2 = null;
            }
            */
            if (target.HasThing && target.Pawn !=null)
            {
                Pawn hitPawn = (Pawn)target;
                Rand.PushState();
                MainHarmonyInstance.ThrowEffect(CasterPawn, hitPawn, (int)Rand.Range(2, Math.Min(10, CasterPawn.BodySize - hitPawn.BodySize)), true);
                Rand.PopState();
            }
            yield break;
        }

        // Token: 0x060039E2 RID: 14818 RVA: 0x001B83E4 File Offset: 0x001B67E4
        public override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            DamageWorker.DamageResult result = new DamageWorker.DamageResult();
            foreach (DamageInfo dinfo in this.DamageInfosToApply(target))
            {
                if (target.ThingDestroyed)
                {
                    break;
                }
                result = target.Thing.TakeDamage(dinfo);
            }
            
            return result;
        }

    }
}
