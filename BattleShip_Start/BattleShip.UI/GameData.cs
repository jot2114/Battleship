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
    public class GameData
    {
        public string player1 { get; set; }
        public string player2 { get; set; }

        public Board board1  = new Board();
        public Board board2 = new Board();
  
    }
}
