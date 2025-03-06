using System.Collections.Generic;
using Zuul;

class Room {
	// Private fields
	private string description;
	private string name;
	private Dictionary<string, Room> exits; // stores exits of this room.
	private Inventory chest;
	private Item conditionalItem;
	private Dictionary<string, dynamic> inhabitants;

	// properties
	public Inventory Chest { get { return this.chest; } }
	public Item ConditionalItem { get { return this.conditionalItem; } set { this.conditionalItem = value; } }
	public bool IsUnlocked { get; set; }
	public string Name { get { return this.name; } }
	
	/*
	 * Create a room described "description". Initially, it has no exits.
	 * "description" is something like "in a kitchen" or "in a court yard".
	 */
	public Room(string desc, string name) {
		description = desc;
		this.name =  name;
		exits = new Dictionary<string, Room>();
		chest = new Inventory(Int32.MaxValue-1);
		IsUnlocked = true;
		inhabitants = new Dictionary<string, dynamic>();
	}
	
	public Room(string desc, string name, Item conditionalItem) {
		description = desc;
		this.name = name;
		exits = new Dictionary<string, Room>();
		chest = new Inventory(Int32.MaxValue-1);
		inhabitants = new Dictionary<string, dynamic>();
		this.conditionalItem = conditionalItem;
		IsUnlocked = false;
	}

	// Define an exit for this room.
	public void AddExit(string direction, Room neighbor) {
		exits.Add(direction, neighbor);
	}

	// Return the description of the room.
	public string GetShortDescription() {
		return description;
	}
	
	/// <summary>
	/// used to get the full description of a room
	/// </summary>
	/// <returns>Return a long description of this room, in the form:
	///				You are in the kitchen.
	///				Exits: north, west
	///	 </returns>
	public string GetLongDescription() {
		string str = "You are ";
		int itterations = 0;
		str += description;
		str += ".\n";
		str += GetExitString();
		str += ".\n";
		if (inhabitants.Count != 1) {
			str += "the rooms inhabitants are ";
			foreach (var keyValuePair in inhabitants) {
				itterations++;
				if (inhabitants.Count > itterations) {
					if (!(keyValuePair.Value is Player || keyValuePair.Value is Trap)) {
						str += keyValuePair.Key;
						str += ",\nhealth: ";
						str += (keyValuePair.Value).Health;
						if (itterations != inhabitants.Count-1) {
							str += ",\n";
						}
					}
				}
			}
			str += ".\n";
		}
		if (this.chest.Count() != 0) {
			str += "The room contains these items\n";
			str += this.Chest.Show();
		}
		return str;
	}

	/// <returns> the room that is reached if we go from this room in direction
	/// "direction". If there is no room in that direction, return null. </returns>
	public Room GetExit(string direction) {
		if (exits.ContainsKey(direction)) {
			return exits[direction];
		}
		return null;
	}

	/// <returns> a string describing the room's exits, for example 
	/// "Exits: north, west".</returns>
	private string GetExitString() {
		string str = "Exits: ";
		str += String.Join(", ", exits.Keys);

		return str;
	}

	public Room Clone() {
		return new Room(this.description, this.name, this.conditionalItem);
	}

	public bool HasExit(string direction) {
		return GetExit(direction) != null;
	}

	public void ForEachExit(Action<KeyValuePair<string, Room>> consumer) {
		foreach (var keyValuePair in exits) {
			consumer.Invoke(keyValuePair);
		}
	}

	public void AddInhabitant( string name, Entity entity) {
		this.inhabitants.TryAdd(name, entity);
	}

	public void RemoveInhabitant(string name) {
		this.inhabitants.Remove(name);
	}

	public bool ContainsInhabitant(string name) {
		return this.inhabitants.ContainsKey(name);
	}

	public void ForEachInhabitant(Action<KeyValuePair<string, dynamic>> action) {
		try {
			foreach (var inhabitant in inhabitants) {
				action.Invoke(inhabitant);
			}
		} catch (Exception e) {}
	}

	public int GetExitCount() {
		return this.exits.Count;
	}
}
