using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    public class GameParams
    {

        public int RowsCount { get; set; }
        public int ColumnsCount { get; set; }
        public int BombCount { get; set; }
        public bool TimeGame { get; set; }
        public TimeSpan Time { get; set; }

        public static GameParams DefaultGameParams
        {
            get
            {
                return new GameParams()
                {
                    RowsCount = 16,
                    ColumnsCount = 16,
                    BombCount = 40,
                    TimeGame = false,
                    Time = TimeSpan.FromSeconds(300)
                };
            }
        }
    }
}
