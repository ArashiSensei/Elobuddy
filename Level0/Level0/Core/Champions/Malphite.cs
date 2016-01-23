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
    class Malphite : PluginModel
    {
        readonly AIHeroClient Player = EloBuddy.Player.Instance;

        Spell.Targeted Q { get { return (Spell.Targeted)Spells[0]; } }
        Spell.Active W { get { return (Spell.Active)Spells[1]; } }
        Spell.Active E { get { return (Spell.Active)Spells[2]; } }
        Spell.Skillshot R { get { return (Spell.Skillshot)Spells[3]; } }

        public override void Init()
        {
            InitVariables();
        }

        public override void InitVariables()
        {
            Activator = new Activator(DamageType.Physical);

            Spells = new List<Spell.SpellBase>
            {
                new Spell.Targeted(SpellSlot.Q, 625),
                new Spell.Active(SpellSlot.W),
                new Spell.Active(SpellSlot.E, 320),
                new Spell.Skillshot(SpellSlot.R, 1000, SkillShotType.Circular, 250, 700, 270)
            };

            R.AllowedCollisionCount = int.MaxValue;
            R.MinimumHitChance = HitChance.Medium;

            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Q, new float[] { 0, 70, 120, 170, 220, 270 }, new [] { 0, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f }, DamageType.Magical),
                new SpellDamage(E, new float[] { 0, 60, 100, 140, 180, 220 }, new [] { 0, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f }, DamageType.Magical),
                new SpellDamage(R, new float[] { 0, 200, 300, 400 }, new [] { 0, 1f, 1f, 1f, 1f, 1f }, DamageType.Magical)
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
                    new ValueCheckbox(true, "combo.e", "Combo E"),
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
                    new ValueCheckbox(true, "harass.w", "Harass W"),
                    new ValueCheckbox(true, "harass.e", "Harass E"),
                    new ValueSlider(99, 1, 30, "harass.mana%", "Harass MinMana%")
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
                    new ValueCheckbox(true,  "laneclear.w", "Lane Clear W"),
                    new ValueCheckbox(true,  "laneclear.e", "Lane Clear E", true),
                    new ValueSlider(7, 1, 3, "laneclear.e.minminions", "Min minions E"),
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
                    new ValueSlider(100, 1, 30, "lasthit.mana%", "Last Hit MinMana%", true)
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
                    new ValueCheckbox(true,  "jungleclear.e", "Jungle Clear E"),
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
                    new ValueKeybind(false, "misc.autor", "Auto R on target (Ignore the min enemies slider):", KeyBind.BindTypes.HoldActive, 'J', true),
                    new ValueCheckbox(true, "misc.gapcloser", "Q on enemy gapcloser", true),
                    new ValueCheckbox(true, "misc.interrupter", "Interrupt dangerous spells", true)
                }
            };

            feature.ToMenu();
            Features.Add(feature);
        }

        public override void PermaActive()
        {
            var misc = Features.First(it => it.NameFeature == "Misc");

            //----Auto R

            if (R.IsReady() && misc.IsChecked("misc.autor"))
            {
                var Target = TargetSelector.GetTarget(R.Range, DamageType.Magical);

                if (Target != null && Target.IsValidTarget(R.Range - 50)) R.Cast(Target);
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

            if (R.IsReady() && draw.IsChecked("draw.rposandhits"))
            {
                var Target = TargetSelector.GetTarget(R.Range + 300, DamageType.Magical);

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

            if (draw.IsChecked("draw.r"))
                Circle.Draw(R.IsReady() ? Color.Blue : Color.Red, R.Range, Player.Position);

            if (draw.IsChecked("draw.q"))
                Circle.Draw(Q.IsReady() ? Color.Blue : Color.Red, Q.Range, Player.Position);

            if (draw.IsChecked("draw.e"))
                Circle.Draw(E.IsReady() ? Color.Blue : Color.Red, E.Range, Player.Position);

            DamageIndicator.Enabled = draw.IsChecked("dmgIndicator");

        }

        public override void OnCombo()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Target == null || !Target.IsValidTarget()) return;

            var mode = Features.First(it => it.NameFeature == "Combo");

            if (R.IsReady() && Target.IsValidTarget(R.Range) && mode.IsChecked("combo.r"))
            {
                var PosAndHits = GetBestRPos(Target.ServerPosition.To2D());

                if (PosAndHits.First().Value >= mode.SliderValue("combo.r.minenemies"))
                {
                    if (EntityManager.Heroes.Allies.Where(ally => ally != Player && ally.Distance(Player) <= 700).Count() > 0)
                    {
                        if (Activator.righteousGlory.IsReady()) Activator.righteousGlory.Cast();
                        if (Activator.talisma.IsReady()) Activator.talisma.Cast();
                    }
                    R.Cast(PosAndHits.First().Key.To3D());
                }
            }

            if (Q.IsReady() && Target.IsValidTarget(Q.Range) && mode.IsChecked("combo.q")) Q.Cast(Target);

            if (E.IsReady() && Target.IsValidTarget(E.Range) && mode.IsChecked("combo.e")) E.Cast();

            if (W.IsReady() && Player.IsInAutoAttackRange(Target) && mode.IsChecked("combo.w")) W.Cast();

            return;
        }

        public override void OnHarass()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (Target == null || !Target.IsValidTarget()) return;

            var mode = Features.First(it => it.NameFeature == "Harass");

            if (Q.IsReady() && Target.IsValidTarget(Q.Range) && mode.IsChecked("harass.q")) Q.Cast(Target);

            if (E.IsReady() && Target.IsValidTarget(E.Range) && mode.IsChecked("harass.e")) E.Cast();

            if (W.IsReady() && Player.IsInAutoAttackRange(Target) && mode.IsChecked("harass.w")) W.Cast();

            return;
        }

        public override void OnLaneClear()
        {
            var mode = Features.First(it => it.NameFeature == "Lane Clear");

            bool UseItem = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, 400).Count() >= 3;
            if (UseItem) { Activator.tiamat.Cast(); Activator.hydra.Cast(); }

            if (Player.ManaPercent <= mode.SliderValue("laneclear.mana%")) return;

            var ListMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition, Q.Range);

            if (!ListMinions.Any()) return;

            if (E.IsReady() && mode.IsChecked("laneclear.e") && ListMinions.Count(it => it.IsValidTarget(E.Range+70)) >= mode.SliderValue("laneclear.e.minminions"))
                E.Cast();

            if (Q.IsReady() && mode.IsChecked("laneclear.q"))
            {
                var qminion = ListMinions.FirstOrDefault(it => !Player.IsInAutoAttackRange(it) && DamageUtil.GetSpellDamage(it, SpellSlot.Q, true, true) >= it.Health);
                if (qminion != null) Q.Cast(qminion);
            }

            if (W.IsReady() && mode.IsChecked("laneclear.w") && ListMinions.Count(it => it.IsValidTarget(Player.GetAutoAttackRange() + 100)) >= 4)
                W.Cast();

            return;
        }

        public override void OnLastHit()
        {
            base.OnLastHit();

            var mode = Features.First(it => it.NameFeature == "Last Hit");

            if (Player.ManaPercent <= mode.SliderValue("lasthit.mana%") || !Q.IsReady() || !mode.IsChecked("lasthit.q")) return;

            var qminion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, Q.Range).FirstOrDefault(it => !Player.IsInAutoAttackRange(it) && DamageUtil.GetSpellDamage(it, SpellSlot.Q, true, true) >= it.Health);
            if (qminion != null) Q.Cast(qminion);

            return;
        }

        public override void OnJungleClear()
        {
            var mode = Features.First(it => it.NameFeature == "Jungle Clear");

            bool UseItem = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, 400).Count() >= 1;
            if (UseItem) { Activator.tiamat.Cast(); Activator.hydra.Cast(); }

            if (Player.ManaPercent < mode.SliderValue("jungleclear.mana%")) return;

            var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Q.Range);

            if (!minions.Any()) return;

            if (E.IsReady() && mode.IsChecked("jungleclear.e") && minions.Count(it => it.IsValidTarget(E.Range)) == minions.Count()) E.Cast();

            if (W.IsReady() && mode.IsChecked("jungleclear.w") && minions.Any(it => Player.IsInAutoAttackRange(it))) W.Cast();

            return;
        }

        public override void OnFlee()
        {
            base.OnFlee();

            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target != null && Target.IsValidTarget(Q.Range)) Q.Cast(Target);
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            base.OnAfterAttack(target, args);

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) return;

            var jungleclear = Features.First(it => it.NameFeature == "Jungle Clear");
            var Target = (Obj_AI_Base)target;

            if (Player.ManaPercent < jungleclear.SliderValue("jungleclear.mana%") || !Q.IsReady() || !jungleclear.IsChecked("jungleclear.q") || target.Health <= Player.GetAutoAttackDamage((Obj_AI_Base)Target)) return;

            Q.Cast(Target);
        }

        public override void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            base.OnGapCloser(sender, e);

            if (!Q.IsReady() || sender.IsMe || sender.IsAlly || !sender.IsValidTarget(Q.Range)) return;

            var gapclose = Features.First(it => it.NameFeature == "Misc").IsChecked("misc.gapcloser");

            if (gapclose) Q.Cast(sender);

            return;
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            base.OnPossibleToInterrupt(sender, args);

            var misc = Features.First(it => it.NameFeature == "Misc");

            if (!R.IsReady() || !sender.IsValidTarget(R.Range) || args.DangerLevel < DangerLevel.High || !misc.IsChecked("misc.interrupter")) return;

            R.Cast(sender);
        }

        //------------------------------------|| Extension ||--------------------------------------

        //-------------------------CountRHits(Vector2 CastPosition)---------------------------

        int CountRHits(Vector2 CastPosition)
        {
            int Hits = new int();

            foreach (Vector3 EnemyPos in GetEnemiesPosition())
            {
                if (CastPosition.Distance(EnemyPos) <= 260) Hits += 1;
            }

            return Hits;
        }

        //-----------------------GetBestRPos(Vector2 TargetPosition)--------------------------

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

        //------------------------------GetEnemiesPosition()----------------------------------

        List<Vector3> GetEnemiesPosition()
        {
            List<Vector3> Positions = new List<Vector3>();

            foreach (AIHeroClient Hero in EntityManager.Heroes.Enemies.Where(hero => !hero.IsDead && Player.Distance(hero) <= R.Range + E.Range))
            {
                Positions.Add(Prediction.Position.PredictUnitPosition(Hero, 500).To3D());
            }

            return Positions;
        }

        //----------------------------------GetEDamage()--------------------------------------

        float GetEDamage(Obj_AI_Base target)
        {
            return Player.CalculateDamageOnUnit(target, DamageType.Magical, new [] { 0, 60, 100, 140, 180, 220 }[E.Level] + 0.3f * Player.Armor + 0.2f * Player.TotalMagicalDamage);
        }

        //-----------------------------------------KS-----------------------------------------

        void KS()
        {
            if (Q.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && DamageUtil.GetSpellDamage(enemy, SpellSlot.Q, true, true) >= enemy.Health);
                if (bye != null) { Q.Cast(bye); return; }
            }

            if (E.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(E.Range) && GetEDamage(enemy) >= enemy.Health);
                if (bye != null) { E.Cast(); return; }
            }

            if (Q.IsReady() && E.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(E.Range) && GetEDamage(enemy) + DamageUtil.GetSpellDamage(enemy, SpellSlot.Q, true, true) >= enemy.Health);
                if (bye != null) { E.Cast(); return; }
            }
        }
    }
}