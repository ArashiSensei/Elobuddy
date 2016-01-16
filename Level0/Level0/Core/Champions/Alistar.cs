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
using Color = System.Drawing.Color;
using Activator = LevelZero.Controller.Activator;

namespace LevelZero.Core.Champions
{
    class Alistar : PluginModel
    {
        const int QWMANA = 270;
        bool Insecing;
        bool Combing;
        Spell.Skillshot Flash;
        Vector3 WalkPos;
        List<string> DodgeSpells = new List<string>() { "LuxMaliceCannon", "LuxMaliceCannonMis", "EzrealtrueShotBarrage", "KatarinaR", "YasuoDashWrapper", "ViR", "NamiR", "ThreshQ", "AbsoluteZero", "xerathrmissilewrapper", "yasuoq3w", "UFSlash" };

        public override void Init()
        {
            InitVariables();
        }

        public override void InitVariables()
        {
            Activator = new Activator(DamageType.Magical);

            Spells = new List<Spell.SpellBase>
            {
                new Spell.Active(SpellSlot.Q, 365),
                new Spell.Targeted(SpellSlot.W, 650),
                new Spell.Active(SpellSlot.E, 575),
                new Spell.Active(SpellSlot.R)
            };

            DamageUtil.SpellsDamage = new List<SpellDamage>
            {
                new SpellDamage(Spells[0], new float[]{ 0, 60 , 105 , 150 , 195 , 240 }, new [] { 0, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f }, DamageType.Magical),
                new SpellDamage(Spells[1], new float[]{ 0, 55 , 110 , 165 , 220 , 275 }, new [] { 0, 0.7f, 0.7f, 0.7f, 0.7f, 0.7f }, DamageType.Magical)
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
                    new ValueCheckbox(true,  "combo.r", "Combo R"),
                    new ValueSlider(100, 1, 30, "combo.r.health%", "Health% to ult"),
                    new ValueSlider(5, 1, 2, "combo.r.minenemies", "Min Enemies to ult")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Misc",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueKeybind(false, "misc.insec", "Insec", KeyBind.BindTypes.HoldActive, 'J'),
                    new ValueSlider(200, -200, 0, "misc.W/Q Delay", "W/Q Delay", true),
                    new ValueCheckbox(true,  "misc.heal", "Use E", true),
                    new ValueCheckbox(true,  "misc.heal.myself", "Heal myself"),
                    new ValueSlider(99, 1, 50, "misc.heal.health%", "Heal when ally health% <=", true),
                    new ValueSlider(99, 1, 30, "misc.heal.mana%", "Heal when mana% >="),
                    new ValueCheckbox(true, "misc.gapcloser", "W/Q on enemy gapcloser", true),
                    new ValueCheckbox(true, "misc.interrupter", "Interrupt enemy spells"),
                    new ValueKeybind(false, "misc.hu3HU3hu3", "hu3HU3hu3", KeyBind.BindTypes.HoldActive, 'U', true),
                    new ValueSlider(4, 1, 3, "misc.hu3HU3hu3.mode", "hu3HU3hu3 mode, 1:joke, 2:taunt, 3:dance, 4:laugh")
                }
            };

            feature.ToMenu();
            Features.Add(feature);
        }

