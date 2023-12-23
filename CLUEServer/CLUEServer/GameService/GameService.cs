using GameService.Contracts;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    public partial class GameService : IGameManager
    {
        public void MovePawn(string xPosition, string yPosition, string pawn)
        {
            
        }

        public int RollDice()
        {
            Random random = new Random();
            int rollDice = random.Next(2, 13);
            return rollDice;
        }
    }
}