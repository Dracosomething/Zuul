namespace Zuul;

class Player {
    // fields
    private Room currentRoom;
    private int health;
    private Inventory backPack;
    
    // properties
    public Room CurrentRoom { get { return this.currentRoom; } set { this.currentRoom = value; } }
    public int Health { get { return this.health; } set { this.health = value; } }
    public Inventory BackPack { get { return this.backPack; } }
    
    // constructor
    public Player() {
        this.currentRoom = null;
        this.health = 100;
        this.backPack = new Inventory(25);
    }

    // methods
    /// <summary>
    /// deals damage to the player
    /// </summary>
    /// <param name="amount">the amount of damage</param>
    public void damage (int amount) {
        health -= amount;
    }

    /// <summary>
    /// heals the player
    /// </summary>
    /// <param name="amount">the amount of health that should be added</param>
    public void heal (int amount) {
        health += amount;
    }

    /// <summary>
    /// checks if the players health is above 0
    /// </summary>
    /// <returns>true if health is above 0, otherwise returns false.</returns>
    public bool isAlive() {
        return health > 0;
    }

    public bool TakeFromChest(string itemName) {
        Item item = CurrentRoom.Chest.Get(itemName);
        if (BackPack.Put(itemName, item)) {
            CurrentRoom.Chest.Remove(itemName);
            Console.WriteLine("successfully added item to backpack.");
            return true;
        }
        Console.WriteLine("Item couldn't be found in current room.");
        return false;
    }
    
    public bool DropToChest(string itemName) {
        Item item = BackPack.Get(itemName);
        if (currentRoom.Chest.Put(itemName, item)) {
            backPack.Remove(itemName);
            Console.WriteLine("successfully added item to room.");
            return true;
        }
        Console.WriteLine("couldn't find item in backpack.");
        return false;
    }
}