using GitStat.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitStat.Core.Entities
{
    public class Statistic : IStatistic
    {
        public string DeveloperName { get; set; }
        public int Commits { get ; set; }
        public int Insertions { get ; set ; }
        public int Deletions { get ; set ; }
        public int FilesChanged { get ; set ; }
    }
}
