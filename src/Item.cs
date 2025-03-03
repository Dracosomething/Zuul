﻿namespace Zuul;

class Item {
    // fields
    private string description;
    private string name;
    private int weight;
    private int armorModifier;
    private int damageModifier;
    private int healthModifier;
    
    // attributes
    public int Weight { get { return weight; } }
    public string Description { get { return description; } }
    public string Name { get { return this.name; } }
    public bool Equiped { get; set; }

    // constructor
    public Item(int weight, string description, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        this.armorModifier = 0;
        this.damageModifier = 0;
        Equiped = false;
    }
    
    public Item(int weight, int armorModifier, int healthModifier, string description, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        this.armorModifier = armorModifier;
        this.healthModifier = healthModifier;
        Equiped = false;
    }
    
    public Item(int weight, int armorModifier, int damageModifier, int healthModifier, string description, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        this.healthModifier = healthModifier;
        this.armorModifier = armorModifier;
        this.damageModifier = damageModifier;
        Equiped = false;
    }

    public Item(int weight, int modifier, string description, string modType, string name) {
        this.name = name;
        this.weight = weight;
        this.description = description;
        switch (modType.ToLower()) {
            case "armor":
                this.armorModifier = modifier;
                break;
            case "damage":
                this.damageModifier = modifier;
                break;
            case "health":
                this.healthModifier = modifier;
                break;
        }
        Equiped = false;
    }
    
    // methods
    public void ApplyModifiers(Entity entity) {
        entity.DamageModifier += damageModifier;
        entity.ArmorModifier += armorModifier;
        entity.Health += healthModifier;
    }

    public void RemoveModifiers(Entity entity) {
        entity.DamageModifier -= damageModifier;
        entity.ArmorModifier -= armorModifier;
        entity.Health -= healthModifier;
    }

    public Item Clone() {
        Item clone = new Item(this.weight, this.armorModifier, this.damageModifier, this.healthModifier, this.description, this.name);
        return clone;
    }
}