using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WarnetClient.Event;
using WarnetClient.Overlays;

namespace WarnetClient
{
		/// <summary>
		/// Interaction logic for MainWindow.xaml
		/// </summary>
		public partial class MainWindow : Window
		{
				ClientService service;

				OverlayWindow overlayWindow;
				FindingServerWindow findingServerWindow = null;
				FailedFindingServerWindow failedFindingServerWindow = null;
				NoTimeLeftWindow noTimeLeftWindow = null;
				BillRevokedWindow billRevokedWindow = null;

				public MainWindow()
				{
						InitializeComponent();

						// Make the window fill the screen
						Left = 0;
						Top = 0;
						Width = SystemParameters.PrimaryScreenWidth;
						Height = SystemParameters.PrimaryScreenHeight;

						service = ClientService.Instance;
						service.Connecting += Service_Connecting;
						service.FailedConnecting += Service_FailedConnecting;
						service.Connected += Service_Connected;
						service.BillAdded += Service_BillAdded;
						service.BillRevoked += Service_BillRevoked;
						service.OnDebug += Service_OnDebug;

						overlayWindow = new OverlayWindow();
						overlayWindow.OnTimeIsUp += OverlayWindow_OnTimeIsUp;
						overlayWindow.Closed += delegate { Close(); };

						// Start the communication service
						service.Initialize();

						// Tell the user in the Welcome Screen our Computer IP Address.
						IPAddressText.Text = "Alamat IP: " + Utility.GetIPAdress();
				}

				/// <summary>
				/// Used for writing debug message.
				/// Not used for now. Will be removed later.
				/// </summary>
				/// <param name="text">Debug message</param>
				private void WriteDebug(string text)
				{
						DebugListBox.Dispatcher.Invoke(() => DebugListBox.Items.Add(text));
				}

				/// <summary>
				/// Welcome screen is shown after the Client has successfully connected to the Server
				/// and the Computer hasn't been leased by anyone.
				/// Use this to show the welcome screen.
				/// </summary>
				private void ShowWelcomeScreen()
				{
						// Make the window visible first
						Show();

						var opacityAnim = new DoubleAnimation();
						opacityAnim.To = 1;
						opacityAnim.Duration = TimeSpan.FromMilliseconds(1000);
						opacityAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };

						var storyboard = new Storyboard();
						storyboard.Children.Add(opacityAnim);
						Storyboard.SetTargetName(opacityAnim, RootWindow.Name);
						Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(Window.OpacityProperty));

						storyboard.Begin(this);

						//BackgroundVideo.Play();
				}

				/// <summary>
				/// Welcome screen is hidden after the Computer has been leased for the first time.
				/// Use this to hide the welcome screen.
				/// </summary>
				private void HideWelcomeScreen()
				{
						var opacityAnim = new DoubleAnimation();
						opacityAnim.To = 0;
						opacityAnim.Duration = TimeSpan.FromMilliseconds(1000);
						opacityAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };


						var storyboard = new Storyboard();
						storyboard.Children.Add(opacityAnim);
						Storyboard.SetTargetName(opacityAnim, RootWindow.Name);
						Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(Window.OpacityProperty));

						storyboard.Begin(this);

						// Wait until the animation finished before hiding this window.
						// The animation will play for 1 second.
						Task.Run(() =>
						{
								Thread.Sleep(1000);
								RootWindow.Dispatcher.Invoke(() =>
								{
										Hide();
								});
						});
				}

				// Stop all communication before closing
				public new void Close()
				{
						service.Dispose();
						base.Close();
				}

				// ===== Events =====

				private void Service_OnDebug(object sender, DebugEventArgs e)
				{
						WriteDebug("Debug: " + e.Text);
				}

				private void Service_Connecting(object sender, EventArgs e)
				{
						RootWindow.Dispatcher.Invoke(() =>
						{
								// Close FailedFindingServer window first
								// if the window is still visible to the user.
								if (failedFindingServerWindow != null)
								{
										failedFindingServerWindow.Close();
										failedFindingServerWindow = null;
								}
								
								findingServerWindow = new FindingServerWindow();
								findingServerWindow.Show();
						});
				}

				private void Service_FailedConnecting(object sender, ErrorEventArgs e)
				{
						RootWindow.Dispatcher.Invoke(() =>
						{
								// Close FindingServer window first
								// if the window is visible to the user.
								if (findingServerWindow != null)
								{
										findingServerWindow.Close();
										findingServerWindow = null;
								}		

								failedFindingServerWindow = new FailedFindingServerWindow();
								failedFindingServerWindow.Show(e.Reason);
						});
				}

				private void Service_Connected(object sender, EventArgs e)
				{
						RootWindow.Dispatcher.Invoke(() =>
						{
								// Close FindingServer window first
								// if the window is visible to the user.
								if (findingServerWindow != null)
								{
										findingServerWindow.Close();
										findingServerWindow = null;
								}

								ShowWelcomeScreen();
						});
				}

				private void Service_BillAdded(object sender, AddBillEventArgs e)
				{
						RootWindow.Dispatcher.Invoke(() =>
						{
								HideWelcomeScreen();

								// Close the BillRevoked window
								// if the window is visible to the user
								if (billRevokedWindow != null)
								{
										billRevokedWindow.Close();
										billRevokedWindow.Closed += delegate
										{
												billRevokedWindow = null;
												StartCounting();
										};
								}
								else
								{
										// Close the NoTimeLeft window
										// if the window is visible to the user
										if (noTimeLeftWindow != null)
										{
												noTimeLeftWindow.Close();
												noTimeLeftWindow.Closed += delegate
												{
														noTimeLeftWindow = null;
														StartCounting();
												};
										}
										else
										{
												StartCounting();
										}
								}

								void StartCounting()
								{
										overlayWindow.Show();
										overlayWindow.StartCounting(e.Seconds);
								}
						});
				}

				private void Service_BillRevoked(object sender, EventArgs e)
				{
						RootWindow.Dispatcher.Invoke(() =>
						{
								overlayWindow.StopCounting();

								if (billRevokedWindow == null)
								{
										billRevokedWindow = new BillRevokedWindow();
										billRevokedWindow.Show();
								}
						});
				}

				private void OverlayWindow_OnTimeIsUp(object sender, EventArgs e)
				{
						RootWindow.Dispatcher.Invoke(() =>
						{
								if (noTimeLeftWindow == null)
								{
										noTimeLeftWindow = new NoTimeLeftWindow();
										noTimeLeftWindow.Show();
								}
						});
				}
		}
}
