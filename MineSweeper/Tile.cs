using System;
using System.ComponentModel;
using System.Drawing;

namespace MineSweeper
{
    public class Tile : INotifyPropertyChanged
    {
        public Tile(bool bomb, int row, int column)
        {
            Bomb = bomb;
            Row = row;
            Column = column;
        }
        public int Row { get; }
        public int Column { get; }
        public bool Bomb { get; }
        public int AdjacentBombsCount { get; set; }
        public bool Discovered { get; set; }
        public bool Flaged { get; set; }
        public object Display
        {
            get
            {
                if (Flaged)
                    return Properties.Resources.Flag;
                else if (Discovered || MainWindow.DEBUG)
                {
                    if (Bomb)
                        return Properties.Resources.Bomb;
                    else
                        return AdjacentBombsCount > 0 ? AdjacentBombsCount.ToString() : String.Empty;
                }
                else
                    return null;

            }
        }

        public void RefreshDisplay()
        {
            OnPropertyChanged(nameof(Display));
        }

        public void RefreshDiscovered()
        {
            OnPropertyChanged(nameof(Discovered));
        }

        #region INotfyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }


}
