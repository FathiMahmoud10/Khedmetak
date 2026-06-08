using Khedmetak.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    public class UserDocument : BaseEntity
    {
        public string Name  { get; set; }
        public string FilePath { get; set; }
    }
}
