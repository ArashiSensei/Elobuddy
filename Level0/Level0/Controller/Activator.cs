using System;
using System.Linq;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using LevelZero.Util;
using LevelZero.Model;
using LevelZero.Model.Values;

/*

# ANTI-COPY/PASTA
# !(iSpaghettiCODE)

*/

namespace LevelZero.Controller
{
    class Activator
    {
        string[] _autoZhonyaSpells = new string[] { "zed.r", "veigar.r", "veigar.w", "malphite.r", "garen.r", "darius.r", "fizz.r", "lux.r", "ezreal.r", "leesin.r", "morgana.r", "chogath.r", "nunu.r" };

        List<Feature> Features = new List<Feature>();
        public Feature start, offensives, defensives, speed, potions, summoners, misc;

        AIHeroClient Player = EloBuddy.Player.Instance;
        public AIHeroClient Target;

        //Target Selector values
        int _range;
        DamageType _damageType;

        //Items and Summoners management
        ItemUtil _itemUtil;
        ItemController _itens;
        SummonersController _summoners;

        //<Summoners>
        public Spell.Active heal, barrier, ghost, cleanse;
        public Spell.Targeted ignite, exhaust, smite;
        public Spell.Skillshot flash;

        //<Itens>
        public Item
        //Offensives
        hextech, botrk, bilgewater, tiamat, hydra, titanic, youmuus,

        //Defensives
        faceMountain, mikael, solari, randuin, scimitar, qss, seraph, zhonya,

        //Speed
        talisma, righteousGlory,

        //Potions
        healthPotion, biscuitPotion, corruptingPotion, huntersPotion, refillablePotion;

        /*
        Wards
        _wardingTotem, _stealthWard, VisionWard _pinkWard, _sightstone, _rubySightstone,
        _tracersKnife, _eyeWatchers, _eyeOasis, _eyeEquinox,
        */

        #region Initializing
        
