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
        private Activator Activator { get { return PluginModel.Activator; } }

        #region Activator methods

        #region Offensive Itens

        public void AutoHextechGunBlade()
        {
            if (!Activator.hextech.IsOwned() || !Activator.hextech.IsReady() || Activator.Target == null || !Activator.Target.IsValidTarget() || !Activator.offensives.IsChecked("offensives.hextech")) return;
            
            if (Activator.hextech.IsInRange(Activator.Target))
                Activator.hextech.Cast(Activator.Target);

            return;
        }

        public void AutoTiamatHydra()
        {
            if (!Player.IsMelee) return;

            if (Activator.tiamat.IsOwned() && Activator.tiamat.IsReady() && !Activator.offensives.IsChecked("offensives.tiamat") && Activator.tiamat.IsInRange(Activator.Target) && Activator.tiamat.Cast()) return;

            if (Activator.hydra.IsOwned() && Activator.hydra.IsReady() && !Activator.offensives.IsChecked("offensives.hydra") && Activator.hydra.IsInRange(Activator.Target) && Activator.hydra.Cast()) return;

            return;
        }

        public void AutoTitanicHydra()
        {
            if (!Activator.titanic.IsOwned() || !Activator.titanic.IsReady() || !Activator.offensives.IsChecked("offensives.titanic")) return;

            if (Activator.titanic.IsInRange(Activator.Target) && Activator.titanic.Cast()) Orbwalker.ResetAutoAttack();

            return;
        }

        public void AutoYoumuusGhostBlade()
        {
            if (Activator.youmuus.IsOwned() && Activator.youmuus.IsReady() && Activator.offensives.IsChecked("offensives.ghostblade") && Player.IsMelee ? Player.Distance(Activator.Target) <= 400 : Player.IsInAutoAttackRange(Activator.Target) && Activator.youmuus.Cast()) return;

            return;
        }

        public void AutoBilgeBtrk()
        {
            if (!Activator.botrk.IsInRange(Activator.Target) && !Activator.offensives.IsChecked("offensives.botrk/bilgewater")) return;

            if (Activator.botrk.IsOwned() && Activator.botrk.IsReady() && Activator.botrk.Cast(Activator.Target)) return;

            if (Activator.bilgewater.IsOwned() && Activator.bilgewater.IsReady() && Activator.bilgewater.Cast(Activator.Target)) return;

            return;
        }

        #endregion

        #region Defensive Itens

        private AIHeroClient GetPrioritedProtectionTarget(int healthpercent, int range = 600)
        {
            var target = Allies.FirstOrDefault(ally => ally.HealthPercent <= healthpercent && Enemies.Any(enemy => enemy.IsValidTarget() && !enemy.IsDead && enemy.IsInAutoAttackRange(ally)));

            return target;
        }

        public void AutoFaceOfTheMountain()
        {
            if (!Activator.faceMountain.IsOwned() || !Activator.faceMountain.IsReady() || !Activator.defensives.IsChecked("defensives.fotmountain")) return;

            var target = GetPrioritedProtectionTarget(Activator.defensives.SliderValue("defensives.fotmountain.health%"));

            if (target == null || !target.IsValidTarget() || !Activator.faceMountain.IsInRange(target)) return;

            Activator.faceMountain.Cast(target);

            return;
        }

        public void AutoMikael()
        {
            if (!Activator.mikael.IsOwned() || !Activator.mikael.IsReady() || !Activator.defensives.IsChecked("defensives.mikael")) return;

            var target = GetPrioritedProtectionTarget(Activator.defensives.SliderValue("defensives.mikael.health%"));

            if (target == null || !target.IsValidTarget() || !Activator.mikael.IsInRange(target)) return;

            Activator.mikael.Cast(target);

            return;
        }

        public void AutoSolari()
        {
            if (!Activator.solari.IsOwned() || !Activator.solari.IsReady() || !Activator.defensives.IsChecked("defensives.solari")) return;

            var target = GetPrioritedProtectionTarget(Activator.defensives.SliderValue("defensives.solari.health%"));

            if (target == null || !target.IsValidTarget() || !Activator.solari .IsInRange(target)) return;

            Activator.solari .Cast(target);

            return;
        }

        public void AutoScimitarQSS()
        {
            if (!Activator.defensives.IsChecked("defensives.scimitar/qss")) return;

            if (Activator.qss.IsOwned() && Activator.qss.IsReady())
            {
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.blind") && Player.HasBuffOfType(BuffType.Blind)) Activator.qss.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.charm") && Player.HasBuffOfType(BuffType.Charm)) Activator.qss.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.fear") && Player.HasBuffOfType(BuffType.Fear)) Activator.qss.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.polymorph") && Player.HasBuffOfType(BuffType.Polymorph)) Activator.qss.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.silence") && Player.HasBuffOfType(BuffType.Silence)) Activator.qss.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.sleep") && Player.HasBuffOfType(BuffType.Sleep)) Activator.qss.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.slow") && Player.HasBuffOfType(BuffType.Slow)) Activator.qss.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.snare") && Player.HasBuffOfType(BuffType.Snare)) Activator.qss.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.stun") && Player.HasBuffOfType(BuffType.Stun)) Activator.qss.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.suppression") && Player.HasBuffOfType(BuffType.Suppression)) Activator.qss.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.taunt") && Player.HasBuffOfType(BuffType.Taunt)) Activator.qss.Cast();
            }
            else if (Activator.scimitar.IsOwned() && Activator.scimitar.IsReady())
            {
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.blind") && Player.HasBuffOfType(BuffType.Blind)) Activator.scimitar.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.charm") && Player.HasBuffOfType(BuffType.Charm)) Activator.scimitar.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.fear") && Player.HasBuffOfType(BuffType.Fear)) Activator.scimitar.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.polymorph") && Player.HasBuffOfType(BuffType.Polymorph)) Activator.scimitar.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.silence") && Player.HasBuffOfType(BuffType.Silence)) Activator.scimitar.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.sleep") && Player.HasBuffOfType(BuffType.Sleep)) Activator.scimitar.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.slow") && Player.HasBuffOfType(BuffType.Slow)) Activator.scimitar.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.snare") && Player.HasBuffOfType(BuffType.Snare)) Activator.scimitar.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.stun") && Player.HasBuffOfType(BuffType.Stun)) Activator.scimitar.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.suppression") && Player.HasBuffOfType(BuffType.Suppression)) Activator.scimitar.Cast();
                if (Activator.defensives.IsChecked("defensives.scimitar/qss.taunt") && Player.HasBuffOfType(BuffType.Taunt)) Activator.scimitar.Cast();
            }

            return;
        }

        public void AutoZhonya()
        {
            if (!Activator.zhonya.IsOwned() || !Activator.zhonya.IsReady() || !Activator.defensives.IsChecked("defensives.zhonya") || Player.HealthPercent > Activator.defensives.SliderValue("defensives.zhonya.health%")) return;

            if (Enemies.Any(it => it.IsInAutoAttackRange(Player))) Activator.zhonya.Cast();

            return;
        }

        public void AutoRanduin()
        {
            if (Activator.randuin.IsOwned() && Activator.randuin.IsReady() && Activator.randuin.IsInRange(Activator.Target) && Activator.randuin.Cast()) return;

            return;
        }

        public void AutoSeraphEmbrace()
        {
            if (!Activator.seraph.IsOwned() || !Activator.seraph.IsReady() || !Activator.defensives.IsChecked("defensives.seraph") || Player.HealthPercent > Activator.defensives.SliderValue("defensives.seraph.health%")) return;

            if (Enemies.Any(it => it.IsInAutoAttackRange(Player))) Activator.seraph.Cast();

            return;
        }

        #endregion

        #region Speed Itens

        public void AutoTalisma()
        {
            if (!Activator.talisma.IsOwned() || !Activator.talisma.IsReady() || !Activator.speed.IsChecked("speed.talisma") || Player.CountAlliesInRange(600) <= 0) return;

            if (Player.Distance(Activator.Target) > 400) Activator.talisma.Cast();

            return;
        }

        public void AutoRighteousGlory()
        {
            if (!Activator.righteousGlory.IsOwned() || !Activator.righteousGlory.IsReady() || !Activator.speed.IsChecked("speed.righteousGlory") || Player.CountAlliesInRange(600) <= 0) return;

            if (Player.Distance(Activator.Target) > 400) Activator.righteousGlory.Cast();

            return;
        }

        #endregion

        #region Potion Itens

        private bool CanUsePotion(string name)
        {
            if (Activator.potions.IsChecked("potions." + name + "potion") && Player.HealthPercent <= Activator.potions.SliderValue("potions." + name + "potion.health%")) return true;

            return false;
        }

        public void AutoHealthPotion()
        {
            if (!Activator.healthPotion.IsOwned() || !Activator.healthPotion.IsReady() || !CanUsePotion("health")) return;

            Activator.healthPotion.Cast();

            return;
        }

        public void AutoBiscuitPotion()
        {
            if (!Activator.biscuitPotion.IsOwned() || !Activator.biscuitPotion.IsReady() || !CanUsePotion("biscuit")) return;

            Activator.biscuitPotion.Cast();

            return;
        }

        public void AutoCorruptingPotion()
        {
            if (!Activator.corruptingPotion.IsOwned() || !Activator.corruptingPotion.IsReady() || !CanUsePotion("corrupting")) return;

            Activator.corruptingPotion.Cast();

            return;
        }

        public void AutoHuntersPotion()
        {
            if (!Activator.huntersPotion.IsOwned() || !Activator.huntersPotion.IsReady() || !CanUsePotion("hunters")) return;

            Activator.huntersPotion.Cast();

            return;
        }

        public void AutoRefillablePotion()
        {
            if (!Activator.refillablePotion.IsOwned() || !Activator.refillablePotion.IsReady() || !CanUsePotion("refillable")) return;

            Activator.refillablePotion.Cast();

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
