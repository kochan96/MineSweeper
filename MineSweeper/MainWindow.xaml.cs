using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using System.Windows.Threading;

namespace MineSweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor
        public MainWindow()
        {


            //default values
            Game = new Game();
            DataContext = Game;
            InitializeComponent();
            SetLanguage(MineSweeper.Language.English);
        }
        #endregion


        public Game Game { get; }

        #region MenuClicks
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            Game.NewGame();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow dialog = new SettingsWindow(Game.Params);
            dialog.Owner = this;
            Game.StopGame();
            if (dialog.ShowDialog() == true)
            {
                Game.NewGame(dialog.GameParams);
            }
            else
            {
                if (!Game.CheckEndGame())
                    Game.StartGame();
            }


        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Properties.Resources.AboutMessage, Properties.Resources.AboutText, MessageBoxButton.OK, MessageBoxImage.Information);
        }



        private void Polish_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage(MineSweeper.Language.Polish);
        }

        private void English_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage(MineSweeper.Language.English);
        }
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            MessageBoxResult result = MessageBox.Show(Properties.Resources.ConfirmationMessage,
                Properties.Resources.ConfirmationCaption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            e.Cancel = result == MessageBoxResult.No;
        }

        #endregion

        #region Languages
        private void SetLanguage(Language lang)
        {
            Localizer.SetLanguage(lang);
        }
        #endregion

        #region Logic

        private void Tile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Game.CheckEndGame())
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            Button button = sender as Button;
            if (button == null)
                return;

            Tile selectedTile = button.DataContext as Tile;
            if (selectedTile == null || selectedTile.Discovered)
                return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!selectedTile.Flaged)
                {
                    Game.DiscoverTile(selectedTile);
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                Game.FlagTile(selectedTile);
            }
        }
        #endregion


    }
}
