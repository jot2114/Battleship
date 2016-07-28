using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using BattleShip.BLL.GameLogic;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Ships;

namespace BattleShip.UI
{
    public class GameManager
    {
        // an object that translates a number to its corresponding letter for the X coordinate
        Dictionary<string, Coordinate> translate = new Dictionary<string, Coordinate>();
        private GameData data;

        public GameManager()
        {
            translation();
        }

        public void RunGame()
        {
            Console.ForegroundColor = ConsoleColor.White;

            do
            {
                data = new GameData();
                DisplayWelcomeToGame();
                PromptForPlayers();
                Console.WriteLine();

                ConsoleIO.Prompt($"{data.player1}'s board");
                Console.WriteLine();
                DisplayBoardSetup(data.board1);
                PlaceShipsOnBoard(data.board1);
                Console.ReadLine();

                Console.Clear();
                ConsoleIO.Prompt($"{data.player2}'s board");
                Console.WriteLine();
                DisplayBoardSetup(data.board2);
                PlaceShipsOnBoard(data.board2);
                Console.Clear();

                PlayersFireShot(data);
            } while (PlayAgain());
        }

        private void PromptForPlayers()
        {
            data.player1 = ConsoleIO.Prompt("Enter the Player1: ");
            data.player2 = ConsoleIO.Prompt("Enter the player2: ");
        }

        private void DisplayWelcomeToGame()
        {
            Console.WriteLine(
                @" ____       _______ _______ _      ______  _____ _    _ _____ _____  
 |  _ \   /\|__   __|__   __| |    |  ____|/ ____| |  | |_   _|  __ \ 
 | |_) | /  \  | |     | |  | |    | |__  | (___ | |__| | | | | |__) |
 |  _ < / /\ \ | |     | |  | |    |  __|  \___ \|  __  | | | |  ___/ 
 | |_) / ____ \| |     | |  | |____| |____ ____) | |  | |_| |_| |     
 |____/_/    \_\_|     |_|  |______|______|_____/|_|  |_|_____|_|     
                                                                      ");

