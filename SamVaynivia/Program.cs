using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;

namespace SamVaynivia
{
    class Program
    {
        public static Spell.Skillshot Wall;
        public static string CondemnName;
        public static GameObject CondemnTarget;
        public static int LastCondemnTick;
        public static string VayneName;
        public static int Delay = 500;
        public static AIHeroClient Vayne;
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            // Check if our champ is Anivia
            if (Player.Instance.Hero != Champion.Anivia) return;
            // Check that Vayne is present in this game
            var vayne = EntityManager.Heroes.Allies.FirstOrDefault(h => h.ChampionName == "Vayne");
            if (vayne == null) return;
            Vayne = vayne;
            Chat.Print("Loaded");
            // Get Condemn in-game name;
            CondemnName = vayne.Spellbook.GetSpell(SpellSlot.E).Name;
            // Get Vayne in-game champion name for later use;
            VayneName = vayne.BaseSkinName;
            // Anivia W;
            Wall = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Circular, 0, int.MaxValue, 1);
            // vs OnTick
            Game.OnUpdate += Game_OnUpdate;
            // 
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            //var target = TargetSelector.SelectedTarget;
            //if(target == null) return;
            //Chat.Print("EndScene");
            //Drawing.DrawLine(Player.Instance.Position.To2D(), target.Position.To2D(), 9, Color.AntiqueWhite);

        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            // Check if it's Vayne's spell
            if (sender.BaseSkinName != VayneName) return;
            // Check if it's Condemn spell
            if (args.SData.Name != CondemnName) return;
            // Check if target is champion
            Chat.Print("Condemn");
            //if (args.Target.Type != GameObjectType.AIHeroClient) return;
            // Save info
            Chat.Print("Condemn");
            LastCondemnTick = Environment.TickCount;
            CondemnTarget = args.Target;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if(Player.Instance.IsDead) return;
            //Chat.Print("Update");
            if(LastCondemnTick + Delay > Environment.TickCount && CondemnTarget.IsValid)
                TryCombo();
        }

        private static void TryCombo()
        {
            Chat.Print("Combon");
            var wallPos = CondemnTarget.Position.To2D().Extend(Vayne.ServerPosition.To2D(), -150).To3D();
            var distance = Player.Instance.Distance(wallPos);
            Chat.Print(distance);
            var canTry = (Player.Instance.Distance(wallPos) < 1000) && Wall.IsReady();
            Chat.Print(canTry);
            if (canTry)
                Wall.Cast(wallPos);

        }
    }
}