        public override void OnUpdate(EventArgs args)
        {
            base.OnUpdate(args);

            var misc = Features.First(it => it.NameFeature == "Misc");

            //hu3HU3hu3

            if (misc.IsChecked("misc.hu3HU3hu3"))
            {
                switch (misc.SliderValue("misc.hu3HU3hu3.mode"))
                {
                    case 1:
                        Player.DoEmote(Emote.Joke);
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        break;

                    case 2:
                        Player.DoEmote(Emote.Taunt);
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        break;

                    case 3:
                        Player.DoEmote(Emote.Dance);
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        break;

                    case 4:
                        Player.DoEmote(Emote.Laugh);
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        break;

                    default:
                        break;
                }
            }
            
            //Insec

            if (misc.IsChecked("misc.insec"))
            {
                var Target = TargetSelector.GetTarget(1000, DamageType.Magical);

                var flashslot = Player.Instance.GetSpellSlotFromName("summonerflash");

                if (flashslot != SpellSlot.Unknown)
                {
                    Flash = new Spell.Skillshot(flashslot, 425, SkillShotType.Linear);
                }

                if (!Insecing && !Target.HasBuffOfType(BuffType.SpellImmunity) && !Target.HasBuffOfType(BuffType.Invulnerability))
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Target);

                    if (Spells[1].IsReady())
                    {
                        if (Spells[0].IsReady() && (Target.IsValidTarget(Spells[0].Range - 130) || (Target.IsValidTarget(Spells[0].Range - 50) && !CanMove(Target))) && Player.Instance.Mana >= (Player.Instance.Spellbook.GetSpell(SpellSlot.W).SData.ManaCostArray[Spells[1].Level - 1] + Player.Instance.Spellbook.GetSpell(SpellSlot.Q).SData.ManaCostArray[Spells[0].Level - 1]))
                        {
                            Insecing = true;
                            QWInsec(Target);
                        }
                        else if (Flash != null)
                        {
                            var WalkPos = Game.CursorPos.Extend(Target, Game.CursorPos.Distance(Target) + 100);

                            if ((Player.Instance.Distance(WalkPos) <= Flash.Range - 80 || (Target.IsValidTarget(Flash.Range - 50) && !CanMove(Target))) && Flash.IsReady() && Player.Instance.Mana >= Player.Instance.Spellbook.GetSpell(SpellSlot.W).SData.ManaCostArray[Spells[1].Level - 1])
                            {
                                Insecing = true;

                                if (Flash.Cast(WalkPos.To3D())) Spells[1].Cast(Target);

                                Insecing = false;
                            }

                            else if ((Target.IsValidTarget(Flash.Range + Spells[0].Range - 130) || (Target.IsValidTarget(Flash.Range + Spells[0].Range - 50) && !CanMove(Target))) && Flash.IsReady() && Spells[0].IsReady() && Player.Instance.Mana >= (Player.Instance.Spellbook.GetSpell(SpellSlot.W).SData.ManaCostArray[Spells[1].Level - 1] + Player.Instance.Spellbook.GetSpell(SpellSlot.Q).SData.ManaCostArray[Spells[0].Level - 1]))
                            {
                                Insecing = true;
                                QWInsec(Target, true);
                            }
                        }
                    }
                }
            }

            //Heal

            if (Spells[2].IsReady() && misc.IsChecked("misc.heal") && Player.Instance.ManaPercent >= misc.SliderValue("misc.heal.mana%") && EntityManager.Heroes.Allies.Any(it => it.HealthPercent <= misc.SliderValue("misc.heal.health%") && Spells[2].IsInRange(it)))
            {
                if (!misc.IsChecked("misc.heal.myself") && Player.Instance.HealthPercent <= misc.SliderValue("misc.heal.health%")) { }
                else Spells[2].Cast();
            }

            return;
        }

        public override void OnDraw(EventArgs args)
        {
            var draw = Features.Find(f => f.NameFeature == "Draw");

            if (draw.IsChecked("disable") || Player.Instance.IsDead)
            {
                DamageIndicator.Enabled = false;
                return;
            }

            var Target = TargetSelector.GetTarget(1000, DamageType.Magical);
            var insec = Features.First(it => it.NameFeature == "Misc").IsChecked("misc.insec");
            int qwmana = new[] { 0, 65, 70, 75, 80, 85 }[Spells[0].Level] + new[] { 0, 65, 70, 75, 80, 85 }[Spells[1].Level];

            if (Target != null && Spells[1].IsReady())
            {
                if (Spells[0].IsReady() && (Target.IsValidTarget(600)) && Player.Instance.Mana >= qwmana)
                {
                    Drawing.DrawText(Target.Position.WorldToScreen().X - 30, Target.Position.WorldToScreen().Y - 180, Color.Yellow, "W/Q is possible !!");
                }

                if (Spells[0].IsReady() && (Target.IsValidTarget(Spells[0].Range - 130) || (Target.IsValidTarget(Spells[0].Range - 50) && !CanMove(Target)) && Player.Instance.Mana >= qwmana))
                {
                    Drawing.DrawText(Target.Position.WorldToScreen().X - 30, Target.Position.WorldToScreen().Y - 150, Color.Yellow, "Q/W Insec !!");
                    Drawing.DrawLine(Target.Position.WorldToScreen(), Game.CursorPos2D, 3, Color.Yellow);
                    Drawing.DrawCircle(WalkPos, 70, Color.BlueViolet);
                }
                else if (Flash != null)
                {
                    if (Flash.IsReady() && Player.Instance.Distance(WalkPos) <= Flash.Range - 100 && Player.Instance.Mana >= Player.Instance.Spellbook.GetSpell(SpellSlot.W).SData.ManaCostArray[Spells[1].Level - 1])
                    {
                        Drawing.DrawText(Target.Position.WorldToScreen().X - 30, Target.Position.WorldToScreen().Y - 150, Color.Yellow, "Flash/W Insec !!");
                        Drawing.DrawLine(Target.Position.WorldToScreen(), Game.CursorPos2D, 3, Color.Yellow);
                        Drawing.DrawCircle(WalkPos, 70, Color.BlueViolet);
                    }

                    else if (Flash.IsReady() && Spells[0].IsReady() && Target.IsValidTarget(Flash.Range + Spells[0].Range - 40) && Player.Instance.Mana >= qwmana)
                    {
                        Drawing.DrawText(Target.Position.WorldToScreen().X - 30, Target.Position.WorldToScreen().Y - 150, Color.Yellow, "Flash/Q/W Insec !!");
                        Drawing.DrawLine(Target.Position.WorldToScreen(), Game.CursorPos2D, 3, Color.Yellow);
                        Drawing.DrawCircle(Player.Instance.Position.Extend(Target, Flash.Range).To3D(), 70, Color.Yellow);
                        Drawing.DrawCircle(WalkPos, 70, Color.BlueViolet);
                    }
                }
            }

            if (draw.IsChecked("draw.q"))
                Circle.Draw(Spells[0].IsReady() ? SharpDX.Color.Blue : SharpDX.Color.Red, Spells[0].Range, Player.Instance.Position);

            if (draw.IsChecked("draw.w"))
                Circle.Draw(Spells[1].IsReady() ? SharpDX.Color.Blue : SharpDX.Color.Red, Spells[1].Range, Player.Instance.Position);

            if (draw.IsChecked("draw.e"))
                Circle.Draw(Spells[2].IsReady() ? SharpDX.Color.Blue : SharpDX.Color.Red, Spells[2].Range, Player.Instance.Position);

            DamageIndicator.Enabled = draw.IsChecked("dmgIndicator");

        }

        public override void OnCombo()
        {
            if (!Combing)
            {
                var Target = TargetSelector.GetTarget(1000, DamageType.Magical);

                if (Spells[0].IsReady() && Target.IsValidTarget(Spells[0].Range - 80) && !Player.Instance.IsDashing()) Spells[0].Cast();

                else if (Spells[0].IsReady() && Spells[1].IsReady() && Target.IsValidTarget(625) && Player.Instance.Mana >= QWMANA) { WQ(Target); Combing = true; }

                var combo = Features.First(it => it.NameFeature == "Combo");

                if (Spells[3].IsReady() && combo.IsChecked("combo.r") && Player.Instance.CountEnemiesInRange(600) >= combo.SliderValue("combo.r.minenemies") && Player.Instance.HealthPercent <= combo.SliderValue("combo.r.health%")) Spells[3].Cast();
            }

            return;
        }

        public override void OnHarass()
        {
            if (!Combing)
            {
                var Target = TargetSelector.GetTarget(1000, DamageType.Magical);

                if (Spells[0].IsReady() && Target.IsValidTarget(Spells[0].Range - 80) && !Player.Instance.IsDashing()) Spells[0].Cast();

                else if (Spells[0].IsReady() && Spells[1].IsReady() && Target.IsValidTarget(625) && Player.Instance.Mana >= (Player.Instance.Spellbook.GetSpell(SpellSlot.W).SData.ManaCostArray[Spells[1].Level - 1] + Player.Instance.Spellbook.GetSpell(SpellSlot.Q).SData.ManaCostArray[Spells[0].Level - 1])) { WQ(Target); Combing = true; }
            }

            return;
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            base.OnPossibleToInterrupt(sender, args);

            var interrupt = Features.First(it => it.NameFeature == "Misc").IsChecked("misc.interrupter");

            if (sender.IsEnemy && interrupt && args.DangerLevel == DangerLevel.High)
            {
                if (Spells[1].IsReady() && sender.IsValidTarget(300)) Spells[1].Cast(sender);
                else if (Spells[0].IsReady() && sender.IsValidTarget(Spells[0].Range)) Spells[0].Cast();
            }

            return;
        }

        public override void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            base.OnProcessSpell(sender, args);

            var interrupt = Features.First(it => it.NameFeature == "Misc").IsChecked("misc.interrupter");

            if (sender.IsEnemy && interrupt && DodgeSpells.Any(it => it == args.SData.Name))
            {
                if (args.SData.Name == "KatarinaR")
                {
                    if (Spells[0].IsReady() && Spells[0].IsInRange(sender)) Spells[0].Cast();
                    else if (Spells[1].IsReady() && Spells[1].IsInRange(sender)) Spells[1].Cast(sender);
                    return;
                }

                if (args.SData.Name == "AbsoluteZero")
                {
                    if (Spells[0].IsReady() && Spells[0].IsInRange(sender)) Spells[0].Cast();
                    else if (Spells[1].IsReady() && Spells[1].IsInRange(sender)) Spells[1].Cast(sender);
                    return;
                }

                if (args.SData.Name == "EzrealtrueShotBarrage")
                {
                    if (Spells[0].IsReady() && Spells[0].IsInRange(sender)) Spells[0].Cast();
                    else if (Spells[1].IsReady() && Spells[1].IsInRange(sender)) Spells[1].Cast(sender);
                    return;
                }

                if (Spells[0].IsReady() && Spells[0].IsInRange(sender)) { Spells[0].Cast(); return; }
                if (Spells[1].IsReady() && sender.Distance(Player.Instance) <= 300) { Spells[1].Cast(sender); return; }
            }

            return;
        }

        public override void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            base.OnGapCloser(sender, e);

            var gapclose = Features.First(it => it.NameFeature == "Misc").IsChecked("misc.gapcloser");

            if (sender.IsEnemy && gapclose)
            {
                if (sender.IsValidTarget(Spells[0].Range)) Spells[0].Cast();
                else if (sender.IsValidTarget(Spells[1].Range)) Spells[1].Cast(sender);
            }

            return;
        }

        //------------------------------------|| Extension ||--------------------------------------

        private void WQ(Obj_AI_Base target)
        {
            if (target != null && target.IsValidTarget())
            {
                Combing = true;
                int ADelay = Features.First(it => it.NameFeature == "Misc").SliderValue("misc.W/Q Delay");
                int delay = (int)((150 * (Player.Instance.Distance(target))) / 650 + ADelay);

                if (Player.CastSpell(SpellSlot.W, target))
                {
                    EloBuddy.SDK.Core.DelayAction(() => Spells[0].Cast(), delay);
                    EloBuddy.SDK.Core.DelayAction(() => Combing = false, delay + 1000);
                }
                else Combing = false;
            }

            return;
        }

        private void CheckWDistance(Obj_AI_Base target)
        {
            if (Player.Instance.Distance(WalkPos) <= 70) Spells[1].Cast(target);
            else Insecing = false;

            return;
        }

        private void QWInsec(Obj_AI_Base target, bool flash = false)
        {
            if (flash)
            {
                var FlashPos = Player.Instance.Position.Extend(target, Flash.Range).To3D();

                var Flashed = Flash.Cast(FlashPos);

                if (Flashed)
                {
                    EloBuddy.SDK.Core.DelayAction(delegate
                    {
                        if (Spells[0].Cast())
                        {
                            WalkPos = Game.CursorPos.Extend(target, Game.CursorPos.Distance(target) + 150).To3D();

                            int delay = (int)(Player.Instance.Distance(WalkPos) / Player.Instance.MoveSpeed * 1000) + 300 + Spells[0].CastDelay + 2 * Game.Ping;

                            Player.IssueOrder(GameObjectOrder.MoveTo, WalkPos);

                            EloBuddy.SDK.Core.DelayAction(() => CheckWDistance(target), delay);
                            EloBuddy.SDK.Core.DelayAction(() => Insecing = false, delay + 1000);
                        }
                        else Insecing = false;
                    }, Game.Ping + 70);
                }
                else Insecing = false;

                return;
            }

            else
            {
                if (Spells[0].Cast())
                {
                    WalkPos = Game.CursorPos.Extend(target, Game.CursorPos.Distance(target) + 150).To3D();

                    int delay = (int)(Player.Instance.Distance(WalkPos) / Player.Instance.MoveSpeed * 1000) + 300 + Spells[0].CastDelay + 2 * Game.Ping;

                    Player.IssueOrder(GameObjectOrder.MoveTo, WalkPos);
                    EloBuddy.SDK.Core.DelayAction(() => CheckWDistance(target), delay);
                    EloBuddy.SDK.Core.DelayAction(() => Insecing = false, delay + 1000);
                }
                else Insecing = false;

                return;
            }
        }

        private bool CanMove(Obj_AI_Base target)
        {
            if (target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Sleep) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt)) return false;

            return true;
        }
    }
}