using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
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
    class Annie : PluginModel
    {
        readonly AIHeroClient Player = EloBuddy.Player.Instance;
        Obj_AI_Minion Tibbers;
        const float ANGLE = 5 * (float)Math.PI / 18;

        Spell.Targeted Q { get { return (Spell.Targeted)Spells[0]; } }
        Spell.Skillshot W { get { return (Spell.Skillshot)Spells[1]; } }
        Spell.Active E { get { return (Spell.Active)Spells[2]; } }
        Spell.Skillshot R { get { return (Spell.Skillshot)Spells[3]; } }

        public override void Init()
        {
            InitVariables();
        }

        public override void InitEvents()
        {
            base.InitEvents();

            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
        }

        public override void InitVariables()
        {
            Activator = new Activator(DamageType.Magical);

            Spells = new List<Spell.SpellBase>
            {
                new Spell.Targeted(SpellSlot.Q, 625),
                new Spell.Skillshot(SpellSlot.W, 625, SkillShotType.Cone, 250, int.MaxValue, 210),
                new Spell.Active(SpellSlot.E),
                new Spell.Skillshot(SpellSlot.R, 600, SkillShotType.Circular, 50, int.MaxValue, 250)
            };

            W.MinimumHitChance = HitChance.Medium;
            W.AllowedCollisionCount = int.MaxValue;
            W.ConeAngleDegrees = 50;

            R.AllowedCollisionCount = int.MaxValue;
            R.MinimumHitChance = HitChance.Medium;

            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Q, new float[] { 0, 80, 115, 150, 185, 220 }, new [] { 0, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f }, DamageType.Magical),
                new SpellDamage(W, new float[] { 0, 70, 115, 160, 205, 250 }, new [] { 0, 0.85f, 0.85f, 0.85f, 0.85f, 0.85f }, DamageType.Magical),
                new SpellDamage(R, new float[] { 0, 175, 300, 425 }, new [] { 0, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f }, DamageType.Magical)
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
                    new ValueCheckbox(true, "draw.q/w/r", "Draw Q/W/R"),
                    new ValueCheckbox(true, "draw.flash+r", "Draw Flash + R"),
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
                    new ValueCheckbox(true, "combo.w", "Combo W"),
                    new ValueCheckbox(true,  "combo.r", "Combo R", true),
                    new ValueCheckbox(false,  "combo.r.jiws", "Just R with stun"),
                    new ValueSlider(5, 1, 2, "combo.r.minenemies", "Min enemies R"),
                    new ValueCheckbox(true,  "combo.aa", "AA ?", true),
                    new ValueCheckbox(false,  "combo.aa.maxrange", "AA in max range ?")
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
                    new ValueCheckbox(true, "harass.w", "Harass W")
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
                    new ValueSlider(2, 0, 2, "laneclear.q.mode", "Q Mode, 0 : Always, 1 : AlwaysIfNoEnemiesAround, 2 : AlwaysIfNoStun"),
                    new ValueCheckbox(true,  "laneclear.w", "Lane Clear W", true),
                    new ValueSlider(7, 1, 4, "laneclear.w.minminions", "Min minions W"),
                    new ValueSlider(100, 1, 30, "laneclear.mana%", "Lane Clear MinMana%", true)
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Last Hit",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "lasthit.q", "Last Hit Q"),
                    new ValueSlider(2, 0, 2, "lasthit.q.mode", "Q Mode, 0 : Always, 1 : AlwaysIfNoEnemiesAround, 2 : AlwaysIfNoStun"),
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Jungle Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "jungleclear.q", "Lane Clear Q"),
                    new ValueCheckbox(true,  "jungleclear.w", "Lane Clear W"),
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
                    new ValueCheckbox(true,  "misc.ks", "KS"),
                    new ValueCheckbox(true,  "misc.stackstun", "Auto stack stun"),
                    new ValueKeybind(false, "misc.autor", "Auto R on target (Ignore the min enemies slider):", KeyBind.BindTypes.HoldActive, 'J', true),
                    new ValueKeybind(false, "misc.autoflash+r", "Auto Flash + R (Doesn't ignore the min enemies slider):", KeyBind.BindTypes.HoldActive, 'J', true),
                    new ValueCheckbox(true, "misc.gapcloser", "Stun on enemy gapcloser", true),
                    new ValueCheckbox(true, "misc.interrupter", "Stun to interrupt spells", true)
                }
            };

            feature.ToMenu();
            Features.Add(feature);
        }

        public override void PermaActive()
        {
            var misc = Features.First(it => it.NameFeature == "Misc");

            Tibbers = EntityManager.MinionsAndMonsters.Minions.FirstOrDefault(it => !it.IsDead && it.IsValidTarget() && it.Name.ToLower().Contains("tibbers") || it.Name.ToLower().Contains("infernal") || it.Name.ToLower().Contains("guardian"));

            //----Auto R / Flash+R

            if (R.IsReady() && (misc.IsChecked("misc.autor") || misc.IsChecked("misc.autoflash+r")))
            {
                var Target = TargetSelector.GetTarget(Activator.flash.Range + R.Range, DamageType.Magical);

                if (misc.IsChecked("misc.autor") && Target.IsValidTarget(R.Range - 50)) R.Cast(Target);

                else if (misc.IsChecked("misc.autoflash+r") && Target.IsValidTarget(R.Range + Activator.flash.Range - 100) && Activator.flash.IsReady())
                {
                    var RPos = GetBestRPos(Target.ServerPosition.To2D());

                    if (RPos.First().Value > 0)
                    {
                        var FlashPos = Player.Position.Extend(RPos.First().Key, Activator.flash.Range).To3D();

                        Activator.flash.Cast(FlashPos);

                        EloBuddy.SDK.Core.DelayAction(() => R.Cast(RPos.First().Key.To3D()), 200);
                    }
                }
            }

            //----KS
            if (misc.IsChecked("misc.ks") && EntityManager.Heroes.Enemies.Any(it => Q.IsInRange(it))) KS();

            //----Stack stun

            if (misc.IsChecked("misc.stackstun") && !Player.HasBuff("recall") && !Player.HasBuff("pyromania_particle"))
            {
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && E.IsReady()) E.Cast();
                if (Player.IsInShopRange() && W.IsReady()) W.Cast(Game.CursorPos);
            }
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

            if (R.IsReady() && draw.IsChecked("draw.rposandhits"))
            {
                var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (Target != null && Target.IsValidTarget())
                {
                    var PosAndHits = GetBestRPos(Target.ServerPosition.To2D());
                    var minenemiesr = Features.First(it => it.NameFeature == "Combo").SliderValue("combo.r.minenemies");

                    if (PosAndHits.First().Value >= minenemiesr)
                    {
                        Circle.Draw(Color.Yellow, 70, PosAndHits.First().Key.To3D());
                        Drawing.DrawText(Drawing.WorldToScreen(Player.Position).X, Drawing.WorldToScreen(Player.Position).Y - 200, System.Drawing.Color.Yellow, string.Format("R WILL HIT {0} ENEMIES", PosAndHits.First().Value));
                    }
                }
            }

            if (draw.IsChecked("draw.q/w/r"))
                Circle.Draw((Q.IsReady() || W.IsReady() || R.IsReady()) ? Color.Blue : Color.Red, R.Range, Player.Position);

            if (draw.IsChecked("draw.flash+r"))
                Circle.Draw(Activator.flash.IsReady() && R.IsReady() ? Color.Blue : Color.Red, Activator.flash.Range + R.Range, Player.Position);

            DamageIndicator.Enabled = draw.IsChecked("dmgIndicator");

        }

        public override void OnCombo()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Target == null || !Target.IsValidTarget()) return;

            var mode = Features.First(it => it.NameFeature == "Combo");

            if (mode.IsChecked("combo.r.jiws"))
            {
                if (R.IsReady() && mode.IsChecked("combo.r") && Target.IsValidTarget(R.Range))
                {
                    var rpos = GetBestRPos(Target.ServerPosition.To2D());

                    if (rpos.Values.First() >= mode.SliderValue("combo.r.minenemies"))
                    {
                        var pos = rpos.Keys.First().To3D();
                        R.Cast(pos);
                    }
                }
                else
                {
                    if (W.IsReady() && mode.IsChecked("combo.w") && Target.IsValidTarget(W.Range))
                    {
                        var wpos = GetBestWPos(false, Target);
                        if (wpos != default(Vector3)) W.Cast(wpos);
                    }

                    if (Q.IsReady() && mode.IsChecked("combo.q") && Target.IsValidTarget(Q.Range)) Q.Cast(Target);
                }
            }
            else
            {
                if (R.IsReady() && mode.IsChecked("combo.r") && Target.IsValidTarget(R.Range))
                {
                    var rpos = GetBestRPos(Target.ServerPosition.To2D());

                    if (rpos.Values.First() >= mode.SliderValue("combo.r.minenemies"))
                    {
                        var pos = rpos.Keys.First().To3D();
                        R.Cast(pos);
                    }
                }

                if (W.IsReady() && mode.IsChecked("combo.w") && Target.IsValidTarget(W.Range))
                {
                    var wpos = GetBestWPos(false, Target);
                    if (wpos != default(Vector3)) W.Cast(wpos);
                }

                if (Q.IsReady() && mode.IsChecked("combo.q") && Target.IsValidTarget(Q.Range)) Q.Cast(Target);
            }

            if (Player.HasBuff("infernalguardiantime"))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, Target);

                if (Tibbers != null && Tibbers.IsValid && Tibbers.IsInAutoAttackRange(Target))
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttackPet, Target);
            }

            return;
        }

        public override void OnHarass()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Target == null || !Target.IsValidTarget()) return;

            var mode = Features.First(it => it.NameFeature == "Harass");

            if (W.IsReady() && mode.IsChecked("harass.w") && Target.IsValidTarget(W.Range))
            {
                var wpos = GetBestWPos(false, Target);
                if (wpos != default(Vector3)) W.Cast(wpos);
            }

            if (Q.IsReady() && mode.IsChecked("harass.q") && Target.IsValidTarget(Q.Range)) Q.Cast(Target);

            if (Player.HasBuff("infernalguardiantime"))
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, Target);

                if (Tibbers != null && Tibbers.IsValid && Tibbers.IsInAutoAttackRange(Target))
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttackPet, Target);
            }

            return;
        }

        public override void OnLaneClear()
        {
            var mode = Features.First(it => it.NameFeature == "Lane Clear");

            bool UseItem = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, 400).Count() >= 3;
            if (UseItem) { Activator.tiamat.Cast(); Activator.hydra.Cast(); }

            if (Player.ManaPercent <= mode.SliderValue("laneclear.mana%")) return;

            if (Q.IsReady() && mode.IsChecked("laneclear.q"))
            {
                IEnumerable<Obj_AI_Base> IEMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition, Q.Range).Where(minion => minion.Health <= DamageUtil.GetSpellDamage(minion, SpellSlot.Q, true , true)).OrderBy(minion => minion.Distance(Player.Position.To2D()));

                if (IEMinions.Any())
                {
                    switch (mode.SliderValue("laneclear.q.mode"))
                    {
                        case 0:
                            Q.Cast(IEMinions.First());
                            break;
                        case 1:
                            if (Player.CountEnemiesInRange(700) == 0) Q.Cast(IEMinions.First());
                            break;
                        case 2:
                            if (!Player.HasBuff("pyromania_particle")) Q.Cast(IEMinions.First());
                            break;
                    }
                }
            }

            if (mode.IsChecked("laneclear.w") && W.IsReady() && Player.ManaPercent >= mode.SliderValue("laneclear.mana%"))
            {
                var WPos = GetBestWPos(true);
                if (WPos != default(Vector3)) W.Cast(WPos);
            }

            return;
        }

        public override void OnLastHit()
        {
            base.OnLastHit();

            var mode = Features.First(it => it.NameFeature == "Last Hit");

            if (Player.ManaPercent <= mode.SliderValue("lasthit.mana%")) return;

            if (Q.IsReady() && mode.IsChecked("lasthit.q"))
            {
                IEnumerable<Obj_AI_Base> IEMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition, Q.Range).Where(minion => minion.Health <= DamageUtil.GetSpellDamage(minion, SpellSlot.Q, true, true)).OrderBy(minion => minion.Distance(Player.Position.To2D()));

                if (IEMinions.Any())
                {
                    switch (mode.SliderValue("lasthit.q.mode"))
                    {
                        case 0:
                            Q.Cast(IEMinions.First());
                            break;
                        case 1:
                            if (Player.CountEnemiesInRange(700) == 0) Q.Cast(IEMinions.First());
                            break;
                        case 2:
                            if (!Player.HasBuff("pyromania_particle")) Q.Cast(IEMinions.First());
                            break;
                    }
                }
            }

            return;
        }

        public override void OnJungleClear()
        {
            var mode = Features.First(it => it.NameFeature == "Jungle Clear");

            bool UseItem = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, 400).Count() >= 1;
            if (UseItem) { Activator.tiamat.Cast(); Activator.hydra.Cast(); }

            if (Player.ManaPercent < mode.SliderValue("jungleclear.mana%")) return;

            var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Q.Range).FirstOrDefault(it => it.Health > Player.GetAutoAttackDamage(it));

            if (minion != null)
            {
                if (Q.IsReady() && mode.IsChecked("jungleclear.q")) Q.Cast(minion);

                if (W.IsReady() && mode.IsChecked("jungleclear.w")) W.Cast(minion);
            }

            return;
        }

        public override void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            base.OnGapCloser(sender, e);

            if (sender.IsMe || sender.IsAlly || !sender.IsValidTarget(W.Range)) return;

            var gapclose = Features.First(it => it.NameFeature == "Misc").IsChecked("misc.gapcloser");

            if (gapclose && (Player.HasBuff("pyromania_particle") || (Player.GetBuffCount("pyromania") >= 3 && Q.IsReady() && W.IsReady())))
            {
                if (W.IsReady() && sender.IsValidTarget(W.Range)) W.Cast(sender);
                if (Q.IsReady() && sender.IsValidTarget(Q.Range)) Q.Cast(sender);
            }

            return;
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            base.OnPossibleToInterrupt(sender, args);

            var misc = Features.First(it => it.NameFeature == "Misc");

            if (!sender.IsValidTarget(R.Range) || args.DangerLevel < DangerLevel.High || !misc.IsChecked("misc.interrupter")) return;

            if (Player.HasBuff("pyromania_particle"))
            {
                if (W.IsReady()) { W.Cast(sender); return; }

                if (Q.IsReady()) { Q.Cast(sender); return; }

                if (R.IsReady()) { R.Cast(sender); return; }

                return;
            }
            else
            {
                var qisready = Q.IsReady();
                var wisready = W.IsReady();
                var eisready = E.IsReady();
                var risready = R.IsReady();

                if (Player.GetBuffCount("pyromania") >= 3)
                {
                    if (qisready)
                    {
                        if (wisready)
                        {
                            if (W.Cast(sender))
                                EloBuddy.SDK.Core.DelayAction(() => Q.Cast(sender), 100);
                            return;
                        }
                        
                        if (eisready)
                        {
                            if (E.Cast(sender))
                                EloBuddy.SDK.Core.DelayAction(() => Q.Cast(sender), 100);
                            return;
                        }

                        if (risready)
                        {
                            if (R.Cast(sender))
                                EloBuddy.SDK.Core.DelayAction(() => Q.Cast(sender), 100);
                            return;
                        }

                        return;
                    }

                    if (wisready)
                    {
                        if (eisready)
                        {
                            if (E.Cast(sender))
                                EloBuddy.SDK.Core.DelayAction(() => W.Cast(sender), 100);
                            return;
                        }

                        if (risready)
                        {
                            if (R.Cast(sender))
                                EloBuddy.SDK.Core.DelayAction(() => W.Cast(sender), 100);
                            return;
                        }

                        return;
                    }

                    if (eisready && risready)
                    {
                        if (E.Cast())
                            EloBuddy.SDK.Core.DelayAction(() => R.Cast(sender), 100);
                    }
                }

                return;
            }
        }

        private void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var mode = Features.First(it => it.NameFeature == "Combo");

            if (!mode.IsChecked("combo.aa"))
            { args.Process = false; return; }

            if (!mode.IsChecked("combo.aa.maxrange") && Player.Distance(target) >= 530)
            { args.Process = false; return; }
        }

        //------------------------------------|| Extension ||--------------------------------------

        //-----------------------------CountRHits(Vector2 CastPosition)-----------------------------

        int CountRHits(Vector2 CastPosition)
        {
            int Hits = new int();

            foreach (Vector2 EnemyPos in GetEnemiesPosition())
            {
                if (CastPosition.Distance(EnemyPos) <= 250) Hits += 1;
            }

            return Hits;
        }

        //----------------------------GetBestRPos(Vector2 TargetPosition)---------------------------

        Dictionary<Vector2, int> GetBestRPos(Vector2 TargetPosition)
        {
            Dictionary<Vector2, int> PosAndHits = new Dictionary<Vector2, int>();

            List<Vector2> RPos = new List<Vector2>
            {
                new Vector2(TargetPosition.X - 250, TargetPosition.Y + 100),
                new Vector2(TargetPosition.X - 250, TargetPosition.Y),

                new Vector2(TargetPosition.X - 200, TargetPosition.Y + 300),
                new Vector2(TargetPosition.X - 200, TargetPosition.Y + 200),
                new Vector2(TargetPosition.X - 200, TargetPosition.Y + 100),
                new Vector2(TargetPosition.X - 200, TargetPosition.Y - 100),
                new Vector2(TargetPosition.X - 200, TargetPosition.Y),

                new Vector2(TargetPosition.X - 160, TargetPosition.Y - 160),

                new Vector2(TargetPosition.X - 100, TargetPosition.Y + 300),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y + 200),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y + 100),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y + 250),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y - 200),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y - 100),
                new Vector2(TargetPosition.X - 100, TargetPosition.Y),

                new Vector2(TargetPosition.X, TargetPosition.Y + 300),
                new Vector2(TargetPosition.X, TargetPosition.Y + 270),
                new Vector2(TargetPosition.X, TargetPosition.Y + 200),
                new Vector2(TargetPosition.X, TargetPosition.Y + 100),

                new Vector2(TargetPosition.X, TargetPosition.Y),

                new Vector2(TargetPosition.X, TargetPosition.Y - 100),
                new Vector2(TargetPosition.X, TargetPosition.Y - 200),

                new Vector2(TargetPosition.X + 100, TargetPosition.Y),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y - 100),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y - 200),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y + 100),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y + 200),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y + 250),
                new Vector2(TargetPosition.X + 100, TargetPosition.Y + 300),

                new Vector2(TargetPosition.X + 160, TargetPosition.Y - 160),

                new Vector2(TargetPosition.X + 200, TargetPosition.Y),
                new Vector2(TargetPosition.X + 200, TargetPosition.Y - 100),
                new Vector2(TargetPosition.X + 200, TargetPosition.Y + 100),
                new Vector2(TargetPosition.X + 200, TargetPosition.Y + 200),
                new Vector2(TargetPosition.X + 200, TargetPosition.Y + 300),

                new Vector2(TargetPosition.X + 250, TargetPosition.Y),
                new Vector2(TargetPosition.X + 250, TargetPosition.Y + 100),
            };

            foreach (Vector2 pos in RPos)
            {
                PosAndHits.Add(pos, CountRHits(pos));
            }

            Vector2 PosToGG = PosAndHits.First(pos => pos.Value == PosAndHits.Values.Max()).Key;
            int Hits = PosAndHits.First(pos => pos.Key == PosToGG).Value;

            return new Dictionary<Vector2, int>() { { PosToGG, Hits } };
        }

        //----------------------------------GetEnemiesPosition()------------------------------------

        List<Vector3> GetEnemiesPosition()
        {
            List<Vector3> Positions = new List<Vector3>();

            foreach (AIHeroClient Hero in EntityManager.Heroes.Enemies.Where(hero => !hero.IsDead && Player.Distance(hero) <= 1200))
            {
                Positions.Add(Prediction.Position.PredictUnitPosition(Hero, 500).To3D());
            }

            return Positions;
        }

        //--------------------------------------GetBestWPos----------------------------------------

        Vector3 GetBestWPos(bool minions = false, AIHeroClient Target = null)
        {
            if (minions)
            {
                var CS = new List<Geometry.Polygon.Sector>();

                var Minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(it => it.IsValidTarget(W.Range)).OrderByDescending(it => it.Distance(Player)).FirstOrDefault();

                if (Minion == null) return default(Vector3);

                var Vectors = new List<Vector3>() { new Vector3(Minion.ServerPosition.X + 550, Minion.ServerPosition.Y, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X - 550, Minion.ServerPosition.Y, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X, Minion.ServerPosition.Y + 550, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X, Minion.ServerPosition.Y - 550, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X + 230, Minion.ServerPosition.Y, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X - 230, Minion.ServerPosition.Y, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X, Minion.ServerPosition.Y + 230, Minion.ServerPosition.Z), new Vector3(Minion.ServerPosition.X, Minion.ServerPosition.Y - 230, Minion.ServerPosition.Z), Minion.ServerPosition };

                var CS1 = new Geometry.Polygon.Sector(Player.Position, Vectors[0], ANGLE, 585);
                var CS2 = new Geometry.Polygon.Sector(Player.Position, Vectors[1], ANGLE, 585);
                var CS3 = new Geometry.Polygon.Sector(Player.Position, Vectors[2], ANGLE, 585);
                var CS4 = new Geometry.Polygon.Sector(Player.Position, Vectors[3], ANGLE, 585);
                var CS5 = new Geometry.Polygon.Sector(Player.Position, Vectors[4], ANGLE, 585);
                var CS6 = new Geometry.Polygon.Sector(Player.Position, Vectors[5], ANGLE, 585);
                var CS7 = new Geometry.Polygon.Sector(Player.Position, Vectors[6], ANGLE, 585);
                var CS8 = new Geometry.Polygon.Sector(Player.Position, Vectors[7], ANGLE, 585);
                var CS9 = new Geometry.Polygon.Sector(Player.Position, Vectors[8], ANGLE, 585);

                CS.Add(CS1);
                CS.Add(CS2);
                CS.Add(CS3);
                CS.Add(CS4);
                CS.Add(CS5);
                CS.Add(CS6);
                CS.Add(CS7);
                CS.Add(CS8);
                CS.Add(CS9);

                var CSHits = new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                for (byte j = 0; j < 9; j++)
                {
                    foreach (Obj_AI_Base minion in EntityManager.MinionsAndMonsters.EnemyMinions.Where(it => it.IsValidTarget(W.Range)))
                    {
                        if (CS.ElementAt(j).IsInside(minion)) CSHits[j]++;
                    }
                }

                int i = CSHits.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;

                if (CSHits[i] < Features.First(it => it.NameFeature == "Lane Clear").SliderValue("laneclear.w.minminions")) return default(Vector3);

                return Vectors[i];
            }
            else if (Target != null && Target.IsValidTarget())
            {
                var CS = new List<Geometry.Polygon.Sector>();
                var Vectors = new List<Vector3>() { new Vector3(Target.ServerPosition.X + 550, Target.ServerPosition.Y, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X - 550, Target.ServerPosition.Y, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X, Target.ServerPosition.Y + 550, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X, Target.ServerPosition.Y - 550, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X + 230, Target.ServerPosition.Y, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X - 230, Target.ServerPosition.Y, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X, Target.ServerPosition.Y + 230, Target.ServerPosition.Z), new Vector3(Target.ServerPosition.X, Target.ServerPosition.Y - 230, Target.ServerPosition.Z), Target.ServerPosition };

                var CS1 = new Geometry.Polygon.Sector(Player.Position, Vectors[0], ANGLE, 585);
                var CS2 = new Geometry.Polygon.Sector(Player.Position, Vectors[1], ANGLE, 585);
                var CS3 = new Geometry.Polygon.Sector(Player.Position, Vectors[2], ANGLE, 585);
                var CS4 = new Geometry.Polygon.Sector(Player.Position, Vectors[3], ANGLE, 585);
                var CS5 = new Geometry.Polygon.Sector(Player.Position, Vectors[4], ANGLE, 585);
                var CS6 = new Geometry.Polygon.Sector(Player.Position, Vectors[5], ANGLE, 585);
                var CS7 = new Geometry.Polygon.Sector(Player.Position, Vectors[6], ANGLE, 585);
                var CS8 = new Geometry.Polygon.Sector(Player.Position, Vectors[7], ANGLE, 585);
                var CS9 = new Geometry.Polygon.Sector(Player.Position, Vectors[8], ANGLE, 585);

                CS.Add(CS1);
                CS.Add(CS2);
                CS.Add(CS3);
                CS.Add(CS4);
                CS.Add(CS5);
                CS.Add(CS6);
                CS.Add(CS7);
                CS.Add(CS8);
                CS.Add(CS9);

                var CSHits = new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                for (byte j = 0; j < 9; j++)
                {
                    foreach (AIHeroClient hero in EntityManager.Heroes.Enemies.Where(enemy => !enemy.IsDead && enemy.IsValidTarget(W.Range)))
                    {
                        if (CS.ElementAt(j).IsInside(hero)) CSHits[j]++;
                        if (hero == Target) CSHits[j] += 10;
                    }
                }

                byte i = (byte)CSHits.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;

                if (CSHits[i] <= 0) return default(Vector3);

                return Vectors[i];
            }

            return default(Vector3);
        }

        //-------------------------------------------KS--------------------------------------------

        void KS()
        {
            if (Q.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && DamageUtil.GetSpellDamage(enemy, SpellSlot.Q, true, true) >= enemy.Health);
                if (bye != null) { Q.Cast(bye); return; }
            }

            if (W.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(W.Range) && DamageUtil.GetSpellDamage(enemy, SpellSlot.W, true, true) >= enemy.Health);
                if (bye != null) { W.Cast(bye); return; }
            }

            if (Q.IsReady() && W.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && DamageUtil.GetSpellDamage(enemy, SpellSlot.Q, true, true) + DamageUtil.GetSpellDamage(enemy, SpellSlot.W, true, true) >= enemy.Health);
                if (bye != null) { W.Cast(bye); return; }
            }
        }
    }
}
