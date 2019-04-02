using GitStat.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GitStat.Core.Contracts
{
    public interface IDeveloperRepository
    {
        IEnumerable<Statistic> GetStatisticFromDevelopers();
    }
}
