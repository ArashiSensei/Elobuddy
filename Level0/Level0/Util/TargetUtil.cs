﻿using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace LevelZero.Util
{
    static class TargetUtil
    {
        //TODO: Popular com buffs que invalidam o alvo (Trindamere, Kayle, Kindred's etc...)
        public static string[] invalidTargetBuffs = { "" };

        /*
            Checkage if target is valid.
        */

        public static bool IsValidTargetUtil(this Obj_AI_Base target)
        {
            return invalidTargetBuffs.All(buff => !target.HasBuff(buff));
        }

        public static bool IsValidTargetUtil(this Obj_AI_Base target, float range)
        {
            if (invalidTargetBuffs.Any(target.HasBuff))
            {
                return false;
            }

            return !(Player.Instance.Distance(target) > range);
        }

        public static bool CanMove(this Obj_AI_Base target)
        {
            if (target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Sleep) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt)) return false;

            return true;
        }
    }
}
