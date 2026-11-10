//namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

//public class WorkCenterToWorkCenterViewModelConverter : ITypeConverter<WorkCenter, WorkCenterViewModel>
//{
//    private readonly IMapper _mapper;

//    public WorkCenterToWorkCenterViewModelConverter(IMapper mapper)
//    {
//        _mapper = mapper;
//    }

//    public WorkCenterViewModel Convert(WorkCenter source, WorkCenterViewModel destination, ResolutionContext context)
//    {
//        var workUnitViewModel = _mapper.Map<List<WorkUnitViewModel>>(source.WorkUnits);
//        return new WorkCenterViewModel(source.HierarchyModelId, source.Parent.HierarchyModelId, source.Name, source.AbsolutePath, source.WorkCenterType, workUnitViewModel);
//    }
//}
