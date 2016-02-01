/*
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using SharpDX;
using Color = System.Drawing.Color;

//https://www.reddit.com/r/leagueoflegends/comments/3bvk6q/riven_combos_that_pros_use_guide
namespace SamRiven2
{
   
    internal class Program
    {
        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static Item Hydra
        {
            get { return ItemManager.Hydra; }
        }
        private static Item Titan
        {
            get { return ItemManager.Titanic; }
        }

        private static bool CanCastHydra()
        {
            return Hydra != null && Hydra.IsReady();
        }

        private static bool CanCastTitan()
        {
            return Titan != null && Titan.IsReady();
        }
        private static Dictionary<string, string> debugDraw = new Dictionary<string, string>();
        public static string AssemblyName = "SamRiven";
        public static Spell.Active Q = new Spell.Active(SpellSlot.Q);
        public static Spell.Active E = new Spell.Active(SpellSlot.E, 325);
        public static Spell.Skillshot R = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Cone, 250, 1600, 45)
        {
            MinimumHitChance = HitChance.High,
            AllowedCollisionCount = -1
        };
        public static Spell.Active W = new Spell.Active(SpellSlot.W, (uint)(70 + ObjectManager.Player.BoundingRadius + 120));
        public static uint WRange
        {
            get
            {
                return (uint)
                        (70 + ObjectManager.Player.BoundingRadius +
                         (ObjectManager.Player.HasBuff("RivenFengShuiEngine") ? 195 : 120));
            }
        }
        static void Main(string[] args)
        {
            Hacks.RenderWatermark = false;
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            Config.Init();
            ItemManager.Init();
            Game.OnUpdate += Game_OnUpdate;
            Q.OnSpellCasted += Q_OnSpellCasted;
            E.OnSpellCasted += E_OnSpellCasted;
            W.OnSpellCasted += W_OnSpellCasted;
            R.OnSpellCasted += R_OnSpellCasted;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Orbwalker.OnAttack += Orbwalker_OnAttack;
            Flash = Player.Spells.FirstOrDefault(a => a.SData.Name == "summonerflash");
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Utils.Print("Loaded");
        }

        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.SData.Name != "summonerflash" || afterFlash == "") return;
            switch (afterFlash)
            {
                case "W":
                    Core.DelayAction(() => Player.CastSpell(SpellSlot.W), 50);
                    break;
            }
            afterFlash = "";
        }

        private static string afterFlash = "";
        static void R_OnSpellCasted(Spell.SpellBase spell, GameObjectProcessSpellCastEventArgs args)
        {
            Utils.Debug(spell.Name);
            if (Config.BurstMode && BurstTarget != null && BurstTarget.IsValidTarget() && afterR1 == "Flash")
            {
                afterFlash = "W";
                Core.DelayAction(()=>Player.CastSpell(Flash.Slot, BurstTarget), 50);
            }
            afterR1 = "";
        }

        static void W_OnSpellCasted(Spell.SpellBase spell, GameObjectProcessSpellCastEventArgs args)
        {
            bool resetWanimationwithQ = true;
            //Utils.Debug("w onspellcast");
            if (CanCastHydra())
            {
                Utils.Debug("CASTING HYDRA LOL");
                Hydra.Cast();
            }
            if (CanCastTitan())
            {
                Utils.Debug("CASTING HYDRA LOL");
                Titan.Cast();
            }
            if (resetWanimationwithQ)
            {
                Core.DelayAction(() => CastQ(Game.CursorPos), 100);
            }
        }

        static void E_OnSpellCasted(Spell.SpellBase spell, GameObjectProcessSpellCastEventArgs args)
        {
            Utils.Debug(args.Time);
            Utils.Debug(Core.GameTickCount - args.Time*1000);
            if (afterE == "") return;
            switch (afterE)
            {
                case "W":
                    Core.DelayAction(() => CastW(), 100);
                    break;
                case "R1":
                    afterR1 = "Flash";
                    Core.DelayAction(() => CastR1(), 100);
                    break;
            }
            afterE = "";

        }

        static void Orbwalker_OnAttack(AttackableUnit target, EventArgs args)
        {
            debugDraw["Attack"] = "true;";
        }

        private static int lastAA = 0;
        private static int count = 0;
        private static int all = 0;
        private static string afterE = "";
        static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (Config.BurstMode && BurstTarget.IsValidTarget())
            {
                afterQ = "R2";
                CastQ(BurstTarget.ServerPosition);
                return;
            }
            lastAA = Environment.TickCount;
  
            debugDraw["Q" + QNum] = "" + (lastAA - lastQ);
            debugDraw["Q"] = QNum.ToString();
            //debugDraw["Attack"] = "false";
            if ((target == null || !target.IsValidTarget()) && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.Combo) return;
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo && (target == null || !target.IsValidTarget()))
            {
                target = TargetSelector.GetTarget(Q.Range + WRange + E.Range, DamageType.Physical);
                if (target == null || !target.IsValidTarget()) return;
            }
                
            if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None)
            {
                bool useQ = UseQ();
                bool useW = UseW();
                bool useE = UseE();
                
                if (useQ)
                {
                    Utils.Debug(QNum);
                    if (QNum == 2 && useW)
                    {

                        
                        if (target.Distance(_Player) < WRange)
                        {
                            W.Cast();
                            return;
                        }
                        if (useE && target.Distance(_Player) < WRange + E.Range)
                        {
                            Player.CastSpell(SpellSlot.E, target.Position);
                            afterE = "W";
                            //W.Cast();
                            return;
                        }


                    }
                    if (QNum != 1 && QNum != 2)
                    {
                        if (CanCastTitan())
                        {
                            Titan.Cast();
                            return;
                        }
                        bool willKillFromR2 = true;
                        if (willKillFromR2)
                        {
                            var t = target as Obj_AI_Base;
                            CastR2(t);
                        }
                    }
                    
                    
                        
                    CastQ(target.Position); 
                }
                
                
            }
                
        }

        private static void CastR2(Obj_AI_Base target)
        {
            if (target != null && target.IsValidTarget())
            {
                if (R.Name == "rivenizunablade" && R.IsReady())
                {
                    R.Cast(target);
                }
            }
        }

        private static bool UseE()
        {
            return E.IsReady();
        }

        private static bool UseW()
        {
            return W.IsReady();
        }

        private static bool UseQ()
        {
            return Q.IsReady();
        }

        private static bool debugdraw = false;
        static void Drawing_OnEndScene(EventArgs args)
        {
            debugDraw["lastQ"] = lastQ.ToString();
            debugDraw["Orbwalker.LastAA"] = lastAA.ToString();
            var i = 0;
            var heropos = _Player.Position.WorldToScreen();
            if(debugdraw)
            foreach (var kvp in debugDraw)
            {
                i++;
                Drawing.DrawText(heropos.X, heropos.Y + 15*(i+1), Color.WhiteSmoke, kvp.Key + ": "+ kvp.Value);
            }
            Drawing.DrawText(heropos.X, heropos.Y + 15 * (i + 1), Color.WhiteSmoke, "Force R:" + (Config.ForceR ? "ON" : "OFF"));
        }

        private static int QNum = 0;
        private static int lastW = 0;
        static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (_Player.IsDead) return;
            if (!sender.IsMe) return;
            int delay = 0;
            switch (args.Animation)
            {
                case "Spell1a":
                    delay = Config.Q1Delay;
                    lastQ = Environment.TickCount;
                    QNum = 1;
                    break;
                case "Spell1b":
                    delay = Config.Q2Delay;
                    lastQ = Environment.TickCount;
                    QNum = 2;
                    break;
                case "Spell1c":
                    delay = Config.Q3Delay;
                    lastQ = Environment.TickCount;
                    QNum = 3;
                    break;
                case "Spell2":
                    //delay = Config.WDelay;
                    Core.DelayAction(() => Player.DoEmote(Emote.Dance), Config.WDelay);
                    lastW = Environment.TickCount;
                    break;
                case "Dance":
                    if (lastQ > Environment.TickCount - 500 || lastW > Environment.TickCount - 500)
                    {
                        ResetAA();
                        //Utils.Debug("reset");
                    }
                        
                    break;
            }
            if (delay != 0 && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None)
            {
                
                Core.DelayAction(() => Player.DoEmote(Emote.Dance), delay);
                
            }
            if (delay != 0)
            {
                int qn = QNum;
                Core.DelayAction(() => ResetQ(qn), 3600);
            }
                
            
            /*if(args.Animation != "Run")
            Utils.Debug(args.Animation);#1#
        }
        
        static void ResetQ(int qn)
        {
            //Utils.Debug(qn);
            if (qn == QNum)
                QNum = 0;
        }
        static void ResetAA()
        {
            //Core.DelayAction(Orbwalker.ResetAutoAttack, 100);
            if (!Orbwalker.CanAutoAttack) //
                Orbwalker.ResetAutoAttack();
        }
      

        static void Q_OnSpellCasted(Spell.SpellBase spell, GameObjectProcessSpellCastEventArgs args)
        {

            switch (afterQ)
            {
                case "R2":
                    if (Config.BurstMode)
                        Core.DelayAction(() => CastR2(BurstTarget), 50);
                    break;
            }
            //Player.DoEmote(Emote.Dance);
        }

        static void Game_OnUpdate(EventArgs args)
        {
            debugDraw["qstate"] = Q.Name.ToString();
            debugDraw["qnum"] = QNum.ToString();
            if (_Player.IsDead) return;
            if (Config.BurstMode) Burst();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) JungleClear();

        }
        public static SpellDataInst Flash;
        private static AIHeroClient BurstTarget;
        private static void Burst()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            var target = TargetSelector.SelectedTarget;
            if (target == null || !target.IsValidTarget()) return;
            Orbwalker.ForcedTarget = target;
            // requirenments
            Utils.Debug("lel");
            if (!(Flash.IsReady && R.IsReady() && R.Name == "RivenFengShuiEngine" && E.IsReady() && W.IsReady() && Q.IsReady())) return;
            Utils.Debug("lel2");
            if (_Player.Distance(target) < E.Range + W.Range/2 + 425)
            {
                BurstTarget = target;
                afterE = "R1";
                CastE(BurstTarget.ServerPosition);
            }
        }

        static void JungleClear()
        {
            var mob = EntityManager.MinionsAndMonsters.Monsters.FirstOrDefault(m => _Player.IsInAutoAttackRange(m));
            if (mob == null || !mob.IsValid) return;
            var aaRange = _Player.AttackRange;
            var dist = _Player.Distance(mob);
            if (E.IsReady() && dist < E.Range + aaRange)
            {
                CastE(Game.CursorPos);
            }
            if (Q.IsReady() && dist < Q.Range)
            {
                debugDraw["isInAutoRange"] = _Player.IsInAutoAttackRange(mob).ToString();
                if (_Player.IsInAutoAttackRange(mob) && lastQ > lastAA) return;
                //CastQ(mob.ServerPosition);
            }
        }

        private static bool qFinished()
        {
            int delay = 0;
            switch (QNum)
            {
                case 0:
                    delay = 0;
                    break;
                case 1:
                    delay = Config.Q1Delay;
                    break;
                case 2:
                    delay = Config.Q2Delay;
                    break;
                case 3:
                    delay = Config.Q3Delay;
                    break;
            }
            return lastQ > Environment.TickCount + delay;
        }
        private static void Combo()
        {
            if (Orbwalker.IsAutoAttacking) return;
            var aaRange = _Player.AttackRange;
            var target = TargetSelector.GetTarget(E.Range + Q.Range + aaRange, DamageType.Physical);
            if (target == null || !target.IsValidTarget()) return;
            var dist = _Player.Distance(target);
            if (E.IsReady() && dist < E.Range + aaRange)
            {
                if(Config.ForceR)
                    afterE = "R1";
                CastE(Game.CursorPos);
            }
            bool waitforAA = true;
            if (!waitforAA && Q.IsReady() && dist < Q.Range + aaRange)
            {
                debugDraw["isInAutoRange"] = _Player.IsInAutoAttackRange(target).ToString();
                //debugDraw["isInAutoRange"] = _Player.IsInAutoAttackRange(target).ToString();
                if (_Player.IsInAutoAttackRange(target) && lastQ > lastAA) return;
                CastQ(target.ServerPosition);
            }
            //Core.DelayAction(()=>Chat.Print("Delay"), 100);
        }

        private static int lastQ = 0;
        private static string afterQ = "";
        private static string afterR1 = "";

        private static void CastQ(Vector3 pos)
        {
            //var delay = 10;
            //if (lastQ + delay> Environment.TickCount) return;
            Player.CastSpell(SpellSlot.Q, pos);
        }

        private static void CastW()
        {
            if (W.IsReady())
                Player.CastSpell(SpellSlot.W);

        }

        private static void CastR1()
        {
            if (R.IsReady() && R.Name == "RivenFengShuiEngine")
                Player.CastSpell(SpellSlot.R);
        }
        private static void CastE(Vector3 pos)
        {
            Player.CastSpell(SpellSlot.E, pos);
        }
    }
}
*/
