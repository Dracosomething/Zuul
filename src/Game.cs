using System;
using Zuul;

class Game {
	// Private fields
	private Parser parser;
	private Player player;
	private Room StartingRoom;
	private Room winRoom = new Room("", "");
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
		Item axe = new Item(20, 5 , "A shiny axe, it might be usefull later.", "damage", "axe");
		Item yellowKey = new Item(1, "Used to open the yellow lock", "yellow-key");
		Item greenKey = new Item(1, "Used to open the green lock", "green-key");
		Item blueKey = new Item(1, "Used to open the blue lock", "blue-key");
		Item redKey = new Item(1, "Used to open the red lock", "red-key");
		Item medKit = new Item(10, "A box filled with medical supplies", "med-kit");
		Item noteBook = new Item(0, "A book where you can note down the exits of rooms.", "notebook");
		
		// Enemies
		Enemy guard = new Enemy(50, 10, 100, 1, "Guard");
		Enemy kid = new Enemy(5, 1, 5, 1, "billy");
		
		// Create the rooms
		Room outside = new Room("outside the main entrance of the university", "outside");
		Room theatre = new Room("in a lecture theatre", "theatre");
		Room pub = new Room("in the campus pub", "pub", yellowKey);
		Room lab = new Room("in a computing lab", "lab");
		Room office = new Room("in the computing admin office", "office", redKey);
		Room bacement = new Room("in the basement, it is filled with beer fats.", "bacement", blueKey);
		Room attic = new Room("in the attic, there are a lot of cobwebs.", "attic", greenKey);
		Room pubStairMid = new Room("in a room with a staircase.", "pub-stairs-middle");
		Room pubStairTip = new Room("at the top of the staircase", "pub-stairs-top");
		Room pubStairBottom = new Room("at the bottom of the staircase", "pub-stairs-bottom");
		
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
		bacement.Chest.Put(axe.Name, axe);
		bacement.Chest.Put(greenKey.Name, greenKey);
		office.Chest.Put(blueKey.Name, blueKey);
		pub.Chest.Put(redKey.Name, redKey);
		office.Chest.Put(medKit.Name, medKit);

		guard.CurrentRoom = attic;
		guard.SetWeapon(axe.Clone());
		attic.AddInhabitant(guard.Name, guard);

		Generartion generartion = new Generartion();
		generartion.GenerateWorld(attic, winRoom, 47);

		kid.CurrentRoom = theatre;
		kid.Inventory.Put(yellowKey.Name, yellowKey);
		theatre.AddInhabitant(kid.Name, kid);
		
		// Start game outside
		player.BackPack.Put("notebook", noteBook);
		player.CurrentRoom = outside;
		player.CurrentRoom.AddInhabitant("player", player);
		StartingRoom = outside;
	}

	//  Main play routine. Loops until end of play.
	public void Play() {
		// Enter the main command loop. Here we repeatedly read commands and
		// execute them until the player wants to quit.
		bool finished = !PrintWelcome();
		if (!finished) {
			Console.WriteLine(player.CurrentRoom.GetLongDescription());
		}
		while (!finished) {
			if (!player.IsAlive()) {
				Console.WriteLine("you died and lost the game.");
				finished = !PrintWelcome();
			}
			if (player.CurrentRoom == winRoom) {
				Console.WriteLine("You won.");
				finished = !PrintWelcome();
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
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}

	// Print out the opening message for the player.
	private bool PrintWelcome() {
		Console.WriteLine();
		Console.WriteLine("Welcome to Zuul!");
		Console.WriteLine("Zuul is a new, incredibly boring adventure game.");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Console.WriteLine("type start to start\ntype quit to quit");
		return Console.ReadLine() == "start";
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
				Console.WriteLine($"room needs {nextRoom.ConditionalItem.Name} to be unlocked");
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
			player.Damage(5);
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
		if (command.SecondWord.ToLower().Equals("equip")) {
			Item item = player.BackPack.Get(command.ThirdWord);
			if (item == null) {
				Console.WriteLine("You don't have that item.");
				return;
			}
			
			item.ApplyModifiers(player);
			item.Equiped = true;
			return;
		}
		if (command.SecondWord.ToLower().Equals("unequip")) {
			Item item = player.BackPack.Get(command.ThirdWord);
			if (item == null) {
				Console.WriteLine("You don't have that item.");
				return;
			}
			
			item.RemoveModifiers(player);
			item.Equiped = false;
			return;
		}
		
		Item useItem = player.BackPack.Get(command.SecondWord);
		if (useItem == null) {
			Console.WriteLine("You don't have that item.");
			return;
		}
		if (command.SecondWord.Equals("med-kit")) {
			player.Heal(20);
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
			if (useCase == null && useCase != "write" && useCase != "read") {
				Console.WriteLine("I dont understand how to use that.");
				Console.WriteLine("valid uses are: write, read");
				return;
			}
			if (useCase.ToLower().Equals("write")) {
				player.NoteDown(player.CurrentRoom);
				Console.WriteLine("successfully wrote down the rooms exits");
			}
			if (useCase.ToLower().Equals("read")) {
				Console.WriteLine(player.Read());
			}
			return;
		}
		if (command.SecondWord.Equals("food-plate")) {
			player.Heal(10);
			Console.WriteLine("You ate the food and feel rejuvenated");
			player.BackPack.Remove(command.SecondWord);
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
			Console.WriteLine("This room needs nothing to be opened");
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
			Console.WriteLine("You dont have that weapon.");
			return;
		}

		Item weapon = player.BackPack.Get(item);
		if (weapon == null && item != "fists") {
			Console.WriteLine("You dont have that weapon.");
			return;
		} 
		if (item != "fists") {
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
}
