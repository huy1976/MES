namespace MesMicroservice.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class MaterialDefinitionsController : ApiControllerBase
{
    public MaterialDefinitionsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("CreateMaterialDefinition")]
    public async Task<IActionResult> CreateMaterialDefinition([FromBody] CreateMaterialDefinitionCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPost("AddMaterialDefinitionProperties")]
    public async Task<IActionResult> AddMaterialDefinitionProperties([FromBody] AddMaterialDefinitionPropertiesCommand command)
    {
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{materialDefinitionId}")]
    public async Task<IActionResult> UpdateMaterialDefinition([FromRoute] string materialDefinitionId, [FromBody] UpdateMaterialDefinitionViewModel materialDefinition)
    {
        var command = new UpdateMaterialDefinitionCommand(materialDefinitionId, materialDefinition.Name, materialDefinition.PrimaryUnit, materialDefinition.Properties, materialDefinition.MaterialClass);
        return await RequestAsync(command);
    }


    [HttpGet("GetMaterialDefinitionIdsByFinishedProduct")]
    public async Task<IActionResult> GetMaterialDefinitionIdsByFinishedProduct()
    {
        var query = new MaterialDefinitionIdsQuery();
        return await RequestAsync(query);
    }

    [HttpGet("GetMaterialDefinitionById/{materialId}")]
    public async Task<IActionResult?> GetMaterialDefinitionById([FromRoute] string materialId)
    {
        var query = new MaterialDefinitionByIdQuery(materialId);
        return await RequestAsync(query);
    }

    [HttpGet("GetMaterialDefinitionByMaterialId/{materialId}")]
    public async Task<IActionResult?> GetMaterialDefinitionByMaterialId([FromRoute] string materialId)
    {
        var query = new MaterialDefinitionByMaterialIdQuery(materialId);
        return await RequestAsync(query);
    }


    [HttpGet("GetMaterialDefinitionPropertyNames")]
    public async Task<IActionResult> GetMaterialDefinitionPropertyNames()
    {
        var query = new MaterialDefinitionPropertyNamesQuery();
        return await RequestAsync(query);
    }

    [HttpGet("GetMaterialDefinitions")]
    public async Task<IActionResult> GetMaterialDefinitions([FromQuery] MaterialDefinitionsQuery query)
    {
        return await RequestAsync(query);
    }

    [HttpGet("GetMaterialDefinitionIds")]
    public async Task<IActionResult> GetMaterialDefinitionIds()
    {
        var query = new GetMaterialDefinitionIdsQuery();
        return await RequestAsync(query);
    }

    [HttpGet("GetMaterialDefinitionIdsByClassId/{classId}")]
    public async Task<IActionResult> GetMaterialDefinitionIdsByClassId([FromRoute] string classId = "NVL")
    {
        var query = new GetMaterialDefinitionIByClassIdQuery(classId);
        return await RequestAsync(query);
    }

    [HttpGet("GetMaterialUnitByMaterialId/{materialId}")]
    public async Task<IActionResult?> GetMaterialUnitByMaterialId([FromRoute] string materialId)
    {
        var query = new GetMaterialUnitByMaterialIdQuery { MaterialDefinitionId = materialId};

        return await RequestAsync(query);
    }


    [HttpDelete]
    [Route("{materialDefinitionId}")]
    public async Task<IActionResult> DeleteMaterialDefinition([FromRoute] string materialDefinitionId)
    {
        var command = new DeleteMaterialDefinitionCommand(materialDefinitionId);
        return await RequestAsync(command);
    }

    [HttpPost("CreateMaterialUnit")]
    public async Task<IActionResult> CreateMaterialUnit(string materialDefinitionId, [FromBody] CreateMaterialUnitViewModel materilUnit)
    {
        var command = new CreateMaterialUnitCommand(materialDefinitionId, materilUnit.UnitId, materilUnit.UnitName, materilUnit.ConversionValueToPrimaryUnit);
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{materialDefinitionId}/materialUnits/{materialUnitId}")]
    public async Task<IActionResult> UpdateMaterialUnit([FromRoute] string materialDefinitionId, [FromRoute] string materialUnitId, [FromBody] UpdateMaterialUnitViewModel materialUnit)
    {
        var command = new UpdateMaterialUnitCommand(materialDefinitionId, materialUnitId, materialUnit.UnitName, materialUnit.ConversionValueToPrimaryUnit);
        return await RequestAsync(command);
    }

    [HttpDelete]
    [Route("{materialDefinitionId}/materialUnits/{materialUnitId}")]
    public async Task<IActionResult> DeleteMaterialUnit([FromRoute] string materialDefinitionId, [FromRoute] string materialUnitId)
    {
        var command = new DeleteMaterialUnitCommand(materialDefinitionId, materialUnitId);
        return await RequestAsync(command);
    }

    [HttpPost]
    [Route("{materialDefinitionId}/operations")]
    public async Task<IActionResult> CreateOperation([FromRoute] string materialDefinitionId, [FromBody] CreateOperationViewModel operation)
    {
        var command = new CreateOperationCommand(materialDefinitionId, operation.OperationId, operation.Name, operation.PrerequisiteOperation);
        return await RequestAsync(command);
    }

    [HttpPut]
    [Route("{materialDefinitionId}/operations/{operationId}")]
    public async Task<IActionResult> UpdateOperation([FromRoute] string materialDefinitionId, [FromRoute] string operationId, [FromBody] UpdateOperationViewModel operation)
    {
        var command = new UpdateOperationCommand(materialDefinitionId, operationId, operation.Name, operation.PrerequisiteOperation);
        return await RequestAsync(command);
    }

    [HttpDelete]
    [Route("{materialDefinitionId}/operations/{operationId}")]
    public async Task<IActionResult> DeleteOperation([FromRoute] string materialDefinitionId, [FromRoute] string operationId)
    {
        var command = new DeleteOperationCommand(materialDefinitionId, operationId);
        return await RequestAsync(command);
    }
}
