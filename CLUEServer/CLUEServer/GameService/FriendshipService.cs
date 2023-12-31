﻿using DataBaseManager;
using GameService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Services
{
    public partial class GameService : IFriendshipManager
    {
        public List<string> GetFriendList(string gamertag)
        {
            using (var databaseContext = new SpiderClueDbEntities())
            {
                var friendList = databaseContext.friendLists.Where(user => user.gamertag == gamertag)
                    .Select(user => user.friend).ToList();
                return friendList;
            }
        }

        public void DeleteFriend(string gamertag, string friendGamertag)
        {
            using (var databaseContext = new SpiderClueDbEntities())
            {
                var friendEliminated = databaseContext.friendLists
                .Where(friend => friend.gamertag == gamertag && friend.friend == friendGamertag);
                if(friendEliminated.Any())
                {
                    databaseContext.friendLists.RemoveRange(friendEliminated);
                    databaseContext.SaveChanges();
                    DeleteFriend(friendGamertag, gamertag);
                }
            }
        }

        public void AddFriend(string gamertag, string friendGamertag)
        {
            using (var databaseContext = new SpiderClueDbEntities())
            {
                var existingFriendship = databaseContext.friendLists
                .FirstOrDefault(f => f.gamertag == gamertag && f.friend == friendGamertag);
                if (existingFriendship == null)
                {
                    var newFriends = new DataBaseManager.friendList
                    {
                        gamertag = gamertag,
                        friend = friendGamertag
                    };
                    databaseContext.friendLists.Add(newFriends);
                    databaseContext.SaveChanges();
                    AddFriend(friendGamertag, gamertag);
                    
                }    
            }
        }

        public bool AreNotFriends(string gamertag, string friendGamertag)
        {
            using (var databaseContext = new SpiderClueDbEntities())
            {
                Boolean areNotFriends = true;
                var existingFriendship = databaseContext.friendLists
                    .FirstOrDefault(friends => friends.gamertag == gamertag && friends.friend== friendGamertag);
                if (existingFriendship != null)
                {
                    areNotFriends = false;
                }
                return areNotFriends;
            }
        }

        public bool ThereIsNoFriendRequest(string gamertag, string friendGamertag)
        {
            using (var databaseContext = new SpiderClueDbEntities())
            {
                Boolean ThereIsNotFriendRequest = true;
                var existingFriendRequest = databaseContext.friendRequests
                    .FirstOrDefault(friendRequest => friendRequest.senderGamertag == gamertag && friendRequest.receiverGamertag == friendGamertag);
                if(existingFriendRequest != null)
                {
                    ThereIsNotFriendRequest = false;
                }
                return ThereIsNotFriendRequest;
            }
        }
    }
}
