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
using System.Windows.Shapes;

using WebSocketSharp;

namespace WarnetClient.Overlays
{
		/// <summary>
		/// Interaction logic for TimeOverlay.xaml
		/// </summary>
		public partial class OverlayWindow : Window
		{
				public enum OverlayState
				{
						Minimizing,
						Minimized,
						TimeCounter,
						Message
				}

				public OverlayState state = OverlayState.Minimized;

				public event EventHandler OnTimeIsUp;

				public OverlayWindow()
				{
						InitializeComponent();

						RegisterName("OverlayBackgroundBrush", OverlayBackgroundBrush);
						RegisterName("OverlayRoot_Translate", OverlayRoot_Translate);
						RegisterName("UnminimizeButton_Translate", UnminimizeButton_Translate);

						OverlayTimeCounter.Root = this;

						var service = new ClientService();
				}

				public void StartCounting(int seconds)
				{
						if (state == OverlayState.Minimized)
								OpenOverlay();

						OverlayTimeCounter.Start(TimeSpan.FromSeconds(seconds));
				}

				public void StopCounting()
				{
						OverlayTimeCounter.Stop();
						MinimizeOverlay();
				}

				public void ResizeTo(double width)
				{
						if (state == OverlayState.Minimizing)
								return;

						var doubleAnim = new DoubleAnimation();
						doubleAnim.To = width;
						doubleAnim.Duration = new Duration(TimeSpan.FromMilliseconds(250));
						doubleAnim.EasingFunction = new CubicEase();

						var storyboard = new Storyboard();
						storyboard.Children.Add(doubleAnim);
						Storyboard.SetTargetName(doubleAnim, OverlayRoot.Name);
						Storyboard.SetTargetProperty(doubleAnim, new PropertyPath(Grid.WidthProperty));

						storyboard.Begin(this);
				}

				public void OpenOverlay()
				{
						/*var doubleAnimTop1 = new DoubleAnimation();
						doubleAnimTop1.From = -100;
						doubleAnimTop1.To = 0;
						doubleAnimTop1.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						doubleAnimTop1.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

						var storyboard = new Storyboard();
						storyboard.Children.Add(doubleAnimTop1);
						Storyboard.SetTargetName(doubleAnimTop1, this.Name);
						Storyboard.SetTargetProperty(doubleAnimTop1, new PropertyPath(Window.TopProperty));

						storyboard.Begin(this);*/

						state = OverlayState.TimeCounter;

						animateOpen();
				}

				public void MinimizeOverlay()
				{
						state = OverlayState.Minimizing;

						animateMinimize();
				}

				public void MinimizeOverlayWithoutAnim()
				{
						state = OverlayState.Minimized;

						OverlayRoot_Translate.Y = -60;
				}

				void animateOpen()
				{
						var doubleAnimWidth = new DoubleAnimation();
						doubleAnimWidth.From = 20;
						doubleAnimWidth.To = 290;
						doubleAnimWidth.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						doubleAnimWidth.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
						var doubleAnimHeight = new DoubleAnimation();
						doubleAnimHeight.From = 20;
						doubleAnimHeight.To = 64;
						doubleAnimHeight.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						doubleAnimHeight.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

						var doubleAnimTop2 = new DoubleAnimation();
						doubleAnimTop2.To = 0;
						doubleAnimTop2.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						doubleAnimTop2.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn };

