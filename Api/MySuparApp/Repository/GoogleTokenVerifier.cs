using Google.Apis.Auth;
using Google.Apis.Util;
using UserAuthModel;

namespace GoogleSigninTokenVerification
{
    public class GoogleTokenVerifier
    {
        public async Task<GoogleUserInfo> VerifyGoogleTokenAndGetEmailAsync(string idToken, string expectedClientId)
        {
            var result = new GoogleUserInfo();
            try
            {
                // Set up validation settings with the expected client ID
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { expectedClientId }
                };

                // Verify the token with the validation settings
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

                // Extract email from payload
                result.Email = payload.Email;   
                result.EmailVerified = payload.EmailVerified;
                result.msg = "Success";
                //string email = payload.Email;

                // You can also access other information like name, profile picture, etc. if needed
                // string name = payload.Name;
                // string pictureUrl = payload.Picture;

                return result;
            }
            catch (InvalidJwtException e)
            {
                // Invalid token or mismatched client ID
                result.Email = "";
                result.EmailVerified = false;
                result.msg = e.Message;
                return (result);
            }
        }
    }
}
