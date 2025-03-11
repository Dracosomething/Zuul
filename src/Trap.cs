namespace Zuul;

class Trap : Entity {
    // fields
    private string describtion;
    private bool hasNextRoom;
    private Room nextRoom;
    private Action function;
    
    // properties
    public Action Function { set { this.function = value; } }
    
    // constructor
    public Trap(int damage, string name, string describtion, bool hasNextRoom, Action func) : base(damage, 0, 0, name) {
        this.describtion = describtion;
        this.hasNextRoom = hasNextRoom;
        this.nextRoom = null;
        this.function = func;
    }
    
    public Trap(int damage, string name, string describtion, bool hasNextRoom) : base(damage, 0, 0, name) {
        this.describtion = describtion;
        this.hasNextRoom = hasNextRoom;
        this.nextRoom = null;
        this.function = null;
    }
    
    public Trap(int damage, string name, string describtion, bool hasNextRoom, Room nextRoom, Action func) : base(damage, 0, 0, name) {
        this.describtion = describtion;
        this.hasNextRoom = hasNextRoom;
        this.nextRoom = nextRoom;
        this.function = func;
    }
    
    // methods
    /// <summary>
    /// deactivates a trap
    /// </summary>
    public void Disarm() {
        this.Discard();
        this.CurrentRoom = null;
        Console.WriteLine($"Successfully disarmed {this.Name}");
    }

    /// <summary>
    /// removes a trap from the room
    /// </summary>
    public void Discard() {
        this.CurrentRoom.RemoveInhabitant(this.Name);
        this.CurrentRoom = null;
    }

    /// <summary>
    /// executes the logic of the trap every game update
    /// </summary>
    public new void Tick() {
        this.CurrentRoom.ForEachInhabitant((currentRoomInhabitant) => {
            if (!(currentRoomInhabitant.Value is Trap)) {
                this.function.Invoke();
                Console.WriteLine(this.describtion);
            }
        });
    }
}