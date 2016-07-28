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
    class Program
    {
        public static void Main(string[] args)
        {
           
            GameManager manager = new GameManager();
            manager.RunGame();
        }
    }
 }
