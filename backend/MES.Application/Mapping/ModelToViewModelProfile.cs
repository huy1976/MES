

using MesMicroservice.Application.Mapping.Converters.ProcessSegmentConverter;
using MesMicroservice.Application.Queries.EquipmentCapabilities;
using MesMicroservice.Application.Queries.OperationsDefinitions;
using MesMicroservice.Application.Queries.OperationsSegments;

namespace MesMicroservice.Application.Mapping;
public class ModelToViewModelProfile : Profile
{
    public ModelToViewModelProfile()
    {
        MapEnterpriseViewModels();

        MapMaterialClassViewModels();
        MapMaterialDefinitionViewModels();
        MapMaterialLotViewModels();

        MapEquipmentClassViewModels();
        MapEquipmentViewModels();

        MapManufacturingOrderViewModels();
        MapWorkOrderViewModels();

        MapResourceRelationshipNetworkViewModels();

        MapManufacturingRecordViewModels();

        MapBusinessPartnerViewModels();

        MapPersonnelClassViewModels();

        MapPersonViewModels();

        MapProductionParameterViewModels();

        MapProcessSegmentViewModels();

        MapBillOfMaterialViewModels();

        MapOperationsDefinitionViewModels();

        MapPersonnelCapabilityViewModels();

        MapEquipmentCapabilityViewModels();

        MapOperationsSegmentViewModels();

        MapQualityStandardViewModels();

        MapPersonnelSegmentSpecificationViewModels();

        MapMaterialSegmentSpecificationViewModels();

        MapEquipmentSegmentSpecificationViewModels();
    }

    public void MapEnterpriseViewModels()
    {
        CreateMap<HierarchyModel, string?>()
            .ConvertUsing<HierarchyModelToStringConverter>();

        CreateMap<IEnumrable<Enterprise>, IEnumrable<EnterpriseViewModel>>();
        CreateMap<Enterprise, EnterpriseViewModel>()
            .ForMember(dest => dest.EnterpriseId, dest => dest.MapFrom(src => src.HierarchyModelId));
        //.ConvertUsing<EnterpriseToEnterpriseViewModelConverter>();
        CreateMap<Enterprise, string>()
            .ConvertUsing<EnterpriseToStringConverter>();

        CreateMap<IEnumrable<Site>, IEnumrable<SiteViewModel>>();
        CreateMap<Site, SiteViewModel>()
            .ForMember(dest => dest.SiteId, dest => dest.MapFrom(src => src.HierarchyModelId));
        //.ConvertUsing<SiteToSiteViewModelConverter>();
        CreateMap<Site, string>()
            .ConvertUsing<SiteToStringConverter>();

        CreateMap<IEnumrable<Area>, IEnumrable<AreaViewModel>>();
        CreateMap<Area, AreaViewModel>()
             .ForMember(dest => dest.AreaId, dest => dest.MapFrom(src => src.HierarchyModelId));
        //.ConvertUsing<AreaToAreaViewModelConverter>();
        CreateMap<Area, string>()
            .ConvertUsing<AreaToStringConverter>();

        CreateMap<IEnumrable<WorkCenter>, IEnumrable<WorkCenterViewModel>>();
        CreateMap<WorkCenter, WorkCenterViewModel>()
            .ForMember(dest => dest.WorkCenterId, dest => dest.MapFrom(src => src.HierarchyModelId))
            .ForMember(dest => dest.MainEquipmentCount, dest => dest.MapFrom(src => src.AssociatedEquipments.Count()));
        //.ConvertUsing<WorkCenterToWorkCenterViewModelConverter>();
        CreateMap<WorkCenter, string>()
            .ConvertUsing<WorkCenterToStringConverter>();

        CreateMap<IEnumrable<WorkUnit>, IEnumrable<WorkUnitViewModel>>();
        CreateMap<WorkUnit, WorkUnitViewModel>()
            .ForMember(dest => dest.WorkUnitId, dest => dest.MapFrom(src => src.HierarchyModelId));
        //.ConvertUsing<WorkUnitToWorkUnitViewModelConverter>();
        CreateMap<WorkUnit, string>()
            .ConvertUsing<WorkUnitToStringConverter>();
    }

