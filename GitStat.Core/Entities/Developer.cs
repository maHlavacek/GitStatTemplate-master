using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GitStat.Core.Entities
{
    /// <summary>
    /// Developer mit Messwerten (Measurements)
    /// </summary>
    public class Developer : EntityObject
    {
        [Required]
        public string Name { get; set; }
        public ICollection<Commit> Commits { get; set; }

        public Developer()
        {
            Commits = new List<Commit>();
        }
    }
}
