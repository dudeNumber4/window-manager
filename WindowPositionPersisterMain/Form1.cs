using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowPositionPersisterLibrary;

namespace WindowPositionPersisterMain
{
	public partial class Form1 : Form
	{
		
		private WindowManager _windowManager = new WindowManager();

		public Form1()
		{
			// This only seems to work when it's called right here.
			SystemEvents.SessionSwitch += (s, eventArgs) =>
			{
				if (eventArgs.Reason == SessionSwitchReason.SessionLock)
				{
					_windowManager.CacheWindows();
					_windowManager.Persist();
				}
				if (eventArgs.Reason == SessionSwitchReason.SessionUnlock)
				{
					// Doesn't work when called directly at the session unlock point.
					Thread.Sleep(5000);
					_windowManager.Restore();
				}
			};
			InitializeComponent();
		}

		/// <summary>
		/// Turn this into a non-visible app (see WindowManager.CacheWindows() for why can't run as service).
		/// </summary>
		/// <param name="value"></param>
		protected override void SetVisibleCore(bool value)
		{
			if (!IsHandleCreated)
			{
				value = false;
				CreateHandle();
			}
			base.SetVisibleCore(value);
		}

	}
}
