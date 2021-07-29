using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MHC_Hospital_Redesign.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        public string NameOnCard { get; set; }
        public string CardHash { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }

        [ForeignKey("Invoice")]
        public int InvoiceID { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}