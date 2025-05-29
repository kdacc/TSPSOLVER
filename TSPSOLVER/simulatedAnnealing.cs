using System;
using System.Collections.Generic;
using System.Linq;

namespace TSPSOLVER
{
    public static class SimulatedAnnealing
    {
        private const double DefaultInitialTemperature = 1000;
        private const double DefaultCoolingRate = 0.95;
        private const double DefaultStoppingTemperature = 0.01; 
        private const int DefaultIterationsPerTemperature = 30;
        private static readonly Random random = new Random();

        public static (int[] path, double distance, int iterations) SimulatedAnnealingAlgorithm(
            double[,] distanceMatrix,
            double initialTemperature = DefaultInitialTemperature, 
            double coolingRate = DefaultCoolingRate,            
            double stoppingTemperature = DefaultStoppingTemperature, 
            int iterationsPerTemperature = DefaultIterationsPerTemperature)
        {
            int numOfCities = distanceMatrix.GetLength(0);

            if (numOfCities == 0) return (new int[0], 0.0, 0);
            if (numOfCities == 1) return (new int[] { 0, 0 }, 0.0, 0);


            int[] currentSolution = GenerateRandomPath(numOfCities);
            double currentEnergy = CalculatePathDistance(currentSolution, distanceMatrix);

            int[] bestSolution = (int[])currentSolution.Clone();
            double bestEnergy = currentEnergy;

            double temperature = initialTemperature; 
            int noImprovementCount = 0;
            int totalIterations = 0;

            while (temperature > stoppingTemperature)
            {
                for (int i = 0; i < iterationsPerTemperature; i++)
                {
                    totalIterations++;
                    int[] newSolution = GenerateNeighborPath(currentSolution);
                    double newEnergy = CalculatePathDistance(newSolution, distanceMatrix);
                    double deltaEnergy = newEnergy - currentEnergy;

                    if (deltaEnergy < 0 || Math.Exp(-deltaEnergy / temperature) > random.NextDouble())
                    {
                        currentSolution = newSolution;
                        currentEnergy = newEnergy;

                        if (currentEnergy < bestEnergy)
                        {
                            bestSolution = (int[])currentSolution.Clone();
                            bestEnergy = currentEnergy;
                            noImprovementCount = 0; 
                        }
                    }
                }

                temperature *= coolingRate; 
                if (noImprovementCount > numOfCities * 20)
                {
                    currentSolution = GenerateRandomPath(numOfCities); 
                    currentEnergy = CalculatePathDistance(currentSolution, distanceMatrix);
                    if (currentEnergy < bestEnergy)
                    {
                        bestSolution = (int[])currentSolution.Clone();
                        bestEnergy = currentEnergy;
                    }
                    temperature = initialTemperature * 0.7; 
                    noImprovementCount = 0;
                }
            }

            int[] closedBestPath = new int[numOfCities + 1];
            Array.Copy(bestSolution, closedBestPath, numOfCities);
            closedBestPath[numOfCities] = bestSolution[0];

            return (closedBestPath, bestEnergy, totalIterations);
        }

        private static int[] GenerateRandomPath(int numOfCities)
        {
            List<int> cities = Enumerable.Range(0, numOfCities).ToList();
            for (int i = cities.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (cities[i], cities[j]) = (cities[j], cities[i]);
            }
            return cities.ToArray();
        }

        private static int[] GenerateNeighborPath(int[] currentPath)
        {
            int[] newPath = (int[])currentPath.Clone();
            int numOfCities = newPath.Length;
            if (numOfCities < 2)
            {
                return newPath;
            }
            
            int method = random.Next(0, 2);

            switch (method)
            {
                case 0: // Swap
                    if (numOfCities < 2) break;
                    int pos1 = random.Next(numOfCities);
                    int pos2 = random.Next(numOfCities);
                    while (pos1 == pos2)
                    {
                        pos2 = random.Next(numOfCities);
                    }
                    (newPath[pos1], newPath[pos2]) = (newPath[pos2], newPath[pos1]);
                    break;

                case 1: //Reverse segment
                    if (numOfCities < 3) break;
                    int idx1 = random.Next(numOfCities);
                    int idx2 = random.Next(numOfCities);
                    while (idx1 == idx2 || Math.Abs(idx1 - idx2) < 1 && numOfCities > 2)
                    {
                        idx2 = random.Next(numOfCities);
                    }

                    int start = Math.Min(idx1, idx2);
                    int end = Math.Max(idx1, idx2);

                    Array.Reverse(newPath, start, end - start + 1);
                    break;
            }
            return newPath;
        }

        private static double CalculatePathDistance(int[] path, double[,] distanceMatrix)
        {
            double totalDistance = 0.0;
            int numUniqueCitiesInPath = path.Length;

            if (numUniqueCitiesInPath == 0) return 0.0;
            if (numUniqueCitiesInPath == 1) return 0.0;

            for (int i = 0; i < numUniqueCitiesInPath - 1; i++)
            {
                totalDistance += distanceMatrix[path[i], path[i + 1]];
            }
            totalDistance += distanceMatrix[path[numUniqueCitiesInPath - 1], path[0]];

            return totalDistance;
        }
    }
}