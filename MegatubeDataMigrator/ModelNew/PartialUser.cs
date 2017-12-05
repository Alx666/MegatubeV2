using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegatubeDataMigrator.ModelNew
{
    public partial class User
    {
        public override string ToString() => $"{this.Id} {this.Name} {this.LastName}";       
    }
}
