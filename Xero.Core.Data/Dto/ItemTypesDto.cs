using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xero.Core.Data.Dto
{
    public class ItemTypesDto
    {
        public int Recnum { get; set; }
        public string Title { get; set; }
        public string RosterGroup { get; set; } 
        public double? Amount { get; set; }
        public string Unit { get; set; }
    }
}
