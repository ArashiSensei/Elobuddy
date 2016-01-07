using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using LevelZero.Controller;
using LevelZero.Model;
using LevelZero.Model.Values;
using LevelZero.Util;
using SharpDX;
using Circle = EloBuddy.SDK.Rendering.Circle;

namespace LevelZero.Core.Champions
{
    class Darius : PluginModel
    {
        #region Scope Variables

        public bool castW = false;

        #endregion

        public override void Init()
        {
            InitVariables();
        }

        public override void InitVariables()
        {
            Spells = new List<Spell.SpellBase>
            {
                new Spell.Skillshot(SpellSlot.Q, 0, SkillShotType.Circular, 750, int.MaxValue, 425) { AllowedCollisionCount = int.MaxValue },
                new Spell.Active(SpellSlot.W),
                new Spell.Skillshot(SpellSlot.E, 550, SkillShotType.Cone, 250, 1100, 300) {AllowedCollisionCount = int.MaxValue},
                new Spell.Targeted(SpellSlot.R, 460)
        };
            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Spells[0], new float[]{ 0, 20 , 35 , 50 , 65 , 80 }, new [] { 0, 0.5f, 0.55f, 0.60f, 0.65f, 0.7f }, DamageType.Physical),
                new SpellDamage(Spells[3], new float[]{ 0, 100 , 200, 300 }, new [] { 0, 0.75f, 0.75f, 0.75f }, DamageType.True)
            };

            InitMenu();
            DamageIndicator.Initialize(DamageUtil.GetComboDamage);
            new SkinController(5);
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
                    new ValueCheckbox(false, "draw.qMin", "Draw Q Minimum Range"),
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
                    new ValueSlider(100, 0 , 50, "harass.mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "harass.q", "Harass Q"),
                    new ValueCheckbox(true,  "harass.w", "Harass W"),
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
                    new ValueSlider(100, 0 , 50, "laneclear.mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "laneclear.q", "Lane Clear Q"),
                    new ValueCheckbox(true,  "laneclear.w", "Lane Clear W")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Jungle Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueSlider(100, 0 , 50, "jungleclear.mana", "Minimum mana %"),
                    new ValueCheckbox(true,  "jungleclear.q", "Jungle Clear Q"),
                    new ValueCheckbox(true, "jungleclear.w", "Jungle Clear W")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Misc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueKeybind(false, "misc.tapToUlt", "Tap to Cast Utl"),
                    new ValueCheckbox(true,  "misc.antiGapE", "E on gap closers"),
                    new ValueSlider(100, 0 , 40, "misc.antiGapQ.antigap", "Anti-Gap Minimum HP % to cast Q")
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
                Circle.Draw(Spells[0].IsReady() ? Color.Blue : Color.Red, 425, Player.Instance.Position);

            if (draw.IsChecked("draw.qMin"))
                Circle.Draw(Spells[0].IsReady() ? Color.Blue : Color.Red, 205, Player.Instance.Position);

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
            var target = TargetSelector.GetTarget(Spells[2].Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget()) return;

            var combo = Features.Find(f => f.NameFeature == "Combo");

            if (Spells[0].IsReady() && combo.IsChecked("combo.q") && target.GetBuffCount("dariushemo") < 4)
            {
                Orbwalker.OrbwalkTo(orbwalkerGoodPos(target));
            }

            if (Spells[0].IsReady() && combo.IsChecked("combo.q") && ((!Spells[1].IsReady() && !Spells[2].IsReady()) || IsInQRange(target) || Player.Instance.CountEnemiesInRange(425) >= 2))
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.Q, target.Position);
            }

            if (Spells[1].IsReady() && combo.IsChecked("combo.w") && Player.Instance.IsInAutoAttackRange(target))
            {
                if (target.IsFacing(Player.Instance))
                {
                    castW = true;
                }
                else
                {
                    Spells[1].Cast();
                    castW = false;
                }

            }

            if (Spells[3].IsReady() && combo.IsChecked("combo.r") && Spells[3].IsInRange(target) && DamageUtil.Killable(target, SpellSlot.R, -(target.GetBuffCount("dariushemo") * (20 * Spells[3].Level))))
            {
                Spells[3].Cast(target);
            }

