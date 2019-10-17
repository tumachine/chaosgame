using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinding
{
    class Program
    {
        static void Main(string[] args)
        {
            int WIDTH = 15;
            int HEIGHT = 15;
            Board board = new Board(WIDTH, HEIGHT);

            board.SetStart(1, 1);
            board.SetEnd(12, 12);

            List<Square> obstacles = new List<Square>();

            Random random = new Random();

            // add 80 or less obstacles
            // ~35.5% map filled
            // not considering a chance of obstacles filling start or end node
            for (int i = 0; i < 80; i++)
            {
                obstacles.Add(new Square(random.Next(WIDTH), random.Next(HEIGHT)));
            }
            /*
            for (int x = 3; x < 100 - 1; x++)
            {
                int tempHeight = 100 / 2;
                int tempWidth = 100 / 2;
                obstacles.Add(new Square(x, tempHeight));
                obstacles.Add(new Square(tempWidth, x));
            }
            */
            board.InsertObstacles(obstacles);

            board.FindAPath();
        }
    }
}
