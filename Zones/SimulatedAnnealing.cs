using Furniture;
using ScottPlot;
using Zones;

namespace RoomClass.Zones
{
    //TODO Implement Singleton pattern for SimulatedAnnealing

    public class SimulatedAnnealing
    {
        // T = MaxDiff * 1.2
        public double Temperature { get; set; }
        public double CostDiff { get; set; }
        public double MaxDiff { get; set; }
        public double MinStep { get; set; }
        public double MaxStep { get; set; }
        public double StepDecreaseRatio { get; set; } = 0.95;
        public double TempDecreaseRatio { get; set; } = 0.9;
        public int NumTrials { get; set; } = 400;
        public int IterPerTemp { get; set; } = 100;

        public int RoomWidth { get; set; }
        public int RoomHeight { get; set; }
        private List<GeneralFurniture> Doors { get; set; }
        private List<AnnealingZone> AnnealingZones { get; set; }
        public SolutionClass InitialSolution { get; set; }
        public SolutionClass CurrentSolution { get; set; }
        public SolutionClass NeighbourSolution { get; set; }

        //TODO Create Initial solution instance inside ctor

        public SimulatedAnnealing(List<Zone> zones, int aisle, int roomWidth, int roomHeight, List<GeneralFurniture> doors)
        {
            RoomHeight = roomHeight;
            RoomWidth = roomWidth;
            Doors = doors;

            MaxStep = int.Min(roomHeight, roomWidth) / 2;
            AnnealingZones = new List<AnnealingZone>(zones.Count);

            foreach (Zone zone in zones)
            {
                AnnealingZones.Add(new AnnealingZone(zone));
            }

            InitialSolution = new SolutionClass(AnnealingZones, aisle, RoomWidth, RoomHeight, doors);
            InitialSolution.PrepareSolutionForSA();

            Temperature = DetermineInitialTemp();

        }

        private double DetermineInitialTemp()
        {
            List<SolutionClass> solutions = new(NumTrials);

            for (int i = 0; i < NumTrials; i++)
            {
                solutions.Add(SolutionClass.GenerateNeighbour(MaxStep, InitialSolution));
            }

            List<SolutionClass> randomSolutions = new(NumTrials);

            foreach (var item in solutions)
            {
                randomSolutions.Add(SolutionClass.GenerateNeighbour(MaxStep, item));
            }

            return (randomSolutions.Max(s => s.Cost) - randomSolutions.Min(s => s.Cost)) * 1.2;
        }

        private void PrintGraph(double[] xAxis, double[] yAxis, string name)
        {
            var plt = new ScottPlot.Plot();

            plt.AddScatter(xAxis, yAxis);

            // Axes can be customized
            plt.XAxis.Label("Iteration");
            plt.YAxis.Label("Cost");
            plt.XAxis2.Label("Important Experiment");


            plt.SaveFig($"{name}.png");
        }




        public SolutionClass Launch(SolutionClass InitialSolution)
        {
            int iterAmount = 1;

            Random random = new();

            double initCost;
            float probability;

            List<double> cost = new(IterPerTemp);

            CurrentSolution = InitialSolution;
            #region Simulated Annealing

            do
            {
                initCost = CurrentSolution.Cost;
                for (int i = 0; i < IterPerTemp; i++)
                {
                    cost.Add(CurrentSolution.Cost);
                    NeighbourSolution = SolutionClass.GenerateNeighbour(MaxStep, CurrentSolution);
                    probability = (float) Math.Exp(-Math.Abs(CurrentSolution.Cost - NeighbourSolution.Cost) / Temperature);
                    Console.WriteLine("DIFF : " + $"{-Math.Abs(CurrentSolution.Cost - NeighbourSolution.Cost)}");
                    Console.WriteLine(probability);

                    if (NeighbourSolution.Cost <= CurrentSolution.Cost)
                    {
                        CurrentSolution = NeighbourSolution;
                    }


                    else if (random.NextDouble() < probability)
                    {
                        CurrentSolution = NeighbourSolution;
                    }

                }

                Temperature *= TempDecreaseRatio;
                MaxStep = Math.Max(MaxStep * StepDecreaseRatio, InitialSolution.Aisle);
                CostDiff = initCost - CurrentSolution.Cost;
                iterAmount--;
            }
            while ((CostDiff > 0.1 || CostDiff < 0) /*&& iterAmount > 0*/);

            #endregion
            var costArray = cost.ToArray();
            double[] iterations = DataGen.Consecutive(cost.Count);

            PrintGraph(iterations, costArray, "AnnealingGraph");
            return CurrentSolution;

        }
    }
}
