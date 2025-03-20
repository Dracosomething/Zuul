namespace Zuul;

class Enemy : Entity {
    // fields
    private Inventory inventory;
    private Item mainWeapon;
    private bool hasSeenPlayer;
    private bool isSub;
    
    // attributes
    public Inventory Inventory { get { return inventory; } }

    // constructor
    public Enemy(int hp, int dmg, int invSize, int armor, string enemyName) : 
        this(hp, dmg, invSize, armor, enemyName, false) {}
    
    public Enemy(int hp, int dmg, int invSize, int armor, string enemyName, bool isSub) : base(dmg, armor, hp, enemyName) {
        inventory = new Inventory(invSize);
        mainWeapon = null;
        hasSeenPlayer = false;
        this.isSub = isSub;
    }
    
    // methods
    /// <summary>
    /// used to give an enemy a new item.
    /// </summary>
    /// <param name="itemName">the name of the item</param>
    /// <param name="item">the item to be added</param>
    /// <returns> true if the item is added and false if it fails</returns>
    public bool GiveItem(string itemName, Item item) {
        if (Inventory.Put(itemName, item)) {
            item.ApplyModifiers(this);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="itemName">the name of the item</param>
    /// <returns> true if the item is removed else false.</returns>
    public bool RemoveItem(string itemName) {
        if (Inventory.Remove(itemName)) {
            Item item = this.inventory.Get(itemName);
            item.RemoveModifiers(this);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// used to add items to the current room
    /// </summary>
    /// <param name="itemName">the name of the item</param>
    /// <returns>true if it gets dropped, else false</returns>
    public bool DropToChest(string itemName) {
        Item item = Inventory.Get(itemName);
        if (CurrentRoom.Chest.Put(itemName, item)) {
            item.RemoveModifiers(this);
            Inventory.Remove(itemName);
            
            Console.WriteLine($"Enemy dropped {itemName}.");
            return true;
        }
        
        Console.WriteLine("Enemy had no items.");
        return false;
    }

    /// <summary>
    /// method that runs every game update, handles all the logic of an enemy.
    /// </summary>
    public new void Tick() {
        base.Tick();
        Random random = new Random();
        // checks if the enemy is alive
        if (!IsAlive()) {
            OnDeath();
        } else {
            // makes shure it is in the room
            this.CurrentRoom.AddInhabitant(this.Name, this);
                // code to attack the player
                CurrentRoom.ForEachInhabitant((inhabitant) => {
                    if (!isSub) {
                        if (inhabitant.Value is Player) {
                            if (this.mainWeapon != null) {
                                if (mainWeapon.IsPoisoned) inhabitant.Value.TicksOnFire = 5;
                                Console.WriteLine($"{this.Name} attacked {inhabitant.Key} using {mainWeapon.Name}."); }
                            else {
                                Console.WriteLine($"{this.Name} attacked {inhabitant.Key}.");
                            }

                            inhabitant.Value.Damage(DamageModifier, false);
                            hasSeenPlayer = true;
                        }
                    } else {
                        if (!inhabitant.Value is Player) {
                            if (this.mainWeapon != null) {
                                if (mainWeapon.IsPoisoned) inhabitant.Value.TicksOnFire = 5;
                                Console.WriteLine($"{this.Name} attacked {inhabitant.Key} using {mainWeapon.Name}."); }
                            else {
                                Console.WriteLine($"{this.Name} attacked {inhabitant.Key}.");
                            }

                            inhabitant.Value.Damage(DamageModifier, false);
                            hasSeenPlayer = true;
                        }
                    }
                });
            int count = this.CurrentRoom.GetExitCount();
            int itterations = 0;
            // allows enemy to move to other rooms
            this.CurrentRoom.ForEachExit((exit) => {
                if (hasSeenPlayer) {
                    if (exit.Value.ContainsInhabitant("player") || 
                        (this.CurrentRoom.ContainsInhabitant("player"))) {
                        
                        // makes it so an enemy cant run from the player
                        if (this.CurrentRoom.ContainsInhabitant("player")) {
                            return;
                        }
                        // 50% chance for the enemy to move or if their a minion a 100% chance
                        int chance = this.isSub ? 100 : 50;
                        if (random.Next(0, 100) <= chance) {
                            this.CurrentRoom.RemoveInhabitant(this.Name);
                            this.CurrentRoom = exit.Value;
                            exit.Value.AddInhabitant(this.Name, this);
                        }
                    } else if (itterations == count) { // makes shure the enemy wont run the code anymore.
                        hasSeenPlayer = false;
                    }

                    itterations++;
                }
            });
        }
    }

    /// <summary>
    /// code that drops an enemies items and removes it from the world
    /// </summary>
    private void OnDeath() {
        this.CurrentRoom.RemoveInhabitant(this.Name);
        Console.WriteLine($"{this.Name} died and dropped {inventory.GetContents()}");
        this.inventory.ForEachItemName((itemName) => {
            this.DropToChest(itemName);
        });
    }

    /// <summary>
    /// sets the main weapon of an enemy
    /// </summary>
    /// <param name="item">The item that should be the main weapon</param>
    public void SetWeapon(Item item) {
        this.inventory.Put(item.Name, item);
        item.ApplyModifiers(this);
        item.Equiped = true;
        this.mainWeapon = item;
    }

    public Enemy Clone() {
        return new Enemy(this.Health, this.DamageModifier, this.inventory.MaxWeight, this.ArmorModifier, this.Name,
            this.isSub);
    }
}