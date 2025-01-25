using FireTracker.Api.DTOs;
using FireTracker.Api.Services;
using FireTracker.Api.Validators;
using FireTracker.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FireTracker.Api.Controllers
{
    [ApiController]
    [Route("api/track")]
    public class TrackerController : ControllerBase
    {
        private readonly RoutingService _routingService;
        private readonly ILogger<TrackerController> _logger;

        public TrackerController(RoutingService routingService, ILogger<TrackerController> logger)
        {
            _routingService = routingService;
            _logger = logger;
        }

        /// <summary>
        /// Receives GIS data + Guid
        /// </summary>
        [HttpPost("gis")]
        public async Task<IActionResult> AddGis([FromBody] GisRequest request, CancellationToken token)
        {
            var errorOrNull = await this.ErrorOrNull(request, _logger, new GisValidator(), token);
            
            if (errorOrNull != null)
                return errorOrNull;
            
            await _routingService.SendGisRequest(request, token);

            return Ok();
        }

        /// <summary>
        /// Receives Photo + SessionGuid
        /// </summary>
        [HttpPost("photo")]
        public async Task<IActionResult> AddPhoto([FromForm] PhotoRequest request, CancellationToken token)
        {
            var errorOrNull = await this.ErrorOrNull(request, _logger, new PhotoValidator(), token);
            
            if (errorOrNull != null)
                return errorOrNull;

            // 
            await _routingService.SendPhotoRequest(request, token);
            
            return Ok();
        }

        /// <summary>
        /// Receives Location (String) + Guid
        /// </summary>
        [HttpPost("location")]
        public async Task<IActionResult> AddLocation([FromBody] LocationRequest request, CancellationToken token)
        {
            var errorOrNull = await this.ErrorOrNull(request, _logger, new LocationValidator(), token);
            
            if (errorOrNull != null)
                return errorOrNull;
            
            await _routingService.SendRelationalLocation(request, token);

            return Ok();
        }
    }
}