namespace Zuul;

class Enemy : Entity {
    // fields
    private string name;
    private Inventory inventory;
    private Item mainWeapon;
    private bool hasSeenPlayer;
    
    // attributes
    public string Name { get { return name; } }
    public Inventory Inventory { get { return inventory; } }

    // constructor
    public Enemy(int hp, int dmg, int invSize, int armor, string enemyName) : base(dmg, armor, hp) {
        name = enemyName;
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

    public void Tick() {
        Random random = new Random();
        if (!IsAlive()) {
            OnDeath();
        }
        else {
            this.CurrentRoom.AddInhabitant(this.Name, this);
            CurrentRoom.ForEachInhabitant((inhabitant) => {
                if (inhabitant.Value is Player player) {
                    Console.WriteLine(
                        $"{this.name} attacked player using {(mainWeapon == null ? "fists" : mainWeapon.Name)}.");
                    player.Damage(DamageModifier);
                    hasSeenPlayer = true;
                }
            });
            this.CurrentRoom.ForEachExit((exit) => {
                if (hasSeenPlayer) {
                    if (exit.Value.ContainsInhabitant("player") || (this.CurrentRoom.ContainsInhabitant("player"))) {
                        if (random.Next(0, 100) <= 50) {
                            this.CurrentRoom.RemoveInhabitant(this.name);
                            this.CurrentRoom = exit.Value;
                            exit.Value.AddInhabitant(this.name, this);
                        }
                    } else {
                        hasSeenPlayer = false;
                    }
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
        this.inventory.Put(nameof(item), item);
        item.ApplyModifiers(this);
        item.Equiped = true;
        this.mainWeapon = item;
    }
}