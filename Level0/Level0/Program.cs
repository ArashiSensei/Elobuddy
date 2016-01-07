using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using LevelZero.Core.Champions;
using LevelZero.Model;
using LevelZero.Util;
using SharpDX;

namespace LevelZero
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += GameLoaded;
        }

        private static void GameLoaded(EventArgs args)
        {
            //VersionUtil.VersionChecker();
            try
            {
                //var handle = Activator.CreateInstance(null, "LevelZero.Core.Champions." + Player.Instance.ChampionName);
                //var pluginModel = (PluginModel)handle.Unwrap();
                NotificationUtil.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, ObjectManager.Player.ChampionName + " Loaded !", Color.DeepSkyBlue));
                new Darius();

                /*
                    Anyone wants your name here ?
                */
                NotificationUtil.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, "Addon by: MrArticuno and WujuSan", Color.White));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                NotificationUtil.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, ObjectManager.Player.ChampionName + " is Not Supported", Color.Red));
            }
        }
    }
}
