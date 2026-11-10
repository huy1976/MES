using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MesMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeSegmentTypeDynamic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Đổi SegmentType từ int -> string.
            // SQL Server sẽ giữ dữ liệu cũ dưới dạng "0", "1", "3".
            migrationBuilder.AlterColumn<string>(
                name: "SegmentType",
                table: "ProcessSegment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SegmentType",
                table: "OperationsSegment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // 2. Chuyển dữ liệu cũ từ giá trị enum sang tên có ý nghĩa.
            migrationBuilder.Sql(@"
                UPDATE ProcessSegment
                SET SegmentType =
                    CASE SegmentType
                        WHEN '0' THEN 'Drawing'
                        WHEN '1' THEN 'Enameling'
                        WHEN '3' THEN 'Both'
                        ELSE SegmentType
                    END;
            ");

            migrationBuilder.Sql(@"
                UPDATE OperationsSegment
                SET SegmentType =
                    CASE SegmentType
                        WHEN '0' THEN 'Drawing'
                        WHEN '1' THEN 'Enameling'
                        WHEN '3' THEN 'Both'
                        ELSE SegmentType
                    END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nếu sau này đã thêm loại công đoạn mới như Packaging,
            // thì không thể tự động rollback về enum cũ.
            // Chặn rollback để tránh mất dữ liệu.
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM ProcessSegment
                    WHERE SegmentType NOT IN
                    ('Drawing', 'Enameling', 'Both', '0', '1', '3')
                )
                BEGIN
                    THROW 50001,
                    'Cannot rollback: ProcessSegment contains new dynamic SegmentType values.',
                    1;
                END;
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM OperationsSegment
                    WHERE SegmentType NOT IN
                    ('Drawing', 'Enameling', 'Both', '0', '1', '3')
                )
                BEGIN
                    THROW 50002,
                    'Cannot rollback: OperationsSegment contains new dynamic SegmentType values.',
                    1;
                END;
            ");

            // Chuyển tên cũ trở lại giá trị enum.
            migrationBuilder.Sql(@"
                UPDATE ProcessSegment
                SET SegmentType =
                    CASE SegmentType
                        WHEN 'Drawing' THEN '0'
                        WHEN 'Enameling' THEN '1'
                        WHEN 'Both' THEN '3'
                        ELSE SegmentType
                    END;
            ");

            migrationBuilder.Sql(@"
                UPDATE OperationsSegment
                SET SegmentType =
                    CASE SegmentType
                        WHEN 'Drawing' THEN '0'
                        WHEN 'Enameling' THEN '1'
                        WHEN 'Both' THEN '3'
                        ELSE SegmentType
                    END;
            ");

            // 3. Đổi string trở lại int khi rollback.
            migrationBuilder.AlterColumn<int>(
                name: "SegmentType",
                table: "ProcessSegment",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "SegmentType",
                table: "OperationsSegment",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}