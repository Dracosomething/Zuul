using System.Text.Json;
using System.Text;
class Program {
	public static void Main(string[] args) {
		// Create and play the Game.
		Console.WriteLine("======================================");
		Console.WriteLine("===              Zuul              ===");
		Console.WriteLine("======================================");
		Console.WriteLine();
		Console.WriteLine("start game?");
		Console.WriteLine("yes/no");
		string input = Console.ReadLine();
		Console.Clear();
		
		if (input != null && input.ToLower() == "yes") {
			Console.WriteLine("start new game?");
			Console.WriteLine("yes/no");
			input = Console.ReadLine();
			Console.Clear();
			if (input != null && input == "yes") {
				Console.Write("seed: ");
				input = Console.ReadLine();
				Random random = new Random();
				int seed = random.Next();
				if (!String.IsNullOrEmpty(input) && !String.IsNullOrWhiteSpace(input)) {
					seed = Convert.ToInt32(input);
				}
				Console.Clear();
				Game game = new Game(seed);
				game.Play();
			} else {
				string directory = Getdirectory();
				string jsonBase64String = File.ReadAllText(directory);
				string base64String = JsonSerializer.Deserialize<string>(jsonBase64String);
				Byte[] bytes = Convert.FromBase64String(base64String);
				string jsonString = Encoding.UTF8.GetString(bytes);
				int seed = JsonDocument.Parse(jsonString).RootElement.GetProperty("Seed").GetInt32();
				Console.WriteLine(seed);
				Game game = new Game(seed);
				game.Play();
			}
		}
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}
	
	/// <summary>
	/// used to get the location of the player.json file
	/// </summary>
	/// <returns>the directory as a string</returns>
	private static string Getdirectory() {
		string directory;
		if (OperatingSystem.IsWindows()) {
			directory = Directory.GetCurrentDirectory() + "\\save\\world.json";
		} else {
			directory = Directory.GetCurrentDirectory() + "/save/world.json";
		}

		return directory;
	}
}
