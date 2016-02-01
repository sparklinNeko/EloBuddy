using System;
using System.Linq;
using System.Security.AccessControl;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using SamOrb = SamRiven2.Orb.Orbwalker;
namespace SamRiven2
{
    public class Program
    {
        private static AIHeroClient _Player
        {
            get { return Player.Instance; }
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
        public static string AssemblyName = "SamRiven";
        public static Spell.Active Q = new Spell.Active(SpellSlot.Q, 275);
        public static Spell.Active E = new Spell.Active(SpellSlot.E, 325);
        public static Spell.Skillshot R = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Cone, 250, 1600, 45);
        public static Spell.Active W = new Spell.Active(SpellSlot.W, (uint)(70 + ObjectManager.Player.BoundingRadius + 120));
        
        static void Main(string[] args)
        {
            Hacks.RenderWatermark = false;
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (_Player.ChampionName != "Riven") return;
            Config.Init();
            ItemManager.Init();
            //SamOrb.Init();
            //Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdate2;
            //Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Obj_AI_Base.OnSpellCast += AfterAttack;
            
        }

        

        static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.Slot == SpellSlot.W)
            {
                forceW = false;
                /*lastW = Core.GameTickCount;
                if (CanCastHydra())
                    Hydra.Cast();
                Utils.Debug(lastW - lastHydra);*/
                if (Orbwalker.LastTarget != null && Orbwalker.LastTarget.IsValidTarget())
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        Utils.Debug("CAST Q");
                        Core.DelayAction(() => Player.CastSpell(SpellSlot.Q, Orbwalker.LastTarget.Position), 1);
                    }
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {
                        Utils.Debug("CAST Q");
                        Core.DelayAction(() => Player.CastSpell(SpellSlot.Q, Orbwalker.LastTarget.Position), 1);
                    }
                }

            }
            if (args.SData.Name == "ItemTiamatCleave")
            {

                if (Orbwalker.LastTarget != null && Orbwalker.LastTarget.IsValidTarget())
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        Utils.Debug("CAST Q");
                        Core.DelayAction(() => Player.CastSpell(SpellSlot.Q, Orbwalker.LastTarget.Position), 1);
                    }
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {
                        Utils.Debug("CAST Q");
                        Core.DelayAction(() => Player.CastSpell(SpellSlot.Q, Orbwalker.LastTarget.Position), 1);
                    }
                }
            }
        }

        private static int lastW;
        private static int lastHydra;
        private static bool forceW = false;
        // Hydra -> E is faster
        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            Utils.Debug(args.SData.Name);
            if (args.SData.Name == "ItemTiamatCleave" && W.IsReady(500))
            {
                
                forceW = true;
                if(W.IsReady())
                    W.Cast();
                Core.DelayAction(()=>forceW = false, 500);
                return;
            }
            if (args.Slot == SpellSlot.W)
            {
                forceW = false;
                lastW = Core.GameTickCount;
                if (CanCastHydra())
                    Hydra.Cast();
                Utils.Debug(lastW - lastHydra);
                

            }
            
        }

        

        private static int lastQ;
        private static int lastQDelay;
        private static int QNum = 0;
        static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (_Player.IsDead) return;
            if (!sender.IsMe) return;
            int delay = 0;
            switch (args.Animation)
            {
                case "Spell1a":
                    delay = Config.Q1Delay;
                    lastQ = Core.GameTickCount;
                    QNum = 1;
                    break;
                case "Spell1b":
                    delay = Config.Q2Delay;
                    lastQ = Core.GameTickCount;
                    QNum = 2;
                    break;
                case "Spell1c":
                    delay = Config.Q3Delay;
                    lastQ = Core.GameTickCount;
                    QNum = 3;
                    break;
                case "Dance":
                    if (lastQ > Core.GameTickCount - 500)
                    {
                        
                        Orbwalker.ResetAutoAttack();
                        //Utils.Debug("reset");
                    }
                        
                    break;
            }
            
            if (delay != 0 && (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None))
            {
                //Utils.Debug(delay);
                lastQDelay = delay;
                Core.DelayAction(DanceIfNotAborted, delay);
                
                    //Orbwalker.ResetAutoAttack();
                
                    
                Utils.Debug("reset");
                    
            }
 
            if(args.Animation != "Run")
                Utils.Debug(args.Animation);
        }

        private static void DanceIfNotAborted()
        {
            Player.DoEmote(Emote.Dance);
            
            Orbwalker.ResetAutoAttack();
            if (ComboTarget != null && ComboTarget.IsValidTarget(_Player.AttackRange))
            {
                Player.IssueOrder(GameObjectOrder.AttackUnit, ComboTarget);
                return;
            }
            /*if (JCTarget != null && JCTarget.IsValidTarget(_Player.AttackRange))
            {
                Player.IssueOrder(GameObjectOrder.AttackUnit, ComboTarget);
                return;
            }*/
        }
        private static int lastAA;
        static void Orbwalker_OnAttack(AttackableUnit target, EventArgs args)
        {
            lastAA = Core.GameTickCount;
            if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None)
            {
                lastAA = Core.GameTickCount;
            }
        }
        private static void AfterAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            var target = args.Target as AttackableUnit;
            if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None && target != null && target.IsValidTarget())
            {
                if (!Q.IsReady() && !W.IsReady() && CanCastHydra())
                    Hydra.Cast();
                if (!Q.IsReady() && !W.IsReady() && CanCastTitan())
                    Titan.Cast();
                lastAA = Core.GameTickCount;
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)))
                    {
                        if (QNum == 2 && W.IsReady())
                        {
                            var target2 = (ComboTarget != null && ComboTarget.IsValidTarget(WRange)) ? ComboTarget :
                            TargetSelector.GetTarget(WRange, DamageType.Physical, null, true);
                            if (target2 != null && target2.IsValidTarget() && !CanCastHydra())
                            {
                                Player.CastSpell(SpellSlot.W);
                                return;
                            }
                            if (CanCastHydra())
                                Hydra.Cast();

                        }
                    }
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {
                        if (QNum == 2 && W.IsReady())
                        {
                            var target2 = (JCTarget != null && JCTarget.IsValidTarget(WRange)) ? JCTarget : EntityManager.MinionsAndMonsters.GetJungleMonsters(null, WRange, true).FirstOrDefault();
                            if (target2 != null && target2.IsValidTarget() && !CanCastHydra())
                            {
                                Player.CastSpell(SpellSlot.W);
                                return;
                            }
                            if (CanCastHydra())
                                Hydra.Cast();

                        }
                    }


                    if (target != null && target.IsValidTarget() && target.IsValid && !target.IsDead && !target.IsZombie)
                    {
                        if (QNum != 2 || !W.IsReady())
                            Player.CastSpell(SpellSlot.Q, target.Position);
                    }
                }

            }
        }
        static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None && target != null && target.IsValidTarget())
            {
                if (!Q.IsReady() && !W.IsReady() && CanCastHydra())
                    Hydra.Cast();
                if (!Q.IsReady() && !W.IsReady() && CanCastTitan())
                    Titan.Cast();
                lastAA = Core.GameTickCount;
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    if((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)))
                    {
                        if (QNum == 2 && W.IsReady())
                    {
                        var target2 = (ComboTarget != null && ComboTarget.IsValidTarget(WRange)) ? ComboTarget :
                        TargetSelector.GetTarget(WRange, DamageType.Physical, null, true);
                        if (target2 != null && target2.IsValidTarget() && !CanCastHydra())
                        {
                            Player.CastSpell(SpellSlot.W);
                            return;
                        }
                        if (CanCastHydra())
                            Hydra.Cast();

                    }
                    }
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    {
                        if (QNum == 2 && W.IsReady())
                        {
                            var target2 = (JCTarget != null && JCTarget.IsValidTarget(WRange)) ? JCTarget : EntityManager.MinionsAndMonsters.GetJungleMonsters(null, WRange, true).FirstOrDefault();
                            if (target2 != null && target2.IsValidTarget() && !CanCastHydra())
                            {
                                Player.CastSpell(SpellSlot.W);
                                return;
                            }
                            if(CanCastHydra())
                                Hydra.Cast();

                        }
                    }

                    
                    if (target != null && target.IsValidTarget() && target.IsValid && !target.IsDead && !target.IsZombie)
                    {
                        if(QNum != 2 || !W.IsReady())
                            Player.CastSpell(SpellSlot.Q, target.Position);
                    }
                }

            }
            
        }
        public static uint WRange
        {
            get
            {
                return (uint)
                        (70 + _Player.BoundingRadius +
                         (_Player.HasBuff("RivenFengShuiEngine") ? 195 : 120));
            }
        }
        private static void Game_OnUpdate2(EventArgs args)
        {
            if (lastQ + 3650 < Core.GameTickCount)
                QNum = 0;
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) JungleClear();
        }

        

        static void Game_OnUpdate(EventArgs args)
        {
            if (forceW)
            {
                Player.CastSpell(SpellSlot.W);
            }
            if (SamOrb.ActiveModes.HasFlag(SamOrb.Modes.Combo)) Combo();
        }

        private static Obj_AI_Minion JCTarget;
        static void JungleClear()
        {

            if (Q.IsReady() && QNum == 0 && !Config.AAFirst)
            {
                var target = (JCTarget != null && JCTarget.IsValidTarget(Q.Range + Player.Instance.BoundingRadius)) ? JCTarget : EntityManager.MinionsAndMonsters.GetJungleMonsters(null, Q.Range, true).FirstOrDefault();
                if (target != null && target.IsValidTarget())
                {
                    JCTarget = target;
                    Player.CastSpell(SpellSlot.Q, target.ServerPosition);
                }
            }
        }


        private static AIHeroClient ComboTarget;
        static void Combo()
        {
            
            //Utils.Debug(QNum);
            if (Q.IsReady() && QNum == 4 && !Config.AAFirst)
            {
                var target = (ComboTarget != null && ComboTarget.IsValidTarget(Q.Range + Player.Instance.BoundingRadius)) ? ComboTarget :
                TargetSelector.GetTarget(Q.Range, DamageType.Physical, null, true);
                if (target != null && target.IsValidTarget())
                {
                    ComboTarget = target;
                    //Utils.Debug(Q.Range);
                    //Utils.Debug(lastQ + "  "+ lastQDelay + "  <= " + lastAA);
                        Player.CastSpell(SpellSlot.Q, target.ServerPosition);
                }
            }

        }
    }
}