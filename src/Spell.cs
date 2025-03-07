using System.Net.Mime;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Zuul;

class Spell {
    // fields
    private string name;
    private string description;
    private int manaCost;
    private Action effect;
    private bool isSingleUse;
    private MagicEntity caster;
    
    // properties
    public string Name { get { return this.name; } set { this.name = value; } }
    public string Description { get { return this.description; } set { this.description = value; } }
    public int ManaCost { get { return this.manaCost; } set { this.manaCost = value; } }
    [JsonIgnore]
    public Action Effect { get { return this.effect; } set { this.effect = value; } }
    public bool IsSingleUse { get { return this.isSingleUse; } set { this.isSingleUse = value; } }
    [JsonIgnore]
    public MagicEntity Caster { get { return this.caster; } set { this.caster = value; } }
    
    // constructor
    public Spell(string name, string description, int manaCost, bool isSingleUse) {
        this.name = name;
        this.description = description;
        this.effect = null;
        this.manaCost = manaCost;
        this.isSingleUse = isSingleUse;
    }
}