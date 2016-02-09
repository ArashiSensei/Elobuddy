using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
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
    class Elise : PluginModel
    {
        #region Scope Variables

        public bool spiderForm = false;

        Spell.Targeted Q1 { get { return (Spell.Targeted)Spells[0]; } }
        Spell.Targeted Q2 { get { return (Spell.Targeted)Spells[4]; } }
        Spell.Skillshot W1 { get { return (Spell.Skillshot)Spells[1]; } }
        Spell.Active W2 { get { return (Spell.Active)Spells[5]; } }
        Spell.Skillshot E1 { get { return (Spell.Skillshot)Spells[2]; } }
        Spell.Skillshot E2 { get { return (Spell.Skillshot)Spells[6]; } }
        Spell.Active R { get { return (Spell.Active)Spells[3]; } }

        #endregion

        public override void Init()
        {
            InitVariables();
        }

        public override void InitVariables()
        {
            Activator = new Activator();

            Spells = new List<Spell.SpellBase>
            {
                new Spell.Targeted(SpellSlot.Q, 625),
                new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular),
                new Spell.Skillshot(SpellSlot.E, 1075, SkillShotType.Linear, 250, 1600, 80) { AllowedCollisionCount = 0},
                new Spell.Active(SpellSlot.R),
                new Spell.Targeted(SpellSlot.Q, 475),
                new Spell.Active(SpellSlot.W),
                new Spell.Skillshot(SpellSlot.E, 750, SkillShotType.Circular),
            };

            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Spells[0], new float[]{ 0, 40 , 75 , 110 , 145 , 180 }, new [] { 0, 0f, 0f, 0f, 0f, 0f }, DamageType.Magical) {HpPercentIncrease = new []{0f, 4f, 4f, 4f, 4f, 4f}},
                new SpellDamage(Spells[0], new float[]{ 0, 60 , 100 , 140 , 180 , 220 }, new [] { 0, 0f, 0f, 0f, 0f, 0f }, DamageType.Magical) {HpPercentDecrease = new []{0f, 8f, 8f, 8f, 8f, 8f}},
                new SpellDamage(Spells[1], new float[]{ 0, 75 , 125 , 175 , 225 , 275 }, new [] { 0, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f }, DamageType.Magical)
            };

            InitMenu();

            DamageIndicator.Initialize(DamageUtil.GetComboDamage);

            //new SkinController(4);
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
                    new ValueCheckbox(true, "draw.q", "Draw Q"),
                    new ValueCheckbox(true, "draw.w", "Draw W"),
                    new ValueCheckbox(true, "draw.e", "Draw E")
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
                    new ValueCheckbox(true,  "combo.w", "Combo W"),
                    new ValueCheckbox(true,  "combo.e", "Combo E"),
                    new ValueCheckbox(true,  "combo.q2", "Combo Q2"),
                    new ValueCheckbox(true,  "combo.w2", "Combo W2"),
                    new ValueCheckbox(true,  "combo.e2", "Combo E2"),
                    new ValueCheckbox(true,  "combo.r", "Change Form")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Harass/Gank",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 50, "harass.mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "harass.q", "Harass Q"),
                    new ValueCheckbox(true,  "harass.w", "Harass W"),
                    new ValueCheckbox(true,  "harass.e", "Harass E"),
                    new ValueCheckbox(true,  "harass.q2", "Harass Q2"),
                    new ValueCheckbox(true,  "harass.w2", "Harass W2"),
                    new ValueCheckbox(false, "harass.e2", "Harass E2"),
                    new ValueCheckbox(true,  "harass.r", "Change Form")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Lane Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 50, "laneclear.mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "laneclear.q", "Lane Clear Q"),
                    new ValueCheckbox(true,  "laneclear.w", "Lane Clear W"),
                    new ValueCheckbox(true,  "laneclear.q2", "Lane Clear Q2"),
                    new ValueCheckbox(true,  "laneclear.w2", "Lane Clear W2"),
                    new ValueCheckbox(true,  "laneclear.r", "Change Form")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Jungle Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 10, "jungleclear.mana", "Minimum mana %"),
                    new ValueCheckbox(true, "jungleclear.q", "Jungle Clear Q"),
                    new ValueCheckbox(true, "jungleclear.w", "Jungle Clear W"),
                    new ValueCheckbox(true, "jungleclear.q2", "Jungle Clear Q2"),
                    new ValueCheckbox(true, "jungleclear.w2", "Jungle Clear W2"),
                    new ValueCheckbox(true, "jungleclear.r", "Change Form")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Misc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "misc.antiGapE", "E on gap closers"),
                    new ValueSlider(100, 0 , 60, "misc.antiGapE.antigap", "Anti-Gap Minimum HP % to cast E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);
        }

        public override void OnDraw(EventArgs args)
        {
            var draw = Features.Find(f => f.NameFeature == "Draw");

            if (draw.IsChecked("disable"))
            {
                DamageIndicator.Enabled = false;
                return;
            }

            if (!spiderForm)
            {
                if (draw.IsChecked("draw.q"))
                    Circle.Draw(Q1.IsReady() ? Color.Blue : Color.Red, Q1.Range, Player.Instance.Position);

                if (draw.IsChecked("draw.w"))
                    Circle.Draw(W1.IsReady() ? Color.Blue : Color.Red, W1.Range, Player.Instance.Position);

                if (draw.IsChecked("draw.e"))
                    Circle.Draw(E1.IsReady() ? Color.Blue : Color.Red, E1.Range, Player.Instance.Position);

            }
            else
            {
                if (draw.IsChecked("draw.q"))
                    Circle.Draw(Q2.IsReady() ? Color.Blue : Color.Red, Q2.Range, Player.Instance.Position);

                if (draw.IsChecked("draw.e"))
                    Circle.Draw(E2.IsReady() ? Color.Blue : Color.Red, E2.Range, Player.Instance.Position);

            }

            DamageIndicator.Enabled = draw.IsChecked("dmgIndicator");

        }

        public override void OnCombo()
        {
            var target = TargetSelector.GetTarget(spiderForm ? E2.Range : E1.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            var combo = Features.Find(f => f.NameFeature == "Combo");

            if (spiderForm)
            {
                if (R.IsReady() && combo.IsChecked("combo.r"))
                {
                    if (W1.IsReady() && Q1.IsReady() && E1.IsReady())
                    {
                        R.Cast();
                    }
                }

                if (Q2.IsReady() && combo.IsChecked("combo.q2") && Q2.IsInRange(target))
                {
                    Q2.Cast(target);
                }

                if (E2.IsReady() && combo.IsChecked("combo.e2") && E2.IsInRange(target) && !Q2.IsInRange(target))
                {
                    E2.Cast(target);
                }

                if (W2.IsReady() && combo.IsChecked("combo.w2") && Player.Instance.IsInAutoAttackRange(target))
                {
                    W2.Cast();
                }
            }
            else
            {
                if (R.IsReady() && combo.IsChecked("combo.r"))
                {
                    if (!Q1.IsReady())
                    {
                        if (W1.IsReady() && combo.IsChecked("combo.w") && W1.IsInRange(target))
                        {
                            W1.Cast(W1.GetPrediction(target).CastPosition);
                            EloBuddy.SDK.Core.DelayAction(() => R.Cast(), 250);
                        }
                        else
                        {
                            R.Cast();
                        }
                        
                    }
                }

                if (E1.IsReady() && combo.IsChecked("combo.e") && E1.IsInRange(target))
                {
                    SpellsUtil.HitChanceCast(E1, target);
                }

                if (Q1.IsReady() && combo.IsChecked("combo.q") && Q1.IsInRange(target))
                {
                    Q1.Cast(target);
                }
            }
            
        }

        public override void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            
        }

        public override void OnHarass()
        {
            var target = TargetSelector.GetTarget(spiderForm ? E2.Range : E1.Range, DamageType.Magical);

            var harass = Features.Find(f => f.NameFeature == "Harass");

            if (target == null || !target.IsValidTarget(1000) || harass.SliderValue("harass.mana") > Player.Instance.ManaPercent) return;

            if (spiderForm)
            {
                if (R.IsReady() && harass.IsChecked("harass.r"))
                {
                    if (!W2.IsReady() && !Q2.IsReady() && !Player.Instance.IsInAutoAttackRange(target))
                    {
                        R.Cast();
                    }
                }

                if (Q2.IsReady() && harass.IsChecked("harass.q2") && Q2.IsInRange(target))
                {
                    Q2.Cast(target);
                }

                if (E2.IsReady() && harass.IsChecked("harass.e2") && E2.IsInRange(target) && !Q2.IsInRange(target))
                {
                    E2.Cast(target);
                }

                if (W2.IsReady() && harass.IsChecked("harass.w2") && Player.Instance.IsInAutoAttackRange(target))
                {
                    W2.Cast();
                }
            }
            else
            {
                if (R.IsReady() && harass.IsChecked("harass.r"))
                {
                    if (!Q1.IsReady() && !E1.IsReady())
                    {
                        if (W1.IsReady() && harass.IsChecked("harass.w") && W1.IsInRange(target))
                        {
                            W1.Cast(W1.GetPrediction(target).CastPosition);
                            EloBuddy.SDK.Core.DelayAction(() => R.Cast(), 250);
                        }
                        else
                        {
                            R.Cast();
                        }

                    }
                }

                if (E1.IsReady() && harass.IsChecked("harass.e") && E1.IsInRange(target))
                {
                    SpellsUtil.HitChanceCast(E1, target);
                }

                if (Q1.IsReady() && harass.IsChecked("harass.q") && Q1.IsInRange(target))
                {
                    Q1.Cast(target);
                }

                if (W1.IsReady() && harass.IsChecked("harass.w") && W1.IsInRange(target))
                {
                    W1.Cast(W1.GetPrediction(target).CastPosition);
                }
            }
        }

        public override void OnLaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsValidTarget(spiderForm ? E2.Range : E1.Range));

            var laneclear = Features.Find(f => f.NameFeature == "Lane Clear");

            if (minions == null || !minions.Any(t => t.IsValidTarget(spiderForm ? E2.Range : E1.Range)) || laneclear.SliderValue("laneclear.mana") > Player.Instance.ManaPercent) return;

            var target = minions.Aggregate((curMax, x) => ((curMax == null && x.IsValid) || x.MaxHealth > curMax.MaxHealth ? x : curMax));

            if (!target.IsValidTarget(spiderForm ? E2.Range : E1.Range))
                target = minions.FirstOrDefault();

            if (spiderForm)
            {
                if (R.IsReady() && laneclear.IsChecked("laneclear.r"))
                {
                    if (!W2.IsReady() && !Q2.IsReady())
                    {
                        R.Cast();
                    }
                }

                if (Q2.IsReady() && laneclear.IsChecked("laneclear.q2") && Q2.IsInRange(target))
                {
                    Q2.Cast(target);
                }

                if (W2.IsReady() && laneclear.IsChecked("laneclear.w2") && Player.Instance.IsInAutoAttackRange(target))
                {
                    W2.Cast();
                }
            }
            else
            {
                if (R.IsReady() && laneclear.IsChecked("laneclear.r"))
                {
                    if (!Q1.IsReady() && !W1.IsReady())
                    {
                        if (W1.IsReady() && laneclear.IsChecked("laneclear.w") && W1.IsInRange(target))
                        {
                            W1.Cast(target.Position);
                            EloBuddy.SDK.Core.DelayAction(() => R.Cast(), 250);
                        }
                        else
                        {
                            R.Cast();
                        }

                    }
                }

                if (Q1.IsReady() && laneclear.IsChecked("laneclear.q") && Q1.IsInRange(target))
                {
                    Q1.Cast(target);
                }

                if (W1.IsReady() && laneclear.IsChecked("laneclear.w") && W1.IsInRange(target))
                {
                    W1.Cast(target.Position);
                }
            }

        }

        public override void OnJungleClear()
        {
            var minions = EntityManager.MinionsAndMonsters.Monsters.Where(t => t.IsValidTarget(spiderForm ? E2.Range : E1.Range));

            var jungleclear = Features.Find(f => f.NameFeature == "Jungle Clear");

            if (minions == null || !minions.Any(t => t.IsValidTarget(spiderForm ? E2.Range : E1.Range)) || jungleclear.SliderValue("jungleclear.mana") > Player.Instance.ManaPercent) return;

            var target = minions.Aggregate((curMax, x) => ((curMax == null && x.IsValid) || x.MaxHealth > curMax.MaxHealth ? x : curMax));

            if(!target.IsValidTarget(spiderForm ? E2.Range : E1.Range))
                target = minions.FirstOrDefault();

            if (spiderForm)
            {
                if (R.IsReady() && jungleclear.IsChecked("jungleclear.r"))
                {
                    if (W1.IsReady() && Q1.IsReady())
                    {
                        R.Cast();
                    }
                }

                if (Q2.IsReady() && jungleclear.IsChecked("jungleclear.q2") && Q2.IsInRange(target))
                {
                    Q2.Cast(target);
                }

                if (W2.IsReady() && jungleclear.IsChecked("jungleclear.w2") && Player.Instance.IsInAutoAttackRange(target))
                {
                    W2.Cast();
                }
            }
            else
            {
                if (R.IsReady() && jungleclear.IsChecked("jungleclear.r"))
                {
                    if (!Q1.IsReady() && !W1.IsReady())
                    {
                        if (W1.IsReady() && jungleclear.IsChecked("jungleclear.w") && W1.IsInRange(target))
                        {
                            W1.Cast(target.Position);
                            EloBuddy.SDK.Core.DelayAction(() => R.Cast(), 250);
                        }
                        else
                        {
                            R.Cast();
                        }

                    }
                }

                if (Q1.IsReady() && jungleclear.IsChecked("jungleclear.q") && Q1.IsInRange(target))
                {
                    Q1.Cast(target);
                }

                if (W1.IsReady() && jungleclear.IsChecked("jungleclear.w") && W1.IsInRange(target))
                {
                    W1.Cast(target.Position);
                }
            }

        }

        public override void PermaActive()
        {
            spiderForm = Player.Instance.AttackRange < 500;
        }

        public override void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            base.OnGapCloser(sender, e);

            var misc = Features.Find(f => f.NameFeature == "Misc");

            if (!misc.IsChecked("misc.antiGapE")) return;

            if (e.End.Distance(Player.Instance) < 50 && Player.Instance.HealthPercent <= misc.SliderValue("misc.antiGapE.antigap"))
            {
                Spells[0].Cast(Player.Instance.Position);
            }
        }
    }
}
