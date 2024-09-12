// See https://aka.ms/new-console-template for more information
using System;
using System.Security.Cryptography.X509Certificates;
using ConsoleMinesweeper;

public static class Program {
	public static void Main () {

		// outer loop
		while (true) {
			switch (Menu ()) {
				case 'P':
					Play (20, 18, 50); // starts a game
					break;
				case 'E':
					return;// exits the game
			}
		}
	}

	// gets the players input
	public static char Menu () {
		int menuPos = 0;
		var options = new string[]{ "Play", "Exit" };
		while (true) {// game loop
			Console.Clear ();
			Console.WriteLine ("Minesweeper\n(A&D to move curser, Space to select)");// prints the title and button prompts
			for (int i = 0; i < options.Length; i++) { // prints the menu
				bool selected = menuPos == i;
				Console.Write ((selected ? ">" : "")+ options[i]+(selected ? '<' : ' '));
			}
			switch (Console.ReadKey ().KeyChar) {//takes inputs
				case 'a':
					menuPos--;
					break;

				case 'd':
					menuPos++;
					break;
				case ' ':
					return options[menuPos][0];
			}
			menuPos = Math.Min (Math.Max (0, menuPos), options.Length-1); // keeps the curser from going out of scope
		}
		
	}

	// starts the game
	public static void Play (int boardWidth, int boardHeight, int numBombs) {
		int curserX = 0, curserY = 0;
		var board = new Board (boardWidth, boardHeight, numBombs);
		bool isFirstDig = true;//tracks whether this is the first tile the player clears
		char input = ' ';
		while (input != 'e') {
			Console.Clear ();
			Console.WriteLine ("Controls: WASD to move curser, space to uncover a tile, and F to plant a flag");// prompts the player for input
			board.PrintBoard ((curserX, curserY));// prints the board
			input = Console.ReadKey ().KeyChar; // reads the player input
			switch (input) {// runs code based on said input
				// moves the curser
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

				// plants flag
				case 'f':
					board.ToggleFlag ((curserX, curserY));
					break;

				// digs up turrain
				case ' ':
					if (isFirstDig)
						board.RerollLocations ((curserX, curserY), 1);// ensures that the first tile cleared is not a bomb
					isFirstDig = false;
					//Console.ReadKey ();
					board.Uncover ((curserX, curserY), true);
					
					break;
			}
			// keeps the curser from going off the board
			curserX = Math.Min (Math.Max (curserX, 0), board.boardSize.width-1);
			curserY = Math.Min (Math.Max (curserY, 0), board.boardSize.height-1);

			// checks for if the player has lost
			if (board.LostGame ()) {
				Console.Clear ();
				board.PrintBoard ((curserX, curserY));
				PrintLooseMessage ();
				Console.ReadKey ();
				return;
			}

			// checks for if the player has one.
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
