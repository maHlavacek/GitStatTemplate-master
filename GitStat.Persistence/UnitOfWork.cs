using System;
using GitStat.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Utils;


namespace GitStat.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private bool _disposed;

        public UnitOfWork()
        {
            _dbContext = new ApplicationDbContext();
            CommitRepository = new CommitRepository(_dbContext);
            DeveloperRepository = new DeveloperRepository(_dbContext);

            //MyLogger.InitializeLogger();

            var serviceProvider = _dbContext.GetInfrastructure();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddSerilog();
        }

        public ICommitRepository CommitRepository { get; }
        public IDeveloperRepository DeveloperRepository { get; }


        /// <summary>
        /// Repository-übergreifendes Speichern der Änderungen
        /// </summary>
        public int SaveChanges()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }
        public void DeleteDatabase() => _dbContext.Database.EnsureDeleted();
        public void MigrateDatabase() => _dbContext.Database.Migrate();


    }
}