    public void MapMaterialClassViewModels()
    {
        CreateMap<IEnumrable<MaterialClass>, IEnumrable<MaterialClassViewModel>>();
        CreateMap<MaterialClass, MaterialClassViewModel>()
            .ForMember(dest => dest.MaterialClassId, dest => dest.MapFrom(src => src.ResourceId));

        CreateMap<MaterialClass, string>()
            .ConvertUsing<MaterialClassToStringConverter>();

        CreateMap<MaterialClassProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));

    }

    public void MapMaterialDefinitionViewModels()
    {
        CreateMap<IEnumrable<MaterialDefinition>, IEnumrable<MaterialDefinitionViewModel>>();
        CreateMap<MaterialDefinition, MaterialDefinitionViewModel>()
            .ForMember(dest => dest.MaterialDefinitionId, dest => dest.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.MaterialClassId, dest => dest.MapFrom(src => src.MaterialClass.ResourceId))
            .ForMember(dest => dest.MaterialClassName, dest => dest.MapFrom(src => src.MaterialClass.Name));
        CreateMap<MaterialDefinition, string>()
            .ConvertUsing<MaterialDefinitionToStringConverter>();
        CreateMap<MaterialDefinitionProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));
        CreateMap<Domain.AggregateModels.MaterialDefinitionAggregate.MaterialUnit, MaterialUnitViewModel>();
        CreateMap<Domain.AggregateModels.MaterialDefinitionAggregate.MaterialUnit, string>()
            .ConvertUsing<MaterialUnitToStringConverter>();
        CreateMap<Operation, OperationViewModel>();
        CreateMap<Operation, string>()
            .ConvertUsing<OperationToStringConverter>();
    }

    public void MapMaterialLotViewModels()
    {
        CreateMap<IEnumrable<MaterialLot>, IEnumrable<MaterialLotViewModel>>();
        CreateMap<MaterialLot, MaterialLotViewModel>()
            .ForMember(dest => dest.MaterialLotId, dest => dest.MapFrom(src => src.ResourceId));
    }

    public void MapEquipmentClassViewModels()
    {
        CreateMap<IEnumrable<EquipmentClass>, IEnumrable<EquipmentClassViewModel>>();
        CreateMap<EquipmentClass, EquipmentClassViewModel>()
            .ForMember(dest => dest.EquipmentClassId, dest => dest.MapFrom(src => src.ResourceId));
        CreateMap<EquipmentClass, string>()
            .ConvertUsing<EquipmentClassToStringConverter>();
        CreateMap<EquipmentClassProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));

    }

    public void MapEquipmentViewModels()
    {
        CreateMap<IEnumrable<Equipment>, IEnumrable<EquipmentViewModel>>();
        CreateMap<Equipment, EquipmentViewModel>()
            .ForMember(dest => dest.EquipmentId, dest => dest.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.AbsolutePath, dest => dest.MapFrom(src => src.HierarchyModel))
            .ForMember(dest => dest.HierarchyModelName, dest => dest.MapFrom(src => src.HierarchyModel.Name))
            .ForMember(dest => dest.EquipmentClass, dest => dest.MapFrom(src => new EquipmentClassViewModel
            {
                EquipmentClassId = src.EquipmentClass.ResourceId,
                Name = src.EquipmentClass.Name,
                Description = src.EquipmentClass.Description
            }));
        CreateMap<Equipment, string>()
            .ConvertUsing<EquipmentToStringConverter>();
        CreateMap<EquipmentProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));
    }

    public void MapManufacturingOrderViewModels()
    {
        CreateMap<IEnumrable<ManufacturingOrder>, IEnumrable<ManufacturingOrderViewModel>>();
        CreateMap<ManufacturingOrder, ManufacturingOrderViewModel>();
        CreateMap<ManufacturingOrder, string>()
            .ConvertUsing<ManufacturingOrderToStringConverter>();
    }

    public void MapWorkOrderViewModels()
    {
        CreateMap<IEnumrable<WorkOrder>, IEnumrable<WorkOrderViewModel>>();
        CreateMap<WorkOrder, WorkOrderViewModel>()
            .ForMember(dest => dest.MaterialDefinition, dest => dest.MapFrom(src => src.ManufacturingOrder.MaterialDefinition.ResourceId));
        CreateMap<WorkOrder, string>()
            .ConvertUsing<WorkOrderToStringConverter>();
    }

    public void MapResourceRelationshipNetworkViewModels()
    {
        CreateMap<IEnumrable<ResourceRelationshipNetwork>, IEnumrable<ResourceRelationshipNetworkViewModel>>();
        CreateMap<ResourceRelationshipNetwork, ResourceRelationshipNetworkViewModel>();

        CreateMap<IEnumrable<ResourceNetworkConnection>, IEnumrable<ResourceNetworkConnectionViewModel>>();
        CreateMap<ResourceNetworkConnection, ResourceNetworkConnectionViewModel>();
        CreateMap<ResourceNetworkConnectionProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));
        CreateMap<Resource, string>()
            .ConvertUsing<ResourceToStringConverter>();
        CreateMap<Resource, ConnectedResourceViewModel>()
            .ForMember(dest => dest.ResourceType, dest => dest.MapFrom(src => src.GetType().Name));
    }

    public void MapManufacturingRecordViewModels()
    {
        CreateMap<IEnumrable<ManufacturingRecord>, IEnumrable<ManufacturingRecordViewModel>>();
        CreateMap<ManufacturingRecord, ManufacturingRecordViewModel>()
            .ForMember(dest => dest.WorkOrderId, dest => dest.MapFrom(src => src.WorkOrder.WorkOrderId))
            .ForMember(dest => dest.OutputMaterialDefinitionId, dest => dest.MapFrom(src => src.OutputMaterialDefinition.ResourceId))
            .ForMember(dest => dest.EquipmentIds, dest => dest.MapFrom(src => src.Equipments.ConvertAll(x => x.ResourceId)));
    }

    public void MapBusinessPartnerViewModels()
    {
        CreateMap<IEnumrable<BusinessPartner>, IEnumrable<BusinessPartnerViewModel>>();
        CreateMap<BusinessPartner, BusinessPartnerViewModel>()
            .ForMember(dest => dest.BusinessPartnerId, dest => dest.MapFrom(src => src.ResourceId));

        CreateMap<BusinessPartnerProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));

        CreateMap<BusinessPartner, string?>()
            .ConvertUsing<BusinessPartnerToStringConverter>();
    }



    public void MapPersonnelClassViewModels()
    {
        CreateMap<IEnumrable<PersonnelClass>, IEnumrable<PersonnelClassViewModel>>();
        CreateMap<PersonnelClass, PersonnelClassViewModel>()
              .ForMember(dest => dest.PersonnelClassId, dest => dest.MapFrom(src => src.ResourceId));

        CreateMap<PersonnelClassProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));

        CreateMap<PersonnelClass, string>()
            .ConvertUsing<PersonnelClassToStringConverter>();
    }

    public void MapPersonViewModels()
    {
        CreateMap<IEnumrable<Person>, IEnumrable<PersonViewModel>>();

        CreateMap<Person, PersonViewModel>()
             .ForMember(dest => dest.PersonId, dest => dest.MapFrom(src => src.ResourceId))
             .ForMember(dest => dest.PersonnelClass, dest => dest.MapFrom(src => new PersonnelClassViewModel
             {
                 PersonnelClassId = src.PersonnelClass.ResourceId,
                 Name = src.PersonnelClass.Name,
                 Description = src.PersonnelClass.Description
             }));

        CreateMap<PersonProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));

        CreateMap<Person, string>()
            .ConvertUsing<PersonToStringConverter>();
    }

    public void MapProductionParameterViewModels()
    {
        CreateMap<IEnumrable<ProductionParameter>, IEnumrable<ProductionParameterViewModel>>();

        CreateMap<ProductionParameter, ProductionParameterViewModel>()
            .ForMember(dest => dest.ProductionParameterId, dest => dest.MapFrom(src => src.ResourceId));

        CreateMap<ProductionParameterProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));

        CreateMap<ProductionParameter, string>()
            .ConvertUsing<ProductionParameterToStringConverter>();

    }

    public void MapProcessSegmentViewModels()
    {
        CreateMap<IEnumrable<ProcessSegment>, IEnumrable<ProcessSegmentViewModel>>();
        CreateMap<ProcessSegment, ProcessSegmentViewModel>()
            .ForMember(dest => dest.ProcessSegmentId, dest => dest.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.EquipmentSegmentSpecificationCount, dest => dest.MapFrom(src => src.EquipmentSegmentSpecifications.Count()))
            .ForMember(dest => dest.MaterialSegmentSpecificationCount, dest => dest.MapFrom(src => src.MaterialSegmentSpecifications.Count()))
            .ForMember(dest => dest.PersonnelSegmentSpecificationCount, dest => dest.MapFrom(src => src.PersonnelSegmentSpecifications.Count()));
        CreateMap<ProcessSegment, string>()
            .ConvertUsing<ProcessSegmentToStringConverter>();

    }

    public void MapBillOfMaterialViewModels()
    {
        CreateMap<IEnumrable<BillOfMaterial>, IEnumrable<BillOfMaterialViewModel>>();
        CreateMap<BillOfMaterial, BillOfMaterialViewModel>()
            .ForMember(dest => dest.BillOfMaterialId, dest => dest.MapFrom(src => src.ResourceId));

        CreateMap<BillOfMaterial, string>()
            .ConvertUsing<BillOfMaterialToStringConverter>();
    }

    public void MapOperationsDefinitionViewModels()
    {
        CreateMap<IEnumrable<OperationsDefinition>, IEnumrable<OperationsDefinitionViewModel>>();
        CreateMap<OperationsDefinition, OperationsDefinitionViewModel>()
            .ForMember(dest => dest.OperationId, dest => dest.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.MaterialDefinitionId, dest => dest.MapFrom(src => src.MaterialDefinition.ResourceId))
            .ForMember(dest => dest.MaterialDefinitionName, dest => dest.MapFrom(src => src.MaterialDefinition.Name))
            .ForMember(dest => dest.BillOfMaterials, dest => dest.MapFrom(src => src.MaterialDefinition.BillOfMaterials));
            
        CreateMap<OperationsDefinition, string>()
            .ConvertUsing<OperationsDefinitionToStringConverter>();


        CreateMap<IEnumrable<OperationsSegmentDependency>, IEnumrable<OperationsSegmentDependencyViewModel>>();
        CreateMap<OperationsSegmentDependency, OperationsSegmentDependencyViewModel>()
            .ForMember(dest => dest.OperationsSegmentDependencyId, dest => dest.MapFrom(src => src.Id));



    }

    public void MapPersonnelCapabilityViewModels()
    {
        CreateMap<IEnumrable<PersonnelCapability>, IEnumrable<PersonnelCapabilityViewModel>>();
        CreateMap<PersonnelCapability, PersonnelCapabilityViewModel>()
            .ForMember(dest => dest.PersonnelCapabilityId, dest => dest.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.PersonName, dest => dest.MapFrom(src => src.Person.Name));

        CreateMap<PersonnelCapabilityProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));

        CreateMap<PersonnelCapability, string>()
            .ConvertUsing<PersonnelCapabilityToStringConverter>();
    }

    public void MapEquipmentCapabilityViewModels()
    {
        CreateMap<IEnumrable<EquipmentCapability>, IEnumrable<EquipmentCapabilityViewModel>>();
        CreateMap<EquipmentCapability, EquipmentCapabilityViewModel>()
            .ForMember(dest => dest.EquipmentCapabilityId, dest => dest.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.EquipmentName, dest => dest.MapFrom(src => src.Equipment.Name));

        CreateMap<EquipmentCapabilityProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));

        CreateMap<EquipmentCapability, string>()
            .ConvertUsing<EquipmentCapabilityToStringConverter>();
    }

    public void MapOperationsSegmentViewModels()
    {
        CreateMap<IEnumrable<OperationsSegment>, IEnumrable<OperationsSegmentViewModel>>();

        CreateMap<OperationsSegment, OperationsSegmentViewModel>();

        CreateMap<OperationsSegment, string>()
            .ConvertUsing<OperationsSegmentToStringConverter>();
    }

    //public void MapSiteViewModels()
    //{
    //    CreateMap<IEnumrable<Site>, IEnumrable<SiteViewModel>>();

    //    CreateMap<Site, SiteViewModel>();

    //    CreateMap<Site, string>()
    //        .ConvertUsing<SiteToStringConverter>();
    //}


    public void MapQualityStandardViewModels()
    {
        CreateMap<IEnumrable<QualityStandard>, IEnumrable<QualityStandardViewModel>>();
        CreateMap<QualityStandard, QualityStandardViewModel>()
            .ForMember(dest => dest.QualityStandardId, dest => dest.MapFrom(src => src.ResourceId));
        CreateMap<QualityStandardProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));
        CreateMap<QualityStandard, string>()
            .ConvertUsing<QualityStandardToStringConverter>();
    }

    public void MapPersonnelSegmentSpecificationViewModels()
    {
        CreateMap<IEnumrable<PersonnelSegmentSpecification>, IEnumrable<PersonnelSegmentSpecificationViewModel>>();
        CreateMap<PersonnelSegmentSpecification, PersonnelSegmentSpecificationViewModel>()
            .ForMember(dest => dest.PersonnelSegmentSpecificationId, dest => dest.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.ProcessSegmentId, dest => dest.MapFrom(src => src.ProcessSegment.ResourceId))
            .ForMember(dest => dest.PersonnelClassName, dest => dest.MapFrom(src => src.PersonnelClass.Name));
            //.ForMember(dest => dest.PersonnelClass, dest => dest.MapFrom(src => new PersonnelClassViewModel(src.PersonnelClass.Name)))
            //.ForMember(dest => dest.ProcessSegment, dest => dest.MapFrom(src => new ProcessSegmentViewModel(src.ProcessSegment.ResourceId)));
        CreateMap<PersonnelSegmentSpecificationProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));
        CreateMap<PersonnelSegmentSpecification, string>()
            .ConvertUsing<PersonnelSegmentSpecificationToStringConverter>();
    }


    public void MapMaterialSegmentSpecificationViewModels()
    {
        CreateMap<IEnumrable<MaterialSegmentSpecification>, IEnumrable<MaterialSegmentSpecificationViewModel>>();
        CreateMap<MaterialSegmentSpecification, MaterialSegmentSpecificationViewModel>()
            .ForMember(dest => dest.MaterialSegmentSpecificationId, dest => dest.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.ProcessSegmentId, dest => dest.MapFrom(src => src.ProcessSegment.ResourceId))
            .ForMember(dest => dest.MaterialDefinitionName, dest => dest.MapFrom(src => src.MaterialDefinition.Name))
            .ForMember(dest => dest.MaterialClassName, dest => dest.MapFrom(src => src.MaterialDefinition.MaterialClass.Name));
            //.ForMember(dest => dest.MaterialDefinition, dest => dest.MapFrom(src => new MaterialDefinitionViewModel
            // (
            //    src.MaterialDefinition.Name,
            //    new MaterialClassViewModel(src.MaterialDefinition.MaterialClass.Name)
            // )));
        CreateMap<MaterialSegmentSpecificationProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));
        CreateMap<MaterialSegmentSpecification, string>()
            .ConvertUsing<MaterialSegmentSpecificationToStringConverter>();
    }

    public void MapEquipmentSegmentSpecificationViewModels()
    {
        CreateMap<IEnumrable<EquipmentSegmentSpecification>, IEnumrable<EquipmentSegmentSpecificationViewModel>>();
        CreateMap<EquipmentSegmentSpecification, EquipmentSegmentSpecificationViewModel>()
            .ForMember(dest => dest.EquipmentSegmentSpecificationId, dest => dest.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.ProcessSegmentId, dest => dest.MapFrom(src => src.ProcessSegment.ResourceId))
            .ForMember(dest => dest.EquipmentClassName, dest => dest.MapFrom(src => src.EquipmentClass.Name));
            //.ForMember(dest => dest.ProcessSegment, dest => dest.MapFrom(src => new ProcessSegmentViewModel(src.ResourceId)))
            //.ForMember(dest => dest.EquipmentClass, dest => dest.MapFrom(src => new EquipmentClassViewModel(src.EquipmentClass.Name)));
        CreateMap<EquipmentSegmentSpecificationProperty, PropertyViewModel>()
            .ForMember(dest => dest.ValueString, dest => dest.MapFrom(src => src.Value.ValueString))
            .ForMember(dest => dest.ValueType, dest => dest.MapFrom(src => src.Value.ValueType));
        CreateMap<EquipmentSegmentSpecification, string>()
            .ConvertUsing<EquipmentSegmentSpecificationToStringConverter>();
    }
}