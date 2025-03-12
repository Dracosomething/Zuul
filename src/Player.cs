namespace Zuul;

class Player : MagicEntity {
    // fields
    private string noteBook;
    private bool isHurt;
    
    // properties
    public string NoteBook { get { return this.noteBook; } set { this.noteBook = value; } }
    public bool IsHurt { get { return isHurt; } set { isHurt = value; } }
    
    // constructor
    public Player() : base(1, 0, 100, 50, 3, "player") {
        this.BackPack = new Inventory(25);
        noteBook = "";
        IsHurt = true;
    }

    // methods
    /// <summary>
    /// takes an item from a room
    /// </summary>
    /// <param name="itemName">The name of the item that should be taken</param>
    /// <returns>if the player got the item</returns>
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
    
    /// <summary>
    /// heals some health of the player
    /// </summary>
    /// <param name="amount">The amount of health that should get healed</param>
    public new void Heal (int amount) {
        if ((Health += amount) == MaxHealth) return;
        base.Heal(amount);
    }
    
    /// <summary>
    /// drops an item from the players inventory into the room.
    /// </summary>
    /// <param name="itemName">The name of the item</param>
    /// <returns>If the item is successfully dropped</returns>
    public bool DropToChest(string itemName) {
        Item item = BackPack.Get(itemName);
        if (item != null) {
            if (CurrentRoom.Chest.Put(itemName, item)) {
                if (item.Equiped) {
                    item.RemoveModifiers(this);
                }
                BackPack.Remove(itemName);
                Console.WriteLine("successfully added item to room.");
                return true;
            }
        }
        Console.WriteLine("couldn't find item in backpack.");
        return false;
    }

    /// <summary>
    /// uses an item 
    /// </summary>
    /// <param name="itemName">the name of the item that should be used</param>
    /// <returns>The success message</returns>
    public string Use(string itemName) {
        if (itemName.Equals("med-kit")) {
            this.Heal(20);
            Console.WriteLine("used med kit and healed 20 hp");
            this.BackPack.Remove(itemName);
        }
        if (itemName.Equals("stiches")) {
            IsHurt = false;
            Console.WriteLine("used stiches and closed your wounds");
            this.BackPack.Remove(itemName);
        }
        if (itemName.Equals("food-plate")) {
            this.Heal(10);
            Console.WriteLine("You ate the food and feel rejuvenated");
            this.BackPack.Remove(itemName);
        }
        return $"Successfully used {itemName}.";
    }

    /// <summary>
    /// Notes down a rooms info
    /// </summary>
    /// <param name="room">The room that should get its info noted down</param>
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

    /// <summary>
    /// used to get the notebook string
    /// </summary>
    /// <returns>The notebook string</returns>
    public string Read() {
        return this.noteBook;
    }
    
    /// <summary>
    /// allows the player to learn a new spell
    /// </summary>
    /// <param name="spellName">The name of the spell that should be learned</param>
    /// <returns>if the spells is learned</returns>
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
    
    /// <summary>
    /// Used to get a list of all spells as a string
    /// </summary>
    /// <returns>All spells the player has as a string</returns>
    public string ShowSpells() {
        string spellString = "";
        int loopTimes = 0;
        foreach (var keyValuePair in SpellBook) {
            loopTimes++;
            spellString += keyValuePair.Key;
            spellString += $"[description: \"{keyValuePair.Value.Description}\", cost: {keyValuePair.Value.ManaCost}, is single use: {keyValuePair.Value.IsSingleUse}]";
            if (SpellBook.Count > loopTimes) {
                spellString += ", \n";
            }
        }
        
        return spellString;
    }
}