using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PInvoke.User32;
using static PInvoke.Kernel32;

namespace WarnetClient.Helper
{
		/// <summary>
		/// A service used to reject all keyboard input
		/// </summary>
		internal class KeyboardLockService
		{
				private static readonly KeyboardLockService instance = new KeyboardLockService();
				public static KeyboardLockService Instance { get { return instance; } }
				
				// Keyboard input information.
				// Reference: https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-kbdllhookstruct
				struct KBDLHOOKSTRUCT
				{
						public int vkCode;
						public int scanCode;
						public int flags;
						public int time;
						public IntPtr extra;
				}

				// Keyboard messages.
				// Reference: https://learn.microsoft.com/en-us/windows/win32/inputdev/keyboard-input-notifications
				const int WM_KEYDOWN = 0x0100;
				const int WM_KEYUP = 0x0101;
				const int WM_SYSKEYDOWN = 0x0104;
				const int WM_SYSKEYUP = 0x0105;

				private WindowsHookDelegate keyHookProc;
				private SafeHookHandle hHook;
				private delegate void delegateCallback();

				// Because "PInvoke" doesn't provide interop function for UnhookWindowsHookEx
				// we should type the function defintion by ourselves
				[DllImport("user32.dll", SetLastError = true)]
				static extern bool UnhookWindowsHookEx(IntPtr hhk);

				/// <summary>
				/// Start the service. Any key that the user inputted will be rejected.
				/// </summary>
				public void Start()
				{
						if (keyHookProc == null || hHook == null)
						{
								keyHookProc = KeyHookCallback;
								hHook = SetWindowsHookEx(WindowsHookType.WH_KEYBOARD_LL, keyHookProc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
						}
				}

				/// <summary>
				/// Callback to proccess all user keyboard input.
				/// </summary>
				private int KeyHookCallback(int code, IntPtr param, IntPtr lparam)
				{
						if (code < 0)
								return CallNextHookEx(hHook.DangerousGetHandle(), code, param, lparam);

						// Cast the keyboard event information pointer to C# structure
						// so that we can use it in our code
						KBDLHOOKSTRUCT info = (KBDLHOOKSTRUCT)Marshal.PtrToStructure(lparam, typeof(KBDLHOOKSTRUCT));

						bool rejectKey = false;

						// Reject all user keyboard input
						switch (param.ToInt32())
						{
								case WM_KEYDOWN:
										rejectKey = true;
										break;
								case WM_KEYUP:
										rejectKey = true;
										break;
								case WM_SYSKEYDOWN:
										rejectKey = true;
										break;
								case WM_SYSKEYUP:
										rejectKey = true;
										break;
						}

						if (rejectKey) return 1;
						else return CallNextHookEx(hHook.DangerousGetHandle(), code, param, lparam);
				}

				/// <summary>
				/// Stop the service.
				/// </summary>
				public void Stop()
				{
						if (hHook != null)
						{
								UnhookWindowsHookEx(hHook.DangerousGetHandle());
						}
				}
		}
}
