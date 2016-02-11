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
        private readonly Activator Activator = PluginModel.Activator;

        #region Activator methods

        public void AutoIgnite()
        {
            if (PluginModel.Activator.ignite == null || !PluginModel.Activator.ignite.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.ignite")) return;

            var igniteEnemy = EntityManager.Heroes.Enemies.FirstOrDefault(it => Player.GetSummonerSpellDamage(it, DamageLibrary.SummonerSpells.Ignite) >= it.Health + 28);

            if (igniteEnemy == null) return;

            if ((igniteEnemy.Distance(Player) >= 300 || Player.HealthPercent <= 40))
                PluginModel.Activator.ignite.Cast(igniteEnemy);

            return;
        }

        public void AutoSmite()
        {
            if (PluginModel.Activator.smite == null || !PluginModel.Activator.smite.IsReady()) return;

            if (PluginModel.Activator.summoners.IsChecked("summoners.smite") && PluginModel.Activator.summoners.IsChecked("summoners.smite.ks"))
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(PluginModel.Activator.smite.Range) && DamageLibrary.GetSummonerSpellDamage(Player, it, DamageLibrary.SummonerSpells.Smite) >= it.Health);

                if (bye != null) { PluginModel.Activator.smite.Cast(bye); return; }
            }

            var Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, PluginModel.Activator.smite.Range).FirstOrDefault(it => (it.Name.Contains("SRUDragon") || it.Name.Contains("SRUBaron")) && DamageUtil.GetSmiteDamage() >= it.Health);

            if (Mob != null) { PluginModel.Activator.smite.Cast(Mob); return; }
        }

        public void AutoSmiteMob()
        {
            if (PluginModel.Activator.smite == null || !PluginModel.Activator.smite.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.smite")) return;

            var Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, PluginModel.Activator.smite.Range).FirstOrDefault(it => DamageUtil.GetSmiteDamage() >= it.Health);

            if (Mob == null) return;

            if (Mob.Name.StartsWith("SRU_Red") && PluginModel.Activator.summoners.IsChecked("summoners.smite.red")) PluginModel.Activator.smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Blue") && PluginModel.Activator.summoners.IsChecked("summoners.smite.blue")) PluginModel.Activator.smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Murkwolf") && PluginModel.Activator.summoners.IsChecked("summoners.smite.wolf")) PluginModel.Activator.smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Krug") && PluginModel.Activator.summoners.IsChecked("summoners.smite.krug")) PluginModel.Activator.smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Gromp") && PluginModel.Activator.summoners.IsChecked("summoners.smite.gromp")) PluginModel.Activator.smite.Cast(Mob);
            else if (Mob.Name.StartsWith("SRU_Razorbeak") && PluginModel.Activator.summoners.IsChecked("summoners.smite.raptor")) PluginModel.Activator.smite.Cast(Mob);
        }
        
        public void AutoHeal()
        {
            if (PluginModel.Activator.heal == null || !PluginModel.Activator.heal.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.heal")) return;

            var target = EntityManager.Heroes.Allies.FirstOrDefault(it => it.IsValidTarget(PluginModel.Activator.heal.Range) && it.HealthPercent <= PluginModel.Activator.summoners.SliderValue("summoners.heal.health%"));

            if (target != null)
            {
                if (EntityManager.Heroes.Enemies.Any(it => it.IsValidTarget() && it.IsInAutoAttackRange(target))) PluginModel.Activator.heal.Cast();
            }
        }

        public void AutoBarrier()
        {
            if (PluginModel.Activator.barrier == null || !PluginModel.Activator.barrier.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.barrier") || Player.HealthPercent > PluginModel.Activator.summoners.SliderValue("summoners.barrier.health%")) return;

            PluginModel.Activator.barrier.Cast();
        }

        public void AutoGhost()
        {
            if (PluginModel.Activator.ghost == null || !PluginModel.Activator.ghost.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.ghost") || PluginModel.Activator.Target == null || !PluginModel.Activator.Target.IsValidTarget()) return;

            if (!Player.IsInAutoAttackRange(PluginModel.Activator.Target) && Player.Distance(PluginModel.Activator.Target) <= 400)
                PluginModel.Activator.ghost.Cast();
        }

        public void AutoCleanse()
        {
            if (PluginModel.Activator.cleanse == null || !PluginModel.Activator.cleanse.IsReady() || !PluginModel.Activator.summoners.IsChecked("summoners.cleanse")) return;

            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.blind") && Player.HasBuffOfType(BuffType.Blind)) PluginModel.Activator.cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.charm") && Player.HasBuffOfType(BuffType.Charm)) PluginModel.Activator.cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.fear") && Player.HasBuffOfType(BuffType.Fear)) PluginModel.Activator.cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.polymorph") && Player.HasBuffOfType(BuffType.Polymorph)) PluginModel.Activator.cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.silence") && Player.HasBuffOfType(BuffType.Silence)) PluginModel.Activator.cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.sleep") && Player.HasBuffOfType(BuffType.Sleep)) PluginModel.Activator.cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.slow") && Player.HasBuffOfType(BuffType.Slow)) PluginModel.Activator.cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.snare") && Player.HasBuffOfType(BuffType.Snare)) PluginModel.Activator.cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.stun") && Player.HasBuffOfType(BuffType.Stun)) PluginModel.Activator.cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.suppression") && Player.HasBuffOfType(BuffType.Suppression)) PluginModel.Activator.cleanse.Cast();
            if (PluginModel.Activator.summoners.IsChecked("summoners.cleanse.taunt") && Player.HasBuffOfType(BuffType.Taunt)) PluginModel.Activator.cleanse.Cast();
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
