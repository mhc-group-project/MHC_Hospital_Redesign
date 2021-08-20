using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHC_Hospital_Redesign.Models.ViewModels
{
    public class DetailsListing
    {
        public ListingDto SelectedListing { get; set; }
        public IEnumerable<ApplicationUserDto> AssignedUsers { get; set; }
        public IEnumerable<ApplicationUserDto> AvailableUsers { get; set; }
        public IEnumerable<DepartmentDto> Departments { get; set; }
    }
}