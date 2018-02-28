using System;
using System.ComponentModel;
using System.Drawing;

namespace MineSweeper
{
    public class Tile : INotifyPropertyChanged
    {
        public Tile(int row, int column)
        {
            Row = row;
            Column = column;
        }
        public int Row { get; }
        public int Column { get; }
        public bool Bomb { get; set; }
        public int AdjacentBombsCount { get; set; }
        public bool Revealed { get; set; }
        public bool Flaged { get; set; }
        public object Display
        {
            get
            {
                if (Bomb && Flaged && Revealed)
                    return Properties.Resources.DiffusedBomb;
                else if (Flaged)
                    return Properties.Resources.Flag;
                else if (Revealed)
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

        public void Reveal()
        {
            Revealed = true;
            OnPropertyChanged(nameof(Revealed));
            OnPropertyChanged(nameof(Display));
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
