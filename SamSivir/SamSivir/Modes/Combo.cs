using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
// Using the config like this makes your life easier, trust me
using Settings = SamSivir.Config.Modes.Combo;

namespace SamSivir.Modes
{
    public sealed class Combo : ModeBase
    {
        private int LastAATime = 0;
        private int LastAggersiveSpell = 0;
        public Combo()
        {
            Orbwalker.OnPostAttack += OnPostAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }
        public void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && !args.SData.IsAutoAttack() && args.Target != null && args.Target.NetworkId == ObjectManager.Player.NetworkId)
            {
                //Chat.Print("Lel");
                LastAggersiveSpell = Environment.TickCount;
            }
            //if (sender.IsEnemy && !args.SData.IsAutoAttack() && args.Target != null && args.Target.NetworkId == ObjectManager.Player.NetworkId)
            //{
            //    LastAggersiveSpell = Environment.TickCount;
            //}
            if (sender.IsMe && args.SData.Name == W.Name)
            {
                Orbwalker.ResetAutoAttack();
            }
            if (sender.IsMe && args.SData.Name == Q.Name)
            {
                Orbwalker.ResetAutoAttack();
            }
        }
        public void OnPostAttack(AttackableUnit target, EventArgs args)
        {
            LastAATime = Environment.TickCount;
        }
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on combo mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            //Chat.Print("We are in combo");
            if (LastAggersiveSpell + 500 > Environment.TickCount && E.IsReady())
            {
                //Chat.Print("LASTAS");
                E.Cast();
            }
                
            // TODO: Check
            //Chat.Print(ObjectManager.Player.AttackDelay);
            if (LastAATime + ObjectManager.Player.AttackDelay*1000 / 2 > Environment.TickCount)
            {
                //Chat.Print("AfterAA");
                var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange, DamageType.Physical);
                if (target != null && W.IsReady())
                {
                    Orbwalker.ResetAutoAttack();
                    W.Cast();
                    return;
                }
                target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if(target != null && Q.IsReady())
                {
                    Q.Cast(target);
                }
            }
            // TODO: Add combo logic here
            // See how I used the Settings.UseQ here, this is why I love my way of using
            // the menu in the Config class!
            //if (Settings.UseQ && Q.IsReady())
            //{
            //    var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            //    if (target != null)
            //    {
            //        Q.Cast(target);
            //    }
            //}
        }
    }
}