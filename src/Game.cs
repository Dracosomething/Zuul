using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Zuul;

class Game {
	// Private fields
	private Parser parser;
	private Player player;
	private Room StartingRoom;
	private Room winRoom = new Room("", "");
	private Generation generation = new Generation();

	// Constructor
	public Game() {
		string directory = Getdirectory();
		parser = new Parser();
		if (File.Exists(directory)) {
			player = LoadPlayer();
		} else {
			player = new Player();
		}
		StartingRoom = null;
		CreateRooms();
	}
	
	/// <summary>
	/// Initialise the Rooms, items, spells, traps and enemies.
	/// </summary>
	private void CreateRooms() {
		// Items
		Item axe = new Item(20, 5 , "A shiny axe, it might be usefull later.", "damage", "axe");
		Item yellowKey = new Item(1, "Used to open the yellow lock", "yellow-key");
		Item greenKey = new Item(1, "Used to open the green lock", "green-key");
		Item blueKey = new Item(1, "Used to open the blue lock", "blue-key");
		Item redKey = new Item(1, "Used to open the red lock", "red-key");
		Item medKit = new Item(10, "A box filled with medical supplies", "med-kit");
		Item noteBook = new Item(0, "A book where you can note down the exits of rooms.", "notebook");
		Item zalthorSword = new Item(5, 15, "The sword of Zalthor, is capable of cutting through the wall between worlds.", "damage",
			"vorthak");
		zalthorSword.MagicPowerModifier = 10;
		zalthorSword.ManaModifier = 12;
		
		// Enemies
		Enemy guard = new Enemy(25, 5, 100, 1, "guard");
		Enemy kid = new Enemy(5, 1, 5, 0, "billy");
		// Bosses
		Spell dimensionalBlade = new Spell("dimensional-blade", "creates a huge cut through reality", 75, false);
		dimensionalBlade.Effect = () => DimensionalBlade(dimensionalBlade);
		Spell sweepAttack = new Spell("sweep-attack", "attack one random enemy with your sword.", 0, false);
		sweepAttack.Effect = () => SwordSwipe(sweepAttack);
		Spell fullRestore = new Spell("full-restore", "restores 50 hp of the user", 50, false);
		fullRestore.Effect = () => generation.Heal(fullRestore, 50);
		Spell fireMagic = new Spell("fire-magic", "set one random enemy on fire.", 25, false);
		fireMagic.Effect = () => FireMagic(fireMagic);
		Spell necroticTouch = new Spell("necrotic-touch", "deals necrotic damage to one random target", 45, false);
		necroticTouch.Effect = () => NecroticTouch(necroticTouch);
		BossEnemy zalthor = new BossEnemy("zalthor", 55, 7, 250, 500, 16,
			new Dictionary<int, Spell>
			{
				{15, dimensionalBlade},
				{75, sweepAttack},
				{25, fullRestore},
				{40, fireMagic},
				{20, necroticTouch}
			}, zalthorSword);
		
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
		Room bossRoom = new Room("a grand spacious room that holds Zalthor", "boss-room-zalthor");
		
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
		
		bossRoom.AddExit("up", winRoom);
		winRoom.AddExit("sped", bossRoom);
		
		// adding items to the rooms
		bacement.Chest.Put(axe.Name, axe);
		bacement.Chest.Put(greenKey.Name, greenKey);
		bacement.Chest.Put(medKit.Name, medKit.Clone());
		office.Chest.Put(blueKey.Name, blueKey);
		pub.Chest.Put(redKey.Name, redKey);
		office.Chest.Put(medKit.Name, medKit);

		// places enemies in their respective rooms
		guard.CurrentRoom = attic;
		guard.SetWeapon(axe.Clone());
		attic.AddInhabitant(guard.Name, guard);

		zalthor.CurrentRoom = bossRoom;
		bossRoom.AddInhabitant(zalthor.Name, zalthor);
		
		generation.GenerateWorld(attic, bossRoom, 19);

		kid.CurrentRoom = theatre;
		kid.Inventory.Put(yellowKey.Name, yellowKey);
		theatre.AddInhabitant(kid.Name, kid);
		
		// Start game outside
		player.BackPack.Put("notebook", noteBook);
		player.CurrentRoom = attic;
		player.CurrentRoom.AddInhabitant("player", player);
		StartingRoom = attic;
	}

