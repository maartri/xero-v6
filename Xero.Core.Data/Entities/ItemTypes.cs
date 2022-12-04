using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Xero.Core.Data.Entities
{
    public class ItemTypes
    {
        [Key]
        public int Recnum { get; set; }
        public string? Title { get; set; }
        public string? AccountingIdentifier { get; set; }
        public string? ExtPayID { get; set; }
        public string? RosterGroup { get; set; }
        public double? Amount { get; set; }
        public string? Unit { get; set; }
        public bool? DeletedRecord { get; set; }
    }
}
