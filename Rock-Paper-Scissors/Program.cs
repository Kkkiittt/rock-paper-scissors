using System.Security.Cryptography;

public class Program
{
	private static string _key;
	private static string _move;
	private static string _hmac;
	private static int length;
	public static void Main(string[] args)
	{
		CheckArguments(args);
	}
	public static void CheckArguments(string[] args)
	{
		if(args.Length < 3 || args.Length % 2 == 0)
			throw new ArgumentException("Enter odd number of arguments (but more at least 3)\n");
		Dictionary<string, int> occurences = new Dictionary<string, int>();
		foreach(string arg in args)
			occurences[arg]++;
		if(occurences.Any(x => x.Value > 1))
			throw new ArgumentException("You have repeated some arguments\n");
		return;
	}
	public static void MakeMove(string[] args)
	{
		_key = GenerateKey()
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
}