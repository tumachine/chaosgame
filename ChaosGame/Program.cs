using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos
{
    class Program
    {
        static void Main(string[] args)
        {
            ChaosGame game = new ChaosGame(720, 720);
            //game.DrawChaos(5, ChaosGameDelegates.CurrentNotPrev, "1.bmp", 1000000);
            //game.DrawChaos(6, ChaosGameDelegates.AnyRandomVertex, "2.bmp", 1000000);

            // create 12 variations of chaos game
            for (int i = 3; i <= 8; i++)
            {
                game.DrawChaos(i, ChaosGameDelegates.CurrentNotPrev, $"{i}-notPrev.bmp", 1000000);
                game.DrawChaos(i, ChaosGameDelegates.AnyRandomVertex, $"{i}-anyRandom.bmp", 1000000);
            }

            //Process.Start("C:/ProgramData/Microsoft/Windows/Start Menu/Programs/Accessories/paint.exe", "C:/Users/Tumen/source/repos/Homework19Tumen/Homework19Tumen/bin/Debug/ChaosGame.bmp");
        }
    }
}
