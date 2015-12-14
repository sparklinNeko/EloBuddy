using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using SharpDX;
// ReSharper disable UnusedMember.Local

// ReSharper disable InconsistentNaming



/*
    todo:
 * target selector - all skills the same target ?
 * what about damage type for skills? 
 * 
    
    

 */
namespace SamKogmaw
{
    public static class Kogmaw
    {
        private static Spell.Skillshot Q, E, R;
        private static readonly uint[] wRange = {590, 620, 650, 680, 710};
        private static readonly uint[] rRange = {1200, 1500, 1800};
        private static Spell.Active W;
        private static bool[] ComboSkillsUsed = {false, false, false};
        private static bool isCombo;
        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Init()
        {
            W = new Spell.Active(SpellSlot.W, 590);
            Q = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Linear);
            E = new Spell.Skillshot(SpellSlot.E, 1280, SkillShotType.Linear, 250, null, 120)
            {
                AllowedCollisionCount = -1
            };
            R = new Spell.Skillshot(SpellSlot.R, 1800, SkillShotType.Circular, 1500, int.MaxValue, 200)
            {
                AllowedCollisionCount = -1
            };
            Game.OnUpdate += Game_OnTick;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Obj_AI_Base.OnTeleport += Obj_AI_Base_OnTeleport;
            Game.OnEnd += Game_OnEnd;
            Orbwalker.DisableMovement = false;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Utils.Print("Loaded. Don't forget to check Movement Limit submenu ");
        }

        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (_Player.IsDead) return;
            if(!sender.IsMe || !isCombo) return;
            //Utils.Print(args.Slot);
            if (args.Slot == SpellSlot.Q)
                ComboSkillsUsed[0] = true;
            if (args.Slot == SpellSlot.E)
                ComboSkillsUsed[1] = true;
            if (args.Slot == SpellSlot.R)
                ComboSkillsUsed[2] = true;
        }

        static void Drawing_OnEndScene(EventArgs args)
        {
            var pos = _Player.Position;
            if (!Config.drawMenu.GetCheckBox("enabled")) return;
            var dr = Config.drawMenu.GetCheckBox("drawready");
            var drawQ = dr ? Q.IsReady() && Config.drawMenu.GetCheckBox("drawq") : Config.drawMenu.GetCheckBox("drawq");
            var drawW = dr ? E.IsReady() && Config.drawMenu.GetCheckBox("draww") : Config.drawMenu.GetCheckBox("draww");
            var drawE = dr ? W.IsReady() && Config.drawMenu.GetCheckBox("drawe") : Config.drawMenu.GetCheckBox("drawe");
            var drawR = dr ? R.IsReady() && Config.drawMenu.GetCheckBox("drawr") : Config.drawMenu.GetCheckBox("drawr");
            if (drawQ)
                Circle.Draw(Color.DarkOliveGreen, Q.Range, pos);
            if (drawW)
                Circle.Draw(Color.DarkOliveGreen, GetMyRange(true), pos);
            if (drawE)
                Circle.Draw(Color.DarkOliveGreen, E.Range, pos);
            if (drawR)
                Circle.Draw(Color.DarkOliveGreen, R.Range, pos);
            if (Config.drawMenu.GetCheckBox("drawpd"))
                DrawPredictedDamage();
        }
        private static readonly float _barLength = 104;
        private static readonly float _xOffset = 2;
        private static readonly float _yOffset = 9;
        private static void DrawPredictedDamage()
        {
            foreach (var aiHeroClient in EntityManager.Heroes.Enemies)
            {
                if (!aiHeroClient.IsHPBarRendered || !aiHeroClient.VisibleOnScreen) continue;

                var pos = new Vector2(aiHeroClient.HPBarPosition.X + _xOffset, aiHeroClient.HPBarPosition.Y + _yOffset);
                var fullbar = (_barLength) * (aiHeroClient.HealthPercent / 100);
                var damage = (_barLength) *
                                 ((GetComboDamage(aiHeroClient) / aiHeroClient.MaxHealth) > 1
                                     ? 1
                                     : (GetComboDamage(aiHeroClient) / aiHeroClient.MaxHealth));
                Line.DrawLine(System.Drawing.Color.WhiteSmoke, 9f, new Vector2(pos.X, pos.Y),
                    new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y));
                Line.DrawLine(System.Drawing.Color.Black, 9, new Vector2(pos.X + (damage > fullbar ? fullbar : damage) - 2, pos.Y), new Vector2(pos.X + (damage > fullbar ? fullbar : damage) + 2, pos.Y));
            }
        }

        private static float GetComboDamage(AIHeroClient target)
        {
            return (R.IsReady() ? _Player.GetSpellDamage(target, SpellSlot.R) : 0) + (Q.IsReady() ? _Player.GetSpellDamage(target, SpellSlot.Q) : 0) +
                (E.IsReady() ? _Player.GetSpellDamage(target, SpellSlot.E) : 0) + ((1/_Player.AttackDelay))*_Player.GetAutoAttackDamage(target);
        }

        private static void Game_OnEnd(GameEndEventArgs args)
        {
            Orbwalker.DisableMovement = false;
        }


        private static void Obj_AI_Base_OnTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            // todo CC
            // todo onrecall
            if (_Player.IsDead) return;
            if (sender == null  || !sender.IsEnemy || !Config.interruptrecallMenu.GetCheckBox("enabled")) return;
            bool useR = Config.interruptrecallMenu.GetCheckBox("rinterruptrecall") && R.IsReady() &&
                        Config.interruptrecallMenu.GetSlider("rinterruptrecallmana") < _Player.ManaPercent;
            if (useR && sender.Distance(_Player) < rRange[R.Level - 1])
            {
                Player.CastSpell(SpellSlot.R, sender.Position);
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            // todo CC
            // todo onrecall
            //if(sender.IsMe) Utils.Debug(args.Buff.Name);
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (_Player.IsDead || !Config.gapcloseMenu.GetCheckBox("antigapcloseenabled")) return;
            if (!Config.gapcloseMenu.GetCheckBox("eantigapclose") ||
                (_Player.ManaPercent < Config.gapcloseMenu.GetSlider("eantigapclosemana"))) return;
            if (sender == null || !e.Sender.IsValidTarget(E.Range) || e.Sender.IsAlly) return;
            //if (e.Start.Distance(_Player) < e.End.Distance(_Player)) return;

            
            var edelay = Config.gapcloseMenu.GetSlider("eantigapclosedelay");
            if (edelay > 0)
                Core.DelayAction(() => CastE(sender, HitChance.High), edelay);
            else
                CastE(sender, HitChance.High);
        }

        private static int lastSkill;
        private static void CastR(Obj_AI_Base target, HitChance minimumHitchance = HitChance.Unknown)
        {
            if (lastSkill == Core.GameTickCount) return;
            lastSkill = Core.GameTickCount;
            if (!R.IsReady()) return;
            //Utils.Debug("Casting R");
            var hitchance = HitChance.High;
            if (minimumHitchance != HitChance.Unknown)
            {
                hitchance = R.MinimumHitChance;
                R.MinimumHitChance = minimumHitchance;
            }
            R.Cast(target);
            if (minimumHitchance != HitChance.Unknown)
                R.MinimumHitChance = hitchance;
        }
        private static void CastQ(Obj_AI_Base target, HitChance minimumHitchance = HitChance.Unknown)
        {
            if (lastSkill == Core.GameTickCount) return;
            lastSkill = Core.GameTickCount;
            if (!Q.IsReady()) return;
            //Utils.Debug("Casting Q");
            var hitchance = HitChance.High;
            if (minimumHitchance != HitChance.Unknown)
            {
                hitchance = Q.MinimumHitChance;
                Q.MinimumHitChance = minimumHitchance;
            }
            Q.Cast(target);
            if (minimumHitchance != HitChance.Unknown)
                Q.MinimumHitChance = hitchance;
        }
        private static void CastE(Obj_AI_Base target, HitChance minimumHitchance = HitChance.Unknown)
        {
            if (lastSkill == Core.GameTickCount) return;
            lastSkill = Core.GameTickCount;
            if (!E.IsReady()) return;
            //Utils.Debug("Casting E");
            var hitchance = HitChance.High;
            if (minimumHitchance != HitChance.Unknown)
            {
                hitchance = E.MinimumHitChance;
                E.MinimumHitChance = minimumHitchance;
            }
            E.Cast(target);
            if (minimumHitchance != HitChance.Unknown)
                E.MinimumHitChance = hitchance;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (_Player.IsDead) return;

            if(Program.LagFree(0))
                ChangeSkills();
            if (Program.LagFree(1))
                LimitOrbwalker();
            if (Program.LagFree(2))
                KillSteal();
            if (Program.LagFree(3))
                AutoHarass();
            
            //Utils.Debug(W.State);
            if (Program.LagFree(4))
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) Combo();
            if (Program.LagFree(5))
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) LaneClear();
            if (Program.LagFree(6))
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) Harass();
        }

        private static void KillSteal()
        {
            if (!Config.killstealMenu.GetCheckBox("enabled")) return;
            bool useQ = UseQ("killsteal");
            bool useE = UseE("killsteal");
            bool useR = UseR("killsteal");
            
            if(!useQ && !useR && !useE) return;
            var maxRange = useR
                ? rRange[R.Level - 1]
                : useE
                    ? E.Range
                    : Q.Range;

            foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies)
            {
                if(enemy == null || !enemy.IsValidTarget(maxRange)) continue;
                if (useR)
                {
                    if (_Player.GetSpellDamage(enemy, SpellSlot.R) >= enemy.Health)
                    {
                        CastR(enemy, HitChance.High);
                        return;
                    }
                        
                }
                if (useE)
                {
                    if (_Player.GetSpellDamage(enemy, SpellSlot.E) >= enemy.Health)
                    {
                        CastE(enemy, HitChance.High);
                        return;
                    }
                        
                }
                if (useQ)
                {
                    if (_Player.GetSpellDamage(enemy, SpellSlot.Q) >= enemy.Health)
                    {
                        CastQ(enemy, HitChance.High);
                        return;
                    }
                        
                }
            }
        }
        // gotta rewrite this menu shit
        private static readonly Dictionary<string, Menu> ModeMenus = new Dictionary<string, Menu>()
        {
            {"combo", Config.comboMenu},
            {"harass", Config.harassMenu},
            {"laneclear", Config.laneclearMenu},
            {"killsteal", Config.killstealMenu},
            {"autoharass", Config.autoharassMenu}

        };
        private static bool UseQ(string mode)
        {
            return ModeMenus[mode].GetCheckBox("q"+mode) && Q.IsReady() &&
                         ModeMenus[mode].GetSlider("q"+mode+"mana") < _Player.ManaPercent;
        }
        private static bool UseW(string mode)
        {
            return ModeMenus[mode].GetCheckBox("w" + mode) && W.IsReady() &&
                         ModeMenus[mode].GetSlider("w" + mode + "mana") < _Player.ManaPercent;
        }
        private static bool UseE(string mode)
        {
            return ModeMenus[mode].GetCheckBox("e" + mode) && E.IsReady() &&
                         ModeMenus[mode].GetSlider("e" + mode + "mana") < _Player.ManaPercent;
        }
        private static bool UseR(string mode)
        {
            return ModeMenus[mode].GetCheckBox("r" + mode) && R.IsReady() &&
                         ModeMenus[mode].GetSlider("r" + mode + "mana") < _Player.ManaPercent;
        }
        

        private static void ChangeSkills()
        {
            if (W.State == SpellState.Surpressed && E.CastDelay == 250)
            {
                Q.CastDelay = 125;
                E.CastDelay = 125;
                R.CastDelay = 750;
            }
            if (W.State != SpellState.Surpressed && E.CastDelay == 125)
            {
                Q.CastDelay = 250;
                E.CastDelay = 250;
                R.CastDelay = 1500;
            }
            if (isCombo && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ComboSkillsUsed[0] = false;
                ComboSkillsUsed[1] = false;
                ComboSkillsUsed[2] = false;
                isCombo = false;
            }
        }

        private static bool CheckBetween(int max)
        {
            //Utils.Debug(min+ " "+ (1/_Player.AttackDelay)+" "+ max);
            return ((int)((1/_Player.AttackDelay)*1000) < max);
        }

        private static void LimitOrbwalker()
        {
            // if w is not active (as <2.5) and forcelimit is off
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee) || W.State != SpellState.Surpressed && !Config.limitMenu.GetCheckBox("forcelimit"))
            {

                Orbwalker.DisableMovement = false;
                return;
            }
            var target = TargetSelector.GetTarget(_Player.AttackRange, DamageType.Mixed);
            if(isCombo && (target == null|| !target.IsValidTarget()))
            {
                Orbwalker.DisableMovement = false;
                return;
            }
            //todo : gotta test
            /*if (Orbwalker.LastTarget == null || !Orbwalker.LastTarget.IsValidTarget()
                || CheckBetween((Core.GameTickCount - Orbwalker.LastAutoAttack)/2))
            {
                Utils.Debug("Lel");
                Orbwalker.DisableMovement = false;
                return;
            }*/
            
            var num = Config.miscMenu.GetSlider("delaynum");

            var newDelay = Config.limitMenu.GetSlider("mmd_1");
            var allowed = CheckBetween(Config.limitMenu.GetSlider("maxas_1"));
            if (num == 1)
            {
                if (allowed)
                {
                    Orbwalker.DisableMovement = false;
                    return;
                }
            }
            else
            {
                for (var i = num; i > 1; i--)
                {
                    if (Config.limitMenu.GetSlider("maxas_" + (i)) < Config.limitMenu.GetSlider("maxas_" + (i - 1)))
                    {
                        Utils.Print("Max Attack Speed (" + i + "): " + Config.limitMenu.GetSlider("maxas_" + (i)) + " must be more than previous one: " + Config.limitMenu.GetSlider("maxas_" + (i-1)));
                        break;
                    }
                    allowed = CheckBetween(Config.limitMenu.GetSlider("maxas_" + (i)));
                    if (!allowed)
                    {
                        newDelay = Config.limitMenu.GetSlider("mmd_" + (i));
                        break;
                    }
                }
            }

            if (allowed)
            {
                Orbwalker.DisableMovement = false;
                return;
            }
            // && W.State != SpellState.Surpressed && !Config.limitMenu.GetCheckBox("forcelimit")
            if (newDelay > Orbwalker.MovementDelay)
            {
                Orbwalker.DisableMovement = Orbwalker.LastMovementSent + newDelay > Core.GameTickCount;
            }
        }

        private static void AutoHarass()
        {
           
            if (!Config.autoharassMenu.GetCheckBox("autoharassenabled")) return;
            // autoharass 
            var immobile = Config.autoharassMenu.GetCheckBox("onlyimmobile");

            // some mode is active
            if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None) return;
            bool useR = UseR("autoharass") &&
                        Config.autoharassMenu.GetSlider("rstacks") > CurrentRStacks();
            bool useE = UseE("autoharass");
            bool useQ = UseQ("autoharass");
            if (!useQ && !useE && !useR) return;
            var range = useQ
                ? Q.Range
                : useE
                    ? E.Range
                    : rRange[R.Level - 1];
            if (range == 0) return;
            var enemy = TargetSelector.GetTarget(range, DamageType.Magical);
            // Prioritize R over E over Q, one at a time
            
                
                PredictionResult pred;
                if (useR)
                {
                    if (enemy.IsImmobile())
                    {
                        Player.CastSpell(SpellSlot.R, enemy.ServerPosition);
                        return;
                    }
                    else if(!immobile)
                    {
                        pred = R.GetPrediction(enemy);
                        if (pred.HitChance >= HitChance.High)
                        {
                            Player.CastSpell(SpellSlot.R, pred.CastPosition);
                            return;
                        }  
                    }

                    
                }
                
                if (useE)
                {
                    if (enemy.IsImmobile())
                    {
                        Player.CastSpell(SpellSlot.E, enemy.ServerPosition);
                        return;
                    }
                    else if (!immobile)
                    {
                        pred = E.GetPrediction(enemy);
                        if (pred.HitChance >= HitChance.High)
                        {
                            Player.CastSpell(SpellSlot.E, pred.CastPosition);
                            return;
                        } 
                    }
                    
                }

                if (useQ)
                {
                    if (enemy.IsImmobile())
                    {
                        Player.CastSpell(SpellSlot.Q, enemy.ServerPosition);
                        return;
                    }
                    else if (!immobile)
                    {
                        pred = Q.GetPrediction(enemy);
                        if (pred.HitChance >= HitChance.High)
                        {
                            Player.CastSpell(SpellSlot.Q, pred.CastPosition);
                            return;
                        }
                    }

                }
                

            
        }

        private static void Combo()
        {
            if (!isCombo)
                isCombo = true;
            
            bool useW = Config.comboMenu.GetCheckBox("wcombo") && W.IsReady() &&
                        Config.comboMenu.GetSlider("wcombomana") < _Player.ManaPercent;
            bool useQ = Config.comboMenu.GetCheckBox("qcombo") && Q.IsReady() &&
                        Config.comboMenu.GetSlider("qcombomana") < _Player.ManaPercent;
            bool useE = Config.comboMenu.GetCheckBox("ecombo") && E.IsReady() &&
                        Config.comboMenu.GetSlider("ecombomana") < _Player.ManaPercent;
            bool useR = Config.comboMenu.GetCheckBox("rcombo") && R.IsReady() &&
                        Config.comboMenu.GetSlider("rcombomana") < _Player.ManaPercent &&
                        Config.comboMenu.GetSlider("rstacks") > CurrentRStacks();
            var aarange = GetMyRange(useW);
            var target = TargetSelector.GetTarget(aarange, DamageType.Physical);
            if (target != null && target.IsValidTarget())
            {
                W.Cast();
                if(target.HealthPercent <= Config.itemMenu.GetSlider("botrkenemyhp") && _Player.HealthPercent <= Config.itemMenu.GetSlider("botrkmyhp"))
                    ItemManager.CastBotrkOrCutlass(target);
            }
                
            if (Config.comboMenu.GetCheckBox("aapriority") && target != null && target.IsValidTarget(aarange))
            {
                useQ = useQ && !ComboSkillsUsed[0];
                useE = useE && !ComboSkillsUsed[1];
                useR = useR && !ComboSkillsUsed[2];
            }
            if (!useQ && !useE && !useR) return;
            // todo : wtf it's always true? -_-
            // okay got it  - 
            // w8 wtf, so actually if everything above is false than 
            // oh.. fuck this shit.
            var range = useQ
                ? Q.Range
                : useE
                    ? E.Range
                    : useR 
                        ? rRange[R.Level - 1]
                        : aarange;
            target = TargetSelector.GetTarget(range, DamageType.Magical);
            if (target == null || !target.IsValidTarget()) return;
            if (useE)
            {
                CastE(target);
                //ComboSkillsUsed[1] = true;
            }
            if (useR)
            {
                CastR(target);
                //ComboSkillsUsed[2] = true;
            }
            
            if (useQ)
            {
                CastQ(target);
                //ComboSkillsUsed[0] = true;
            }
        }

        private static float GetMyRange(bool useW)
        {
            return (useW && W.IsReady())
                ? wRange[W.Level-1] + _Player.BoundingRadius
                : _Player.AttackRange + _Player.BoundingRadius;
        }
        // We expect player to have all skills learned
        /*private static float GetSkillsRange(bool useQ, bool useW, bool useE, bool useR)
        {

            return useQ
                ? Q.Range
                : useE
                    ? E.Range
                    : useR
                        ? rRange[R.Level - 1]
                        : GetMyRange(useW);
        }*/
        
        private static void LaneClear()
        {
            if (!Config.laneclearMenu.GetCheckBox("enabled")) return;
            bool useQ = Config.laneclearMenu.GetCheckBox("qlaneclear") && Q.IsReady() &&
                        Config.laneclearMenu.GetSlider("qlaneclearmana") < _Player.ManaPercent;
            bool useW = Config.laneclearMenu.GetCheckBox("wlaneclear") && W.IsReady() &&
                        Config.laneclearMenu.GetSlider("wlaneclearmana") < _Player.ManaPercent;
            bool useE = Config.laneclearMenu.GetCheckBox("elaneclear") && E.IsReady() &&
                        Config.laneclearMenu.GetSlider("elaneclearmana") < _Player.ManaPercent;
            bool useR = Config.laneclearMenu.GetCheckBox("rlaneclear") && R.IsReady() &&
                        Config.laneclearMenu.GetSlider("rlaneclearmana") < _Player.ManaPercent &&
                        Config.laneclearMenu.GetSlider("rstacks") > CurrentRStacks();
        
            int minionNumber = Config.laneclearMenu.GetSlider("minions");

            if (!useQ && !useW && !useE && !useR) return;
            var aarange = GetMyRange(useW);
            if (useW)
            {
                var wMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, null,
                    aarange);
                if(wMinions != null)
                if (wMinions.Count() >= minionNumber)
                    W.Cast();
            }
            if (useE)
            {
                var eMinions =
                EntityManager.MinionsAndMonsters.GetLineFarmLocation(EntityManager.MinionsAndMonsters.EnemyMinions,
                    E.Width, (int) E.Range);
                if (eMinions.HitNumber >= minionNumber)
                    Player.CastSpell(SpellSlot.E, eMinions.CastPosition);
            }
            if (useR)
            {
                var rMinions =
                    EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                        EntityManager.MinionsAndMonsters.EnemyMinions,
                        R.Width, (int) rRange[R.Level -1 ]);
                if (rMinions.HitNumber >= minionNumber)
                    Player.CastSpell(SpellSlot.R, rMinions.CastPosition);
            }
            if (useQ)
            {
                var qMinion = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                    m => m.Distance(_Player) < Q.Range && m.Health < _Player.GetSpellDamage(m, SpellSlot.Q) && m.IsValidTarget());
                if (qMinion != null)
                    CastQ(qMinion);
            }
            
        }

        private static int CurrentRStacks()
        {
            if (_Player.HasBuff("kogmawlivingartillerycost"))
                return _Player.Buffs.Find(b => b.Name == "kogmawlivingartillerycost").Count;
            return 0;
        }

        private static bool IsImmobileBuff(this BuffInstance buff)
        {
            return (buff.Type == BuffType.Stun || buff.Type == BuffType.Snare ||
                    buff.Type == BuffType.Knockup ||
                    buff.Type == BuffType.Charm || buff.Type == BuffType.Fear ||
                    buff.Type == BuffType.Knockback ||
                    buff.Type == BuffType.Taunt || buff.Type == BuffType.Suppression);
        }
        private static bool IsImmobile(this Obj_AI_Base target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) ||
                target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression))
            {
                return true;
            }
            return false;
        }

        private static void Harass()
        {
            if (!Config.harassMenu.GetCheckBox("enabled")) return;
            bool useQ = Config.harassMenu.GetCheckBox("qharass") && Q.IsReady() &&
                        Config.harassMenu.GetSlider("qharassmana") < _Player.ManaPercent;
            bool useW = Config.harassMenu.GetCheckBox("wharass") && W.IsReady() &&
                        Config.harassMenu.GetSlider("wharassmana") < _Player.ManaPercent;
            bool useE = Config.harassMenu.GetCheckBox("eharass") && E.IsReady() &&
                        Config.harassMenu.GetSlider("eharassmana") < _Player.ManaPercent;
            bool useR = Config.harassMenu.GetCheckBox("rharass") && R.IsReady() &&
                        Config.harassMenu.GetSlider("rharassmana") < _Player.ManaPercent &&
                        Config.harassMenu.GetSlider("rstacks") > CurrentRStacks();

            if (!useE && !useR && !useQ) return;
            // so like, at least one must be true
            // and because it's && (and) then the last one (useQ) always be true
            var range = useR
                ? rRange[R.Level - 1]
                : useE
                    ? E.Range
                    : Q.Range;
            // because we already checked useQ etc.
            //if (range == 0) return;
            // todo: Maybe target selector here ?
            var enemy = TargetSelector.GetTarget(range, DamageType.Magical);
            if (enemy == null || !enemy.IsValidTarget()) return;
            if (useW)
            {
                var target = TargetSelector.GetTarget(GetMyRange(true), DamageType.Mixed);
                if (target != null && target.IsValidTarget())
                    W.Cast();
            }
            // Prioritize R over E over Q, one at a time

                PredictionResult pred;
                if (useR)
                {
                    pred = R.GetPrediction(enemy);
                    if (pred.HitChance >= HitChance.High)
                    {
                        Player.CastSpell(SpellSlot.R, pred.CastPosition);
                        return;
                    }
                }

                if (useE)
                {
                    pred = E.GetPrediction(enemy);
                    if (pred.HitChance >= HitChance.High)
                    {
                        Player.CastSpell(SpellSlot.E, pred.CastPosition);
                        return;
                    }
                }

                if (useQ)
                {
                    pred = Q.GetPrediction(enemy);
                    if (pred.HitChance >= HitChance.High)
                    {
                        Player.CastSpell(SpellSlot.Q, pred.CastPosition);
                        return;
                    }
                }


            
        }
        // todo when onkillable fixed?
        /*private static void Lasthit()
        {
        }*/
    }
}