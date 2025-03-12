namespace Zuul;

class BossEnemy : MagicEntity {
    // fields
    private Dictionary<int, Spell> abilities;
    private Item mainWeapon;
    private bool hasSeenPlayer;
    
    // constructor
    public BossEnemy(string name, int damageModifier, int armorModifier, int health, int mana, 
        int magicPower, Dictionary<int, Spell> abilities, Item mainWeapon) : base(
        damageModifier, armorModifier, health, mana, magicPower, name) {
        this.abilities = abilities;
        this.mainWeapon = mainWeapon;
        this.hasSeenPlayer = false;
        foreach (var ability in abilities) {
            this.SpellBook.Add(ability.Value.Name, ability.Value);
        }
    }
    
    // methods
    public new void Tick() {
        Random random = new Random();
        // checks if the enemy is alive
        if (!IsAlive()) {
            OnDeath();
        } else {
            base.Tick();
            HealMana(5);
            // selects the next move to use
            Spell nextMove = null;
            while (nextMove == null) {
                foreach (var ability in abilities) {
                    if (random.Next(0, 100) <= ability.Key) {
                        nextMove = ability.Value;
                        break;
                    }
                }
            }
            // uses set attack
            UseSpell(nextMove.Name);
            // makes shure it is in the room
            this.CurrentRoom.AddInhabitant(this.Name, this);
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
                        // 50% chance for the enemy to move
                        if (random.Next(0, 100) <= 25) {
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
        Console.WriteLine($"{this.Name} died and dropped {BackPack.GetContents()}");
        this.BackPack.ForEach((item) => {
            if (CurrentRoom.Chest.Put(item.Key, item.Value)) {
                if (item.Value.Equiped) {
                    item.Value.RemoveModifiers(this);
                }
                BackPack.Remove(item.Key);
            }
        });
    }
}