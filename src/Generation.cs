namespace Zuul;

class Generation
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
    private Room trapRoomEmpty = new Room("an empty room", "empty-room-trap");
    private Room trapRoomChest = new Room("a room with a chest", "chest-room-trap");
    private Room celler = new Room("a room filled with beer fats", "cellar");
    private Room roomWithSpellBook = new Room("a room with a magical book in the center", "room-spellbook");
    
    // constructor
    public Generation()
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
        roomPool.Add(roomWithSpellBook);
        
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
    public void GenerateWorld(Room startRoom, Room winRoom, int iterations) {
        Random random = new Random(); // creates the randomizer.
        int chance; // an empty variable that gets assigned the chance for the randomizer to succeed.
        int roomAmount = 0; // the variable that stores all rooms, used for debugging.
        
        // the for loop for the generation
        for (int i = 0; i <= iterations; i++) {
            List<string> directionPoolCopy = directionPool;
            
            if (i == 0) {
                Room nextRoom = roomPool[random.Next(0, roomPool.Count)].Clone();
                
                startRoom.AddExit("east", nextRoom);
                nextRoom.AddExit("west", startRoom);
                
                nextRoom.Chest.Put("stiches", new Item(1, "medical supplies", "stiches"));
                nextRoom.Chest.Put("bigger-backpack", new Item(0, "A backpack that can hold more then your current one.", "bigger-backpack"));
                
                directionPoolCopy.Remove("west");
                currentRooms.Add(nextRoom);
                continue;
            }

            List<Room> toBeAdded= new List<Room>();
            
            foreach (var room in currentRooms) {
                chance = 100;
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
                                Item rustySword = new Item(5, 5, "A old and rusty sword", "damage", "rusty-sword");
                                Item sword = new Item(random.Next(5, 7), random.Next(5, 14), "A sharp and long sword",
                                    "damage", "sword");
                                Item rustyShield = new Item(2, 1, "A rusty old shield, used for defending yourself.",
                                    "armor", "shield");
                                Item shield = new Item(random.Next(2, 3), random.Next(1, 6),
                                    "A normal shield, used for defending yourself.", "armor", "shield");
                                Item oldArmor = new Item(10, 2, 1, "A suit of rusty old armor.", "old-armor");
                                Item armor = new Item(random.Next(10, 20), random.Next(2, 7), random.Next(1, 6),
                                    "A suit of heavy armor.", "armor");
                                
                                List<Item> items = [rustyShield, shield, oldArmor, armor, rustySword, sword];
                                
                                Item gear = items[random.Next(0, items.Count)];
                                
                                List<string> names = ["ermanno", "leofwine", "gerald", "sam", "timothee", "guard", "jimmy"];

                                string name = names[random.Next(0, names.Count)];

                                Enemy enemy = new Enemy(random.Next(1, 101), random.Next(1, 21), 9999, random.Next(1, 11), name);
                                enemy.SetWeapon(gear);
                                nextRoom.AddInhabitant(name, enemy);
                                enemy.CurrentRoom = nextRoom;
                            } else if (roomPoolChosen.Name.Equals(roomChest.Name)) {
                                Item rustySword = new Item(5, 5, "A old and rusty sword", "damage", "rusty-sword");
                                Item sword = new Item(random.Next(5, 7), random.Next(5, 14), "A sharp and long sword",
                                    "damage", "sword");
                                Item rustyShield = new Item(2, 1, "A rusty old shield, used for defending yourself.",
                                    "armor", "shield");
                                Item shield = new Item(random.Next(2, 3), random.Next(1, 6),
                                    "A normal shield, used for defending yourself.", "armor", "shield");
                                Item oldArmor = new Item(10, 2, 1, "A suit of rusty old armor.", "old-armor");
                                Item armor = new Item(random.Next(10, 20), random.Next(2, 7), random.Next(1, 6),
                                    "A suit of heavy armor.", "armor");
                                Item backpack = new Item(0, "A backpack that can hold more then your current one.",
                                    "bigger-backpack");
                                Item medKit = new Item(10, "A box filled with medical supplies", "med-kit");
                                Item cloackOfHealth = new Item(1, 25, "A magical cloack.", "health",
                                    "cloack-of-health");
                                Item excalibur = new Item(7, 14, "A old and rusty sword", "damage", "excalibur");
                                Item kavacha = new Item(20, 7, 6,
                                    "A legendary suit of armor, some say it holds incredible magical powers",
                                    "kavacha");
                                Item pridwen = new Item(3, 6,
                                    "A legendary shield that comes from the Arthurian legend.", "armor", "pridwen");
                                excalibur.ManaModifier = 10;
                                excalibur.MagicPowerModifier = 2;
                                kavacha.ManaModifier = 4;
                                kavacha.MagicPowerModifier = 1;
                                pridwen.ManaModifier = 5;
                                pridwen.MagicPowerModifier = 3;

                                List<Item> items = [rustyShield, shield, pridwen, oldArmor, armor, kavacha, rustySword, sword, excalibur, backpack, medKit, cloackOfHealth];

                                Item contents = items[random.Next(0, items.Count)];

                                nextRoom.Chest.Put(contents.Name, contents);
                            } else if (roomPoolChosen.Name.Equals(celler.Name)) {
                                Item food = new Item(0, "a delicious meal.", "food-plate");

                                nextRoom.Chest.Put(food.Name, food);
                            } else if (roomPoolChosen.Name.Equals(trapRoomChest.Name)) {
                                Trap mimic = new Trap(0, "Mimic", "The chest reveals itself to be a mimic.", false);
                                mimic.Function = (() => MimicSpawn(mimic));
                                
                                nextRoom.AddInhabitant(mimic.Name, mimic);
                                mimic.CurrentRoom = nextRoom;
                            } else if (roomPoolChosen.Name.Equals(trapRoomEmpty.Name)) {
                                Trap arrowWall = new Trap(7, "Arrow-Wall", "The walls open up and fire arrows at you.", false);
                                arrowWall.Function = () => ShootArrows(arrowWall);
                                
                                nextRoom.AddInhabitant(arrowWall.Name, arrowWall);
                                arrowWall.CurrentRoom = nextRoom;
                            } else if (roomPoolChosen.Name.Equals(roomWithSpellBook.Name)) {
                                Spell fireball = new Spell("fireball", "This spell creates a large ball of fire that engulfs an entire room.", 20, false);
                                fireball.Effect = () => Fireball(fireball);
                                Spell lesserHeal = new Spell("lesser-heal", "Heals 5 hp of the caster", 3, false);
                                lesserHeal.Effect = () => Heal(lesserHeal, 5);
                                Spell greaterHeal = new Spell("greater-heal", "Heals 20 hp of the caster", 15, false);
                                greaterHeal.Effect = () => Heal(greaterHeal, 20);
                                Spell smite = new Spell("smite",
                                    "Creates a beam of light from the users weapon that strengthens the weapon and deals some damage to one enemy in the room when casted",
                                    30, true);
                                smite.Effect = () => Smite(smite);
                                Spell magicMissile = new Spell("magic-missile",
                                    "fires a tiny missile that deals small damage to one enemy in your room", 5, false);
                                magicMissile.Effect = () => MagicMissile(magicMissile);
                                Spell conjureShield = new Spell("conjure-shield",
                                    "creates a magical shield from the casters mana", 12, false);
                                conjureShield.Effect = () => ConjureShield(conjureShield);
                                Spell conjureSword = new Spell("conjure-sword",
                                    "creates a magical sword from the casters mana", 12, false);
                                conjureSword.Effect = () => ConjureSword(conjureSword);
                                
                                List<Spell> spellPool = [fireball, lesserHeal, greaterHeal, smite, magicMissile, conjureShield, conjureSword];
                                
                                Spell spell = spellPool[random.Next(0, spellPool.Count)];
                                
                                nextRoom.AddSpell(spell);
                            }
                            nextRoom.AddExit(
                                dir.Equals("west") ? "east" :
                                dir.Equals("east") ? "west" :
                                dir.Equals("north") ? "south" : "north", room);
                            room.AddExit(dir, nextRoom);

                            toBeAdded.Add(nextRoom);
                        }
                    }
                    chance -= 20; 
                }
            }
            currentRooms.Clear();
            Console.WriteLine(i);
            foreach (var roomAdded in toBeAdded) {
                currentRooms.Add(roomAdded);
            }
            roomAmount += currentRooms.Count;
            
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
        
        Console.WriteLine(roomAmount);
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
        if (!trap.CurrentRoom.ContainsInhabitant("player")) {
            Trap newMimic = new Trap(0, "Mimic", "A chest that when opened becomes a monster.", false);
            newMimic.Function = (() => MimicSpawn(newMimic));
                                
            trap.CurrentRoom.AddInhabitant(newMimic.Name, newMimic);
            newMimic.CurrentRoom = trap.CurrentRoom;
        } else {
            Room trappedRoom = trap.CurrentRoom;
            trap.Discard();
            Enemy mimic = new Enemy(25, 5, 700, 3, "Mimic");
            mimic.CurrentRoom = trappedRoom;
            mimic.Tick();
        }
    }
    
    private void ShootArrows(Trap trap) {
        trap.CurrentRoom.ForEachInhabitant((Inhabitant) => {
            Entity inhabitant = Inhabitant.Value;
            if (!(inhabitant is Trap)) {
                inhabitant.Damage(trap.DamageModifier, false);
            }
        });
    }

    public void Fireball(Spell spell) {
        spell.Caster.CurrentRoom.ForEachInhabitant((inhabitant) => {
            if (!inhabitant.Value.Equals(spell.Caster)) {
                inhabitant.Value.Damage(spell.Caster.MagicPower*2);
                inhabitant.Value.TicksOnFire = (int)Math.Floor((double) spell.Caster.MagicPower / 3);
            }
        });
        Console.WriteLine($"{spell.Caster.Name} casted fireball. The room gets engulfed in a sea of fire.");
    }

    public void Heal(Spell spell, int amount) {
        spell.Caster.Heal(amount);
        Console.WriteLine($"Healed {spell.Caster.Name}");
    }

    public void Smite(Spell spell) {
        Random random = new Random();
        Entity entity = spell.Caster.CurrentRoom.GetInhabitants()[random.Next(0, spell.Caster.CurrentRoom.GetInhabitants().Count)];
        while (entity.Equals(spell.Caster) || entity is Trap) {
            entity = spell.Caster.CurrentRoom.GetInhabitants()[
                random.Next(0, spell.Caster.CurrentRoom.GetInhabitants().Count)];
        }
        entity.Damage((spell.Caster.MagicPower), true);
        spell.Caster.BackPack.ForEachItem((item) => {
            if (item.Name.Contains("sword") || item.Name.Contains("excalibur")) {
                item.DamageModifier += (int)Math.Floor((double) spell.Caster.MagicPower / 2);
            }
        });
        Console.WriteLine($"{spell.Caster.Name} casted smite, their swords now glow and one enemy looks severally weakened.");
    }

    public void ConjureSword(Spell spell) {
        MagicEntity caster = spell.Caster;
        if (!caster.BackPack.Items.ContainsKey("conjured-sword")) {
            Item magicSword = new Item(0, 5, "A magical sword that slowly decays.", "damage", "conjured-sword");
            magicSword.DecayTicks = 5;
            Console.WriteLine($"{caster.Name} conjured a sword out of thin air.");
        } else {
            Console.WriteLine($"{caster.Name} already has conjured a sword.");
        }
    }

    public void ConjureShield(Spell spell) {
        MagicEntity caster = spell.Caster;
        if (!caster.BackPack.Items.ContainsKey("conjured-shield")) {
            Item magicShield = new Item(0, 5, "A magical shield that slowly decays.", "armor", "conjured-shield");
            magicShield.DecayTicks = 5;
            Console.WriteLine($"{caster.Name} conjured a shield out of thin air.");
        } else {
            Console.WriteLine($"{caster.Name} already has conjured a shield.");
        }
    }

    public void MagicMissile(Spell spell) {
        Random random = new Random();
        Entity entity = spell.Caster.CurrentRoom.GetInhabitants()[random.Next(0, spell.Caster.CurrentRoom.GetInhabitants().Count)];
        while (entity.Equals(spell.Caster) || entity is Trap) {
            entity = spell.Caster.CurrentRoom.GetInhabitants()[
                random.Next(0, spell.Caster.CurrentRoom.GetInhabitants().Count)];
        }
        entity.Damage(5, false);
        Console.WriteLine($"{spell.Caster.Name} used magic missile on {entity.Name}");
    }
}
