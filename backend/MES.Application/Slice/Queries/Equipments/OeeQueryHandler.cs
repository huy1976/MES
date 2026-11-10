namespace MesMicroservice.Application.Queries.Equipments;

public class OeeQueryHandler : IRequestHandler<OeeQuery, IEnumerable<OeeViewModel>>
{
    private readonly IManufacturingRecordRepository _manufacturingRecordRepository;

    public OeeQueryHandler(IManufacturingRecordRepository manufacturingRecordRepository)
    {
        _manufacturingRecordRepository = manufacturingRecordRepository;
    }

    public async Task<IEnumerable<OeeViewModel>> Handle(OeeQuery request, CancellationToken cancellationToken)
    {
        var manufacturingRecords = await _manufacturingRecordRepository.GetAllAsync();

        manufacturingRecords = manufacturingRecords.Where(x =>
            x.EndTime > request.From &&
            x.StartTime < request.To &&
            x.Equipments.Any(e => e.ResourceId == request.EquipmentId));

        var oeeRecords = new List<OeeViewModel>();
        var frameStartTime = request.From;

        while (frameStartTime < request.To)
        {
            var frameEndTime = frameStartTime.AddSeconds(request.TimeFrameBySecond);
            var frameRecords = manufacturingRecords.Where(x => x.EndTime > frameStartTime &&
            x.StartTime < frameEndTime);

            var totalOutput = frameRecords.Sum(x => x.Output);
            var totalRawOutput = frameRecords.Sum(x => x.RawOutput);
            TimeSpan totalManufacturingTime = TimeSpan.Zero;
            foreach (var record in frameRecords)
            {
                var startTime = record.StartTime > frameStartTime ? record.StartTime : frameStartTime;
                var endTime = record.EndTime < frameEndTime ? record.EndTime : frameEndTime;
                var manufacturingTime = endTime - startTime;
                totalManufacturingTime = totalManufacturingTime.Add(manufacturingTime);
            }
            var totalTime = frameEndTime - frameStartTime;

            var a = (decimal)(totalManufacturingTime / totalTime);
            var p = 1;
            var q = totalRawOutput == 0 ? 0 : totalOutput / totalRawOutput;
            var oee = a * p * q;
            var oeeRecord = new OeeViewModel(request.EquipmentId, frameStartTime, frameEndTime, a, p, q, oee);
            oeeRecords.Add(oeeRecord);

            frameStartTime = frameStartTime.AddSeconds(request.TimeFrameBySecond);
        }

        return oeeRecords;
    }
}
