using System;

namespace GitStat.Core.Contracts
{
    public interface IUnitOfWork: IDisposable
    {
 
        ICommitRepository CommitRepository { get; }
        IDeveloperRepository DeveloperRepository { get; }

        int SaveChanges();

        void DeleteDatabase();

        void MigrateDatabase();
    }
}
