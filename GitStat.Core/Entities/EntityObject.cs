using System.ComponentModel.DataAnnotations;
using GitStat.Core.Contracts;

namespace GitStat.Core.Entities
{
    public class EntityObject : IEntityObject
    {
        [Key]
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion
        {
            get;
            set;
        }
    }
}
