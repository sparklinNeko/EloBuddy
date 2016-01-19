using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SamEzreal2
{
    public static class Config
    {
        public static Menu config,
            comboMenu,
            harassMenu,
            laneclearMenu,
            lasthitMenu;
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
            config = MainMenu.AddMenu("Sam Ezreal", "samezreal2");
            config.AddLabel("Sometimes when you hold spacebar orbwalker will stop manual E from casting,");
            config.AddLabel("this will make sure it doesn't");
            config.Add("etomouse", new KeyBind("Force E", false, KeyBind.BindTypes.HoldActive, 'E'));
            
            comboMenu = config.AddSubMenu("Combo");
            comboMenu.addSkillSheit("Q", 70, 1, 100, 15);
            comboMenu.addSkillSheit("W", 80, 1, 100, 60);

            harassMenu = config.AddSubMenu("Harass");
            harassMenu.addSkillSheit("Q", 85, 1, 100, 60);
            harassMenu.addSkillSheit("W", 85, 1, 100, 80);

            lasthitMenu = config.AddSubMenu("LastHit");
            lasthitMenu.Add("Q.Enabled", new CheckBox("Use Q"));
            lasthitMenu.Add("Q.Mana", new Slider("Min Mana to use {0}%", 60, 1, 100));

            laneclearMenu = config.AddSubMenu("LaneClear");
            laneclearMenu.Add("Q.Enabled", new CheckBox("Use Q"));
            laneclearMenu.Add("Q.Mana", new Slider("Min Mana to use {0}%", 60, 1, 100));

        }

        private static void addSkillSheit(this Menu m, string slot, int dh = 70,
            int minh = 1, int maxh = 100, int dm = 30, int minm = 1, int maxm = 100)
        {
            m.Add(slot+".Enabled", new CheckBox("Use " + slot));
            m.Add(slot + ".Hitchance", new Slider("Min Hitchance {0}%", dh, minh, maxh));
            m.Add(slot + ".Mana", new Slider("Min Mana to use {0}%", dm, minm, maxm));
        }
    }
}