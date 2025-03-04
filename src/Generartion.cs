namespace Zuul;

class Generartion
{
    // fields
    private List<Room> roomPool = new List<Room>();
    private List<string> directionPool = new List<string>();
    private Room hallway = new Room("in a long winding hallway.", "hallway");
    private Room stairwell = new Room("in a room with a staircase.", "staircase-middle");
    private Room stairwellTip = new Room("at the top of the staircase", "staircase-top");
    private Room stairwellBottom = new Room("at the bottom of the staircase", "staircase-bottom");
    private Room roomChest = new Room("a room with a chest", "chest-room");
    private Room roomEmpty = new Room("an empty room", "empty-room");
    private Room roomWithEnemy = new Room("a room with an enemy", "room-with-enemy");
    private Room trapRoomEmpty = new Room("an empty room", "empty-room");
    private Room trapRoomChest = new Room("a room with a chest", "chest-room");
    private Room celler = new Room("a room filled with beer fats", "cellar");
    
    // constructor
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

    // list for all rooms that have to get a new room
    private List<Room> currentRooms = new List<Room>();

    /// <summary>
    /// generates a new dungeon when called, it does this by using a for-loop that loops for 47 times.
    /// </summary>
    /// <param name="startRoom">The room where the generations should start at.</param>
    /// <param name="winRoom">The room that makes the player win.</param>
    public void GenerateWorld(Room startRoom, Room winRoom) {
        Random random = new Random(); // creates the randomizer.
        int chance; // an empty variable that gets assigned the chance for the randomizer to succeed.
        int roomAmount = 0; // the variable that stores all rooms, used for debugging.
        
        // the for loop for the generation
        for (int i = 0; i <= 47; i++) {
            List<string> directionPoolCopy = directionPool;
            if (i == 0) {
                Room nextRoom = roomPool[random.Next(0, roomPool.Count)].Clone();
                startRoom.AddExit("east", nextRoom);
                nextRoom.AddExit("west", startRoom);
                nextRoom.Chest.Put("stiches", new Item(1, "medical supplies", "stiches"));
                directionPoolCopy.Remove("west");
                currentRooms.Add(nextRoom);
                continue;
            }
            
            List<Room> toBeAdded= new List<Room>();
            foreach (var room in currentRooms) { 
                chance = 25;
                if (room.Name.Equals("staircase-top")) {
                    Room nextStairs = random.Next(0, 2) == 1 ? stairwell.Clone() : stairwellBottom.Clone();
                    if (!room.HasExit("down")) {
                        room.AddExit("down", nextStairs);
                        nextStairs.AddExit("up", room);
                        toBeAdded.Add(nextStairs);
                    }
                } else if (room.Name.Equals("staircase-middle")) {
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
                } else if (room.Name.Equals("staircase-bottom")) {
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
                        if (!room.HasExit(dir)) {
                            Room nextRoom = roomPoolChosen.Clone();
                            nextRoom.AddExit(
                                dir.Equals("west") ? "east" :
                                dir.Equals("east") ? "west" :
                                dir.Equals("north") ? "south" : "north", room);
                            room.AddExit(dir, nextRoom);

                            toBeAdded.Add(nextRoom);
                        }
                    }
                    chance += 30; 
                }
            }
            currentRooms.Clear();
            Console.WriteLine(i);
            foreach (var roomAdded in toBeAdded) {
                currentRooms.Add(roomAdded);
            }
            roomAmount += currentRooms.Count;
            if (i >= 47) {
                chance = 1;
                foreach (var room in currentRooms) {
                    if (random.Next(0, currentRooms.Count) <= chance) {
                        string dir = directionPoolCopy[random.Next(0, directionPoolCopy.Count)];
                        while (room.HasExit(dir)) {
                            dir = directionPoolCopy[random.Next(0, directionPoolCopy.Count)];
                        }
                        room.AddExit(dir, winRoom);
                        Console.WriteLine("placed win room");
                        break;
                    }
                    chance++;
                }
            }
        }
        Console.WriteLine(roomAmount);
    }
}
