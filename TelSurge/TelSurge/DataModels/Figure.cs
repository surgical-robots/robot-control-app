using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TelSurge.DataModels
{
    public class Figure
    {
        public Color Color { get; set; }
        public Point[] Path { get; set; }

        public Figure()
        {
            this.Color = Color.Red;
        }
        public Figure(Color Color)
        {
            this.Color = Color;
        }
        public Figure(Color color, Point[] path)
        {
            this.Color = color;
            this.Path = path;
        }
    }
}
