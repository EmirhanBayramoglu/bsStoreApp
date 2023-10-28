using Entities.DataTransferObjects;
using Entities.LinkModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contract
{
    public interface IBookLinks
    {
        LinkResponse TryGenerateLinks(IEnumerable<BookDto> bookDto,
            string fields, HttpContext httpContext);

    }
}
