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
        public List<Point[]> RedMarkings { get; set; }
        public List<Point[]> BlackMarkings { get; set; }
        public List<Point[]> BlueMarkings { get; set; }
        public List<Point[]> WhiteMarkings { get; set; }
        public List<Point[]> YellowMarkings { get; set; }
        public List<Point[]> GreenMarkings { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }

        public Markings()
        {
            RedMarkings = new List<Point[]>();
            BlackMarkings = new List<Point[]>();
            BlueMarkings = new List<Point[]>();
            WhiteMarkings = new List<Point[]>();
            YellowMarkings = new List<Point[]>();
            GreenMarkings = new List<Point[]>();
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

        public void Clear()
        {
            RedMarkings = new List<Point[]>();
            BlackMarkings = new List<Point[]>();
            BlueMarkings = new List<Point[]>();
            WhiteMarkings = new List<Point[]>();
            YellowMarkings = new List<Point[]>();
            GreenMarkings = new List<Point[]>();
        }

        public void RemoveFigure(Point[] figure)
        {
            if (RedMarkings.Contains(figure))
                RedMarkings.Remove(RedMarkings.Find(x => x.Equals(figure)));
            if (BlackMarkings.Contains(figure))
                BlackMarkings.Remove(BlackMarkings.Find(x => x.Equals(figure)));
            if (BlueMarkings.Contains(figure))
                BlueMarkings.Remove(BlueMarkings.Find(x => x.Equals(figure)));
            if (WhiteMarkings.Contains(figure))
                WhiteMarkings.Remove(WhiteMarkings.Find(x => x.Equals(figure)));
            if (YellowMarkings.Contains(figure))
                YellowMarkings.Remove(YellowMarkings.Find(x => x.Equals(figure)));
            if (GreenMarkings.Contains(figure))
                GreenMarkings.Remove(GreenMarkings.Find(x => x.Equals(figure)));
        }
    }
}
