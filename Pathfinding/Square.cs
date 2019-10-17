using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinding
{
    class Square : IEqualityComparer<Square>
    {
        public Square(int x, int y, bool obstacle = false)
        {
            X = x;
            Y = y;
            Obstacle = obstacle;

            GCost = 0;
            HCost = 0;
            FCost = 0;

            XStart = x * Width;
            YStart = y * Height;
            XEnd = XStart + Width;
            YEnd = YStart + Height;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public bool Obstacle { get; set; }

        public int XStart { get; set; }
        public int YStart { get; set; }
        public int XEnd { get; set; }
        public int YEnd { get; set; }

        public static int Width = 50;
        public static int Height = 50;

        public static int BorderWidth = 2;

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get; set; }

        public List<Square> Neighbors { get; set; }

        public Square Previous { get; set; }

        public void Render(Bitmap bitmap, Color color)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // render border if X or Y in border range
                    if (XStart + x < XStart + BorderWidth || 
                        XStart + x > XEnd - BorderWidth || 
                        YStart + y < YStart + BorderWidth || 
                        YStart + y > YEnd - BorderWidth)
                        bitmap.SetPixel(XStart + x, YStart + y, Color.Black);
                    // render obstacle
                    else if (Obstacle)
                        bitmap.SetPixel(XStart + x, YStart + y, Color.Gray);
                    // render whatever main color of a square is
                    else
                        bitmap.SetPixel(XStart + x, YStart + y, color);

                }
            }
        }

        // override equals so later we can compare
        // different squares with their X and Y values
        public bool Equals(Square a, Square b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public int GetHashCode(Square obj)
        {
            return obj.X * 8 + obj.Y * 3;
        }
    }
}