            if (Spells[2].IsReady() && combo.IsChecked("combo.e") && Spells[2].IsInRange(target) && target.Distance(Player.Instance) > Player.Instance.GetAutoAttackRange())
            {
                var predictionE = ((Spell.Skillshot)Spells[2]).GetPrediction(target);

                if (predictionE.HitChance >= HitChance.Medium)
                {
                    Spells[2].Cast(predictionE.CastPosition);
                }
            }
        }

        public override void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            base.OnAfterAttack(target, args);

            if (castW)
            {
                Spells[1].Cast();
                castW = false;
            }
        }

        public override void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "DariusNoxianTaticsONH")
            {
                Orbwalker.ResetAutoAttack();
            }
        }

        public override void OnHarass()
        {
            var target = TargetSelector.GetTarget(Spells[2].Range, DamageType.Physical);

            var harass = Features.Find(f => f.NameFeature == "Harass");

            if (target == null || !target.IsValidTarget(1000) || harass.SliderValue("harass.mana") > Player.Instance.ManaPercent) return;

            if (Spells[0].IsReady() && harass.IsChecked("harass.q") && target.GetBuffCount("dariushemo") < 4)
            {
                Orbwalker.OrbwalkTo(orbwalkerGoodPos(target));
            }
            else if (Player.Instance.Distance(target.Position) < 300)
            {
                var predictionMove = Prediction.Position.PredictUnitPosition(target, 250);
                Orbwalker.OrbwalkTo(predictionMove.To3D());
            }

            if (Spells[0].IsReady() && harass.IsChecked("harass.q") && IsInQRange(target))
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.Q, target.Position);
            }

            if (Spells[1].IsReady() && harass.IsChecked("harass.w") && Player.Instance.IsInAutoAttackRange(target))
            {
                if (target.IsFacing(Player.Instance))
                {
                    castW = true;
                }
                else
                {
                    Spells[1].Cast();
                    castW = false;
                }

            }

            if (Spells[2].IsReady() && harass.IsChecked("harass.e") && Spells[2].IsInRange(target) && target.Distance(Player.Instance) > Player.Instance.GetAutoAttackRange())
            {
                var predictionE = ((Spell.Skillshot)Spells[2]).GetPrediction(target);

                if (predictionE.HitChance >= HitChance.High)
                {
                    Spells[2].Cast(predictionE.CastPosition);
                }
            }
        }

        public override void OnLaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsValidTarget(425));

            var laneclear = Features.Find(f => f.NameFeature == "Lane Clear");

            if (minions == null || !minions.Any(t => t.IsValidTarget(425)) || laneclear.SliderValue("laneclear.mana") > Player.Instance.ManaPercent) return;

            if (Spells[0].IsReady() && laneclear.IsChecked("laneclear.q") && minions.Count(m => m.Distance(Player.Instance) < 425) >= 3)
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.Q, minions.FirstOrDefault().Position);
            }

            if (Spells[1].IsReady() && laneclear.IsChecked("laneclear.w") && Player.Instance.IsInAutoAttackRange(minions.FirstOrDefault()))
            {
                if (minions.FirstOrDefault().IsFacing(Player.Instance))
                {
                    castW = true;
                }
                else
                {
                    Spells[1].Cast();
                    castW = false;
                }

            }

        }

        public override void OnJungleClear()
        {
            var minions = EntityManager.MinionsAndMonsters.Monsters.Where(t => t.IsValidTarget(425));

            var jungleclear = Features.Find(f => f.NameFeature == "Jungle Clear");

            if (minions == null || !minions.Any(t => t.IsValidTarget(425)) || jungleclear.SliderValue("jungleclear.mana") > Player.Instance.ManaPercent) return;

            if (Spells[0].IsReady() && jungleclear.IsChecked("jungleclear.q") && minions.Count(m => m.Distance(Player.Instance) < 425) >= 1)
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.Q, minions.FirstOrDefault().Position);
            }

            if (Spells[1].IsReady() && jungleclear.IsChecked("jungleclear.w") && Player.Instance.IsInAutoAttackRange(minions.FirstOrDefault()))
            {
                castW = true;
            }

        }

        public override void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            base.OnGapCloser(sender, e);

            var misc = Features.Find(f => f.NameFeature == "Misc");

            if (!misc.IsChecked("misc.antiGapQ")) return;

            if (e.End.Distance(Player.Instance) < 50 && Player.Instance.HealthPercent <= misc.SliderValue("misc.antiGapE.antigap"))
            {
                Spells[0].Cast(Player.Instance.Position);
            }
        }

        public bool IsInQRange(Obj_AI_Base target)
        {
            return target.Distance(Player.Instance) > 205 && target.Distance(Player.Instance) < 425;
        }

        public Vector3 orbwalkerGoodPos(Obj_AI_Base target)
        {
            var aRc = new Util.Circle(Player.Instance.ServerPosition.To2D(), 350).ToClipperPath();
            var cursorPos = Game.CursorPos;
            var targetPosition = target.ServerPosition;
            var pList = new List<Vector3>();
            var additionalDistance = (0.106 + Game.Ping / 2000f) * target.MoveSpeed;
            foreach (var v3 in aRc.Select(p => new Vector2(p.X, p.Y).To3D()))
            {
                if (target.IsFacing(Player.Instance))
                {
                    if (v3.Distance(targetPosition) < 350) pList.Add(v3);
                }
                else
                {
                    if (v3.Distance(targetPosition) < 350 - additionalDistance) pList.Add(v3);
                }
            }
            return pList.Count > 1 ? pList.OrderByDescending(el => el.Distance(cursorPos)).FirstOrDefault() : Vector3.Zero;
        }
    }
}
