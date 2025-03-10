namespace Zuul;

class Enemy : Entity {
    // fields
    private Inventory inventory;
    private Item mainWeapon;
    private bool hasSeenPlayer;
    
    // attributes
    public Inventory Inventory { get { return inventory; } }

    // constructor
    public Enemy(int hp, int dmg, int invSize, int armor, string enemyName) : base(dmg, armor, hp, enemyName) {
        inventory = new Inventory(invSize);
        mainWeapon = null;
        hasSeenPlayer = false;
    }
    
    // methods
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
        if (CurrentRoom.Chest.Put(itemName, item)) {
            item.RemoveModifiers(this);
            Inventory.Remove(itemName);
            Console.WriteLine($"Enemy dropped {itemName}.");
            return true;
        }
        Console.WriteLine("Enemy had no items.");
        return false;
    }

    public new void Tick() {
        base.Tick();
        Random random = new Random();
        if (!IsAlive()) {
            OnDeath();
        } else {
            this.CurrentRoom.AddInhabitant(this.Name, this);
            CurrentRoom.ForEachInhabitant((inhabitant) => {
                if (inhabitant.Value is Player player) {
                    if (this.mainWeapon != null) {
                        Console.WriteLine($"{this.Name} attacked player using {mainWeapon.Name}.");
                    } else {
                        Console.WriteLine($"{this.Name} attacked player.");
                    }
                    player.Damage(DamageModifier, false);
                    hasSeenPlayer = true;
                }
            });
            int count = this.CurrentRoom.GetExitCount();
            int itterations = 0;
            this.CurrentRoom.ForEachExit((exit) => {
                if (hasSeenPlayer) {
                    if (exit.Value.ContainsInhabitant("player") || (this.CurrentRoom.ContainsInhabitant("player"))) {
                        if (this.CurrentRoom.ContainsInhabitant("player")) {
                            return;
                        }
                        if (random.Next(0, 100) <= 50) {
                            this.CurrentRoom.RemoveInhabitant(this.Name);
                            this.CurrentRoom = exit.Value;
                            exit.Value.AddInhabitant(this.Name, this);
                        }
                    } else if (itterations == count) {
                        hasSeenPlayer = false;
                    }

                    itterations++;
                }
            });
        }
    }

    private void OnDeath() {
        this.CurrentRoom.RemoveInhabitant(this.Name);
        Console.WriteLine($"{this.Name} died and dropped {inventory.GetContents()}");
        this.inventory.ForEachItemName((itemName) => {
            this.DropToChest(itemName);
        });
    }

    public void SetWeapon(Item item) {
        this.inventory.Put(item.Name, item);
        item.ApplyModifiers(this);
        item.Equiped = true;
        this.mainWeapon = item;
    }
}