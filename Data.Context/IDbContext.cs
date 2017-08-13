﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Data.Context
{
    public interface IDbContext : IDisposable
    {
//        bool LazyLoadingEnabled { get; set; }
//        bool ProxyCreationEnabled { get; set; }
//        bool AutoDetectChangesEnabled { get; set; }
//        void SetEntityState(object entity, EntityState state);
        int SaveChanges();

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync();
    }
}
