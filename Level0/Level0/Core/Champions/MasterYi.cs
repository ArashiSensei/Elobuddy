using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using LevelZero.Controller;
using LevelZero.Model;
using LevelZero.Model.Values;
using LevelZero.Util;
using SharpDX;

namespace LevelZero.Core.Champions
{
    class MasterYi : PluginModel
    {
        static AIHeroClient Player = EloBuddy.Player.Instance;
        static List<string> MenuSpells = new List<string>();
        static List<string> DodgeSpells = new List<string>() { "SorakaQ", "SorakaE", "TahmKenchW", "TahmKenchQ", "Bushwhack", "ForcePulse", "KarthusFallenOne", "KarthusWallOfPain", "KarthusLayWasteA1", "KarmaWMantra", "KarmaQMissileMantra", "KarmaSpiritBind", "KarmaQ", "JinxW", "JinxE", "JarvanIVGoldenAegis", "HowlingGaleSpell", "SowTheWind", "ReapTheWhirlwind", "IllaoiE", "HeimerdingerUltWDummySpell", "HeimerdingerUltEDummySpell", "HeimerdingerW", "HeimerdingerE", "HecarimUlt", "HecarimRampAttack", "GravesQLineSpell", "GravesQLineMis", "GravesClusterShot", "GravesSmokeGrenade", "GangplankR", "GalioIdolOfDurand", "GalioResoluteSmite", "FioraE", "EvelynnR", "EliseHumanE", "EkkoR", "EkkoW", "EkkoQ", "DravenDoubleShot", "InfectedCleaverMissileCast", "DariusExecute", "DariusAxeGrabCone", "DariusNoxianTacticsONH", "DariusCleave", "PhosphorusBomb", "MissileBarrage", "BraumQ", "BrandFissure", "BardR", "BardQ", "AatroxQ", "AatroxE", "AzirE", "AzirEWrapper", "AzirQWrapper", "AzirQ", "AzirR", "Pulverize", "AhriSeduce", "CurseoftheSadMummy", "InfernalGuardian", "Incinerate", "Volley", "EnchantedCrystalArrow", "BraumRWrapper", "CassiopeiaPetrifyingGaze", "FeralScream", "Rupture", "EzrealEssenceFlux", "EzrealMysticShot", "EzrealTrueshotBarrage", "FizzMarinerDoom", "GnarW", "GnarBigQMissile", "GnarQ", "GnarR", "GragasQ", "GragasE", "GragasR", "RiftWalk", "LeblancSlideM", "LeblancSlide", "LeonaSolarFlare", "UFSlash", "LuxMaliceCannon", "LuxLightStrikeKugel", "LuxLightBinding", "yasuoq3w", "VelkozE", "VeigarEventHorizon", "VeigarDarkMatter", "VarusR", "ThreshQ", "ThreshE", "ThreshRPenta", "SonaQ", "SonaR", "ShenShadowDash", "SejuaniGlacialPrisonCast", "RivenMartyr", "JavelinToss", "NautilusSplashZone", "NautilusAnchorDrag", "NamiR", "NamiQ", "DarkBindingMissile", "StaticField", "RocketGrab", "RocketGrabMissile", "timebombenemybuff", "NocturneUnspeakableHorror", "SyndraQ", "SyndraE", "SyndraR", "VayneCondemn", "Dazzle", "Overload", "AbsoluteZero", "IceBlast", "LeblancChaosOrb", "JudicatorReckoning", "KatarinaQ", "NullLance", "Crowstorm", "FiddlesticksDarkWind", "BrandWildfire", "Disintegrate", "FlashFrost", "Frostbite", "AkaliMota", "InfiniteDuress", "PantheonW", "blindingdart", "JayceToTheSkies", "IreliaEquilibriumStrike", "maokaiunstablegrowth", "nautilusgandline", "runeprison", "WildCards", "BlueCardAttack", "RedCardAttack", "GoldCardAttack", "AkaliShadowDance", "Headbutt", "PowerFist", "BrandConflagration", "CaitlynYordleTrap", "CaitlynAceintheHole", "CassiopeiaNoxiousBlast", "CassiopeiaMiasma", "CassiopeiaTwinFang", "Feast", "DianaArc", "DianaTeleport", "EliseHumanQ", "EvelynnE", "Terrify", "FizzPiercingStrike", "Parley", "GarenQAttack", "GarenR", "IreliaGatotsu", "IreliaEquilibriumStrike", "SowTheWind", "JarvanIVCataclysm", "JaxLeapStrike", "JaxEmpowerTwo", "JaxCounterStrike", "JayceThunderingBlow", "KarmaSpiritBind", "NetherBlade", "KatarinaR", "JudicatorRighteousFury", "KennenBringTheLight", "LeblancChaosOrbM", "BlindMonkRKick", "LeonaZenithBlade", "LeonaShieldOfDaybreak", "LissandraW", "LissandraQ", "LissandraR", "LuluQ", "LuluW", "LuluE", "LuluR", "SeismicShard", "AlZaharMaleficVisions", "AlZaharNetherGrasp", "MaokaiUnstableGrowth", "MordekaiserMaceOfSpades", "MordekaiserChildrenOfTheGrave", "SoulShackles", "NamiW", "NasusW", "NautilusGrandLine", "Takedown", "NocturneParanoia", "PoppyDevastatingBlow", "PoppyHeroicCharge", "QuinnE", "PuncturingTaunt", "RenektonPreExecute", "SpellFlux", "SejuaniWintersClaw", "TwoShivPoisen", "Fling", "SkarnerImpale", "SonaHymnofValor", "SwainTorment", "SwainDecrepify", "BlindingDart", "OrianaIzunaCommand", "OrianaDetonateCommand", "DetonatingShot", "BusterShot", "TrundleTrollSmash", "TrundlePain", "MockingShout", "Expunge", "UdyrBearStance", "UrgotHeatseekingLineMissile", "UrgotSwap2", "VeigarBalefulStrike", "VeigarPrimordialBurst", "ViR", "ViktorPowerTransfer", "VladimirTransfusion", "VolibearQ", "HungeringStrike", "XenZhaoComboTarget", "XenZhaoSweep", "YasuoQ3W", "YasuoQ3Mis", "YasuoQ3", "YasuoRKnockUpComboW" };
        static Menu EOMenu;