        public Activator(DamageType damageType = DamageType.Physical, int range = 700)
        {
            InitMenu();
            InitVariables();

            _range = range;
            _damageType = damageType;

            Game.OnTick += Game_OnTick;

            //AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        void InitMenu()
        {
            var feature = new Feature
            {
                NameFeature = "Start",
                MenuValueStyleList = new List<ValueAbstract>()
                {
                    new ValueCheckbox(true, "enable", "Use Activator")
                }
            };
            feature.ToActivatorMenu();
            Features.Add(feature);

            #region Summoners

            feature = new Feature
            {
                NameFeature = "Summoners",
                MenuValueStyleList = new List<ValueAbstract>()
                {
                    new ValueCheckbox(true, "summoners.ignite", "Use Ignite"),

                    new ValueCheckbox(true, "summoners.exhaust", "Use Exhaust (Combo Mode, instantly)"),

                    new ValueCheckbox(true, "summoners.heal", "Use Heal", true),
                    new ValueCheckbox(true, "summoners.heal.dangerousspells", "Use Heal on dangerous spells"),
                    new ValueSlider(1500, 1, 100, "summoners.heal.dangerousspells.safelife", "(Dangerous) Use Heal just if my health will be >=", true),
                    new ValueSlider(99, 1, 30, "summoners.heal.health%", "Use Heal when health% is at:"),

                    new ValueCheckbox(true, "summoners.barrier", "Use Barrier", true),
                    new ValueCheckbox(true, "summoners.barrier.dangerousspells", "Use Barrier on dangerous spells"),
                    new ValueSlider(1500, 1, 100, "summoners.barrier.dangerousspells.safelife", "(Dangerous) Use Barrier just if my health will be >=", true),
                    new ValueSlider(99, 1, 30, "summoners.barrier.health%", "Use Barrier when health% is at:"),

                    new ValueCheckbox(true, "summoners.ghost", "Use Ghost", true),

                    new ValueCheckbox(true, "summoners.smite", "Use Smite", true),
                    new ValueCheckbox(true, "summoners.smite.enemies", "Enemies - Use Smite", true),
                    new ValueCheckbox(true, "summoners.smite.ks", "KS - Use Smite"),
                    new ValueCheckbox(true, "summoners.smite.red", "Red - Use Smite"),
                    new ValueCheckbox(true, "summoners.smite.blue", "Blue - Use Smite"),
                    new ValueCheckbox(true, "summoners.smite.wolf", "Wolf - Use Smite"),
                    new ValueCheckbox(true, "summoners.smite.gromp", "Gromp - Use Smite"),
                    new ValueCheckbox(true, "summoners.smite.raptor", "Raptor - Use Smite"),
                    new ValueCheckbox(true, "summoners.smite.krug", "Krug - Use Smite"),

                    new ValueCheckbox(true, "summoners.cleanse", "Use Cleanse", true),
                    new ValueCheckbox(true, "summoners.cleanse.stun", "Stun - Use Cleanse", true),
                    new ValueCheckbox(true, "summoners.cleanse.polymorph", "Polymorph - Use Cleanse"),
                    new ValueCheckbox(false, "summoners.cleanse.slow", "Slow - Use Cleanse"),
                    new ValueCheckbox(true, "summoners.cleanse.suppression", "Suppression - Use Cleanse"),
                    new ValueCheckbox(true, "summoners.cleanse.taunt", "Taunt - Use Cleanse"),
                    new ValueCheckbox(true, "summoners.cleanse.charm", "Charm - Use Cleanse"),
                    new ValueCheckbox(true, "summoners.cleanse.fear", "Fear - Use Cleanse"),
                    new ValueCheckbox(true, "summoners.cleanse.snare", "Snare - Use Cleanse"),
                    new ValueCheckbox(true, "summoners.cleanse.blind", "Blind - Use Cleanse"),
                    new ValueCheckbox(true, "summoners.cleanse.sleep", "Sleep - Use Cleanse"),
                    new ValueCheckbox(true, "summoners.cleanse.silence", "Silence - Use Cleanse"),
                }
            };

            feature.ToActivatorMenu();
            Features.Add(feature);

            #endregion

            #region Offensives

            feature = new Feature
            {
                NameFeature = "Offensives",
                MenuValueStyleList = new List<ValueAbstract>()
                {
                    new ValueCheckbox(true, "offensives.hextech", "Use Hextech"),

                    new ValueCheckbox(true, "offensives.botrk/bilgewater", "Use BOTRK/Bilgewater", true),
                    new ValueSlider(99, 1, 30, "offensives.botrk/bilgewater.health%", "Use BOTRK/Bilgewater when my health% is at:"),

                    new ValueCheckbox(true, "offensives.hydra/tiamat", "Use Hydra/Tiamat", true),

                    new ValueCheckbox(true, "offensives.ghostblade", "Use Ghost Blade"),
                }
            };

            feature.ToActivatorMenu();
            Features.Add(feature);

            #endregion

            #region Defensives

            feature = new Feature
            {
                NameFeature = "Defensives",
                MenuValueStyleList = new List<ValueAbstract>()
                {
                    new ValueCheckbox(true, "defensives.randuin", "Use Randuin"),

                    new ValueCheckbox(true, "defensives.scimitar/qss", "Use Scimitar/QSS", true),
                    new ValueCheckbox(true, "defensives.scimitar/qss.stun", "Stun - Use Scimitar/Qss", true),
                    new ValueCheckbox(true, "defensives.scimitar/qss.polymorph", "Polymorph - Use Scimitar/Qss"),
                    new ValueCheckbox(false, "defensives.scimitar/qss.slow", "Slow - Use Scimitar/Qss"),
                    new ValueCheckbox(true, "defensives.scimitar/qss.suppression", "Suppression - Use Scimitar/Qss"),
                    new ValueCheckbox(true, "defensives.scimitar/qss.taunt", "Taunt - Use Scimitar/Qss"),
                    new ValueCheckbox(true, "defensives.scimitar/qss.charm", "Charm - Use Scimitar/Qss"),
                    new ValueCheckbox(true, "defensives.scimitar/qss.fear", "Fear - Use Scimitar/Qss"),
                    new ValueCheckbox(true, "defensives.scimitar/qss.snare", "Snare - Use Scimitar/Qss"),
                    new ValueCheckbox(true, "defensives.scimitar/qss.blind", "Blind - Use Scimitar/Qss"),
                    new ValueCheckbox(true, "defensives.scimitar/qss.sleep", "Sleep - Use Scimitar/Qss"),
                    new ValueCheckbox(true, "defensives.scimitar/qss.silence", "Silence - Use Scimitar/Qss"),

                    new ValueCheckbox(true, "defensives.fotmountain", "Use Face of the Mountain", true),
                    new ValueSlider(99, 1, 30, "defensives.fotmountain.health%", "Use Face of the Mountain when ally health% is at:"),

                    new ValueCheckbox(true, "defensives.mikael", "Use Mikael", true),
                    new ValueSlider(99, 1, 30, "defensives.mikael.health%", "Use Mikael when ally health% is at:"),

                    new ValueCheckbox(true, "defensives.solari", "Use Iron Solari", true),
                    new ValueSlider(99, 1, 30, "defensives.solari.health%", "Use Iron Solari when ally health% is at:"),

                    new ValueCheckbox(true, "defensives.seraph", "Use Seraph Embrance", true),
                    new ValueSlider(99, 1, 30, "defensives.seraph.health%", "Use Seraph Embrance when my health% is at:"),

                    new ValueCheckbox(true, "defensives.zhonya", "Use Zhonya", true),
                    new ValueSlider(99, 1, 30, "defensives.zhonya.health%", "Use Zhonya when my health% is at:"),
                    new ValueCheckbox(true, "defensives.zhonya.zed.r", "Use Zhonya on Zed R", true),
                    new ValueCheckbox(true, "defensives.zhonya.veigar.r", "Use Zhonya on Veigar R"),
                    new ValueCheckbox(true, "defensives.zhonya.veigar.w", "Use Zhonya on Veigar W"),
                    new ValueCheckbox(true, "defensives.zhonya.malphite.r", "Use Zhonya on Malphite R"),
                    new ValueCheckbox(true, "defensives.zhonya.ezreal.r", "Use Zhonya on Ezreal R"),
                    new ValueCheckbox(true, "defensives.zhonya.darius.r", "Use Zhonya on Darius R"),
                    new ValueCheckbox(true, "defensives.zhonya.garen.r", "Use Zhonya on Garen R"),
                    new ValueCheckbox(true, "defensives.zhonya.fizz.r", "Use Zhonya on Fizz R"),
                    new ValueCheckbox(true, "defensives.zhonya.leesin.r", "Use Zhonya on Lee Sin R"),
                    new ValueCheckbox(true, "defensives.zhonya.chogath.r", "Use Zhonya on ChoGath R"),
                    new ValueCheckbox(true, "defensives.zhonya.lux.r", "Use Zhonya on Lux R"),
                    new ValueCheckbox(true, "defensives.zhonya.nunu.r", "Use Zhonya on Nunu R"),
                    new ValueCheckbox(true, "defensives.zhonya.morgana.r", "Use Zhonya on Morgana R (when stun)")
                }
            };

            feature.ToActivatorMenu();
            Features.Add(feature);

            #endregion

            #region Potions

            feature = new Feature
            {
                NameFeature = "Potions",
                MenuValueStyleList = new List<ValueAbstract>()
                {
                    new ValueCheckbox(true, "potions.healthpotion", "Use Health Potion"),
                    new ValueSlider(99, 1, 60, "potions.healthpotion.health%", "Use Health Potion when my health% is at:"),

                    new ValueCheckbox(true, "potions.biscuitpotion", "Use Biscuit Potion", true),
                    new ValueSlider(99, 1, 60, "potions.biscuitpotion.health%", "Use Biscuit Potion when my health% is at:"),

                    new ValueCheckbox(true, "potions.corruptingpotion", "Use Corrupting Potion", true),
                    new ValueSlider(99, 1, 60, "potions.corruptingpotion.health%", "Use Corrupting Potion when my health% is at:"),

                    new ValueCheckbox(true, "potions.hunterspotion", "Use Hunters Potion", true),
                    new ValueSlider(99, 1, 70, "potions.hunterspotion.health%", "Use Hunters Potion when my health% is at:"),

                    new ValueCheckbox(true, "potions.refillablepotion", "Use Refillable Potion", true),
                    new ValueSlider(99, 1, 70, "potions.refillablepotion.health%", "Use Refillable Potion when my health% is at:"),

                }
            };

            feature.ToActivatorMenu();
            Features.Add(feature);

            #endregion

            #region Speed

            feature = new Feature
            {
                NameFeature = "Speed",
                MenuValueStyleList = new List<ValueAbstract>()
                {
                    new ValueCheckbox(true, "speed.talisma", "Use Talisma"),

                    new ValueCheckbox(true, "speed.glory", "Use Glory"),
                }
            };

            feature.ToActivatorMenu();
            Features.Add(feature);

            #endregion

            #region Misc

            feature = new Feature
            {
                NameFeature = "Misc",
                MenuValueStyleList = new List<ValueAbstract>()
                {
                    new ValueCheckbox(true, "misc.itemsonlaneclear", "Use Items on Lane Clear"),
                    new ValueCheckbox(true, "misc.itemsonjungleclear", "Use Items on Jungle Clear")
                }
            };

            feature.ToActivatorMenu();
            Features.Add(feature);

            #endregion

            start = Features.First(it => it.NameFeature == "Start");
            offensives = Features.First(it => it.NameFeature == "Offensives");
            defensives = Features.First(it => it.NameFeature == "Defensives");
            speed = Features.First(it => it.NameFeature == "Speed");
            potions = Features.First(it => it.NameFeature == "Potions");
            summoners = Features.First(it => it.NameFeature == "Summoners");
            misc = Features.First(it => it.NameFeature == "Misc");

            return;
        }

        void InitVariables()
        {
            _itemUtil = new ItemUtil();

            #region Summoners

            flash = SpellsUtil.GetSkillshotSpell(SpellsUtil.Summoners.Flash);

            ignite = SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Ignite);
            exhaust = SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Exhaust);
            smite = SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Smite);

