﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace SamKogmaw
{
    internal class Program
    {
        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static readonly string AssemblyName = "Sam Kogmaw";
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }
        public static bool LagFree(int offset)
        {
            if (tickIndex == offset)
                return true;
            else
                return false;
        }
        static void Loading_OnLoadingComplete(EventArgs args)
        {
            Orbwalker.DisableMovement = false;
            Hacks.RenderWatermark = false;
            if (_Player.Hero != Champion.KogMaw) return;
            Config.Init();
            Kogmaw.Init();
            Game.OnUpdate += Game_OnUpdate;
        }
        private static int tickNum = 7, tickIndex = 0;
        static void Game_OnUpdate(EventArgs args)
        {
            tickIndex++;

            if (tickIndex >= tickNum)
                tickIndex = 0;
            
        }
    }
}
