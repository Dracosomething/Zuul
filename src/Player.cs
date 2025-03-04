namespace Zuul;

class Player : Entity {
    // fields
    private Inventory backPack;
    private string noteBook;
    
    // properties
    public Inventory BackPack { get { return this.backPack; } }
    
    // constructor
    public Player() : base(1, 1, 100) {
        this.backPack = new Inventory(25);
        noteBook = "";
    }

    // methods
    /// <summary>
    /// deals damage to the player
    /// </summary>
    /// <param name="amount">the amount of damage</param>
    public void damage (int amount) {
        Health -= amount;
    }

    /// <summary>
    /// heals the player
    /// </summary>
    /// <param name="amount">the amount of health that should be added</param>
    public void heal (int amount) {
        Health += amount;
    }

    /// <summary>
    /// checks if the players health is above 0
    /// </summary>
    /// <returns>true if health is above 0, otherwise returns false.</returns>
    public bool isAlive() {
        return Health > 0;
    }

    public bool TakeFromChest(string itemName) {
        Item item = CurrentRoom.Chest.Get(itemName);
        if (item != null) {
            if (BackPack.Put(itemName, item)) {
                CurrentRoom.Chest.Remove(itemName);
                Console.WriteLine("successfully added item to backpack.");
                return true;
            }
        }
        Console.WriteLine("Item couldn't be found in current room.");
        return false;
    }
    
    public bool DropToChest(string itemName) {
        Item item = BackPack.Get(itemName);
        if (item != null) {
            if (CurrentRoom.Chest.Put(itemName, item)) {
                item.RemoveModifiers(this);
                backPack.Remove(itemName);
                Console.WriteLine("successfully added item to room.");
                return true;
            }
        }
        Console.WriteLine("couldn't find item in backpack.");
        return false;
    }

    public string Use(string itemName) {
        if (itemName.Equals("medKit")) {
            this.heal(20);
        }
        return $"Successfully used {itemName}.";
    }

    public void NoteDown(Room room) {
        room.ForEachExit((keyValuePair) => {
            this.noteBook += keyValuePair.Key;
            this.noteBook += "\n"; 
        });
    }

    public string Read() {
        return this.noteBook;
    }
}