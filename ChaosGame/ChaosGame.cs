using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    class ChaosGame
    {
        static private Random random = new Random();

        private int Width { get; }
        private int Height { get; }
        private int Radius { get; }
        
        private int CentreX { get; }
        private int CentreY { get; }

        private List<Point> VertexPoints { get; set; }
        private List<Color> VertexColors { get; set; }
        private Point jumpingPoint;

        private Bitmap Bitmap { get; }

        public ChaosGame(int width, int height)
        {
            Width = width;
            Height = height;
            Radius = Math.Min(width, height) / 2;
            CentreX = width / 2;
            CentreY = height / 2;
            Bitmap = new Bitmap(width, height);
            // set a random point so we can instantly start the game
            jumpingPoint = new Point(random.Next(width), random.Next(height));
            VertexPoints = new List<Point>();
            VertexColors = new List<Color>();
        }
        
        public void DrawChaos(int numOfVertexes, Func<int, int> Rules, string fileName, int numOfIterations = 100000)
        {
            //RemovePictures();

            SetBackground(Color.White);
            SetColors(numOfVertexes);
            SetVertexes(numOfVertexes);

            FillBitmap(Rules, numOfIterations);
            Bitmap.Save(fileName);
            Console.WriteLine($"Created chaos image of {numOfVertexes} angles with {numOfIterations} pixels");
            Console.WriteLine($"Path: {Directory.GetCurrentDirectory()}\\{fileName}\n");

            // can be done better
            ChaosGameDelegates.Reset();
            Reset();
        }

        public void RemovePictures()
        {
            foreach (string fileName in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                if (Path.GetExtension(fileName) == ".bmp")
                    File.Delete(fileName);
            }
        }

        private void SetBackground(Color color)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Bitmap.SetPixel(x, y, color);
        }

        private void SetVertexes(int numOfVertexes)
        {
            double angle = 2 * Math.PI / numOfVertexes;
            for (int i = 0; i < numOfVertexes; i++)
            {
                int x = (int)(CentreX + Radius * Math.Sin(i * angle));
                int y = (int)(CentreY + Radius * Math.Cos(i * angle));
                VertexPoints.Add(new Point(x, y));
            }
        }

        private void SetColors(int numOfVertexes)
        {
            for (int i = 0; i < numOfVertexes; i++)
            {
                Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                VertexColors.Add(color);
            }
        }

        public void FillBitmap(Func<int, int> Rule, int numOfIterations = 100000)
        {
            for (int i = 0; i < numOfIterations; i++)
            {
                int vertexByRule = Rule(VertexPoints.Count);
                Point middlePoint = MiddleBetweenTwoPoints(jumpingPoint, VertexPoints[vertexByRule]);
                
                Bitmap.SetPixel(middlePoint.X, middlePoint.Y, VertexColors[vertexByRule]);
                jumpingPoint.X = middlePoint.X;
                jumpingPoint.Y = middlePoint.Y;
                //if (i % 2500 == 0)
                //    Bitmap.Save($"{i}.bmp");
            }
        }



        private static Point MiddleBetweenTwoPoints(Point a, Point b)
        {
            int x = (a.X + b.X) / 2;
            int y = (a.Y + b.Y) / 2;
            return new Point(x, y);
        }

        private void Reset()
        {
            SetBackground(Color.White);
            VertexPoints = new List<Point>();
            VertexColors = new List<Color>();
        }
    }

    static class ChaosGameDelegates
    {
        static private Random random = new Random();
        static private int prevIndex = 0;
        static private int currentIndex = 0;

        public static int AnyRandomVertex(int vertexPoints)
        {
            return random.Next(vertexPoints);
        }

        // select a vertex that was not chosen right before
        public static int CurrentNotPrev(int vertexPoints)
        {
            while (currentIndex == prevIndex)
                currentIndex = random.Next(vertexPoints);

            prevIndex = currentIndex;
            return currentIndex;
        }

        public static void Reset()
        {
            prevIndex = 0;
            currentIndex = 0;
        }


    }
}
