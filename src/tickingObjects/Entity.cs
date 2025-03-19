using System.Text.Json.Serialization;

namespace Zuul;

class Entity {
    // fields
    private int damageModifier;
    private int armorModifier;
    private int health;
    private Room currentRoom;
    private string name;
    private int ticksOnFire;

    // attributes
    public int DamageModifier { get { return damageModifier; } set { damageModifier = value; } }
    public int ArmorModifier { get { return armorModifier; } set { armorModifier = value; } }
    [JsonIgnore]
    public Room CurrentRoom { get { return currentRoom; } set { this.currentRoom = value; } }
    public int Health { get { return health; } set { health = value; } }
    public string Name { get { return name; } set { this.name = value; } }
    public int TicksOnFire { set { ticksOnFire = value; } }

    
    // constructor
    public Entity(int damageModifier, int armorModifier, int health, string name) {
        this.damageModifier = damageModifier;
        this.armorModifier = armorModifier;
        this.health = health;
        this.currentRoom = null;
        this.name = name;
        ticksOnFire = 0;
    }
    
    // methods
    public void Equip(Item item) {
        item.ApplyModifiers(this);
        item.Equiped = true;
    }
    
    /// <summary>
    /// deals damage to the entity
    /// </summary>
    /// <param name="amount">the amount of damage</param>
    /// <param name="bypassDefense">if the damage should bypass the entities defence</param>
    public void Damage (int amount, bool bypassDefense) {
        if (!bypassDefense) {
            // makes shure that the entitie wont take negative damage
            if (amount * ((ArmorModifier + 1) * 0.01) < 0 && (ArmorModifier != 0)) amount = 0;
            else {
                // makes shure that you always take at least 1 damage
                if ((int)Math.Ceiling(amount * ((ArmorModifier + 1) * 0.01)) == 1 && amount == 1) amount = 1;
                else amount -= (int)Math.Ceiling(amount * ((ArmorModifier + 1) * 0.01));
            }
        }
        Health -= amount;
    }

    /// <summary>
    /// heals the entity
    /// </summary>
    /// <param name="amount">the amount of health that should be added</param>
    public void Heal (int amount) {
        Health += amount;
    }

    /// <summary>
    /// checks if the entities health is above 0
    /// </summary>
    /// <returns>true if health is above 0, otherwise returns false.</returns>
    public bool IsAlive() {
        return Health > 0;
    }

    /// <summary>
    /// runs every game update, deals damage to an enemy if it is on fire.
    /// </summary>
    public void Tick() {
        if (ticksOnFire > 0) {
            this.Damage(2, false);
            this.ticksOnFire--;
        }
    }
}