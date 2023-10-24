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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WarnetClient.Views
{
		public enum TimeCounterMessage
		{
				LittleTimeLeft,
				Finish
		}

		/// <summary>
		/// Interaction logic for ModernTimeCounter.xaml
		/// </summary>
		public partial class ModernTimeCounter : UserControl
		{
				Thread timeCounterThread;

				public bool IsRunning = false;
				public Action<TimeCounterMessage> OnMessage;

				public ModernTimeCounter()
				{
						InitializeComponent();
				}

				public void Start(TimeSpan time)
				{
						IsRunning = true;

						timeCounterThread = new Thread(() =>
						{
								TimeSpan currentTime = time;
								TimeSpan lastTime = time;

								Dispatcher.Invoke(() =>
								{
										hoursText.Content = timeToString(currentTime.Hours);
										minutesText.Content = timeToString(currentTime.Minutes);
										secondsText.Content = timeToString(currentTime.Seconds);
								});

								while (IsRunning)
								{
										Dispatcher.Invoke(() =>
										{
												if (currentTime.TotalSeconds < 0)
												{
														IsRunning = false;
														OnMessage(TimeCounterMessage.Finish);
														return;
												}
												if (currentTime.TotalSeconds <= 10)
												{
														OnMessage(TimeCounterMessage.LittleTimeLeft);
												}

												if (currentTime.Hours != lastTime.Hours)
														changeHour(currentTime.Hours);

												if (currentTime.Minutes != lastTime.Minutes)
														changeMinute(currentTime.Minutes);

												if (currentTime.Seconds != lastTime.Seconds)
														changeSecond(currentTime.Seconds);
										});

										lastTime = currentTime;
										currentTime = currentTime.Subtract(TimeSpan.FromSeconds(1));

										Thread.Sleep(1000);
								}
						});
						timeCounterThread.Start();
				}

				public void Stop()
				{
						if (timeCounterThread != null)
								timeCounterThread.Abort();

						changeHour(0);
						changeMinute(0);
						changeSecond(0);
				}

				void animateChangeTime(Label realText, Label fakeText)
				{
						fakeText.Visibility = Visibility.Visible;

						var realTextXAnim = new DoubleAnimation();
						realTextXAnim.From = -20;
						realTextXAnim.To = 0;
						realTextXAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						realTextXAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
						var realTextYAnim = new DoubleAnimation();
						realTextYAnim.From = 20;
						realTextYAnim.To = 0;
						realTextYAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						realTextYAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

						var fakeTextXAnim = new DoubleAnimation();
						fakeTextXAnim.From = 0;
						fakeTextXAnim.To = 20;
						fakeTextXAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						fakeTextXAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
						var fakeTextYAnim = new DoubleAnimation();
						fakeTextYAnim.From = 0;
						fakeTextYAnim.To = -20;
						fakeTextYAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						fakeTextYAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

						var realTextOpacityAnim = new DoubleAnimation();
						realTextOpacityAnim.From = 0;
						realTextOpacityAnim.To = 1;
						realTextOpacityAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						realTextOpacityAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
						var fakeTextOpacityAnim = new DoubleAnimation();
						fakeTextOpacityAnim.From = 1;
						fakeTextOpacityAnim.To = 0;
						fakeTextOpacityAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						fakeTextOpacityAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

						var realTextBlurAnim = new DoubleAnimation();
						realTextBlurAnim.From = 12;
						realTextBlurAnim.To = 0;
						realTextBlurAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));
						var fakeTextBlurAnim = new DoubleAnimation();
						fakeTextBlurAnim.From = 0;
						fakeTextBlurAnim.To = 12;
						fakeTextBlurAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));

						string realTextTransformName = realText.Name + "Translate";
						string fakeTextTransformName = fakeText.Name + "Translate";
						string realTextBlurName = realText.Name + "Blur";
						string fakeTextBlurName = fakeText.Name + "Blur";

						var storyboard = new Storyboard();
						storyboard.Children.Add(realTextXAnim);
						storyboard.Children.Add(realTextYAnim);
						storyboard.Children.Add(fakeTextXAnim);
						storyboard.Children.Add(fakeTextYAnim);
						storyboard.Children.Add(realTextOpacityAnim);
						storyboard.Children.Add(fakeTextOpacityAnim);
						storyboard.Children.Add(realTextBlurAnim);
						storyboard.Children.Add(fakeTextBlurAnim);
						Storyboard.SetTargetName(realTextXAnim, realTextTransformName);
						Storyboard.SetTargetProperty(realTextXAnim, new PropertyPath(TranslateTransform.XProperty));
						Storyboard.SetTargetName(realTextYAnim, realTextTransformName);
						Storyboard.SetTargetProperty(realTextYAnim, new PropertyPath(TranslateTransform.YProperty));
						Storyboard.SetTargetName(fakeTextXAnim, fakeTextTransformName);
						Storyboard.SetTargetProperty(fakeTextXAnim, new PropertyPath(TranslateTransform.XProperty));
						Storyboard.SetTargetName(fakeTextYAnim, fakeTextTransformName);
						Storyboard.SetTargetProperty(fakeTextYAnim, new PropertyPath(TranslateTransform.YProperty));
						Storyboard.SetTargetName(realTextOpacityAnim, realText.Name);
						Storyboard.SetTargetProperty(realTextOpacityAnim, new PropertyPath(OpacityProperty));
						Storyboard.SetTargetName(fakeTextOpacityAnim, fakeText.Name);
						Storyboard.SetTargetProperty(fakeTextOpacityAnim, new PropertyPath(OpacityProperty));
						Storyboard.SetTargetName(realTextBlurAnim, realTextBlurName);
						Storyboard.SetTargetProperty(realTextBlurAnim, new PropertyPath(BlurEffect.RadiusProperty));
						Storyboard.SetTargetName(fakeTextBlurAnim, fakeTextBlurName);
						Storyboard.SetTargetProperty(fakeTextBlurAnim, new PropertyPath(BlurEffect.RadiusProperty));

						storyboard.Begin(this);

						Task.Run(() =>
						{
								Thread.Sleep(500);
								Dispatcher.Invoke(() => fakeText.Visibility = Visibility.Hidden);
						});
				}

				void changeHour(int hour)
				{
						hoursTextFake.Content = hoursText.Content;
						hoursText.Content = timeToString(hour);
						animateChangeTime(hoursText, hoursTextFake);
				}

				void changeMinute(int minute)
				{
						minutesTextFake.Content = minutesText.Content;
						minutesText.Content = timeToString(minute);
						animateChangeTime(minutesText, minutesTextFake);
				}

				void changeSecond(int second)
				{
						secondsTextFake.Content = secondsText.Content;
						secondsText.Content = timeToString(second);
						animateChangeTime(secondsText, secondsTextFake);
				}

				string timeToString(int time)
				{
						if (time < 10)
								return "0" + time.ToString();
						else
								return time.ToString();
				}
		}
}
