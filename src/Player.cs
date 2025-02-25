namespace Zuul;

class Player {
    private Room currentRoom;
    private int health;
    
    public Room CurrentRoom { get; set; }
    public int Health { get { return this.health; } set { this.health = value; } }

    public Player() {
        currentRoom = null;
        health = 100;
    }

    public void damage (int amount) {
        health -= amount;
    }

    public void heal (int amount) {
        health += amount;
    }

    public bool isAlive() {
        return health > 0;
    }
}