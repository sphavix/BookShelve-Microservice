using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookShelve.ServerUI.Providers
{
    public class JwtAuthenticationProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public JwtAuthenticationProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            var storedToken = await _localStorage.GetItemAsync<string>("accessToken");

            if(storedToken == null)
            {
                return new AuthenticationState(user);
            }

            var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(storedToken);

            // validate the token
            if(jwtToken.ValidTo < DateTime.Now)
            {
                return new AuthenticationState(user);
            }

            // assign claims
            var claims = await GetClaims();

            user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            return new AuthenticationState(user);
        }

        public async Task LoggedIn()
        {

            var claims = await GetClaims();
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            var authState = Task.FromResult(new AuthenticationState(user)); // create auth state to the principal

            NotifyAuthenticationStateChanged(authState);
        }

        public async Task LoggedOut()
        {
            await _localStorage.RemoveItemAsync("accessToken"); // clear the token from the browser

            var loggedOut = new ClaimsPrincipal(new ClaimsIdentity());

            var authState = Task.FromResult(new AuthenticationState(loggedOut));

            NotifyAuthenticationStateChanged(authState);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var storedToken = await _localStorage.GetItemAsync<string>("accessToken");

            var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(storedToken);

            var claims = jwtToken.Claims.ToList();

            claims.Add(new Claim(ClaimTypes.Name, jwtToken.Subject));

            return claims;
        }
    }
}
