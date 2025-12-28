using ProcurementManager.Core.Models;

namespace ProcurementManager.UI.Services
{
    public class UserSessionService : IUserSessionService
    {
        public User? CurrentUser { get; private set; }

        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }
    }
}
