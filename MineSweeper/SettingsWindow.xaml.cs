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
using System.Windows.Shapes;

namespace MineSweeper
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(GameParams gameParams)
        {
            GameParams = new GameParams()
            {
                RowsCount = gameParams.RowsCount,
                ColumnsCount = gameParams.ColumnsCount,
                BombCount = gameParams.BombCount,
                TimeGame = gameParams.TimeGame,
                Time = gameParams.Time
            };
            DataContext = GameParams;
            InitializeComponent();
        }
        public GameParams GameParams { get; set; }
        decimal MaxBombPercent = 0.6m;

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            int maxBombs = (int)((GameParams.RowsCount * GameParams.ColumnsCount) * MaxBombPercent);
            if (GameParams.BombCount >maxBombs )
            {
                MessageBox.Show(Properties.Resources.TooManyBombsMessage+" "+(maxBombs+1), Properties.Resources.TooManyBombsCaption, 
                    MessageBoxButton.OK,MessageBoxImage.Error);

                return;
            }

            //close dialog
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


    }
}
