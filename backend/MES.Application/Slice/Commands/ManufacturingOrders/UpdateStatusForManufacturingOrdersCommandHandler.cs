using MesMicroservice.Domain.AggregateModels.ManufacturingOrderAggregate;
using MesMicroservice.Domain.AggregateModels.WorkOrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesMicroservice.Application.Commands.ManufacturingOrders
{
    public class UpdateStatusForManufacturingOrdersCommandHandler : IRequestHandler<UpdateStatusForManufacturingOrdersCommand, bool>
    {
        private readonly IManufacturingOrderRepository _manufacturingOrderRepository;
        public UpdateStatusForManufacturingOrdersCommandHandler(IManufacturingOrderRepository manufacturingOrderRepository)
        {
            _manufacturingOrderRepository = manufacturingOrderRepository;
        }

        public async Task<bool> Handle(UpdateStatusForManufacturingOrdersCommand request, CancellationToken cancellationToken)
        {
            var manufacturingOrders = await _manufacturingOrderRepository.GetAll();

            foreach (var mo in manufacturingOrders)
            {
                var workOrders = mo.WorkOrders;

                if (workOrders.Any(wo => wo.WorkOrderStatus == EStatus.Pending))
                    mo.UpdateStatus(EStatus.Pending);
                else if (workOrders.Any(wo => wo.WorkOrderStatus == EStatus.PendingScheduling))
                    mo.UpdateStatus(EStatus.PendingScheduling);
                else if (workOrders.Any(wo => wo.WorkOrderStatus == EStatus.ReadyForProduction))
                    mo.UpdateStatus(EStatus.ReadyForProduction);
                else if (workOrders.Any(wo => wo.WorkOrderStatus == EStatus.InProduction))
                    mo.UpdateStatus(EStatus.InProduction);
                else if (workOrders.All(wo => wo.WorkOrderStatus == EStatus.Completed))
                    mo.UpdateStatus(EStatus.Completed);
                else
                    mo.UpdateStatus(EStatus.Cancelled);

                var latestEndDate = mo.WorkOrders.Max(wo => wo.EndTime);
                var earliestStartDate = mo.WorkOrders.Min(wo => wo.StartTime);
                if (latestEndDate.HasValue && earliestStartDate.HasValue)
                {
                    mo.UpdateDates(earliestStartDate.Value, latestEndDate.Value);
                }
            }

            return await _manufacturingOrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

    }
}
