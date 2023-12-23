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
        void MovePawn(int columns, int rows, string pawn);

        [OperationContract]
        int RollDice();
    }

    [ServiceContract]
    public interface IGameManagerCallback
    {
        [OperationContract]
        void ReceivePawnsMove();    
    }

    [DataContract]
    public class Pawn
    {
        private string color;
        private int xPosition;
        private int yPosition;

        [DataMember]
        public string Color { get { return color; } set { color = value; } }

        [DataMember]
        public int XPosition { get {  return xPosition; } set {  xPosition = value; } }

        [DataMember]
        public int YPosition { get { return yPosition; } set {  yPosition = value; } }

    }

    [DataContract]
    public class RestrictedSquare
    {
        private int xPosition;
        private int yPosition;

        [DataMember]
        public int Xposition { get { return xPosition; } set { xPosition = value; } }

        [DataMember]
        public int Yposition { get { return yPosition; } set { yPosition = value; } }
    }
}
