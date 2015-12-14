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
            limitMenu,
            itemMenu;

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
            config.AddLabel("Thanks for using SamKogmaw addon, don't forget to check Movement Limit submenu");

            itemMenu = config.AddSubMenu("Items");

            itemMenu.Add("enabled", new CheckBox("Enable items usage"));
            itemMenu.AddGroupLabel("Blade Of The Ruined King");
            itemMenu.Add("botrkmyhp", new Slider("Self min HP %", 80));
            itemMenu.Add("botrkenemyhp", new Slider("Enemy min HP %", 80));


            comboMenu = config.AddSubMenu("Combo");
            comboMenu.Add("aapriority", new CheckBox("Prioritize Auto Attacks over Skills", false));
            //comboMenu.Add("tryburst", new CheckBox("Try bursting target with skills"));
            addSkillSheit(comboMenu, "combo", "q", 30);
            addSkillSheit(comboMenu, "combo", "w", 0);
            addSkillSheit(comboMenu, "combo", "e", 40);
            addSkillSheit(comboMenu, "combo", "r", 20);
            comboMenu.Add("rstacks", new Slider("Maximum R stacks", 1, 1, 9));

            /*lasthitMenu = config.AddSubMenu("Lasthit");
            lasthitMenu.AddLabel("Lasthit unkillable minion with spells");
            addSkillSheit(lasthitMenu, "lasthit", "q");
            addSkillSheit(lasthitMenu, "lasthit", "w");
            addSkillSheit(lasthitMenu, "lasthit", "e");
            addSkillSheit(lasthitMenu, "lasthit", "r");*/

            harassMenu = config.AddSubMenu("Harass");
            harassMenu.Add("enabled", new CheckBox("Enable harass"));
            addSkillSheit(harassMenu, "harass", "q");
            addSkillSheit(harassMenu, "harass", "w");
            addSkillSheit(harassMenu, "harass", "e");
            addSkillSheit(harassMenu, "harass", "r");
            harassMenu.Add("rstacks", new Slider("Maximum R stacks", 2, 1, 9));

            gapcloseMenu = config.AddSubMenu("Anti gapclose");
            gapcloseMenu.Add("antigapcloseenabled", new CheckBox("Enable antigapclose"));
            addSkillSheit(gapcloseMenu, "antigapclose", "e", 30);
            gapcloseMenu.Add("eantigapclosedelay", new Slider("Delay before casting E", 100, 0, 500));

            laneclearMenu = config.AddSubMenu("Laneclear");
            laneclearMenu.Add("enabled", new CheckBox("Enable laneclear"));
            laneclearMenu.Add("minions", new Slider("Minimum number of minions to use spells", 3, 1, 20));
            addSkillSheit(laneclearMenu, "laneclear", "q", 40);
            addSkillSheit(laneclearMenu, "laneclear", "w", 30);
            addSkillSheit(laneclearMenu, "laneclear", "e", 60);
            addSkillSheit(laneclearMenu, "laneclear", "r", 60);
            laneclearMenu.Add("rstacks", new Slider("Maximum R stacks", 1, 1, 9));

            jungleclearMenu = config.AddSubMenu("Jungleclear");
            jungleclearMenu.Add("enabled", new CheckBox("Enable jungleclear"));
            jungleclearMenu.Add("minions", new Slider("Minimum number of minions to use spells", 3, 1, 20));
            addSkillSheit(jungleclearMenu, "jungleclear", "q");
            addSkillSheit(jungleclearMenu, "jungleclear", "w");
            addSkillSheit(jungleclearMenu, "jungleclear", "e");
            addSkillSheit(jungleclearMenu, "jungleclear", "r");

            killstealMenu = config.AddSubMenu("Killsteal");
            killstealMenu.Add("enabled", new CheckBox("Enable killsteal"));
            addSkillSheit(killstealMenu, "killsteal", "q", 20);
            addSkillSheit(killstealMenu, "killsteal", "e", 20);
            addSkillSheit(killstealMenu, "killsteal", "r", 20);
            killstealMenu.Add("rstacks", new Slider("Maximum R stacks", 2, 1, 9));

            interruptrecallMenu = config.AddSubMenu("Interrupt Recall");
            interruptrecallMenu.Add("enabled", new CheckBox("Enable recall interrupter"));
/*            addSkillSheit(interruptrecallMenu, "interruptrecall", "q");
            addSkillSheit(interruptrecallMenu, "interruptrecall", "e");*/
            addSkillSheit(interruptrecallMenu, "interruptrecall", "r");

            autoharassMenu = config.AddSubMenu("Auto harass");
            autoharassMenu.Add("autoharassenabled", new CheckBox("Enable autoharass", false));
            autoharassMenu.Add("onlyimmobile", new CheckBox("Autoharass only immobile targets", true));
            addSkillSheit(autoharassMenu, "autoharass", "q");
            addSkillSheit(autoharassMenu, "autoharass", "e");
            addSkillSheit(autoharassMenu, "autoharass", "r");
            autoharassMenu.Add("rstacks", new Slider("Maximum R stacks", 1, 1, 9));
            addChampSelector(autoharassMenu, "autoharass");

            drawMenu = config.AddSubMenu("Drawings");
            drawMenu.Add("enabled", new CheckBox("Enable drawings"));
            drawMenu.Add("drawready", new CheckBox("Draw only ready spells"));
            drawMenu.Add("drawq", new CheckBox("Draw Q range"));
            drawMenu.Add("draww", new CheckBox("Draw W range"));
            drawMenu.Add("drawe", new CheckBox("Draw E range"));
            drawMenu.Add("drawr", new CheckBox("Draw R range"));
            drawMenu.Add("drawpd", new CheckBox("Draw predicted damage range"));

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
                            500, 0, 2000));
 
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

        private static void addSkillSheit(Menu m, string modeString, string s, int def = 80)
        {
            m.AddGroupLabel(s.ToUpper());
            m.Add(s + modeString, new CheckBox("Use " + s.ToUpper()));
            m.Add(s + modeString + "mana", new Slider("Minimum mana to use " + s.ToUpper()+" {0}%", def, 0, 101));
            
        }


    }
}