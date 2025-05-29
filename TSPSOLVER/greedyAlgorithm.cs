using System;
using System.Collections.Generic;
using System.Linq;
namespace TSPSOLVER
{
    public static class GreedyTSP
    {
        public static (int[], double, int iterations) GreedyAlgorithm(double[,] distanceMatrix)
        {

            int numOfCities = distanceMatrix.GetLength(0);
            int iterations = 0;
            if (numOfCities < 3)
            {
                if (numOfCities == 0) return (new int[0], 0, 0);
                if (numOfCities == 1) return (new int[] { 0, 0 }, 0, 0);
                if (numOfCities == 2) return (new int[] { 0, 1, 0 }, distanceMatrix[0, 1] + distanceMatrix[1, 0], 0);
            }

            List<(int from, int to, double distance)> edges = new List<(int, int, double)>();
            for (int i = 0; i < numOfCities; i++)
            {
                for (int j = i + 1; j < numOfCities; j++)
                {
                    edges.Add((i, j, distanceMatrix[i, j]));
                }
            }

            edges.Sort((e1, e2) => e1.distance.CompareTo(e2.distance));

            Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
            for (int i = 0; i < numOfCities; i++)
            {
                graph[i] = new List<int>();
            }

            double totalDistance = 0.0;
            int edgesAdded = 0;

            foreach (var edge in edges)
            {
                iterations++;
                if (edgesAdded >= numOfCities) break;

                int u = edge.from;
                int v = edge.to;

                // Перевіряємо, чи можна додати це ребро
                // 1. Не повинно бути більше 2 ребер, інцидентних вершині
                if (graph[u].Count >= 2 || graph[v].Count >= 2)
                    continue;

                // 2. Додання ребра не повинно створювати цикл, якщо ми ще не додали всі міста
                if (graph[u].Count > 0 && graph[v].Count > 0)
                {
                    // Перевіряємо, чи не створить це ребро передчасний цикл
                    if (WillFormPrematureCycle(graph, u, v, numOfCities, edgesAdded))
                        continue;
                }

                graph[u].Add(v);
                graph[v].Add(u);
                totalDistance += edge.distance;
                edgesAdded++;
            }

            if (edgesAdded < numOfCities)
            {
                List<int> endVertices = new List<int>();
                for (int i = 0; i < numOfCities; i++)
                {
                    if (graph[i].Count == 1)
                        endVertices.Add(i);
                }

                if (endVertices.Count == 2)
                {
                    int u = endVertices[0];
                    int v = endVertices[1];
                    graph[u].Add(v);
                    graph[v].Add(u);
                    totalDistance += distanceMatrix[u, v];
                }
            }

            int[] route = new int[numOfCities + 1];
            bool[] visited = new bool[numOfCities];

            int currentCity = 0; 
            route[0] = currentCity;
            visited[currentCity] = true;

            for (int i = 1; i < numOfCities; i++)
            {
                foreach (int neighbor in graph[currentCity])
                {
                    if (!visited[neighbor])
                    {
                        route[i] = neighbor;
                        visited[neighbor] = true;
                        currentCity = neighbor;
                        break;
                    }
                }
            }

            route[numOfCities] = route[0]; 

            totalDistance = 0;
            for (int i = 0; i < numOfCities; i++)
            {
                totalDistance += distanceMatrix[route[i], route[i + 1]];
            }

            return (route, totalDistance, iterations);
        }

        private static bool WillFormPrematureCycle(Dictionary<int, List<int>> graph, int u, int v, int numOfCities, int edgesAdded)
        {
            // Якщо це останнє ребро, то цикл дозволений
            if (edgesAdded == numOfCities - 1)
                return false;

            // Перевіряємо, чи існує шлях від u до v (крім прямого ребра)
            HashSet<int> visited = new HashSet<int>();
            visited.Add(u);

            return PathExists(graph, u, v, visited);
        }

        private static bool PathExists(Dictionary<int, List<int>> graph, int current, int target, HashSet<int> visited)
        {
            foreach (int neighbor in graph[current])
            {
                if (neighbor == target)
                    return true;

                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    if (PathExists(graph, neighbor, target, visited))
                        return true;
                }
            }

            return false;
        }
    }
}