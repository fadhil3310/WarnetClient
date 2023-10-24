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

namespace WarnetClient
{
		/// <summary>
		/// Interaction logic for TopLevelOverlay.xaml
		/// </summary>
		public partial class FindingServerWindow : Window
		{
				public FindingServerWindow()
				{
						InitializeComponent();

						Width = SystemParameters.PrimaryScreenWidth;
						Height = SystemParameters.PrimaryScreenHeight;

						/*Task.Run(() =>
						{
								Thread.Sleep(3000);
								OverlayRoot.Dispatcher.Invoke(() =>
								{
										Close();
										new FailedFindingServerWindow().Show();
								});
						});*/
				}

				public new void Close()
				{
						RootWindow.Dispatcher.Invoke(() =>
						{
								var opacityAnim = new DoubleAnimation();
								opacityAnim.To = 0;
								opacityAnim.Duration = TimeSpan.FromMilliseconds(250);

								var storyboard = new Storyboard();
								storyboard.Children.Add(opacityAnim);
								Storyboard.SetTargetName(opacityAnim, OverlayRoot.Name);
								Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(Grid.OpacityProperty));

								storyboard.Begin(this);

								Task.Run(() =>
								{
										Thread.Sleep(250);
										RootWindow.Dispatcher.Invoke(() => base.Close());
								});
						});
				}
		}
}
