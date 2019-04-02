using System;
using System.Collections.Generic;
using System.Text;

namespace GitStat.Core.Contracts
{
    public interface IStatistic
    {

        string DeveloperName { get; set; }
        int Commits { get;  set; }

        int Insertions { get; set; }
        int Deletions { get; set; }

        int FilesChanged { get; set; }
    }
}
