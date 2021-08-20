using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHC_Hospital_Redesign.Models.ViewModels
{
    public class ListListing
    {
        public bool IsAdmin { get; set; }
        public IEnumerable<ListingDto> Listings { get; set; }
    }
}