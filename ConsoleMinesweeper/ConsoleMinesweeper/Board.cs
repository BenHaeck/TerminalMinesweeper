using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMinesweeper {
	public class Board {
		Random random = new Random ();
		readonly bool[,] bombs;
		readonly bool[,] uncovered;
		readonly bool[,] flaged;
		List<int> bombLocations;
		public readonly (int width, int height) boardSize;
		public Board (int width, int height, int numBombs) {
			bombs = new bool[height, width];
			uncovered = new bool[height, width];
			flaged = new bool[height, width];
			boardSize = (width, height);

			bombLocations = new List<int>();

			for (int i = 0; i < numBombs; i++) {
				RandomlyPlaceBomb ();
			}

			for (int i = 0; i < bombLocations.Count; i++) {
				//Console.Write (bombLocations[i]+" ");
				bombs[bombLocations[i] / boardSize.width, bombLocations[i] % boardSize.width] = true;
			}
		}

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

		public void RerollLocations ((int x, int y) loc, int s) {
			for (int j = 0; j <= 4; j++) {
				for (int y = -s; y <= s; y++) {
					if (0 > y+loc.y || y+loc.y >= boardSize.height) continue;
					for (int x = -s; x <= s; x++) {
						if (0 > x+loc.x || x+loc.x >= boardSize.width) continue;

						for (int i = 0; i < bombLocations.Count; i++) {

							if (bombLocations[i] == ((loc.y+y)*boardSize.width + (loc.x+x))) {

								bombLocations.RemoveAt (i);

								RandomlyPlaceBomb ();
								break;
							}
						}
					}
				}
			}
			FormatBombs ();
		}

		public int RandomlyPlaceBomb () {
			return SafeInsert (random.Next (boardSize.width* boardSize.height - bombLocations.Count));
		}

		public int SafeInsert (int loc) {
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

		

		public int CountBombs ((int x, int y) location) {
			int numBombs = 0;
			for (int y = location.y - 1; y <= location.y + 1; y++) {
				if (y < 0) continue;
				if (y >= boardSize.height) break;
				for (int x = location.x - 1; x <= location.x + 1; x++) {
					if (x < 0) continue;
					if (x >= boardSize.width) break;

					numBombs += bombs[y, x] ? 1 : 0;
				}
			}
			return numBombs;
		}

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

		public void ToggleFlag ((int x, int y) curser) {
			if (!uncovered[curser.y, curser.x])
				flaged[curser.y, curser.x] = !flaged[curser.y, curser.x];
		}

		

		public void Uncover ((int x, int y) curser, bool initialClick) {
			if (curser.x < 0 || curser.x >= boardSize.width) return;
			if (curser.y < 0 || curser.y >= boardSize.height) return;
			if (flaged[curser.y, curser.x]) return;
			if (!uncovered[curser.y, curser.x]) {
				
				uncovered[curser.y, curser.x] = true;
				if (CountBombs (curser) <= 0) {
					for (int d = -1; d <= 1; d+=2) {
						Uncover ((curser.x + d, curser.y), false);
						Uncover ((curser.x, curser.y + d), false);
						Uncover ((curser.x + d, curser.y + d), false);
						Uncover ((curser.x - d, curser.y + d), false);
					}
				}
				
			}
			else if (initialClick) {
				for (int y = -1; y <= 1; y += 1) {
					if (0 > y+curser.y || y+curser.y >= boardSize.height) continue;
					for (int x = -1; x <= 1; x += 1) {
						Uncover ((curser.x + x, curser.y + y), false);
					}
				}
			}
		}

		public bool IsBomb ((int x, int y) loc) {
			return bombs[loc.y, loc.x];
		}

		public bool LostGame () {
			for (int y = 0; y < boardSize.height; y++) {
				for (int x = 0; x < boardSize.width; x++) {
					if (bombs[y, x] && uncovered[y, x]) return true;
				}
			}
			return false;
		}

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
