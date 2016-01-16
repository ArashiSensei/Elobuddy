using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LevelZero.Util;
using LevelZero.Model;
using SharpDX;


namespace LevelZero.Controller
{
    class ItemController
    {
        private readonly ItemUtil _itemUtil = new ItemUtil();
        private readonly AIHeroClient Player = EloBuddy.Player.Instance;
        private readonly List<AIHeroClient> Allies = EntityManager.Heroes.Allies;
        private readonly List<AIHeroClient> Enemies = EntityManager.Heroes.Enemies;

        #region Activator methods

        #region Offensive Itens

        public void AutoHextechGunBlade()
        {
            if (!PluginModel.Activator._hextech.IsOwned() || !PluginModel.Activator._hextech.IsReady() || PluginModel.Activator.Target == null || !PluginModel.Activator.Target.IsValidTarget() || !PluginModel.Activator.offensives.IsChecked("offensives.hextech")) return;
            
            if (PluginModel.Activator._hextech.IsInRange(PluginModel.Activator.Target))
                PluginModel.Activator._hextech.Cast(PluginModel.Activator.Target);

            return;
        }

        public void AutoTiamatHydra()
        {
            if (!Player.IsMelee) return;

            if (PluginModel.Activator._tiamat.IsOwned() && PluginModel.Activator._tiamat.IsReady() && !PluginModel.Activator.offensives.IsChecked("offensives.tiamat") && PluginModel.Activator._tiamat.IsInRange(PluginModel.Activator.Target) && PluginModel.Activator._tiamat.Cast()) return;

            if (PluginModel.Activator._hydra.IsOwned() && PluginModel.Activator._hydra.IsReady() && !PluginModel.Activator.offensives.IsChecked("offensives.hydra") && PluginModel.Activator._hydra.IsInRange(PluginModel.Activator.Target) && PluginModel.Activator._hydra.Cast()) return;

            return;
        }

        public void AutoTitanicHydra()
        {
            if (!PluginModel.Activator._titanic.IsOwned() || !PluginModel.Activator._titanic.IsReady() || !PluginModel.Activator.offensives.IsChecked("offensives.titanic")) return;

            if (PluginModel.Activator._titanic.IsInRange(PluginModel.Activator.Target) && PluginModel.Activator._titanic.Cast()) Orbwalker.ResetAutoAttack();

            return;
        }

        public void AutoYoumuusGhostBlade()
        {
            if (PluginModel.Activator._youmuus.IsOwned() && PluginModel.Activator._youmuus.IsReady() && PluginModel.Activator.offensives.IsChecked("offensives.ghostblade") && Player.IsMelee ? Player.Distance(PluginModel.Activator.Target) <= 400 : Player.IsInAutoAttackRange(PluginModel.Activator.Target) && PluginModel.Activator._youmuus.Cast()) return;

            return;
        }

        public void AutoBilgeBtrk()
        {
            if (!PluginModel.Activator._botrk.IsInRange(PluginModel.Activator.Target) && !PluginModel.Activator.offensives.IsChecked("offensives.botrk/bilgewater")) return;

            if (PluginModel.Activator._botrk.IsOwned() && PluginModel.Activator._botrk.IsReady() && PluginModel.Activator._botrk.Cast(PluginModel.Activator.Target)) return;

            if (PluginModel.Activator._bilgewater.IsOwned() && PluginModel.Activator._bilgewater.IsReady() && PluginModel.Activator._bilgewater.Cast(PluginModel.Activator.Target)) return;

            return;
        }

        #endregion

        #region Defensive Itens

        private AIHeroClient GetPrioritedProtectionTarget(int healthpercent, int range = 600)
        {
            var target = Allies.FirstOrDefault(ally => ally.HealthPercent <= healthpercent && Enemies.Any(enemy => enemy.IsInAutoAttackRange(ally)));

            return target;
        }

        public void AutoFaceOfTheMountain()
        {
            if (!PluginModel.Activator._faceMountain.IsOwned() || !PluginModel.Activator._faceMountain.IsReady() || !PluginModel.Activator.defensives.IsChecked("defensives.fotmountain")) return;

            var target = GetPrioritedProtectionTarget(PluginModel.Activator.defensives.SliderValue("defensives.fotmountain.health%"));

            if (target == null || !target.IsValidTarget() || !PluginModel.Activator._faceMountain.IsInRange(target)) return;

            PluginModel.Activator._faceMountain.Cast(target);

            return;
        }

