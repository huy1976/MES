using Microsoft.AspNetCore.Mvc.Filters;

namespace MesMicroservice.Application.Queries.WorkOrders
{
    public class WorkOrderByStatusQueryHandler : IRequestHandler<WorkOrderByStatusQuery, List<WorkOrderViewDTO>>
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        private readonly IProcessSegmentRepository _processSegmentRepository;
        private readonly IMapper _mapper;

        public WorkOrderByStatusQueryHandler(IWorkOrderRepository workOrderRepository, IManufacturingOrderRepository manufacturingOrderRepository, IProcessSegmentRepository processSegmentRepository, IMapper mapper)
        {
            _workOrderRepository = workOrderRepository;
            _manufacturingOrderRepository = manufacturingOrderRepository;
            _processSegmentRepository = processSegmentRepository;
            _mapper = mapper;
        }

        public async Task<List<WorkOrderViewDTO>> Handle(WorkOrderByStatusQuery request, CancellationToken cancellationToken)
        {
            var workOrders = await _workOrderRepository.GetAll();

            workOrders = workOrders.Where(wo => wo.WorkOrderStatus == request.Status).ToList();

            var workOrderDTOs = new List<WorkOrderViewDTO>();
            foreach (var wo in workOrders) 
            {
                var woDTO = new WorkOrderViewDTO(manufacturingOrderId: wo.ManufacturingOrder.Id,
                                                 manufacturingOrderCode: wo.ManufacturingOrder.ManufacturingOrderId,
                                                 id: wo.Id,
                                                 workOrderId: wo.WorkOrderId,
                                                 segmentId: wo.SegmentId,
                                                 sequenceNumber: wo.SequenceNumber,
                                                 materialDefinitionId: wo.MaterialDefinition.ResourceId,
                                                 productionTarget: wo.ProductionTarget,
                                                 unit: wo.Unit,
                                                 start: wo.StartTime,
                                                 end: wo.EndTime,
                                                 productionParamId: wo.OperationsSegment.ProductionParameterId,
                                                 qualityParamId: wo.ManufacturingOrder.QualityStandard.ResourceId,
                                                 status: wo.WorkOrderStatus.ToString());

                var processSegment = await _processSegmentRepository.GetAsync(wo.OperationsSegment.SegmentId);
                if (processSegment is null) throw new ResourceNotFoundException(nameof(ProcessSegment), wo.SegmentId);

                var manufacturingOrder = await _manufacturingOrderRepository.GetByIdAsync(wo.ManufacturingOrder.ManufacturingOrderId);

                // So sánh materialDefinitionId để lấy Items trong BOM VÀ CẬP NHẬT TÊN SEGMENT
                foreach (var item in manufacturingOrder.OperationsDefinition.OperationsMaterialBill.Items)
                {
                    foreach (var materialSpecification in wo.OperationsSegment.MaterialSpecifications)
                    {
                        if (item.MaterialDefinitionId == materialSpecification.MaterialDefinitionId)
                        {
                            woDTO.AddBOMItem(item.BillId);
                        }
                    }

                }

                woDTO.UpdateSegmentName(processSegment.Name );

                workOrderDTOs.Add(woDTO);
            }

            return workOrderDTOs;
        }
    }
}
