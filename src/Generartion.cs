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

    private Room currentRoom;
    private List<Room> currentRooms = new List<Room>();
    private Room previousRoom;

    public void GenerateWorld(Room startRoom) {
        Random random = new Random();
        int chance;
        
        for (int i = 0; i <= 10; i++) {
            List<string> directionPoolCopy = directionPool;
            if (i == 0) {
                Room nextRoom = roomPool[random.Next(0, roomPool.Count)].Clone();
                nextRoom.PreviousRoom = startRoom;
                startRoom.AddExit("east", nextRoom);
                nextRoom.AddExit("west", startRoom);
                nextRoom.Chest.Put("stiches", new Item(1, "medical supplies"));
                directionPoolCopy.Remove("west");
                currentRoom = nextRoom;
                currentRooms.Add(currentRoom);
                continue;
            }
            
            List<Room> toBeAdded = new List<Room>();
            Console.WriteLine("text");
            foreach (var room in currentRooms) { 
                // Console.WriteLine(room.GetLongDescription()); 
                chance = 25;
                // Console.WriteLine(chance); 
                foreach (var dir in directionPoolCopy) { 
                    // Console.WriteLine(dir); 
                    Room nextRoom = roomPool[random.Next(0, roomPool.Count)].Clone(); 
                    nextRoom.PreviousRoom = room; 
                    // Console.WriteLine(nextRoom.GetLongDescription()); 
                    if (random.Next(1, 100) <= chance) { 
                        nextRoom.AddExit(dir.Equals("west") ? "east" : dir.Equals("east") ? "west" : dir.Equals("north") ? "south" : "north", room); 
                        if (!room.HasExit(dir)) { 
                            room.AddExit(dir, nextRoom);
                        } 
                        // Console.WriteLine("added exit");
                    }
                    toBeAdded.Add(nextRoom);
                    chance += 25; 
                    // Console.WriteLine(chance);
                }
                // Console.WriteLine(room.GetLongDescription());
            }
            currentRooms.Clear();
            Console.WriteLine(currentRooms.Count);
            foreach (var roomAdded in toBeAdded) {
                currentRooms.Add(roomAdded);
            }
            Console.WriteLine(currentRooms.Count);
        }
    }

    private List<Room> GetNextGenerationRooms(Room start) {
        Console.WriteLine(start.GetLongDescription());
        List<Room> rooms = new List<Room>();
        foreach (var dir in directionPool) {
            Console.WriteLine(dir);
            if (start.HasExit(dir)) {
                Console.WriteLine("has exit");
                rooms.Add(start.GetExit(dir));
                Console.WriteLine("added exit to dir          " + start.GetExit(dir));
            }
        }
        return rooms;
    }
}