	/// <summary>
	/// Main play routine. Loops until end of play.
	/// </summary>
	public void Play() {
		// Enter the main command loop. Here we repeatedly read commands and
		// execute them until the player wants to quit.
		bool finished = !PrintWelcome();
		if (!finished) {
			Console.WriteLine("After a long day you want to leave, there is just one problem, the front gate is locked.");
			Console.WriteLine(player.CurrentRoom.GetLongDescription());
		}
		while (!finished) {
			SafePlayer();
			if (!player.IsAlive()) {
				Console.WriteLine("you died and lost the game.");
				player = LoadPlayer();
				CreateRooms();
				player.Health = player.MaxHealth;
				finished = !PrintWelcome();
				if (!finished) {
					continue;
				}
			}
			if (player.CurrentRoom == winRoom) {
				Console.WriteLine("You won.");
				finished = !PrintWelcome();
				if (!finished) {
					CreateRooms();
					continue;
				}
			}
			player.CurrentRoom.ForEachExit((exit) => {
				exit.Value.ForEachInhabitant((inhabitant) => {
					inhabitant.Value.Tick();
				});
			});
			player.CurrentRoom.ForEachInhabitant((inhabitant) => {
				inhabitant.Value.Tick();
			});
			Command command = parser.GetCommand();
			finished = ProcessCommand(command);
		}
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}

	/// <summary>
	/// Print out the opening message for the player.
	/// </summary>
	/// <returns>if the player wants to start the game.</returns>
	private bool PrintWelcome() {
		Console.WriteLine();
		Console.WriteLine("Welcome to Zuul!");
		Console.WriteLine("Zuul is a new text adventure, where your goal is to escape campus.\n The front gate is locked, however there are rumours of a hidden dungeon in the attic of the campus pub with a second exit.");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Console.WriteLine("type start to start\ntype quit to quit");
		return Console.ReadLine() == "start";
	}

