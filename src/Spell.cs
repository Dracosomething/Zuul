namespace Zuul;

class Spell {
    // fields
    private string name;
    private string description;
    private int manaCost;
    private Action effect;
    private bool isSingleUse;
    
    // properties
    public string Name { get { return this.name; } set { this.name = value; } }
    public string Description { get { return this.description; } set { this.description = value; } }
    public int ManaCost { get { return this.manaCost; } set { this.manaCost = value; } }
    public Action Effect { get { return this.effect; } set { this.effect = value; } }
    public bool IsSingleUse { get { return this.isSingleUse; } set { this.isSingleUse = value; } }
    
    // constructor
    public Spell(string name, string description, Action effect, int manaCost, bool isSingleUse) {
        this.name = name;
        this.description = description;
        this.effect = effect;
        this.manaCost = manaCost;
        this.isSingleUse = isSingleUse;
    }
}