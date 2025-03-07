namespace Zuul;

class MagicEntity : Entity {
    // fields
    private int mana;
    private int maxMana;
    private int magicPower;
    private Dictionary<string, Spell> spellBook;
    private Inventory backPack;
    
    // properties
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
    }
    
    // method
    public new bool IsAlive() {
        return base.IsAlive() && this.mana > 0;
    }

    public new void Tick() {
        base.Tick();
        HealMana(1);
        if (mana > maxMana) {
            mana--;
            this.Damage(1, true);
        }
    }

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
    }

    public void HealMana(int amount) {
        if ((mana+=amount) > maxMana) return;
        mana += amount;
    }
}