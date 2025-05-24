using System.Collections.Generic;
using System.Linq; 

namespace TSPSOLVER
{
    public class TSPSolution
    {
        public List<int> Path { get; }
        public double TotalDistance { get; }
        public int Iterations { get; set; }

        public TSPSolution(List<int> path, double totalDistance, int iterations)
        {
            Path = path;
            TotalDistance = totalDistance;
            Iterations = iterations;
        }
    }

    public interface ITSPSolver
    {
        string Name { get; }
        TSPSolution Solve(double[,] weightsMatrix);
    }

    public class GreedyTSPSolver : ITSPSolver
    {
        public string Name => "Greedy Algorithm";

        public TSPSolution Solve(double[,] weightsMatrix)
        {
            (int[] pathArray, double totalDistance, int iterations) = GreedyTSP.GreedyAlgorithm(weightsMatrix);
            return new TSPSolution(pathArray.ToList(), totalDistance, iterations);
        }
    }

    public class NearestNeighborTSPSolver : ITSPSolver
    {
        public string Name => "Nearest Neighbor Algorithm";

        public TSPSolution Solve(double[,] weightsMatrix)
        {
            (int[] pathArray, double totalDistance, int iterations) = NearestNeighbor.NearestNeighborTSP(weightsMatrix);
            return new TSPSolution(pathArray.ToList(), totalDistance, iterations);
        }
    }

    public class SimulatedAnnealingTSPSolver : ITSPSolver
    {
        public string Name => "Simulated Annealing Algorithm";

        public TSPSolution Solve(double[,] weightsMatrix)
        {
            (int[] pathArray, double totalDistance, int iterations) = SimulatedAnnealing.SimulatedAnnealingAlgorithm(weightsMatrix);
            return new TSPSolution(pathArray.ToList(), totalDistance, iterations);
        }
    }
}