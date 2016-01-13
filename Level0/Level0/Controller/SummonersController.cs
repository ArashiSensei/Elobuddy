using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LevelZero.Model;
using SharpDX;
using LevelZero.Util;

namespace LevelZero.Controller
{
    class SummonersController
    {
        public void AutoIgnite(Spell.SpellBase ignite)
        {
            var igniteEnemy = EntityManager.Heroes.Enemies.FirstOrDefault(it => Player.Instance.GetSummonerSpellDamage(it, DamageLibrary.SummonerSpells.Ignite) >= it.Health - 30);

            if (igniteEnemy == null && (igniteEnemy.Distance(Player.Instance) >= 300 || Player.Instance.HealthPercent <= 40)) return;

            ignite.Cast(igniteEnemy);
        }

        public void AutoSmite(Spell.SpellBase Smite)
        {
            var Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Smite.Range).FirstOrDefault();

            if (Mob != default(Obj_AI_Minion))
            {
                var kill = DamageUtil.GetSmiteDamage() >= Mob.Health;

                if (kill && (Mob.Name.Contains("SRU_Dragon") || Mob.Name.Contains("SRU_Baron"))) Smite.Cast(Mob);
            }
        }

        public void AutoSmiteMob(Spell.SpellBase Smite, Feature smiteusage)
        {
            var Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Smite.Range).FirstOrDefault();

            if (Mob == default(Obj_AI_Minion)) return;

            var kill = DamageUtil.GetSmiteDamage() >= Mob.Health;

            if (!kill) return;

            if (Mob.Name.StartsWith("SRU_Red") && smiteusage.IsChecked("smiteusage.red")) Smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Blue") && smiteusage.IsChecked("smiteusage.blue")) Smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Murkwolf") && smiteusage.IsChecked("smiteusage.wolf")) Smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Krug") && smiteusage.IsChecked("smiteusage.krug")) Smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Gromp") && smiteusage.IsChecked("smiteusage.gromp")) Smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Razorbeak") && smiteusage.IsChecked("smiteusage.raptor")) Smite.Cast(Mob);
        }
        
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
