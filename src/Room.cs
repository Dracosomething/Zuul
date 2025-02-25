using System.Collections.Generic;
using Zuul;

class Room {
	// Private fields
	private string description;
	private Dictionary<string, Room> exits; // stores exits of this room.
	private Inventory chest;

	// properties
	public Inventory Chest { get { return this.chest; } }
	
	/*
	 * Create a room described "description". Initially, it has no exits.
	 * "description" is something like "in a kitchen" or "in a court yard".
	 */
	public Room(string desc) {
		description = desc;
		exits = new Dictionary<string, Room>();
		chest = new Inventory(Int32.MaxValue-1);
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
		str += description;
		str += ".\n";
		str += GetExitString();
		str += ".\n";
		str += "The room contains ";
		str += this.Chest.Show();
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
}
