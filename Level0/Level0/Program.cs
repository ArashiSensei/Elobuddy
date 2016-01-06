using System;
using System.Net;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using EloBuddy;
using EloBuddy.SDK.Events;
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
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var AssVersion = Assembly.GetExecutingAssembly().GetName().Version;

                    string Text = new WebClient().DownloadString("https://raw.githubusercontent.com/mrarticuno/Elobuddy/master/Level0/Level0/Properties/AssemblyInfo.cs");

                    var Match = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match(Text);

                    if (Match.Success)
                    {
                        var CorrectVersion = new System.Version(string.Format("{0}.{1}.{2}.{3}", Match.Groups[1], Match.Groups[2], Match.Groups[3], Match.Groups[4]));

                        if (CorrectVersion > AssVersion)
                        {
                            Chat.Print("Your AIO is OUTDATED, update it");
                            NotificationUtil.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, "Your AIO version is OUTDATED, current version is " + CorrectVersion, Color.Yellow));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "\n [ [RIP] Search ]");
                }
            });

            try
            {
                var handle = Activator.CreateInstance(null, "LevelZero.Core.Champions." + Player.Instance.ChampionName);
                var pluginModel = (PluginModel)handle.Unwrap();
                NotificationUtil.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, ObjectManager.Player.ChampionName + " Loaded !", Color.DeepSkyBlue));

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
