namespace Zuul;

class Generartion
{
    private List<Room> roomPool = new List<Room>();
    private List<string> directionPool = new List<string>();
    private Room hallway = new Room("in a long winding hallway.");
    private Room stairwell = new Room("in a room with a staircase.");
    private Room stairwellTip = new Room("at the top of the staircase");
    private Room stairwellBottom = new Room("at the bottom of the staircase");
    private Room roomChest = new Room("a room with a chest");
    private Room roomEmpty = new Room("an empty room");
    private Room roomWithEnemy = new Room("a room with an enemy");
    private Room trapRoomEmpty = new Room("an empty room");
    private Room trapRoomChest = new Room("a room with a chest");
    private Room celler = new Room("a room filled with beer fats");
    
    public Generartion()
    {
        roomPool.Add(hallway);
        roomPool.Add(stairwell);
        roomPool.Add(stairwellBottom);
        roomPool.Add(stairwellTip);
        roomPool.Add(roomChest);
        roomPool.Add(roomEmpty);
        roomPool.Add(roomWithEnemy);
        roomPool.Add(trapRoomEmpty);
        roomPool.Add(trapRoomChest);
        roomPool.Add(celler);
        
        directionPool.Add("west");
        directionPool.Add("east");
        directionPool.Add("north");
        directionPool.Add("south");
    }

    public void GenerateWorld(Room startRoom) {
        Random random = new Random();

        for (int i = 0; i <= 25; i++) {
            Console.WriteLine(i);
            Room room = roomPool[random.Next(0, roomPool.Count)].Clone();
            Console.WriteLine(room);
            List<string> directionPoolCopy = directionPool;
            Console.WriteLine(directionPoolCopy);
            if (i == 0) {
                startRoom.AddExit("east", room);
                Console.WriteLine("first");
                room.AddExit("west", startRoom);
                Item stiches = new Item(10, "medical supplies to patch up your wounds");
                room.Chest.Put(nameof(stiches), stiches);
                Console.WriteLine("efdefwe");
                directionPoolCopy.Remove("west");
                Console.WriteLine(directionPoolCopy);
            }

            int chance = 50;
            if (room.Equals(stairwell)) {
                Console.WriteLine("stairs");
                List<Room> stairRooms = new List<Room>();
                List<string> upOrDown = new List<string>();
                
                stairRooms.Add(stairwell);
                stairRooms.Add(stairwellBottom);
                stairRooms.Add(stairwellTip);
                Console.WriteLine(stairRooms.ToArray());
                
                upOrDown.Add("up");
                upOrDown.Add("down");
                Console.WriteLine(upOrDown.ToArray());

                foreach (var direction in upOrDown) {
                    Console.WriteLine(direction);
                    int randNumber = random.Next(1, 100);
                    Console.WriteLine(randNumber);
                    Console.WriteLine(chance);
                    Room nextStair = stairRooms[random.Next(0, stairRooms.Count)].Clone();
                    Console.WriteLine(nextStair);
                    if (randNumber <= chance) {
                        Console.WriteLine("added exit");
                        room.AddExit(direction, nextStair);
                    }
                    chance += 50;
                    Console.WriteLine(chance);
                }
            }

            chance = 10;
            Console.WriteLine(chance);
            foreach (var direction in directionPoolCopy) {
                Console.WriteLine(direction);
                int randNumber = random.Next(1, 100);
                Console.WriteLine(randNumber);
                Room nextRoom = roomPool[random.Next(0, roomPool.Count)].Clone();
                Console.WriteLine(nextRoom);
                if (randNumber <= chance) {
                    room.AddExit(direction, nextRoom);
                    Console.WriteLine("added exit");
                }
                chance += 30;
                Console.WriteLine(chance);
            }
        }
    }

    private List<Room> GetNextGenerationRoom(Room start) {
        List<Room> rooms = new List<Room>();
        foreach (var dir in directionPool) {
            if (start.HasExit(dir)) {
                rooms.Add(start.GetExit(dir));
            }
        }
        return rooms;
    }
}