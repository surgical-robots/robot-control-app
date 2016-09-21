using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TelSurge.DataModels
{
    public class Markings
    {
        public List<Figure> RedMarkings { get; set; }
        public List<Figure> BlackMarkings { get; set; }
        public List<Figure> BlueMarkings { get; set; }
        public List<Figure> WhiteMarkings { get; set; }
        public List<Figure> YellowMarkings { get; set; }
        public List<Figure> GreenMarkings { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }

        public Markings()
        {
            RedMarkings = new List<Figure>();
            BlackMarkings = new List<Figure>();
            BlueMarkings = new List<Figure>();
            WhiteMarkings = new List<Figure>();
            YellowMarkings = new List<Figure>();
            GreenMarkings = new List<Figure>();
            OffsetX = 0;
            OffsetY = 0;
        }

        public Markings Merge(Markings newMarkings) 
        {
            RedMarkings.AddRange(newMarkings.RedMarkings);
            BlackMarkings.AddRange(newMarkings.BlackMarkings);
            BlueMarkings.AddRange(newMarkings.BlueMarkings);
            WhiteMarkings.AddRange(newMarkings.WhiteMarkings);
            YellowMarkings.AddRange(newMarkings.YellowMarkings);
            GreenMarkings.AddRange(newMarkings.GreenMarkings);

            return this;
        }

        public Point[][] GetAllPaths(List<Figure> Markings)
        {
            List<Point[]> figures = new List<Point[]>();
            Markings.ForEach(x => figures.Add(x.Path));
            return figures.ToArray();
        }

        public void Clear()
        {
            RedMarkings = new List<Figure>();
            BlackMarkings = new List<Figure>();
            BlueMarkings = new List<Figure>();
            WhiteMarkings = new List<Figure>();
            YellowMarkings = new List<Figure>();
            GreenMarkings = new List<Figure>();
        }

        public void RemoveLastFigure(List<Figure> Markings)
        {
            Markings.RemoveAt(Markings.Count);
        }
    }
}
