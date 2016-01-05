using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using LevelZero.Util;
using SharpDX;

namespace LevelZero.Controller
{
    class ItemController
    {
        #region Offensive Itens
        
        public bool CastHextechGunBlade(Obj_AI_Base target)
        {
            var Hextech = ItemUtil.GetItem(ItemId.Hextech_Gunblade, 700);

            if (Hextech.IsOwned() && Hextech.IsReady() && Hextech.IsInRange(target) && Hextech.Cast(target)) return true;

            return false;
        }

        public bool CastTiamatHydra(Obj_AI_Base target = null)
        {
            if (target != null)
            {
                var tiamatHydra = ItemUtil.GetItem(ItemId.Tiamat_Melee_Only, 400);

                if (tiamatHydra.IsOwned() && tiamatHydra.IsReady() && tiamatHydra.IsInRange(target) && tiamatHydra.Cast()) return true;

                tiamatHydra = ItemUtil.GetItem(ItemId.Ravenous_Hydra_Melee_Only, 400);

                if (tiamatHydra.IsOwned() && tiamatHydra.IsReady() && tiamatHydra.IsInRange(target) && tiamatHydra.Cast()) return true;
            }
            else
            {
                var tiamatHydra = ItemUtil.GetItem(ItemId.Tiamat_Melee_Only, 400);

                if (tiamatHydra.IsOwned() && tiamatHydra.IsReady() && tiamatHydra.Cast()) return true;

                tiamatHydra = ItemUtil.GetItem(ItemId.Ravenous_Hydra_Melee_Only, 400);

                if (tiamatHydra.IsOwned() && tiamatHydra.IsReady() && tiamatHydra.Cast()) return true;
            }

            return false;
        }

        public bool CastYoumuu()
        {
            var youmuu = ItemUtil.GetItem(ItemId.Youmuus_Ghostblade);

            if (youmuu.IsOwned() && youmuu.IsReady() && youmuu.Cast()) return true;

            return false;
        }

        public bool CastBilgeBtrk(Obj_AI_Base target)
        {
            var bilgewaterBtrk = ItemUtil.GetItem(ItemId.Bilgewater_Cutlass, 550);

            if (bilgewaterBtrk.IsOwned() && bilgewaterBtrk.IsReady() && bilgewaterBtrk.IsInRange(target) && bilgewaterBtrk.Cast(target)) return true;

            bilgewaterBtrk = ItemUtil.GetItem(ItemId.Blade_of_the_Ruined_King, 550);

            if (bilgewaterBtrk.IsOwned() && bilgewaterBtrk.IsReady() && bilgewaterBtrk.IsInRange(target) && bilgewaterBtrk.Cast(target)) return true;

            return false;
        }

        #endregion

        #region Defensive Itens

        public bool CastFaceOfTheMountain(Obj_AI_Base ally)
        {
            var fotmountain = ItemUtil.GetItem(ItemId.Face_of_the_Mountain, 600);

            if (fotmountain.IsOwned() && fotmountain.IsReady() && fotmountain.IsInRange(ally) && fotmountain.Cast(ally)) return true;

            return false;
        }

        public bool CastMikael(Obj_AI_Base ally)
        {
            var mikael = ItemUtil.GetItem(ItemId.Mikaels_Crucible, 600);

            if (mikael.IsOwned() && mikael.IsReady() && mikael.IsInRange(ally) && mikael.Cast(ally)) return true;

            return false;
        }

        public bool CastSolari(Obj_AI_Base ally)
        {
            var solari = ItemUtil.GetItem(ItemId.Locket_of_the_Iron_Solari, 600);

            if (solari.IsOwned() && solari.IsReady() && solari.IsInRange(ally) && solari.Cast(ally)) return true;

            return false;
        }

        public bool CastScimitarQSS()
        {
            var QSS = ItemUtil.GetItem(ItemId.Quicksilver_Sash);

            if (QSS.IsOwned() && QSS.IsReady() && QSS.Cast()) return true;

            QSS = ItemUtil.GetItem(ItemId.Mercurial_Scimitar);

            if (QSS.IsOwned() && QSS.IsReady() && QSS.Cast()) return true;

            return false;
        }

        public bool CastZhonya()
        {
            var zhonya = ItemUtil.GetItem(ItemId.Zhonyas_Hourglass);

            if (!zhonya.IsOwned() || !zhonya.IsReady()) return false;

            zhonya.Cast();

            return true;
        }

