namespace Zuul;

class Item {
    // fields
    private string description;
    private string name;
    private int weight;
    private int speedModifier;
    private int damageModifier;
    
    // attributes
    public int Weight { get { return weight; } }
    public string Description { get { return description; } }
    public string Name { get { return this.name; } }

    // constructor
    public Item(int weight, string description, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        this.speedModifier = 0;
        this.damageModifier = 0;
    }
    
    public Item(int weight, int speedModifier, int damageModifier, string description, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        this.speedModifier = speedModifier;
        this.damageModifier = damageModifier;
    }

    public Item(int weight, int modifier, string description, string modType, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        switch (modType.ToLower()) {
            case "speed":
                this.speedModifier = modifier;
                break;
            case "damage":
                this.damageModifier = modifier;
                break;
        }
    }
    
    // methods
    public void ApplyModifiers(Entity entity) {
        entity.DamageModifier += damageModifier;
        entity.SpeedModifier += speedModifier;
    }

    public void RemoveModifiers(Entity entity) {
        entity.DamageModifier -= damageModifier;
        entity.SpeedModifier -= speedModifier;
    }

    public Item Clone() {
        Item clone = new Item(this.weight, this.speedModifier, this.damageModifier, this.description, this.name);
        return clone;
    }
}