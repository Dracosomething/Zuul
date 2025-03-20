using System.Numerics;

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
        this.BackPack = new Inventory(9999);
        if (mainWeapon != null) {
            this.BackPack.Put(mainWeapon.Name, mainWeapon);
        }
        this.hasSeenPlayer = false;
        foreach (var ability in abilities) {
            if (ability.Value != null) {
                // Console.WriteLine(ability.Value.Name);
                this.SpellBook.Add(ability.Value.Name, ability.Value);
            }
        }
    }
    
    // methods
    public new void Tick() {
        Random random = new Random();
        // checks if the enemy is alive
        if (!IsAlive()) {
            OnDeath();
        } else {
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
            base.Tick();
        }
    }
    
    /// <summary>
    /// code that drops an enemies items and removes it from the world
    /// </summary>
    private void OnDeath() {
        this.CurrentRoom.RemoveInhabitant(this.Name);
        Console.WriteLine($"{this.Name} died and dropped {BackPack.GetContents()}");
        this.BackPack.ForEach((item) => {
            if (item.Value.DecayTicks == -1) {
                if (CurrentRoom.Chest.Put(item.Key, item.Value)) {
                    if (item.Value.Equiped) {
                        item.Value.RemoveModifiers(this);
                    }

                    BackPack.Remove(item.Key);
                }
            }
        });
        Spell learnSpell = abilities.Values.ToArray()[0];
        if (learnSpell != null) {
            Console.WriteLine($"{learnSpell.Name} is now available to learn.");
            this.CurrentRoom.AddSpell(learnSpell);
        }
    }
}