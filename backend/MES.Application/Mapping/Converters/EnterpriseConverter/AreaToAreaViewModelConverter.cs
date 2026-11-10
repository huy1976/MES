namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

public class AreaToAreaViewModelConverter : ITypeConverter<Area, AreaViewModel>
{
    private readonly IMapper _mapper;

    public AreaToAreaViewModelConverter(IMapper mapper)
    {
        _mapper = mapper;
    }
    public AreaViewModel Convert(Area source, AreaViewModel destination, ResolutionContext context)
    {
        var workCenterViewModel = _mapper.Map<List<WorkCenterViewModel>>(source.WorkCenters);
        return new AreaViewModel(source.HierarchyModelId, source.Name, source.AbsolutePath, workCenterViewModel);
    }
}
