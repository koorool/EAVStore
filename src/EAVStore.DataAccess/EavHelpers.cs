using System.Collections.Generic;
using System.Linq;
using EAVStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EAVStore.DataAccess
{
    public static class EavHelpers
    {
        public static IIncludableQueryable<EavEntity, List<AttributeValueEntity>> WithAttributes(
            this IQueryable<EavEntity> query
        ) {
            return query.Include(x => x.AttributeValues);
        }
    }
}