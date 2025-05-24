using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPSOLVER
{
    public static class NearestNeighbor
    {
        public static (int[], double, int iterations) NearestNeighborTSP(double[,] distanceMatrix)
        {
            int numOfCities = distanceMatrix.GetLength(0);
            int iterations = 0;

            if (numOfCities == 0)
            {
                return (new int[0], 0.0, 0);
            }
            if (numOfCities == 1)
            {
                return (new int[] { 0, 0 }, 0.0, 0); 
            }

            bool[] visited = new bool[numOfCities];
            int[] route = new int[numOfCities + 1]; 
            double totalDistance = 0.0;

            int currentCityIndexInRoute = 0; 
            route[currentCityIndexInRoute] = 0; 
            visited[0] = true;
            int citiesVisitedCount = 1; 
            int lastAddedCity = 0; 

            while (citiesVisitedCount < numOfCities)
            {
                iterations++;
                double minDistance = double.MaxValue;
                int nearestCity = -1;

                for (int j = 0; j < numOfCities; j++)
                {
                    if (!visited[j] && distanceMatrix[lastAddedCity, j] < minDistance)
                    {
                        minDistance = distanceMatrix[lastAddedCity, j];
                        nearestCity = j;
                    }
                }

                if (nearestCity == -1)
                {
                    Console.WriteLine("Помилка: Не вдалося знайти наступне невідвідане місто.");
                    for (int k = citiesVisitedCount; k < numOfCities; k++) route[k + 1] = -1; // +1 бо route[0] вже є
                    route[numOfCities] = route[0]; 
                    return (route, double.NaN, iterations);
                }

                currentCityIndexInRoute++;
                route[currentCityIndexInRoute] = nearestCity;
                totalDistance += minDistance;
                visited[nearestCity] = true;
                lastAddedCity = nearestCity;
                citiesVisitedCount++;
            }
            totalDistance += distanceMatrix[lastAddedCity, route[0]];
            route[numOfCities] = route[0]; 
            return (route, totalDistance, iterations);
        }
    }
}