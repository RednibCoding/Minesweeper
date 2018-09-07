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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow:Window
	{
		Game _game;

		public MainWindow()
		{
			InitializeComponent();
			// Initialize a new game and make this MainWindow's Grid it's parent
			_game=new Game();
			_game.init(RootGrid);
		}
	}
}
