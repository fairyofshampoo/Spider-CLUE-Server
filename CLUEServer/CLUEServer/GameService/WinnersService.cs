using DataBaseManager;
using GameService.Contracts;
using GameService.Utilities;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;

namespace GameService.Services
{
    public partial class GameService : IWinnersManager
    {
        public List<Winner> GetTopGlobalWinners()
        {
            LoggerManager logger = new LoggerManager(this.GetType());
            List <Winner> topGlobal = new List<Winner>();
            try
            {
                using (var dataBaseContext = new SpiderClueDbEntities())
                {
                    topGlobal = dataBaseContext.gamers
                        .OrderByDescending(top => top.gamesWon)
                        .Take(3)
                        .Select(top => new Winner
                        {
                            Gamertag = top.gamertag,
                            GamesWon = top.gamesWon,
                            Icon = top.imageCode
                        })
                        .ToList();
                }
            }
            catch (EntityException entityException)
            {
                logger.LogError(entityException);
            }
            catch (SqlException sqlException)
            {
                logger.LogError(sqlException);
            }

            return topGlobal;
        }
    }
}
