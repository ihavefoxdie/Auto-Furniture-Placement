using Zones;
using RoomClass;

namespace RoomClass.Zones
{
    //TODO Implement Singleton pattern for SimulatedAnnealing

    internal class SimulatedAnnealing
    {
        // T = MaxDiff * 1.2
        public double Temperature { get; set; }
        public double TempDiff { get; set; }
        public double MaxDiff { get; set; }
        public double MinStep { get; set; }

        //TODO MaxStep = room_min_dimension/2
        public double MaxStep { get; set; }
        public double StepDecreaseRatio { get; set; } = 0.95;
        public double TempDecreaseRatio { get; set; } = 0.9;
        public int NumTrials { get; set; } = 200;
        public int IterPerTemp { get; set; } = 100;

        public int RoomWidth { get; set; }
        public int RoomHeight { get; set; }

        //TODO Create Initial solution instance inside ctor
        public SimulatedAnnealing(List<AnnealingZone> annealingZones, int aisle, int roomWidth, int roomHeight )
        {
            RoomHeight = roomHeight;
            RoomWidth = roomWidth;

            InitialSolution = new SolutionClass(annealingZones, aisle, RoomWidth, RoomHeight);
            Temperature = DetermineInitialTemp();
        }

        private double DetermineInitialTemp()
        {
            Random random = new Random();

            //TODO Create a full ctor for SolutionClass
            List<SolutionClass> solutions = new(200);

            for (int i = 0; i < NumTrials; i++)
            {




            }

            while (NumTrials > 0)
            {
                if (random.Next(10) < 7)
                {

                }

                NumTrials--;
            }


            throw new NotImplementedException();


        }

        public SolutionClass InitialSolution { get; set; }
        public SolutionClass CurrentSolution { get; set; }
        public SolutionClass NeighbourSolution { get; set; }



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
