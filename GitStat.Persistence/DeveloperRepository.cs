using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GitStat.Core.Contracts;
using GitStat.Core.Entities;

namespace GitStat.Persistence
{
    public class DeveloperRepository : IDeveloperRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DeveloperRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Statistic> GetStatisticFromDevelopers()
        {
            return _dbContext.Developers.Select(s => new Statistic
            {
                DeveloperName = s.Name,
                Commits = s.Commits.Count(),
                FilesChanged = s.Commits.Sum(fCh => fCh.FilesChanges),
                Insertions = s.Commits.Sum(ins => ins.Insertions),
                Deletions = s.Commits.Sum(del => del.Deletions)
            }
            ).ToList()
             .OrderByDescending(o => o.Commits);
        }
    }
}