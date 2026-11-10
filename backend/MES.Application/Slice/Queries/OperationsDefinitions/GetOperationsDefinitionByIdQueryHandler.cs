using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesMicroservice.Application.Queries.OperationsDefinitions
{
    public class GetOperationsDefinitionByIdQueryHandler : IRequestHandler<GetOperationsDefinitionByIdQuery, OperationDTO>
    {
        private readonly IOperationsDefinitionRepository _operationsDefinitionRepository;
        private readonly IMapper _mapper;

        public GetOperationsDefinitionByIdQueryHandler(IOperationsDefinitionRepository operationsDefinitionRepository, IMapper mapper)
        {
            _operationsDefinitionRepository = operationsDefinitionRepository;
            _mapper = mapper;
        }

        public async Task<OperationDTO> Handle(GetOperationsDefinitionByIdQuery request, CancellationToken cancellationToken)
        {
            var op = await _operationsDefinitionRepository.GetByIdAsync(request.OperationId);
            if (op is null)
            {
                throw new ResourceNotFoundException(nameof(OperationsDefinition), request.OperationId);
            }

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
                var bomDTO = new BomDTO
                {
                    BomId = bom.BillId,
                    RawMaterialId = bom.MaterialDefinitionId,
                    Quantity = (double)bom.Quantity.GetValue(),
                    UnitOfMeasure = bom.ValueUnitOfMeasure
                };
                bills.Add(bomDTO);
            }

            operationDTO.segmentDTOs = segments;
            operationDTO.bomDTOs = bills;

            return operationDTO;
        }
    }
}
