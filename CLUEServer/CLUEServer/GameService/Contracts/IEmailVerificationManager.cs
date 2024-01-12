using System.ServiceModel;

namespace GameService.Contracts
{
    /// <summary>
    /// Service contract for managing email verification.
    /// </summary>
    [ServiceContract]
    interface IEmailVerificationManager
    {
        /// <summary>
        /// Generates a verification code for the specified email.
        /// </summary>
        /// <param name="email">The email address for which to generate the verification code.</param>
        /// <returns>True if the verification code is successfully generated; otherwise, false.</returns>
        [OperationContract]
        bool GenerateVerificationCode(string email);

        /// <summary>
        /// Verifies a verification code for the specified email.
        /// </summary>
        /// <param name="email">The email address to verify.</param>
        /// <param name="code">The verification code to check.</param>
        /// <returns>True if the verification code is valid for the specified email; otherwise, false.</returns>
        [OperationContract]
        bool VerifyCode(string email, string code);
    }
}