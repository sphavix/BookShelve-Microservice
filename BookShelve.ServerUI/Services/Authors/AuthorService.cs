using Blazored.LocalStorage;
using BookShelve.ServerUI.Services.Base;

namespace BookShelve.ServerUI.Services.Authors
{
    public class AuthorService : BaseHttpService, IAuthorService
    {
        private readonly IClient _client;
        public AuthorService(IClient client, ILocalStorageService localStorage) : base(client, localStorage)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<Response<List<ReadAuthorDto>>> GetAuthorsAsync()
        {
            Response<List<ReadAuthorDto>> response;

            try
            {
                // Add the bearer token to the headers
                await GetBearerTokeAsync();

                var content = await _client.AuthorsAllAsync();
                response = new Response<List<ReadAuthorDto>>
                {
                    Data = content.ToList(),
                    IsSuccess = true
                };
            }
            catch(ApiException exception)
            {
                response = ConvertApiExceptions<List<ReadAuthorDto>>(exception);
            }

            return response;
        }
    }
}
