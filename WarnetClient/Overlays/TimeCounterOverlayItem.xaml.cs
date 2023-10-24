using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using WarnetClient.Views;

namespace WarnetClient.Overlays
{
		/// <summary>
		/// Interaction logic for TimeCounterOverlay.xaml
		/// </summary>
		public partial class TimeCounterOverlayItem : UserControl
		{
				public OverlayWindow Root { get; set; }

				public TimeCounterOverlayItem()
				{
						InitializeComponent();

						timeCounter.OnMessage = (TimeCounterMessage message) =>
						{
								if (message == TimeCounterMessage.LittleTimeLeft)
								{
										littleTimeLeft();
								}
								else {
										Root.NoTimeLeft();
								}
						};
				}

				public void Start(TimeSpan time)
				{
						timeCounter.Start(time);
				}

				public void Stop()
				{
						timeCounter.Stop();
				}

				void littleTimeLeft()
				{
						if (Root.state == OverlayWindow.OverlayState.Minimized)
								return;

						var backgroundAnim1 = new ColorAnimation();
						backgroundAnim1.To = Colors.Red;
						backgroundAnim1.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						var backgroundAnim2 = new ColorAnimation();
						backgroundAnim2.To = Color.FromRgb(25, 25, 25);
						backgroundAnim2.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						backgroundAnim2.BeginTime = TimeSpan.FromMilliseconds(500);

						var windowX1 = new DoubleAnimation();
						windowX1.To = -25;
						windowX1.Duration = new Duration(TimeSpan.FromMilliseconds(250));
						windowX1.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
						var windowX2 = new DoubleAnimation();
						windowX2.To = 25;
						windowX2.Duration = new Duration(TimeSpan.FromMilliseconds(250));
						windowX2.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
						windowX2.BeginTime = TimeSpan.FromMilliseconds(250);
						var windowX3 = new DoubleAnimation();
						windowX3.To = 0;
						windowX3.Duration = new Duration(TimeSpan.FromMilliseconds(250));
						windowX3.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
						windowX3.BeginTime = TimeSpan.FromMilliseconds(500);

						var storyboard = new Storyboard();
						storyboard.Children.Add(backgroundAnim1);
						storyboard.Children.Add(backgroundAnim2);
						storyboard.Children.Add(windowX1);
						storyboard.Children.Add(windowX2);
						storyboard.Children.Add(windowX3);
						Storyboard.SetTargetName(backgroundAnim1, "OverlayBackgroundBrush");
						Storyboard.SetTargetProperty(backgroundAnim1, new PropertyPath(SolidColorBrush.ColorProperty));
						Storyboard.SetTargetName(backgroundAnim2, "OverlayBackgroundBrush");
						Storyboard.SetTargetProperty(backgroundAnim2, new PropertyPath(SolidColorBrush.ColorProperty));					
						Storyboard.SetTargetName(windowX1, Root.Name);
						Storyboard.SetTargetProperty(windowX1, new PropertyPath(Window.LeftProperty));
						Storyboard.SetTargetName(windowX2, Root.Name);
						Storyboard.SetTargetProperty(windowX2, new PropertyPath(Window.LeftProperty));
						Storyboard.SetTargetName(windowX3, Root.Name);
						Storyboard.SetTargetProperty(windowX3, new PropertyPath(Window.LeftProperty));

						storyboard.Begin(Root);
				}

				private void CloseOverlayButton_Click(object sender, RoutedEventArgs e)
				{
						Root.MinimizeOverlay();
				}

				private void OverlayTimeCounter_MouseEnter(object sender, MouseEventArgs e)
				{
						Root.ResizeTo(360);
				}

				private void OverlayTimeCounter_MouseLeave(object sender, MouseEventArgs e)
				{
						Root.ResizeTo(290);
				}
		}
}
