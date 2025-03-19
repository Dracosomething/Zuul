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
	private Dictionary<string, Spell> spellBook;

	// properties
	public Inventory Chest { get { return this.chest; } }
	public Item ConditionalItem { get { return this.conditionalItem; } set { this.conditionalItem = value; } }
	public bool IsUnlocked { get; set; }
	public string Name { get { return this.name; } }

	// constructor
	public Room(string desc, string name) : 
		this(desc, name, null) {}
	
	public Room(string desc, string name, Item conditionalItem) {
		description = desc;
		this.name = name;
		exits = new Dictionary<string, Room>();
		chest = new Inventory(Int32.MaxValue-1);
		inhabitants = new Dictionary<string, dynamic>();
		this.spellBook = new Dictionary<string, Spell>();
		this.conditionalItem = conditionalItem;
		IsUnlocked = false;
	}

	// methods
	/// <summary>
	/// Define an exit for this room.
	/// </summary>
	/// <param name="direction">The direction of the exit</param>
	/// <param name="neighbor">the room it links to</param>
	public void AddExit(string direction, Room neighbor) {
		exits.Add(direction, neighbor);
	}

	/// <summary>
	/// Return the description of the room.
	/// </summary>
	/// <returns>the description of the room.</returns>
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
		if (this.spellBook.Count() != 0) {
			str += "The room has a spell book filled with these spells\n";
			foreach (var spell in spellBook)
			{
				str += spell.Key;
				str += ", ";
				str += spell.Value.Description;
			}
		}
		return str;
	}

	/// <summary>
	///	used to get the room an exit leads to
	/// </summary>
	/// <returns>
	/// the room that is reached if we go from this room in direction
	/// "direction". If there is no room in that direction, return null.
	/// </returns>
	public Room GetExit(string direction) {
		if (exits.ContainsKey(direction)) {
			return exits[direction];
		}
		return null;
	}

	/// <summary>
	/// gets all exits of a room as a string.
	/// </summary>
	/// <returns>
	/// a string describing the room's exits, for example 
	/// "Exits: north, west".
	/// </returns>
	private string GetExitString() {
		string str = "Exits: ";
		str += String.Join(", ", exits.Keys);

		return str;
	}

	/// <summary>
	/// used to get an exact copy of a room
	/// </summary>
	/// <returns>A copy of the room</returns>
	public Room Clone() {
		return new Room(this.description, this.name, this.conditionalItem);
	}

	/// <summary>
	/// checks if the room has an exit
	/// </summary>
	/// <param name="direction">The direction of the exit</param>
	/// <returns>if the exit is not null</returns>
	public bool HasExit(string direction) {
		return GetExit(direction) != null;
	}

	/// <summary>
	/// loops through all exits of a room
	/// </summary>
	/// <param name="consumer">The code executed for each exit</param>
	public void ForEachExit(Action<KeyValuePair<string, Room>> consumer) {
		foreach (var keyValuePair in exits) {
			consumer.Invoke(keyValuePair);
		}
	}

	/// <summary>
	/// adds a new inhabitant to a room
	/// </summary>
	/// <param name="name">the name of the entity</param>
	/// <param name="entity">The entity to be added</param>
	public void AddInhabitant( string name, Entity entity) {
		this.inhabitants.TryAdd(name, entity);
	}

	/// <summary>
	/// removes an inhabitant
	/// </summary>
	/// <param name="name">The name of the inhabitant</param>
	public void RemoveInhabitant(string name) {
		this.inhabitants.Remove(name);
	}

	/// <summary>
	/// checks if the room has a specific inhabitant
	/// </summary>
	/// <param name="name">The name of the inhabitant</param>
	/// <returns>if the inhbitants dictionary contains <paramref name="name"/></returns>
	public bool ContainsInhabitant(string name) {
		return this.inhabitants.ContainsKey(name);
	}

	/// <summary>
	/// loops through all inhabitants of the room
	/// </summary>
	/// <param name="action">the code executed for each inhabitant</param>
	public void ForEachInhabitant(Action<KeyValuePair<string, dynamic>> action) {
		try {
			foreach (var inhabitant in inhabitants) {
				action.Invoke(inhabitant);
			}
		} catch {}
	}

	/// <summary>
	/// used to get the amount of exits the room has
	/// </summary>
	/// <returns><code>this.exits.Count</code></returns>
	public int GetExitCount() {
		return this.exits.Count;
	}

	/// <summary>
	/// used to add a new spell to a room
	/// </summary>
	/// <param name="spell">the spell that should get added</param>
	public void AddSpell(Spell spell) {
		this.spellBook.TryAdd(spell.Name, spell);
	}

	/// <summary>
	/// removes a spell from the roo,
	/// </summary>
	/// <param name="spellName">The name of the spell</param>
	public void RemoveSpell(string spellName) {
		this.spellBook.Remove(spellName);
	}
	
	/// <summary>
	/// gets a spell in the room
	/// </summary>
	/// <param name="spellName">the name of the spell</param>
	/// <returns>the spell gotten from <paramref name="spellName"/></returns>
	public Spell GetSpell(string spellName) {
		return this.spellBook[spellName];
	}

	/// <summary>
	/// used to get a list of all inhabitants
	/// </summary>
	/// <returns>A list of all inhabitants in the room.</returns>
	public List<Entity> GetInhabitants() {
		List<Entity> returnValue = new List<Entity>();
		ForEachInhabitant((inhabitant) => {
			if (!(inhabitant.Value is Trap)) {
				returnValue.Add(inhabitant.Value);
			}
		});
		return returnValue;
	}

	/// <summary>
	/// used to get a random entity in this room
	/// </summary>
	/// <returns>A random entity in the room</returns>
	public Entity GetRandomInhabitant() {
		Random random = new Random();
		string[] inhabitantNames = this.inhabitants.Keys.ToArray();
		return inhabitants[inhabitantNames[random.Next(0, inhabitantNames.Length)]];
	}

	public Entity GetInhabitant(string name) {
		return inhabitants[name];
	}
}
