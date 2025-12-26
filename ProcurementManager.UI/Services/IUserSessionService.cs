using ProcurementManager.Core.Models;

namespace ProcurementManager.UI.Services
{
    public interface IUserSessionService
    {
        User? CurrentUser { get; }
        void SetCurrentUser(User user);
    }
}
