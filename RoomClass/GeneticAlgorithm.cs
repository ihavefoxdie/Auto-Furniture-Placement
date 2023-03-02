using System;
using System.Linq;


namespace RoomClass
{
    public class GeneticAlgoritm
    {
        public List<Room> Rooms {get; private set;}
        private bool KeepUp {get; set;}
        public int Generation {get; private set;}

        public GeneticAlgoritm(Room room)
        {
            Rooms = new();
            KeepUp = true;
            for (int i = 0; i < 6; i++)
            {
                Rooms.Add(room);
            }
        }

        public Room Start()
        {
            while(KeepUp)
            {
                Rooms = Rooms.OrderBy(room => room.Penalty).ToList();

                int transfer = (70*Rooms.Count)/100;

                List<Room> newRoomSet = SUS(transfer, Rooms);

                transfer = Rooms.Count - transfer;
            }
            return null;
        }

        private List<Room> SUS(int amountToKeep, List<Room> rooms)
        {
            Random a = new();

            double totalPenalty = 0;
            for (int i = 0; i < rooms.Count; i++)
                totalPenalty+= rooms[i].Penalty;
            
            int distance = (int)(totalPenalty/amountToKeep);

            List<int> pointers = new();
            for (int i = 0; i < amountToKeep; i++)
                pointers.Add(a.Next(distance) + i*distance);
            
            return RWS(rooms, pointers);
        }

        private List<Room> RWS(List<Room> rooms, List<int> pointers)
        {
            List<Room> rooms2keep = new();

            for (int i = 0; i < pointers.Count; i++)
            {
                int selector = -1;
                double sum = 0;
                while (sum < pointers[i])
                {
                    selector++;
                    sum += rooms[selector].Penalty;
                }
                rooms2keep.Add(rooms[selector]);   
            }

            return rooms2keep;
        }
    }
}