using DataBaseManager;
using GameService.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace GameService.Services
{
    public partial class GameService : IWinnersManager
    {
        public List<Winner> GetTopGlobalWinners()
        {
            using (var dataBaseContext = new SpiderClueDbEntities())
            {
                var topGlobal = dataBaseContext.gamers
                    .OrderByDescending(top => top.gamesWon)
                    .Take(3)
                    .Select(top => new Winner
                    {
                        Gamertag = top.gamertag,
                        GamesWon = top.gamesWon,
                        Icon =  top.imageCode
                    })
                    .ToList();
                return topGlobal;
            }
        }
    }
}
