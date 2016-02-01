using System;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace SamRiven2.Orb
{
    internal static class Orbwalker
    {
        [Flags]
        public enum Modes
        {
            None = 0,
            LastHit = 1,
            Harass = 2,
            LaneClear = 4,
            JungleClear = 8,
            Combo = 16,
            Burst = 32,
            Flee = 64
        }

        private static Menu OrbMenu;
        public static long LastAutoAttack;
        public static Obj_AI_Base LastTarget;
        public static long LastMovement;
        public static Modes ActiveModes { get; private set; }
        public static Vector3 OrbwalkDestination { get; private set; }
        public static void Init()
        {
            Game.OnUpdate += Game_OnUpdate;
            MenuSetup();
        }

        public static void MenuSetup()
        {
            OrbMenu = MainMenu.AddMenu("Sam Orbwalker", "samrivenorbwalker");
            OrbMenu.Add("combo", new KeyBind("Combo", false, KeyBind.BindTypes.HoldActive, 32));
            OrbMenu.Add("burst", new KeyBind("Burst", false, KeyBind.BindTypes.HoldActive, 'T'));
            OrbMenu.Add("lasthit", new KeyBind("LastHit", false, KeyBind.BindTypes.HoldActive, 'X'));
            OrbMenu.Add("flee", new KeyBind("Flee", false, KeyBind.BindTypes.HoldActive, 'Z'));
            OrbMenu.Add("harass", new KeyBind("Harass", false, KeyBind.BindTypes.HoldActive, 'C'));
            OrbMenu.Add("laneclear", new KeyBind("LaneClear", false, KeyBind.BindTypes.HoldActive, 'V'));
            OrbMenu.Add("jungleclear", new KeyBind("JungleClear", false, KeyBind.BindTypes.HoldActive, 'V'));
            
        }

        static bool getMode(string s)
        {
            return OrbMenu[s].Cast<KeyBind>().CurrentValue;
        }
        static void Game_OnUpdate(System.EventArgs args)
        {
            UpdateMode();

        }
        
        static void UpdateMode()
        {
            Modes temp = Modes.None;
            if (getMode("combo"))
                temp = temp | Modes.Combo;
            if (getMode("burst"))
                temp = temp | Modes.Burst;
            if (getMode("flee"))
                temp = temp | Modes.Flee;
            if (getMode("harass"))
                temp = temp | Modes.Harass;
            if (getMode("jungleclear"))
                temp = temp | Modes.JungleClear;
            if (getMode("laneclear"))
                temp = temp | Modes.LaneClear;
            if (getMode("lasthit"))
                temp = temp | Modes.LastHit;
            ActiveModes = temp;
        }
    }
}