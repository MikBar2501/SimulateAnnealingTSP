using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulateAnnealingTSP
{
    class Program
    {
        public static Random rand = new Random();
        public static Values values;
        static void Main(string[] args)
        {
            Console.WriteLine("Set name of file: ");
            string name = Console.ReadLine();
            string nameFile = "TSP\\" + name;

            string pathFile = nameFile;
            Point[] points = TSPFileReader.ReadTspFile(pathFile);
            Graph graph = new Graph(points);


            double temp = 1000;
            double stoppingTemp = 0.00000001;
            double alpha = 0.9995;
            int stoppingIter = 400000;


            values = new Values(0, pathFile, "SA");
            values.StartTime();
            //Solve(graph, bees, 1000, (UInt32)(3));
            Solve(graph, temp, alpha, stoppingTemp, stoppingIter);
            values.StopTime();
            Console.WriteLine("End");
            Console.ReadLine();
        }

        public static void Solve(Graph graph, double t, double a, double sT, int sI)
        {
            int iteration = 0;
            double temperature = t;
            double alpha = a;

            List<UInt32> bestSolution = new List<UInt32>();
            List<UInt32> currSolution = new List<UInt32>();
            currSolution = GreedNN(graph);
            bestSolution = currSolution.ToList();

            double currentWeight = graph.CalculateRouteLength(currSolution);
            double initialWeight = currentWeight;
            double minWeight = currentWeight;

            //List<double> weightList = new List<double>();
            //weightList.Add(currentWeight);
            Console.WriteLine("Initial weight: {0}", currentWeight);

            while (temperature >= sT && iteration < sI)
            {
                List<UInt32> candidate = currSolution.ToList();
                int l = rand.Next(2, (int)graph.dimension - 1);
                int i = rand.Next(0, (int)graph.dimension - 1);

                if (l > i)
                {
                    for (int k = 0; k < Math.Abs(l - i); k++)
                    {
                        candidate[i + k] = currSolution[l - k];
                    }
                }
                else
                {
                    for (int k = 0; k < Math.Abs(i - l); k++)
                    {
                        candidate[l + k] = currSolution[i - k];
                    }
                }
                //Console.WriteLine(candidate.Count);
                //Console.WriteLine(graph.CalculateRouteLength(candidate));



                double candWeight = graph.CalculateRouteLength(candidate);
                if (candWeight < currentWeight)
                {
                    currentWeight = candWeight;
                    currSolution = candidate.ToList();
                    if (currentWeight < minWeight)
                    {
                        minWeight = currentWeight;
                        bestSolution = candidate.ToList();
                    }
                }
                else
                {
                    double r = rand.NextDouble();
                    if (r < AcceptanceProbability(candWeight, currentWeight, temperature, graph))
                    {
                        currentWeight = candWeight;
                        currSolution = candidate.ToList();
                    }
                }

                temperature *= alpha;
                iteration++;
                //weightList.Add(currentWeight);
                //Console.WriteLine(bestSolution.Count);
                //Console.WriteLine(graph.CalculateRouteLength(bestSolution));
                Console.WriteLine("Minimum weight {0} in iteration {1} with temperature {2}", minWeight, iteration, temperature);
                //Console.WriteLine()
            }
        }

        public static double AcceptanceProbability(double candWeight, double currWeight, double temp, Graph graph)
        {
            return Math.Exp(-Math.Abs(candWeight - currWeight) / temp);
        }



        public static List<UInt32> GreedNN(Graph graph)
        {
            List<UInt32> solution = new List<UInt32>();
            List<UInt32> unVisited = new List<UInt32>();
            int startPoint = rand.Next(0, (int)graph.dimension);
            UInt32 actualPoint = (UInt32)startPoint;
            for (UInt32 i = 0; i < graph.dimension; i++)
            {
                unVisited.Add(i);
            }
            unVisited.Remove(actualPoint);
            solution.Add(actualPoint);

            while (unVisited.Count != 0)
            {
                double nearestPath = 0;
                int nearestPoint = 0;

                foreach (int vert in unVisited)
                {
                    if (nearestPath == 0)
                    {
                        nearestPath = graph.edgeWeight[actualPoint, vert];
                        nearestPoint = vert;
                    }

                    if (nearestPath > graph.edgeWeight[actualPoint, vert])
                    {
                        nearestPath = graph.edgeWeight[actualPoint, vert];
                        nearestPoint = vert;
                    }
                }


                actualPoint = (UInt32)nearestPoint;
                unVisited.Remove(actualPoint);
                solution.Add(actualPoint);
            }

            return solution;
        }
    }
}
