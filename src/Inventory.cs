namespace Zuul;

class Inventory {
    // fields
    private int maxWeight;
    private Dictionary<string, Item> items;
    
    // constructor
    public Inventory(int maxWeight) {
        this.maxWeight = maxWeight;
        this.items = new Dictionary<string, Item>();
    }
    
    // methods
    public bool Put(string itemName, Item item) {
        int itemWeight = item.Weight;
        if (itemWeight <= FreeWeight()) {
            items.Add(itemName, item);
            return true;
        }
        return false;
    }

    public Item Get(string itemName) {
        if (items.ContainsKey(itemName)) {
            return items[itemName];
        }
        return null;
    }

    public int TotalWeight() {
        int total = 0;
        foreach (var keyValuePair in items) {
            total += keyValuePair.Value.Weight;
        }
        return total;
    }

    public int FreeWeight() {
        return maxWeight - TotalWeight();
    }
}