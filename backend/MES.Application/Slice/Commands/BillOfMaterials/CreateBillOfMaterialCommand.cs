namespace MesMicroservice.Application.Commands.BillOfMaterials
{
    [DataContract]
    public class CreateBillOfMaterialCommand : IRequest<bool>
    {
        [DataMember]
        public string BillOfMaterialId { get; set; }
        [DataMember]
        public string? Description { get; set; }
        [DataMember]
        public EMaterialUse? MaterialUse { get; set; }
        [DataMember]
        public EAssemblyType? AssemblyType { get; set; }
        [DataMember]
        public EAssemblyRelationship? AssemblyRelationship { get; set; }
        [DataMember]
        public string ValueString { get; set; }
        [DataMember]
        public EValueType ValueType { get; set; }
        [DataMember]
        public string ValueUnitOfMeasure { get; set; }
        [DataMember]
        public string MaterialDefinitionId { get; set; }

        public CreateBillOfMaterialCommand(string billOfMaterialId, string? description, EMaterialUse? materialUse, EAssemblyType? assemblyType, EAssemblyRelationship? assemblyRelationship, string valueString, EValueType valueType, string valueUnitOfMeasure, string materialDefinitionId)
        {
            BillOfMaterialId = billOfMaterialId;
            Description = description;
            MaterialUse = materialUse;
            AssemblyType = assemblyType;
            AssemblyRelationship = assemblyRelationship;
            ValueString = valueString;
            ValueType = valueType;
            ValueUnitOfMeasure = valueUnitOfMeasure;
            MaterialDefinitionId = materialDefinitionId;
        }
    }
}