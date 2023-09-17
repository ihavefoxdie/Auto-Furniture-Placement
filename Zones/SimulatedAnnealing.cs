using Furniture;
using Rooms;
using ScottPlot;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Zones;

namespace RoomClass.Zones
{
    //TODO Implement Singleton pattern for SimulatedAnnealing
    public class SimulatedAnnealing
    {
        // T = MaxDiff * 1.2
        [JsonIgnore]
        public double Temperature { get; set; }
        [JsonIgnore]

        public double CostDiff { get; set; }
        [JsonIgnore]
        public double MaxDiff { get; set; }
        [JsonIgnore]
        public double MinStep { get; set; }
        [JsonIgnore]
        public double MaxStep { get; set; }
        [JsonIgnore]
        public double StepDecreaseRatio { get; set; } = 0.95;
        [JsonIgnore]
        public double TempDecreaseRatio { get; set; } = 0.9;
        [JsonIgnore]
        public int NumTrials { get; set; } = 100;
        [JsonIgnore]
        public int IterPerTemp { get; set; } = 200;
        [JsonIgnore]
        public int RoomWidth { get; set; }
        [JsonIgnore]
        public int RoomHeight { get; set; }
        [JsonIgnore]
        private List<GeneralFurniture> Doors { get; set; }
        [JsonIgnore]
        private List<AnnealingZone> AnnealingZones { get; set; }
        [JsonIgnore]
        public SolutionClass InitialSolution { get; set; }
        [JsonIgnore]
        public SolutionClass CurrentSolution { get; set; }
        [JsonIgnore]
        public SolutionClass NeighbourSolution { get; set; }

        [JsonInclude]
        public List<double> OverlappingPenalty { get; set; } = new();

        [JsonInclude]
        public List<double> FreeSpacePenalty { get; set; } = new();

        [JsonInclude]
        public List<double> ZoneShapePenalty { get; set; } = new();

        [JsonInclude]
        public List<double> SpaceRatioPenalty { get; set; } = new();

        [JsonInclude]
        public List<double> ByWallPenalty { get; set; } = new();

        [JsonInclude]
        public List<double> DoorSpacePenalty { get; set; } = new();


        public SimulatedAnnealing(Room room)
        {
            RoomHeight = room.ContainerHeight;
            RoomWidth = room.ContainerWidth;
            Doors = room.Doors;

            List<Zone> list = new();

            // List contains distinct zones
            List<string> unique = new();

            foreach (var item in room.FurnitureArray)
            {
                unique.Add(item.Data.Zone);
            }

            unique = unique.Distinct().ToList();

            foreach (var item in unique)
            {
                Zone zone = new(room.FurnitureArray.ToList(), item);
                list.Add(zone);
            }


            MaxStep = int.Min(RoomHeight, RoomWidth) / 2;
            AnnealingZones = new List<AnnealingZone>(list.Count);

            foreach (Zone zone in list)
            {
                AnnealingZones.Add(new AnnealingZone(zone));
            }



            InitialSolution = new SolutionClass(AnnealingZones, room.Aisle, RoomWidth, RoomHeight, room.Doors);
            InitialSolution.PrepareSolutionForSA();

            //Temperature = DetermineInitialTemp();
            Temperature = 1000;

        }


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
            //InitialSolution.PrepareSolutionForSA();

            //Temperature = DetermineInitialTemp();
            Temperature = 50;
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




        public SolutionClass Launch()
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
                for (int i = 0; i < IterPerTemp; i++)
                {
                    initCost = CurrentSolution.Cost;
                    cost.Add(CurrentSolution.Cost);
                    NeighbourSolution = SolutionClass.GenerateNeighbour(MaxStep, CurrentSolution);
                    probability = (float)Math.Exp(-Math.Abs(CurrentSolution.Cost - NeighbourSolution.Cost) / Temperature);
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

                    Temperature *= TempDecreaseRatio;
                    //MaxStep = Math.Max(MaxStep * StepDecreaseRatio, InitialSolution.Aisle);
                    MaxStep = 1;
                    CostDiff = initCost - CurrentSolution.Cost;
                }

                iterAmount--;
            }
            while ((CostDiff > 0.1 || CostDiff < 0)/*&& iterAmount > 0*/);

            #endregion
            var costArray = cost.ToArray();
            double[] iterations = DataGen.Consecutive(cost.Count);

            PrintGraph(iterations, costArray, "AnnealingGraph");
            return CurrentSolution;

        }

        public static void AnnealingDataSaver(SimulatedAnnealing simulatedAnnealing)
        {
            #region GraphView
            string fileName = "AnnealingGraph.png";
            var filePath = Path.Combine(Environment.CurrentDirectory, fileName);
            Process photoViewer = new Process();
            photoViewer.StartInfo.UseShellExecute = true;
            photoViewer.StartInfo.FileName = fileName;
            photoViewer.Start();
            #endregion


            simulatedAnnealing.OverlappingPenalty = ZoneClassInfo.OverlappingPenalty.ToList();
            simulatedAnnealing.FreeSpacePenalty = ZoneClassInfo.FreeSpacePenalty.ToList();
            simulatedAnnealing.ZoneShapePenalty = ZoneClassInfo.ZoneShapePenalty.ToList();
            simulatedAnnealing.SpaceRatioPenalty = ZoneClassInfo.SpaceRatioPenalty.ToList();
            simulatedAnnealing.ByWallPenalty = ZoneClassInfo.ByWallPenalty.ToList();
            simulatedAnnealing.DoorSpacePenalty = ZoneClassInfo.DoorSpacePenalty.ToList();

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string temp = JsonSerializer.Serialize(simulatedAnnealing, options);
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "PenaltyData.json"), temp);
        }
    }
}
