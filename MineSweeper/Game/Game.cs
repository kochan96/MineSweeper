using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace MineSweeper
{
    public class Game : INotifyPropertyChanged
    {
        public Game()
        {
            NewGame(GameParams.DefaultGameParams);
        }

        #region Properties
        public Tile[] Tiles { get; private set; }
        public GameParams Params { get; private set; }
        public string TimeDisplay { get; private set; }
        public int BombCountDisplay { get { return Params.BombCount - FlagCount; } }
        #endregion

        #region Fields
        bool GameOver;
        static Random rnd = new Random();
        bool FirstClick;
        DateTime start;
        DateTime stop;
        DispatcherTimer Timer;
        int TilesShown;
        int FlagCount;


        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Create

        internal void NewGame()
        {
            NewGame(Params);
        }
        internal void NewGame(GameParams gameParams)
        {
            Params = new GameParams()
            {
                RowsCount = gameParams.RowsCount,
                ColumnsCount = gameParams.ColumnsCount,
                BombCount = gameParams.BombCount,
                TimeGame = gameParams.TimeGame,
                Time = gameParams.Time
            };

            if (Params.TimeGame)
                TimeDisplay = gameParams.Time.ToString(@"hh\:mm\:ss");
            else
                TimeDisplay = TimeSpan.FromSeconds(0).ToString(@"hh\:mm\:ss");

            OnPropertyChanged(nameof(TimeDisplay));

            FirstClick = true;
            TilesShown = 0;
            FlagCount = 0;
            OnPropertyChanged(nameof(BombCountDisplay));

            Tiles = new Tile[Params.RowsCount * Params.ColumnsCount];
            bool[] bombs = new bool[Tiles.Length];
            for (int i = 0; i < Params.BombCount; i++)
                bombs[i] = true;

            Shuffle(bombs);
            for (int i = 0; i < bombs.Length; i++)
            {
                Tile tile = new Tile(i / Params.ColumnsCount, i % Params.ColumnsCount);
                tile.Bomb = bombs[i];
                int index = CalculateIndex(tile.Row, tile.Column);
                Debug.WriteLine(index);
                Tiles[index] = tile;
            }

            for (int i = 0; i < Tiles.Length; i++)
            {

                if (Tiles[i].Bomb)
                {

                    ProccessAdjacent(Tiles[i].Column, Tiles[i].Row, (t) => t.AdjacentBombsCount++);
                }
            }

            OnPropertyChanged(nameof(Tiles));
            OnPropertyChanged(nameof(Params));

            if (Timer == null)
            {
                Timer = new DispatcherTimer();
                Timer.Interval = TimeSpan.FromMilliseconds(1);
                Timer.Tick += Count;
            }
            start = DateTime.Now;
            Timer.Start();
        }

        private void Count(object sender, EventArgs e)
        {
            if (Params.TimeGame)
            {
                TimeSpan time = (Params.Time - DateTime.Now.Subtract(start));
                TimeDisplay = time.ToString(@"hh\:mm\:ss");
                if (time <= TimeSpan.FromSeconds(0))
                    GameOver = true;
            }
            else
            {
                TimeDisplay = DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss");
            }

            OnPropertyChanged(nameof(TimeDisplay));
        }
        internal void EndGame()
        {
            ShowAllBombs();
            StopGame();
            if (BombCountDisplay == 0)
                MessageBox.Show(Properties.Resources.WinText, Properties.Resources.LoseText, MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show(Properties.Resources.LoseText, Properties.Resources.LoseText, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        internal void StopGame()
        {
            Timer.Stop();
            stop = DateTime.Now;
        }

        internal void StartGame()
        {
            Timer.Start();
            start = start.Add(DateTime.Now.Subtract(stop));
        }

        #endregion

        #region Game Logic

        internal void ShowAllAdjacent(Tile tile)
        {
            int column = tile.Column;
            int row = tile.Row;
            int index = CalculateIndex(column, row);
            if (index > Tiles.Length || index < 0 || tile.Discovered == true || tile.Flaged)
                return;

            tile.Discovered = true;
            tile.RefreshDiscovered();
            tile.RefreshDisplay();
            TilesShown++;

            if (tile.Display.ToString() != String.Empty)
                return;

            ProccessAdjacent(column, row, (t) => ShowAllAdjacent(t));
        }

        internal void ShowAllBombs()
        {
            foreach (Tile tile in Tiles.Where(t => t.Bomb))
            {
                tile.Discovered = true;
                tile.RefreshDiscovered();
                tile.RefreshDisplay();
            }
            //endGame
            TilesShown = Tiles.Length - FlagCount;
        }

        internal void FlagTile(Tile tile)
        {
            tile.Flaged = !tile.Flaged;
            if (tile.Flaged)
                FlagCount++;
            else
                FlagCount--;

            OnPropertyChanged(nameof(BombCountDisplay));

            tile.RefreshDisplay();
        }

        internal void DiscoverTile(Tile selectedTile)
        {

            if (selectedTile.Bomb == true)
                if (FirstClick)
                    MoveBomb(selectedTile);
                else
                    EndGame();
            else
            {
                FirstClick = false;
                ShowAllAdjacent(selectedTile);
                if (CheckEndGame())
                    EndGame();
            }
        }

        internal void MoveBomb(Tile tile)
        {
            Tile tmp = Tiles.First(t2 => t2.Bomb == false);

            tile.Bomb = false;
            ProccessAdjacent(tile.Column, tile.Row, (t) => t.AdjacentBombsCount--);
            tmp.Bomb = true;
            ProccessAdjacent(tmp.Column, tmp.Row, (t2) => { t2.AdjacentBombsCount++; });

            FirstClick = false;
            DiscoverTile(tile);
        }

        internal bool CheckEndGame()
        {

            return TilesShown + FlagCount == Tiles.Length || GameOver;

        }

        #endregion


        #region Help Methods

        private int CalculateIndex(int y, int x)
        {
            return y * Params.ColumnsCount + x;
        }

        private void Shuffle(bool[] list)
        {
            int n = list.Length;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                bool value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

        }

        private void ProccessAdjacent(int x, int y, Action<Tile> method)
        {
            if (x > 0)
            {
                if (y > 0)
                    method(Tiles[CalculateIndex(y - 1, x - 1)]);//top-left
                if (y < Params.RowsCount)
                    method(Tiles[CalculateIndex(y, x - 1)]);//left
                if (y < Params.RowsCount - 1)
                    method(Tiles[CalculateIndex(y + 1, x - 1)]);//bottom-left
            }
            if (x < Params.ColumnsCount)
            {
                if (y > 0)
                    method(Tiles[CalculateIndex(y - 1, x)]);//top
                if (y < Params.RowsCount - 1)
                    method(Tiles[CalculateIndex(y + 1, x)]);//bottom
            }
            if (x < Params.ColumnsCount - 1)
            {
                if (y > 0)
                    method(Tiles[CalculateIndex(y - 1, x + 1)]);//top-right
                if (y < Params.RowsCount)
                    method(Tiles[CalculateIndex(y, x + 1)]);//right
                if (y < Params.RowsCount - 1)
                    method(Tiles[CalculateIndex(y + 1, x + 1)]);//bottom-right
            }
        }
        #endregion
    }
}
