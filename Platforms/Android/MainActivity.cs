using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.Maui;

namespace Workout_Tracker
{
    [Activity(Theme = "@style/Maui.SplashTheme",
              MainLauncher = true,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();
            }
            catch (Exception ex) when (ex is ObjectDisposedException || ex is JavaProxyThrowable || (ex.InnerException is ObjectDisposedException))
            {
                // Host already disposed during teardown — swallow to avoid crash.
                // Consider fixing host/service lifetimes instead of relying on this guard.
            }
        }
    }
}