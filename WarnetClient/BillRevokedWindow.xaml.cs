using GI.Screenshot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
using WarnetClient.Helper;

namespace WarnetClient
{
		/// <summary>
		/// Interaction logic for BillRevokedWindow.xaml
		/// </summary>
		public partial class BillRevokedWindow : Window
		{
				public BillRevokedWindow()
				{
						InitializeComponent();

						Left = 0;
						Top = 0;
						Width = SystemParameters.PrimaryScreenWidth;
						Height = SystemParameters.PrimaryScreenHeight;

						meshGeometry3d.Positions = new Point3DCollection
						{
								ViewportHelper.CoordinateToViewport(
										Width,
										Height,
										0, 0),
							ViewportHelper.CoordinateToViewport(
										Width,
										Height,
										0, Height),
							ViewportHelper.CoordinateToViewport(
										Width,
										Height,
										Width, Height),
							ViewportHelper.CoordinateToViewport(
										Width,
										Height,
										Width, 0),
						};

						RegisterName("modelTranslate", modelTranslate);
						RegisterName("modelRotation", modelRotation);
						RegisterName("viewportTranslateTransform", viewportTranslateTransform);
						RegisterName("viewportScaleTransform", viewportScaleTransform);
						RegisterName("viewportCamera", viewportCamera);

						Task.Run(() =>
						{
								Thread.Sleep(500);
								RootWindow.Dispatcher.Invoke(() => animateIntroScreen());
						});

						KeyboardLockService.Instance.Start();
				}

				public new void Close()
				{
						animateOutroScreen();
						Task.Run(() =>
						{
								Thread.Sleep(2000);
								KeyboardLockService.Instance.Stop();
								RootWindow.Dispatcher.Invoke(() => base.Close());
						});
				}

				void animateIntroScreen()
				{
						var scaleAnimX = new DoubleAnimation();
						scaleAnimX.To = 0.8;
						scaleAnimX.Duration = TimeSpan.FromMilliseconds(2000);
						scaleAnimX.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						var scaleAnimY = new DoubleAnimation();
						scaleAnimY.To = 0.8;
						scaleAnimY.Duration = TimeSpan.FromMilliseconds(2000);
						scaleAnimY.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };


						var storyboard = new Storyboard();
						storyboard.Children.Add(scaleAnimX);
						storyboard.Children.Add(scaleAnimY);
						Storyboard.SetTargetName(scaleAnimX, "viewportScaleTransform");
						Storyboard.SetTargetProperty(scaleAnimX, new PropertyPath(ScaleTransform.ScaleXProperty));
						Storyboard.SetTargetName(scaleAnimY, "viewportScaleTransform");
						Storyboard.SetTargetProperty(scaleAnimY, new PropertyPath(ScaleTransform.ScaleYProperty));

						viewportScaleTransform.CenterX = viewport.ActualWidth / 2;
						viewportScaleTransform.CenterY = viewport.ActualHeight / 2;

						modelMaterial.Brush = new ImageBrush(Screenshot.CaptureAllScreens());
						RootWindow.Background = Brushes.Black;

						storyboard.Begin(this);
				}

				void animateOutroScreen()
				{
						var scaleAnimX = new DoubleAnimation();
						scaleAnimX.To = 1;
						scaleAnimX.Duration = TimeSpan.FromMilliseconds(2000);
						scaleAnimX.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						var scaleAnimY = new DoubleAnimation();
						scaleAnimY.To = 1;
						scaleAnimY.Duration = TimeSpan.FromMilliseconds(2000);
						scaleAnimY.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };

						var opacityAnim = new DoubleAnimation();
						opacityAnim.To = 0;
						opacityAnim.Duration = TimeSpan.FromMilliseconds(500);
						opacityAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };


						var storyboard = new Storyboard();
						storyboard.Children.Add(scaleAnimX);
						storyboard.Children.Add(scaleAnimY);
						storyboard.Children.Add(opacityAnim);
						Storyboard.SetTargetName(scaleAnimX, "viewportScaleTransform");
						Storyboard.SetTargetProperty(scaleAnimX, new PropertyPath(ScaleTransform.ScaleXProperty));
						Storyboard.SetTargetName(scaleAnimY, "viewportScaleTransform");
						Storyboard.SetTargetProperty(scaleAnimY, new PropertyPath(ScaleTransform.ScaleYProperty));
						Storyboard.SetTargetName(opacityAnim, HeaderContainer.Name);
						Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(Grid.OpacityProperty));

						viewportScaleTransform.CenterX = viewport.ActualWidth / 2;
						viewportScaleTransform.CenterY = viewport.ActualHeight / 2;

						storyboard.Begin(this);
				}
		}
}
