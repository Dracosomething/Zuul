namespace Zuul;

class Player {
    // fields
    private Room currentRoom;
    private int health;
    private Inventory backPack;
    
    // properties
    public Room CurrentRoom { get; set; }
    public int Health { get { return this.health; } set { this.health = value; } }

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
}