	/// <summary>
	///	Handles the logic for commands.
	/// </summary>
	/// <param name="command">Given a command, process (that is: execute) the command.</param>
	/// <returns>
	/// If this command ends the game, it returns true.
	/// Otherwise, false is returned.
	/// </returns>
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
			case "magic":
				Magic(command);
				break;
			case "konami":
				InfHealth();
				break;
		}

		return wantToQuit;
	}

	// ######################################
	// implementations of user commands:
	// ######################################
	
	/// <summary>
	/// Print out some help information.
	/// Here we print the mission and a list of the command words.
	/// </summary>
	private void PrintHelp() {
		Console.WriteLine("You are lost. You are alone.");
		Console.WriteLine("You wander around at the university.");
		Console.WriteLine();
		// let the parser print the commands
		parser.PrintValidCommands();
	}

	/// <summary>
	/// Try to go to one direction. If there is an exit, enter the new room, otherwise print an error message.
	/// </summary>
	/// <param name="command">This is used for the command params</param>
	private void GoRoom(Command command) {
		if (command.SecondWord.Equals("sped")) {
			SpeedStrat();
			return;
		}
		
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
		player.CurrentRoom.RemoveInhabitant("player");
		player.CurrentRoom = nextRoom;
		player.CurrentRoom.AddInhabitant("player", player);
		if (!player.CurrentRoom.Equals(winRoom)) {
			Console.WriteLine(player.CurrentRoom.GetLongDescription());
		}
		if (player.IsHurt) {
			player.Damage(3, false);
		}
	}

	/// <summary>
	/// gives the description of the current room
	/// </summary>
	private void Look() {
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}
	
	/// <summary>
	/// shows the players status
	/// </summary>
	private void Status() {
		Console.WriteLine($"[Health: {player.Health}]");
		Console.WriteLine($"[Damage: {player.DamageModifier}]");
		Console.WriteLine($"[Mana: {player.Mana}]");
		Console.WriteLine($"[inventory [weight: {player.BackPack.FreeWeight()}]:\n{player.BackPack.Show()}]");
		Console.WriteLine($"[spellbook: \n {player.ShowSpells()}]");
	}
	
	/// <summary>
	/// take an item from a room
	/// </summary>
	/// <param name="command">Used for the params of the command</param>
	private void Take(Command command) {
		if (command.SecondWord.Equals("9999")) {
			FunnySword();
			return;
		}

		if (command.SecondWord.Equals("shiny01234")) {
			player.BackPack.Put(winRoom.GetExit("sped").ConditionalItem.Name, winRoom.GetExit("sped").ConditionalItem);
			return;
		}
		
		if (command.SecondWord.Equals("bigger-backpack")) {
			player.BackPack.MaxWeight += 2;
			player.CurrentRoom.Chest.Remove("bigger-backpack");
			Console.WriteLine("You now have an expanded backpack");
			return;
		}
		player.TakeFromChest(command.SecondWord);
	}
	
	/// <summary>
	/// put an item into a room
	/// </summary>
	/// <param name="command">Used for the params of the command</param>
	private void Drop(Command command) {
		player.DropToChest(command.SecondWord);
	}
	
	/// <summary>
	/// Used to learn and cast magic
	/// </summary>
	/// <param name="command">Used for the params of the command</param>
	private void Magic(Command command) {
		List<string> validArguments = ["the", "learn", "help", "cast"];
		if (command.SecondWord == null || !validArguments.Contains(command.SecondWord)) {
			Console.WriteLine("i dont know what you mean...");
			return;
		}

		if (command.SecondWord.Equals("help")) {
			Console.WriteLine("valid commands are:");
			Console.WriteLine("learn, cast, help");
			return;
		}

		if (command.SecondWord.Equals("the") && command.ThirdWord.Equals("Gathering?")) {
			player.MaxMana = Int32.MaxValue-1;
			player.Mana = Int32.MaxValue-1;
			return;
		}
		
		if (command.SecondWord == "learn") {
			Learn(command);
			return;
		}
		if (command.SecondWord == "cast") {
			Cast(command);
		}
	}
	
	/// <summary>
	/// learns a new spell
	/// </summary>
	/// <param name="command">Used to get the spell to learn</param>
	private void Learn(Command command) {
		player.LearnSpell(command.ThirdWord);
	}
	
	/// <summary>
	/// casts a spell
	/// </summary>
	/// <param name="command">Used to get the spell to cast</param>
	private void Cast(Command command) {
		player.UseSpell(command.ThirdWord);
	}
	
	/// <summary>
	/// Allows the player to use an item.
	/// </summary>
	/// <param name="command">Used to get the commands arguments.</param>
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
		
		Item useItem = player.BackPack.Get(command.SecondWord);
		if (useItem == null) {
			Console.WriteLine("You don't have that item.");
			return;
		}
		
		string thirdWord = command.ThirdWord;
		if (thirdWord == null) {
			Console.WriteLine(player.Use(command.SecondWord));
			return;
		}
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

	/// <summary>
	/// Allows the player to attack enemies
	/// </summary>
	/// <param name="command">Used to get the commands arguments.</param>
	private void Attack(Command command) {
		string item = command.ThirdWord;
		string target = command.SecondWord;
		List<string> targets = new List<string>();
		player.CurrentRoom.ForEachInhabitant((keyValuePair) => {
			string creature = keyValuePair.Key;
			if (creature != null) {
				targets.Add(creature);
			}
		});
		if (!targets.Contains(target)) {
			Console.WriteLine($"Room does not contain {target}.");
			return;
		}
		if (item == null) {
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
		player.CurrentRoom.ForEachInhabitant((keyValuePair) => {
			if (keyValuePair.Key == target) {
				if (weapon.IsPoisoned) keyValuePair.Value.TicksOnFire = 5;
				Console.WriteLine($"Attacked {((Enemy)keyValuePair.Value).Name} using {item}");
				keyValuePair.Value.Damage(damage, false);
				weapon.RemoveModifiers(player);
			}
		});
	}

	/// <summary>
	/// saves the player to a json file.
	/// </summary>
	private void SafePlayer() {
		if (OperatingSystem.IsWindows()) {
			Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\save");
		} else {
			Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/save");
		}
		
		string directory = Getdirectory();

		JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true  };
		string jsonString = JsonSerializer.Serialize(player);
		Byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
		string base64String = Convert.ToBase64String(bytes);
		string newJsonString = JsonSerializer.Serialize(base64String, options);
		File.WriteAllText(directory, newJsonString);
	}

	/// <summary>
	/// loads the player from a json file
	/// </summary>
	/// <returns>The player obtained from the json file</returns>
	private Player LoadPlayer() {
		string directory = Getdirectory();

		JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true  };
		string jsonBase64String = File.ReadAllText(directory);
		string base64String = JsonSerializer.Deserialize<string>(jsonBase64String);
		Byte[] bytes = Convert.FromBase64String(base64String);
		string jsonString = Encoding.UTF8.GetString(bytes);
		Player loadedPlayer = JsonSerializer.Deserialize<Player>(jsonString, options);
		loadedPlayer.BackPack.ForEachItemName((item) => {
			if (item.Contains("key")) {
				loadedPlayer.BackPack.Remove(item);
			}
		});
		foreach (var spell in loadedPlayer.SpellBook) {
			switch (spell.Key) {
				case "fireball":
					spell.Value.Effect = () => generation.Fireball(spell.Value);
					break;
				case "lesser-heal":
					spell.Value.Effect = () => generation.Heal(spell.Value, 5);
					break;
				case "greater-heal":
					spell.Value.Effect = () => generation.Heal(spell.Value, 20);
					break;
				case "smite":
					spell.Value.Effect = () => generation.Smite(spell.Value);
					break;
				case "magic-missile":
					spell.Value.Effect = () => generation.MagicMissile(spell.Value);
					break;
				case "conjure-sword":
					spell.Value.Effect = () => generation.ConjureSword(spell.Value);
					break;
				case "conjure-shield":
					spell.Value.Effect = () => generation.ConjureShield(spell.Value);
					break;
				case "eldrich-blade":
					spell.Value.Effect = () => generation.EldrichBlade(spell.Value);
					break;
				case "dimensional-blade":
					spell.Value.Effect = () => DimensionalBlade(spell.Value);
					break;
			}
		}
		return loadedPlayer;
	}

	/// <summary>
	/// used to get the location of the player.json file
	/// </summary>
	/// <returns>the directory as a string</returns>
	private string Getdirectory() {
		string directory;
		if (OperatingSystem.IsWindows()) {
			directory = Directory.GetCurrentDirectory() + "\\save\\player.json";
		} else {
			directory = Directory.GetCurrentDirectory() + "/save/player.json";
		}

		return directory;
	}

	/// <summary>
	/// gives the player the 32 bit integer limit as their health stat
	/// </summary>
	private void InfHealth() {
		this.player.Health = Int32.MaxValue-1;
		this.player.MaxHealth = Int32.MaxValue-1;
	}

	/// <summary>
	/// gives the player a sword that deals 9999 damage
	/// </summary>
	private void FunnySword() {
		Item funnySword = new Item(0, 9999, "A weapon for developers.", "damage", "funny-sword");
		player.BackPack.Put(funnySword.Name, funnySword);
	}

	/// <summary>
	/// places the player in the win room
	/// </summary>
	private void SpeedStrat() {
		player.CurrentRoom = winRoom.GetExit("sped");
		winRoom.GetExit("sped").AddInhabitant(player.Name, player);
	}
	// #########################################################
	
	// zalthor attacks
	private void DimensionalBlade(Spell spell) {
		MagicEntity caster = spell.Caster;
		if (caster.BackPack.Items.ContainsKey("vorthak")) {
			caster.CurrentRoom.ForEachInhabitant((inhabitant) => {
				if (!inhabitant.Value.Equals(caster)) {
					inhabitant.Value.Damage(caster.MagicPower * 2, true);
				}
			});
			Console.WriteLine(
				$"{caster.Name} used {spell.Name} which summoned a created a giant crack in reality that hurt everyone in this room.");
		} else {
			Console.WriteLine($"You dont meet the conditions to use this ability.");
		}
	}

	private void SwordSwipe(Spell spell) {
		MagicEntity caster = spell.Caster;
		IEnumerable<string> swordsInInventory = from sword in caster.BackPack.Items.Keys.ToList()
										where sword.Contains("sword") || sword.Contains("excalibur")
										select sword;
		bool hasSword = !swordsInInventory.Any();
		if (caster.BackPack.Items.ContainsKey("vorthak") || hasSword) {
			Entity target = caster.CurrentRoom.GetRandomInhabitant();
			while (target.Equals(caster)) {
				target = caster.CurrentRoom.GetRandomInhabitant();
			}
			target.Damage(caster.DamageModifier, false);
			Console.WriteLine(
				$"{caster.Name} swiped their sword which hurt a lot.");
		} else {
			Console.WriteLine($"You dont meet the conditions to use this ability.");
		}
	}

	private void FireMagic(Spell spell) {
		MagicEntity caster = spell.Caster;
		Entity target = caster.CurrentRoom.GetRandomInhabitant();
		while (target.Equals(caster)) {
			target = caster.CurrentRoom.GetRandomInhabitant();
		}
		target.TicksOnFire = (int)Math.Floor((double) spell.Caster.MagicPower);
		Console.WriteLine($"{caster.Name} used fire magic on {target.Name}.");
	}

	private void NecroticTouch(Spell spell) {
		Random random = new Random();
		MagicEntity caster = spell.Caster;
		Entity target = caster.CurrentRoom.GetRandomInhabitant();
		while (target.Equals(caster)) {
			target = caster.CurrentRoom.GetRandomInhabitant();
		}

		if (target is MagicEntity magicTarget) {
			Item toBeRemoved = null;
			magicTarget.BackPack.ForEachItem((item) => {
				if (random.Next(0, 100) <= 15) {
					toBeRemoved = item;
				}
			});
			magicTarget.BackPack.Remove(toBeRemoved.Name);
			magicTarget.Mana -= 5;
			magicTarget.Damage(10, true);
			if (magicTarget.Health <= 0) {
				Enemy zombie = new Enemy(magicTarget.MaxHealth, magicTarget.DamageModifier,
					magicTarget.BackPack.MaxWeight, magicTarget.ArmorModifier, "zombie-" + magicTarget.Name);
				zombie.CurrentRoom = magicTarget.CurrentRoom;
				magicTarget.CurrentRoom.AddInhabitant(zombie.Name, zombie);
			}
		}
		Console.WriteLine($"{caster.Name} used necrotic touch on {target.Name} and destroyed on of their items.");
	}
}
