using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            //Create and shuffle Bombs
            bool[] bombs = Enumerable.Repeat(true, Params.BombCount)
                .Concat(Enumerable.Repeat(false, Tiles.Length - Params.BombCount))
                .OrderBy(r => rnd.Next())
                .ToArray();

            //CreateGrid
            for (int i = 0; i < bombs.Length; i++)
            {
                CalculateRowColumn(i, out int row, out int column);
                Tiles[i] = new Tile(row, column) { Bomb = bombs[i] };
            }

            //Calculate Number of AdjacentBombs
            foreach (Tile t in Tiles)
            {
                if (t.Bomb)
                {
                    ProccessNeighboors(t, tile => { tile.AdjacentBombsCount++; }, true);
                }

            }

            //Refresh
            OnPropertyChanged(nameof(Tiles));
            OnPropertyChanged(nameof(Params));

            //StartTimer
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

        internal void Reveal(Tile tile)
        {
            //OldWay
            /*if (tile.Discovered == true || tile.Flaged || tile.Bomb)
                return;

            tile.Discovered = true;
            tile.RefreshDiscovered();
            tile.RefreshDisplay();
            TilesShown++;

            //if tile contains something return 
            if (tile.Display.ToString() != String.Empty)
                return;

            ProccessNeighboors(tile, t => { ShowANeighbours(t); }, false);*/

            //NewWay
            if (tile.Revealed || tile.Bomb || tile.Flaged)
                return;
            if (tile.AdjacentBombsCount > 0)
            {
                tile.Reveal();
                return;
            }

            Queue<Tile> queue = new Queue<Tile>();
            queue.Enqueue(tile);
            while (queue.Count > 0)
            {
                Tile West = queue.Dequeue();
                Tile East = West;
                //if tile in grid and not discovered not flagged not bomb
                //go west
                while (West.Column > 0 && West.Revealed != true && !West.Flaged && !West.Bomb && West.AdjacentBombsCount == 0)
                    West = Tiles[CalculateIndex(West.Row, West.Column - 1)];
                //go east
                while (East.Column < Params.ColumnsCount - 1 && East.Revealed != true && !East.Flaged && !East.Bomb && East.AdjacentBombsCount == 0)
                    East = Tiles[CalculateIndex(East.Row, East.Column + 1)];

                for (int column = West.Column; column <= East.Column; column++)
                {
                    Tile tmp = Tiles[CalculateIndex(West.Row, column)];
                    //discover
                    tmp.Reveal();
                    tmp.RefreshDisplay();
                    if (tmp.Row > 0)
                    {
                        Tile South = Tiles[CalculateIndex(tmp.Row - 1, tmp.Column)];
                        if (South.Revealed != true && !South.Flaged && !South.Bomb)
                            if (South.AdjacentBombsCount == 0)
                                queue.Enqueue(South);
                            else
                                South.Reveal();
                    }
                    if (tmp.Row < Params.RowsCount - 1)
                    {
                        Tile North = Tiles[CalculateIndex(tmp.Row + 1, tmp.Column)];
                        if (North.Revealed != true && !North.Flaged && !North.Bomb)
                            if (North.AdjacentBombsCount == 0)
                                queue.Enqueue(North);
                            else
                                North.Reveal();
                    }

                }
            }


        }

        internal void ShowAllBombs()
        {
            foreach (Tile tile in Tiles)
            {
                if (tile.Bomb)
                {
                    tile.Reveal();
                }
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
                Reveal(selectedTile);
                if (CheckEndGame())
                    EndGame();
            }
        }

        internal void MoveBomb(Tile tile)
        {
            Tile tmp = null;
            foreach (Tile t in Tiles)
            {
                if (!t.Bomb)
                    tmp = t;
            }

            if (tmp == null)
                throw new Exception("Could not find tile without Bomb");

            tile.Bomb = false;
            ProccessNeighboors(tile, t => t.AdjacentBombsCount--, true);
            tmp.Bomb = true;
            ProccessNeighboors(tmp, t => t.AdjacentBombsCount++, true);
            FirstClick = false;
            DiscoverTile(tile);
        }

        internal bool CheckEndGame()
        {

            return TilesShown + FlagCount == Tiles.Length || GameOver;

        }

        #endregion


        #region Help Methods

        private int CalculateIndex(int row, int col)
        {
            return row * Params.ColumnsCount + col;
        }

        private void CalculateRowColumn(int index, out int row, out int column)
        {
            row = index / Params.ColumnsCount;
            column = index % Params.ColumnsCount;
        }

        private void ProccessNeighboors(Tile t, Action<Tile> method, bool includeSender)
        {
            for (int rowOff = -1; rowOff <= 1; rowOff++)
            {
                for (int colOff = -1; colOff <= 1; colOff++)
                {
                    int row = t.Row + rowOff;
                    int column = t.Column + colOff;
                    if (row > -1 && row < Params.RowsCount && column > -1 && column < Params.ColumnsCount && (includeSender || colOff != 0 || rowOff != 0))
                        method(Tiles[CalculateIndex(row, column)]);
                }
            }
        }

        #endregion

    }
}
