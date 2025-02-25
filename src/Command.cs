class Command {
	public string CommandWord { get; init; }
	public string SecondWord { get; init; }
	
	/*
	 Create a command object. First and second word must be supplied, but
	 either one (or both) can be null. See Parser.GetCommand() 
	*/
	public Command(string first, string second) {
		CommandWord = first;
		SecondWord = second;
	}

	
	/// <returns> true if this command was not understood.</returns>
	public bool IsUnknown() {
		return CommandWord == null;
	}

	
	/// <returns> Return true if the command has a second word. </returns>
	public bool HasSecondWord() {
		return SecondWord != null;
	}
}
