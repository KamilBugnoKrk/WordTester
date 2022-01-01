// Copyright (C) 2022  Kamil Bugno
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using MediatR;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Shared.RequestModels;
using MyBlazorApp.Shared.ResponseModels;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Handlers.QueryHandlers
{
    public class GetAllLanguagesQueryHandler : IRequestHandler<GetAllLanguagesRequestModel, GetAllLanguagesResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllLanguagesQueryHandler(IUnitOfWork unitOfWork)
        {           
            _unitOfWork = unitOfWork;
        }
        public Task<GetAllLanguagesResponseModel> Handle(GetAllLanguagesRequestModel request, CancellationToken cancellationToken)
        {
            var languages = _unitOfWork.Languages.GetAll();

            return Task.FromResult(new GetAllLanguagesResponseModel
            { 
                LanguageNames = languages.Select(l => l.Name)
            });
        }
    }
}
