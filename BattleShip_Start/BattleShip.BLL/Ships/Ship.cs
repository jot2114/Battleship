using System.Linq;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;

namespace BattleShip.BLL.Ships
{
    public class Ship
    {
        public ShipType ShipType { get; private set; }
        public string ShipName { get { return ShipType.ToString(); } }
        public Coordinate[] BoardPositions { get; set; }
        private int _lifeRemaining;
        public bool IsSunk { get { return _lifeRemaining == 0; } }

        // / Instantiates a new ship, sets its properties based on the ship type
        public Ship(ShipType shipType, int numberOfSlots)
        {
            ShipType = shipType;
            _lifeRemaining = numberOfSlots;
            BoardPositions = new Coordinate[numberOfSlots];
        }

        // /changes shot status whether a ship is present in the given coordinates
        public ShotStatus FireAtShip(Coordinate position)
        {
            if (BoardPositions.Contains(position))
            {
                _lifeRemaining--;

                if(_lifeRemaining == 0)
                    return ShotStatus.HitAndSunk;

                return ShotStatus.Hit;
            }
            return ShotStatus.Miss;
        }
    }
}
