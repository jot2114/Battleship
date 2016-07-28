using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleShip.BLL.GameLogic;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Ships;

namespace BattleShip.UI
{
    public class ConsoleIO
    {
        public static string Prompt(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }

        public static int intPrompt(string message)
        {
            Console.WriteLine(message);
            return int.Parse(Console.ReadLine());
        }

       
    }
}
