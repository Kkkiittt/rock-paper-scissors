using System.Security.Cryptography;
using System.Text;

using ConsoleTables;

public class Program
{
	private static string[] _messages = { "You lose!", "Draw.", "You win!" };
	private static byte[] _key;
	private static int _move;
	private static string _hmac;
	private static string[] _args;
	private static int _userMove;
	private const int _length = 32;
	public static void Main(string[] args)
	{
		try
		{
			Game(args);
		}
		catch(Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}
	public static void Game(string[] args)
	{
		CheckArguments(args);
		MakeMove();
		Console.WriteLine("HMAC-Hash: " + _hmac);
		AskMove();
	}
	public static void CheckArguments(string[] args)
	{
		if(args.Length < 3 || args.Length % 2 == 0)
			throw new ArgumentException("Enter odd number of arguments (but at least 3)\n");
		List<string> keys = new List<string>();
		foreach(string arg in args)
			if(keys.Contains(arg))
				throw new ArgumentException("You have repeated some move");
			else
				keys.Add(arg);
		_args = args;
		return;
	}
	public static void AskMove()
	{
		for(int i = 0; i < _args.Length; i++)
		{
			Console.WriteLine($"{i + 1} - {_args[i]}");
		}
		Console.WriteLine("0 - Quit\n? - Help\nYour move:");
		string s = Console.ReadLine();
		if(int.TryParse(s, out _userMove))
			CheckResult();
		else if(s == "?")
			Help();
		else
			AskMove();
	}
	public static void CheckResult()
	{
		_userMove--;
		if(_userMove < 0)
			return;
		Console.WriteLine("Your move: " + _args[_userMove]);
		Console.WriteLine("PC's move: " + _args[_move]);
		Console.WriteLine(_messages[(int)Wins(_userMove, _move) + 1]);
		Console.WriteLine($"HMAC-Key: {ArrayToString(_key)}");
	}
	public static int Normalize(int value, int by, int max)
	{
		return (value - by + max) % max;
	}
	public static Win Wins(int user, int pc)
	{
		return user == pc ? Win.Draw : Normalize(pc, user, _args.Length) <= _args.Length / 2 ? Win.Loses : Win.Wins;
	}
	public static void Help()
	{
		var table = new ConsoleTable(new string[] { "PC's Move->" }.Concat(_args).ToArray());
		foreach(var move in _args)
		{
			table.AddRow(new object[] { move }.Concat(_args.Select(x => Wins(Array.IndexOf(_args, move), Array.IndexOf(_args, x)).ToString())).ToArray());
		}
		table.Options.EnableCount = false;
		Console.WriteLine(table);
		AskMove();
	}
	public static void MakeMove()
	{
		_key = GenerateKey(_length);
		_move = (new Random()).Next(0, _args.Length);
		_hmac = ArrayToString((new HMACSHA256(_key)).ComputeHash(Encoding.UTF8.GetBytes(_args[_move])));
	}
	public static byte[] GenerateKey(int length)
	{
		byte[] key = new byte[length];
		using(var rng = RandomNumberGenerator.Create())
		{
			rng.GetBytes(key);
		}
		return key;
	}
	public static string ArrayToString(byte[] bytes)
	{
		return BitConverter.ToString(bytes).Replace("-", "").ToLower();
	}
}
public enum Win
{
	Wins = 1,
	Loses = -1,
	Draw = 0,
}