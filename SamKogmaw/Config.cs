using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
// ReSharper disable InconsistentNaming

namespace SamKogmaw
{
    public static class Config
    {

        public static Menu config,
            comboMenu,
            harassMenu,
            laneclearMenu,
            jungleclearMenu,
            interruptrecallMenu,
            gapcloseMenu,
            autoharassMenu,
            lasthitMenu,
            killstealMenu,
            drawMenu,
            miscMenu,
            limitMenu;

        public static bool GetCheckBox(this Menu m, string s)
        {
            return m[s].Cast<CheckBox>().CurrentValue;
        }
        public static bool GetKeyBind(this Menu m, string s)
        {
            return m[s].Cast<KeyBind>().CurrentValue;
        }
        public static int GetSlider(this Menu m, string s)
        {
            return m[s].Cast<Slider>().CurrentValue;
        }

        public static void Init()
        {
            config = MainMenu.AddMenu(Program.AssemblyName, Program.AssemblyName.ToLower());

            comboMenu = config.AddSubMenu("Combo");
            combo.Add("aapriority", new CheckBox("Prioritize Auto Attacks over Skills"));
            addSkillSheit(comboMenu, "combo", "q");
            addSkillSheit(comboMenu, "combo", "w");
            addSkillSheit(comboMenu, "combo", "e");
            addSkillSheit(comboMenu, "combo", "r");

            /*lasthitMenu = config.AddSubMenu("Lasthit");
            lasthitMenu.AddLabel("Lasthit unkillable minion with spells");
            addSkillSheit(lasthitMenu, "lasthit", "q");
            addSkillSheit(lasthitMenu, "lasthit", "w");
            addSkillSheit(lasthitMenu, "lasthit", "e");
            addSkillSheit(lasthitMenu, "lasthit", "r");*/

            harassMenu = config.AddSubMenu("Harass");
            addSkillSheit(harassMenu, "harass", "q");
            addSkillSheit(harassMenu, "harass", "e");
            addSkillSheit(harassMenu, "harass", "r");

            gapcloseMenu = config.AddSubMenu("Anti gapclose");
            gapcloseMenu.Add("antigapcloseenabled", new CheckBox("Enable antigapclose"));
            addSkillSheit(gapcloseMenu, "antigapclose", "e");
            gapcloseMenu.Add("eantigapclosedelay", new Slider("Delay before casting E", 100, 0, 500));

            laneclearMenu = config.AddSubMenu("Laneclear");
            addSkillSheit(laneclearMenu, "laneclear", "q");
            addSkillSheit(laneclearMenu, "laneclear", "w");
            addSkillSheit(laneclearMenu, "laneclear", "e");
            addSkillSheit(laneclearMenu, "laneclear", "r");

            jungleclearMenu = config.AddSubMenu("Jungleclear");
            addSkillSheit(jungleclearMenu, "jungleclear", "q");
            addSkillSheit(jungleclearMenu, "jungleclear", "w");
            addSkillSheit(jungleclearMenu, "jungleclear", "e");
            addSkillSheit(jungleclearMenu, "jungleclear", "r");

            killstealMenu = config.AddSubMenu("Killsteal");
            addSkillSheit(killstealMenu, "killsteal", "q");
            addSkillSheit(killstealMenu, "killsteal", "w");
            addSkillSheit(killstealMenu, "killsteal", "e");
            addSkillSheit(killstealMenu, "killsteal", "r");

            interruptrecallMenu = config.AddSubMenu("Interrupt Recall");
/*            addSkillSheit(interruptrecallMenu, "interruptrecall", "q");
            addSkillSheit(interruptrecallMenu, "interruptrecall", "e");*/
            addSkillSheit(interruptrecallMenu, "interruptrecall", "r");

            autoharassMenu = config.AddSubMenu("Auto harass");
            autoharassMenu.Add("autoharassenabled", new CheckBox("Enable autoharass"));
            addSkillSheit(autoharassMenu, "autoharass", "q");
            addSkillSheit(autoharassMenu, "autoharass", "e");
            addSkillSheit(autoharassMenu, "autoharass", "r");
            addChampSelector(autoharassMenu, "autoharass");

            drawMenu = config.AddSubMenu("Drawings");

            miscMenu = config.AddSubMenu("Misc");
            
           
            
            var md = miscMenu.Add("delaynum", new Slider("Number of movement delays", 1, 1, 5));
            var infoLabel = miscMenu.Add("infodisp", new Label(" "));
            md.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                Magic(limitMenu, args.OldValue, args.NewValue);
                infoLabel.CurrentValue = "Now check Movement Limit submenu!";
            };
            




            limitMenu = config.AddSubMenu("Movement Limit", "movementlimit");
            limitMenu.Add("forcelimit", new CheckBox("Limit movements even if W is not active", false));
            limitMenu.AddLabel("If any movement delay is less than your orbwalker's - the last one will be used");
            limitMenu.AddLabel("Number of this movement delays you can change at Misc");
            Magic(limitMenu, 0, md.CurrentValue);
        }

        private static void Magic(Menu m, int o, int n)
        {
            if (o <= n)
            {
                for (int i = o + 1; i <= n; i++)
                {
                    m.Add("label_"+i, new EloBuddy.SDK.Menu.Values.GroupLabel("("+i+") Movement limit"));
                    var mm = m.Add("maxas_" + i, new Slider("Limit movements after 3.000 attack speed", 3000, 0, 5000));
                    mm.DisplayName = "Limit movements after " + (((float)mm.CurrentValue) / 1000) + " attack speed";
                    mm.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        mm.DisplayName = "Limit movements after " + (((float)args.NewValue) / 1000) + " attack speed";
                    };
                    m.Add("mmd_"+i,
                        new Slider("Delay between movements when attack speed is higher than value above",
                            Orbwalker.MovementDelay, 0, 1000));
 
                    //Utils.Print("adding " + i);
                }
            }
            else
            {
                for (int i = n+1; i <= o; i++)
                {
                    m.Remove("maxas_" + i);
                    m.Remove("label_"+i);
                    m.Remove("mmd_"+i);
                    //Utils.Print("removing " + i);
                }
            }
        }

        private static void addChampSelector(Menu m, string modeString)
        {
            if(!EntityManager.Heroes.Enemies.Any()) return;
            m.AddGroupLabel("Use "+char.ToUpper(modeString[0]) + modeString.Substring(1)+" on:");
            foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies)
            {
                m.Add(enemy.ChampionName + modeString, new CheckBox(enemy.ChampionName));
            }
        }

        private static void addSkillSheit(Menu m, string modeString, string s)
        {
            m.AddGroupLabel(s.ToUpper());
            m.Add(s + modeString, new CheckBox("Use " + s.ToUpper()));
            m.Add(s + modeString + "mana", new Slider("Minimum mana to use " + s.ToUpper()+" {0}%", 80, 0, 101));
            
        }


    }
}