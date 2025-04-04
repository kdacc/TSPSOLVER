using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPSOLVER
{
    public static class NearestNeighbor
    {
        public static (int[], double) NearestNeighborTSP(double[,] distanceMatrix)
        {
            int numOfCities = distanceMatrix.GetLength(0);
            bool[] visited = new bool[numOfCities];
            int[] route = new int[numOfCities + 1];
            double totalDistance = 0;

            int visitedCities = 1;
            route[0] = 0;
            visited[0] = true;

            do
            {
                double minDistance = double.MaxValue;
                int nearestCity = -1;

                for (int j = 0; j < numOfCities; j++)
                {
                    if (!visited[j] && distanceMatrix[route[visitedCities - 1], j] < minDistance)
                    {
                        minDistance = distanceMatrix[route[visitedCities - 1], j];
                        nearestCity = j;
                    }
                }

                visitedCities++;
                route[visitedCities - 1] = nearestCity;
                totalDistance += minDistance;
                visited[nearestCity] = true;

            } while (visitedCities < numOfCities);

            totalDistance += distanceMatrix[route[numOfCities - 1], route[0]];
            route[numOfCities] = route[0];

            return (route, totalDistance);
        }
    }
}
