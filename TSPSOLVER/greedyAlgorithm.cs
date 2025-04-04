using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPSOLVER
{
    public static class GreedyTSP
    {
        public static (int[], double) GreedyAlgorithm(double[,] distanceMatrix)
        {
            int numOfCities = distanceMatrix.GetLength(0);
            bool[] visited = new bool[numOfCities];
            int[] route = new int[numOfCities + 1];
            double totalDistance = 0;
            Random random = new Random();
            int currentCity = random.Next(numOfCities);
            route[0] = currentCity;
            visited[currentCity] = true;

            for (int i = 1; i < numOfCities; i++)
            {
                int nearestCity = -1;
                double minDistance = double.MaxValue;

                for (int j = 0; j < numOfCities; j++)
                {
                    if (!visited[j] && distanceMatrix[currentCity, j] < minDistance)
                    {
                        minDistance = distanceMatrix[currentCity, j];
                        nearestCity = j;
                    }
                }

                route[i] = nearestCity;
                totalDistance += minDistance;
                visited[nearestCity] = true;
                currentCity = nearestCity;
            }
            route[numOfCities] = route[0];
            totalDistance += distanceMatrix[currentCity, route[0]];

            return (route, totalDistance);

        }
    }
}
