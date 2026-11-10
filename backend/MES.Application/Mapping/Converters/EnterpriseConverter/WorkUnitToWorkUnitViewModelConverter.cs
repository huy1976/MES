namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

public class WorkUnitToWorkUnitViewModelConverter : ITypeConverter<WorkUnit, WorkUnitViewModel>
{
    public WorkUnitViewModel Convert(WorkUnit source, WorkUnitViewModel destination, ResolutionContext context)
    {
        return new WorkUnitViewModel(source.HierarchyModelId, source.Name, source.AbsolutePath);
    }
}
