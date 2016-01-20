using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using LevelZero.Controller;
using LevelZero.Model;
using LevelZero.Model.Values;
using LevelZero.Util;
using SharpDX;
using Circle = EloBuddy.SDK.Rendering.Circle;
using Activator = LevelZero.Controller.Activator;

namespace LevelZero.Core.Champions
{
    class Jhin : PluginModel
    {
        readonly AIHeroClient Player = EloBuddy.Player.Instance;

        readonly PredictionUtil _predictionutil = new PredictionUtil();

        Spell.Targeted Q { get { return (Spell.Targeted)Spells[0]; } }
        Spell.Skillshot W { get { return (Spell.Skillshot)Spells[1]; } }
        Spell.Skillshot E { get { return (Spell.Skillshot)Spells[2]; } }
        Spell.Skillshot R { get { return (Spell.Skillshot)Spells[3]; } }

        private bool _canCastQ = false;
        private bool _isUltimateOn = false;

        public override void Init()
        {
            InitVariables();
            InitEvents();
        }

        public override void InitVariables()
        {
            Activator = new Activator();

            Spells = new List<Spell.SpellBase>
            {
                new Spell.Targeted(SpellSlot.Q,  700),
                new Spell.Skillshot(SpellSlot.W, 2000, SkillShotType.Linear),
                new Spell.Skillshot(SpellSlot.E,  600, SkillShotType.Circular),
                new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear)
            };

