using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using LevelZero.Controller;
using LevelZero.Model;
using LevelZero.Model.Values;
using LevelZero.Util;
using SharpDX;
using Circle = EloBuddy.SDK.Rendering.Circle;
using Activator = LevelZero.Controller.Activator;

namespace LevelZero.Core.Champions
{
    class Quinn : PluginModel
    {
        Spell.Skillshot Q { get { return (Spell.Skillshot)Spells[0]; } }
        Spell.Active W { get { return (Spell.Active)Spells[1]; } }
        Spell.Targeted E { get { return (Spell.Targeted)Spells[2]; } }
        Spell.Active R { get { return (Spell.Active)Spells[3]; } }

        private bool castQ = false;

        public override void Init()
        {
            InitVariables();
        }

        public override void InitVariables()
        {
            Activator = new Activator();

            Spells = new List<Spell.SpellBase>
            {
                new Spell.Skillshot(SpellSlot.Q, 1025, SkillShotType.Linear, 0, 750, 210),
                new Spell.Active(SpellSlot.W, 2100),
                new Spell.Targeted(SpellSlot.E, 675),
                new Spell.Active(SpellSlot.R, 700)
            };
            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Q, new float[]{ 0, 20 , 45 , 70 , 95 , 120 }, new [] { 0, 0.8f, 0.9f, 1f, 1.1f, 1.2f }, DamageType.Physical),
                new SpellDamage(E, new float[]{ 0, 40 , 70 , 100 , 130 , 160 }, new [] { 0, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f }, DamageType.Physical)
            };
            InitMenu();
            DamageIndicator.Initialize(DamageUtil.GetComboDamage);
            new SkinController(4);
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
                    new ValueCheckbox(false, "draw.q", "Draw Q"),
                    new ValueCheckbox(false, "draw.w", "Draw W"),
                    new ValueCheckbox(false, "draw.e", "Draw E"),
                    new ValueCheckbox(false, "draw.r", "Draw R")
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
                    new ValueCheckbox(true,  "combo.e", "Combo E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "ComboMisc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(false,  "combo.misc.q", "Only Q After AA")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Harass",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueKeybind(false, "harass.auto", "Auto Harass", KeyBind.BindTypes.PressToggle),
                    new ValueSlider(100, 0 , 50, "mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "harass.q", "Harass Q"),
                    new ValueCheckbox(true,  "harass.e", "Harass E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Last Hit",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 50, "mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "lasthit.q", "Last hit Q"),
                    new ValueCheckbox(true,  "lasthit.e", "Last hit E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Lane Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 50, "mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "laneclear.q", "Lane Clear Q"),
                    new ValueCheckbox(true,  "laneclear.e", "Lane Clear E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Jungle Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 50, "mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "jungleclear.q", "Jungle Clear Q"),
                    new ValueCheckbox(true,  "jungleclear.e", "Jungle Clear E")
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

            if (draw.IsChecked("draw.q"))
                Circle.Draw(Spells[0].IsReady() ? Color.Blue : Color.Red, Spells[0].Range, Player.Instance.Position);

            if (draw.IsChecked("draw.w"))
                Circle.Draw(Spells[1].IsReady() ? Color.Blue : Color.Red, Spells[1].Range, Player.Instance.Position);

            if (draw.IsChecked("draw.e"))
                Circle.Draw(Spells[2].IsReady() ? Color.Blue : Color.Red, Spells[2].Range, Player.Instance.Position);

            if (draw.IsChecked("draw.r"))
                Circle.Draw(Spells[3].IsReady() ? Color.Blue : Color.Red, Spells[3].Range, Player.Instance.Position);

            DamageIndicator.Enabled = draw.IsChecked("dmgIndicator");

        }

        /*
            Spells[0] = Q
            Spells[1] = W
            Spells[2] = E
            Spells[3] = R
        */

        public override void OnCombo()
        {
            var combo = Features.Find(f => f.NameFeature == "Combo");
            var comboMisc = Features.Find(f => f.NameFeature == "ComboMisc");

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);

            if (target == null || !target.IsValidTarget(Q.Range) || !target.IsValidTargetUtil()) return;

            if (combo.IsChecked("combo.q") && Q.IsReady() && !Player.HasBuff("QuinnR"))
            {
                if (comboMisc.IsChecked("combo.misc.q"))
                {
                    castQ = true;
                }
                else
                {
                    var qPredict = Q.GetPrediction(target);

                    if (qPredict.HitChancePercent >= 75)
                    {
                        if (Q.Cast(qPredict.CastPosition))
                            castQ = false;
                    }
                }
            }

            if (combo.IsChecked("combo.e") && E.IsReady() && E.IsInRange(target))
            {
                if (target.Distance(Player.Instance) < Player.Instance.AttackRange - 100 || DamageUtil.Killable(target, SpellSlot.E))
                {
                    E.Cast(target);
                }
            }
        }

        public override void OnHarass()
        {
            var target = TargetSelector.GetTarget(Player.Instance.AttackRange, DamageType.Mixed);
            var harass = Features.Find(f => f.NameFeature == "Harass");

            if (target == null || !target.IsValidTarget() || !target.IsValidTargetUtil() || harass.SliderValue("mana") > Player.Instance.ManaPercent) return;

            if (harass.IsChecked("harass.q") && Q.IsReady())
            {
                var qPredict = Q.GetPrediction(target);

                if (qPredict.HitChancePercent >= 85)
                {
                    if (Q.Cast(qPredict.CastPosition))
                        castQ = false;
                }
            }
            if (harass.IsChecked("harass.e") && E.IsReady() && E.IsInRange(target))
            {
                if (target.Distance(Player.Instance) < Player.Instance.AttackRange || DamageUtil.Killable(target, SpellSlot.E))
                {
                    E.Cast(target);
                }
            }

        }

        public override void OnLaneClear()
        {
            if (!EntityManager.MinionsAndMonsters.EnemyMinions.Any(m => m.Distance(Player.Instance) < Q.Range)) return;

            var targets = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsValidTarget(Q.Range) && t.IsValidTarget(Q.Range));

            var laneclear = Features.Find(f => f.NameFeature == "Lane Clear");

            if (!targets.Any() || laneclear.SliderValue("mana") > Player.Instance.ManaPercent) return;

            var bestTarget = targets.Aggregate((curMax, x) => ((curMax == null && x.IsValid) || x.MaxHealth > curMax.MaxHealth ? x : curMax));

            if (Q.IsReady() && laneclear.IsChecked("laneclear.q"))
            {
                Q.Cast(Q.GetPrediction(bestTarget).CastPosition);
            }

            if (E.IsReady() && laneclear.IsChecked("laneclear.e") && DamageUtil.Killable(bestTarget, SpellSlot.E) && bestTarget != Orbwalker.LastTarget)
            {
                E.Cast(bestTarget);
            }

        }

