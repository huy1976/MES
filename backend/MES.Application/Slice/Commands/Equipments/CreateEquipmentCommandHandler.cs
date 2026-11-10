namespace MesMicroservice.Application.Commands.Equipments;

public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, bool>
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IEquipmentClassRepository _equipmentClassRepository;
    private readonly IEnterpriseRepository _enterpriseRepository;

    public CreateEquipmentCommandHandler(IEquipmentRepository equipmentRepository, IEquipmentClassRepository equipmentClassRepository, IEnterpriseRepository enterpriseRepository)
    {
        _equipmentRepository = equipmentRepository;
        _equipmentClassRepository = equipmentClassRepository;
        _enterpriseRepository = enterpriseRepository;
    }

    public async Task<bool> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var properties = request.Properties.ConvertAll(x => new EquipmentProperty(
            x.PropertyId,
            x.Description,
            new Value(x.ValueString, x.ValueType),
            x.ValueUnitOfMeasure,
            x.Name))
;
        var equipmentClass = await _equipmentClassRepository.GetAsync(request.EquipmentClass) ?? throw new ResourceNotFoundException(nameof(EquipmentClass), request.EquipmentClass);
        var hierarchyModel = await GetHierarchyModelByAbsolutePath(request.AbsolutePath);
        var equipment = new Equipment(request.EquipmentId, request.Name, EEquipmentLevel.Equipment, request.EquipmentStatus ,properties, equipmentClass, hierarchyModel);

        await _equipmentRepository.Add(equipment);

        return await _equipmentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }

    private async Task<HierarchyModel?> GetHierarchyModelByAbsolutePath(string? absolutePath)
    {
        if (string.IsNullOrEmpty(absolutePath))
        {
            return null;
        }

        var hierarchyModelIds = absolutePath.Split('/');
        var enterprise = await _enterpriseRepository.GetAsync(hierarchyModelIds[0]) ?? throw new ResourceNotFoundException(nameof(Enterprise), hierarchyModelIds[0]);

        var hierarchyModel = new HierarchyModel();
        switch (hierarchyModelIds.Length)
        {
            case 1: hierarchyModel = enterprise;
            break;
            case 2: hierarchyModel = enterprise.Sites.FirstOrDefault(x => x.AbsolutePath == absolutePath) ?? throw new ResourceNotFoundException(nameof(Site), hierarchyModelIds[1]);
            break;
            case 3: hierarchyModel = enterprise.Sites
                    .SelectMany(x => x.Areas)
                    .FirstOrDefault(x => x.AbsolutePath == absolutePath) ?? throw new ResourceNotFoundException(nameof(Area), hierarchyModelIds[2]);
            break;
            case 4:
                hierarchyModel = enterprise.Sites
                .SelectMany(x => x.Areas)
                .SelectMany(x => x.WorkCenters)
                .FirstOrDefault(x => x.AbsolutePath == absolutePath) ?? throw new ResourceNotFoundException(nameof(WorkCenter), hierarchyModelIds[3]);
            break;
            case 5:
                hierarchyModel = enterprise.Sites
                .SelectMany(x => x.Areas)
                .SelectMany(x => x.WorkCenters)
                .SelectMany(x => x.WorkUnits)
                .FirstOrDefault(x => x.AbsolutePath == absolutePath) ?? throw new ResourceNotFoundException(nameof(WorkUnit), hierarchyModelIds[4]);
                break;
        }
        
        return hierarchyModel;
    }
}