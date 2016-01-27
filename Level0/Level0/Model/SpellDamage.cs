using EloBuddy;
using EloBuddy.SDK;

namespace LevelZero.Model
{
    class SpellDamage
    {
        public Spell.SpellBase Spell { get; set; }
        public float[] SpellDamageValue { get; set; }
        public float[] SpellDamageModifier { get; set; }
        public DamageType DamageType { get; set; }
        public float[] DamageIncrease { get; set; }
        public float[] HpPercentIncrease { get; set; }
        public float[] HpPercentDecrease { get; set; }

        public SpellDamage(Spell.SpellBase spell, float[] spellDamageValue, float[] spellDamageModifier, DamageType damageType)
        {
            Spell = spell;
            SpellDamageValue = spellDamageValue;
            SpellDamageModifier = spellDamageModifier;
            DamageType = damageType;
        }

        public SpellDamage(Spell.SpellBase spell, float[] spellDamageValue, float[] spellDamageModifier, DamageType damageType, float[] damageIncrease, float[] hpPercentIncrease, float[] hpPercentDecrease)
        {
            Spell = spell;
            SpellDamageValue = spellDamageValue;
            SpellDamageModifier = spellDamageModifier;
            DamageType = damageType;
            DamageIncrease = damageIncrease;
            HpPercentIncrease = hpPercentIncrease;
            HpPercentDecrease = hpPercentDecrease;
        }

    }
}
