using DataBaseManager;
using GameService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService
{
    public partial class GameService : IWinnersManager
    {
        public List<Winner> GetTopGlobalWinners()
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                
            }
        }
    }
}
