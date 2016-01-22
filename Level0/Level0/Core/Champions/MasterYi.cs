using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Events;
using LevelZero.Controller;
using LevelZero.Model;
using LevelZero.Model.Values;
using LevelZero.Util;
using SharpDX;
using Circle = EloBuddy.SDK.Rendering.Circle;
using Activator = LevelZero.Controller.Activator;

namespace LevelZero.Core.Champions
{
    class MasterYi : PluginModel
    {
        readonly List<string> DodgeSpells = new List<string>() { "SorakaQ", "SorakaE", "TahmKenchW", "TahmKenchQ", "Bushwhack", "ForcePulse", "KarthusFallenOne", "KarthusWallOfPain", "KarthusLayWasteA1", "KarmaWMantra", "KarmaQMissileMantra", "KarmaSpiritBind", "KarmaQ", "JinxW", "JinxE", "JarvanIVGoldenAegis", "HowlingGaleSpell", "SowTheWind", "ReapTheWhirlwind", "IllaoiE", "HeimerdingerUltWDummySpell", "HeimerdingerUltEDummySpell", "HeimerdingerW", "HeimerdingerE", "HecarimUlt", "HecarimRampAttack", "GravesQLineSpell", "GravesQLineMis", "GravesClusterShot", "GravesSmokeGrenade", "GangplankR", "GalioIdolOfDurand", "GalioResoluteSmite", "FioraE", "EvelynnR", "EliseHumanE", "EkkoR", "EkkoW", "EkkoQ", "DravenDoubleShot", "InfectedCleaverMissileCast", "DariusExecute", "DariusAxeGrabCone", "DariusNoxianTacticsONH", "DariusCleave", "PhosphorusBomb", "MissileBarrage", "BraumQ", "BrandFissure", "BardR", "BardQ", "AatroxQ", "AatroxE", "AzirE", "AzirEWrapper", "AzirQWrapper", "AzirQ", "AzirR", "Pulverize", "AhriSeduce", "CurseoftheSadMummy", "InfernalGuardian", "Incinerate", "Volley", "EnchantedCrystalArrow", "BraumRWrapper", "CassiopeiaPetrifyingGaze", "FeralScream", "Rupture", "EzrealEssenceFlux", "EzrealMysticShot", "EzrealTrueshotBarrage", "FizzMarinerDoom", "GnarW", "GnarBigQMissile", "GnarQ", "GnarR", "GragasQ", "GragasE", "GragasR", "RiftWalk", "LeblancSlideM", "LeblancSlide", "LeonaSolarFlare", "UFSlash", "LuxMaliceCannon", "LuxLightStrikeKugel", "LuxLightBinding", "yasuoq3w", "VelkozE", "VeigarEventHorizon", "VeigarDarkMatter", "VarusR", "ThreshQ", "ThreshE", "ThreshRPenta", "SonaQ", "SonaR", "ShenShadowDash", "SejuaniGlacialPrisonCast", "RivenMartyr", "JavelinToss", "NautilusSplashZone", "NautilusAnchorDrag", "NamiR", "NamiQ", "DarkBindingMissile", "StaticField", "RocketGrab", "RocketGrabMissile", "timebombenemybuff", "NocturneUnspeakableHorror", "SyndraQ", "SyndraE", "SyndraR", "VayneCondemn", "Dazzle", "Overload", "AbsoluteZero", "IceBlast", "LeblancChaosOrb", "JudicatorReckoning", "KatarinaQ", "NullLance", "Crowstorm", "FiddlesticksDarkWind", "BrandWildfire", "Disintegrate", "FlashFrost", "Frostbite", "AkaliMota", "InfiniteDuress", "PantheonW", "blindingdart", "JayceToTheSkies", "IreliaEquilibriumStrike", "maokaiunstablegrowth", "nautilusgandline", "runeprison", "WildCards", "BlueCardAttack", "RedCardAttack", "GoldCardAttack", "AkaliShadowDance", "Headbutt", "PowerFist", "BrandConflagration", "CaitlynYordleTrap", "CaitlynAceintheHole", "CassiopeiaNoxiousBlast", "CassiopeiaMiasma", "CassiopeiaTwinFang", "Feast", "DianaArc", "DianaTeleport", "EliseHumanQ", "EvelynnE", "Terrify", "FizzPiercingStrike", "Parley", "GarenQAttack", "GarenR", "IreliaGatotsu", "IreliaEquilibriumStrike", "SowTheWind", "JarvanIVCataclysm", "JaxLeapStrike", "JaxEmpowerTwo", "JaxCounterStrike", "JayceThunderingBlow", "KarmaSpiritBind", "NetherBlade", "KatarinaR", "JudicatorRighteousFury", "KennenBringTheLight", "LeblancChaosOrbM", "BlindMonkRKick", "LeonaZenithBlade", "LeonaShieldOfDaybreak", "LissandraW", "LissandraQ", "LissandraR", "LuluQ", "LuluW", "LuluE", "LuluR", "SeismicShard", "AlZaharMaleficVisions", "AlZaharNetherGrasp", "MaokaiUnstableGrowth", "MordekaiserMaceOfSpades", "MordekaiserChildrenOfTheGrave", "SoulShackles", "NamiW", "NasusW", "NautilusGrandLine", "Takedown", "NocturneParanoia", "PoppyDevastatingBlow", "PoppyHeroicCharge", "QuinnE", "PuncturingTaunt", "RenektonPreExecute", "SpellFlux", "SejuaniWintersClaw", "TwoShivPoisen", "Fling", "SkarnerImpale", "SonaHymnofValor", "SwainTorment", "SwainDecrepify", "BlindingDart", "OrianaIzunaCommand", "OrianaDetonateCommand", "DetonatingShot", "BusterShot", "TrundleTrollSmash", "TrundlePain", "MockingShout", "Expunge", "UdyrBearStance", "UrgotHeatseekingLineMissile", "UrgotSwap2", "VeigarBalefulStrike", "VeigarPrimordialBurst", "ViR", "ViktorPowerTransfer", "VladimirTransfusion", "VolibearQ", "HungeringStrike", "XenZhaoComboTarget", "XenZhaoSweep", "YasuoQ3W", "YasuoQ3Mis", "YasuoQ3", "YasuoRKnockUpComboW" };
        readonly List<string> MenuSpells = new List<string>();

