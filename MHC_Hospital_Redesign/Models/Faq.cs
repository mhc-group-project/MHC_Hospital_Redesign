using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MHC_Hospital_Redesign.Models
{
    public class Faq
    {
        [Key]
        public int FaqID { get; set; }

        public DateTime DateAdded { get; set; }

        public string FaqQuestions { get; set; }

        public string FaqAnswers { get; set; }

        public int FaqSort { get; set; }

        public ICollection<FaqCategory> FaqCategories { get; set; }

    }

    public class FaqDto
    {
        public int FaqID { get; set; }

        public DateTime DateAdded { get; set; }

        public string FaqQuestions { get; set; }

        public string FaqAnswers { get; set; }

        public int FaqSort { get; set; }
    }
}