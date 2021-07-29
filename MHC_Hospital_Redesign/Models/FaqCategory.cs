using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MHC_Hospital_Redesign.Models
{
    public class FaqCategory
    {
        [Key]
        public int FaqCategoryID { get; set; }

        public DateTime CategoryDateAdded { get; set; }

        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

        public string CategoryColor { get; set; }

        public ICollection<Faq> Faqs { get; set; }
    }

    public class FaqCategoryDto
    {
        public int FaqCategoryID { get; set; }

        public DateTime CategoryDateAdded { get; set; }

        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

        public string CategoryColor { get; set; }

    }
}