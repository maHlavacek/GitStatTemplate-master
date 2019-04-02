using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitStat.Core.Entities
{
    public class Commit : EntityObject
    {

        public int DeveloperId { get; set; }

        [ForeignKey(nameof(DeveloperId))]
        public Developer Developer { get; set; }

        public DateTime Date { get; set; }
        public string HashCode { get; set; }

        public string Message { get; set; }

        public int FilesChanges { get; set; }
        public int Insertions { get; set; }
        public int Deletions { get; set; }

    }
}
