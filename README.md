# TSPSolver 

A C# WPF application designed to calculate, optimize, visualize, and analyze routing solutions for the classic **Traveling Salesman Problem (TSP)**. 

## Architecture & Overview
The Traveling Salesman Problem asks: *"Given a list of cities and the distances between each pair of cities, what is the shortest possible route that visits each city exactly once and returns to the origin city?"*

Because the TSP is an NP-hard problem, finding the absolute optimal route via brute force becomes computationally impossible as the number of nodes grows. This project tackles that challenge by providing an **interactive graphical interface (GUI)** to implement and benchmark multiple heuristic and metaheuristic algorithms, finding near-optimal paths while analyzing computational trade-offs.

## Key Features
* **Interactive Visualization:** Users can dynamically generate cities on a 2D Canvas and visually observe the construction of the optimal route (rendered with red lines).
* **Flexible Data Input:** Supports both manual entry of distances (weights) between cities and automatic randomized generation.
* **Data Export:** The final calculated route, total distance, and the complete distance matrix can be easily exported to a `.txt` file for further analysis.
* **Modular Architecture:** Built with strict Object-Oriented Programming (OOP) principles using `ITSPSolver` interfaces and models like `City` and `TSPSolution` to allow seamless swapping of algorithms via the Strategy Pattern.

## Algorithmic Implementations

This solver implements three distinct approaches to navigate the trade-off between execution iterations and route optimality:

### 1. Nearest Neighbor
* **Approach:** Starting from a random node, the algorithm consistently visits the nearest unvisited node until a tour is complete.
* **Time Complexity:** $O(n^2)$
* **Characteristics:** Highly performant with the lowest number of iterations, but heavily prone to falling into local optima.

### 2. Greedy Algorithm
* **Approach:** Iteratively adds the shortest available edge to the tour globally, provided it does not create a premature cycle or a vertex with a degree greater than 2.
* **Time Complexity:** $O(n^2)$
* **Characteristics:** Generally yields a shorter total distance than Nearest Neighbor, but requires more computational iterations to validate edge constraints.

### 3. Simulated Annealing
* **Approach:** A probabilistic technique inspired by the annealing process in metallurgy. It starts with a sub-optimal route and iteratively attempts random mutations. Worse solutions are occasionally accepted based on a "temperature" parameter that decreases over time, allowing the algorithm to escape local optima.
* **Time Complexity:** $O(n)$ per iteration (execution time is heavily dependent on the cooling schedule).
* **Characteristics:** Requires the highest number of iterations, but consistently produces the most highly optimized routing among the three implementations.

## Performance Analysis & Benchmarking

The algorithms were benchmarked against a randomly generated dataset of **$N = 50$ cities**.

| Algorithm | Total Iterations | Final Route Distance | Optimality | Time Complexity |
| :--- | :--- | :--- | :--- | :--- |
| **Nearest Neighbor** | 49 | 473 | Baseline | $O(n^2)$ |
| **Greedy Algorithm** | 1,106 | 432 | +8.6% vs Baseline | $O(n^2)$ |
| **Simulated Annealing**| 6,750 | 263 | +44.4% vs Baseline | $O(n)$ per iteration |

### Key Takeaways:
* **Speed vs. Accuracy:** The *Nearest Neighbor* algorithm is the fastest (only 49 iterations) but yields the worst route. *Simulated Annealing* takes significantly more iterations (6,750) but drastically improves the route optimality (reducing distance from 473 to 263).
* **Algorithmic Trade-offs:** The project clearly demonstrates that advanced metaheuristics (Simulated Annealing) are strictly required for high-quality TSP solutions, justifying the extra computational cost.

## Tech Stack & Design Patterns
* **Language:** C#
* **Framework:** .NET Framework
* **UI/UX:** WPF (Windows Presentation Foundation)
* **Design Principles:** Object-Oriented Programming (OOP), Strategy Pattern.
