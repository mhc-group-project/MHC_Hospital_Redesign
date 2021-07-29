using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace MHC_Hospital_Redesign.Models
{
    public class Appointment
    {
        [Key]
        public int AId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string DateTime { get; set; }
        public string Status { get; set; }

        [DisplayName("Patient")]
        [ForeignKey("PatientUser")]
        public string PatientId { get; set; }
        public virtual ApplicationUser PatientUser { get; set; }

        [DisplayName("Doctor")]
        [ForeignKey("DoctorUser")]
        public string DoctorId { get; set; }
        public virtual ApplicationUser DoctorUser { get; set; }

    }
}