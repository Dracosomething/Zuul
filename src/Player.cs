namespace Zuul;

class Player : Entity {
    // fields
    private Inventory backPack;
    private string noteBook;
    
    // properties
    public Inventory BackPack { get { return this.backPack; } set { this.backPack = value; } }
    public string NoteBook { get { return this.noteBook; } set { this.noteBook = value; } }
    
    // constructor
    public Player() : base(1, 0, 100) {
        this.backPack = new Inventory(25);
        noteBook = "";
    }

    // methods
    /// <summary>
    /// deals damage to the player
    /// </summary>
    /// <param name="amount">the amount of damage</param>
    public void Damage (int amount) {
        amount = (int) Math.Floor(amount * (ArmorModifier * 0.01));
        Health -= amount;
    }

    /// <summary>
    /// heals the player
    /// </summary>
    /// <param name="amount">the amount of health that should be added</param>
    public void Heal (int amount) {
        Health += amount;
    }

    /// <summary>
    /// checks if the players health is above 0
    /// </summary>
    /// <returns>true if health is above 0, otherwise returns false.</returns>
    public bool IsAlive() {
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
            this.Heal(20);
        }
        return $"Successfully used {itemName}.";
    }

    public void NoteDown(Room room) {
        this.noteBook += room.Name;
        this.noteBook += ":\n\t";
        room.ForEachExit((keyValuePair) => {
            this.noteBook += keyValuePair.Key;
            this.noteBook += " -> ";
            this.noteBook += room.GetExit(keyValuePair.Key).Name;
            this.noteBook += ".\n\t";
        });
        this.noteBook += "\n";
    }

    public string Read() {
        return this.noteBook;
    }
}