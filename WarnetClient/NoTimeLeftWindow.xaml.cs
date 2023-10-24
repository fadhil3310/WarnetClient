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

using GI.Screenshot;

using WarnetClient.Helper;

namespace WarnetClient
{
		

		/// <summary>
		/// Interaction logic for NoTimeLeftWindow.xaml
		/// </summary>
		public partial class NoTimeLeftWindow : Window
		{
				public NoTimeLeftWindow()
				{
						InitializeComponent();

						// Make the window fill the screen
						Left = 0;
						Top = 0;
						Width = SystemParameters.PrimaryScreenWidth;
						Height = SystemParameters.PrimaryScreenHeight;

						// Position all the screen mesh's triangles
						// to make the screen mesh fill the screen
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

						infoContainer.Opacity = 0;

						// Wait for 0.5 seconds, then start the animation
						Task.Run(() =>
						{
								Thread.Sleep(500);
								Dispatcher.Invoke(() => animateIntroScreen());
						});

						// Reject all user input so that Andi will be angry because of it
						KeyboardLockService.Instance.Start();
				}

				void animateIntroScreen()
				{
						var translateAnim1 = new DoubleAnimation();
						translateAnim1.To = -0.12;
						translateAnim1.Duration = TimeSpan.FromMilliseconds(2000);
						translateAnim1.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

						var rotationAnim = new DoubleAnimation();
						rotationAnim.To = -50;
						rotationAnim.Duration = TimeSpan.FromMilliseconds(4000);
						rotationAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						rotationAnim.BeginTime = TimeSpan.FromMilliseconds(1750);

						var translateAnim2 = new DoubleAnimation();
						translateAnim2.To = 70;
						translateAnim2.Duration = TimeSpan.FromMilliseconds(4000);
						translateAnim2.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						translateAnim2.BeginTime = TimeSpan.FromMilliseconds(1750);

						var fovAnim = new DoubleAnimation();
						fovAnim.To = 90;
						fovAnim.Duration = TimeSpan.FromMilliseconds(4000);
						fovAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						fovAnim.BeginTime = TimeSpan.FromMilliseconds(1750);

						var scaleAnimX = new DoubleAnimation();
						scaleAnimX.To = 1;
						scaleAnimX.Duration = TimeSpan.FromMilliseconds(3750);
						scaleAnimX.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						scaleAnimX.BeginTime = TimeSpan.FromMilliseconds(2000);
						var scaleAnimY = new DoubleAnimation();
						scaleAnimY.To = 1;
						scaleAnimY.Duration = TimeSpan.FromMilliseconds(3750);
						scaleAnimY.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						scaleAnimY.BeginTime = TimeSpan.FromMilliseconds(2000);

						var infoOpacityAnim = new DoubleAnimation();
						infoOpacityAnim.From = 0;
						infoOpacityAnim.To = 1;
						infoOpacityAnim.Duration = TimeSpan.FromMilliseconds(1000);
						infoOpacityAnim.BeginTime = TimeSpan.FromMilliseconds(3000);

						var storyboard = new Storyboard();
						storyboard.Children.Add(rotationAnim);
						storyboard.Children.Add(translateAnim1);
						storyboard.Children.Add(translateAnim2);
						storyboard.Children.Add(scaleAnimX);
						storyboard.Children.Add(scaleAnimY);
						storyboard.Children.Add(fovAnim);
						storyboard.Children.Add(infoOpacityAnim);
						Storyboard.SetTargetName(rotationAnim, "modelRotation");
						Storyboard.SetTargetProperty(rotationAnim, new PropertyPath(AxisAngleRotation3D.AngleProperty));
						Storyboard.SetTargetName(translateAnim1, "modelTranslate");
						Storyboard.SetTargetProperty(translateAnim1, new PropertyPath(TranslateTransform3D.OffsetZProperty));
						Storyboard.SetTargetName(translateAnim2, "viewportTranslateTransform");
						Storyboard.SetTargetProperty(translateAnim2, new PropertyPath(TranslateTransform.YProperty));
						Storyboard.SetTargetName(fovAnim, "viewportCamera");
						Storyboard.SetTargetProperty(fovAnim, new PropertyPath(PerspectiveCamera.FieldOfViewProperty));
						Storyboard.SetTargetName(scaleAnimX, "viewportScaleTransform");
						Storyboard.SetTargetProperty(scaleAnimX, new PropertyPath(ScaleTransform.ScaleXProperty));
						Storyboard.SetTargetName(scaleAnimY, "viewportScaleTransform");
						Storyboard.SetTargetProperty(scaleAnimY, new PropertyPath(ScaleTransform.ScaleYProperty));
						Storyboard.SetTargetName(infoOpacityAnim, infoContainer.Name);
						Storyboard.SetTargetProperty(infoOpacityAnim, new PropertyPath(OpacityProperty));

						viewportScaleTransform.CenterX = viewport.ActualWidth / 2;
						viewportScaleTransform.CenterY = viewport.ActualHeight / 2;

						modelMaterial.Brush = new ImageBrush(Screenshot.CaptureAllScreens());
						window.Background = Brushes.Black;

						storyboard.Begin(this);
				}

