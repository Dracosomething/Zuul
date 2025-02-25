namespace Zuul;

class Player {
    public Room CurrentRoom { get; set; }
    private int Health;

    public Player() {
        CurrentRoom = null;
        Health = 100;
    }

    public void Damage(int amount)
    {
        Health -= amount;
    }

    public void Heal(int amount)
    {
        Health += amount;
    }
    
    
}