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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Constructor
        public MainWindow()
        {
            SetLanguage(MineSweeper.Language.English);
            DataContext = this;
            ColumnsCount = 16;
            RowsCount = 16;
            BombCount = 40;
            FlagCount = 0;
            CreateGame();

            InitializeComponent();
        }
        #endregion

        #region Properties
        public string CurrentLanguage { get; private set; }
        public bool TimeGame { get; set; }
        public List<Tile> Tiles { get; set; }
        public int RowsCount { get; set; }
        public int ColumnsCount { get; set; }

        public int BombCountDisplay
        {
            get { return BombCount - FlagCount; }
        }
        int BombCount;
        int FlagCount;
        int TilesShown;

        internal static bool DEBUG = true;
        static Random rnd = new Random();
        bool GameOver;
        #endregion

        #region MenuClicks
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            CreateGame();
            GameOver = false;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Properties.Resources.AboutMessage, Properties.Resources.AboutText, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Rules_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Controls_Click(object sender, RoutedEventArgs e)
        {

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
            switch (lang)
            {
                case MineSweeper.Language.English:
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                        CurrentLanguage = Properties.Resources.EnglishText;
                        break;
                    }
                case MineSweeper.Language.Polish:
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("pl-PL");
                        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("pl-PL");
                        CurrentLanguage = Properties.Resources.PolishText;
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }

            ObjectDataProvider provider = TryFindResource(nameof(CultureResources)) as ObjectDataProvider;
            provider?.Refresh();
            OnPropertyChanged(nameof(CurrentLanguage));
        }

        

     
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Logic
        private void CreateGame()
        {
            TilesShown = 0;
            FlagCount = 0;
            Tiles = new List<Tile>(RowsCount * ColumnsCount);
            int[] bombs = new int[Tiles.Capacity];
            for (int i = 0; i < BombCount; i++)
                bombs[i] = -1;

            Shuffle(bombs);
            for (int i = 0; i < bombs.Length; i++)
            {
                Tile tile = new Tile(bombs[i] == -1, i / RowsCount, i % ColumnsCount);
                Tiles.Add(tile);
            }
            for (int i = 0; i < Tiles.Count; i++)
            {

                if (Tiles[i].Bomb)
                {
                    int x = i % ColumnsCount;
                    int y = i / RowsCount;
                    ProccessAdjacent(x, y, (t) => t.AdjacentBombsCount++);
                }
            }
            OnPropertyChanged(nameof(Tiles));
            OnPropertyChanged(nameof(RowsCount));
            OnPropertyChanged(nameof(ColumnsCount));
            OnPropertyChanged(nameof(BombCountDisplay));
        }
        private int CalculateIndex(int y, int x)
        {
            return y * ColumnsCount + x;
        }
        public void Shuffle(int[] list)
        {
            int n = list.Length;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

        }



        private void EndGame()
        {
            MessageBox.Show(Properties.Resources.LoseText, Properties.Resources.LoseText, MessageBoxButton.OK, MessageBoxImage.Asterisk);
            GameOver = true;
        }

        private void ShowAllBombs()
        {
            foreach (Tile tile in Tiles.Where(t => t.Bomb && !t.Flaged))
            {
                tile.Discovered = true;
                tile.RefreshDiscovered();
                tile.RefreshDisplay();
            }
        }

        private void ShowAllAdjacent(Tile tile)
        {
            int x = tile.Column;
            int y = tile.Row;
            int index = CalculateIndex(x, y);
            if (index > Tiles.Count || index < 0 || tile.Discovered == true || tile.Flaged)
                return;

            tile.Discovered = true;
            tile.RefreshDiscovered();
            tile.RefreshDisplay();
            TilesShown++;

            if (tile.Display.ToString() != String.Empty)
                return;


            ProccessAdjacent(x, y, (t) => ShowAllAdjacent(t));
        }

        private void ProccessAdjacent(int x, int y, Action<Tile> method)
        {
            if (x > 0)
            {
                if (y > 0)
                    method(Tiles[CalculateIndex(y - 1, x - 1)]);//top-left
                if (y < RowsCount)
                    method(Tiles[CalculateIndex(y, x - 1)]);//left
                if (y < RowsCount - 1)
                    method(Tiles[CalculateIndex(y + 1, x - 1)]);//bottom-left
            }
            if (x < ColumnsCount)
            {
                if (y > 0)
                    method(Tiles[CalculateIndex(y - 1, x)]);//top
                if (y < RowsCount - 1)
                    method(Tiles[CalculateIndex(y + 1, x)]);//bottom
            }
            if (x < ColumnsCount - 1)
            {
                if (y > 0)
                    method(Tiles[CalculateIndex(y - 1, x + 1)]);//top-right
                if (y < RowsCount)
                    method(Tiles[CalculateIndex(y, x + 1)]);//right
                if (y < RowsCount - 1)
                    method(Tiles[CalculateIndex(y + 1, x + 1)]);//bottom-right
            }
        }

        private void Tile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (GameOver)
                return;

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
                    if (selectedTile.Bomb == true)
                    {
                        ShowAllBombs();
                        EndGame();
                    }
                    else
                    {
                        ShowAllAdjacent(selectedTile);
                    }
                    OnPropertyChanged(nameof(BombCountDisplay));
                    CheckGame();
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                selectedTile.Flaged = !selectedTile.Flaged;
                selectedTile.RefreshDisplay();
                if (selectedTile.Flaged)
                    FlagCount++;
                else
                    FlagCount--;
                OnPropertyChanged(nameof(BombCountDisplay));
                CheckGame();
            }
        }

        private void CheckGame()
        {
            if (TilesShown + FlagCount == Tiles.Count)
            {
                MessageBox.Show(Properties.Resources.WinText, Properties.Resources.WinText, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                GameOver = true;
            }

        }


        #endregion


    }
}
