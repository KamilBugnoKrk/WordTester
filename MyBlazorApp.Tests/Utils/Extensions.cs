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

using Microsoft.EntityFrameworkCore;
using Moq;
using MyBlazorApp.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazorApp.Tests.Utils
{
    public static class Extensions
    {
        public static DbSet<T> AsDbSet<T>(this IEnumerable<T> entities) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(entities.AsQueryable().Provider);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(entities.AsQueryable().Expression);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(entities.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(entities.AsQueryable().GetEnumerator());
            return dbSetMock.Object;
        }

        public static ApplicationDbContext MockContext<T>(this DbSet<T> dbSet, Expression<Func<ApplicationDbContext, DbSet<T>>> expression) where T : class
        {
            var context = new Mock<ApplicationDbContext>();
            context.Setup(x => x.Set<T>()).Returns(dbSet);
            context.Setup(expression).Returns(dbSet);
            return context.Object;
        }
    }
}
