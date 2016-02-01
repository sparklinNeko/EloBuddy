using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SamRiven2
{
    public static class Config
    {
        public static int Q1Delay
        {
            get { return delayMenu["q1delay"].Cast<Slider>().CurrentValue; }
        }
        public static int Q2Delay
        {
            get { return delayMenu["q2delay"].Cast<Slider>().CurrentValue; }
        }
        public static int Q3Delay
        {
            get { return delayMenu["q3delay"].Cast<Slider>().CurrentValue; }
        }
        public static int WDelay
        {
            get { return delayMenu["wdelay"].Cast<Slider>().CurrentValue; }
        }
        public static bool ForceR
        {
            get { return delayMenu["rforce"].Cast<KeyBind>().CurrentValue; }
        }
        public static bool BurstMode
        {
            get { return delayMenu["burst"].Cast<KeyBind>().CurrentValue; }
        }
        public static bool AAFirst
        {
            get { return config["aafirst"].Cast<CheckBox>().CurrentValue; }
        }
        private static Menu config, delayMenu;
        public static void Init()
        {
            config = MainMenu.AddMenu(Program.AssemblyName, Program.AssemblyName.ToLower());
            config.Add("aafirst", new CheckBox("AA before Q1"));
            delayMenu = config.AddSubMenu("Delays");
            delayMenu.Add("q1delay", new Slider("Q1 animation reset delay {0}ms default 293", 291, 0, 500));
            delayMenu.Add("q2delay", new Slider("Q2 animation reset delay {0}ms default 293", 291, 0, 500));
            delayMenu.Add("q3delay", new Slider("Q3 animation reset delay {0}ms default 393", 393, 0, 500));
            delayMenu.Add("wdelay", new Slider("W animation reset delay {0}ms default 170", 170, 0, 500));
            delayMenu.Add("rforce", new KeyBind("Force R", false, KeyBind.BindTypes.PressToggle, 'G'));
            delayMenu.Add("burst", new KeyBind("Burst", false, KeyBind.BindTypes.HoldActive, 'T'));
        }
    }
}