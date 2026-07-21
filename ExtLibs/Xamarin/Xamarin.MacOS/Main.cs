using AppKit;
using System;

namespace Xamarin.MacOS
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            if (Array.IndexOf(args, "--startup-resource-smoke-test") >= 0)
            {
                try
                {
                    new System.Drawing.android.android();
                    MissionPlanner.Program.RunStartupResourceSmokeTest();

                    Console.WriteLine("Mission Planner startup resource smoke test passed.");
                    return;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Mission Planner startup resource smoke test failed: " + ex);
                    Environment.ExitCode = 1;
                    return;
                }
            }

            NSApplication.Init();
            NSApplication.SharedApplication.Delegate = new AppDelegate();
            NSApplication.Main(args);
        }
    }
}
