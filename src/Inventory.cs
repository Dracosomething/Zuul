namespace Zuul;

class Inventory {
    // fields
    private int maxWeight;
    private Dictionary<string, Item> items;
    
    // attributes
    public int MaxWeight { get { return maxWeight; } set { maxWeight = value; } }
    public Dictionary<string, Item> Items { get {return items; } set { items = value; } }
    
    // constructor
    public Inventory(int maxWeight) {
        this.maxWeight = maxWeight;
        this.items = new Dictionary<string, Item>();
    }
    
    // methods
    /// <summary>
    /// adds something to the inventory.
    /// </summary>
    /// <param name="itemName">the name of an item</param>
    /// <param name="item">the item to be added</param>
    /// <returns>true if an item is added, false if it fials</returns>
    public bool Put(string itemName, Item item) {
        int itemWeight = item.Weight;
        if (itemWeight <= FreeWeight()) {
            items.TryAdd(itemName, item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets an item from the inventory
    /// </summary>
    /// <param name="itemName">The name of the item to be gotten</param>
    /// <returns>the item or null if it fails</returns>
    public Item Get(string itemName) {
        if (items.ContainsKey(itemName)) {
            return items[itemName];
        }
        return null;
    }

    /// <summary>
    /// used to get the total weight of all items in the inventory
    /// </summary>
    /// <returns>the total weight in the inventory</returns>
    public int TotalWeight() {
        int total = 0;
        foreach (var keyValuePair in items) {
            total += keyValuePair.Value.Weight;
        }
        return total;
    }

    /// <summary>
    /// Used to get the free space in an inventory
    /// </summary>
    /// <returns>the weight free</returns>
    public int FreeWeight() {
        return maxWeight - TotalWeight();
    }

    /// <summary>
    /// shows the contents of the inventory
    /// </summary>
    /// <returns>The inventory as a string</returns>
    public string Show() {
        string itemString = "";
        int loopTimes = 0;
        foreach (var keyValuePair in items) {
            loopTimes++;
            itemString += keyValuePair.Key;
            itemString += $"[description: \"{keyValuePair.Value.Description}\", weight: {keyValuePair.Value.Weight}, is equipped: {keyValuePair.Value.Equiped}]";
            if (items.Count > loopTimes) {
                itemString += ", \n";
            }
        }
        
        return itemString;
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="itemName">The name of the item that should be removed</param>
    /// <returns>if the item is removed</returns>
    public bool Remove(string itemName) {
        return items.Remove(itemName);
    }
    
    /// <summary>
    /// loops through all KeyValuePair in the items dictionary
    /// </summary>
    /// <param name="consumer">the code that should get applied to the KeyValuePair</param>
    public void ForEach(Action<KeyValuePair<string, Item>> consumer) {
        foreach (var keyValuePair in items) {
            consumer.Invoke(keyValuePair);
        }
    }
    
    /// <summary>
    /// loops through all string in the items dictionary
    /// </summary>
    /// <param name="consumer">the code that should get applied to the string</param>
    public void ForEachItemName(Action<string> consumer) {
        foreach (var keyValuePair in items) {
            consumer.Invoke(keyValuePair.Key);
        }
    }
    
    /// <summary>
    /// loops through all <code>Item</code> in the items dictionary
    /// </summary>
    /// <param name="consumer">the code that should get applied to the Item</param>
    public void ForEachItem(Action<Item> consumer) {
        foreach (var keyValuePair in items) {
            consumer.Invoke(keyValuePair.Value);
        }
    }

    /// <summary>
    /// used to get all names of the items in the inventory
    /// </summary>
    /// <returns>A string of all item names in the inventory</returns>
    public string GetContents() {
        string itemsAsString = "";
        int loopTimes = 0;
        ForEachItemName((item) => {
            loopTimes++;
            itemsAsString += item;
            if (items.Count > loopTimes) {
                itemsAsString += ", ";
            } else {
                itemsAsString += ".";
            }
        });
        return itemsAsString;
    }

    /// <summary>
    /// used to get the size of the items dictionary
    /// </summary>
    /// <returns>The amount of objects in the dictionary</returns>
    public int Count() {
        return this.items.Count;
    }
}