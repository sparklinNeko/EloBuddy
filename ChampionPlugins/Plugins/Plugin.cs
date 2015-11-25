using System.Configuration;

namespace ChampionPlugins.Plugins
{
    public abstract class Plugin
    {
        public static bool Loaded = false;
        public abstract void Combo();
        public abstract void Harass();
        public abstract void Flee();
        public abstract void LaneClear();
        public abstract void JungleClear();
        public abstract void LastHit();
        public abstract void Init();
        public abstract void Perma();
        public void HiddenInit()
        {
            if(Loaded) return;
            Loaded = true;
            Init();
        }
        protected Plugin()
        {
            
        }
    }
}