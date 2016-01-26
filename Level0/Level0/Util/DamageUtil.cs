using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LevelZero.Model;

namespace LevelZero.Util
{
    static class DamageUtil
    {
        public static List<SpellDamage> SpellsDamage { get; set; }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            return SpellsDamage.Where(spellBase => spellBase.Spell.IsReady()).Sum(spellBase => GetSpellDamage(enemy,spellBase.Spell.Slot));
        }

        public static bool Killable(Obj_AI_Base enemy, SpellSlot slot, float damageDecrease = 0)
        {
            var possibleDamage = GetSpellDamage(enemy, slot);

            return enemy.Health < (possibleDamage - damageDecrease);
        }

        public static float GetSmiteDamage()
        {
            var damage = new float(); //Arithmetic Progression OP :D

            if (Player.Instance.Level < 10) damage = 360 + (Player.Instance.Level - 1) * 30;

            else if (Player.Instance.Level < 15) damage = 280 + (Player.Instance.Level - 1) * 40;

            else if (Player.Instance.Level < 19) damage = 150 + (Player.Instance.Level - 1) * 50;

            return damage;
        }

        public static float GetSpellDamage(Obj_AI_Base enemy, SpellSlot slot, bool isAbility = true, bool isAaOrTargetted = false)
        {
            var damage = new float();

            foreach (var dmg in SpellsDamage.Where(it => it.Spell.Slot == slot))
            {
                switch (dmg.DamageType)
                {
                    case DamageType.Magical:
                        damage += Player.Instance.CalculateDamageOnUnit(enemy, dmg.DamageType, dmg.SpellDamageValue[dmg.Spell.Level] + dmg.DamageIncrease[dmg.Spell.Level] + dmg.SpellDamageModifier[dmg.Spell.Level] * Player.Instance.FlatMagicDamageMod, isAbility, isAaOrTargetted);
                        if (dmg.HpPercentIncrease != null)
                        {
                            damage += (enemy.MaxHealth * dmg.HpPercentIncrease[dmg.Spell.Level]) / 100;
                        }
                        if (dmg.HpPercentDecrease != null)
                        {
                            damage += (enemy.Health * dmg.HpPercentDecrease[dmg.Spell.Level]) / 100;
                        }
                        continue;
                    case DamageType.Physical:
                        damage += Player.Instance.CalculateDamageOnUnit(enemy, dmg.DamageType, dmg.SpellDamageValue[dmg.Spell.Level] + dmg.DamageIncrease[dmg.Spell.Level] + dmg.SpellDamageModifier[dmg.Spell.Level] * Player.Instance.FlatPhysicalDamageMod, isAbility, isAaOrTargetted);
                        if (dmg.HpPercentIncrease != null)
                        {
                            damage += (enemy.MaxHealth * dmg.HpPercentIncrease[dmg.Spell.Level]) / 100;
                        }
                        if (dmg.HpPercentDecrease != null)
                        {
                            damage += (enemy.Health * dmg.HpPercentDecrease[dmg.Spell.Level]) / 100;
                        }
                        continue;
                    case DamageType.Mixed:
                        damage += Player.Instance.CalculateDamageOnUnit(enemy, dmg.DamageType, dmg.SpellDamageValue[dmg.Spell.Level] + dmg.DamageIncrease[dmg.Spell.Level] + dmg.SpellDamageModifier[dmg.Spell.Level] * (dmg.DamageType == DamageType.Mixed ? Player.Instance.FlatPhysicalDamageMod : Player.Instance.FlatMagicDamageMod), isAbility, isAaOrTargetted);
                        damage += Player.Instance.CalculateDamageOnUnit(enemy, dmg.HybridType, dmg.SpellDamageValueHybrid[dmg.Spell.Level] + dmg.SpellDamageModifierHybrid[dmg.Spell.Level] * (dmg.HybridType == DamageType.Mixed ? Player.Instance.FlatPhysicalDamageMod : Player.Instance.FlatMagicDamageMod), isAbility, isAaOrTargetted);
                        if (dmg.HpPercentIncrease != null)
                        {
                            damage += (enemy.MaxHealth * dmg.HpPercentIncrease[dmg.Spell.Level]) / 100;
                        }
                        if (dmg.HpPercentDecrease != null)
                        {
                            damage += (enemy.Health * dmg.HpPercentDecrease[dmg.Spell.Level]) / 100;
                        }
                        continue;
                    case DamageType.True:
                        damage += Player.Instance.CalculateDamageOnUnit(enemy, dmg.DamageType, dmg.SpellDamageValue[dmg.Spell.Level] + dmg.DamageIncrease[dmg.Spell.Level] + dmg.SpellDamageModifier[dmg.Spell.Level] * Player.Instance.FlatPhysicalDamageMod, isAbility, isAaOrTargetted);
                        if (dmg.HpPercentIncrease != null)
                        {
                            damage += (enemy.MaxHealth * dmg.HpPercentIncrease[dmg.Spell.Level]) / 100;
                        }
                        if (dmg.HpPercentDecrease != null)
                        {
                            damage += (enemy.Health * dmg.HpPercentDecrease[dmg.Spell.Level]) / 100;
                        }
                        break;
                }
            }

            return damage;
        }
    }
}
