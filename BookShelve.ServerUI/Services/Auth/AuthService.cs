using Blazored.LocalStorage;
using BookShelve.ServerUI.Providers;
using BookShelve.ServerUI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;

namespace BookShelve.ServerUI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IClient _client;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationProvider;

        public AuthService(IClient client, ILocalStorageService localStorage, AuthenticationStateProvider authenticationProvider)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
            _authenticationProvider = authenticationProvider ?? throw new ArgumentNullException(nameof(authenticationProvider));
        }
        public async Task<bool> AuthenticateAsync(LoginUserDto loginUserDto)
        {
            var response = await _client.LoginAsync(loginUserDto);

            // generate and store token
            await _localStorage.SetItemAsync("accessToken", response.Token);

            // change auth state of the app

            await ((JwtAuthenticationProvider)_authenticationProvider).LoggedIn();

            return true;
        }

        public async Task LogOut()
        {
            await ((JwtAuthenticationProvider)_authenticationProvider).LoggedOut();
        }
    }
}
