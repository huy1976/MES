namespace MesMicroservice.Application.Mapping.Converters.EnterpriseConverter;

public class SiteToSiteViewModelConverter : ITypeConverter<Site, SiteViewModel>
{
    private readonly IMapper _mapper;

    public SiteToSiteViewModelConverter(IMapper mapper)
    {
        _mapper = mapper;
    }

    public SiteViewModel Convert(Site source, SiteViewModel destination, ResolutionContext context)
    {
        var areaViewModel = _mapper.Map<List<AreaViewModel>>(source.Areas);
        return new SiteViewModel(source.HierarchyModelId, source.Name, source.AbsolutePath, areaViewModel);
    }
}
