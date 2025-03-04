using System;
using Zuul;

class Game {
	// Private fields
	private Parser parser;
	private Player player;
	private Room StartingRoom;
	private Room winRoom = new Room("");
	private bool isHurt;

	// Constructor
	public Game() {
		parser = new Parser();
		player = new Player();
		StartingRoom = null;
		isHurt = true;
		CreateRooms();
	}

	// Initialise the Rooms (and the Items)
	private void CreateRooms() {
		// Items
		Item knife = new Item(4, 2, "A sharp pointy object.", "damage");
		Item cloack = new Item(2, 3, "A simple cloack, it could be used to bypass sertain obstacles.", "speed");
		Item axe = new Item(20, -1, 5, "A shiny axe, it might be usefull later.");
		Item lockOpener = new Item(4, "Can be used to open locks.");
		Item medKit = new Item(10, "A box filled with medical supplies");
		Item noteBook = new Item(0, "A book where you can note down the exits of rooms.");
		
		// Enemies
		Enemy guard = new Enemy(50, 10, 100, 1, "Guard");
		Enemy kid = new Enemy(5, 1, 5, 1, "billy");
		
		// Create the rooms
		Room outside = new Room("outside the main entrance of the university");
		Room theatre = new Room("in a lecture theatre");
		Room pub = new Room("in the campus pub", cloack);
		Room lab = new Room("in a computing lab");
		Room office = new Room("in the computing admin office", lockOpener);
		Room bacement = new Room("in the basement, it is filled with beer fats.", knife);
		Room attic = new Room("in the attic, there are a lot of cobwebs.", axe);
		Room pubStairMid = new Room("in a room with a staircase.");
		Room pubStairTip = new Room("at the top of the staircase");
		Room pubStairBottom = new Room("at the bottom of the staircase");
		
		// Initialise room exits
		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);

		theatre.AddExit("west", outside);

		pub.AddExit("east", outside);
		pub.AddExit("west", pubStairMid);
		
		pubStairMid.AddExit("up", pubStairTip);
		pubStairMid.AddExit("down", pubStairBottom);
		
		pubStairBottom.AddExit("east", bacement);
		pubStairBottom.AddExit("up", pubStairMid);
		
		pubStairTip.AddExit("east", attic);
		pubStairTip.AddExit("down", pubStairMid);
		
		lab.AddExit("north", outside);
		lab.AddExit("east", office);

		office.AddExit("west", lab);
		
		bacement.AddExit("west", pubStairBottom);
		
		attic.AddExit("west", pubStairTip);
		// attic.AddExit("west", winRoom);
		
		// adding items to the rooms
		bacement.Chest.Put(nameof(axe), axe);
		bacement.Chest.Put(nameof(knife), knife.Clone());
		office.Chest.Put(nameof(knife), knife);
		pub.Chest.Put(nameof(lockOpener), lockOpener);
		office.Chest.Put(nameof(medKit), medKit);

		guard.CurrentRoom = attic;
		guard.SetWeapon(axe.Clone());
		attic.AddInhabitant(guard.Name, guard);

		Generartion generartion = new Generartion();
		generartion.GenerateWorld(attic, winRoom);

		kid.CurrentRoom = theatre;
		kid.Inventory.Put(nameof(cloack), cloack);
		theatre.AddInhabitant(kid.Name, kid);
		
		// Start game outside
		player.BackPack.Put("notebook", noteBook);
		player.CurrentRoom = attic;
		player.CurrentRoom.AddInhabitant("player", player);
		StartingRoom = attic;
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
			if (player.CurrentRoom.Inhabitants != null) {
				foreach (var currentRoomInhabitant in player.CurrentRoom.Inhabitants) {
					if (currentRoomInhabitant.Value is Enemy entity) {
						entity.Tick();
					}
				}
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
			case "use":
				Use(command);
				break;
			case "attack":
				Attack(command);
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

		if (nextRoom.ConditionalItem != null) {
			if (!nextRoom.IsUnlocked) {
				Console.WriteLine("room is blocked");
				return;
			}
		}
		player.CurrentRoom.Inhabitants.Remove("player");
		player.CurrentRoom = nextRoom;
		player.CurrentRoom.AddInhabitant("player", player);
		if (!player.CurrentRoom.Equals(winRoom)) {
			Console.WriteLine(player.CurrentRoom.GetLongDescription());
		}
		if (isHurt) {
			player.damage(5);
		}
	}