        public bool CastRanduin(Obj_AI_Base target = null)
        {
            if (target != null)
            {
                var randuin = ItemUtil.GetItem(ItemId.Randuins_Omen, 450);//500 of range

                if (randuin.IsOwned() && randuin.IsReady() && randuin.IsInRange(target) && randuin.Cast()) return true;
            }
            else
            {
                var randuin = ItemUtil.GetItem(ItemId.Randuins_Omen, 500);//500 of range

                if (randuin.IsOwned() && randuin.IsReady() && randuin.Cast()) return true;
            }

            return false;
        }

        public bool CastSeraphEmbrace()
        {
            var seraph = ItemUtil.GetItem(3040);

            if (!seraph.IsOwned() || !seraph.IsReady()) return false;

            seraph.Cast();

            return true;
        }

        #endregion

        #region Speed Itens

        public bool CastTalisma()
        {
            var talisma = ItemUtil.GetItem(ItemId.Talisman_of_Ascension);

            if (talisma.IsOwned() && talisma.IsReady() && talisma.Cast()) return true;

            return false;
        }

        public bool CastRighteousGlory()
        {
            var glory = ItemUtil.GetItem(ItemId.Righteous_Glory);

            if (glory.IsOwned() && glory.IsReady() && glory.Cast()) return true;

            return false;
        }

        #endregion

        #region Ward Itens

        public Vector3 CorrectRange(Vector3 position, int range = 600)
        {
            if (Player.Instance.Distance(position) <= range) return position;
            else return Player.Instance.Position.Extend(position, range).To3D();
        }

        public bool CastWardingTotem(Vector3 position)
        {
            var ward = ItemUtil.GetItem(ItemId.Warding_Totem_Trinket);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastStealthWard(Vector3 position)
        {
            var ward = ItemUtil.GetItem(ItemId.Stealth_Ward);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastVisionWard(Vector3 position)
        {
            var ward = ItemUtil.GetItem(ItemId.Vision_Ward);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastSightstone(Vector3 position)
        {
            var ward = ItemUtil.GetItem(ItemId.Sightstone);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastRubySightstone(Vector3 position)
        {
            var ward = ItemUtil.GetItem(ItemId.Ruby_Sightstone);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastTrackersKnife(Vector3 position)
        {
            var ward = ItemUtil.GetItem(3711);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastEyeOfTheWatchers(Vector3 position)
        {
            var ward = ItemUtil.GetItem(2301);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastEyeOfTheOasis(Vector3 position)
        {
            var ward = ItemUtil.GetItem(2302);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        public bool CastEyeOfTheEquinox(Vector3 position)
        {
            var ward = ItemUtil.GetItem(2303);

            if (ward.IsOwned() && ward.IsReady() && ward.Cast(CorrectRange(position))) return true;

            return false;
        }

        #endregion

        #region Potion Itens

        public bool CastHealthPotion()
        {
            var HealthPotion = ItemUtil.GetItem(ItemId.Health_Potion);

            if (HealthPotion.IsOwned() && HealthPotion.IsReady() && HealthPotion.Cast()) return true;

            return false;
        }

        public bool CastBiscuitPotion()
        {
            var BiscuitPotion = ItemUtil.GetItem(2010);

            if (BiscuitPotion.IsOwned() && BiscuitPotion.IsReady() && BiscuitPotion.Cast()) return true;

            return false;
        }

        public bool CastCorruptingPotion()
        {
            var CorruptingPotion = ItemUtil.GetItem(2033);

            if (CorruptingPotion.IsOwned() && CorruptingPotion.IsReady() && CorruptingPotion.Cast()) return true;

            return false;
        }

        public bool CastHuntersPotion()
        {
            var HuntersPotion = ItemUtil.GetItem(2032);

            if (HuntersPotion.IsOwned() && HuntersPotion.IsReady() && HuntersPotion.Cast()) return true;

            return false;
        }

        public bool CastRefillablePotion()
        {
            var RefillablePotion = ItemUtil.GetItem(2031);

            if (RefillablePotion.IsOwned() && RefillablePotion.IsReady() && RefillablePotion.Cast()) return true;

            return false;
        }

        #endregion
    }
}
