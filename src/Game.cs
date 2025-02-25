using System;
using Zuul;

class Game {
	// Private fields
	private Parser parser;
	private Player player;
	private Room StartingRoom;
	private Room winRoom = new Room("");

	// Constructor
	public Game() {
		parser = new Parser();
		player = new Player();
		StartingRoom = null;
		CreateRooms();
	}

	// Initialise the Rooms (and the Items)
	private void CreateRooms() {
		// Create the rooms
		Room outside = new Room("outside the main entrance of the university");
		Room theatre = new Room("in a lecture theatre");
		Room pub = new Room("in the campus pub");
		Room lab = new Room("in a computing lab");
		Room office = new Room("in the computing admin office");
		Room bacement = new Room("in the basement, it is filled with beer fats.");
		Room attic = new Room("in the attic, there are a lot of cobwebs.");

		// Initialise room exits
		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);

		theatre.AddExit("west", outside);

		pub.AddExit("east", outside);
		pub.AddExit("down", bacement);
		pub.AddExit("up", attic);

		lab.AddExit("north", outside);
		lab.AddExit("east", office);

		office.AddExit("west", lab);
		
		bacement.AddExit("up", pub);
		
		attic.AddExit("down", pub);
		
		attic.AddExit("west", winRoom);
		
		// Items
		Item knife = new Item(4, "A sharp pointy object.");
		Item cloack = new Item(2, "A simple cloack, it could be used to bypass sertain obstacles.");
		Item axe = new Item(20, "A shiny axe, it might be usefull later.");
		Item lockOpener = new Item(4, "Can be used to open locks.");
		
		// adding items to the rooms
		bacement.Chest.Put(nameof(axe), axe);
		theatre.Chest.Put(nameof(cloack), cloack);
		office.Chest.Put(nameof(knife), knife);
		pub.Chest.Put(nameof(lockOpener), lockOpener);
		
		// Start game outside
		player.CurrentRoom = outside;
		StartingRoom = outside;
	}

	//  Main play routine. Loops until end of play.
	public void Play() {
		PrintWelcome();

		// Enter the main command loop. Here we repeatedly read commands and
		// execute them until the player wants to quit.
		bool finished = false;
		while (!finished && player.isAlive()) {
			if (player.CurrentRoom == winRoom) {
				finished = AnounceWin();
			}
			Command command = parser.GetCommand();
			finished = ProcessCommand(command);
		}
		if (!player.isAlive()) {
			finished = AnounceDeath();
		}
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}

	// Print out the opening message for the player.
	private void PrintWelcome() {
		Console.WriteLine();
		Console.WriteLine("Welcome to Zuul!");
		Console.WriteLine("Zuul is a new, incredibly boring adventure game.");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}

	/// <summary>
	/// <param name="command">Given a command, process (that is: execute) the command.</param>
	/// <returns>
	/// If this command ends the game, it returns true.
	/// Otherwise false is returned.
	/// </returns>
	/// </summary>
	private bool ProcessCommand(Command command) {
		bool wantToQuit = false;

		if(command.IsUnknown()) {
			Console.WriteLine("I don't know what you mean...");
			return wantToQuit; // false
		}

		switch (command.CommandWord) {
			case "help":
				PrintHelp();
				break;
			case "go":
				GoRoom(command);
				break;
			case "quit":
				wantToQuit = true;
				break;
			case "look":
				Look();
				break;
			case "status":
				Status();
				break;
			case "take":
				Take(command);
				break;
			case "drop":
				Drop(command);
				break;
		}

		return wantToQuit;
	}

	// ######################################
	// implementations of user commands:
	// ######################################
	
	/*
	 * Print out some help information.
	 * Here we print the mission and a list of the command words.
	 */
	private void PrintHelp() {
		Console.WriteLine("You are lost. You are alone.");
		Console.WriteLine("You wander around at the university.");
		Console.WriteLine();
		// let the parser print the commands
		parser.PrintValidCommands();
	}

	// Try to go to one direction. If there is an exit, enter the new
	// room, otherwise print an error message.
	private void GoRoom(Command command) {
		if(!command.HasSecondWord()) {
			// if there is no second word, we don't know where to go...
			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		// Try to go to the next room.
		Room nextRoom = player.CurrentRoom.GetExit(direction);
		if (nextRoom == null) {
			Console.WriteLine($"There is no door to {direction}!");
			return;
		}
		player.CurrentRoom = nextRoom;
		if (!player.CurrentRoom.Equals(winRoom)) {
			Console.WriteLine(player.CurrentRoom.GetLongDescription());
		}
		player.damage(5);
	}

	// gives the description of the current room
	private void Look() {
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}
	
	// shows the players status
	private void Status() {
		Console.WriteLine($"Health: {player.Health}");
		Console.WriteLine($"inventory:\n{player.BackPack.Show()}");
	}
	
	// take an item from a room
	private void Take(Command command) {
		
	}
	
	// put an item into a room
	private void Drop(Command command) {
		
	}
	
	// #########################################################
	// death message
	private bool AnounceDeath() {
		Console.WriteLine("Game Over\n you died by bleeding out.");
		Console.WriteLine("type \"continue\" to continue");
		Console.WriteLine("type \"quit\" to quit");
		return Restart();
	}
	
	// win message
	private bool AnounceWin() {
		Console.WriteLine("You won");
		Console.WriteLine("type \"continue\" to continue");
		Console.WriteLine("type \"quit\" twice to quit");
		return Restart();
	}
	
	// resets game
	private bool Restart() {
		Command command = parser.GetCommand();
		switch (command.CommandWord)
		{
			case "continue":
				player.Health = 100;
				player.CurrentRoom = StartingRoom;
				Play();
				return false;
				break;
			default:
				return ProcessCommand(command);
				break;
		}
	}
}
