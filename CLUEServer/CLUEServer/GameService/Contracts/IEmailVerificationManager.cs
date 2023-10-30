using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Contracts
{
    [ServiceContract]
    interface IEmailVerificationManager
    {
        [OperationContract]
        bool GenerateVerificationCode(string email);

        [OperationContract]
        bool VerifyCode(string email, string code);
    }
}
