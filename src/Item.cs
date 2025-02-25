namespace Zuul;

class Item {
    // fields
    private int weight;
    private string description;
    
    // attributes
    public int Weight
    {
        get { return weight; }
    }

    public string Description
    {
        get { return description; }
    }

    // constructor
    public Item(int weight, string description) {
        this.weight = weight;
        this.description = description;
    }
}