        Menu EOMenu;

        AIHeroClient Target;
        readonly AIHeroClient Player = EloBuddy.Player.Instance;

        Spell.Targeted Q { get { return (Spell.Targeted)Spells[0]; } }
        Spell.Active W { get { return (Spell.Active)Spells[1]; } }
        Spell.Active E { get { return (Spell.Active)Spells[2]; } }
        Spell.Active R { get { return (Spell.Active)Spells[3]; } }

        public override void Init()
        {
            InitVariables();
        }

        public override void InitEvents()
        {
            base.InitEvents();

            Obj_AI_Minion.OnPlayAnimation += Obj_AI_Minion_OnPlayAnimation;
        }

        private void Obj_AI_Minion_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            var trytododge = Features.First(it => it.NameFeature == "Misc").IsChecked("misc.dodgefireballs");

            if (!trytododge || !(sender is Obj_AI_Minion) || !sender.Name.Contains("SRU_Dragon")) return;

            if (args.Animation == "Spell1")
            {
                var delay = (int)((500 - Game.Ping) * Player.Distance(sender) / 74.6f);

                EloBuddy.SDK.Core.DelayAction(() => Q.Cast(sender), delay - Game.Ping);
                //Chat.Print(Player.Distance(sender));
            }
        }

        public override void InitVariables()
        {
            Activator = new Activator();

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

            new SkinController(9);
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
                    new ValueCheckbox(true, "combo.w.aareset", "Use W AA Reset"),
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
                    new ValueCheckbox(false, "harass.w.aareset", "Use W AA Reset"),
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
                NameFeature = "Misc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "misc.ks", "KS"),
                    new ValueCheckbox(true, "misc.dodgefireballs", "Try to dodge dragon fireballs"),
                    new ValueCheckbox(true, "misc.gapcloser", "Q on enemy gapcloser")
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

        public override void PermaActive()
        {
            Target = TargetSelector.GetTarget(800, DamageType.Physical);

            //KS

            var misc = Features.First(it => it.NameFeature == "Misc");

            if (misc.IsChecked("misc.ks") && EntityManager.Heroes.Enemies.Any(it => Q.IsInRange(it))) KS();
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

        public override void OnCombo()
        {
            if (Target == null) return;

            var mode = Features.First(it => it.NameFeature == "Combo");

            if (Q.IsReady() && mode.IsChecked("combo.q") && Q.IsReady() && Target.IsValidTarget(Q.Range))
            {
                if (mode.IsChecked("combo.q.smartq")) { QLogic(); }
                else if (mode.IsChecked("combo.q.saveqtododgespells")) { }
                else { Q.Cast(Target); }
            }

            if (R.IsReady() && mode.IsChecked("combo.r") && Player.Distance(Target) <= Player.GetAutoAttackRange(Target) + 400) { R.Cast(); }

            if (E.IsReady() && mode.IsChecked("combo.e") && Player.IsInAutoAttackRange(Target)) E.Cast();

            return;
        }

        public override void OnHarass()
        {
            if (Target == null) return;

            var mode = Features.First(it => it.NameFeature == "Harass");

            if (Q.IsReady() && mode.IsChecked("harass.q") && Target.IsValidTarget(Q.Range)) Q.Cast(Target);

            if (E.IsReady() && mode.IsChecked("harass.e") && Player.IsInAutoAttackRange(Target)) E.Cast();

            return;
        }

        public override void OnLaneClear()
        {
            var mode = Features.First(it => it.NameFeature == "Lane Clear");

            bool UseItem = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, 400).Count() >= 3;
            if (UseItem) { Activator.tiamat.Cast(); Activator.hydra.Cast(); }

            if (Player.ManaPercent < mode.SliderValue("laneclear.mana%") || !Q.IsReady()) return;

            if (mode.IsChecked("laneclear.q"))
            {
                IEnumerable<Obj_AI_Minion> ListMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition, 1000).OrderBy(minion => minion.Distance(Player));
                int hits = new int();

                if (ListMinions.Any() && ListMinions.Count() >= mode.SliderValue("laneclear.q.minminions"))
                {
                    if (!(ListMinions.First().Distance(Player) > Q.Range))
                    {
                        hits += 1;

                        for (int i = 0; i < ListMinions.Count(); i++)
                        {
                            if (i + 1 == ListMinions.Count()) break;
                            else if (ListMinions.ElementAt(i).Distance(ListMinions.ElementAt(i + 1)) <= 400) { hits += 1; }
                            else break;
                        }

                        if (hits >= mode.SliderValue("laneclear.q.minminions"))
                        {
                            if (mode.IsChecked("laneclear.q.jimwd"))
                            {
                                if (GetQDamage(ListMinions.First()) >= ListMinions.First().Health || GetQDamage(ListMinions.ElementAt(1)) >= ListMinions.ElementAt(1).Health) Q.Cast(ListMinions.First());
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
            var mode = Features.First(it => it.NameFeature == "Jungle Clear");

            if (E.IsReady() && EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Player.GetAutoAttackRange()).Any() && mode.IsChecked("jungleclear.e")) E.Cast();

            bool UseItem = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, 400).Count() >= 1;
            if (UseItem) { Activator.tiamat.Cast(); Activator.hydra.Cast(); }

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
                var AARCombo = Features.First(it => it.NameFeature == "Combo").IsChecked("combo.w.aareset");

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && AARCombo)
                {
                    if (W.Cast())
                    {
                        Orbwalker.ResetAutoAttack();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                    }

                    return;
                }

                var AARHarass = Features.First(it => it.NameFeature == "Harass").IsChecked("harass.w.aareset");

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

            if (sender.IsEnemy && MenuSpells.Any(el => el == args.SData.Name) && Player.Distance(sender) <= args.SData.CastRange)
            {
                if (Q.IsReady() && (EOMenu[args.SData.Name].Cast<Slider>().CurrentValue == 1 || EOMenu[args.SData.Name].Cast<Slider>().CurrentValue == 3))
                {
                    if (args.SData.Name == "JaxCounterStrike") { EloBuddy.SDK.Core.DelayAction(() => Dodge(), 2000 - Game.Ping - 100); return; }

                    if (args.SData.Name == "KarthusFallenOne") { EloBuddy.SDK.Core.DelayAction(() => Dodge(), 3000 - Game.Ping - 100); return; }

                    if (args.SData.Name == "ZedR" && args.Target.IsMe) { EloBuddy.SDK.Core.DelayAction(() => Dodge(), 750 - Game.Ping - 100); return; }

                    if (args.SData.Name == "SoulShackles") { EloBuddy.SDK.Core.DelayAction(() => Dodge(), 3000 - Game.Ping - 100); return; }

                    if (args.SData.Name == "AbsoluteZero") { EloBuddy.SDK.Core.DelayAction(() => Dodge(), 3000 - Game.Ping - 100); return; }

                    if (args.SData.Name == "NocturneUnspeakableHorror" && args.Target.IsMe) { EloBuddy.SDK.Core.DelayAction(() => Dodge(), 2000 - Game.Ping - 100); return; }

                    EloBuddy.SDK.Core.DelayAction(delegate
                    {
                        if (Target != null && Target.IsValidTarget(Q.Range)) Q.Cast(Target);
                    }, (int)args.SData.SpellCastTime - Game.Ping - 100);

                    EloBuddy.SDK.Core.DelayAction(delegate
                    {
                        if (sender.IsValidTarget(Q.Range)) Q.Cast(sender);
                    }, (int)args.SData.SpellCastTime - Game.Ping - 50);

                    return;
                }

                else if (W.IsReady() && Player.IsFacing(sender) && EOMenu[args.SData.Name].Cast<Slider>().CurrentValue > 1 && (args.Target != null && args.Target.IsMe || new Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.LineWidth).IsInside(Player) || new Geometry.Polygon.Circle(args.End, args.SData.CastRadius).IsInside(Player)))
                {
                    int delay = (int)(Player.Distance(sender) / ((args.SData.MissileMaxSpeed + args.SData.MissileMinSpeed) / 2) * 1000) - 150 + (int)args.SData.SpellCastTime;

                    if (args.SData.Name != "ZedR" && args.SData.Name != "NocturneUnpeakableHorror")
                    {
                        EloBuddy.SDK.Core.DelayAction(() => W.Cast(), delay);
                        if (Target != null && Target.IsValidTarget()) EloBuddy.SDK.Core.DelayAction(() => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, Target), delay + 100);
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

        //-------------------------------------SpellDamage()---------------------------------------

        float GetQDamage(Obj_AI_Base unit)
        {
            if (!unit.IsValidTarget()) return 0;

            if (unit.IsMinion)
                return Player.CalculateDamageOnUnit(unit, DamageType.Physical, new[] { 0, 100, 160, 220, 280, 340 }[Q.Level] + Player.TotalAttackDamage, true, true);

            return Player.CalculateDamageOnUnit(unit, DamageType.Physical, new[] { 0, 25, 60, 95, 130, 165 }[Q.Level] + Player.TotalAttackDamage, true, true);

        }

        //----------------------------------------QLogic()-----------------------------------------

        void QLogic()
        {
            if (Target.IsDashing()) Q.Cast(Target);
            if (Target.HealthPercent <= 30) Q.Cast(Target);
            if (Player.HealthPercent <= 30) Q.Cast(Target);
            if (GetQDamage(Target) >= Target.Health) Q.Cast(Target);
        }

        //----------------------------------------Dodge()------------------------------------------

        void Dodge()
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

        //-------------------------------------------KS--------------------------------------------

        void KS()
        {
            if (Q.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && GetQDamage(enemy) >= enemy.Health);
                if (bye != null) { Q.Cast(bye); return; }
            }
        }
    }
}