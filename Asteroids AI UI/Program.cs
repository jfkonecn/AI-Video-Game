using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AsteroidsHandler;
using NeuralNetwork;

namespace AsteroidsAIUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Windows.MainMenu());
           
            //FeedForwardNet net = new FeedForwardNet( 100,200,100);

            //for (int j = 0; j < 100; j++)
            //{
            //    DateTime startTime = DateTime.Now;
            //    Console.Write("Input:\n");
            //    for (int i = 0; i < net.InputArray.Length; i++)
            //    {
            //        net.InputArray[i] = 0.5;
            //        Console.Write("{0}\t", net.InputArray[i]);
            //    }
            //    Console.Write("\n");
            //    net.calculateResults();
            //    Console.Write("Output:\n");
            //    for (int i = 0; i < net.OutputArray.Length; i++)
            //    {
            //        Console.Write("{0}\t", net.OutputArray[i]);
            //    }
            //    Console.Write("Calculation ran for {0} secs\n",(DateTime.Now - startTime).TotalSeconds);
            //    startTime = DateTime.Now;
            //    net.adjustWeights();
            //    Console.Write("Adjusting Weights ran for {0} secs\n\n", (DateTime.Now - startTime).TotalSeconds);

            //}



        }

      

    }
}
