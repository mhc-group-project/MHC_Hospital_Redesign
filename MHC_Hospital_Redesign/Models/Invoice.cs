using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MHC_Hospital_Redesign.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
    }
}