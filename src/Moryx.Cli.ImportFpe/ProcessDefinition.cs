using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Cli.ImportFpe
{
    public class ProcessDefinition
    {
        [Column("Bezeichnung")]
        public string Name { get; set; }
    }
}
