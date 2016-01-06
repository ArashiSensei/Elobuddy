using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace LevelZero.Util
{
    class SpellsUtil
    {
        public static Spell.SpellBase GetActiveSpell(string name, uint range = 0)
        {
            var slot = Player.Instance.GetSpellSlotFromName(name);

            if (slot != SpellSlot.Unknown)
            {
                return new Spell.Active(slot, range);
            }

            return null;
        }

        public static Spell.SpellBase GetTargettedSpell(string name, uint range)
        {
            var slot = Player.Instance.GetSpellSlotFromName(name);

            if (slot != SpellSlot.Unknown)
            {
                return new Spell.Targeted(slot, range);
            }

            return null;
        }

        public static Spell.SpellBase GetSkillshotSpell(string name, uint range, SkillShotType type)
        {
            var slot = Player.Instance.GetSpellSlotFromName(name);

            if (slot != SpellSlot.Unknown)
            {
                return new Spell.Skillshot(slot, range, type);
            }

            return null;
        }
    }
}
