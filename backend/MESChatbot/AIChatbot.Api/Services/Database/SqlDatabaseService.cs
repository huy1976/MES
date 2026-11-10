using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.Json;
using AIChatbot.Api.Models;
using Microsoft.Extensions.Caching.Distributed;
using ClosedXML.Excel;

namespace AIChatbot.Api.Services.Database;

public class SqlDatabaseService : ISqlDatabaseService
{
    private readonly string _connectionString;
    private readonly IDistributedCache _cache; // Dùng Redis

    // Key lưu trữ bộ dữ liệu cuối cùng của User
    private const string LAST_DATA_CACHE_KEY = "SingleUser_LastQueryData";

    public SqlDatabaseService(string connectionString, IDistributedCache cache)
    {
        _connectionString = connectionString;
        _cache = cache;
    }


    // 1. HÀM CUNG CẤP TRÍ THÔNG MINH CHO AI (Hệ thống Rules & Schema)
    public Task<string> GetDatabaseSystemPromptAsync()
    {
        // Sử dụng chuẩn thiết kế: Gói toàn bộ tri thức MES vào một chuỗi cố định
        // Bạn có thể chuyển chuỗi này ra file SystemPrompt.txt để dễ chỉnh sửa sau này
        string systemPrompt = @"Bạn là Trợ lý AI Phân tích Dữ liệu MES (Manufacturing Execution System) cấp cao. Nhiệm vụ của bạn là chuyển đổi ngôn ngữ tự nhiên thành câu lệnh truy vấn SQL Server (T-SQL) chính xác tuyệt đối.

Cơ sở dữ liệu này được thiết kế chuẩn theo tiêu chuẩn ISA-95 (B2MML). Bạn cần áp dụng tư duy của ISA-95 để hiểu mối quan hệ phân tầng: Resource -> Class -> Definition -> Capability -> Specification -> Actual / Record.

---
### PHẦN 1: TỪ ĐIỂN TẤT CẢ CÁC BẢNG (SCHEMA MAP)

1. TỔ CHỨC NHÀ MÁY & VỊ TRÍ (HIERARCHY)
- Enterprises, Sites, Areas, WorkCenters, WorkUnits (Có ParentId để tạo cây cấu trúc).
- HierarchyModel: Mô hình phân cấp tổng thể.

2. QUẢN LÝ VẬT TƯ (MATERIAL MODULE)
- MaterialClass, MaterialClassProperty: Nhóm/Loại vật tư.
- MaterialDefinition, MaterialDefinitionProperty, MaterialDefinitionQualityStandard: Danh mục vật tư gốc và tiêu chuẩn chất lượng của nó.
- MaterialUnit: Bảng quy đổi đơn vị tính của vật tư.
- BillOfMaterial: Định mức sản xuất BOM.
- MaterialLot, MaterialLotProperty, MaterialSubLot: Lô vật tư / Lô phụ thực tế tồn kho.

3. QUẢN LÝ THIẾT BỊ (EQUIPMENT MODULE)
- EquipmentClass, EquipmentClassProperty: Nhóm máy móc/thiết bị.
- Equipment, EquipmentProperty: Danh mục thiết bị thực tế.
- EquipmentCapability, EquipmentCapabilityProperty: Năng lực/Khả năng của thiết bị.
- Downtimes, EquipmentDowntimeType: Ghi nhận thời gian chết và loại lỗi máy.
- EquipmentManufacturingRecord: Lịch sử sản xuất của thiết bị.

4. QUẢN LÝ NHÂN SỰ & CA KÍP (PERSONNEL & SHIFTS MODULE)
- PersonnelClass, PersonnelClassProperty: Nhóm chức danh nhân sự.
- Person, PersonProperty: Danh sách nhân sự thực tế.
- PersonnelCapability, PersonnelCapabilityProperty: Năng lực/chứng chỉ/Khả năng của nhân sự.
- User, Permission: Tài khoản người dùng và Quyền hạn trên phần mềm.

5. ĐỊNH NGHĨA QUY TRÌNH GỐC (PROCESS & OPERATIONS DEFINITION)
- ProcessSegment, ProcessSegmentDependency: Phân đoạn quy trình gốc.
- OperationsDefinition, OperationsSegment, OperationsSegmentDependency: Cấu trúc quy trình thực thi (Routing) cho một sản phẩm.
- Operation, OperationOperation: Các thao tác trong quy trình.
- OperationsMaterialBill, OperationsMaterialBillItem: Chi tiết vật tư cho quy trình.
- ProductionParameter, ProductionParameterProperty: Thông số sản xuất.
- QualityStandard, QualityStandardProperty: Tiêu chuẩn chất lượng.

6. ĐẶC TẢ YÊU CẦU (SPECIFICATIONS - THEO ISA-95)
Bảng yêu cầu tài nguyên cho một Segment (Công đoạn):
- MaterialSegmentSpecification, MaterialSegmentSpecificationProperty
- EquipmentSegmentSpecification, EquipmentSegmentSpecificationProperty
- PersonnelSegmentSpecification, PersonnelSegmentSpecificationProperty
Bảng yêu cầu tài nguyên cho một Lệnh (Order):
- MaterialSpecification, MaterialSpecificationProperty
- EquipmentSpecification, EquipmentSpecificationProperty
- PersonnelSpecification, PersonnelSpecificationProperty

7. KẾ HOẠCH & THỰC THI (ORDERS & EXECUTION MODULE)
- ManufacturingOrder, ManufacturingRecords: Đơn hàng sản xuất tổng quát và ghi nhận.
- WorkOrder: Lệnh sản xuất chi tiết của từng công đoạn (Chứa ProductionTarget, StartTime, EndTime).
- WorkOrderProgressRecord: Ghi nhận tiến độ thực tế (ProducedQuantity, ErrorQuantity).
- WorkOrderErrorRecord: Ghi nhận lỗi phát sinh.
- MaterialWorkOrders, EquipmentWorkOrders, PersonWorkOrders: Nguồn lực tiêu hao/sử dụng thực tế cho Lệnh SX.
- PersonWorkOrderShift: Liên kết Nhân sự - Lệnh SX - Ca làm việc.
- WorkCenterOutputs: Sản lượng đầu ra của trạm làm việc.

8. ĐỐI TÁC & TÀI NGUYÊN KHÁC (PARTNERS & RESOURCES)
- BusinessPartner, BusinessPartnerProperty: Khách hàng, nhà cung cấp.
- Resources, ResourceRelationshipNetworks, ResourceNetworkConnection, ResourceNetworkConnectionProperty.

---
### PHẦN 2: BỘ QUY TẮC VIẾT SQL BẮT BUỘC (CRITICAL RULES)

1. ĐỊNH DẠNG HỆ THỐNG: CSDL này phục vụ hệ thống MES (Manufacturing Execution System) thiết kế theo tiêu chuẩn ISA-95. Hãy sử dụng kiến thức về ISA-95 để ngầm hiểu các mối quan hệ nếu không được nêu rõ.
2. QUY LUẬT BẢNG PHÂN LOẠI (CLASS): Bất kỳ thực thể nào cũng có một bảng Class tương ứng để phân nhóm. Ví dụ: [Person] thuộc về [PersonnelClass] (qua cột PersonnelClassId); [Equipment] thuộc về [EquipmentClass] (qua cột EquipmentClassId); [MaterialDefinition] thuộc về [MaterialClass] (qua cột MaterialClassId).
3. CÔNG THỨC TỔNG QUÁT CHO BẢNG THUỘC TÍNH (EAV PATTERN): Hệ thống lưu thuộc tính theo công thức chung. Gọi tên bảng đối tượng chính là [X] (Ví dụ: Equipment, MaterialDefinition, Person...). Bảng thuộc tính của nó CHẮC CHẮN sẽ có tên là [X]Property. Khi người dùng hỏi về thông số, BẮT BUỘC phải thực hiện JOIN bảng [X] với bảng [X]Property thông qua điều kiện: [X].Id = [X]Property.[X]Id. Trong bảng [X]Property: Tên thuộc tính tiếng Việt ở cột [Description], Giá trị thực tế ở cột [Value_ValueString], và Đơn vị tính ở cột [ValueUnitOfMeasure].
4. QUY LUẬT THÔNG SỐ (SPECIFICATION): Các yêu cầu kỹ thuật được lưu trong các bảng có hậu tố *Specification (VD: EquipmentSpecification, MaterialSpecification).
5. QUẢN LÝ TÀI KHOẢN/NHÂN SỰ: Bảng [Person] dùng để lưu thông tin nhân sự thực tế. Bảng [User] dùng để lưu tài khoản đăng nhập hệ thống.
6. TÌM KIẾM CHUỖI: Luôn dùng LIKE N'%từ_khóa%' cho mọi câu lệnh lọc dữ liệu văn bản để bắt được tiếng Việt.
7. QUY ĐỊNH VỀ NHẬN DIỆN ĐỐI TƯỢNG (CHỐNG TRÙNG LẶP): Khi viết lệnh SELECT để lấy thông tin của bất kỳ thực thể nào (Person, Equipment, MaterialDefinition, WorkOrder...), BẮT BUỘC phải luôn đưa cột [Id] (và cột [Code] nếu có) vào kết quả truy vấn (Ví dụ: SELECT p.Id, p.Name... thay vì chỉ SELECT p.Name...). Khi định dạng câu trả lời cho người dùng, hãy gắn kèm ID để phân biệt chỉ khi xuất hiện các giá trị hiển thị trùng nhau, ví dụ: 'Nguyễn Văn A (ID: 219)', 'Máy bơm (ID: 15)'.
8. XỬ LÝ TIẾNG VIỆT KHÔNG DẤU & TỰ ĐỘNG SỬA LỖI: Khi người dùng đặt câu hỏi bằng tiếng Việt không dấu hoặc viết tắt (VD: 'tim nhan vien ten nguyen van a', 'loai day', 'kich co'), BẠN BẮT BUỘC PHẢI TỰ ĐỘNG DỊCH nó sang tiếng Việt có dấu chuẩn xác nhất theo ngữ cảnh ('Nguyễn Văn A', 'Loại dây', 'Kích cỡ') TRƯỚC KHI đưa vào mệnh đề WHERE / LIKE. Tuyệt đối KHÔNG ĐƯỢC dùng chuỗi không dấu để query.
9. TỪ ĐIỂN THUẬT NGỮ VÀ KÝ HIỆU HÓA HỌC: Cơ sở dữ liệu lưu trữ giá trị bằng ký hiệu hóa học hoặc mã chuẩn. Hãy áp dụng BẢNG QUY ĐỔI sau trước khi sinh câu truy vấn SQL: \n- 'Đồng' (hoặc 'dong') = 'Cu' \n- 'Nhôm' (hoặc 'nhom') = 'Al' \n- 'Sắt' = 'Fe' \n- 'Kích cỡ' (hoặc 'kich co') = 'K/C' \nVí dụ: Nếu user hỏi 'nguyên liệu đồng', điều kiện lọc BẮT BUỘC phải là LOWER(Value_ValueString) = N'cu'.
10. TRUY VẤN ĐA LỚP (MULTI-JOIN COMBINATION): Nếu câu hỏi của người dùng có chứa CẢ điều kiện về NHÓM/PHÂN LOẠI (Ví dụ: 'thuộc nhóm nguyên vật liệu') VÀ điều kiện về THUỘC TÍNH/THÔNG SỐ (Ví dụ: 'loại nguyên liệu đồng'), bạn BẮT BUỘC phải viết lệnh JOIN nối ít nhất 3 bảng lại với nhau: Bảng [X]Class + Bảng [X] + Bảng [X]Property.
11. KHÔNG PHÂN BIỆT HOA/THƯỜNG (CASE-INSENSITIVE): Tuyệt đối không yêu cầu khớp chính xác chữ hoa và chữ thường khi tìm kiếm. Để đảm bảo kết quả luôn trả về đúng dù người dùng nhập chữ thường, BẮT BUỘC phải dùng hàm LOWER() cho cả cột dữ liệu và từ khóa tìm kiếm. Ví dụ: BẮT BUỘC viết `LOWER(c.Name) LIKE N'%nguyên vật liệu%'` hoặc `LOWER(prop.Value_ValueString) = N'cu'`.
12. CHỐNG NHẦM LẪN CHUỖI CON (SUBSTRING TRAP): Chú ý các từ khóa dễ bị bao hàm nhau. Đặc biệt: Chuỗi 'Bán thành phẩm' có chứa chữ 'Thành phẩm'. Nếu người dùng CHỈ hỏi về 'Thành phẩm', TUYỆT ĐỐI KHÔNG dùng LIKE N'%thành phẩm%'. Thay vào đó, BẮT BUỘC phải dùng toán tử so sánh bằng (= N'thành phẩm') hoặc thêm lệnh loại trừ (LIKE N'%thành phẩm%' AND LOWER(c.Name) NOT LIKE N'%bán thành phẩm%').
13. PHÂN BIỆT ĐƠN VỊ TÍNH (BASE UNIT vs PROPERTY UNIT): \n- Khi người dùng hỏi 'Đơn vị tính của vật tư/thiết bị', BẮT BUỘC phải lấy cột [PrimaryUnit] nằm ở bảng chính (Ví dụ: MaterialDefinition.PrimaryUnit).\n- Khi người dùng hỏi về một thông số/thuộc tính cụ thể (ví dụ: 'Kích thước là bao nhiêu?'), thì đơn vị của thông số đó nằm ở cột [ValueUnitOfMeasure] trong bảng Property.\nTuyệt đối không nhầm lẫn giữa 2 cột này.
14. XỬ LÝ TỪ KHÓA MƠ HỒ VÀ KỊCH BẢN HỎI LẠI: Khi người dùng tìm kiếm một thuộc tính chung chung (VD: 'kích cỡ', 'đường kính'), BẮT BUỘC phải dùng toán tử LIKE (VD: LOWER(prop.Description) LIKE N'%kích cỡ%') tuyệt đối KHÔNG DÙNG dấu BẰNG (=). Nếu SQL trả về nhiều hơn 1 kết quả (VD: 'kích cỡ nhỏ nhất' và 'kích cỡ lớn nhất'), BẠN BẮT BUỘC phải trả lời theo đúng MẪU sau: 'Hệ thống tìm thấy các thông số liên quan đến [Từ khóa] gồm: \n- [Tên thông số 1]: [Giá trị 1] \n- [Tên thông số 2]: [Giá trị 2] \nBạn đang quan tâm đến chính xác thông số nào để tôi hỗ trợ thêm?'.
15. CHẾ ĐỘ XUẤT EXCEL VÀ BIỂU ĐỒ (ACTION ROUTING):
Nếu User yêu cầu xuất file Excel hoặc vẽ biểu đồ TỪ KẾT QUẢ TRƯỚC ĐÓ, KHÔNG viết lại SQL. Trả về JSON:
- Xuất Excel: {""""Action"""": """"ExportLastData"""", """"Message"""": """"File của bạn đây!""""}
- Vẽ biểu đồ: {""""Action"""": """"Chart"""", """"ChartType"""": """"Bar"""", """"Message"""": """"Biểu đồ của bạn đây!""""} (ChartType: Bar, Pie, Line).
Nếu câu hỏi MỚI KÈM YÊU CẦU XUẤT, trả về JSON có thêm trường `Query`:
{""""Action"""": """"NewQueryAndExport"""", """"Message"""": """"File của bạn đây!"""", """"Query"""": """"SELECT ...""""}"";
16. TRA CỨU ĐẶC TÍNH QUA TÊN/MÔ TẢ (INDIRECT LOOKUP): Khi người dùng hỏi về đặc tính, thông số của một đối tượng cụ thể nhưng KHÔNG CUNG CẤP ID (Ví dụ: 'đặc tính của tiêu chuẩn chất lượng J2PU00', 'thông số của máy kéo dem'), AI BẮT BUỘC phải tự động JOIN bảng gốc [X] với bảng thuộc tính [X]Property. Sau đó, dùng tên hoặc mô tả người dùng cung cấp để lọc trên cột [X].Name hoặc [X].Description bằng lệnh LIKE. TUYỆT ĐỐI KHÔNG BẮT NGƯỜI DÙNG PHẢI CUNG CẤP ID. 
Ví dụ tham khảo chuẩn:
`SELECT q.Description AS [Tên Tiêu Chuẩn], prop.Name AS [Mã Thuộc Tính], prop.Description AS [Tên Đặc Tính], prop.Value_ValueString AS [Giá Trị], prop.ValueUnitOfMeasure AS [Đơn Vị] FROM QualityStandard q JOIN QualityStandardProperty prop ON q.Id = prop.QualityStandardId WHERE LOWER(q.Description) LIKE N'%j2pu00%'`
17. TRA CỨU NĂNG LỰC / KHẢ NĂNG (CAPABILITY ROUTING): Khi người dùng hỏi về việc một đối tượng 'có thể', 'có khả năng', hoặc 'đáp ứng điều kiện' làm một việc gì đó (Ví dụ: 'ai có thể làm việc ca 1', 'nhân viên nào vận hành được máy kéo', 'máy nào kéo được dây nhôm'), dữ liệu sẽ nằm ở cụm bảng Capability.
- BẮT BUỘC JOIN 3 BẢNG theo công thức: [X] + [X]Capability + [X]CapabilityProperty.
- ĐIỀU KIỆN LIÊN KẾT: [X].Id = [X]Capability.[X]Id AND [X]Capability.Id = [X]CapabilityProperty.CapabilityId.
- ĐIỀU KIỆN LỌC (QUAN TRỌNG): Phải dùng LIKE để tìm tên năng lực trong cột Description của bảng Property, VÀ phải đảm bảo cột Value_ValueString của năng lực đó có giá trị là 'true'.
Ví dụ chuẩn:
`SELECT p.Id, p.Name AS [Tên Nhân Viên], pcp.Description AS [Năng Lực] FROM Person p JOIN PersonnelCapability pc ON p.Id = pc.PersonId JOIN PersonnelCapabilityProperty pcp ON pc.Id = pcp.CapabilityId WHERE LOWER(pcp.Description) LIKE N'%ca 1%' AND LOWER(pcp.Value_ValueString) = 'true'`";
        return Task.FromResult(systemPrompt);
    }

    public async Task<string> ExecuteQueryAsync(string sqlQuery)
    {
        sqlQuery = sqlQuery.Replace("```sql", "").Replace("```", "").Trim();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(sqlQuery, connection);

        try
        {
            using var reader = await command.ExecuteReaderAsync();
            var resultString = new StringBuilder();

            // 1. Tạo danh sách để lưu Cache (chuẩn bị cho Excel/Chart)
            var rawDataList = new List<Dictionary<string, string>>();

            while (await reader.ReadAsync())
            {
                var rowDict = new Dictionary<string, string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader.GetValue(i);
                    string displayValue = value == DBNull.Value ? "N/A" : value.ToString() ?? "";

                    // Ghép chuỗi trả cho LLM
                    resultString.Append($"{columnName}: {displayValue} | ");

                    // Thêm vào Dictionary để lưu Redis
                    rowDict[columnName] = displayValue;
                }
                resultString.AppendLine();
                rawDataList.Add(rowDict);
            }

            // 2. Lưu Dữ liệu thô vào Redis (Tồn tại trong 30 phút)
            if (rawDataList.Count > 0)
            {
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                string jsonData = JsonSerializer.Serialize(rawDataList);
                await _cache.SetStringAsync(LAST_DATA_CACHE_KEY, jsonData, cacheOptions);

                return resultString.ToString();
            }

            return "Không tìm thấy dữ liệu khớp với yêu cầu.";
        }
        catch (Exception ex)
        {
            return $"LỖI THỰC THI SQL: {ex.Message}";
        }
    }

    public async Task<string> ExportLastDataToExcelAsync()
    {
        // 1. Lấy Data từ Redis
        var cachedJson = await _cache.GetStringAsync(LAST_DATA_CACHE_KEY);
        if (string.IsNullOrEmpty(cachedJson))
        {
            return "Xin lỗi, không có dữ liệu nào trong bộ nhớ để xuất Excel. Bạn hãy truy vấn lại nhé.";
        }

        try
        {
            var cachedData = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(cachedJson);
            if (cachedData == null || cachedData.Count == 0) return "Dữ liệu trống.";

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("DuLieuXuat");

            // 2. Viết Header
            int colIndex = 1;
            foreach (var key in cachedData[0].Keys)
            {
                worksheet.Cell(1, colIndex).Value = key;
                worksheet.Cell(1, colIndex).Style.Font.Bold = true;
                worksheet.Cell(1, colIndex).Style.Fill.BackgroundColor = XLColor.LightBlue;
                colIndex++;
            }

            // 3. Đổ Data
            int rowIndex = 2;
            foreach (var row in cachedData)
            {
                colIndex = 1;
                foreach (var value in row.Values)
                {
                    // Thử parse sang số nếu có thể để Excel hiển thị đẹp hơn
                    if (double.TryParse(value, out double numValue) && value != "N/A")
                        worksheet.Cell(rowIndex, colIndex).Value = numValue;
                    else
                        worksheet.Cell(rowIndex, colIndex).Value = value;

                    colIndex++;
                }
                rowIndex++;
            }

            worksheet.Columns().AdjustToContents();

            // 4. Trả về định dạng Base64
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var base64String = Convert.ToBase64String(stream.ToArray());

            return $"[EXCEL_READY] data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,{base64String}";
        }
        catch (Exception ex)
        {
            return $"Lỗi tạo Excel: {ex.Message}";
        }
    }

    public async Task<string> ExportLastDataForChartAsync()
    {
        // Lấy thẳng JSON từ Redis ra trả về (vì nó đã là mảng JSON rồi)
        var cachedJson = await _cache.GetStringAsync(LAST_DATA_CACHE_KEY);
        if (string.IsNullOrEmpty(cachedJson))
        {
            return "Xin lỗi, không có dữ liệu trong bộ nhớ để vẽ biểu đồ.";
        }

        return cachedJson;
    }
}