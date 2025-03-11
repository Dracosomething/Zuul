class Command {
	// properties
	public string CommandWord { get; init; }
	public string SecondWord { get; init; }
	public string ThirdWord { get; init; }
	
	/*
	 Create a command object. First and second word must be supplied, but
	 either one (or both) can be null. See Parser.GetCommand() 
	*/
	public Command(string first, string second, string third) {
		CommandWord = first;
		SecondWord = second;
		ThirdWord = third;
	}

	/// <summary>
	/// Checks if the command does not exist
	/// </summary>
	/// <returns> if the first word of the command is null</returns>
	public bool IsUnknown() {
		return CommandWord == null;
	}

	/// <summary>
	/// checks if the command has a second word
	/// </summary>
	/// <returns>if the command has a second word</returns>
	public bool HasSecondWord() {
		return SecondWord != null;
	}

	/// <summary>
	/// checks if the command has a third word
	/// </summary>
	/// <returns>if the command has a third word</returns>
	public bool HasThirdWord() {
		return ThirdWord != null;
	}
}