        public void AutoMikael()
        {
            if (!PluginModel.Activator._mikael.IsOwned() || !PluginModel.Activator._mikael.IsReady() || !PluginModel.Activator.defensives.IsChecked("defensives.mikael")) return;

            var target = GetPrioritedProtectionTarget(PluginModel.Activator.defensives.SliderValue("defensives.mikael.health%"));

            if (target == null || !target.IsValidTarget() || !PluginModel.Activator._mikael.IsInRange(target)) return;

            PluginModel.Activator._mikael.Cast(target);

            return;
        }

        public void AutoSolari()
        {
            if (!PluginModel.Activator._solari .IsOwned() || !PluginModel.Activator._solari .IsReady() || !PluginModel.Activator.defensives.IsChecked("defensives.solari")) return;

            var target = GetPrioritedProtectionTarget(PluginModel.Activator.defensives.SliderValue("defensives.solari.health%"));

            if (target == null || !target.IsValidTarget() || !PluginModel.Activator._solari .IsInRange(target)) return;

            PluginModel.Activator._solari .Cast(target);

            return;
        }

        public void AutoScimitarQSS()
        {
            if (!PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss")) return;

            if (PluginModel.Activator._qss.IsOwned() && PluginModel.Activator._qss.IsReady())
            {
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.blind") && Player.HasBuffOfType(BuffType.Blind)) PluginModel.Activator._qss.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.charm") && Player.HasBuffOfType(BuffType.Charm)) PluginModel.Activator._qss.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.fear") && Player.HasBuffOfType(BuffType.Fear)) PluginModel.Activator._qss.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.polymorph") && Player.HasBuffOfType(BuffType.Polymorph)) PluginModel.Activator._qss.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.silence") && Player.HasBuffOfType(BuffType.Silence)) PluginModel.Activator._qss.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.sleep") && Player.HasBuffOfType(BuffType.Sleep)) PluginModel.Activator._qss.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.slow") && Player.HasBuffOfType(BuffType.Slow)) PluginModel.Activator._qss.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.snare") && Player.HasBuffOfType(BuffType.Snare)) PluginModel.Activator._qss.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.stun") && Player.HasBuffOfType(BuffType.Stun)) PluginModel.Activator._qss.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.suppression") && Player.HasBuffOfType(BuffType.Suppression)) PluginModel.Activator._qss.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.taunt") && Player.HasBuffOfType(BuffType.Taunt)) PluginModel.Activator._qss.Cast();
            }
            else if (PluginModel.Activator._scimitar.IsOwned() && PluginModel.Activator._scimitar.IsReady())
            {
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.blind") && Player.HasBuffOfType(BuffType.Blind)) PluginModel.Activator._scimitar.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.charm") && Player.HasBuffOfType(BuffType.Charm)) PluginModel.Activator._scimitar.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.fear") && Player.HasBuffOfType(BuffType.Fear)) PluginModel.Activator._scimitar.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.polymorph") && Player.HasBuffOfType(BuffType.Polymorph)) PluginModel.Activator._scimitar.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.silence") && Player.HasBuffOfType(BuffType.Silence)) PluginModel.Activator._scimitar.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.sleep") && Player.HasBuffOfType(BuffType.Sleep)) PluginModel.Activator._scimitar.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.slow") && Player.HasBuffOfType(BuffType.Slow)) PluginModel.Activator._scimitar.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.snare") && Player.HasBuffOfType(BuffType.Snare)) PluginModel.Activator._scimitar.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.stun") && Player.HasBuffOfType(BuffType.Stun)) PluginModel.Activator._scimitar.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.suppression") && Player.HasBuffOfType(BuffType.Suppression)) PluginModel.Activator._scimitar.Cast();
                if (PluginModel.Activator.defensives.IsChecked("defensives.scimitar/qss.taunt") && Player.HasBuffOfType(BuffType.Taunt)) PluginModel.Activator._scimitar.Cast();
            }

            return;
        }

        public void AutoZhonya()
        {
            if (!PluginModel.Activator._zhonya.IsOwned() || !PluginModel.Activator._zhonya.IsReady() || !PluginModel.Activator.defensives.IsChecked("defensives.zhonya") || Player.HealthPercent > PluginModel.Activator.defensives.SliderValue("defensives.zhonya.health%")) return;

            if (Enemies.Any(it => it.IsInAutoAttackRange(Player))) PluginModel.Activator._zhonya.Cast();

            return;
        }

        public void AutoRanduin()
        {
            if (PluginModel.Activator._randuin.IsOwned() && PluginModel.Activator._randuin.IsReady() && PluginModel.Activator._randuin.IsInRange(PluginModel.Activator.Target) && PluginModel.Activator._randuin.Cast()) return;

            return;
        }

        public void AutoSeraphEmbrace()
        {
            if (!PluginModel.Activator._seraph.IsOwned() || !PluginModel.Activator._seraph.IsReady() || !PluginModel.Activator.defensives.IsChecked("defensives.seraph") || Player.HealthPercent > PluginModel.Activator.defensives.SliderValue("defensives.seraph.health%")) return;

            if (Enemies.Any(it => it.IsInAutoAttackRange(Player))) PluginModel.Activator._seraph.Cast();

            return;
        }

        #endregion

        #region Speed Itens

        public void AutoTalisma()
        {
            if (!PluginModel.Activator._talisma.IsOwned() || !PluginModel.Activator._talisma.IsReady() || !PluginModel.Activator.speed.IsChecked("speed.talisma") || Player.CountAlliesInRange(600) <= 0) return;

            if (Player.Distance(PluginModel.Activator.Target) > 400) PluginModel.Activator._talisma.Cast();

            return;
        }

        public void AutoRighteousGlory()
        {
            if (!PluginModel.Activator._righteousGlory.IsOwned() || !PluginModel.Activator._righteousGlory.IsReady() || !PluginModel.Activator.speed.IsChecked("speed.righteousGlory") || Player.CountAlliesInRange(600) <= 0) return;

            if (Player.Distance(PluginModel.Activator.Target) > 400) PluginModel.Activator._righteousGlory.Cast();

            return;
        }

        #endregion

        #region Potion Itens

        private bool CanUsePotion(string name)
        {
            if (PluginModel.Activator.potions.IsChecked("potions." + name + "potion") && Player.HealthPercent <= PluginModel.Activator.potions.SliderValue("potions." + name + "potion.health%")) return true;

            return false;
        }

        public void AutoHealthPotion()
        {
            if (!PluginModel.Activator._healthPotion.IsOwned() || !PluginModel.Activator._healthPotion.IsReady() || !CanUsePotion("health")) return;

            PluginModel.Activator._healthPotion.Cast();

            return;
        }

        public void AutoBiscuitPotion()
        {
            if (!PluginModel.Activator._biscuitPotion.IsOwned() || !PluginModel.Activator._biscuitPotion.IsReady() || !CanUsePotion("biscuit")) return;

            PluginModel.Activator._biscuitPotion.Cast();

            return;
        }

        public void AutoCorruptingPotion()
        {
            if (!PluginModel.Activator._corruptingPotion.IsOwned() || !PluginModel.Activator._corruptingPotion.IsReady() || !CanUsePotion("corrupting")) return;

            PluginModel.Activator._corruptingPotion.Cast();

            return;
        }

        public void AutoHuntersPotion()
        {
            if (!PluginModel.Activator._huntersPotion.IsOwned() || !PluginModel.Activator._huntersPotion.IsReady() || !CanUsePotion("hunters")) return;

            PluginModel.Activator._huntersPotion.Cast();

            return;
        }

        public void AutoRefillablePotion()
        {
            if (!PluginModel.Activator._refillablePotion.IsOwned() || !PluginModel.Activator._refillablePotion.IsReady() || !CanUsePotion("refillable")) return;

            PluginModel.Activator._refillablePotion.Cast();

            return;
        }

        #endregion

        #endregion

        #region If you want, use these methods on your champion class

        #region Offensive Itens

        public bool CastHextechGunBlade(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget() || target.IsStructure() || target.IsMinion()) return false;

            var Hextech = _itemUtil.GetItem(ItemId.Hextech_Gunblade, 700);

            if (Hextech.IsOwned() && Hextech.IsReady() && Hextech.IsInRange(target) && Hextech.Cast(target)) return true;

            return false;
        }

        public bool CastTiamatHydra(Obj_AI_Base target = null)
        {
            if (target != null)
            {
                var tiamatHydra = _itemUtil.GetItem(ItemId.Tiamat_Melee_Only, 350);

                if (tiamatHydra.IsOwned() && tiamatHydra.IsReady() && tiamatHydra.IsInRange(target) && tiamatHydra.Cast()) return true;

                tiamatHydra = _itemUtil.GetItem(ItemId.Ravenous_Hydra_Melee_Only, 350);

                if (tiamatHydra.IsOwned() && tiamatHydra.IsReady() && tiamatHydra.IsInRange(target) && tiamatHydra.Cast()) return true;
            }
            else
            {
                var tiamatHydra = _itemUtil.GetItem(ItemId.Tiamat_Melee_Only, 400);

                if (tiamatHydra.IsOwned() && tiamatHydra.IsReady() && tiamatHydra.Cast()) return true;

                tiamatHydra = _itemUtil.GetItem(ItemId.Ravenous_Hydra_Melee_Only, 400);

                if (tiamatHydra.IsOwned() && tiamatHydra.IsReady() && tiamatHydra.Cast()) return true;
            }

            return false;
        }

        public bool CastTitanicHydra(Obj_AI_Base target = null)
        {
            if (target != null)
            {
                var titanicHydra = _itemUtil.GetItem(3053, 100);

                if (titanicHydra.IsOwned() && titanicHydra.IsReady() && titanicHydra.IsInRange(target) && titanicHydra.Cast()) return true;
            }
            else
            {
                var titanicHydra = _itemUtil.GetItem(3053, 150);

                if (titanicHydra.IsOwned() && titanicHydra.IsReady() && titanicHydra.Cast()) return true;
            }

            return false;
        }

        public bool CastYoumuusGhostBlade()
        {
            var youmuu = _itemUtil.GetItem(ItemId.Youmuus_Ghostblade);

            if (youmuu.IsOwned() && youmuu.IsReady() && youmuu.Cast()) return true;

            return false;
        }

        public bool CastBilgeBtrk(Obj_AI_Base target)
        {
            if (target == null || !target.IsValidTarget() || target.IsStructure() || target.IsMinion()) return false;

            var bilgewaterBtrk = _itemUtil.GetItem(ItemId.Bilgewater_Cutlass, 550);

            if (bilgewaterBtrk.IsOwned() && bilgewaterBtrk.IsReady() && bilgewaterBtrk.IsInRange(target) && bilgewaterBtrk.Cast(target)) return true;

            bilgewaterBtrk = _itemUtil.GetItem(ItemId.Blade_of_the_Ruined_King, 550);

            if (bilgewaterBtrk.IsOwned() && bilgewaterBtrk.IsReady() && bilgewaterBtrk.IsInRange(target) && bilgewaterBtrk.Cast(target)) return true;

            return false;
        }

        #endregion

        #region Defensive Itens

        public bool CastFaceOfTheMountain(Obj_AI_Base ally)
        {
            if (ally == null || !ally.IsValidTarget() || ally.IsStructure() || ally.IsMinion()) return false;

            var fotmountain = _itemUtil.GetItem(ItemId.Face_of_the_Mountain, 600);

            if (fotmountain.IsOwned() && fotmountain.IsReady() && fotmountain.IsInRange(ally) && fotmountain.Cast(ally)) return true;

            return false;
        }

        public bool CastMikael(Obj_AI_Base ally)
        {
            if (ally == null || !ally.IsValidTarget() || ally.IsStructure() || ally.IsMinion()) return false;

            var mikael = _itemUtil.GetItem(ItemId.Mikaels_Crucible, 600);

            if (mikael.IsOwned() && mikael.IsReady() && mikael.IsInRange(ally) && mikael.Cast(ally)) return true;

            return false;
        }

        public bool CastSolari(Obj_AI_Base ally)
        {
            if (ally == null || !ally.IsValidTarget() || ally.IsStructure() || ally.IsMinion()) return false;

            var solari = _itemUtil.GetItem(ItemId.Locket_of_the_Iron_Solari, 600);

            if (solari.IsOwned() && solari.IsReady() && solari.IsInRange(ally) && solari.Cast(ally)) return true;

            return false;
        }

        public bool CastScimitarQSS()
        {
            var QSS = _itemUtil.GetItem(ItemId.Quicksilver_Sash);

            if (QSS.IsOwned() && QSS.IsReady() && QSS.Cast()) return true;

            QSS = _itemUtil.GetItem(ItemId.Mercurial_Scimitar);

            if (QSS.IsOwned() && QSS.IsReady() && QSS.Cast()) return true;

            return false;
        }

        public bool CastZhonya()
        {
            var zhonya = _itemUtil.GetItem(ItemId.Zhonyas_Hourglass);

            if (!zhonya.IsOwned() || !zhonya.IsReady()) return false;

            zhonya.Cast();

            return true;
        }

        public bool CastRanduin(Obj_AI_Base target = null)
        {
            if (target != null)
            {
                var randuin = _itemUtil.GetItem(ItemId.Randuins_Omen, 450);//500 of range

                if (randuin.IsOwned() && randuin.IsReady() && randuin.IsInRange(target) && randuin.Cast()) return true;
            }
            else
            {
                var randuin = _itemUtil.GetItem(ItemId.Randuins_Omen, 500);//500 of range

                if (randuin.IsOwned() && randuin.IsReady() && randuin.Cast()) return true;
            }

            return false;
        }

        public bool CastSeraphEmbrace()
        {
            var seraph = _itemUtil.GetItem(3040);

            if (!seraph.IsOwned() || !seraph.IsReady()) return false;

            seraph.Cast();

            return true;
        }

        #endregion

        #region Speed Itens

        public bool CastTalisma()
        {
            var talisma = _itemUtil.GetItem(ItemId.Talisman_of_Ascension);

            if (talisma.IsOwned() && talisma.IsReady() && talisma.Cast()) return true;

            return false;
        }

        public bool CastRighteousGlory()
        {
            var glory = _itemUtil.GetItem(ItemId.Righteous_Glory);

            if (glory.IsOwned() && glory.IsReady() && glory.Cast()) return true;

            return false;
        }

        #endregion
        
        #region Ward Itens

        public Vector3 CorrectRange(Vector3 position, int range = 600)
        {
            if (Player.Distance(position) <= range) return position;
            else return Player.Position.Extend(position, range).To3D();
        }

        public bool CastWardingTotem(Vector3 position)
        {
            var ward = _itemUtil.GetItem(ItemId.Warding_Totem_Trinket);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastStealthWard(Vector3 position)
        {
            var ward = _itemUtil.GetItem(ItemId.Stealth_Ward);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastVisionWard(Vector3 position)
        {
            var ward = _itemUtil.GetItem(ItemId.Vision_Ward);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastSightstone(Vector3 position)
        {
            var ward = _itemUtil.GetItem(ItemId.Sightstone);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastRubySightstone(Vector3 position)
        {
            var ward = _itemUtil.GetItem(ItemId.Ruby_Sightstone);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastTrackersKnife(Vector3 position)
        {
            var ward = _itemUtil.GetItem(3711);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastEyeOfTheWatchers(Vector3 position)
        {
            var ward = _itemUtil.GetItem(2301);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastEyeOfTheOasis(Vector3 position)
        {
            var ward = _itemUtil.GetItem(2302);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastEyeOfTheEquinox(Vector3 position)
        {
            var ward = _itemUtil.GetItem(2303);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        #endregion

        #region Potion Itens

        public bool CastHealthPotion()
        {
            var HealthPotion = _itemUtil.GetItem(ItemId.Health_Potion);

            if (HealthPotion.IsOwned() && HealthPotion.IsReady() && HealthPotion.Cast()) return true;

            return false;
        }

        public bool CastBiscuitPotion()
        {
            var BiscuitPotion = _itemUtil.GetItem(2010);

            if (BiscuitPotion.IsOwned() && BiscuitPotion.IsReady() && BiscuitPotion.Cast()) return true;

            return false;
        }

        public bool CastCorruptingPotion()
        {
            var CorruptingPotion = _itemUtil.GetItem(2033);

            if (CorruptingPotion.IsOwned() && CorruptingPotion.IsReady() && CorruptingPotion.Cast()) return true;

            return false;
        }

        public bool CastHuntersPotion()
        {
            var HuntersPotion = _itemUtil.GetItem(2032);

            if (HuntersPotion.IsOwned() && HuntersPotion.IsReady() && HuntersPotion.Cast()) return true;

            return false;
        }

        public bool CastRefillablePotion()
        {
            var RefillablePotion = _itemUtil.GetItem(2031);

            if (RefillablePotion.IsOwned() && RefillablePotion.IsReady() && RefillablePotion.Cast()) return true;

            return false;
        }

        #endregion

        #endregion
    }
}