				void animateClosingScreen()
				{
						var infoOpacityAnim = new DoubleAnimation();
						infoOpacityAnim.From = 1;
						infoOpacityAnim.To = 0;
						infoOpacityAnim.Duration = TimeSpan.FromMilliseconds(1000);

						var rotationAnim = new DoubleAnimation();
						rotationAnim.To = 0;
						rotationAnim.Duration = TimeSpan.FromMilliseconds(3000);
						rotationAnim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						var translateAnim1 = new DoubleAnimation();
						translateAnim1.To = -0.15;
						translateAnim1.Duration = TimeSpan.FromMilliseconds(3000);
						translateAnim1.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						var translateAnim2 = new DoubleAnimation();
						translateAnim2.To = 0;
						translateAnim2.Duration = TimeSpan.FromMilliseconds(3000);
						translateAnim2.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						var translateAnim3 = new DoubleAnimation();
						translateAnim3.To = 0;
						translateAnim3.Duration = TimeSpan.FromMilliseconds(2000);
						translateAnim3.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut };
						translateAnim3.BeginTime = TimeSpan.FromMilliseconds(3000);

						var storyboard = new Storyboard();
						storyboard.Children.Add(infoOpacityAnim);
						storyboard.Children.Add(rotationAnim);
						storyboard.Children.Add(translateAnim1);
						storyboard.Children.Add(translateAnim2);
						storyboard.Children.Add(translateAnim3);

						Storyboard.SetTargetName(rotationAnim, "modelRotation");
						Storyboard.SetTargetProperty(rotationAnim, new PropertyPath(AxisAngleRotation3D.AngleProperty));
						Storyboard.SetTargetName(translateAnim1, "modelTranslate");
						Storyboard.SetTargetProperty(translateAnim1, new PropertyPath(TranslateTransform3D.OffsetZProperty));
						Storyboard.SetTargetName(translateAnim2, "viewportTranslateTransform");
						Storyboard.SetTargetProperty(translateAnim2, new PropertyPath(TranslateTransform.YProperty));
						Storyboard.SetTargetName(translateAnim3, "modelTranslate");
						Storyboard.SetTargetProperty(translateAnim3, new PropertyPath(TranslateTransform3D.OffsetZProperty));
						Storyboard.SetTargetName(infoOpacityAnim, infoContainer.Name);
						Storyboard.SetTargetProperty(infoOpacityAnim, new PropertyPath(OpacityProperty));

						storyboard.Begin(this);
				}

				public new void Close()
				{
						animateClosingScreen();

						// Wait until the animation ended, then close the window.
						// We assume that the animation will be for 5.2 seconds.
						Task.Run(() =>
						{
								Thread.Sleep(5200);
								KeyboardLockService.Instance.Stop();
								window.Dispatcher.Invoke(() => base.Close());
						});
				}
		}
}
