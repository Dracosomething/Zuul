namespace Zuul;

class Trap : Entity {
    // fields
    private string name;
    private string description;
    private bool hasNextRoom;
    private Room nextRoom;
    private Action function;
    
    // properties
    public Action Function { get { return function; }  set { this.function = value; } }
    public string Name { get { return name; }  set { this.name = value; } }
    public string Description { get { return description; }  set { this.description = value; } }
    public bool HasNextRoom { get { return hasNextRoom; }  set { this.hasNextRoom = value; } }
    public Room NextRoom { get { return nextRoom; }  set { this.nextRoom = value; } }
    
    // constructor
    public Trap(int damage, string name, string description, bool hasNextRoom, Action func) : base(damage, 0, 0) {
        this.name = name;
        this.description = description;
        this.hasNextRoom = hasNextRoom;
        this.nextRoom = null;
        this.function = func;
    }
    
    public Trap(int damage, string name, string description, bool hasNextRoom) : base(damage, 0, 0) {
        this.name = name;
        this.description = description;
        this.hasNextRoom = hasNextRoom;
        this.nextRoom = null;
        this.function = null;
    }
    
    public Trap(int damage, string name, string description, bool hasNextRoom, Room nextRoom, Action func) : base(damage, 0, 0) {
        this.name = name;
        this.description = description;
        this.hasNextRoom = hasNextRoom;
        this.nextRoom = nextRoom;
        this.function = func;
    }
    
    // methods
    public void Disarm() {
        this.Discard();
        this.CurrentRoom = null;
        Console.WriteLine($"Successfully disarmed {this.name}");
    }

    public void Discard() {
        this.CurrentRoom.Inhabitants.Remove(this.name);
    }

    public void Tick() {
        foreach (var currentRoomInhabitant in this.CurrentRoom.Inhabitants) {
            if (!(currentRoomInhabitant.Value is Trap)) {
                this.function.Invoke();
            }
        }
    }
}