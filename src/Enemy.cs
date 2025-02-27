namespace Zuul;

class Enemy : Entity {
    // fields
    private Room room;
    private int health;
    private int damageModifier;
    private int speedModifier;
    private string name;
    private Inventory inventory;
    private Item mainWeapon;
    
    // attributes
    public int Health { get { return health; } set { health = value; } }
    public int DamageModifier { get { return damageModifier; } set { damageModifier = value; } }
    public int SpeedModifier { get { return speedModifier; } set { speedModifier = value; } }
    public string Name { get { return name; } }
    public Room Room { get { return this.room; } set { this.room = value; } }
    public Inventory Inventory { get { return inventory; } }

    // constructor
    public Enemy(int hp, int dmg, int invSize, int speed, string enemyName) : base(dmg, speed) {
        this.room = null;
        health = hp;
        name = enemyName;
        damageModifier = dmg;
        speedModifier = speed;
        inventory = new Inventory(invSize);
        mainWeapon = null;
    }
    
    // methods
    /// <summary>
    /// deals damage to the player
    /// </summary>
    /// <param name="amount">the amount of damage</param>
    public void Damage (int amount) {
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
    
    public bool GiveItem(string itemName, Item item) {
        if (Inventory.Put(itemName, item)) {
            item.ApplyModifiers(this);
            return true;
        }
        return false;
    }
    
    public bool RemoveItem(string itemName, Item item) {
        if (Inventory.Remove(itemName)) {
            item.RemoveModifiers(this);
            return true;
        }
        return false;
    }
    
    public bool DropToChest(string itemName) {
        Item item = Inventory.Get(itemName);
        if (Room.Chest.Put(itemName, item)) {
            item.RemoveModifiers(this);
            Inventory.Remove(itemName);
            Console.WriteLine($"Enemy dropped {itemName}.");
            return true;
        }
        Console.WriteLine("Enemy had no items.");
        return false;
    }

    public void Tick() {
        if (!IsAlive()) {
            OnDeath();
        }
        this.Room.AddInhabitant(this.Name, this);
        foreach (var keyValuePair in this.room.Inhabitants) {
            if (keyValuePair.Value is Player player) {
                Console.WriteLine($"{this.name} attacked player using {nameof(mainWeapon)}.");
                player.damage(damageModifier);
            }
        }
    }

    private void OnDeath() {
        this.Room.Inhabitants.Remove(this.Name);
        Console.WriteLine($"{this.Name} died and dropped {inventory.GetContents()}");
        this.inventory.ForEachItemName((itemName) => {
            this.DropToChest(itemName);
        });
    }

    public void SetWeapon(Item item) {
        this.inventory.Put(nameof(item), item);
        item.ApplyModifiers(this);
        this.mainWeapon = item;
    }
}