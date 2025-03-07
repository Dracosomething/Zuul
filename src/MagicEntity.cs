namespace Zuul;

class MagicEntity : Entity {
    // fields
    private int mana;
    private int maxMana;
    private int magicPower;
    private Dictionary<string, Spell> spellBook;
    
    // properties
    public int MaxMana { get { return maxMana; } set { this.maxMana = value; } }
    public int Mana { get { return mana; } set { this.mana = value; } }
    public int MagicPower { get { return magicPower; } set { this.magicPower = value; } }
    public Dictionary<string, Spell> SpellBook { get { return spellBook; } set { this.spellBook = value; } }
    
    // constructor
    public MagicEntity(int damageModifier, int armorModifier, int health, int mana, int magicPower, string name) : base(
        damageModifier, armorModifier, health, name) {
        this.mana = mana;
        this.maxMana = mana;
        this.magicPower = magicPower;
        this.spellBook = null;
    }
    
    // method
    public new bool IsAlive() {
        return base.IsAlive() && this.mana > 0;
    }

    public new void Tick() {
        this.mana++;
    }

    public void UseSpell(Spell spell) {
        if ((mana -= spell.ManaCost) < 0) return;
        mana -= spell.ManaCost;
        spell.Effect.Invoke();
        if (spell.IsSingleUse) {
            spellBook.Remove(spell.Name);
            Console.WriteLine($"{spell.Name} can no longer be used.");
        }
    }
}