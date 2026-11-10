namespace MesMicroservice.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EnterprisesController : ApiControllerBase
{
    public EnterprisesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> CreateEnterprise([FromBody] CreateEnterpriseCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{enterpriseId}")]
    public async Task<IActionResult> UpdateEnterprise([FromRoute] string enterpriseId, [FromBody] UpdateEnterpriseViewModel enterprise)
    {
        var command = new UpdateEnterpriseCommand(enterpriseId, enterprise.EnterpriseId, enterprise.Name);
        return await RequestAsync(command);
    }

    [HttpGet("GetEnterprises")]
    public async Task<IActionResult> GetEnterprises([FromQuery] EnterprisesQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetSites")]
    public async Task<IActionResult> GetSites([FromQuery] SitesQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetSiteIds")]
    public async Task<IActionResult> GetSiteIds()
    {
        var query = new SiteIdsQuery();
        return await RequestAsync(query);
    }

    [HttpGet("GetAreaIds")]
    public async Task<IActionResult> GetAreaIds()
    {
        var query = new AreaIdsQuery();
        return await RequestAsync(query);
    }

    [HttpDelete]
    [Route("{enterpriseId}")]
    public async Task<IActionResult> DeteteEnterprise([FromRoute] string enterpriseId)
    {
        var command = new DeleteEnterpriseCommand(enterpriseId);
        return await RequestAsync(command);
    }

    [HttpPost]
    [Route("{enterpriseId}")]
    public async Task<IActionResult> CreateSite([FromRoute] string enterpriseId, [FromBody] CreateSiteViewModel site)
    {
        var command = new CreateSiteCommand(enterpriseId, site.SiteId, site.Name, site.Areas);
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{enterpriseId}/{siteId}")]
    public async Task<IActionResult> UpdateSite([FromRoute] string enterpriseId, [FromRoute] string siteId, [FromBody] UpdateSiteViewModel site)
    {
        var command = new UpdateSiteCommand(enterpriseId, siteId, site.SiteId, site.Name);
        return await RequestAsync(command);
    }

    [HttpDelete]
    [Route("{enterpriseId}/{siteId}")]
    public async Task<IActionResult> DeleteSite([FromRoute] string enterpriseId, [FromRoute] string siteId)
    {
        var command = new DeleteSiteCommand(enterpriseId, siteId); 
        return await RequestAsync(command);
    }

    [HttpPost]
    [Route("{enterpriseId}/{siteId}")]
    public async Task<IActionResult> CreateArea([FromRoute] string enterpriseId, [FromRoute] string siteId, [FromBody] CreateAreaViewModel area)
    {
        var command = new CreateAreaCommand(enterpriseId, siteId, area.AreaId, area.Name);
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{enterpriseId}/{siteId}/{areaId}")]
    public async Task<IActionResult> UpdateArea([FromRoute] string enterpriseId, [FromRoute] string siteId, [FromRoute] string areaId, [FromBody] UpdateAreaViewModel area)
    {
        var command = new UpdateAreaCommand(enterpriseId, siteId, areaId, area.AreaId, area.Name);
        return await RequestAsync(command);
    }

    [HttpDelete]
    [Route("{enterpriseId}/{siteId}/{areaId}")]
    public async Task<IActionResult> DeleteArea([FromRoute] string enterpriseId, [FromRoute] string siteId, [FromRoute] string areaId)
    {
        var command = new DeleteAreaCommand(enterpriseId, siteId, areaId);
        return await RequestAsync(command);
    }

    [HttpPost]
    [Route("{enterpriseId}/{siteId}/{areaId}")]
    public async Task<IActionResult> CreateWorkCenter([FromRoute] string enterpriseId, [FromRoute] string siteId, [FromRoute] string areaId, [FromBody] CreateWorkCenterViewModel workCenter)
    {
        var command = new CreateWorkCenterCommand(enterpriseId, siteId, areaId, workCenter.WorkCenterId, workCenter.Name, workCenter.WorkCenterType, workCenter.WorkUnits);
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{enterpriseId}/{siteId}/{areaId}/{workCenterId}")]
    public async Task<IActionResult> UpdateWorkCenter([FromRoute] string enterpriseId, [FromRoute] string siteId, [FromRoute] string areaId, [FromRoute] string workCenterId, [FromBody] UpdateWorkCenterViewModel workCenter)
    {
        var command = new UpdateWorkCenterCommand(enterpriseId, siteId, areaId, workCenterId, workCenter.WorkCenterId, workCenter.Name, workCenter.WorkCenterType);
        return await RequestAsync(command);
    }

    [HttpDelete]
    [Route("{enterpriseId}/{siteId}/{areaId}/{workCenterId}")]
    public async Task<IActionResult> DeleteWorkCenter([FromRoute] string enterpriseId, [FromRoute] string siteId, [FromRoute] string areaId, [FromRoute] string workCenterId)
    {
        var command = new DeleteWorkCenterCommand(enterpriseId, siteId, areaId, workCenterId);
        return await RequestAsync(command);
    }

    [HttpPost]
    [Route("{enterpriseId}/{siteId}/{areaId}/{workCenterId}")]
    public async Task<IActionResult> CreateWorkUnit([FromRoute] string enterpriseId, [FromRoute] string siteId, [FromRoute] string areaId, [FromRoute] string workCenterId, [FromBody] CreateWorkUnitViewModel workUnit)
    {
        var command = new CreateWorkUnitCommand(enterpriseId, siteId, areaId, workCenterId, workUnit.WorkUnitId, workUnit.Name);
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{enterpriseId}/{siteId}/{areaId}/{workCenterId}/{workUnitId}")]
    public async Task<IActionResult> UpdateWokUnit([FromRoute] string enterpriseId, [FromRoute] string siteId, [FromRoute] string areaId, [FromRoute] string workCenterId, [FromRoute] string workUnitId, [FromBody] UpdateWorkUnitViewModel workUnit)
    {
        var command = new UpdateWorkUnitCommand(enterpriseId, siteId, areaId, workCenterId, workUnitId, workUnit.WorkUnitId, workUnit.Name);
        return await RequestAsync(command);
    }

    [HttpDelete]
    [Route("{enterpriseId}/{siteId}/{areaId}/{workCenterId}/{workUnitId}")]
    public async Task<IActionResult> DeleteWorkUnit([FromRoute] string enterpriseId, [FromRoute] string siteId, [FromRoute] string areaId, [FromRoute] string workCenterId, [FromRoute] string workUnitId)
    {
        var command = new DeleteWorkUnitCommand(enterpriseId, siteId, areaId, workCenterId, workUnitId);
        return await RequestAsync(command);
    }

    [HttpGet]
    [Route("Sites/Areas/WorkCenters/WorkUnit")]
    public async Task<IActionResult> GetWorkUnits([FromQuery] WorkUnitsQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetEnterpriseIds")]

    public async Task<IActionResult> GetEnterpriseIds()
    {
        var query = new EnterpriseIdsQuery();
        return await RequestAsync(query);
    }


}
