using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Enumerations;
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
    class Morgana : PluginModel
    {
        readonly AIHeroClient Player = EloBuddy.Player.Instance;
        Menu EMenu;

        List<string> MenuSpells = new List<string>();
        List<string> CollisionSpells = new List<string>() { "TahmKenchQ", "JinxW", "IllaoiE", "HeimerdingerUltWDummySpell", "HeimerdingerW", "EliseHumanE", "InfectedMissileCleaverCast", "MissileBarrage", "BraumQ", "BardQ", "AhriSeduce", "EnchantedCrystalArrow", "EzrealEssenceFlux", "FizzMarinerDoom", "GnarBigQMissile", "GragasE", "LuxLightBinding", "VarusR", "ThreshQ", "SejuaniGlacialPrisonCast", "JavelinToss", "NautilusAnchorDrag", "DarkBindingMissile", "RocketGrab", "RocketGrabMissile", "LissandraQ" };
        List<string> ESpells = new List<string>() { "SorakaQ", "SorakaE", "TahmKenchW", "TahmKenchQ", "Bushwhack", "ForcePulse", "KarthusFallenOne", "KarthusWallOfPain", "KarthusLayWasteA1", "KarmaWMantra", "KarmaQMissileMantra", "KarmaSpiritBind", "KarmaQ", "JinxW", "JinxE", "JarvanIVGoldenAegis", "HowlingGaleSpell", "SowTheWind", "ReapTheWhirlwind", "IllaoiE", "HeimerdingerUltWDummySpell", "HeimerdingerUltEDummySpell", "HeimerdingerW", "HeimerdingerE", "HecarimUlt", "HecarimRampAttack", "GravesQLineSpell", "GravesQLineMis", "GravesClusterShot", "GravesSmokeGrenade", "GangplankR", "GalioIdolOfDurand", "GalioResoluteSmite", "FioraE", "EvelynnR", "EliseHumanE", "EkkoR", "EkkoW", "EkkoQ", "DravenDoubleShot", "InfectedCleaverMissileCast", "DariusExecute", "DariusAxeGrabCone", "DariusNoxianTacticsONH", "DariusCleave", "PhosphorusBomb", "MissileBarrage", "BraumQ", "BrandFissure", "BardR", "BardQ", "AatroxQ", "AatroxE", "AzirE", "AzirEWrapper", "AzirQWrapper", "AzirQ", "AzirR", "Pulverize", "AhriSeduce", "CurseoftheSadMummy", "InfernalGuardian", "Incinerate", "Volley", "EnchantedCrystalArrow", "BraumRWrapper", "CassiopeiaPetrifyingGaze", "FeralScream", "Rupture", "EzrealEssenceFlux", "EzrealMysticShot", "EzrealTrueshotBarrage", "FizzMarinerDoom", "GnarW", "GnarBigQMissile", "GnarQ", "GnarR", "GragasQ", "GragasE", "GragasR", "RiftWalk", "LeblancSlideM", "LeblancSlide", "LeonaSolarFlare", "UFSlash", "LuxMaliceCannon", "LuxLightStrikeKugel", "LuxLightBinding", "yasuoq3w", "VelkozE", "VeigarEventHorizon", "VeigarDarkMatter", "VarusR", "ThreshQ", "ThreshE", "ThreshRPenta", "SonaQ", "SonaR", "ShenShadowDash", "SejuaniGlacialPrisonCast", "RivenMartyr", "JavelinToss", "NautilusSplashZone", "NautilusAnchorDrag", "NamiR", "NamiQ", "DarkBindingMissile", "StaticField", "RocketGrab", "RocketGrabMissile", "timebombenemybuff", "karthusfallenonetarget", "NocturneUnspeakableHorror", "SyndraQ", "SyndraE", "SyndraR", "VayneCondemn", "Dazzle", "Overload", "AbsoluteZero", "IceBlast", "LeblancChaosOrb", "JudicatorReckoning", "KatarinaQ", "NullLance", "Crowstorm", "FiddlesticksDarkWind", "BrandWildfire", "Disintegrate", "FlashFrost", "Frostbite", "AkaliMota", "InfiniteDuress", "PantheonW", "blindingdart", "JayceToTheSkies", "IreliaEquilibriumStrike", "maokaiunstablegrowth", "nautilusgandline", "runeprison", "WildCards", "BlueCardAttack", "RedCardAttack", "GoldCardAttack", "AkaliShadowDance", "Headbutt", "PowerFist", "BrandConflagration", "CaitlynYordleTrap", "CaitlynAceintheHole", "CassiopeiaNoxiousBlast", "CassiopeiaMiasma", "CassiopeiaTwinFang", "Feast", "DianaArc", "DianaTeleport", "EliseHumanQ", "EvelynnE", "Terrify", "FizzPiercingStrike", "Parley", "GarenQAttack", "GarenR", "IreliaGatotsu", "IreliaEquilibriumStrike", "SowTheWind", "JarvanIVCataclysm", "JaxLeapStrike", "JaxEmpowerTwo", "JaxCounterStrike", "JayceThunderingBlow", "KarmaSpiritBind", "NetherBlade", "KatarinaR", "JudicatorRighteousFury", "KennenBringTheLight", "LeblancChaosOrbM", "BlindMonkRKick", "LeonaZenithBlade", "LeonaShieldOfDaybreak", "LissandraW", "LissandraQ", "LissandraR", "LuluQ", "LuluW", "LuluE", "LuluR", "SeismicShard", "AlZaharMaleficVisions", "AlZaharNetherGrasp", "MaokaiUnstableGrowth", "MordekaiserMaceOfSpades", "MordekaiserChildrenOfTheGrave", "SoulShackles", "NamiW", "NasusW", "NautilusGrandLine", "Takedown", "NocturneParanoia", "PoppyDevastatingBlow", "PoppyHeroicCharge", "QuinnE", "PuncturingTaunt", "RenektonPreExecute", "SpellFlux", "SejuaniWintersClaw", "TwoShivPoisen", "Fling", "SkarnerImpale", "SonaHymnofValor", "SwainTorment", "SwainDecrepify", "BlindingDart", "OrianaIzunaCommand", "OrianaDetonateCommand", "DetonatingShot", "BusterShot", "TrundleTrollSmash", "TrundlePain", "MockingShout", "Expunge", "UdyrBearStance", "UrgotHeatseekingLineMissile", "UrgotSwap2", "VeigarBalefulStrike", "VeigarPrimordialBurst", "ViR", "ViktorPowerTransfer", "VladimirTransfusion", "VolibearQ", "HungeringStrike", "XenZhaoComboTarget", "XenZhaoSweep", "YasuoQ3W", "YasuoQ3Mis", "YasuoQ3", "YasuoRKnockUpComboW" };

        Spell.Skillshot Q { get { return (Spell.Skillshot)Spells[0]; } }
        Spell.Skillshot W { get { return (Spell.Skillshot)Spells[1]; } }
        Spell.Targeted E { get { return (Spell.Targeted)Spells[2]; } }
        Spell.Active R { get { return (Spell.Active)Spells[3]; } }

        public override void Init()
        {
            InitVariables();
        }

        public override void InitEvents()
        {
            base.InitEvents();

            Dash.OnDash += Dash_OnDash;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
        }

        private void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Player.CountAlliesInRange(2000) > 1 && !Features.First(it => it.NameFeature == "Misc").IsChecked("misc.aaminionswhenallynear"))
                args.Process = false;
        }

        private void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            if (!Q.IsReady() || !(sender.BaseSkinName == "Yasuo" && Player.Distance(e.EndPos) <= 200) || sender.IsMe || sender.IsAlly || Player.Distance(e.EndPos) > Q.Range || !Features.First(it => it.NameFeature == "Misc").IsChecked("misc.autoqondash")) return;

            var rectangle = new Geometry.Polygon.Rectangle(Player.Position, e.EndPos, Q.Width);

            if (!EntityManager.MinionsAndMonsters.EnemyMinions.Any(it => !it.IsDead && it.IsValidTarget() && rectangle.IsInside(it)) || !(EntityManager.Heroes.Enemies.Count(enemy => !enemy.IsDead && enemy.IsValidTarget() && rectangle.IsInside(enemy)) > 0))
            {
                Q.Cast(e.EndPos);
            }
        }

        public override void InitVariables()
        {
            Activator = new Activator(DamageType.Magical);

            Spells = new List<Spell.SpellBase>
            {
                new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 1200, 70),
                new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 250, 2200, 280),
                new Spell.Targeted(SpellSlot.E, 750),
                new Spell.Active(SpellSlot.R, 625)
            };

            Q.MinimumHitChance = HitChance.Medium;
            W.AllowedCollisionCount = int.MaxValue;

            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Q, new float[] { 0, 80, 135, 190, 245, 300 }, new [] { 0, 0.9f, 0.9f, 0.9f, 0.9f, 0.9f }, DamageType.Magical),
                new SpellDamage(W, new float[] { 0, 80, 160, 240, 320, 400 }, new [] { 0, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f }, DamageType.Magical),
                //--------------------------------------- not 0.8f, the true value is 1.1f, 5 seconds in a tormented soil ?
                new SpellDamage(R, new float[] { 0, 150, 225, 300 }, new [] { 0, 0.7f, 0.7f, 0.7f, 0.7f, 0.7f }, DamageType.Magical)
            };

            InitMenu();

            DamageIndicator.Initialize(DamageUtil.GetComboDamage);

            new SkinController(8);
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
                    new ValueCheckbox(true, "draw.r", "Draw R"),
                    new ValueCheckbox(true, "draw.q", "Draw Q"),
                    new ValueCheckbox(true, "draw.e", "Draw E"),
                    new ValueCheckbox(false, "draw.w", "Draw W"),
                    new ValueCheckbox(true, "draw.rposandhits", "Draw R position and hits"),
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
                    new ValueSlider(100, 1, 85, "combo.q.hitchance%", "Q HitChance%"),
                    new ValueCheckbox(true, "combo.w", "Combo W"),
                    new ValueCheckbox(true,  "combo.r", "Combo R", true),
                    new ValueSlider(5, 1, 2, "combo.r.minenemies", "Min enemies R"),
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
                    new ValueSlider(100, 1, 85, "harass.q.hitchance%", "Q HitChance%"),
                    new ValueCheckbox(true, "harass.w", "Harass W", true),
                    new ValueCheckbox(true, "harass.w.jitii", "Just W if target is immobile"),
                    new ValueSlider(99, 1, 30, "harass.mana%", "Harass MinMana%", true)
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Lane Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "laneclear.w", "Lane Clear W"),
                    new ValueSlider(7, 1, 3, "laneclear.w.minminions", "Min minions W"),
                    new ValueSlider(100, 1, 30, "laneclear.mana%", "Lane Clear MinMana%", true)
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
                    new ValueCheckbox(true,  "jungleclear.w", "Jungle Clear W"),
                    new ValueSlider(100, 1, 30, "jungleclear.mana%", "Jungle Clear MinMana%", true)
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Misc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(false,  "misc.ks", "KS"),
                    new ValueCheckbox(false,  "misc.aaminionswhenallynear", "AA minions when ally near?"),
                    new ValueCheckbox(true, "misc.autoqonimmobile", "Auto Q on immobile enemies", true),
                    new ValueCheckbox(true, "misc.autoqonflash", "Auto Q on flash", true),
                    new ValueCheckbox(true, "misc.autoqondash", "Auto Q on dash", true),
                    new ValueCheckbox(true, "misc.autowonimmobile", "Auto W on immobile enemies", true),
                    new ValueCheckbox(true, "misc.gapcloser", "Q on enemy gapcloser", true)
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            //---------------------------------------||   EMenu   ||------------------------------------------

            EMenu = Globals.MENU.AddSubMenu("E Options", "E Options");
            EMenu.AddSeparator();

            foreach (var ally in EntityManager.Heroes.Allies)
            {
                EMenu.Add(ally.BaseSkinName, new Slider(string.Format("{0}'s Priority", ally.BaseSkinName), 3, 1, 5));
                EMenu.AddSeparator();
            }

            EMenu.AddSeparator();

            EMenu.Add("UseShield?", new CheckBox("Use Shield?"));

            EMenu.AddSeparator();

            EMenu.AddGroupLabel("Use shield for:");

            EMenu.AddSeparator();

            foreach (AIHeroClient hero in EntityManager.Heroes.Enemies)
            {
                EMenu.AddGroupLabel(hero.BaseSkinName);
                {
                    foreach (SpellDataInst spell in hero.Spellbook.Spells)
                    {
                        if (ESpells.Any(el => el == spell.SData.Name))
                        {
                            EMenu.Add(spell.Name, new CheckBox(hero.BaseSkinName + " : " + spell.Slot.ToString() + " : " + spell.Name));
                            MenuSpells.Add(spell.Name);
                        }
                    }
                }

                EMenu.AddSeparator();
            }
        }

        public override void PermaActive()
        {
            var misc = Features.First(it => it.NameFeature == "Misc");

            //----Auto Q/W on immobile enemies

            var immobile = EntityManager.Heroes.Enemies.FirstOrDefault(it => !it.IsDead && it.IsValidTarget(Q.Range) && !it.CanMove());

            if (immobile != null)
            {
                if (misc.IsChecked("misc.autoqonimmobile") && Q.IsReady()) SpellsUtil.HitChanceCast(Q, immobile);
                if (misc.IsChecked("misc.autowonimmobile") && W.IsReady() && W.IsInRange(immobile)) W.Cast(immobile.ServerPosition); 
            }

            //----KS
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

            if (draw.IsChecked("draw.r"))
                Circle.Draw(R.IsReady() ? Color.Blue : Color.Red, R.Range, Player.Position);

            if (draw.IsChecked("draw.q"))
                Circle.Draw(Q.IsReady() ? Color.Blue : Color.Red, Q.Range, Player.Position);

            if (draw.IsChecked("draw.e"))
                Circle.Draw(E.IsReady() ? Color.Blue : Color.Red, E.Range, Player.Position);

            if (draw.IsChecked("draw.w"))
                Circle.Draw(W.IsReady() ? Color.Blue : Color.Red, W.Range, Player.Position);

            DamageIndicator.Enabled = draw.IsChecked("dmgIndicator");

        }

        public override void OnCombo()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Target == null || !Target.IsValidTarget()) return;

            var mode = Features.First(it => it.NameFeature == "Combo");

            if (R.IsReady() && Player.CountEnemiesInRange(R.Range) >= mode.SliderValue("combo.r.minenemies"))
                R.Cast();

            if (Q.IsReady() && Target.IsValidTarget(Q.Range) && mode.IsChecked("combo.q"))
                SpellsUtil.HitChanceCast(Q, Target, mode.SliderValue("combo.q.hitchance%"));

            if (W.IsReady() && !Target.CanMove() && mode.IsChecked("combo.w")) W.Cast(Target.ServerPosition);

            return;
        }

        public override void OnHarass()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Target == null || !Target.IsValidTarget()) return;

            var mode = Features.First(it => it.NameFeature == "Harass");

            if (Player.ManaPercent < mode.SliderValue("harass.mana%")) return;

            if (Q.IsReady() && Target.IsValidTarget(Q.Range) && mode.IsChecked("harass.q"))
                SpellsUtil.HitChanceCast(Q, Target, mode.SliderValue("harass.q.hitchance%"));

            if (W.IsReady() && mode.IsChecked("harass.w"))
            {
                if (mode.IsChecked("harass.w.jitii"))
                {
                    if (!Target.CanMove()) W.Cast(Target.ServerPosition);
                }
                else
                {
                    SpellsUtil.HitChanceCast(W, Target);
                }
            }

            return;
        }

        public override void OnLaneClear()
        {
            var mode = Features.First(it => it.NameFeature == "Lane Clear");

            bool UseItem = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, 400).Count() >= 3;
            if (UseItem) { Activator.tiamat.Cast(); Activator.hydra.Cast(); }

            if (Player.ManaPercent <= mode.SliderValue("laneclear.mana%")) return;

            var Minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, W.Range);

            if (Minions != null)
            {
                var FL = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(Minions, 280, (int)W.Range);

                if (FL.HitNumber >= mode.SliderValue("laneclear.w.minminions")) W.Cast(FL.CastPosition);
            }

            return;
        }

        public override void OnFlee()
        {
            base.OnFlee();

            if (!Q.IsReady()) return;

            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (Target != null && Target.IsValidTarget(Q.Range)) SpellsUtil.HitChanceCast(Q, Target);
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            base.OnAfterAttack(target, args);

            var jungleclear = Features.First(it => it.NameFeature == "Jungle Clear");

            if (Player.ManaPercent < jungleclear.SliderValue("jungleclear.mana%")) return;

            var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, W.Range).FirstOrDefault(it => it.Health > 2 * Player.GetAutoAttackDamage(it));

            if (minion == null) return;

            if (Q.IsReady() && jungleclear.IsChecked("jungleclear.q"))
                SpellsUtil.HitChanceCast(Q, minion, 1);

            if (W.IsReady() && jungleclear.IsChecked("jungleclear.w"))
                SpellsUtil.HitChanceCast(W, minion);
        }

        public override void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            base.OnProcessSpell(sender, args);

            if (!sender.IsEnemy) return;

            if (Q.IsReady() && args.SData.Name.ToLower() == "summonerflash" && args.End.Distance(Player) <= Q.Range && Features.First(it => it.NameFeature == "Misc").IsChecked("misc.autoqonflash"))
            {
                //Chat.Print("{0} detected, Q on args.End", args.SData.Name);
                var rectangle = new Geometry.Polygon.Rectangle(Player.Position, args.End, Q.Width + 10);

                if (!EntityManager.MinionsAndMonsters.EnemyMinions.Any(it => rectangle.IsInside(it))) Q.Cast(args.End);
                return;
            }

            if (E.IsReady() && EMenu["UseShield?"].Cast<CheckBox>().CurrentValue && MenuSpells.Any(it => it == args.SData.Name))
            {
                if (EMenu[args.SData.Name].Cast<CheckBox>().CurrentValue)
                {
                    List<AIHeroClient> Allies = new List<AIHeroClient>();

                    //Division
                    if (args.Target != null)
                    {
                        if (args.Target.IsAlly || args.Target.IsMe)
                        {
                            var target = EntityManager.Heroes.Allies.FirstOrDefault(it => it.NetworkId == args.Target.NetworkId);

                            //Chat.Print(args.Target.Name);

                            if (target != null) E.Cast(target);

                            return;
                        }
                    }

                    //Division

                    var rectangle = new Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.LineWidth);

                    foreach (var ally in EntityManager.Heroes.Allies)
                    {
                        if (rectangle.IsInside(ally)) { Allies.Add(ally); continue; }

                        foreach (var point in rectangle.Points)
                        {
                            if (ally.Distance(point) <= 90)
                            {
                                Allies.Add(ally);
                            }
                        }
                    }

                    if (Allies.Any())
                    {
                        //Chat.Print("Rectangle Detection");

                        PriorityCast(sender, args, Allies, rectangle);
                        return;
                    }

                    //Division

                    var circle = new Geometry.Polygon.Circle(args.End, args.SData.CastRadius);

                    foreach (var ally in EntityManager.Heroes.Allies)
                    {
                        if (circle.IsInside(ally)) { Allies.Add(ally); continue; }

                        foreach (var point in circle.Points)
                        {
                            if (ally.Distance(point) <= 90)
                            {
                                Allies.Add(ally);
                            }
                        }
                    }

                    if (Allies.Any())
                    {
                        //Chat.Print("Circle Detection");

                        PriorityCast(sender, args, Allies, circle);
                        return;
                    }
                }
            }

            return;
        }

        public override void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            base.OnGapCloser(sender, e);

            if (!Q.IsReady() || sender.IsMe || sender.IsAlly || !sender.IsValidTarget(Q.Range)) return;

            var gapclose = Features.First(it => it.NameFeature == "Misc").IsChecked("misc.gapcloser");

            if (gapclose) SpellsUtil.HitChanceCast(Q, sender);

            return;
        }

        //------------------------------------|| Extension ||--------------------------------------

        //-------------------------------------PriorityCast-----------------------------------

        void PriorityCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args, List<AIHeroClient> Allies, Geometry.Polygon polygon)
        {
            int delay = new int();

            Allies.OrderBy(it => it.Distance(args.Start));

            var ally = Allies.First();

            if (Allies.Count == 1)
            {
                delay = (int)((uint)(((sender.Distance(ally)-300) / args.SData.MissileMaxSpeed * 1000) + args.SData.SpellCastTime - 300 - Game.Ping));

                EloBuddy.SDK.Core.DelayAction(delegate
                {
                    if (polygon.IsInside(ally) && ally.IsValidTarget(E.Range)) E.Cast(ally);
                    return;
                }, delay);

                //Chat.Print("Shield for {0} : {1}", sender.BaseSkinName, args.Slot.ToString());
                return;
            }
            else
            {
                if (CollisionSpells.Any(it => it == args.SData.Name))
                {
                    delay = (int)((uint)(((sender.Distance(ally)-300) / args.SData.MissileMaxSpeed * 1000) + args.SData.SpellCastTime - Game.Ping));

                    EloBuddy.SDK.Core.DelayAction(delegate
                    {
                        foreach (var Ally in Allies)
                        {
                            if (polygon.IsInside(Ally) && E.IsInRange(Ally)) { E.Cast(Ally); return; }
                        }
                        return;
                    }, delay);

                    //Chat.Print("Shield for {0} : {1}", sender.BaseSkinName, args.Slot.ToString());

                    return;
                }

                else
                {
                    IEnumerable<AIHeroClient> priorities = from aliado in EntityManager.Heroes.Allies orderby EMenu[aliado.BaseSkinName].Cast<Slider>().CurrentValue descending select aliado;

                    delay = (int)((uint)(((sender.Distance(ally)-300) / args.SData.MissileMaxSpeed * 1000) + args.SData.SpellCastTime - 200 - Game.Ping));

                    EloBuddy.SDK.Core.DelayAction(delegate
                    {
                        foreach (var Ally in priorities)
                        {
                            if (polygon.IsInside(Ally) && E.IsInRange(Ally)) { E.Cast(Ally); return; }
                        }
                        return;
                    }, delay);

                    //Chat.Print("Shield for {0} : {1}", sender.BaseSkinName, args.Slot.ToString());
                    return;
                }
            }
        }

        //-----------------------------------------KS-----------------------------------------

        void KS()
        {
            if (Q.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && DamageUtil.GetSpellDamage(enemy, SpellSlot.Q) >= enemy.Health);
                if (bye != null) { SpellsUtil.HitChanceCast(Q, bye); return; }
            }

            if (R.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(R.Range) && DamageUtil.GetSpellDamage(enemy, SpellSlot.R) >= enemy.Health);
                if (bye != null) { R.Cast(); return; }
            }

            if (Q.IsReady() && R.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(R.Range-50) && DamageUtil.GetSpellDamage(enemy, SpellSlot.R) + DamageUtil.GetSpellDamage(enemy, SpellSlot.Q) >= enemy.Health);
                if (bye != null) { SpellsUtil.HitChanceCast(Q, bye); return; }
            }
        }
    }
}