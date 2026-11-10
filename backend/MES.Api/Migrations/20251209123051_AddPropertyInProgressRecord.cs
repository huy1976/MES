using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MesMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyInProgressRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DeltaRawConsumedQuatity",
                table: "WorkOrderProgressRecord",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RawConsumedQuatity",
                table: "WorkOrderProgressRecord",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeltaRawConsumedQuatity",
                table: "WorkOrderProgressRecord");

            migrationBuilder.DropColumn(
                name: "RawConsumedQuatity",
                table: "WorkOrderProgressRecord");
        }
    }
}
