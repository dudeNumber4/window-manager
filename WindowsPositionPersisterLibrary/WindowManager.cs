using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WindowPositionPersisterLibrary
{

	public class WindowManager
	{

		private const string PERSISTENCE_FOLDER_NAME = "WindowManagerPersisted";
		private const string PERSISTENCE_FILE_EXT = ".dat";

		private List<WindowWrapper> _managedWindows = new List<WindowWrapper>();
		private string _persistenceFolderPath;
		private DataContractJsonSerializer _serializer;

		public WindowManager()
		{
			_serializer = new DataContractJsonSerializer(typeof(WindowWrapper));
			InitializePersistenceDirectory();
		}

		public void CacheWindows()
		{
			FlushPersistenceDirectory();
			_managedWindows.Clear();
			// This is the reason this can't run as a windows service.  That would be most handy, but it won't work.
			// https://stackoverflow.com/questions/5298351/enumdesktopwindows-in-windows-service#5298402
			Native.EnumWindows(new Native.EnumWindowsDelegate(EnumWindowsProc), IntPtr.Zero);
		}

		public void Restore()
		{
			ForEachPersistedFile( f =>
			{
				using (StreamReader sr = File.OpenText(f))
				{
					var windowManager = _serializer.ReadObject(sr.BaseStream) as WindowWrapper;
					if (windowManager != null)
					{
						Native.SetWindowPos(windowManager.Hwnd, IntPtr.Zero, windowManager.X, windowManager.Y, windowManager.Width, windowManager.Height, WindowPositionFlags.Flags);
					}
				}
			});
		}

		/// <summary>
		/// Persist what's in _managedWindows.
		/// </summary>
		public void Persist()
		{
			if (Directory.Exists(_persistenceFolderPath))
			{
				try
				{
					PersistWindowList();
				}
				catch (Exception ex)
				{
					var logPath = Path.Combine(_persistenceFolderPath, "WindowManager.log");
					using (StreamWriter s = File.CreateText(logPath))
					{
						s.Write($"Error persisting windows: {ex.Message}");
					}
				}
			}
		}

		private void PersistWindowList()
		{
			_managedWindows.ForEach(window =>
			{
				using (var ms = new MemoryStream())
				{
					_serializer.WriteObject(ms, window);
					var filePath = Path.Combine(_persistenceFolderPath, window.GetHashCode().ToString());
					filePath = Path.ChangeExtension(filePath, PERSISTENCE_FILE_EXT);
					var bytes = StreamUtils.StreamToBytes(ms);
					File.WriteAllBytes(filePath, bytes);
				}
			});
		}

		private void InitializePersistenceDirectory()
		{
			var tempDir = Path.GetTempPath();
			if (!string.IsNullOrEmpty(tempDir))
			{
				_persistenceFolderPath = Path.Combine(tempDir, PERSISTENCE_FOLDER_NAME);
				if (!Directory.Exists(_persistenceFolderPath))
				{
					try
					{
						Directory.CreateDirectory(_persistenceFolderPath);
					}
					catch (Exception)
					{
						_persistenceFolderPath = string.Empty;
					}
				}
			}
		}

		private void FlushPersistenceDirectory()
		{
			bool filesDeleted = true;
			ForEachPersistedFile( f =>
			{
				try
				{
					File.Delete(f);
				}
				catch (Exception)
				{
					filesDeleted = false;
				}
			});
			if (!filesDeleted)
			{
				// Erase folder so downstream processes short-circuit.
				_persistenceFolderPath = string.Empty;
			}
		}

		private void ForEachPersistedFile(Action<string> fileAction)
		{
			if (Directory.Exists(_persistenceFolderPath))
			{
				foreach (var f in Directory.EnumerateFiles(_persistenceFolderPath))
				{
					fileAction?.Invoke(f);
				}
			}
		}

		private bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam)
		{
			if (Native.IsWindowVisible(hwnd))
			{
				CacheVisibleWindow(hwnd);
			}
			return true;
		}

		private void CacheVisibleWindow(IntPtr hwnd)
		{
			Native.RECT rect;
			if (Native.GetWindowRect(hwnd, out rect))
			{
				_managedWindows.Add(new WindowWrapper
				{
					Hwnd = hwnd,
					X = rect.Left,
					Y = rect.Top,
					Width = rect.Right - rect.Left,
					Height = rect.Bottom - rect.Top,
					Title = GetWindowText(hwnd)
				});
			}
		}

		private static string GetWindowText(IntPtr hwnd)
		{
			try
			{
				int textLen = Native.GetWindowTextLength(hwnd);
				StringBuilder sb = new StringBuilder(textLen + 1);
				Native.GetWindowText(hwnd, sb, sb.Capacity);
				return sb.ToString();
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

	}

}
