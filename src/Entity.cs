namespace Zuul;

public class Entity {
    // fields
    private int damageModifier;
    private int speedModifier;

    // attributes
    public int DamageModifier { get { return damageModifier; } set { damageModifier = value; } }
    public int SpeedModifier { get { return speedModifier; } set { speedModifier = value; } }
    
    // constructor
    public Entity(int damageModifier, int speedModifier) {
        this.damageModifier = damageModifier;
        this.speedModifier = speedModifier;
    }
}