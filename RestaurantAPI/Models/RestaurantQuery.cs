using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Models
{
    public class RestaurantQuery
    {
	    public string SearchedPhrase { get; set; }
	    public int PageNumber { get; set; }
	    public int PageRange { get; set; }
	    public string SortBy { get; set; }
	    public SortDirection SortDirection { get; set; }
    }
}
