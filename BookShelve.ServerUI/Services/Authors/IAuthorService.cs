﻿using BookShelve.ServerUI.Services.Base;

namespace BookShelve.ServerUI.Services.Authors
{
    public interface IAuthorService
    {
        Task<Response<List<ReadAuthorDto>>> GetAuthorsAsync();
    }
}
