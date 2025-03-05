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
    /// generates a new dungeon when called, it does this by using a for-loop that loops for <c>iterations</c> times.
    /// </summary>
    /// <param name="startRoom">The room where the generations should start at.</param>
    /// <param name="winRoom">The room that makes the player win.</param>
    /// <param name="iterations">The amount of times it should loop.</param>
    public List<Room> GenerateWorld(Room startRoom, Room winRoom, int iterations) {
        Random random = new Random(); // creates the randomizer.
        int chance; // an empty variable that gets assigned the chance for the randomizer to succeed.
        List<Room> roomAmount = new List<Room>(); // the variable that stores all rooms, used for debugging.
        
        // the for loop for the generation
        for (int i = 0; i <= iterations; i++) {
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
                    if (!room.HasExit("down")) { 
                        Room nextStairs = random.Next(0, 2) == 1 ? stairwell.Clone() : stairwellBottom.Clone(); 
                        
                        room.AddExit("down", nextStairs); 
                        nextStairs.AddExit("up", room); 
                        toBeAdded.Add(nextStairs); 
                    }
                } else if (room.Name.Equals("staircase-middle")) { 
                    if (!room.HasExit("up")) { 
                        Room nextStairsUp = random.Next(0, 2) == 1 ? stairwell.Clone() : stairwellTip.Clone(); 
                        
                        room.AddExit("up", nextStairsUp); 
                        nextStairsUp.AddExit("down", nextStairsUp); 
                        toBeAdded.Add(nextStairsUp); 
                    }
                    
                    if (!room.HasExit("down")) {
                        Room nextStairsDown = random.Next(0, 2) == 1 ? stairwell.Clone() : stairwellBottom.Clone();
                        
                        room.AddExit("down", nextStairsDown);
                        nextStairsDown.AddExit("up", room);
                        toBeAdded.Add(nextStairsDown);
                    }
                    continue;
                } else if (room.Name.Equals("staircase-bottom")) {
                    if (!room.HasExit("up")) {
                        Room nextStairs = random.Next(0, 2) == 1 ? stairwell.Clone() : stairwellTip.Clone();
                        
                        room.AddExit("up", nextStairs);
                        nextStairs.AddExit("down", room);
                        toBeAdded.Add(nextStairs);
                    }
                }
                
                foreach (var dir in directionPoolCopy) {
                    if (random.Next(0, 100) <= chance) {
                        if (!room.HasExit(dir)) {
                            Room roomPoolChosen = roomPool[random.Next(0, roomPool.Count)];
                            Room nextRoom = roomPoolChosen.Clone();
                            if (roomPoolChosen.Name.Equals(roomWithEnemy.Name)) {
                                List<string> names = ["Ermanno", "Leofwine", "Gerald", "Sam", "Timothée", "Guard", "Jimmy"];

                                string name = names[random.Next(0, names.Count)];

                                Enemy enemy = new Enemy(random.Next(1, 101), random.Next(1, 21), 9999, random.Next(1, 11), name);
                                nextRoom.Inhabitants.Add(name, enemy);
                                enemy.CurrentRoom = nextRoom;
                            } else if (roomPoolChosen.Name.Equals(roomChest.Name)) {
                                Item rustySword = new Item(5, 7, "A old and rusty sword", "damage", "rusty-sword");
                                Item shield = new Item(3, 3, "A normal shield, used for defending yourself.", "armor", "shield");
                                Item cloackOfHealth = new Item(1, 25, "A magical cloack.", "health", "cloack-of-health");
                                Item excalibur = new Item(5, 14, "A old and rusty sword", "damage", "excalibur");
                                Item rustyShield = new Item(2, 1, "A rusty old shield, used for defending yourself.", "armor", "shield");
                                Item armor = new Item(20, 5, 5, "A suit of heavy armor.", "armor");
                                Item oldArmor = new Item(10, 2, 1, "A suit of rusty old armor.", "old-armor");
                                Item medKit = new Item(10, "A box filled with medical supplies", "med-kit");

                                List<Item> items = [rustyShield, shield, rustySword, excalibur, cloackOfHealth, medKit, armor, oldArmor];

                                Item contents = items[random.Next(0, items.Count)];

                                nextRoom.Chest.Put(contents.Name, contents);
                            } else if (roomPoolChosen.Name.Equals(celler.Name)) {
                                Item food = new Item(0, "a delicious meal.", "food-plate");

                                nextRoom.Chest.Put(food.Name, food);
                            } else if (roomPoolChosen.Name.Equals(trapRoomChest.Name)) {
                                Trap mimic = new Trap(0, "Mimic", "A chest that when opened becomes a monster.", false);
                                mimic.Function = (() => MimicSpawn(mimic));
                                
                                nextRoom.Inhabitants.Add(mimic.Name, mimic);
                                mimic.CurrentRoom = nextRoom;
                            } else if (roomPoolChosen.Name.Equals(trapRoomEmpty.Name)) {
                                Trap arrowWall = new Trap(7, "Arrow-Wall", "A wall that fires arrows.", false);
                                arrowWall.Function = () => ShootArrows(arrowWall);
                                
                                nextRoom.Inhabitants.Add(arrowWall.Name, arrowWall);
                                arrowWall.CurrentRoom = nextRoom;
                            }
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
            roomAmount.AddRange(currentRooms);
            
            // for placing the rooms that have to be put in the dungeon
            if (i >= iterations) {
                chance = 1;
                Room goldKeyRoom = new Room("A room with a chest in the middle", "gold-key-room");
                Item goldKey = new Item(5, "A very shiny key.", "gold-key");
                goldKeyRoom.Chest.Put(goldKey.Name, goldKey);
                winRoom.ConditionalItem = goldKey;
                
                // call methods here to place the rooms
                PlaceRequiredRooms(chance, directionPoolCopy, currentRooms, winRoom);
                PlaceRequiredRooms(chance, directionPoolCopy, currentRooms, goldKeyRoom);
            }
        }
        
        Console.WriteLine(roomAmount.Count);
        return roomAmount;
    }

    private void PlaceRequiredRooms(int chance, List<string> directionPoolCopy, List<Room> rooms, Room required) {
        Random random = new Random();
        foreach (var room in rooms) {
            if (random.Next(0, rooms.Count) <= chance) {
                string dir = directionPoolCopy[random.Next(0, directionPoolCopy.Count)];
                        
                while (room.HasExit(dir)) {
                    dir = directionPoolCopy[random.Next(0, directionPoolCopy.Count)];
                }
                        
                room.AddExit(dir, required);
                break;
            }

            chance++;
        }
    }

    private void MimicSpawn(Trap trap) {
        if (!trap.CurrentRoom.Inhabitants.ContainsKey("player")) {
            Trap newMimic = new Trap(0, "Mimic", "A chest that when opened becomes a monster.", false);
            newMimic.Function = (() => MimicSpawn(newMimic));
                                
            trap.CurrentRoom.Inhabitants.Add(newMimic.Name, newMimic);
            newMimic.CurrentRoom = trap.CurrentRoom;
        } else {
            Enemy mimic = new Enemy(25, 5, 700, 3, "Mimic");
            trap.CurrentRoom.Inhabitants.Add(mimic.Name, mimic);
            trap.Discard();
            Console.WriteLine("The chest became a mimic.");
        }
    }

    private void ShootArrows(Trap trap) {
        foreach (var Inhabitant in trap.CurrentRoom.Inhabitants) {
            Entity inhabitant = Inhabitant.Value;
            if (!(inhabitant is Trap)) {
                inhabitant.Damage(trap.DamageModifier);
            }
        }
    }
}
