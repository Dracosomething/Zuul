using System.Collections.Generic;

class CommandLibrary {
	// fields
	// A List that holds all valid command words
	private readonly List<string> validCommands;

	// Constructor - initialise the command words.
	public CommandLibrary() {
		validCommands = new List<string>();

		validCommands.Add("help");
		validCommands.Add("go");
		validCommands.Add("quit");
		validCommands.Add("look");
		validCommands.Add("status");
		validCommands.Add("take");
		validCommands.Add("drop");
		validCommands.Add("use");
		validCommands.Add("attack");
		validCommands.Add("magic");
	}
	
	/// <summary>
	/// Check whether a given string is a valid command word.
	/// </summary>
	/// <param name="instring"></param>
	/// <returns>true if it is, false if it isn't.</returns>
	public bool IsValidCommandWord(string instring) {
		return validCommands.Contains(instring);
	}

	/// <summary>
	/// used to get all the usable commands
	/// </summary>
	/// <returns>a list of valid command words as a comma separated string.</returns>
	public string GetCommandsString() {
		return String.Join(", ", validCommands);
	}
}
