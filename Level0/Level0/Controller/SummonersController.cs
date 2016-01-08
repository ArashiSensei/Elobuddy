using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using LevelZero.Util;

namespace LevelZero.Controller
{
    class SummonersController
    {
        public bool CastIgnite(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget() || target.IsStructure() || target.IsMinion) return false;

            var summoner = SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Ignite);

            if (summoner != null && summoner.IsReady() && summoner.IsInRange(target) && summoner.Cast(target)) return true;

            return false;
        }

        public bool CastExhaust(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget() || target.IsStructure() || target.IsMinion) return false;

            var summoner = SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Exhaust);

            if (summoner != null && summoner.IsReady() && summoner.IsInRange(target) && summoner.Cast(target)) return true;

            return false;
        }

        public bool CastSmite(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget() || target.IsStructure()) return false;

            var summoner = SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Smite);

            if (summoner != null && summoner.IsReady() && summoner.IsInRange(target))
            {
                if (target.IsMinion())
                {
                    if (summoner.Cast(target)) return true;
                }

                else if ((summoner.Name.Contains("gank") || summoner.Name.Contains("duel")) && summoner.Cast(target)) return true;
            }

            return false;
        }

        public bool CastHeal()
        {
            var summoner = SpellsUtil.GetActiveSpell(SpellsUtil.Summoners.Heal);

            if (summoner != null && summoner.IsReady() && summoner.Cast()) return true;

            return false;
        }

        public bool CastBarrier()
        {
            var summoner = SpellsUtil.GetActiveSpell(SpellsUtil.Summoners.Barrier);

            if (summoner != null && summoner.IsReady() && summoner.Cast()) return true;

            return false;
        }

        public bool CastCleanse()
        {
            var summoner = SpellsUtil.GetActiveSpell(SpellsUtil.Summoners.Cleanse);

            if (summoner != null && summoner.IsReady() && summoner.Cast()) return true;

            return false;
        }

        public bool CastGhost()
        {
            var summoner = SpellsUtil.GetActiveSpell(SpellsUtil.Summoners.Ghost);

            if (summoner != null && summoner.IsReady() && summoner.Cast()) return true;

            return false;
        }

        public bool CastFlash(Vector3 position)
        {
            var summoner = SpellsUtil.GetSkillshotSpell(SpellsUtil.Summoners.Flash);

            if (Player.Instance.Distance(position) > summoner.Range)
                position = Player.Instance.Position.Extend(position, summoner.Range).To3D();

            if (summoner != null && summoner.IsReady() && summoner.Cast(position)) return true;

            return false;
        }
    }
}
