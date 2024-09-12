using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMinesweeper {
	public class Board {
		Random random = new Random ();
		readonly bool[,] bombs; // the bombs as a grid
		readonly bool[,] uncovered; // whether the player has cleared a tile
		readonly bool[,] flaged; // tracks where the player plants flags
		// the bombs stored as integers. stored in an ordered list. This helps ensure that 2 bombs can't occupy the same tile
		// each number has a corresponding tile. It goes from left to right, and moves down to the next row whenever it reaches the width of the board
		List<int> bombLocations;
		public readonly (int width, int height) boardSize;
		public Board (int width, int height, int numBombs) {
			// creates the 2d Arrays
			bombs = new bool[height, width];
			uncovered = new bool[height, width];
			flaged = new bool[height, width];
			boardSize = (width, height);

			// creates the list of bombs
			bombLocations = new List<int>();

			for (int i = 0; i < numBombs; i++) {
				RandomlyPlaceBomb ();
			}

			for (int i = 0; i < bombLocations.Count; i++) {
				//Console.Write (bombLocations[i]+" ");
				bombs[bombLocations[i] / boardSize.width, bombLocations[i] % boardSize.width] = true;
			}
		}

		// formats bombs from the int list to the 2d array
		public void FormatBombs () {
			for (int y = 0; y < boardSize.height; y++) {
				for (int x = 0; x < boardSize.width; x++) {
					bombs[y, x] = false;
				}
			}

			for (int i = 0; i < bombLocations.Count; i++) {
				bombs[bombLocations[i] / boardSize.width, bombLocations[i] % boardSize.width] = true;
			}
		}

		// clears a S by S square around a tile. This is used to ensure that the player won't land on a mine in their first move
		public void RerollLocations ((int x, int y) loc, int s) {
			for (int j = 0; j <= 4; j++) {
				for (int y = -s; y <= s; y++) {
					if (0 > y+loc.y || y+loc.y >= boardSize.height) continue; // skips anything outside the board
					for (int x = -s; x <= s; x++) {
						if (0 > x+loc.x || x+loc.x >= boardSize.width) continue; // skips anything outside the board

						for (int i = 0; i < bombLocations.Count; i++) {
							// test for bombs
							if (bombLocations[i] == ((loc.y+y)*boardSize.width + (loc.x+x))) {
								// removes the bomb
								bombLocations.RemoveAt (i);
								// places the bomb somewhere else
								RandomlyPlaceBomb ();
								break;
							}
						}
					}
				}
			}
			FormatBombs (); // updates the 2d bomb array
		}

		// places a bomb at a random location where there currently isn't a bomb
		public int RandomlyPlaceBomb () {
			return SafeInsert (random.Next (boardSize.width* boardSize.height - bombLocations.Count));
		}
		// inserts a bomb somewhere there isn't a bomb already while keeping the bomb locations list
		// as an ordered list
		private int SafeInsert (int loc) {
			for (int j = 0; j < bombLocations.Count; j++) {
				if (bombLocations[j] <= loc) {
					loc++;
				}
			}
			int b;
			for (b = bombLocations.Count - 1; b>= 0; b--) {
				if (bombLocations[b] < loc) {
					break;
				}
			}
			bombLocations.Insert (b+1, loc);
			return loc;
		}

		
		// returns how many bombs suround a tile
		public int CountBombs ((int x, int y) location) {
			int numBombs = 0;
			for (int y = location.y - 1; y <= location.y + 1; y++) {
				if (y < 0) continue;// skips tiles outside the board
				if (y >= boardSize.height) break; // skips tiles outside the board
				for (int x = location.x - 1; x <= location.x + 1; x++) {
					if (x < 0) continue; // skips tiles outside the board
					if (x >= boardSize.width) break; // skips tiles outside the board

					numBombs += bombs[y, x] ? 1 : 0;
				}
			}
			return numBombs;
		}

		// prints the board to the terminal
		public void PrintBoard ((int x, int y) curser) {
			for (int y = 0; y < boardSize.height; y++) {
				for (int x = 0; x < boardSize.width; x++) {

					var cc = ConsoleColor.Gray;
					char under = '.';
					int numBombs = CountBombs ((x, y));
					if (numBombs > 0) {
						under = (char)('0' + numBombs);
						cc = ConsoleColor.Blue;
					}
					if (bombs[y, x]) {
						under = 'b';
						cc = ConsoleColor.Red;
					}
					if (!uncovered[y, x]) {
						cc = ConsoleColor.White;
					}

					if (curser.x == x && curser.y == y)
						cc = ConsoleColor.Green;

					Console.ForegroundColor = cc;
					Console.Write ((uncovered[y, x]? under: (flaged[y, x]? 'F': '#')));
					Console.ForegroundColor= ConsoleColor.White;
				}
				Console.WriteLine ();
			}
		}

		// if the specified position is covered, place a flag there if there isn't one alfready, or remove one if theres already one there
		public void ToggleFlag ((int x, int y) curser) {
			if (!uncovered[curser.y, curser.x])
				flaged[curser.y, curser.x] = !flaged[curser.y, curser.x];
		}

		

		public void Uncover ((int x, int y) curser, bool initialClick) {
			if (curser.x < 0 || curser.x >= boardSize.width) return; // stops the funstion if the specified location isn't there
			if (curser.y < 0 || curser.y >= boardSize.height) return;
			if (flaged[curser.y, curser.x]) return;
			if (!uncovered[curser.y, curser.x]) {
				
				uncovered[curser.y, curser.x] = true;// clears a tile
				if (CountBombs (curser) <= 0) { // clears large sections of bombless terrain.
					for (int d = -1; d <= 1; d+=2) {
						Uncover ((curser.x + d, curser.y), false);
						Uncover ((curser.x, curser.y + d), false);
						Uncover ((curser.x + d, curser.y + d), false);
						Uncover ((curser.x - d, curser.y + d), false);
					}
				}
				
			}
			else if (initialClick) { // clears a 3x3 area around the tile, if the tile is already clear.
				for (int y = -1; y <= 1; y += 1) {
					if (0 > y+curser.y || y+curser.y >= boardSize.height) continue;
					for (int x = -1; x <= 1; x += 1) {
						Uncover ((curser.x + x, curser.y + y), false);
					}
				}
			}
		}

		// returns if a tile is a bomb
		public bool IsBomb ((int x, int y) loc) {
			return bombs[loc.y, loc.x];
		}

			
		// determains if the player one or lost the game by looking for tiles that are both uncovered and a bomb
		public bool LostGame () {
			for (int y = 0; y < boardSize.height; y++) {
				for (int x = 0; x < boardSize.width; x++) {
					if (bombs[y, x] && uncovered[y, x]) return true;
				}
			}
			return false;
		}

		// determines if the player has won the game by looking for tiles that are covered and not bombs
		public bool WonGame () {
			bool won = true;
			for (int y = 0; y < boardSize.height; y++) {
				for (int x = 0; x < boardSize.width; x++) {
					if (!bombs[y, x]&&!uncovered[y, x])
						return false;
				}
			}
			return true;
		}
	}
}
