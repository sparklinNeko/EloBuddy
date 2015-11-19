
using System;
using System.Collections.Generic;
using System.Drawing;
using AutoSharp.Plugins;
using EloBuddy;

namespace AutoSharp
{
    public class PluginLoader
    {
        private static bool _loaded;
        public static Dictionary<string, Action> Plugins = new Dictionary<string, Action>()
        {
            //add them here
            {"Sivir", Sivir.Init},
        };
        public PluginLoader()
        {
            if (!_loaded)
            {
                string def = "default";
                if (Plugins.ContainsKey(ObjectManager.Player.ChampionName))
                {
                    Plugins[ObjectManager.Player.ChampionName]();
                    _loaded = true;
                    Chat.Print(Player.Instance.ChampionName+" plugin loaded.", Color.CornflowerBlue);
                }
                else
                {
                    Chat.Print(Player.Instance.ChampionName + " plugin not found.", Color.OrangeRed);   
                }
                
                
            }
        }

    }
}
