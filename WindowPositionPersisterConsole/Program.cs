using System;
using System.Linq;
using WindowPositionPersisterLibrary;

namespace WindowPositionPersisterConsole
{

	/// <summary>
	/// Console app for testing.
	/// Thanks to https://git.brianparks.me/bparks/wm
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			var windowManager = new WindowManager();
			if (args.Length > 0 && args[0].Contains("persist"))
			{
				windowManager.CacheWindows();
				windowManager.Persist();
			}
			else if (args.Length > 0 && args[0].Contains("restore"))
			{
				windowManager.Restore();
			}

			//SystemEvents.SessionSwitch += (sender, e) =>
			//{
			//	if (e.Reason == SessionSwitchReason.SessionLock)
			//	{
			//		windowManager.CacheWindows();
			//		windowManager.Persist();
			//	}
			//	if (e.Reason == SessionSwitchReason.SessionUnlock)
			//	{
			//		windowManager.Restore();
			//	}
			//};

			//do
			//{
			//	Thread.Sleep(int.MaxValue);
			//} while (true);
		}
	}
}
