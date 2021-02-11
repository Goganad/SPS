using System;
using System.Linq;
using System.Security.Cryptography;

namespace SPS
{
    class Program
    {
        private static bool ComputerWins(int playerMove, int computerMove, int movesCount)
        {
            var difference = playerMove - computerMove;
            return difference < 0 && Math.Abs(difference) <= movesCount / 2 || difference > movesCount / 2;
        }

        private static int GetMove(int movesCount)
        {
            int move;
            while (true)
            {
                Console.Write("Enter your move: ");
                move = Convert.ToInt32(Console.ReadLine());
                if (move < 0 || move > movesCount)
                {
                    Console.WriteLine("No such action available");
                }
                else break;
            }
            return move;
        }
        private static void CreateMenu(string[] moves)
        {
            var movesCount = moves.Length;
            Console.WriteLine("Available moves:");
            for (var i = 0; i < movesCount; i++)
            {
                Console.WriteLine(i+1 + " - " + moves[i]);
            }
            Console.WriteLine("0 - exit");
        }
        private static byte[] GenerateKey()
        {
            var result = new byte[16];
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            rngCryptoServiceProvider.GetBytes(result);
            return result;
        }
        
        private static byte[] GenerateHmac(byte[] key, int computerMove)
        {
            var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(BitConverter.GetBytes(computerMove));
        }
        
        static void Main(string[] args)
        {
            if(args.Length < 3 || args.Length % 2 == 0 || args.Distinct().Count() != args.Length)
            {
                Console.WriteLine("Input odd number of distinct parameters (3 minimum). Example: dotnet ... scissors paper stone");
                return;
            }
            var random = new Random();
            var computerMove = random.Next(args.Length) + 1;
            
            var key = GenerateKey();
            var hmac = GenerateHmac(key, computerMove);
            Console.WriteLine("HMAC: " + BitConverter.ToString(hmac).Replace("-", ""));

            CreateMenu(args);
            
            var playerMove = GetMove(args.Length);

            if (playerMove != 0)
            {
                Console.WriteLine("Your move: " + args[playerMove - 1]);
                Console.WriteLine("Computer move: " + args[computerMove - 1]);

                if (playerMove == computerMove) {
                    Console.WriteLine("It's a tie!");
                } else if (ComputerWins(playerMove, computerMove, args.Length)) {
                    Console.WriteLine("You lose!");
                } else {
                    Console.WriteLine("You win!");
                }
                Console.WriteLine("HMAC key: " + BitConverter.ToString(key).Replace("-",""));
            }
        }
    }
}