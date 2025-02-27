namespace Zuul;

class Item {
    // fields
    private int weight;
    private string description;
    private int speedModifier;
    private int damageModifier;
    
    // attributes
    public int Weight { get { return weight; } }
    public string Description { get { return description; } }

    // constructor
    public Item(int weight, string description) {
        this.weight = weight;
        this.description = description;
        this.speedModifier = 0;
        this.damageModifier = 0;
    }
    
    public Item(int weight, int speedModifier, int damageModifier, string description) {
        this.weight = weight;
        this.description = description;
        this.speedModifier = speedModifier;
        this.damageModifier = damageModifier;
    }

    public Item(int weight, int modifier, string description, string modType) {
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
        Item clone = new Item(this.weight, this.speedModifier, this.damageModifier, this.description);
        return clone;
    }
}