            heal = SpellsUtil.GetActiveSpell(SpellsUtil.Summoners.Heal);
            barrier = SpellsUtil.GetActiveSpell(SpellsUtil.Summoners.Barrier);
            ghost = SpellsUtil.GetActiveSpell(SpellsUtil.Summoners.Ghost);
            cleanse = SpellsUtil.GetActiveSpell(SpellsUtil.Summoners.Cleanse);

            #endregion

            #region Offensives

            hextech = _itemUtil.GetItem(ItemId.Hextech_Gunblade, 700);
            botrk = _itemUtil.GetItem(ItemId.Blade_of_the_Ruined_King, 550);
            bilgewater = _itemUtil.GetItem(ItemId.Bilgewater_Cutlass, 550);
            tiamat = _itemUtil.GetItem(ItemId.Tiamat_Melee_Only, 325);//range = 400
            hydra = _itemUtil.GetItem(ItemId.Ravenous_Hydra_Melee_Only, 325);//range = 400
            titanic = _itemUtil.GetItem(3053, 75);//range = 150
            youmuus = _itemUtil.GetItem(ItemId.Youmuus_Ghostblade);

            #endregion

            #region Defensives

            faceMountain = _itemUtil.GetItem(ItemId.Face_of_the_Mountain, 600);
            mikael = _itemUtil.GetItem(ItemId.Mikaels_Crucible, 600);
            solari = _itemUtil.GetItem(ItemId.Locket_of_the_Iron_Solari, 600);
            randuin = _itemUtil.GetItem(ItemId.Randuins_Omen, 450);//range = 500
            scimitar = _itemUtil.GetItem(ItemId.Mercurial_Scimitar);
            qss = _itemUtil.GetItem(ItemId.Quicksilver_Sash);
            seraph = _itemUtil.GetItem(3040);
            zhonya = _itemUtil.GetItem(ItemId.Zhonyas_Hourglass);

