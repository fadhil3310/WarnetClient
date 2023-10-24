using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static PInvoke.User32;
using static PInvoke.DwmApi;
using System.Windows.Automation;

namespace Overlay
{
		internal class WindowEnumeration
		{
				private WNDENUMPROC enumWinsProc;

				[DllImport("user32.dll")]
				[return: MarshalAs(UnmanagedType.Bool)]
				public static extern bool EnumChildWindows(IntPtr hwndParent, WNDENUMPROC lpEnumFunc, IntPtr lParam);

				enum GetAncestorFlags
				{
						// Retrieves the parent window. This does not include the owner, as it does with the GetParent function.
						GetParent = 1,
						// Retrieves the root window by walking the chain of parent windows.
						GetRoot = 2,
						// Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.
						GetRootOwner = 3
				}

				public enum GWL
				{
						GWL_WNDPROC = (-4),
						GWL_HINSTANCE = (-6),
						GWL_HWNDPARENT = (-8),
						GWL_STYLE = (-16),
						GWL_EXSTYLE = (-20),
						GWL_USERDATA = (-21),
						GWL_ID = (-12)
				}

				[Flags]
				private enum WindowStyles : uint
				{
						WS_BORDER = 0x800000,
						WS_CAPTION = 0xc00000,
						WS_CHILD = 0x40000000,
						WS_CLIPCHILDREN = 0x2000000,
						WS_CLIPSIBLINGS = 0x4000000,
						WS_DISABLED = 0x8000000,
						WS_DLGFRAME = 0x400000,
						WS_GROUP = 0x20000,
						WS_HSCROLL = 0x100000,
						WS_MAXIMIZE = 0x1000000,
						WS_MAXIMIZEBOX = 0x10000,
						WS_MINIMIZE = 0x20000000,
						WS_MINIMIZEBOX = 0x20000,
						WS_OVERLAPPED = 0x0,
						WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
						WS_POPUP = 0x80000000u,
						WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
						WS_SIZEFRAME = 0x40000,
						WS_SYSMENU = 0x80000,
						WS_TABSTOP = 0x10000,
						WS_VISIBLE = 0x10000000,
						WS_VSCROLL = 0x200000
				}

				enum DWMWINDOWATTRIBUTE : uint
				{
						NCRenderingEnabled = 1,
						NCRenderingPolicy,
						TransitionsForceDisabled,
						AllowNCPaint,
						CaptionButtonBounds,
						NonClientRtlLayout,
						ForceIconicRepresentation,
						Flip3DPolicy,
						ExtendedFrameBounds,
						HasIconicBitmap,
						DisallowPeek,
						ExcludedFromPeek,
						Cloak,
						Cloaked,
						FreezeRepresentation
				}

				[DllImport("user32.dll")]
				static extern IntPtr GetShellWindow();

				[DllImport("user32.dll")]
				[return: MarshalAs(UnmanagedType.Bool)]
				static extern bool IsWindowVisible(IntPtr hWnd);

				[DllImport("user32.dll", ExactSpelling = true)]
				static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags flags);

