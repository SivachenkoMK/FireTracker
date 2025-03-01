using Microsoft.AspNetCore.Mvc.RazorPages;
using FireTracker.Core.DTOs;
using FireTracker.Core.Persistence;

namespace FireTracker.Core.Pages;

public class IndexModel : PageModel
{
    public IEnumerable<IncidentModel> Incidents { get; set; } = Enumerable.Empty<IncidentModel>();

    public void OnGet()
    {
        Incidents = DbContext.IncidentCollection.Values.ToList();
    }
}