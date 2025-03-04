namespace Zuul;

class Entity {
    // fields
    private int damageModifier;
    private int armorModifier;
    private int health;
    private Room currentRoom;

    // attributes
    public int DamageModifier { get { return damageModifier; } set { damageModifier = value; } }
    public int ArmorModifier { get { return armorModifier; } set { armorModifier = value; } }
    public Room CurrentRoom { get { return currentRoom; } set { this.currentRoom = value; } }
    public int Health { get { return health; } set { health = value; } }

    
    // constructor
    public Entity(int damageModifier, int armorModifier, int health) {
        this.damageModifier = damageModifier;
        this.armorModifier = armorModifier;
        this.health = health;
        this.currentRoom = null;
    }
    
    // methods
    public void Equip(Item item) {
        item.ApplyModifiers(this);
        item.Equiped = true;
    }
}