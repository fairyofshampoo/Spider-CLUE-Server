using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract(CallbackContract = typeof(ILobbyManagerCallback))]
    public interface ILobbyManager
    {

        [OperationContract(IsOneWay = true)]
        void KickPlayer(string gamertag);

        [OperationContract(IsOneWay = true)]
        void BeginMatch(string matchCode);

        [OperationContract]
        bool IsOwnerOfTheMatch(string gamertag,string matchCode);

        [OperationContract(IsOneWay = true)]
        void ConnectToLobby(string gamertag, string matchCode);

        [OperationContract]
        Character GetCharacterPerGamer(string gamertag);
    }


    [ServiceContract]
    public interface ILobbyManagerCallback
    {
        [OperationContract]
        void KickPlayerFromMatch(string gamertag);

        [OperationContract]
        void StartGame();
    }

    [DataContract]
    public class Character
    {
        private string characterName;
        private string pawnName;

        [DataMember]
        public string CharacterName { get { return characterName; } set { characterName = value; } }
        [DataMember]
        public string PawnName { get { return pawnName; } set { pawnName = value; } }
    }
}
