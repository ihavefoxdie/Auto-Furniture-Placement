using Interfaces;
using Rooms;

namespace GeneticAlgorithm
{
    public class GeneticAlgoritm
    {
        public List<IPolygonContainer> Container {get; private set;}
        private bool KeepUp {get; set;}
        public int Generation {get; private set;}

        public GeneticAlgoritm(IPolygonContainer container)
        {
            Container = new();
            KeepUp = true;
            for (int i = 0; i < 6; i++)
            {
                Container.Add(container);
            }
        }
        //TODO CROSSOVER AND MUTATION ALGORITHMS
        public IPolygonContainer? Start()
        {
            while(KeepUp)
            {
                Container = Container.OrderBy(container => container.Penalty).ToList();

                int transfer = (70*Container.Count)/100;

                List<IPolygonContainer> newContainersSet = SUS(transfer, Container);

                transfer = Container.Count - transfer;
            }
            return null;
        }

        private static List<IPolygonContainer> SUS(int amountToKeep, List<IPolygonContainer> containers)
        {
            Random a = new();

            double totalPenalty = 0;
            for (int i = 0; i < containers.Count; i++)
                totalPenalty+= containers[i].Penalty;
            
            int distance = (int)(totalPenalty/amountToKeep);

            List<int> pointers = new();
            for (int i = 0; i < amountToKeep; i++)
                pointers.Add(a.Next(distance) + i*distance);
            
            return RWS(containers, pointers);
        }

        private static List<IPolygonContainer> RWS(List<IPolygonContainer> containers, List<int> pointers)
        {
            List<IPolygonContainer> containersToKeep = new();

            for (int i = 0; i < pointers.Count; i++)
            {
                int selector = -1;
                double sum = 0;
                while (sum < pointers[i])
                {
                    selector++;
                    sum += containers[selector].Penalty;
                }
                containersToKeep.Add(containers[selector]);   
            }

            return containersToKeep;
        }
    }
}