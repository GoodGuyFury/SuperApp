using Google.Apis.Auth;
using Google.Apis.Util;
using AuthModel;

namespace GoogleTokenVerifierRepository
{
    public class GoogleTokenVerifier
    {
        public static async Task<GoogleUserInfo> VerifyGoogleTokenAndGetEmailAsync(string jwtToken, string expectedClientId)
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
                var payload = await GoogleJsonWebSignature.ValidateAsync(jwtToken, validationSettings);

                // Extract email from payload
                result.email = payload.Email;
                result.emailVerified = payload.EmailVerified;
                result.name = payload.Name;
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
                result.email = "";
                result.emailVerified = false;
                result.msg = e.Message;
                return result;
            }
        }
    }
}
