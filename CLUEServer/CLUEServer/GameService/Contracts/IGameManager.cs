using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract(CallbackContract = typeof(IGameManagerCallback))]
    public interface IGameManager
    {
        [OperationContract(IsOneWay = true)]
        void MovePawn(string xPosition, string yPosition, string pawn);

        [OperationContract]
        int RollDice();
    }

    [ServiceContract]
    public interface IGameManagerCallback
    {
        
        [OperationContract]
        void MovePawn();    
    }

}
