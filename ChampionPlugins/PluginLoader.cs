using System;
using System.Collections.Generic;
using System.Configuration;
using ChampionPlugins.Plugins;
using EloBuddy;
using EloBuddy.SDK;

namespace ChampionPlugins
{
    public static class PluginLoader
    {
        public static bool Loaded = false;
        private static string pluginName = "";
        public static Plugin ActivePlugin;
        public static Dictionary<string, Plugin> Plugins = new Dictionary<string, Plugin>()
        {
            //add them here
            {"Sivir", new Sivir()},
            {"Tristana", new Tristana()},
            {"Ashe", new Ashe()},
        };

        public static void Init()
        {
            if (Loaded) return;
            Loaded = true;
            if (Plugins.ContainsKey(ObjectManager.Player.ChampionName))
            {
                Chat.Print("<font color='#33ff44'>Champion plugin </font><font color='#f0f9f9'>" + ObjectManager.Player.ChampionName + "</font><font color='#33ff44'> is loaded</font>");
                ActivePlugin = Plugins[ObjectManager.Player.ChampionName];
            }
            else
            {
                // while we don't have generic plugin
                Loaded = false;
                // todo try to load generic plugins for ad and ap
                Chat.Print("<font color='#ff3344'>Champion </font><font color='#f0f9f9'>"+ObjectManager.Player.ChampionName+"</font><font color='#ff3344'> is not supported</font>");
                pluginName = "Generic";
            }
            if (!Loaded) return;
            ActivePlugin.HiddenInit();
            Game.OnUpdate += Game_OnUpdate;
        }

        static void Game_OnUpdate(EventArgs args)
        {
            ActivePlugin.Perma();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                ActivePlugin.Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                ActivePlugin.Harass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                ActivePlugin.LaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                ActivePlugin.LastHit();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                ActivePlugin.Flee();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                ActivePlugin.JungleClear();
        }

    }
}