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

    private List<Room> currentRooms = new List<Room>();

    public void GenerateWorld(Room startRoom) {
        Random random = new Random();
        int chance;
        int roomAmount = 0;
        
        for (int i = 0; i <= 31; i++) {
            List<string> directionPoolCopy = directionPool;
            if (i == 0) {
                Room nextRoom = roomPool[random.Next(0, roomPool.Count)].Clone();
                startRoom.AddExit("east", nextRoom);
                nextRoom.AddExit("west", startRoom);
                nextRoom.Chest.Put("stiches", new Item(1, "medical supplies"));
                directionPoolCopy.Remove("west");
                currentRooms.Add(nextRoom);
                continue;
            }
            
            List<Room> toBeAdded = new List<Room>();
            Console.WriteLine("text");
            foreach (var room in currentRooms) { 
                chance = 25;
                if (room.GetShortDescription().Contains("at the top of the staircase")) {
                    Room nextStairs = random.Next(0, 2) == 1 ? stairwell.Clone() : stairwellBottom.Clone();
                    if (!room.HasExit("down")) {
                        room.AddExit("down", nextStairs);
                        nextStairs.AddExit("up", room);
                        toBeAdded.Add(nextStairs);
                    }
                } else if (room.GetShortDescription().Contains("in a room with a staircase.")) {
                    Room nextStairsDown = random.Next(0, 2) == 1 ? stairwell.Clone() : stairwellBottom.Clone();
                    Room nextStairsUp = random.Next(0, 2) == 1 ? stairwell.Clone() : stairwellTip.Clone();
                    if (!room.HasExit("up")) { 
                        room.AddExit("up", nextStairsUp);
                        nextStairsUp.AddExit("down", nextStairsUp);
                        toBeAdded.Add(nextStairsUp);
                    }
                    if (!room.HasExit("down")) { 
                        room.AddExit("down", nextStairsDown);
                        nextStairsDown.AddExit("up", room);
                        toBeAdded.Add(nextStairsDown);
                    }
                    continue;
                } else if (room.GetShortDescription().Contains("at the bottom of the staircase")) {
                    Room nextStairs = random.Next(0, 2) == 1 ? stairwell.Clone() : stairwellTip.Clone();
                    if (!room.HasExit("up")) {
                        room.AddExit("up", nextStairs);
                        nextStairs.AddExit("down", room);
                        toBeAdded.Add(nextStairs);
                    }
                }
                foreach (var dir in directionPoolCopy) {
                    if (random.Next(1, 100) <= chance) {
                        Room roomPoolChosen = roomPool[random.Next(0, roomPool.Count)];
                        Room nextRoom = roomPoolChosen.Clone(); 
                        nextRoom.AddExit(dir.Equals("west") ? "east" : dir.Equals("east") ? "west" : dir.Equals("north") ? "south" : "north", room); 
                        if (!room.HasExit(dir)) { 
                            room.AddExit(dir, nextRoom);
                        }
                        toBeAdded.Add(nextRoom);
                    }
                    chance += 25; 
                }
            }
            currentRooms.Clear();
            Console.WriteLine(i);
            foreach (var roomAdded in toBeAdded) {
                currentRooms.Add(roomAdded);
            }
            Console.WriteLine(currentRooms.Count);
            roomAmount += currentRooms.Count;
        }
        Console.WriteLine(roomAmount);
    }
}
