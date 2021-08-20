using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHC_Hospital_Redesign.Models.ViewModels
{
    public class UpdateListing
    {
        public ListingDto SelectedListing { get; set; }
        public IEnumerable<DepartmentDto> DeptOptions { get; set; }
    }
}