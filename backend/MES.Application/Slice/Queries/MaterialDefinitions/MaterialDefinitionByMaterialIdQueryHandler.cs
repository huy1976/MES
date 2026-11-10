namespace MesMicroservice.Application.Queries.MaterialDefinitions
{
    public class MaterialDefinitionByMaterialIdQueryHandler : IRequestHandler<MaterialDefinitionByMaterialIdQuery, MaterialDTO>
    {
        private readonly IMaterialDefinitionRepository _materialDefinitionRepository;

        public MaterialDefinitionByMaterialIdQueryHandler(IMaterialDefinitionRepository materialDefinitionRepository)
        {
            _materialDefinitionRepository = materialDefinitionRepository;
        }

        public async Task<MaterialDTO> Handle(MaterialDefinitionByMaterialIdQuery request, CancellationToken cancellationToken)
        {
            var materialDefinition = await _materialDefinitionRepository.GetMaterialByIdAsync(request.MaterialId);
            if (materialDefinition is null)
            {
                throw new ResourceNotFoundException(nameof(MaterialDefinition), request.MaterialId);
            }

            var materialDTO = new MaterialDTO(
                materialDefinition.ResourceId,
                materialDefinition.Name,
                materialDefinition.PrimaryUnit
            );

            var ops = new List<OperationDTO>();
            foreach (var op in materialDefinition.OperationsDefinitions)
            {
                var segments = new List<SegmentDTO>();
                var bills = new List<BomDTO>();
                var operationDTO = new OperationDTO
                {
                    OperationId = op.ResourceId,
                    Description = op.Description ?? "--",
                };
                foreach (var seg in op.OperationsSegments)
                {
                    var segmentDTO = new SegmentDTO
                    {
                        SegmentId = seg.SegmentId,
                        Description = seg.Description ?? "--",
                        TypeSegment = seg.OperationsType.ToString()
                    };
                    segments.Add(segmentDTO);
                }

                foreach (var bom in op.OperationsMaterialBill.Items)
                {
                    double quantity = Convert.ToDouble(bom.Quantity.GetValue());

                    var bomDTO = new BomDTO
                    {
                        BomId = bom.BillId,
                        RawMaterialId = bom.MaterialDefinitionId,
                        Quantity = quantity,
                        UnitOfMeasure = bom.ValueUnitOfMeasure
                    };

                    bills.Add(bomDTO);
                }


                operationDTO.segmentDTOs = segments;
                operationDTO.bomDTOs = bills;
                ops.Add(operationDTO);
            }
            materialDTO.operationDTOs = ops;

            return materialDTO;
        }
    }
}
