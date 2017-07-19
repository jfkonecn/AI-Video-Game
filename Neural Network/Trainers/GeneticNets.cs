using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Trainers
{
    public class GeneticNets
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputs">total number of net inputs</param>
        /// <param name="outputs">total number of net outputs</param>
        /// <param name="numLayers">total number of net layers</param>
        /// <param name="totalNets">the total number of neural nets to be tested</param>
        public GeneticNets(int inputs, int outputs, int numLayers, int totalNets)
        {
            this.AllNets = new FeedForwardNet[totalNets];
            this.NetScores = new double[totalNets];
            for (int i = 0; i < totalNets; i++)
            {
                this.AllNets[i] = new FeedForwardNet(inputs, outputs, numLayers);
            }
            this.CurrentNetIdx = 0;
            this.CurrentGenerationNum = 1;
        }
        /****************************************************************************
        * Properties
        *****************************************************************************/
        /// <summary>
        /// stores all nerual nets which are used to control the ship
        /// </summary>
        private AbstractNet[] AllNets { get; set; }

        /// <summary>
        /// store the index in allNets of the neural net being used
        /// </summary>
        private int CurrentNetIdx { get; set; }

        public int  GetCurrentNetIdx { get { return this.CurrentNetIdx; } }
        public int GetGenerationNum { get { return this.CurrentGenerationNum; } }
        public int GetTotalNets { get{ return this.AllNets.Length; } }

        /// <summary>
        /// Which generation this is
        /// </summary>
        private int CurrentGenerationNum { get; set; }

        /// <summary>
        /// The current neural net being used
        /// </summary>
        public AbstractNet CurrentNet
        {
            get
            {
                return this.AllNets[this.CurrentNetIdx];
            }
            set
            {
                this.AllNets[this.CurrentNetIdx] = value;
            }
        }

        /// <summary>
        /// stores the scores for all nets
        /// </summary>
        private double[] NetScores { get; set; }


        /****************************************************************************
        * Methods
        *****************************************************************************/
        /// <summary>
        /// sort nets by score using quicksort
        /// </summary>
        /// <param name="lo"></param>
        /// <param name="hi"></param>
        private void sortNetsByScore(int lo, int hi)
        {
            if (lo < hi)
            {
                int p = partition(lo, hi);
                sortNetsByScore(lo, p - 1);
                sortNetsByScore(p + 1, hi);
            }
        }

        /// <summary>
        /// helper function for sortNetsByScore
        /// </summary>
        /// <param name="lo"></param>
        /// <param name="hi"></param>
        private int partition(int lo, int hi)
        {
            double pivot = this.NetScores[hi];
            int i = lo - 1;
            for (int j = lo; j < hi; j++)
            {
                if (this.NetScores[j] <= pivot)
                {
                    i = i + 1;
                    if (i != j)
                    {
                        swap(i, j);
                    }
                }
            }
            swap(i + 1, hi);
            return i + 1;
        }

        /// <summary>
        /// swap nets in array
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private void swap(int i, int j)
        {
            double tempNum = this.NetScores[i];
            this.NetScores[i] = this.NetScores[j];
            this.NetScores[j] = tempNum;

            AbstractNet tempNet = this.AllNets[i];
            this.AllNets[i] = this.AllNets[j];
            this.AllNets[j] = tempNet;
        }

        /// <summary>
        /// moves on to the next neural net
        /// </summary>
        public void goToNextNet()
        {
            this.CurrentNetIdx++;
            //make next generation
            if (this.CurrentNetIdx == AllNets.Length)
            {
                sortNetsByScore(0, AllNets.Length - 1);
                this.CurrentNetIdx = 0;
                for(int i = 0; i < AllNets.Length / 2; i++)
                {
                    AllNets[i] = AllNets[AllNets.Length - 1].copy();
                    AllNets[i].adjustWeights();
                }
                for (int i = 0; i < NetScores.Length; i++)
                {
                    this.NetScores[i] = 0;
                }
                this.CurrentGenerationNum++;
            }
        }

        /// <summary>
        /// sets the score for the current neural net
        /// </summary>
        /// <param name="score"></param>
        public void setCurrentScore(double score)
        {
            this.NetScores[this.CurrentNetIdx] = score;
        }
    }
}
