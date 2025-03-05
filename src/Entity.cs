using System.Text.Json.Serialization;

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
    // [JsonIgnore]
    public Room CurrentRoom { get { return currentRoom; } set { this.currentRoom = value; } }
    public int Health { get { return health; } set { health = value; } }

    
    // constructor
    public Entity(int damageModifier, int armorModifier, int health) {
        this.damageModifier = damageModifier;
        this.armorModifier = armorModifier;
        this.health = health;
        this.currentRoom = null;
    }
    
    public Entity() {
        this.damageModifier = 0;
        this.armorModifier = 0;
        this.health = 0;
        this.currentRoom = null;
    }
    
    // methods
    public void Equip(Item item) {
        item.ApplyModifiers(this);
        item.Equiped = true;
    }
    
    /// <summary>
    /// deals damage to the player
    /// </summary>
    /// <param name="amount">the amount of damage</param>
    public void Damage (int amount) {
        amount -= (int) Math.Ceiling(amount * ((ArmorModifier+1) * 0.01));
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

    public void Tick() {
    }
}