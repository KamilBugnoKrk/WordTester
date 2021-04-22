// Copyright (C) 2021  Kamil Bugno
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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared;
using MyBlazorApp.Shared.RequestModels;
using MyBlazorApp.Shared.ResponseModels;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Handlers.QueryHandlers
{
    public class DeleteWordCommandHandler : IRequestHandler<DeleteWordRequestModel, DeleteWordResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteWordCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteWordResponseModel> Handle(DeleteWordRequestModel request, CancellationToken cancellationToken)
        {
            if (!IsUserWord(request))
                return new DeleteWordResponseModel
                {
                    IsSucceed = false,
                    Error = "You can delete only your words"
                };

            var word = _unitOfWork.Words.Find(w => w.Id.ToString() == request.WordId).First();
            _unitOfWork.Words.Remove(word);
            _unitOfWork.Complete();

            return new DeleteWordResponseModel
            {
                IsSucceed = true
            };
        }

        private bool IsUserWord(DeleteWordRequestModel request)
        {
            var word = _unitOfWork
                .Words
                .GetUserWordById(int.Parse(request.WordId), request.UserId);

            return word != null;
        }
    }
}
