using BookShelve.ServerUI.Services.Base;

namespace BookShelve.ServerUI.Services.Auth
{
    public interface IAuthService
    {
        Task<bool> AuthenticateAsync(LoginUserDto loginUserDto);
        Task LogOut();
    }
}