              Console.WriteLine();
        }

        public bool PlayAgain()
        {
            Console.Write("\nWould you like to play again? ");
            string play = Console.ReadLine();
            switch (play.ToUpper())
            {
                case "Y":
                case "YES":
                case "SURE":
                    Console.Clear();
                    return true;
                default:
                    Console.WriteLine("\nThank you for playing");
                    Console.WriteLine("Hit enter key to exit");
                    Console.ReadLine();
                    return false;
            }
           
        }

        public void translation()
        {
            for (char i = 'A'; i < 'K'; i++)
            {
                for (int j = 1; j < 11; j++)
                {
                    translate.Add(i + j.ToString(), new Coordinate(i - 64, j));  
                }
            }
        }

        //placeships for board1(to place ship we need coordinate,direction, shipType)
        //display name of the ship
        //prompt for the coordinates
        //if valid coordinates
        //prompt for the direction
        //placeship
        //want to place more ships?

        public void DisplayBoardSetup(Board board)
        {
            Console.Clear();
            for (char i = 'A'; i < 'K'; i++)
            {
                Console.Write("   {0}   ", i);
            }
            Console.Write("\n");
            Console.WriteLine("   -----------------------------------------------------------------");

            Ship[] ships = board.GetShips();

            for (int i = 1; i < 11; i++)
            {
                Console.Write("{0,-2}", i);
                for (int j = 1; j < 11; j++)
                {
                    var spot = " _     ";

                    var result = from ship in ships
                                 where ship != null && ship.BoardPositions.Any(r => r.XCoordinate == j && r.YCoordinate == i)
                                 select ship;


                    if (result.Any())
                    {
                        PrintLetter(result.FirstOrDefault());
                    }
                    else
                    {
                        var prevColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(spot);
                        Console.ForegroundColor = prevColor;
                    }
                }
                Console.WriteLine();
                var curColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("   -----------------------------------------------------------------");
                Console.ForegroundColor = curColor;
            }
        }

        public void PrintLetter(Ship currentShip)
        {
            if (currentShip.ShipType == ShipType.Battleship)
            {
                Console.Write(" B     ");
            }
            else if (currentShip.ShipType == ShipType.Carrier)
            {
                Console.Write(" Ca    ");
            }
            else if (currentShip.ShipType == ShipType.Cruiser)
            {
                Console.Write(" Cr    ");
            }
            else if (currentShip.ShipType == ShipType.Destroyer)
            {
                Console.Write(" D     ");
            }
            else
            {
                Console.Write(" S     ");
            }
        }

        public PlaceShipRequest PlayersPlaceShips(ShipType ship)
        {
            PlaceShipRequest newShip = new PlaceShipRequest();
            Console.WriteLine(ship.ToString());
            newShip.Coordinate = GetCoordinate(true);
            newShip.Direction = GetDirection();
            newShip.ShipType = ship;
            return newShip;
        }

        public void PlaceShipsOnBoard(Board board)
        {
            foreach (ShipType ship in Enum.GetValues(typeof(ShipType)))
            {
                bool isValid = false;
                while (!isValid)
                {
                    ShipPlacement resposne = board.PlaceShip(PlayersPlaceShips(ship));
                    switch (resposne)
                    {
                        case ShipPlacement.NotEnoughSpace:
                            Console.WriteLine("Not enough space");
              
                            break;
                        case ShipPlacement.Overlap:
                            Console.WriteLine("Overlapping");
                            break;
                        case ShipPlacement.Ok:
                            isValid = true;
                            break;
                        default:
                            Console.WriteLine("Something wrong");
                            break;
                    }
                }
                DisplayBoardSetup(board);
            }
        }

        public Coordinate GetCoordinate(bool isSetup)
        {
            Coordinate result = null;
            do
            {
                if (isSetup)
                {
                    Console.WriteLine("\nPlease enter coordinate to place the ship");    
                }
                string userChoice = Console.ReadLine().ToUpper();

                if (translate.ContainsKey(userChoice))
                {
                    return translate[userChoice];
                }
                else
                    Console.WriteLine("Invalid Coordinate");

            } while (result == null);
            return result;
        }

        public ShipDirection GetDirection()
        {
            Console.WriteLine("Please enter direction to place the ship");
            string storeDirection = Console.ReadLine().ToUpper();

            switch (storeDirection)
            {
                case "U":
                case "UP":
                    return ShipDirection.Up;
                case "D":
                case "DOWN":
                    return ShipDirection.Down;
                case "R":
                case "RIGHT":
                    return ShipDirection.Right;
                case "L":
                case "LEFT":
                    return ShipDirection.Left;
                default: 
                    Console.WriteLine("Invalid direction. Please enter again");
                    storeDirection = Console.ReadLine().ToUpper();
                    break;
            }
            return ShipDirection.Down; // dummy entry
        }

        #region ShipNames
        //public ShipType GetShipType(object obj)
        //{
        //    var result = GetShi
        //    if (obj.ToString() == "Destroyer")
        //       result = ShipType.Destroyer;
        //    else if (obj.ToString() == "SubMarine")
        //        return ShipType.Submarine;
        //    else if (obj.ToString() == "Cruiser")
        //        return ShipType.Cruiser;
        //    else if (obj.ToString() == "Battleship")
        //        return ShipType.Battleship;
        //    else if (obj.ToString() == "Carrier")
        //        return ShipType.Carrier;
        //    else
        //    {
        //        Console.WriteLine("Invalid entry");
        //    }
        //    return ShipType.Destroyer;//dummy
        //}
        #endregion

        //playGame
        public void PlayersFireShot(GameData data)
        {
            bool isWinner = false;

            while (isWinner == false)
            {
                for (int i = 1; i < 3; i++)
                {
                    Board otherBoard = null;
                    if (i % 2 == 0)
                    {
                        Console.WriteLine("\n{0} , enter coordinate to fire shot: ", data.player2);
                        otherBoard = data.board1;
                    }
                    else
                    {
                        Console.WriteLine("\n{0} , enter coordinate to fire shot: ", data.player1);
                        otherBoard = data.board2;

                    }
                    bool isValidShot = false;
                    while (isValidShot == false)
                    {
                        Coordinate shot = GetCoordinate(false);
                        FireShotResponse response = otherBoard.FireShot(shot);

                        switch (response.ShotStatus)
                        {
                            case ShotStatus.Duplicate:
                                Console.WriteLine("It is a duplicate entry. Please try another coordinate");
                                break;
                            case ShotStatus.Invalid:
                                Console.WriteLine("Its an invalid entry, Please try again");
                                break;
                            case ShotStatus.Miss:
                                Console.WriteLine("You missed a shot"); 
                                DisplayOpponentBoard(otherBoard);  
                                isValidShot = true;
                                break;
                            case ShotStatus.Hit:
                                Console.WriteLine("You hit the ship");
                                DisplayOpponentBoard(otherBoard);
                                isValidShot = true;
                                break;
                            case ShotStatus.HitAndSunk:                               
                                DisplayOpponentBoard(otherBoard);
                                Console.WriteLine("Yeah!! Its hit and sunk");
                                isValidShot = true;
                                break;
                            case ShotStatus.Victory:
                                DisplayOpponentBoard(otherBoard);
                                if (i % 2 == 0)
                                {
                                    Console.WriteLine("\n{0} , You won ", data.player2);
                                }
                                else
                                {
                                    Console.WriteLine("\n{0} , You won ", data.player1);
                                }
                                isValidShot = true;
                                isWinner = true;
                                break;
                        }
                    }
                    if (isWinner == true)
                    {
                        break;
                    }
                }
            }
        }

        public void DisplayOpponentBoard(Board board)
        {
            Console.Clear();
            for (char i = 'A'; i < 'K'; i++)
            {
                Console.Write("    {0}  ", i);
            }
            Console.WriteLine();
            Console.WriteLine("  ---------------------------------------------------------------------------");
            for (int i = 1; i < 11; i++)
            {
                Console.Write("{0,-2}", i);
                for (int j = 1; j < 11; j++)
                {
                    ShotHistory result;
                    Coordinate cor = new Coordinate(j, i);

                    ConsoleColor curColor = Console.ForegroundColor;
                    if (board.ShotHistory.TryGetValue(cor, out result))
                    {
                        if (result == ShotHistory.Miss)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("   M   ");
                            Console.ForegroundColor = curColor;
                        }
                        else if (result == ShotHistory.Hit)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("   H   ");
                            Console.ForegroundColor = curColor;
                        }
                    }
                    else
                    {
                        var prevColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("   _   ");
                        Console.ForegroundColor = prevColor;
                    }
                    
                   
                }
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------------------");
            }
        }

        //display whose turn
        //display opponent board
        //Prompt for shot coordinate
        //Check for invalid or duplicate coordinate
        //check hit/miss/sunk
        //display second board 
    }

}


