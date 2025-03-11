using System.IO.Pipes;
using System.Text.Json.Serialization;

namespace Zuul;

class Item {
    // fields
    private string description;
    private string name;
    private int weight;
    private int armorModifier;
    private int damageModifier;
    private int healthModifier;
    private int magicPowerModifier;
    private int manaModifier;
    
    // attributes
    public int Weight { get { return weight; } set { weight = value; } }
    public string Description { get { return description; } set { description = value; } }
    public string Name { get { return this.name; } set { name = value; } }
    public bool Equiped { get; set; }
    public int ArmorModifier { get { return armorModifier; } set { armorModifier = value; } }
    public int DamageModifier { get { return damageModifier; } set { damageModifier = value; } }
    public int HealthModifier { get { return healthModifier; } set { healthModifier = value; } }
    public int MagicPowerModifier { get { return magicPowerModifier; } set { magicPowerModifier = value; } }
    public int ManaModifier { get { return manaModifier; } set { manaModifier = value; } }
    public int DecayTicks { get; set; }
    
    // constructor
    public Item(int weight, string description, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        this.armorModifier = 0;
        this.damageModifier = 0;
        Equiped = false;
        DecayTicks = -1;
    }
    
    public Item(int weight, int armorModifier, int healthModifier, string description, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        this.armorModifier = armorModifier;
        this.healthModifier = healthModifier;
        this.damageModifier = 0;
        this.manaModifier = 0;
        this.magicPowerModifier = 0;
        Equiped = false;
        DecayTicks = -1;
    }
    
    public Item(int weight, int armorModifier, int damageModifier, int healthModifier, string description, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        this.healthModifier = healthModifier;
        this.armorModifier = armorModifier;
        this.damageModifier = damageModifier;
        this.manaModifier = 0;
        this.magicPowerModifier = 0;
        Equiped = false;
        DecayTicks = -1;
    }
    
    public Item(int weight, int armorModifier, int damageModifier, int healthModifier, int manaModifier, string description, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        this.healthModifier = healthModifier;
        this.armorModifier = armorModifier;
        this.damageModifier = damageModifier;
        this.manaModifier = manaModifier;
        this.magicPowerModifier = 0;
        Equiped = false;
        DecayTicks = -1;
    }
    
    public Item(int weight, int armorModifier, int damageModifier, int healthModifier, int manaModifier, int magicPowerModifier, string description, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        this.healthModifier = healthModifier;
        this.armorModifier = armorModifier;
        this.damageModifier = damageModifier;
        this.manaModifier = manaModifier;
        this.magicPowerModifier = magicPowerModifier;
        Equiped = false;
    }

    public Item(int weight, int modifier, string description, string modType, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        switch (modType.ToLower()) {
            case "armor":
                this.armorModifier = modifier;
                this.damageModifier = 0;
                this.manaModifier = 0;
                this.magicPowerModifier = 0;
                this.healthModifier = 0;
                break;
            case "damage":
                this.damageModifier = modifier;
                this.armorModifier = 0;
                this.manaModifier = 0;
                this.magicPowerModifier = 0;
                this.healthModifier = 0;
                break;
            case "health":
                this.healthModifier = modifier;
                this.damageModifier = 0;
                this.manaModifier = 0;
                this.magicPowerModifier = 0;
                this.armorModifier = 0;
                break;
            case "mana":
                this.manaModifier = modifier;
                this.damageModifier = 0;
                this.armorModifier = 0;
                this.magicPowerModifier = 0;
                this.healthModifier = 0;
                break;
            case "magicPower":
                this.magicPowerModifier = modifier;
                this.damageModifier = 0;
                this.manaModifier = 0;
                this.armorModifier = 0;
                this.healthModifier = 0;
                break;
        }
        Equiped = false;
    }

    [JsonConstructor]
    public Item() {
        this.name = null;
        this.description = null;
        this.weight = 0;
        this.armorModifier = 0;
        this.damageModifier = 0;
        this.healthModifier = 0;
        this.manaModifier = 0;
        this.magicPowerModifier = 0;
        this.Equiped = false;
    }
    
    // methods
    /// <summary>
    /// used to apply the stat modifiers of the item to the entity
    /// </summary>
    /// <param name="entity">the entity the modifiers need to get applied to.</param>
    public void ApplyModifiers(Entity entity) {
        entity.DamageModifier += damageModifier;
        entity.ArmorModifier += armorModifier;
        if (entity.GetType().Name.Equals("MagicEntity")) {
            MagicEntity magicEntity = (MagicEntity)entity;
            magicEntity.MaxMana += manaModifier;
            magicEntity.MagicPower += magicPowerModifier;
        } 
        if (entity.GetType().Name.Equals("Player")) {
            Player player = (Player)entity;
            player.MaxHealth += healthModifier;
        } else {
            entity.Health += healthModifier;
        }
    }

    /// <summary>
    /// used to remove the stat modifiers of the item from the entity
    /// </summary>
    /// <param name="entity">the entity the modifiers need to get removed from.</param>
    public void RemoveModifiers(Entity entity) {
        entity.DamageModifier -= damageModifier;
        entity.ArmorModifier -= armorModifier;
        if (entity.GetType().Name.Equals("MagicEntity")) {
            MagicEntity magicEntity = (MagicEntity)entity;
            magicEntity.MaxMana -= manaModifier;
            magicEntity.MagicPower -= magicPowerModifier;
        } 
        if (entity.GetType().Name.Equals("Player")) {
            Player player = (Player)entity;
            player.MaxHealth -= healthModifier;
        } else {
            entity.Health -= healthModifier;
        }    
    }

    /// <summary>
    /// used to get a clone of an item
    /// </summary>
    /// <returns>an exact copy of an item</returns>
    public Item Clone() {
        Item clone = new Item(this.weight, this.armorModifier, this.damageModifier, this.healthModifier, this.manaModifier, this.magicPowerModifier, this.description, this.name);
        return clone;
    }
}