namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

public class EnterpriseToEnterpriseViewModelConverter : ITypeConverter<Enterprise, EnterpriseViewModel>
{
    private readonly IMapper _mapper;

    public EnterpriseToEnterpriseViewModelConverter(IMapper mapper)
    {
        _mapper = mapper;
    }

    public EnterpriseViewModel Convert(Enterprise source, EnterpriseViewModel destination, ResolutionContext context)
    {
        var siteViewModel = _mapper.Map<List<SiteViewModel>>(source.Sites);
        return new EnterpriseViewModel(source.HierarchyModelId, source.Name, source.AbsolutePath, siteViewModel);
    }
}
