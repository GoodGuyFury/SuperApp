using MySuparApp.Models.Authentication;

namespace MySuparApp.Shared
{
    public interface ICurrentUserService
    {
        CurrentUser? GetCurrentUser();
        void SetCurrentUser(CurrentUser user);
    }
    public class CurrentUserService : ICurrentUserService
    {
        private CurrentUser? _currentUser;

        public CurrentUser? GetCurrentUser()
        {
            return _currentUser;
        }

        public void SetCurrentUser(CurrentUser user)
        {
            _currentUser = user;
        }
    }
}
