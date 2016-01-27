using System;
using System.Collections.Generic;
using System.Linq;
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
    class Ezreal : PluginModel
    {
        Spell.Skillshot Q { get { return (Spell.Skillshot)Spells[0]; } }
        Spell.Skillshot W { get { return (Spell.Skillshot)Spells[1]; } }
        Spell.Skillshot E { get { return (Spell.Skillshot)Spells[2]; } }
        Spell.Skillshot R { get { return (Spell.Skillshot)Spells[3]; } }

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
                new Spell.Skillshot(SpellSlot.Q, 0x47E, SkillShotType.Linear, 0xFA, 0x7D0, 65),
                new Spell.Skillshot(SpellSlot.W, 0x3E8, SkillShotType.Linear, 0, 0x60E, 80) { AllowedCollisionCount = int.MaxValue },
                new Spell.Skillshot(SpellSlot.E, 0x2EE, SkillShotType.Circular),
                new Spell.Skillshot(SpellSlot.R, 5000, SkillShotType.Linear, 0x1F4, 0x7D0, 160) { AllowedCollisionCount = int.MaxValue }
            };
            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Q, new float[]{ 0, 0x23, 0x37, 0x4B, 0x4F, 0x73 }, new [] { 0, 1.1f, 1.1f, 1.1f, 1.1f, 1.1f }, DamageType.Physical),
                new SpellDamage(W, new float[]{ 0, 0x46, 0x73, 0xA0, 0xCD, 0xFA }, new [] { 0, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f }, DamageType.Magical),
                new SpellDamage(E, new float[]{ 0, 0x4B, 0x7D, 0xAF, 0xE1, 0x113 }, new [] { 0, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f }, DamageType.Physical),
                new SpellDamage(R, new float[]{ 0, 350, 0x1F4, 0x28A}, new [] { 0, 1f, 1f, 1f }, DamageType.Physical)
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
                    new ValueCheckbox(true, "draw.q", "Draw Q"),
                    new ValueCheckbox(true, "draw.w", "Draw W"),
                    new ValueCheckbox(true, "draw.e", "Draw E"),
                    new ValueCheckbox(true, "draw.r", "Draw R")
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
                    new ValueCheckbox(true,  "combo.r", "Combo R")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "ComboMisc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(false,  "combo.misc.q", "Only Q After AA"),
                    new ValueSlider(5000, 0 , 2500, "combo.misc.minDistance", "Min Distance for Cast R")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Harass",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 50, "mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "harass.q", "Harass Q"),
                    new ValueCheckbox(true,  "harass.w", "Harass W")
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
                Circle.Draw(Spells[3].IsReady() ? Color.Blue : Color.Red, Features.Find(f => f.NameFeature == "ComboMisc").SliderValue("combo.misc.minDistance"), Player.Instance.Position);


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

            if (R.IsReady() && combo.IsChecked("combo.r"))
            {
                var targetR = TargetSelector.GetTarget(R.Range, DamageType.Mixed);

                if(targetR != null && targetR.IsValidTarget(R.Range))
                if (comboMisc.SliderValue("combo.misc.minDistance") > Player.Instance.Distance(targetR) && targetR != null && targetR.IsValidTarget() && DamageUtil.Killable(targetR, SpellSlot.R, 89))
                {
                    var predictionR = R.GetPrediction(targetR);

                    if (predictionR.HitChance >= HitChance.High)
                    {
                        R.Cast(predictionR.CastPosition);
                    }
                    else
                    {
                        var castPosition = Prediction.Position.PredictUnitPosition(targetR, (int)Math.Round(PredictionUtil.GetArrivalTime(Player.Instance.Distance(targetR), 0.5f, R.Speed)));
                        R.Cast(castPosition.To3D());
                    }
                }
            }

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);

            if (target == null || !target.IsValidTarget(Q.Range) || !target.IsValidTargetUtil()) return;

            if (combo.IsChecked("combo.q") && Q.IsReady())
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

            if (combo.IsChecked("combo.w") && W.IsReady() && W.IsInRange(target))
            {
                var wPredict = W.GetPrediction(target);

                if (wPredict.HitChancePercent >= 85)
                {
                    W.Cast(wPredict.CastPosition);
                }
            }

            if (combo.IsChecked("combo.e") && E.IsReady() && E.IsInRange(target))
            {
                if (target.CountEnemiesInRange(700) < target.CountAlliesInRange(475) && DamageUtil.Killable(target, SpellSlot.E))
                {
                    E.Cast(target.ServerPosition);
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

            if (harass.IsChecked("harass.w") && W.IsReady())
            {
                var wPredict = W.GetPrediction(target);

                if (wPredict.HitChancePercent >= 85)
                {
                    W.Cast(wPredict.CastPosition);
                }
            }
        }

        public override void OnLaneClear()
        {
            if (!EntityManager.MinionsAndMonsters.EnemyMinions.Any(m => m.Distance(Player.Instance) < Q.Range)) return;

            var targets = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsValidTarget(Q.Range) && t.IsValidTarget(Q.Range));

            var laneclear = Features.Find(f => f.NameFeature == "Lane Clear");

            if (!Q.IsReady() || !laneclear.IsChecked("laneclear.q") || !targets.Any() || laneclear.SliderValue("mana") > Player.Instance.ManaPercent) return;

            var bestTarget = targets.Aggregate((curMax, x) => ((curMax == null && x.IsValid) || x.MaxHealth > curMax.MaxHealth ? x : curMax));

            Q.Cast(Q.GetPrediction(bestTarget).CastPosition);
        }

        public override void OnJungleClear()
        {
            if (!EntityManager.MinionsAndMonsters.Monsters.Any(m => m.Distance(Player.Instance) < Q.Range)) return;

            var targets = EntityManager.MinionsAndMonsters.Monsters.Where(t => t.IsValidTarget(Q.Range) && t.IsValidTarget(Q.Range));

            var jungleclear = Features.Find(f => f.NameFeature == "Jungle Clear");

            if (!Q.IsReady() || !jungleclear.IsChecked("jungleclear.q") || !targets.Any() || jungleclear.SliderValue("mana") > Player.Instance.ManaPercent) return;

            Q.Cast(targets.FirstOrDefault());

        }

        public override void OnLastHit()
        {
            var lastHit = Features.Find(f => f.NameFeature == "Last Hit");

            if (lastHit.SliderValue("mana") > Player.Instance.ManaPercent || !lastHit.IsChecked("lasthit.q") || !Q.IsReady()) return;

            var targets = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t != null && t.IsValidTarget(Q.Range) && Orbwalker.LastTarget != null && t.NetworkId != Orbwalker.LastTarget.NetworkId && DamageUtil.Killable(t, SpellSlot.Q));

            if(!targets.Any()) return;

            foreach (var predictQ in targets.Select(minion => Q.GetPrediction(minion)).Where(predictQ => predictQ.HitChance != HitChance.Collision))
            {
                Q.Cast(predictQ.CastPosition);
                return;
            }
        }

        public override void OnAfterAttack(AttackableUnit t, EventArgs args)
        {
            base.OnAfterAttack(t, args);

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (!Q.IsReady() || target == null || !target.IsValidTarget() || !castQ || !Features.Find(f => f.NameFeature == "ComboMisc").IsChecked("combo.misc.q")) return;

            var qPredict = Q.GetPrediction(target);

            if (qPredict.HitChancePercent >= 85)
            {
                if (Q.Cast(qPredict.CastPosition))
                    castQ = false;
            }
        }
    }
}
