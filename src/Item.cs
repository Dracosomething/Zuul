namespace Zuul;

class Item {
    // fields
    private int weight;
    private string description;
    
    // attributes
    public int Weight { get; }
    public string Description { get; }

    // constructor
    public Item(int weight, string description) {
        weight = weight;
        description = description;
    }
}