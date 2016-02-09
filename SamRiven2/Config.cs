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
            get { return config["rforce"].Cast<KeyBind>().CurrentValue; }
        }
        public static bool BurstMode
        {
            get { return config["burst"].Cast<KeyBind>().CurrentValue; }
        }
        public static bool AAFirst
        {
            get { return config["aafirst"].Cast<CheckBox>().CurrentValue; }
        }
        public static bool AlwaysCancel
        {
            get { return config["alwayscancel"].Cast<CheckBox>().CurrentValue; }
        }
        public static int QGapclose
        {
            get { return config["qgapclose"].Cast<Slider>().CurrentValue; }
        }
        private static Menu config, delayMenu;
        public static void Init()
        {
            config = MainMenu.AddMenu(Program.AssemblyName, Program.AssemblyName.ToLower());
            config.Add("aafirst", new CheckBox("AA before Q1"));
            config.Add("alwayscancel", new CheckBox("Cancel animation from manual Qs"));
            config.Add("qgapclose", new Slider("Gaplose with {0}Q", 0, 0, 3));
            
            config.AddLabel("This one will enable gapclosing with Qs.");
            config.AddLabel("0 means it turned off, 1 - it will use Q1, 2 - Q1 and Q2, 3 - Q1,Q2,Q3");
            config.AddSeparator();
            delayMenu = config.AddSubMenu("Delays");
            delayMenu.AddLabel("It takes ping into calculations now");
            config.AddSeparator();
            delayMenu.Add("q1delay", new Slider("Q1 animation reset delay {0}ms default 293", 291, 0, 500));
            delayMenu.Add("q2delay", new Slider("Q2 animation reset delay {0}ms default 293", 291, 0, 500));
            delayMenu.Add("q3delay", new Slider("Q3 animation reset delay {0}ms default 393", 393, 0, 500));
            delayMenu.Add("wdelay", new Slider("W animation reset delay {0}ms default 170", 170, 0, 500));
            config.Add("rforce", new KeyBind("Force R", false, KeyBind.BindTypes.PressToggle, 'G'));
            config.Add("burst", new KeyBind("Burst", false, KeyBind.BindTypes.HoldActive, 'T'));
        }
    }
}