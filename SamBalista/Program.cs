

using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SamBalista
{
    internal class Program
    {
        public static string AssemblyName = "SamBalista";
        private static Spell.Active R = new Spell.Active(SpellSlot.R, 1500);
        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static AIHeroClient Blitz =
            EntityManager.Heroes.Allies.FirstOrDefault(h => h.ChampionName == "Blitzcrank");
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        static void Loading_OnLoadingComplete(System.EventArgs args)
        {
            if (_Player.ChampionName != "Kalista")
            {
                Utils.Print("You must be Kalista ");
                return;
            }
            if (Blitz == null)
            {
                Utils.Print("You must have Blitzcrank in team");
                return;
            }
            
            InitMenu();
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Utils.Print("Loaded");
        }

        static void Drawing_OnEndScene(System.EventArgs args)
        {
            if (!config["drawstatus"].Cast<CheckBox>().CurrentValue || _Player.IsDead) return;
            bool active = config["pull"].Cast<KeyBind>().CurrentValue;
            var heropos = _Player.Position.WorldToScreen();
            Drawing.DrawText(heropos.X - 40, heropos.Y + 40, System.Drawing.Color.White, "Balista  (     )");
            Drawing.DrawText(heropos.X + 17, heropos.Y + 40,
                active ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red,
                active ? "On" : "Off");

        }


        static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (_Player.IsDead || args.Buff.Name != "rocketgrab2") return;
            AIHeroClient target = sender as AIHeroClient;
            if (target == null || !target.IsEnemy) return;
            var pullHim = config["pull" + target.ChampionName];
            if (pullHim == null) return;
            if (!pullHim.Cast<CheckBox>().CurrentValue || target.Distance(_Player) > 2450 || Blitz.Distance(_Player) > R.Range || !R.IsReady()) return;
            Core.DelayAction(() => Player.CastSpell(SpellSlot.R), 1);
            
        }

        private static Menu config;
        static void InitMenu()
        {
            config = MainMenu.AddMenu("Sam Balista", "sambalista");
            config.Add("pull", new KeyBind("Pull Blitz", true, KeyBind.BindTypes.PressToggle, 'T'));
            config.AddGroupLabel("Champions to pull");
            EntityManager.Heroes.Enemies.ForEach(h => 
                config.Add("pull"+h.ChampionName, new CheckBox(h.ChampionName)));
            config.AddGroupLabel("Drawings");
            config.Add("drawstatus", new CheckBox("Draw pull status"));
        }
    }
}
