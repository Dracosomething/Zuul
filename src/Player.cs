namespace Zuul;

class Player : MagicEntity {
    // fields
    private Inventory backPack;
    private string noteBook;
    private int maxHealth;
    
    // properties
    public Inventory BackPack { get { return this.backPack; } set { this.backPack = value; } }
    public string NoteBook { get { return this.noteBook; } set { this.noteBook = value; } }
    public int MaxHealth { get { return this.maxHealth; } set { maxHealth = value; } }
    
    // constructor
    public Player() : base(1, 0, 100, 50, 3, "player") {
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
        base.Heal(amount);
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
    
    public bool LearnSpell(string spellName) {
        Spell spell = CurrentRoom.GetSpell(spellName);
        if (spell != null) {
            if (SpellBook.TryAdd(spell.Name, spell)) {
                Console.WriteLine("successfully learned a new spell.");
                return true;
            }
        }
        Console.WriteLine("That spell is not in this rooms spell book.");
        return false;
    }
    
    public string ShowSpells() {
        string itemString = "";
        int loopTimes = 0;
        foreach (var keyValuePair in SpellBook) {
            loopTimes++;
            itemString += keyValuePair.Key;
            itemString += $"[description: \"{keyValuePair.Value.Description}\", cost: {keyValuePair.Value.ManaCost}, is single use: {keyValuePair.Value.IsSingleUse}]";
            if (SpellBook.Count > loopTimes) {
                itemString += ", \n";
            }
        }
        
        return itemString;
    }
}