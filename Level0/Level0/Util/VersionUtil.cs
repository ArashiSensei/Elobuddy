using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EloBuddy;
using LevelZero.Model;
using SharpDX;
using Version = System.Version;

namespace LevelZero.Util
{
    static class VersionUtil
    {
        public static void VersionChecker()
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
                        var CorrectVersion = new Version(string.Format("{0}.{1}.{2}.{3}", Match.Groups[1], Match.Groups[2], Match.Groups[3], Match.Groups[4]));

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
        }

    }
}