        Spell.Targeted Q { get { return (Spell.Targeted)Spells[0]; } }
        Spell.Active W { get { return (Spell.Active)Spells[1]; } }
        Spell.Active E { get { return (Spell.Active)Spells[2]; } }
        Spell.Active R { get { return (Spell.Active)Spells[3]; } }

        public override void Init()
        {
            InitVariables();
            InitEvents();
        }

        public override void InitEvents()
        {
            base.InitEvents();
        }

        public override void InitVariables()
        {
            Spells = new List<Spell.SpellBase>
            {
                new Spell.Targeted(SpellSlot.Q, 600),
                new Spell.Active(SpellSlot.W),
                new Spell.Active(SpellSlot.E),
                new Spell.Active(SpellSlot.R)
            };

            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Q, new float[] { 0, 25, 60, 95, 130, 165 }, new [] { 0, 1f, 1f, 1f, 1f, 1f }, DamageType.Physical),
                new SpellDamage(E, new float[] { 0, 10, 15, 20, 25, 30 }, new [] { 0, 0.1f, 0.125f, 0.15f, 0.175f, 0.2f }, DamageType.True)
            };

            InitMenu();

            DamageIndicator.Initialize(DamageUtil.GetComboDamage);

            new SkinController(7);
        }

        public override void InitMenu()
        {
            var feature = new Feature
            {
                NameFeature = "Draw",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(false, "disable", "Disable"),
                    new ValueCheckbox(true, "dmgIndicator", "Show Damage Indicator"),
                    new ValueCheckbox(true, "draw.q", "Draw Q")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Combo",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "combo.q", "Combo Q"),
                    new ValueCheckbox(true, "combo.q.smartq", "SmartQ"),
                    new ValueCheckbox(true, "combo.q.saveqtododgespells", "Save Q to dodge spells"),
                    new ValueCheckbox(true, "combo.waareset", "Use W AA Reset"),
                    new ValueCheckbox(true,  "combo.e", "Combo E"),
                    new ValueCheckbox(true,  "combo.r", "Combo R")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Harass",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "harass.q", "Harass Q"),
                    new ValueCheckbox(false, "harass.w", "Use W AA Reset"),
                    new ValueCheckbox(true,  "harass.e", "Harass E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Lane Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "laneclear.q", "Lane Clear Q"),
                    new ValueCheckbox(true,  "laneclear.q.jimwd", "Just Q if minion will die"),
                    new ValueSlider(4, 1, 3, "laneclear.q.minminions", "Min minions to Q"),
                    new ValueSlider(100, 1, 30, "laneclear.mana%", "Lane Clear MinMana%")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Jungle Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "jungleclear.q", "Jungle Clear Q"),
                    new ValueCheckbox(true,  "jungleclear.e", "Jungle Clear E"),
                    new ValueSlider(100, 1, 30, "jungleclear.mana%", "Jungle Clear MinMana%")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Smite Usage",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true, "smiteusage.usesmite", "Use smite"),
                    new ValueCheckbox(true, "smiteusage.red", "Red"),
                    new ValueCheckbox(true, "smiteusage.blue", "Blue"),
                    new ValueCheckbox(true, "smiteusage.wolf", "Wolf"),
                    new ValueCheckbox(true, "smiteusage.gromp", "Gromp"),
                    new ValueCheckbox(true, "smiteusage.raptor", "Raptor"),
                    new ValueCheckbox(true, "smiteusage.krug", "Krug")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Misc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "misc.ks", "KS"),
                    new ValueCheckbox(true, "misc.gapcloser", "Q on enemy gapcloser"),
                    new ValueCheckbox(true, "misc.autoignite", "Auto Ignire")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            EOMenu = Globals.MENU.AddSubMenu("Q/W Evade Options", "Q/W Evade Options");

            foreach (AIHeroClient hero in EntityManager.Heroes.Enemies)
            {
                EOMenu.AddGroupLabel(hero.BaseSkinName);
                {
                    foreach (SpellDataInst spell in hero.Spellbook.Spells)
                    {
                        if (DodgeSpells.Any(el => el == spell.SData.Name))
                        {
                            EOMenu.Add(spell.Name, new Slider(hero.BaseSkinName + " : " + spell.Slot.ToString() + " : " + spell.Name, 3, 0, 3));
                            MenuSpells.Add(spell.Name);
                        }
                    }
                }

                EOMenu.AddSeparator();
            }
        }

        public override void OnUpdate(EventArgs args)
        {
            base.OnUpdate(args);

            var smiteusage = Features.First(it => it.NameFeature == "Smite Usage");
            var misc = Features.First(it => it.NameFeature == "Misc");

            //---------------------------------------------Smite Usage---------------------------------------------

            if (SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Smite) != null)
            {
                var Smite = SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Smite);

                if (Smite.IsReady() && smiteusage.IsChecked("smiteusage.usesmite"))
                {
                    Obj_AI_Minion Mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Smite.Range).FirstOrDefault();

                    if (Mob != default(Obj_AI_Minion))
                    {
                        bool kill = GetSmiteDamage() >= Mob.Health;

                        if (kill)
                        {
                            if ((Mob.Name.Contains("SRU_Dragon") || Mob.Name.Contains("SRU_Baron"))) Smite.Cast(Mob);
                            else if (Mob.Name.StartsWith("SRU_Red") && smiteusage.IsChecked("smiteusage.red")) Smite.Cast(Mob);
                            else if (Mob.Name.StartsWith("SRU_Blue") && smiteusage.IsChecked("smiteusage.blue")) Smite.Cast(Mob);
                            else if (Mob.Name.StartsWith("SRU_Murkwolf") && smiteusage.IsChecked("smiteusage.wolf")) Smite.Cast(Mob);
                            else if (Mob.Name.StartsWith("SRU_Krug") && smiteusage.IsChecked("smiteusage.krug")) Smite.Cast(Mob);
                            else if (Mob.Name.StartsWith("SRU_Gromp") && smiteusage.IsChecked("smiteusage.gromp")) Smite.Cast(Mob);
                            else if (Mob.Name.StartsWith("SRU_Razorbeak") && smiteusage.IsChecked("smiteusage.raptor")) Smite.Cast(Mob);
                        }
                    }
                }
            }

            //------------------------------------------------KS------------------------------------------------

            if (misc.IsChecked("misc.ks") && EntityManager.Heroes.Enemies.Any(it => Q.IsInRange(it))) KS();

            //-----------------------------------------------Auto Ignite----------------------------------------

            if (Features.First(it => it.NameFeature == "Misc").IsChecked("misc.autoignite") && SpellsUtil.GetTargettedSpell("summonerdot", 600) != null)
            {
                var Ignite = SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Ignite);

                if (Ignite.IsReady())
                {
                    var IgniteEnemy = EntityManager.Heroes.Enemies.FirstOrDefault(it => DamageLibrary.GetSummonerSpellDamage(Player, it, DamageLibrary.SummonerSpells.Ignite) >= it.Health - 30);

                    if (IgniteEnemy != null)
                    {
                        if ((IgniteEnemy.Distance(Player) >= 300 || Player.HealthPercent <= 40))
                        {
                            Ignite.Cast(IgniteEnemy);
                        }
                    }
                }
            }

            return;
        }

        public override void OnDraw(EventArgs args)
        {
            base.OnDraw(args);

            var draw = Features.Find(f => f.NameFeature == "Draw");

            if (draw.IsChecked("disable"))
            {
                DamageIndicator.Enabled = false;
                return;
            }

            if (draw.IsChecked("draw.q"))
                Circle.Draw(Q.IsReady() ? Color.Blue : Color.Red, Q.Range, Player.Position);

            DamageIndicator.Enabled = draw.IsChecked("dmgIndicator");

        }

        /*
            Spells[0] = Q - Useless
            Spells[1] = W
            Spells[2] = E
            Spells[3] = R
        */
        public override void OnCombo()
        {
            var combo = Features.First(it => it.NameFeature == "Combo");

            var Target = TargetSelector.GetTarget(900, DamageType.Physical);

            if (Target == null) return;

            var itens = new ItemController();

            if (Player.HasBuffOfType(BuffType.Charm) || Player.HasBuffOfType(BuffType.Blind) || Player.HasBuffOfType(BuffType.Fear) || Player.HasBuffOfType(BuffType.Polymorph) || Player.HasBuffOfType(BuffType.Silence) || Player.HasBuffOfType(BuffType.Sleep) || Player.HasBuffOfType(BuffType.Snare) || Player.HasBuffOfType(BuffType.Stun) || Player.HasBuffOfType(BuffType.Suppression) || Player.HasBuffOfType(BuffType.Taunt)) { itens.CastScimitarQSS(); }

            if (combo.IsChecked("combo.q") && Q.IsReady() && Target.IsValidTarget(Q.Range))
            {
                if (combo.IsChecked("combo.q.smartq")) { QLogic(Target); }
                else if (combo.IsChecked("combo.q.saveqtododgespells")) { }
                else { Q.Cast(Target); }
            }

            if (SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Smite) != null)
            {
                var Smite = SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Smite);

                if (Target.IsValidTarget(Smite.Range) && Smite.IsReady())
                {
                    if (Smite.Name.Contains("gank")) Smite.Cast(Target);
                    else if (Smite.Name.Contains("duel") && Player.IsInAutoAttackRange(Target)) Smite.Cast(Target);
                }
            }

            if (combo.IsChecked("combo.r") && R.IsReady() && Player.Distance(Target) <= Player.GetAutoAttackRange(Target) + 300) { R.Cast(); }

            if (combo.IsChecked("combo.e") && E.IsReady() && Player.IsInAutoAttackRange(Target)) E.Cast();

            if (Target.IsValidTarget(Q.Range)) itens.CastYoumuusGhostBlade();

            itens.CastBilgeBtrk(Target);
            itens.CastTiamatHydra(Target);
            itens.CastRanduin(Target);
            itens.CastHextechGunBlade(Target);
            itens.CastTitanicHydra(Target);

            return;
        }

        public override void OnHarass()
        {
            var harass = Features.First(it => it.NameFeature == "Harass");
            var Target = TargetSelector.GetTarget(900, DamageType.Physical);
            if (Target == null) return;

            if (harass.IsChecked("harass.q") && Q.IsReady() && Target.IsValidTarget(Q.Range)) Q.Cast(Target);

            if (harass.IsChecked("harass.e") && E.IsReady() && Player.IsInAutoAttackRange(Target)) E.Cast();

            return;
        }

        public override void OnLaneClear()
        {
            var laneclear = Features.First(it => it.NameFeature == "Lane Clear");
            var itens = new ItemController();

            bool UseItem = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, 400).Count() >= 3;
            if (UseItem) itens.CastTiamatHydra();

            if (Player.ManaPercent <= laneclear.SliderValue("laneclear.mana%")) return;

            if (laneclear.IsChecked("laneclear.q"))
            {
                IEnumerable<Obj_AI_Minion> ListMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition, 1000).OrderBy(minion => minion.Distance(Player));
                int hits = new int();

                if (ListMinions.Any() && ListMinions.Count() >= laneclear.SliderValue("laneclear.q.minminions"))
                {
                    if (!(ListMinions.First().Distance(Player) > Q.Range))
                    {
                        hits += 1;

                        for (int i = 0; i < ListMinions.Count(); i++)
                        {
                            if (i + 1 == ListMinions.Count()) break;
                            else if (ListMinions.ElementAt(i).Distance(ListMinions.ElementAt(i + 1)) <= 300) { hits += 1; }
                            else break;
                        }

                        if (hits >= laneclear.SliderValue("laneclear.q.minminions"))
                        {
                            if (laneclear.IsChecked("laneclear.q.jimwd"))
                            {
                                if ((DamageUtil.GetSpellDamage(ListMinions.First(), SpellSlot.Q) > ListMinions.First().Health || DamageUtil.GetSpellDamage(ListMinions.ElementAt(1), SpellSlot.Q) > ListMinions.ElementAt(1).Health)) Q.Cast(ListMinions.First());
                            }
                            else { Q.Cast(ListMinions.First()); }
                        }
                    }
                }
            }

            return;
        }

        public override void OnJungleClear()
        {
            var jungleclear = Features.First(it => it.NameFeature == "Jungle Clear");

            if (Player.ManaPercent < jungleclear.SliderValue("jungleclear.mana%")) return;

            if (EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Player.GetAutoAttackRange()).Any() && jungleclear.IsChecked("jungleclear.e")) E.Cast();

            var itens = new ItemController();

            bool UseItem = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, 400).Count() >= 1;
            if (UseItem) itens.CastTiamatHydra();

            return;
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            base.OnAfterAttack(target, args);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && Q.IsReady())
            {
                var jungleclear = Features.First(it => it.NameFeature == "Jungle Clear");

                if (Player.ManaPercent < jungleclear.SliderValue("jungleclear.mana%")) return;

                var QMinion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Player.GetAutoAttackRange()).FirstOrDefault();

                if (QMinion != null) Q.Cast(QMinion);

                return;
            }

            if (W.IsReady() && Player.Distance(target) <= Player.GetAutoAttackRange() - 50)
            {
                var AARCombo = Features.First(it => it.NameFeature == "Combo").IsChecked("combo.waareset");

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && AARCombo)
                {
                    if (W.Cast())
                    {
                        Orbwalker.ResetAutoAttack();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                    }

                    return;
                }

                var AARHarass = Features.First(it => it.NameFeature == "Harass").IsChecked("harass.waareset");

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && AARHarass)
                {
                    if (W.Cast())
                    {
                        Orbwalker.ResetAutoAttack();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                    }

                    return;
                }
            }

            return;
        }

        public override void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            base.OnProcessSpell(sender, args);

            if (sender.IsValidTarget() && sender.IsEnemy && MenuSpells.Any(el => el == args.SData.Name) && Player.Distance(sender) <= args.SData.CastRange)
            {
                var Target = TargetSelector.GetTarget(900, DamageType.Physical); 

                if (Q.IsReady() && (EOMenu[args.SData.Name].Cast<Slider>().CurrentValue == 1 || EOMenu[args.SData.Name].Cast<Slider>().CurrentValue == 3))
                {
                    if (args.SData.Name == "JaxCounterStrike") { EloBuddy.SDK.Core.DelayAction(() => Dodge(Target), 2000 - Game.Ping - 100); return; }

                    if (args.SData.Name == "KarthusFallenOne") { EloBuddy.SDK.Core.DelayAction(() => Dodge(Target), 3000 - Game.Ping - 100); return; }

                    if (args.SData.Name == "ZedR" && args.Target.IsMe) { EloBuddy.SDK.Core.DelayAction(() => Dodge(Target), 750 - Game.Ping - 100); return; }

                    if (args.SData.Name == "SoulShackles") { EloBuddy.SDK.Core.DelayAction(() => Dodge(Target), 3000 - Game.Ping - 100); return; }

                    if (args.SData.Name == "AbsoluteZero") { EloBuddy.SDK.Core.DelayAction(() => Dodge(Target), 3000 - Game.Ping - 100); return; }

                    if (args.SData.Name == "NocturneUnspeakableHorror" && args.Target.IsMe) { EloBuddy.SDK.Core.DelayAction(() => Dodge(Target), 2000 - Game.Ping - 100); return; }

                    EloBuddy.SDK.Core.DelayAction(() => Q.Cast(Target), (int)args.SData.SpellCastTime - Game.Ping - 100);

                    return;
                }

                else if (W.IsReady() && Player.IsFacing(sender) && EOMenu[args.SData.Name].Cast<Slider>().CurrentValue > 1 && (args.Target.IsMe || new Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.LineWidth).IsInside(Player) || new Geometry.Polygon.Circle(args.End, args.SData.CastRadius).IsInside(Player)))
                {
                    int delay = (int)(Player.Distance(sender) / ((args.SData.MissileMaxSpeed + args.SData.MissileMinSpeed) / 2) * 1000) - 150 + (int)args.SData.SpellCastTime;

                    if (args.SData.Name != "ZedR" && args.SData.Name != "NocturneUnpeakableHorror")
                    {
                        EloBuddy.SDK.Core.DelayAction(() => W.Cast(), delay);
                        if (Target != null) EloBuddy.SDK.Core.DelayAction(() => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, Target), delay + 100);
                    }
                    return;
                }
            }

            return;
        }

        public override void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            base.OnGapCloser(sender, e);

            if (sender.IsMe) return;

            var gapclose = Features.First(it => it.NameFeature == "Misc").IsChecked("misc.gapcloser");

            if (Q.IsReady() && sender.IsValidTarget(Q.Range) && gapclose) Q.Cast(sender);

            return;
        }

        //------------------------------------|| Extension ||--------------------------------------

        //----------------------------------------QLogic()-----------------------------------------

        void QLogic(Obj_AI_Base Target)
        {
            if (Target.IsDashing()) Q.Cast(Target);
            if (Target.HealthPercent <= 30) Q.Cast(Target);
            if (Player.HealthPercent <= 30) Q.Cast(Target);
            if (DamageUtil.GetSpellDamage(Target, SpellSlot.Q) >= Target.Health) Q.Cast(Target);
        }

        //----------------------------------------Dodge()------------------------------------------

        void Dodge(AIHeroClient Target)
        {
            if (Target != null)
            {
                if (Q.IsInRange(Target))
                {
                    Q.Cast(Target);
                    return;
                }
            }

            var champ = EntityManager.Heroes.Enemies.FirstOrDefault(it => it.IsValidTarget(Q.Range));

            if (champ != null) { Q.Cast(champ); return; }

            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(it => it.IsValidTarget(Q.Range));

            if (minion != null) { Q.Cast(minion); return; }

            return;
        }

        //------------------------------------GetSmiteDamage()-------------------------------------

        float GetSmiteDamage()
        {
            float damage = new float(); //Arithmetic Progression OP :D

            if (Player.Level < 10) damage = 360 + (Player.Level - 1) * 30;

            else if (Player.Level < 15) damage = 280 + (Player.Level - 1) * 40;

            else if (Player.Level < 19) damage = 150 + (Player.Level - 1) * 50;

            return damage;
        }

        //-------------------------------------------KS--------------------------------------------

        void KS()
        {
            if (Q.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && DamageUtil.GetSpellDamage(enemy, SpellSlot.Q, true, true) >= enemy.Health);
                if (bye != null) { Q.Cast(bye); return; }
            }

            if (SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Smite) != null)
            {
                var Smite = SpellsUtil.GetTargettedSpell(SpellsUtil.Summoners.Smite);

                if (Smite.Name.Contains("gank") && Smite.IsReady())
                {
                    var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Smite.Range) && DamageLibrary.GetSummonerSpellDamage(Player, enemy, DamageLibrary.SummonerSpells.Smite) >= enemy.Health);
                    if (bye != null) { Smite.Cast(bye); return; }
                }
            }
        }
    }
}