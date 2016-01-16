using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using LevelZero.Model.Interfaces;
using Activator = LevelZero.Controller.Activator;

namespace LevelZero.Model
{
    abstract class PluginModel : IChampion
    {
        public static List<Spell.SpellBase> Spells { get; set; }
        public static List<Feature> Features { get; set; }
        public static Activator Activator;

        protected PluginModel()
        {
            Spells = new List<Spell.SpellBase>();
            Features = new List<Feature>();
            Init();
            InitEvents();
        }

        public Spell.SpellBase findSpell(SpellSlot spellSlot)
        {
            var spell = Spells.Find(s => s.Slot == spellSlot);

            return spell != null ? spell : null;
        }

        public virtual void Init()
        {
        }

        public virtual void InitVariables()
        {
            throw new NotImplementedException();
        }

        public virtual void InitMenu()
        {
            throw new NotImplementedException();
        }

        public virtual void InitEvents()
        {
            Game.OnTick += OnUpdate;
            Obj_AI_Base.OnLevelUp += OnPlayerLevelUp;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Drawing.OnDraw += OnDraw;
            Orbwalker.OnPostAttack += OnAfterAttack;
            Interrupter.OnInterruptableSpell += OnPossibleToInterrupt;
            Gapcloser.OnGapcloser += OnGapCloser;
        }
        
        public virtual void OnPlayerLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (!sender.IsMe) return;
        }

        public virtual void OnCombo()
        {
        }

        public virtual void OnHarass()
        {
        }

        public virtual void OnLaneClear()
        {
        }

        public virtual void OnJungleClear()
        {
        }

        public virtual void OnLastHit()
        {
        }

        public virtual void OnFlee()
        {
        }

        public virtual void PermaActive()
        {
        }

        public virtual void OnUpdate(EventArgs args)
        {
            if (Player.Instance.IsDead) return;

            PermaActive();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) OnCombo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) OnHarass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) OnLaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)) OnLastHit();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) OnFlee();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) OnJungleClear();
        }

        public virtual void OnDraw(EventArgs args)
        {
            if (Player.Instance.IsDead) return;
        }

        public virtual void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (Player.Instance.IsDead) return;
        }

        public virtual void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (Player.Instance.IsDead) return;
        }

        public virtual void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if(sender.IsAlly || Player.Instance.IsDead) return;
        }

        public virtual void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Player.Instance.IsDead) return;
        }

        public virtual void GameObjectOnCreate(GameObject sender, EventArgs args)
        {
        }

        public virtual void GameObjectOnDelete(GameObject sender, EventArgs args)
        {
        }
    }
}
