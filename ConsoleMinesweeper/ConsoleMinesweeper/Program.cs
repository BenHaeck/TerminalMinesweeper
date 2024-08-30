// See https://aka.ms/new-console-template for more information
using System;
using System.Security.Cryptography.X509Certificates;
using ConsoleMinesweeper;

public static class Program {
	public static void Main () {

		while (true) {
			switch (Menu ()) {
				case 'P':
					Play (20, 18, 50);
					break;
				case 'E':
					return;
			}
		}
	}

	public static char Menu () {
		int menuPos = 0;
		var options = new string[]{ "Play", "Exit" };
		while (true) {
			Console.Clear ();
			Console.WriteLine ("Minesweeper\n(A&D to move curser, Space to select)");
			for (int i = 0; i < options.Length; i++) {
				bool selected = menuPos == i;
				Console.Write ((selected ? ">" : "")+ options[i]+(selected ? '<' : ' '));
			}
			switch (Console.ReadKey ().KeyChar) {
				case 'a':
					menuPos--;
					break;

				case 'd':
					menuPos++;
					break;
				case ' ':
					return options[menuPos][0];
			}
			menuPos = Math.Min (Math.Max (0, menuPos), options.Length-1);
		}
		
	}

	public static void Play (int boardWidth, int boardHeight, int numBombs) {
		int curserX = 0, curserY = 0;
		var board = new Board (boardWidth, boardHeight, numBombs);
		bool isFirstDig = true;
		char input = ' ';
		while (input != 'e') {
			Console.Clear ();
			Console.WriteLine ("Controls: WASD to move curser, space to uncover a tile, and F to plant a flag");
			board.PrintBoard ((curserX, curserY));
			input = Console.ReadKey ().KeyChar;
			switch (input) {
				case 'a':
					curserX--;
					break;

				case 'd':
					curserX++;
					break;

				case 'w':
					curserY--;
					break;

				case 's':
					curserY++;
					break;

				case 'f':
					board.ToggleFlag ((curserX, curserY));
					break;

				case ' ':
					if (isFirstDig)
						board.RerollLocations ((curserX, curserY), 1);
					isFirstDig = false;
					//Console.ReadKey ();
					board.Uncover ((curserX, curserY), true);
					
					break;
			}
			curserX = Math.Min (Math.Max (curserX, 0), board.boardSize.width-1);
			curserY = Math.Min (Math.Max (curserY, 0), board.boardSize.height-1);
			
			if (board.LostGame ()) {
				Console.Clear ();
				board.PrintBoard ((curserX, curserY));
				PrintLooseMessage ();
				Console.ReadKey ();
				return;
			}

			if (board.WonGame ()) {
				Console.Clear ();
				board.PrintBoard ((curserX, curserY));
				Console.WriteLine ("You Won!");
				Console.ReadKey ();
				return;
			}
		}

	}

	public static void PrintLooseMessage () {
		Console.ForegroundColor = ConsoleColor.White;
		Console.Write ("You ");
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine ("Loose");
		Console.ForegroundColor = ConsoleColor.White;
	}
}