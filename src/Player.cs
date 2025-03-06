namespace Zuul;

class Player : Entity {
    // fields
    private Inventory backPack;
    private string noteBook;
    private int maxHealth;
    
    // properties
    public Inventory BackPack { get { return this.backPack; } set { this.backPack = value; } }
    public string NoteBook { get { return this.noteBook; } set { this.noteBook = value; } }
    public int MaxHealth { get { return this.maxHealth; } set { maxHealth = value; } }
    
    // constructor
    public Player() : base(1, 0, 100, "player") {
        this.backPack = new Inventory(25);
        noteBook = "";
        this.maxHealth = 100;
    }

    // methods
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
    
    public new void Heal (int amount) {
        if ((Health += amount) == maxHealth) return;
        Health += amount;
    }
    
    public bool DropToChest(string itemName) {
        Item item = BackPack.Get(itemName);
        if (item != null) {
            if (CurrentRoom.Chest.Put(itemName, item)) {
                if (item.Equiped) {
                    item.RemoveModifiers(this);
                }
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