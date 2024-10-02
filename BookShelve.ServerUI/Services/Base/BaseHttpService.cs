using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace BookShelve.ServerUI.Services.Base
{
    public class BaseHttpService
    {
        private readonly IClient _client;
        private readonly ILocalStorageService _localStorage;
        public BaseHttpService(IClient client, ILocalStorageService localStorage)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        // Convert API exceptions
        protected Response<Guid> ConvertApiExceptions<Guid>(ApiException apiException)
        {
            if(apiException.StatusCode == 400)
            {
                return new Response<Guid>()
                {
                    Message = "A validation error occured.",
                    ValidationErrors = apiException.Response,
                    IsSuccess = false
                };
            }
            if(apiException.StatusCode == 404)
            {
                return new Response<Guid>()
                {
                    Message = "The requested service could not be found.",
                    ValidationErrors = apiException.Response,
                    IsSuccess = false
                };
            }
            if(apiException.StatusCode == 500)
            {
                return new Response<Guid>()
                {
                    Message = "Internal Server Error",
                    ValidationErrors = apiException.Response,
                    IsSuccess = false
                };
            }
            return new Response<Guid>()
            {
                Message = "Oopsie! Something went wrong! Please try again later.",
                IsSuccess = false
            };
        }

        // Get the store toke from the browser storage
        protected async Task GetBearerTokeAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("accessToken");

            if(token != null)
            {
                _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }

        }
    }
}
