namespace Zuul;

class Entity {
    // fields
    private int damageModifier;
    private int speedModifier;
    private int health;
    private Room currentRoom;

    // attributes
    public int DamageModifier { get { return damageModifier; } set { damageModifier = value; } }
    public int SpeedModifier { get { return speedModifier; } set { speedModifier = value; } }
    public Room CurrentRoom { get { return currentRoom; } set { this.currentRoom = value; } }
    public int Health { get { return health; } set { health = value; } }

    
    // constructor
    public Entity(int damageModifier, int speedModifier, int health) {
        this.damageModifier = damageModifier;
        this.speedModifier = speedModifier;
        this.health = health;
        this.currentRoom = null;
    }
}