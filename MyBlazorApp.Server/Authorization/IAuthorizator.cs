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

using Microsoft.AspNetCore.Identity;
using MyBlazorApp.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Authorization
{
    public interface IAuthorizator
    {
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<bool> CheckPasswordSignInAsync(ApplicationUser user, string password);
        Task SignInAsync(ApplicationUser user, bool isPersistent);
        Task<(bool isSuccess, string error)> CreateAsync(ApplicationUser user, string password);
        Task SignOutAsync();
    }
}
