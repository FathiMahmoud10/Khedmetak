using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        #region Relations
        public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
        public ICollection<GovService> GovServices { get; set; } = new List<GovService>();
        #endregion
    }
}
