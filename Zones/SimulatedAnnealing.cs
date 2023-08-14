using Furniture;
using Zones;

namespace RoomClass.Zones
{
    //TODO Implement Singleton pattern for SimulatedAnnealing

    public class SimulatedAnnealing
    {
        // T = MaxDiff * 1.2
        public double Temperature { get; set; }
        public double TempDiff { get; set; }
        public double MaxDiff { get; set; }
        public double MinStep { get; set; }
        public double MaxStep { get; set; }
        public double StepDecreaseRatio { get; set; } = 0.95;
        public double TempDecreaseRatio { get; set; } = 0.9;
        public int NumTrials { get; set; } = 500;
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
            
            Temperature = DetermineInitialTemp();
        }

        private double DetermineInitialTemp()
        {
            List<SolutionClass> solutions = new(NumTrials);

            for (int i = 0; i < NumTrials; i++)
            {
                solutions.Add(InitialSolution.GenerateNeighbour(MaxStep));
            }

            List<SolutionClass> randomSolutions = new(NumTrials);

            foreach (var item in solutions)
            {
                randomSolutions.Add(item.GenerateNeighbour(MaxStep));
            }

            return (randomSolutions.Max(s => s.Cost) - randomSolutions.Min(s => s.Cost)) * 1.2;
        }




        public SolutionClass Launch(SolutionClass InitialSolution)
        {
            Random random = new();

            #region Simulated Annealing
            while (TempDiff > 0.1 || TempDiff < 0)
            {
                InitialSolution.Cost = CurrentSolution.Cost;
                for (int i = 0; i < IterPerTemp; i++)
                {
                    NeighbourSolution = CurrentSolution.GenerateNeighbour(MaxStep);

                    if (NeighbourSolution.Cost < CurrentSolution.Cost)
                    {
                        CurrentSolution = NeighbourSolution;
                    }

                    else if (random.NextDouble() < Math.Exp(CurrentSolution.Cost - NeighbourSolution.Cost) / Temperature)
                    {
                        CurrentSolution = NeighbourSolution;
                    }

                }

                Temperature *= TempDecreaseRatio;
                MaxStep = Math.Min(MaxStep * StepDecreaseRatio, MinStep);
                TempDiff = InitialSolution.Cost - CurrentSolution.Cost;
            }
            #endregion

            return CurrentSolution;

        }
    }
}
