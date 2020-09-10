using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FLORA.Computations
{
    public class MyLine
    {
        LineGeometry line;
        public Point _start, _end;
        Path _myPath;
        public MyLine(Point start, Point end, Canvas canvas)
        {
            line = new LineGeometry(start, end);
            _start = start;
            _end = end;
            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.Data = line;
            canvas.Children.Add(myPath);
            _myPath = myPath;
        }
        public Path GetMyPath()
        {
            return _myPath;
        }
    }
}