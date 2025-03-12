namespace Zuul;

class MagicEntity : Entity {
    // fields
    private int maxHealth;
    private int mana;
    private int maxMana;
    private int magicPower;
    private Dictionary<string, Spell> spellBook;
    private Inventory backPack;
    
    // properties
    public int MaxHealth { get { return this.maxHealth; } set { maxHealth = value; } }
    public int MaxMana { get { return maxMana; } set { this.maxMana = value; } }
    public int Mana { get { return mana; } set { this.mana = value; } }
    public int MagicPower { get { return magicPower; } set { this.magicPower = value; } }
    public Dictionary<string, Spell> SpellBook { get { return spellBook; } set { this.spellBook = value; } }
    public Inventory BackPack { get { return this.backPack; } set { this.backPack = value; } }

    // constructor
    public MagicEntity(int damageModifier, int armorModifier, int health, int mana, int magicPower, string name) : base(
        damageModifier, armorModifier, health, name) {
        this.mana = mana;
        this.maxMana = mana;
        this.magicPower = magicPower;
        this.spellBook = new Dictionary<string, Spell>();
        this.maxHealth = health;
    }
    
    // method
    /// <summary>
    /// checks if an magic entitie is alive
    /// </summary>
    /// <returns>if the current health is above 0 and if mana is above 0</returns>
    public new bool IsAlive() {
        return base.IsAlive() && this.mana > 0;
    }

    /// <summary>
    /// Runs every game update.
    /// </summary>
    public new void Tick() {
        base.Tick();
        HealMana(1);
        if (mana > maxMana) {
            mana--;
            this.Damage(1, true);
        }
        List<Item> toBeRemoved = new List<Item>();
        this.backPack.ForEachItem((item) => {
            if (item.DecayTicks == 0) {
                toBeRemoved.Add(item);
            } else if (item.DecayTicks > 0) {
                item.DecayTicks -= 1;
            }
        });
        foreach (var item in toBeRemoved) {
            this.backPack.Remove(item.Name);
        }
    }

    /// <summary>
    /// casts a spell
    /// </summary>
    /// <param name="spellName">the name of the spell that needs to be casted</param>
    public void UseSpell(string spellName) {
        Spell spell = spellBook[spellName];
        if (spell == null) {
            Console.WriteLine("You dont have that spell.");
            return;
        }
        if ((mana -= spell.ManaCost) < 0) return;
        spell.Caster = this;
        mana -= spell.ManaCost;
        spell.Effect.Invoke();
        if (spell.IsSingleUse) {
            spellBook.Remove(spell.Name);
            Console.WriteLine($"{spell.Name} can no longer be used.");
        }
        spell.Caster = null;
    }

    /// <summary>
    /// heals mana of a magic entity
    /// </summary>
    /// <param name="amount">The amount of mana that needs to be healed</param>
    public void HealMana(int amount) {
        if ((mana+=amount) > maxMana) return;
        mana += amount;
    }
}