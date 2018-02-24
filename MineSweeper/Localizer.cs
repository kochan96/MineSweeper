using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    public enum Language
    {
        English,
        Polish
    }

    public class Localizer
    {
        public Properties.Resources GetResourceInstance()
        {
            return new Properties.Resources();
        }
    }
}