	// gives the description of the current room
	private void Look() {
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}
	
	// shows the players status
	private void Status() {
		Console.WriteLine($"[Health: {player.Health}]");
		Console.WriteLine($"[Damage: {player.DamageModifier}]");
		Console.WriteLine($"[inventory [weight: {player.BackPack.FreeWeight()}]:\n{player.BackPack.Show()}]");
	}
	
	// take an item from a room
	private void Take(Command command) {
		player.TakeFromChest(command.SecondWord);
	}
	
	// put an item into a room
	private void Drop(Command command) {
		player.DropToChest(command.SecondWord);
	}
	
	// use an item
	private void Use(Command command) {
		Item useItem = player.BackPack.Get(command.SecondWord);
		if (useItem == null) {
			Console.WriteLine("You don't have that item.");
			return;
		}
		if (command.SecondWord.Equals("medKit")) {
			player.heal(20);
			Console.WriteLine("used med kit and healed 20 hp");
			player.BackPack.Remove(command.SecondWord);
			return;
		}
		if (command.SecondWord.Equals("stiches")) {
			isHurt = false;
			Console.WriteLine("used stiches and closed your wounds");
			player.BackPack.Remove(command.SecondWord);
			return;
		}
		if (command.SecondWord.Equals("notebook")) {
			string useCase = command.ThirdWord;
			if (useCase == null) {
				Console.WriteLine("I dont understand how to use that.");
				Console.WriteLine("valid uses are: write, look");
				return;
			}
			if (useCase.ToLower().Equals("write")) {
				player.NoteDown(player.CurrentRoom);
				Console.WriteLine("successfully wrote down the rooms exits");
			}
			if (useCase.ToLower().Equals("look")) {
				Console.WriteLine(player.Read());
			}
			return;
		}
		
		string thirdWord = command.ThirdWord;
		Room nextRoom = player.CurrentRoom.GetExit(thirdWord);
		if (nextRoom == null) {
			Console.WriteLine($"There is no door to {thirdWord}!");
			return;
		}
		Item condition = nextRoom.ConditionalItem;
		if (condition == null) {
			Console.WriteLine("The room is not blocked.");
			return;
		}
		if (condition == useItem) {
			Console.WriteLine(player.Use(command.SecondWord));
			nextRoom.IsUnlocked = true;
			nextRoom.ConditionalItem = null;
			player.BackPack.Remove(command.SecondWord);
		}
	}

	private void Attack(Command command) {
		string item = command.ThirdWord;
		string target = command.SecondWord;
		List<string> targets = new List<string>();
		foreach (var keyValuePair in player.CurrentRoom.Inhabitants) {
			string creature = keyValuePair.Key;
			targets.Add(creature);
		}
		if (!targets.Contains(target)) {
			Console.WriteLine($"Room does not contain {target}.");
			return;
		}
		if (item == null || !item.Equals("fists")) {
			Console.WriteLine("I don't recognize that item.");
			return;
		}

		Item weapon = player.BackPack.Get(item);
		if (weapon == null && item != "fists") {
			Console.WriteLine("You dont have that weapon");
			return;
		} else if (item != "fists") {
			weapon.ApplyModifiers(player);
		}
		int damage = player.DamageModifier;
		foreach (var keyValuePair in player.CurrentRoom.Inhabitants) {
			if (keyValuePair.Key == target) {
				Console.WriteLine($"Attacked {((Enemy)keyValuePair.Value).Name} using {item}");
				((Enemy)keyValuePair.Value).Damage(damage);
			}
		}
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