						var backgroundAnim = new ColorAnimation();
						backgroundAnim.From = Colors.White;
						backgroundAnim.To = Color.FromRgb(25, 25, 25);
						backgroundAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));

						var storyboard = new Storyboard();
						storyboard.Children.Add(doubleAnimWidth);
						storyboard.Children.Add(doubleAnimHeight);
						storyboard.Children.Add(doubleAnimTop2);
						storyboard.Children.Add(backgroundAnim);
						Storyboard.SetTargetName(doubleAnimWidth, OverlayRoot.Name);
						Storyboard.SetTargetProperty(doubleAnimWidth, new PropertyPath(Grid.WidthProperty));
						Storyboard.SetTargetName(doubleAnimHeight, OverlayRoot.Name);
						Storyboard.SetTargetProperty(doubleAnimHeight, new PropertyPath(Grid.HeightProperty));
						Storyboard.SetTargetName(doubleAnimTop2, "OverlayRoot_Translate");
						Storyboard.SetTargetProperty(doubleAnimTop2, new PropertyPath(TranslateTransform.YProperty));
						Storyboard.SetTargetName(backgroundAnim, "OverlayBackgroundBrush");
						Storyboard.SetTargetProperty(backgroundAnim, new PropertyPath(SolidColorBrush.ColorProperty));

						storyboard.Begin(this);
				}

				void animateMinimize()
				{
						var doubleAnimWidth = new DoubleAnimation();
						doubleAnimWidth.From = OverlayRoot.ActualWidth;
						doubleAnimWidth.To = 20;
						doubleAnimWidth.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						doubleAnimWidth.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
						var doubleAnimHeight = new DoubleAnimation();
						doubleAnimHeight.From = OverlayRoot.ActualHeight;
						doubleAnimHeight.To = 20;
						doubleAnimHeight.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						doubleAnimHeight.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

						var doubleAnimTop1 = new DoubleAnimation();
						doubleAnimTop1.To = 25;
						doubleAnimTop1.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						doubleAnimTop1.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
						var doubleAnimTop2 = new DoubleAnimation();
						doubleAnimTop2.To = -60;
						doubleAnimTop2.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						doubleAnimTop2.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn };
						doubleAnimTop2.BeginTime = TimeSpan.FromMilliseconds(500);

						var backgroundAnim = new ColorAnimation();
						backgroundAnim.To = Colors.White;
						backgroundAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));

						var storyboard = new Storyboard();
						storyboard.Children.Add(doubleAnimWidth);
						storyboard.Children.Add(doubleAnimHeight);
						storyboard.Children.Add(doubleAnimTop1);
						storyboard.Children.Add(doubleAnimTop2);
						storyboard.Children.Add(backgroundAnim);
						Storyboard.SetTargetName(doubleAnimWidth, OverlayRoot.Name);
						Storyboard.SetTargetProperty(doubleAnimWidth, new PropertyPath(Grid.WidthProperty));
						Storyboard.SetTargetName(doubleAnimHeight, OverlayRoot.Name);
						Storyboard.SetTargetProperty(doubleAnimHeight, new PropertyPath(Grid.HeightProperty));
						Storyboard.SetTargetName(doubleAnimTop1, "OverlayRoot_Translate");
						Storyboard.SetTargetProperty(doubleAnimTop1, new PropertyPath(TranslateTransform.YProperty));
						Storyboard.SetTargetName(doubleAnimTop2, "OverlayRoot_Translate");
						Storyboard.SetTargetProperty(doubleAnimTop2, new PropertyPath(TranslateTransform.YProperty));
						Storyboard.SetTargetName(backgroundAnim, "OverlayBackgroundBrush");
						Storyboard.SetTargetProperty(backgroundAnim, new PropertyPath(SolidColorBrush.ColorProperty));

						storyboard.Begin(this);

						Task.Run(() =>
						{
								Thread.Sleep(1250);
								state = OverlayState.Minimized;
						});
				}

				public void NoTimeLeft()
				{
						animateMinimize();

						Task.Run(() =>
						{
								Thread.Sleep(1250);
								OnTimeIsUp.Emit(this, new EventArgs());
						});
				}

				private void Label_MouseDown(object sender, MouseButtonEventArgs e)
				{
						MinimizeOverlay();
				}

				private void UnminimizeButton_Click(object sender, RoutedEventArgs e)
				{
						OpenOverlay();
						hideUnminimizeButton();
				}

				private void UnminimizeRoot_MouseEnter(object sender, MouseEventArgs e)
				{
						if (state != OverlayState.Minimized)
								return;

						showUnminimizeButton();
				}

				private void UnminimizeRoot_MouseLeave(object sender, MouseEventArgs e)
				{
						if (state != OverlayState.Minimized)
								return;

						hideUnminimizeButton();
				}

				private void showUnminimizeButton()
				{
						var translateAnim = new DoubleAnimation();
						translateAnim.To = 0;
						translateAnim.Duration = new Duration(TimeSpan.FromMilliseconds(250));
						translateAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

						var storyboard = new Storyboard();
						storyboard.Children.Add(translateAnim);
						Storyboard.SetTargetName(translateAnim, "UnminimizeButton_Translate");
						Storyboard.SetTargetProperty(translateAnim, new PropertyPath(TranslateTransform.YProperty));

						storyboard.Begin(this);
				}

				private void hideUnminimizeButton()
				{
						var translateAnim = new DoubleAnimation();
						translateAnim.To = -60;
						translateAnim.Duration = new Duration(TimeSpan.FromMilliseconds(250));
						translateAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

						var storyboard = new Storyboard();
						storyboard.Children.Add(translateAnim);
						Storyboard.SetTargetName(translateAnim, "UnminimizeButton_Translate");
						Storyboard.SetTargetProperty(translateAnim, new PropertyPath(TranslateTransform.YProperty));

						storyboard.Begin(this);
				}
		}
}
