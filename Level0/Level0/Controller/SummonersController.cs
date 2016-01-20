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
        AIHeroClient Player = EloBuddy.Player.Instance;

        #region Activator methods

        public void AutoIgnite()
        {
            if (PluginModel.Activator._ignite == null || !PluginModel.Activator._ignite.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.ignite")) return;

            var igniteEnemy = EntityManager.Heroes.Enemies.FirstOrDefault(it => Player.GetSummonerSpellDamage(it, DamageLibrary.SummonerSpells.Ignite) >= it.Health + 28);

            if (igniteEnemy == null) return;

            if ((igniteEnemy.Distance(Player) >= 300 || Player.HealthPercent <= 40))
                PluginModel.Activator._ignite.Cast(igniteEnemy);

            return;
        }

        public void AutoSmite()
        {
            if (PluginModel.Activator._smite == null || !PluginModel.Activator._smite.IsReady()) return;

            if (PluginModel.Activator.summoners.IsChecked("summoners.smite") && PluginModel.Activator.summoners.IsChecked("summoners.smite.ks"))
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(PluginModel.Activator._smite.Range) && DamageLibrary.GetSummonerSpellDamage(Player, it, DamageLibrary.SummonerSpells.Smite) >= it.Health);

                if (bye != null) { PluginModel.Activator._smite.Cast(bye); return; }
            }

            var Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, PluginModel.Activator._smite.Range).FirstOrDefault(it => (it.Name.Contains("SRU_Dragon") || it.Name.Contains("SRU_Baron")) && DamageUtil.GetSmiteDamage() >= it.Health);

            if (Mob != null) { PluginModel.Activator._smite.Cast(Mob); return; }
        }

        public void AutoSmiteMob()
        {
            if (PluginModel.Activator._smite == null || !PluginModel.Activator._smite.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.smite")) return;

            var Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, PluginModel.Activator._smite.Range).FirstOrDefault(it => DamageUtil.GetSmiteDamage() >= it.Health);

            if (Mob == null) return;

            if (Mob.Name.StartsWith("SRU_Red") && PluginModel.Activator.summoners.IsChecked("summoners.smite.red")) PluginModel.Activator._smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Blue") && PluginModel.Activator.summoners.IsChecked("summoners.smite.blue")) PluginModel.Activator._smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Murkwolf") && PluginModel.Activator.summoners.IsChecked("summoners.smite.wolf")) PluginModel.Activator._smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Krug") && PluginModel.Activator.summoners.IsChecked("summoners.smite.krug")) PluginModel.Activator._smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Gromp") && PluginModel.Activator.summoners.IsChecked("summoners.smite.gromp")) PluginModel.Activator._smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Razorbeak") && PluginModel.Activator.summoners.IsChecked("summoners.smite.raptor")) PluginModel.Activator._smite.Cast(Mob);
        }
        
        public void AutoHeal()
        {
            if (PluginModel.Activator._heal == null || !PluginModel.Activator._heal.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.heal")) return;

            var target = EntityManager.Heroes.Allies.FirstOrDefault(it => it.IsValidTarget(PluginModel.Activator._heal.Range) && it.HealthPercent <= PluginModel.Activator.summoners.SliderValue("summoners.heal.health%"));

            if (target != null)
            {
                if (EntityManager.Heroes.Enemies.Any(it => it.IsValidTarget() && it.Distance(target) <= it.GetAutoAttackRange())) PluginModel.Activator._heal.Cast();
            }
        }

        public void AutoBarrier()
        {
            if (PluginModel.Activator._barrier == null || !PluginModel.Activator._barrier.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.barrier") || Player.HealthPercent > PluginModel.Activator.summoners.SliderValue("summoners.barrier.health%")) return;

            PluginModel.Activator._barrier.Cast();
        }

        public void AutoGhost()
        {
            if (PluginModel.Activator._ghost == null || !PluginModel.Activator._ghost.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.ghost") || PluginModel.Activator.Target == null || !PluginModel.Activator.Target.IsValidTarget()) return;

            if (!Player.IsInAutoAttackRange(PluginModel.Activator.Target) && Player.Distance(PluginModel.Activator.Target) <= 400)
                PluginModel.Activator._ghost.Cast();
        }

        public void AutoCleanse()
        {
            if (PluginModel.Activator._cleanse == null || !PluginModel.Activator._cleanse.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.cleanse")) return;

            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.blind") && Player.HasBuffOfType(BuffType.Blind)) PluginModel.Activator._cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.charm") && Player.HasBuffOfType(BuffType.Charm)) PluginModel.Activator._cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.fear") && Player.HasBuffOfType(BuffType.Fear)) PluginModel.Activator._cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.polymorph") && Player.HasBuffOfType(BuffType.Polymorph)) PluginModel.Activator._cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.silence") && Player.HasBuffOfType(BuffType.Silence)) PluginModel.Activator._cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.sleep") && Player.HasBuffOfType(BuffType.Sleep)) PluginModel.Activator._cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.slow") && Player.HasBuffOfType(BuffType.Slow)) PluginModel.Activator._cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.snare") && Player.HasBuffOfType(BuffType.Snare)) PluginModel.Activator._cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.stun") && Player.HasBuffOfType(BuffType.Stun)) PluginModel.Activator._cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.suppression") && Player.HasBuffOfType(BuffType.Suppression)) PluginModel.Activator._cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.taunt") && Player.HasBuffOfType(BuffType.Taunt)) PluginModel.Activator._cleanse.Cast();
        }

        #endregion

        #region If you want, use these methods on your champion class

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

            if (Player.Distance(position) > summoner.Range)
                position = Player.Position.Extend(position, summoner.Range).To3D();

            if (summoner != null && summoner.IsReady() && summoner.Cast(position)) return true;

            return false;
        }

        #endregion
    }
}
