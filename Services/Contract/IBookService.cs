﻿using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequesFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contract
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync(BookParameters bookParameters, bool trackChanges);
        Task<BookDto> GetOneBookByIdAsync(int id, bool trackChange);
        Task<BookDto> CreateOneBookAsync(BookDtoForInsertion book);
        Task UpdateOneBookAsync(int id, BookDtoForUpdate bookDto, bool trackChanges);
        Task DeleteOneBookAsync(int id, bool trackChange);
        Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> GetOneBookForPatchAsync(int id, bool trackChanges);

        Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book);

    }
}