        public override void OnJungleClear()
        {
            if (!EntityManager.MinionsAndMonsters.Monsters.Any(m => m.Distance(Player.Instance) < Q.Range)) return;

            var targets = EntityManager.MinionsAndMonsters.Monsters.Where(t => t.IsValidTarget(Q.Range) && t.IsValidTarget(Q.Range));

            var jungleclear = Features.Find(f => f.NameFeature == "Jungle Clear");

            if (!targets.Any() || jungleclear.SliderValue("mana") > Player.Instance.ManaPercent) return;

            var bestTarget = targets.Aggregate((curMax, x) => ((curMax == null && x.IsValid) || x.MaxHealth > curMax.MaxHealth ? x : curMax));

            if (Q.IsReady() && jungleclear.IsChecked("jungleclear.q"))
            {
                Q.Cast(Q.GetPrediction(bestTarget).CastPosition);
            }

            if (E.IsReady() && jungleclear.IsChecked("jungleclear.e") && DamageUtil.Killable(bestTarget, SpellSlot.E) && bestTarget != Orbwalker.LastTarget)
            {
                E.Cast(bestTarget);
            }

        }

        public override void OnLastHit()
        {
            var lastHit = Features.Find(f => f.NameFeature == "Last Hit");

            if (lastHit.SliderValue("mana") > Player.Instance.ManaPercent || !lastHit.IsChecked("lasthit.q") || !Q.IsReady()) return;

            var targets = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t != null && t.IsValidTarget(Q.Range) && Orbwalker.LastTarget != null && t.NetworkId != Orbwalker.LastTarget.NetworkId && DamageUtil.Killable(t, SpellSlot.Q));

            if (!targets.Any()) return;

            foreach (var predictQ in targets.Select(minion => Q.GetPrediction(minion)).Where(predictQ => predictQ.HitChance != HitChance.Collision))
            {
                Q.Cast(predictQ.CastPosition);
                return;
            }
        }

        public override void PermaActive()
        {
            var harass = Features.Find(f => f.NameFeature == "Harass");

            if (harass.IsChecked("harass.auto"))
            {
                OnHarass();
            }
        }

        public override void OnAfterAttack(AttackableUnit t, EventArgs args)
        {
            base.OnAfterAttack(t, args);

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (Player.HasBuff("QuinnR") || !Q.IsReady() || target == null || !target.IsValidTarget() || !castQ || !Features.Find(f => f.NameFeature == "ComboMisc").IsChecked("combo.misc.q")) return;

            var qPredict = Q.GetPrediction(target);

            if (qPredict.HitChancePercent >= 85)
            {
                if (Q.Cast(qPredict.CastPosition))
                    castQ = false;
            }
        }
    }
}
