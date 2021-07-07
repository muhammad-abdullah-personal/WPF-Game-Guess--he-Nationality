using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuessTheNationality.Service
{/// <summary>
/// This class is used to maintain the for cordinates of boxes
/// </summary>
    public class PointsList
    {
        public Dictionary<int, PointInformation> Points;
        public PointsList()
        {
            Points = new Dictionary<int, PointInformation>();
            Points.Add(0, new PointInformation { Name = "Japanese", Point = new Point(1, 1) });
            Points.Add(1, new PointInformation { Name = "Chinese", Point = new Point(400, 1) });
            Points.Add(2, new PointInformation { Name = "Korean", Point = new Point(400, 400) });
            Points.Add(3, new PointInformation { Name = "Thai", Point = new Point(1, 400) });
        }
    }

    public class PointInformation
    {
        public string Name { get; set; }
        public Point Point { get; set; }
    }
}
