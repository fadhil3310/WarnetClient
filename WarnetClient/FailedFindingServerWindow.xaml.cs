using GI.Screenshot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace WarnetClient
{
		/// <summary>
		/// Interaction logic for FailedFindingServerWindow.xaml
		/// </summary>
		public partial class FailedFindingServerWindow : Window
		{
				public FailedFindingServerWindow()
				{
						InitializeComponent();

						Width = SystemParameters.PrimaryScreenWidth;
						Height = SystemParameters.PrimaryScreenHeight;

						RegisterName("OverlayRoot_Scale", OverlayRoot_Scale);
						RegisterName("IPAddressTextBox_Translate", IPAddressTextBox_Translate);

						Task.Run(() =>
						{
								Thread.Sleep(500);
								//Dispatcher.Invoke(() => animateIntroScreen());
						});
				}

				public new void Show(string errorMessage)
				{
						base.Show();
						ErrorMessageExpander.Visibility = Visibility.Visible;
						ErrorMessageTextBlock.Text = errorMessage;
				}

				public new void Close()
				{
						RootWindow.Dispatcher.Invoke(() =>
						{
								var scaleAnimX = new DoubleAnimation();
								scaleAnimX.To = 1.4;
								scaleAnimX.Duration = TimeSpan.FromMilliseconds(250);
								scaleAnimX.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn };
								var scaleAnimY = new DoubleAnimation();
								scaleAnimY.To = 1.4;
								scaleAnimY.Duration = TimeSpan.FromMilliseconds(250);
								scaleAnimY.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn };

								var overlayOpacityAnim = new DoubleAnimation();
								overlayOpacityAnim.To = 0;
								overlayOpacityAnim.Duration = TimeSpan.FromMilliseconds(250);

								var storyboard = new Storyboard();
								storyboard.Children.Add(scaleAnimX);
								storyboard.Children.Add(scaleAnimY);
								storyboard.Children.Add(overlayOpacityAnim);
								Storyboard.SetTargetName(scaleAnimX, "OverlayRoot_Scale");
								Storyboard.SetTargetProperty(scaleAnimX, new PropertyPath(ScaleTransform.ScaleXProperty));
								Storyboard.SetTargetName(scaleAnimY, "OverlayRoot_Scale");
								Storyboard.SetTargetProperty(scaleAnimY, new PropertyPath(ScaleTransform.ScaleYProperty));
								Storyboard.SetTargetName(overlayOpacityAnim, OverlayRoot.Name);
								Storyboard.SetTargetProperty(overlayOpacityAnim, new PropertyPath(Grid.OpacityProperty));

								OverlayRoot_Scale.CenterX = OverlayRoot.ActualWidth / 2;
								OverlayRoot_Scale.CenterY = OverlayRoot.ActualHeight / 2;

								storyboard.Begin(this);

								Task.Run(() =>
								{
										Thread.Sleep(1000);
										RootWindow.Dispatcher.Invoke(() => base.Close());
								});
						});
				}

				private void IPAddressTextBox_KeyDown(object sender, KeyEventArgs e)
				{
						if (e.Key == Key.Enter)
						{
								IPAddress address;
								if (!IPAddress.TryParse(IPAddressTextBox.Text, out address))
										NotifyWrongIPFormat();
								else
										ConnectToServer(address);
						}
				}

				private void ContinueButton_Click(object sender, RoutedEventArgs e)
				{
						IPAddress address;
						if (!IPAddress.TryParse(IPAddressTextBox.Text, out address))
								NotifyWrongIPFormat();
						else
								ConnectToServer(address);
				}

				private void NotifyWrongIPFormat()
				{
						var translateAnim1 = new DoubleAnimation();
						translateAnim1.To = -20;
						translateAnim1.Duration = TimeSpan.FromMilliseconds(100);
						var translateAnim2 = new DoubleAnimation();
						translateAnim2.To = 20;
						translateAnim2.Duration = TimeSpan.FromMilliseconds(100);
						translateAnim2.BeginTime = TimeSpan.FromMilliseconds(100);
						var translateAnim3 = new DoubleAnimation();
						translateAnim3.To = 0;
						translateAnim3.Duration = TimeSpan.FromMilliseconds(100);
						translateAnim3.BeginTime = TimeSpan.FromMilliseconds(200);

						var storyboard = new Storyboard();
						storyboard.Children.Add(translateAnim1);
						storyboard.Children.Add(translateAnim2);
						storyboard.Children.Add(translateAnim3);
						Storyboard.SetTargetName(translateAnim1, IPAddressTextBox.Name + "_Translate");
						Storyboard.SetTargetProperty(translateAnim1, new PropertyPath(TranslateTransform.XProperty));
						Storyboard.SetTargetName(translateAnim2, IPAddressTextBox.Name + "_Translate");
						Storyboard.SetTargetProperty(translateAnim2, new PropertyPath(TranslateTransform.XProperty));
						Storyboard.SetTargetName(translateAnim3, IPAddressTextBox.Name + "_Translate");
						Storyboard.SetTargetProperty(translateAnim3, new PropertyPath(TranslateTransform.XProperty));

						storyboard.Begin(this);
				}

				private void ConnectToServer(IPAddress ipAddress)
				{
						ClientService.Instance.Initialize(ipAddress);
						Close();
				}

				private void ErrorMessageExpander_Expanded(object sender, RoutedEventArgs e)
				{
						/*var opacityAnim = new DoubleAnimation();
						opacityAnim.To = 0;
						opacityAnim.Duration = TimeSpan.FromMilliseconds(250);*/
						var translateAnim1 = new DoubleAnimation();
						translateAnim1.To = HeaderContainer_Translate.Y -= ErrorMessageExpander.ActualHeight / 2;
						translateAnim1.Duration = TimeSpan.FromMilliseconds(250);
						translateAnim1.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };


						var storyboard = new Storyboard();
						//storyboard.Children.Add(opacityAnim);
						storyboard.Children.Add(translateAnim1);
						//Storyboard.SetTargetName(opacityAnim, HeaderContainer.Name);
						//Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(OpacityProperty));
						Storyboard.SetTargetName(translateAnim1, HeaderContainer.Name + "_Translate");
						Storyboard.SetTargetProperty(translateAnim1, new PropertyPath(TranslateTransform.YProperty));

						storyboard.Begin(this);
				}

				private void ErrorMessageExpander_Collapsed(object sender, RoutedEventArgs e)
				{
						/*var opacityAnim = new DoubleAnimation();
						opacityAnim.To = 1;
						opacityAnim.Duration = TimeSpan.FromMilliseconds(250);*/
						var translateAnim = new DoubleAnimation();
						translateAnim.To = -100;
						translateAnim.Duration = TimeSpan.FromMilliseconds(250);
						translateAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

						var storyboard = new Storyboard();
						//storyboard.Children.Add(opacityAnim);
						storyboard.Children.Add(translateAnim);
						//Storyboard.SetTargetName(opacityAnim, HeaderContainer.Name);
						//Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(OpacityProperty));
						Storyboard.SetTargetName(translateAnim, HeaderContainer.Name + "_Translate");
						Storyboard.SetTargetProperty(translateAnim, new PropertyPath(TranslateTransform.YProperty));

						storyboard.Begin(this);
				}
		}
}
