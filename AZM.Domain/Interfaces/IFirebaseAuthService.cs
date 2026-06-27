namespace AZM.Domain.Interfaces
{
    public interface IFirebaseAuthService
    {
        /// <summary>
        /// Validates a Firebase ID token and returns the phone number
        /// encoded in it, or null if the token is invalid/expired.
        /// </summary>
        Task<string?> VerifyPhoneTokenAsync(string firebaseIdToken);
    }
}