            #endregion

            #region Speed

            talisma = _itemUtil.GetItem(ItemId.Talisman_of_Ascension);
            righteousGlory = _itemUtil.GetItem(ItemId.Righteous_Glory, 600);

            #endregion

            #region Potions

            healthPotion = _itemUtil.GetItem(ItemId.Health_Potion);
            biscuitPotion = _itemUtil.GetItem(2010);
            corruptingPotion = _itemUtil.GetItem(2033);
            huntersPotion = _itemUtil.GetItem(2032);
            refillablePotion = _itemUtil.GetItem(2031);

            #endregion

            _itens = new ItemController();
            _summoners = new SummonersController();
        }

        #endregion

        #region Logic

        private void Game_OnTick(EventArgs args)
        {
            if (Player.IsDead || this == null || !start.IsChecked("enable")) return;

            Target = TargetSelector.GetTarget(_range, _damageType);

            //Dragon, Baron, KS
            _summoners.AutoSmite();

            //Potions time
            if (!(Player.IsInShopRange() || Player.HasBuff("RegenerationPotion") || Player.HasBuff("HealthPotion") || Player.HasBuff("BiscuitPotion") || Player.HasBuff("ItemCrystalFlask") || Player.HasBuff("ItemCrystalFlaskJungle") || Player.HasBuff("CorruptingPotion")))
            {
                _itens.AutoHealthPotion();
                _itens.AutoBiscuitPotion();
                _itens.AutoCorruptingPotion();
                _itens.AutoHuntersPotion();
                _itens.AutoRefillablePotion();
            }

            if (Target != null && Target.IsValidTarget() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                //Combo mode 3:)

                //Ghost usage
                _summoners.AutoGhost();

                //Exhaust usage
                if (exhaust != null && exhaust.IsReady() && exhaust.IsInRange(Target) && summoners.IsChecked("summoners.exhaust")) exhaust.Cast(Target);

                //Smite usage
                if (smite != null && smite.IsReady() && smite.IsInRange(Target))
                {
                    if (summoners.IsChecked("summoners.smite") && summoners.IsChecked("summoners.smite.enemies"))
                    {
                        if (smite.Name.Contains("gank")) smite.Cast(Target);
                        else if (smite.Name.Contains("duel") && Player.IsInAutoAttackRange(Target)) smite.Cast(Target);
                    }
                }

                //Offensives
                _itens.AutoBilgeBtrk();
                _itens.AutoHextechGunBlade();
                _itens.AutoTiamatHydra();
                _itens.AutoTitanicHydra();
                _itens.AutoYoumuusGhostBlade();

                _itens.AutoRanduin();

                _itens.AutoRighteousGlory();
                _itens.AutoTalisma();
            }

            if (Player.CountEnemiesInRange(1350) > 0)
            {
                //Defensives

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                {
                    if (smite != null && smite.Name.Contains("gank") && smite.IsReady() && Target.IsValidTarget(smite.Range)) smite.Cast(Target);

                    if (EntityManager.Heroes.Enemies.Any(it => !it.IsDead && it.IsValidTarget(randuin.Range))) randuin.Cast();

                    righteousGlory.Cast();

                    talisma.Cast();
                }

                _itens.AutoScimitarQSS();
                _itens.AutoZhonya();
                _itens.AutoSeraphEmbrace();
                _itens.AutoSolari();
                _itens.AutoMikael();
                _itens.AutoFaceOfTheMountain();

                //Summoners
                _summoners.AutoIgnite();
                _summoners.AutoCleanse();
                _summoners.AutoBarrier();
                _summoners.AutoHeal();
            }

            //Lane Clear
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                //Items usage
                if (misc.IsChecked("misc.itemsonlaneclear"))
                {
                    var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, 400);

                    if (minions.Count() >= 3 && minions.Any(it => it.Health > 150))
                    {
                        if (tiamat.IsOwned() && tiamat.IsReady()) tiamat.Cast();
                        if (hydra.IsOwned() && hydra.IsReady()) hydra.Cast();
                    }
                }
            }

            //Jungle Clear
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                //Smiting mobs
                _summoners.AutoSmiteMob();

                //Items usage
                if (misc.IsChecked("misc.itemsonjungleclear"))
                {
                    var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, 650);

                    if (minions.Count(it => it.Distance(Player) <= 400) == minions.Count() && minions.Any(it => it.Health > 150))
                    {
                        if (tiamat.IsOwned() && tiamat.IsReady()) tiamat.Cast();
                        if (hydra.IsOwned() && hydra.IsReady()) hydra.Cast();
                        if (minions.Any(it => it.Health >= 200) && titanic.IsOwned() && titanic.IsReady()) titanic.Cast();
                    }
                }
            }

            return;
        }

        private void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly || sender.IsMe || Player.Distance(sender) > args.SData.CastRangeDisplayOverride || !(sender is AIHeroClient)) return;

            //Is dangerous spell ?
            var spell = _autoZhonyaSpells.FirstOrDefault(it => it == sender.BaseSkinName.ToLower() + "." + args.Slot.ToString().ToLower());

            //No ? return;
            if (spell == null) return;

            //Yes ? Let's go 3:)

            //Summoners on dangerous

            if (spell != "morgana.r" && spell != "nunu.r" && spell != "zed.r")
            {
                if (barrier != null && summoners.IsChecked("summoners.barrier.dangerousspells") && barrier.IsReady() && (Player.Health + (95 + (20 * Player.Level))) - ((AIHeroClient)sender).GetSpellDamage(Player, args.Slot) > 100 && (Player.Health + (95 + (20 * Player.Level))) - ((AIHeroClient)sender).GetSpellDamage(Player, args.Slot) <= Player.MaxHealth)
                {
                    if (args.Target != null && args.Target.IsMe) { barrier.Cast(); return; }

                    int delay = new int();

                    if (Player.Distance(sender) >= 1000)
                    {
                        delay = (int)((((Player.Distance(sender)) / args.SData.MissileSpeed * 1000) + args.SData.SpellCastTime - Game.Ping) / 1.5);
                    }
                    else if (Player.Distance(sender) >= 400)
                    {
                        delay = (int)((((Player.Distance(sender)) / args.SData.MissileSpeed * 1000) + args.SData.SpellCastTime - Game.Ping) / 2);
                    }

                    EloBuddy.SDK.Core.DelayAction(() => SummonersOnHit(sender, args), delay);

                    return;
                }

                else if (heal != null && summoners.IsChecked("summoners.heal.dangerousspells") && heal.IsReady() && (Player.Health + (75 + (15 * Player.Level))) - ((AIHeroClient)sender).GetSpellDamage(Player, args.Slot) > 100 && (Player.Health + (75 + (15 * Player.Level))) - ((AIHeroClient)sender).GetSpellDamage(Player, args.Slot) <= Player.MaxHealth)
                {
                    if (args.Target != null && args.Target.IsMe) { heal.Cast(); return; }

                    int delay = new int();

                    if (Player.Distance(sender) >= 1000)
                    {
                        delay = (int)((((Player.Distance(sender)) / args.SData.MissileSpeed * 1000) + args.SData.SpellCastTime - Game.Ping) / 1.5);
                    }
                    else if (Player.Distance(sender) >= 400)
                    {
                        delay = (int)((((Player.Distance(sender)) / args.SData.MissileSpeed * 1000) + args.SData.SpellCastTime - Game.Ping) / 2);
                    }

                    EloBuddy.SDK.Core.DelayAction(() => SummonersOnHit(sender, args, true), delay);

                    return;
                }
            }

            //This spells isn't checked on menu ? return;
            if (!defensives.IsChecked("defensives.zhonya." + spell)) return;

            //Zhonya on dangerous
            else if (zhonya.IsOwned() && zhonya.IsReady())
            {
                if (spell == "morgana.r")
                {
                    EloBuddy.SDK.Core.DelayAction(delegate
                    {
                        if (Player.Distance(sender) < args.SData.CastRangeDisplayOverride) zhonya.Cast();
                    }, 2700 - Game.Ping);
                }

                else if (spell == "nunu.r")
                {
                    EloBuddy.SDK.Core.DelayAction(delegate
                    {
                        if (Player.Distance(sender) < args.SData.CastRangeDisplayOverride) zhonya.Cast();
                    }, 600 - Game.Ping);
                }

                else if (spell == "zed.r")
                {
                    EloBuddy.SDK.Core.DelayAction(() => ZhonyaOnHit(sender, args), 450 - Game.Ping);
                }

                else
                {
                    var delay = (int)(((Player.Distance(sender) - 300) / args.SData.MissileSpeed * 1000) + args.SData.SpellCastTime - 50 - Game.Ping);

                    EloBuddy.SDK.Core.DelayAction(() => ZhonyaOnHit(sender, args), delay);
                }
            }

            return;
        }

        //----------------------------------------|| Methods ||-------------------------------------

        private void ZhonyaOnHit(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.Target != null)
            {
                if (args.Target.IsMe) zhonya.Cast();
                return;
            }

            var polygons = new Geometry.Polygon[] { new Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.LineWidth), new Geometry.Polygon.Circle(args.End, args.SData.CastRadius) };

            if (polygons.Any(it => it.IsInside(Player))) zhonya.Cast();

            return;
        }

        private void SummonersOnHit(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args, bool useHeal = false)
        {
            var polygons = new Geometry.Polygon[] { new Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.LineWidth), new Geometry.Polygon.Circle(args.End, args.SData.CastRadius) };

            if (polygons.Any(it => it.IsInside(Player)))
            {
                if (useHeal) heal.Cast();
                else barrier.Cast();
            }

            return;
        }

        #endregion
    }
}
