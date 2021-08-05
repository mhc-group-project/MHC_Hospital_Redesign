﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MHC_Hospital_Redesign.Models
{
    public class Department
    {
        [Key]
        public int DId { get; set; }
        [Required]
        public string DepartmentName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        //A department can have many job list
        public ICollection<Listing> ListID { get; set; }
    }
    public class DepartmentDto
    {

        public int DId { get; set; }
        [Required]
        public string DepartmentName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}