            W.AllowedCollisionCount = int.MaxValue;
            E.AllowedCollisionCount = int.MaxValue;
            R.AllowedCollisionCount = -1;

            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Q, new float[] { 0, 70, 110, 150, 190, 230 }, new [] { 0, 1f, 1f, 1f, 1f, 1f }, DamageType.Physical),
                new SpellDamage(Q, new float[] { 0, 0, 0, 0, 0, 0 }, new [] { 0, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f }, DamageType.Magical),
                new SpellDamage(W, new float[] { 0, 40, 75, 110, 145, 180 }, new [] { 0, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f }, DamageType.Magical),
                new SpellDamage(E, new float[] { 50, 75, 100, 125, 150 }, new [] { 0, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f }, DamageType.Physical)

            };

            InitMenu();

            DamageIndicator.Initialize(DamageUtil.GetComboDamage);

            new SkinController(2);
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
                    new ValueCheckbox(true, "draw.r", "Draw R"),
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
                    new ValueCheckbox(true,  "combo.r", "Combo R"),
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
                    new ValueCheckbox(true,  "harass.e", "Harass E"),
                    new ValueCheckbox(true,  "harass.r", "Harass R")
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
                    new ValueCheckbox(true,  "laneclear.e", "Lane Clear E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Last Hit",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "lasthit.q", "Last Hit Q")
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
                    new ValueCheckbox(true,  "jungleclear.e", "Jungle Clear E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Misc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "misc.ks", "KS")
                    //new ValueCheckbox(true,  "misc.gapcloser", "Auto E on enemy gapcloser")
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
                Circle.Draw(Q.IsReady() ? Color.Blue : Color.Red, Q.Range, Player.Position);

            if (draw.IsChecked("draw.w"))
                Circle.Draw(W.IsReady() ? Color.Blue : Color.Red, W.Range, Player.Position);

            if (draw.IsChecked("draw.e"))
                Circle.Draw(E.IsReady() ? Color.Blue : Color.Red, E.Range, Player.Position);

            if (draw.IsChecked("draw.r"))
                Circle.Draw(R.IsReady() ? Color.Blue : Color.Red, R.Range, Player.Position);

            DamageIndicator.Enabled = draw.IsChecked("dmgIndicator");

        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (_canCastQ && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)))
            {
                Q.Cast((Obj_AI_Base)target);
                _canCastQ = false;
            }
        }

        public override void OnCombo()
        {
            var TargetW = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var TargetR = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (Target == null && TargetR == null && TargetW == null) return;

            var mode = Features.First(it => it.NameFeature == "Combo");

            if (TargetR != null && TargetR.IsValidTarget(R.Range) && mode.IsChecked("combo.r") && R.IsReady() && Player.Distance(TargetR) > 1000)
            {
                var predictionR = R.GetPrediction(TargetR);

                if (predictionR.HitChance >= HitChance.High)
                {
                    Orbwalker.MoveTo(TargetR.Position);
                    R.Cast(predictionR.CastPosition);
                    if (!_isUltimateOn)
                    {
                        Orbwalker.DisableAttacking = true;
                        Orbwalker.DisableMovement = true;
                        _isUltimateOn = true;
                        EloBuddy.SDK.Core.DelayAction(() => _isUltimateOn = false, 9000);
                    }
                }
            }

            if (_isUltimateOn) return;

            if (Target != null && Target.IsValidTarget(Q.Range) && Q.IsReady() && mode.IsChecked("combo.q"))
            {
                _canCastQ = true;
            }

            if (TargetW != null && TargetW.IsValidTarget(W.Range) && W.IsReady() && mode.IsChecked("combo.w"))
            {
                if (Player.IsInAutoAttackRange(TargetW))
                {
                    if (TargetW.Buffs.Any(b => b.Name.ToLower().Contains("jhin")))
                    {
                        var predictionW = W.GetPrediction(TargetW);

                        if (predictionW.HitChancePercent >= 85)
                        {
                            W.Cast(predictionW.CastPosition);
                        }
                    }
                }
                else
                {
                    var predictionW = W.GetPrediction(TargetW);

                    if (predictionW.HitChance == HitChance.Immobile || predictionW.HitChancePercent >= 80)
                    {
                        W.Cast(predictionW.CastPosition);
                    }
                }
            }

            if (Target != null && Target.IsValidTarget(E.Range) && E.IsReady() && mode.IsChecked("combo.E"))
            {
                /*
                Target.HasBuffOfType(BuffType.Knockup) || Target.HasBuffOfType(BuffType.Snare) || Target.HasBuffOfType(BuffType.Stun) || Target.HasBuffOfType(BuffType.Charm)
                */

                if (!CanMove(Target))
                {
                    var pred = E.GetPrediction(Target);
                    E.Cast(pred.CastPosition);
                }
                else
                {
                    var predictionE = E.GetPrediction(Target);

                    if (predictionE.HitChance == HitChance.Immobile || predictionE.HitChancePercent >= 80)
                    {
                        E.Cast(predictionE.CastPosition);
                    }
                }
            }
        }

        public override void OnHarass()
        {
            var TargetW = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (Target == null && TargetW == null) return;

            var mode = Features.First(it => it.NameFeature == "Harass");

            if (Target != null && Target.IsValidTarget(Q.Range) && mode.IsChecked("harass.q") && Q.IsReady())
            {
                _canCastQ = true;
            }

            if (TargetW != null && TargetW.IsValidTarget(W.Range) && mode.IsChecked("harass.w") && W.IsReady())
            {
                if (Player.IsInAutoAttackRange(TargetW))
                {
                    if (TargetW.Buffs.Any(b => b.Name.ToLower().Contains("jhin")))
                    {
                        var predictionW = W.GetPrediction(TargetW);

                        if (predictionW.HitChancePercent >= 85)
                        {
                            W.Cast(predictionW.CastPosition);
                        }
                    }
                }
                else
                {
                    var predictionW = W.GetPrediction(TargetW);

                    if (predictionW.HitChance == HitChance.Immobile || predictionW.HitChancePercent >= 80)
                    {
                        W.Cast(predictionW.CastPosition);
                    }
                }
            }

            if (Target != null && Target.IsValidTarget(E.Range) && mode.IsChecked("harass.E") && E.IsReady())
            {
                /*
                Target.HasBuffOfType(BuffType.Knockup) || Target.HasBuffOfType(BuffType.Snare) || Target.HasBuffOfType(BuffType.Stun) || Target.HasBuffOfType(BuffType.Charm)
                */
                if (!CanMove(Target))
                {
                    var pred = E.GetPrediction(Target);
                    E.Cast(pred.CastPosition);
                }
                else
                {
                    var predictionE = E.GetPrediction(Target);

                    if (predictionE.HitChance == HitChance.Immobile || predictionE.HitChancePercent >= 80)
                    {
                        E.Cast(predictionE.CastPosition);
                    }
                }
            }

        }

        public override void OnLaneClear()
        {
            var mode = Features.First(it => it.NameFeature == "Lane Clear");

            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => Q.IsInRange(m));

            if (!minions.Any()) return;

            if (Q.IsReady() && mode.IsChecked("laneclear.q"))
            {
                Q.Cast(minions.FirstOrDefault());
            }

            if (W.IsReady() && mode.IsChecked("laneclear.e"))
            {
                var minionsW = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => W.IsInRange(m)).Select(m => m.ServerPosition.To2D()).ToList();

                if (!minionsW.Any()) return;

                var predictinClear = _predictionutil.GetBestLineFarmLocation(minionsW, W.Width, W.Range);

                if (predictinClear.MinionsHit >= 2)
                {
                    W.Cast(predictinClear.Position.To3D());
                }
            }

            if (E.IsReady() && mode.IsChecked("laneclear.e"))
            {
                var minionsE = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => E.IsInRange(m)).Select(m => m.ServerPosition.To2D()).ToList();

                if (!minionsE.Any()) return;

                var predictinClear = _predictionutil.GetBestCircularFarmLocation(minionsE, E.Width, E.Range);

                if (predictinClear.MinionsHit >= 2)
                {
                    W.Cast(predictinClear.Position.To3D());
                }
            }
        }

        public override void OnJungleClear()
        {
            /*
            //---------------------------------------------Smite Usage---------------------------------------------

            if (_smite != null)
            {
                var smiteusage = Features.First(it => it.NameFeature == "Smite Usage");

                if (_smite.IsReady() && smiteusage.IsChecked("smiteusage.usesmite"))
                {
                    _summoners.AutoSmiteMob(_smite, smiteusage);
                }
            }
            */

            var mode = Features.First(it => it.NameFeature == "Jungle Clear");

            var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => Q.IsInRange(m));

            if (!minions.Any()) return;

            if (Q.IsReady() && mode.IsChecked("jungleclear.q"))
            {
                Q.Cast(minions.FirstOrDefault());
            }

            if (W.IsReady() && mode.IsChecked("jungleclear.e"))
            {
                var minionsW = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => W.IsInRange(m)).Select(m => m.ServerPosition.To2D()).ToList();

                if (!minionsW.Any()) return;

                var predictinClear = _predictionutil.GetBestLineFarmLocation(minionsW, W.Width, W.Range);

                if (predictinClear.MinionsHit >= 2)
                {
                    W.Cast(predictinClear.Position.To3D());
                }
            }

            if (E.IsReady() && mode.IsChecked("jungleclear.e"))
            {
                var minionsE = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => E.IsInRange(m)).Select(m => m.ServerPosition.To2D()).ToList();

                if (!minionsE.Any()) return;

                var predictinClear = _predictionutil.GetBestCircularFarmLocation(minionsE, E.Width, E.Range);

                if (predictinClear.MinionsHit >= 2)
                {
                    W.Cast(predictinClear.Position.To3D());
                }
            }

        }

        public override void OnLastHit()
        {
            var mode = Features.First(it => it.NameFeature == "Last Hit");

            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => Q.IsInRange(m) && DamageUtil.Killable(m, SpellSlot.Q));

            if (!minions.Any()) return;

            if (mode.IsChecked("lasthit.q") && Q.IsReady())
            {
                Q.Cast(minions.FirstOrDefault());
            }
        }

        public override void PermaActive()
        {
            if (_isUltimateOn && R.IsReady())
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
            }
            else if(Orbwalker.DisableAttacking || Orbwalker.DisableMovement)
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }

            var misc = Features.First(it => it.NameFeature == "Misc");
            //------------------------------------------------KS------------------------------------------------

            if (misc.IsChecked("misc.ks") && EntityManager.Heroes.Enemies.Any(it => R.IsInRange(it))) KS();
        }

        void KS()
        {
            if (Q.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && DamageUtil.GetSpellDamage(enemy, SpellSlot.Q) >= enemy.Health);
                if (bye != null) { Q.Cast(bye); return; }
            }

            if (W.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(W.Range) && DamageUtil.GetSpellDamage(enemy, SpellSlot.W) >= enemy.Health);
                if (bye != null)
                {
                    var pred = W.GetPrediction(bye);
                    if (pred.HitChancePercent >= 85) W.Cast(pred.CastPosition);
                }
            }

            if (Q.IsReady() && W.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(Q.Range) && DamageUtil.GetSpellDamage(enemy, SpellSlot.Q) + DamageUtil.GetSpellDamage(enemy, SpellSlot.W) >= enemy.Health);
                if (bye != null) { Q.Cast(bye); return; }
            }

            if (R.IsReady())
            {
                var bye = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget(R.Range - 500) && DamageUtil.GetSpellDamage(enemy, SpellSlot.R) >= enemy.Health + 50);
                if (bye != null)
                {
                    var pred = R.GetPrediction(bye);
                    if (pred.HitChancePercent >= 85) R.Cast(pred.CastPosition);
                }
            }
        }

        bool CanMove(Obj_AI_Base target)
        {
            if (target.HasBuffOfType(BuffType.Charm) ||
                target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Sleep) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt)) return false;

            return true;
        }
    }
}