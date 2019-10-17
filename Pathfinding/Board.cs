using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinding
{
    class Board
    {
        public Board(int width, int height)
        {
            Squares = new Square[width, height];

            for (int y = 0; y < Squares.GetLength(0); y++)
            {
                for (int x = 0; x < Squares.GetLength(1); x++)
                {
                    Squares[x, y] = new Square(x, y);
                }
            }

            ClosedSet = new List<Square>();
            OpenSet = new List<Square>();
            Path = new List<Square>();
            //Obstacles = new List<Square>();

            Bitmap = new Bitmap(width * Square.Width, height * Square.Height);
            Console.WriteLine($"Board of width {width} and height {height} is created\n");
        }

        public Square Start { get; set; }
        public Square End { get; set; }

        public List<Square> OpenSet { get; set; }
        public List<Square> ClosedSet { get; set; }

        private Bitmap Bitmap { get; }

        private Square[,] Squares { get; }
        //private List<Square> Obstacles { get; set; }

        private List<Square> Path { get; set; }

        public void FindAPath()
        {
            int countPic = 0;
            do
            {
                countPic++;
                DrawCurrentState(countPic);

                Square lastOpenSet = OpenSet[OpenSet.Count - 1];

                Console.WriteLine($"Next square evaluation is at:\n" +
                    $"X: {lastOpenSet.X}\n" +
                    $"Y: {lastOpenSet.Y}\n" +
                    $"FCost: {lastOpenSet.FCost}\n" +
                    $"GCost: {lastOpenSet.GCost}\n" +
                    $"HCost: {lastOpenSet.HCost}\n");
            } while (CalculateNextState());

            DrawCurrentState(countPic);
        }

        public void DrawCurrentState(int picNum)
        {
            for (int y = 0; y < Squares.GetLength(0); y++)
            {
                for (int x = 0; x < Squares.GetLength(1); x++)
                {
                    Squares[x, y].Render(Bitmap, Color.White);
                }
            }

            foreach (Square square in ClosedSet)
                square.Render(Bitmap, Color.Red);

            foreach (Square square in OpenSet)
                square.Render(Bitmap, Color.Green);

            foreach (Square square in Path)
                square.Render(Bitmap, Color.Blue);

            End.Render(Bitmap, Color.Black);
            Start.Render(Bitmap, Color.Black);

            string picName = $"{picNum}.bmp";
            Bitmap.Save(picName);
            Console.WriteLine($"Drawn next state at: {Directory.GetCurrentDirectory()}\\{picName}");
        }

        private bool CalculateNextState()
        {
            if (OpenSet.Count > 0)
            {
                Square current = OpenSet[0];

                // choose a square with a lowest F-cost
                //                      H-cost - distance from a current node to end node + 
                //                      G-cost - distance from starting node to current node
                // if F-cost is the same, then we choose the one with the lowest H-cost
                for (int i = 1; i < OpenSet.Count; i++)
                {
                    if (OpenSet[i].FCost < current.FCost || OpenSet[i].FCost == current.FCost && OpenSet[i].HCost < current.HCost)
                        current = OpenSet[i];
                }

                // remove current from open set
                // add it to closed set, saying that it's
                // no longer going to be operated on
                OpenSet.Remove(current);
                ClosedSet.Add(current);

                // if we reached the end, escape
                if (current == End)
                {
                    RetracePath(Start, End);
                    return false;
                }
                //RetracePath(Start, current);

                // get all the neighbors of the current node
                foreach (Square neighbor in GetNeighbors(current))
                {
                    // skipping neighbors that are already in closed set
                    // and are obstacles
                    if (ClosedSet.Contains(neighbor) || neighbor.Obstacle)
                        continue;

                    // G-cost of a neighbor
                    int costToNeighbor = current.GCost + Heuristics(current, neighbor);

                    // if open set doesn't contain neighbor
                    // then the value if neighbor.GCost is going to be 0
                    // we will need to update neighbor's cost if we meet it second time
                    if (costToNeighbor < neighbor.GCost || !OpenSet.Contains(neighbor))
                    {
                        neighbor.GCost = costToNeighbor;
                        neighbor.HCost = Heuristics(neighbor, End);
                        // remember previous node of neighbor, so that
                        // we can retrace the path 
                        neighbor.Previous = current;

                        if (!OpenSet.Contains(neighbor))
                            OpenSet.Add(neighbor);
                    }
                }
                return true;
            }

            return false;
            
        }

        public List<Square> GetNeighbors(Square square)
        {
            List<Square> neighbors = new List<Square>();
            /*
            // get left
            if (square.X > 0)
                neighbors.Add(Squares[square.X - 1, square.Y]);
            // get right
            if (square.X < Squares.GetLength(1) - 1)
                neighbors.Add(Squares[square.X + 1, square.Y]);
            // get top
            if (square.Y > 0)
                neighbors.Add(Squares[square.X, square.Y - 1]);
            // get bottom
            if (square.Y < Squares.GetLength(0) - 1)
                neighbors.Add(Squares[square.X, square.Y + 1]);
            // top right
            if (square.X + 1 < Squares.GetLength(1) && square.Y - 1 > 0)
                neighbors.Add(Squares[square.X + 1, square.Y - 1]);
            // bottom right
            if (square.X + 1 < Squares.GetLength(1) && square.Y + 1 < Squares.GetLength(0))
                neighbors.Add(Squares[square.X + 1, square.Y + 1]);
            // bottom left
            if (square.X - 1 > 0 && square.Y + 1 < Squares.GetLength(0))
                neighbors.Add(Squares[square.X - 1, square.Y + 1]);
            // top left
            if (square.X - 1 > 0 && square.Y - 1 > 0)
                neighbors.Add(Squares[square.X - 1, square.Y - 1]);
            return neighbors;
            */
            // better alternative 

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    int checkX = square.X + x;
                    int checkY = square.Y + y;

                    if (checkX >= 0 && checkX < Squares.GetLength(1) && checkY >= 0 && checkY < Squares.GetLength(0))
                        neighbors.Add(Squares[checkX, checkY]);
                }
            }
            return neighbors;
        }

        public int Heuristics(Square a, Square b)
        {
            // 14 is a diagonal cost
            // 10 is a vertical and horisontal cost

            int distanceX = Math.Abs(a.X - b.X);
            int distanceY = Math.Abs(a.Y - b.Y);

            // calculate how many diagonal and horisontal moves we need to make
            // if X is greater than Y, we know that there is going to be distanceY diagonal moves
            // and X - Y moves left for horisontal
            if (distanceX > distanceY)
                return 14 * distanceY + 10 * (distanceX - distanceY);
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }

        public void RetracePath(Square start, Square end)
        {
            List<Square> path = new List<Square>();

            Square current = end;

            while (current != start)
            {
                path.Add(current.Previous);
                current = current.Previous;
            }

            Path = path;
        }

        public void SetStart(int x, int y)
        {
            Start = Squares[x, y];
            OpenSet.Add(Start);
        }

        public void SetEnd(int x, int y)
        {
            End = Squares[x, y];
        }

        public void InsertObstacles(List<Square> obstacles)
        {
            foreach (Square obstacle in obstacles)
                Squares[obstacle.X, obstacle.Y].Obstacle = true;
        }
    }
}