				[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
				static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

				[DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
				static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

				// This static method is required because Win32 does not support
				// GetWindowLongPtr directly.
				// http://pinvoke.net/default.aspx/user32/GetWindowLong.html
				static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
				{
						if (IntPtr.Size == 8)
								return GetWindowLongPtr64(hWnd, nIndex);
						else
								return GetWindowLongPtr32(hWnd, nIndex);
				}

				[DllImport("dwmapi.dll")]
				static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out bool pvAttribute, int cbAttribute);

				static public List<WindowInfo> GetVisibleTopLevelWindows()
				{
						List<WindowInfo> windowList = new List<WindowInfo>();

						var topWindow = GetTopWindow(GetDesktopWindow());
						var hWnd = GetWindow(topWindow, GetWindowCommands.GW_HWNDLAST);

						do
						{
								if (IsWindowVisible(hWnd) && !IsIconic(hWnd)) //(IsWindowVisible(hWnd) && !IsZoomed(hWnd))
								{
										try
										{
												string name = GetWindowText(hWnd);
												string className = GetClassName(hWnd);

												//System.Windows.MessageBox.Show("name " + name + " class " + className);

												/*RECT windowRect;
												GetWindowRect(hWnd, out windowRect);*/

												windowList.Add(new WindowInfo(
														hWnd,
														name,
														className,
														/*new Rect(
																windowRect.left,
																windowRect.top,
																windowRect.right - windowRect.left,
																windowRect.bottom - windowRect.top
																),*/
														true,
														IsZoomed(hWnd)));



												//System.Windows.MessageBox.Show("Window " + window + " name " + name + " class " + className);
										}
										catch (Exception ex)
										{
												System.Windows.MessageBox.Show(ex.ToString(), "Error while trying to enumerating windows");
										}

										
								}
								hWnd = GetWindow(hWnd, GetWindowCommands.GW_HWNDPREV);
						} while (hWnd != topWindow);

						/*EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
						{
								if (IsWindowVisible(hWnd) && !IsIconic(hWnd)) //(IsWindowVisible(hWnd) && !IsZoomed(hWnd))
								{
										try
										{
												string name = GetWindowText(hWnd);
												string className = GetClassName(hWnd);

												RECT windowRect;
												GetWindowRect(hWnd, out windowRect);

												windowList.Add(new WindowInfo(
														hWnd,
														name,
														className,
														new Rect(
																windowRect.left,
																windowRect.top,
																windowRect.right - windowRect.left,
																windowRect.bottom - windowRect.top
																),
														true));


												//System.Windows.MessageBox.Show("Window " + hWnd + " name " + name + " class " + className);
										}
										catch (Exception ex)
										{
												System.Windows.MessageBox.Show(ex.ToString(), "Error while trying to enumerating windows");
										}
								}

								return true;
						}, (IntPtr)0);*/

						return windowList;
				}

				public static void GetAllChildWindows()
				{
						List<WindowInfo> windows = new List<WindowInfo>();

						bool Abc(IntPtr hWnd, IntPtr lparam)
						{
								bool check = true;
								check = IsWindowVisible(hWnd);

								if (check) //(IsWindowVisible(hWnd) && !IsZoomed(hWnd))
								{
										try
										{
												string name = GetWindowText(hWnd);
												string className = GetClassName(hWnd);

												RECT windowRect;
												GetWindowRect(hWnd, out windowRect);

												windows.Add(new WindowInfo(
													hWnd,
													name,
													className,
													/*new Rect(
															windowRect.left,
															windowRect.top,
															windowRect.right - windowRect.left,
															windowRect.bottom - windowRect.top
															),*/
													true,
													false));

												EnumChildWindows(hWnd, Abc, hWnd);
										}
										catch (Exception ex)
										{
												System.Windows.MessageBox.Show(ex.ToString(), "Error while trying to enumerating windows");
										}
								}

								return true;
						}

						EnumWindows(Abc, (IntPtr)0);
				}

				// Adapted from WindowCaptureSample
				public static bool IsWindowValidForCapture(IntPtr hwnd)
				{
						if (hwnd.ToInt32() == 0)
						{
								return false;
						}

						if (hwnd == GetShellWindow())
						{
								return false;
						}

						if (!IsWindowVisible(hwnd))
						{
								return false;
						}

						if (GetAncestor(hwnd, GetAncestorFlags.GetRoot) != hwnd)
						{
								return false;
						}

						var style = (WindowStyles)(uint)GetWindowLongPtr(hwnd, (int)GWL.GWL_STYLE).ToInt64();
						if (style.HasFlag(WindowStyles.WS_DISABLED))
						{
								return false;
						}

						var cloaked = false;
						var hrTemp = DwmGetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.Cloaked, out cloaked, Marshal.SizeOf<bool>());
						if (hrTemp == 0 && cloaked)
						{
								return false;
						}

						return true;
				}

				/*static private bool EnumerateWindows(Action<WindowInfo> onGetWindow)
				{
						//EnumWindows(EnumerateWindows, (IntPtr)0);
						bool check = true;
						if (lparam.ToInt32() == 0)
								check = IsWindowVisible(hWnd) && !IsZoomed(hWnd);

						if (check) //(IsWindowVisible(hWnd) && !IsZoomed(hWnd))
						{
								try
								{
										string name = GetWindowText(hWnd);
										string className = GetClassName(hWnd);



										//EnumChildWindows(hWnd, enumWinsProc, hWnd);
								}
								catch (Exception ex)
								{
										System.Windows.MessageBox.Show(ex.ToString(), "Error while trying to enumerating windows");
								}
						}

						return true;
				}*/
		}
}

class WindowInfo
{
		public IntPtr Handle { get; set; }
		public string Name { get; set; }
		public string ClassName { get; set; }
		//public Rect WindowRect { get; set; }
		public bool IsVisible { get; set; }
		public bool IsMaximized { get; set; }

		public WindowInfo(IntPtr handle, string name, string className, /*Rect windowRect,*/ bool isVisible, bool isMaximized)
		{
				Handle = handle;
				Name = name;
				ClassName = className;
				//WindowRect = windowRect;
				IsVisible = isVisible;
				IsMaximized = isMaximized;
		}
}
