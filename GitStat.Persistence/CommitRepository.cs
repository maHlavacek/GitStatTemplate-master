using System;
using System.Collections.Generic;
using System.Linq;
using GitStat.Core.Contracts;
using GitStat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GitStat.Persistence
{
    public class CommitRepository : ICommitRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommitRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddRange(Commit[] commits)
        {
            _dbContext.Commits.AddRange(commits);
        }

        public IEnumerable<Commit> GetAllCommitsFromLastWeeks(int weeks)
        {
            return _dbContext.Commits.Where(w => w.Date >= _dbContext
                    .Commits.Max(m => m.Date).AddDays(-weeks*7))
                    .OrderByDescending(o => o.Date)
                    .Include(d => d.Developer);
        }

        public Commit GetCommitWithId(int id)
        {
            try
            {
                return _dbContext.Commits.Where(w => w.Id == id).Include(d => d.Developer).Single();
            }
            catch (Exception)
            {
                throw new ArgumentException($"Commit mit {id} existiert nicht");
            }
        }
    }
}