using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MesMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateNewDatababse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "application");

            migrationBuilder.CreateSequence(
                name: "areaeq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "enterpriseeq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "hierarchymodeleq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "manufacturingorderseq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "manufacturingrecordeq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "materialuniteq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "operationeq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "resourcenetworkconnectioneq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "resourcerelationshipnetworkeq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "siteeq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "workcentereq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "workcenteroutputeq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "workorderseq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "workuniteq",
                schema: "application",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "EquipmentDowntimeType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentClassId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentDowntimeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HierarchyModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    HierarchyModelId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HierarchyModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false),
                    OperationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessSegmentDependency",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromSegmentId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ToSegmentId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DependencyType = table.Column<int>(type: "int", nullable: false),
                    DependencyFactor_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DependencyFactor_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessSegmentDependency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceRelationshipNetworks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ResourceRelationshipNetworkId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelationshipType = table.Column<int>(type: "int", nullable: false),
                    RelationshipForm = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceRelationshipNetworks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActiveDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderErrorRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ErrorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DowntimeType = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HandledBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationInHours = table.Column<double>(type: "float", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkOrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsUpdateTime = table.Column<bool>(type: "bit", nullable: false),
                    IsSchedule = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderErrorRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderProgressRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProgressPercentage = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalQuantity = table.Column<double>(type: "float", nullable: false),
                    ProducedQuantity = table.Column<double>(type: "float", nullable: false),
                    ErrorQuantity = table.Column<double>(type: "float", nullable: false),
                    DeltaQuantity = table.Column<double>(type: "float", nullable: false),
                    DeltaErrorQuantity = table.Column<double>(type: "float", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Equipment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkOrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderProgressRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enterprises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enterprises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enterprises_HierarchyModel_Id",
                        column: x => x.Id,
                        principalTable: "HierarchyModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enterprises_HierarchyModel_ParentId",
                        column: x => x.ParentId,
                        principalTable: "HierarchyModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OperationOperation",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteOperationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationOperation", x => new { x.OperationId, x.PrerequisiteOperationId });
                    table.ForeignKey(
                        name: "FK_OperationOperation_Operation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperationOperation_Operation_PrerequisiteOperationId",
                        column: x => x.PrerequisiteOperationId,
                        principalTable: "Operation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BusinessPartner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BusinessPartnerType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPartner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessPartner_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipmentLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentClass_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssemblyType = table.Column<int>(type: "int", nullable: false),
                    AssemblyRelationship = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialClass_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelClass_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionParameter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionParameter_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityStandard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityStandard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityStandard_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceNetworkConnection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromResourceId = table.Column<int>(type: "int", nullable: false),
                    ToResourceId = table.Column<int>(type: "int", nullable: false),
                    ResourceRelationshipNetworkId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceNetworkConnection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceNetworkConnection_ResourceRelationshipNetworks_ResourceRelationshipNetworkId",
                        column: x => x.ResourceRelationshipNetworkId,
                        principalTable: "ResourceRelationshipNetworks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceNetworkConnection_Resources_FromResourceId",
                        column: x => x.FromResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ResourceNetworkConnection_Resources_ToResourceId",
                        column: x => x.ToResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Shift",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScheduleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<double>(type: "float", nullable: false),
                    EndTime = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shift_Schedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sites_Enterprises_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Enterprises",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sites_HierarchyModel_Id",
                        column: x => x.Id,
                        principalTable: "HierarchyModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessPartnerProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessPartnerId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPartnerProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessPartnerProperty_BusinessPartner_BusinessPartnerId",
                        column: x => x.BusinessPartnerId,
                        principalTable: "BusinessPartner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EquipmentLevel = table.Column<int>(type: "int", nullable: false),
                    EquipmentStatus = table.Column<int>(type: "int", nullable: false),
                    EquipmentClassId = table.Column<int>(type: "int", nullable: false),
                    HierarchyModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipment_EquipmentClass_EquipmentClassId",
                        column: x => x.EquipmentClassId,
                        principalTable: "EquipmentClass",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Equipment_HierarchyModel_HierarchyModelId",
                        column: x => x.HierarchyModelId,
                        principalTable: "HierarchyModel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Equipment_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentClassProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentClassId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentClassProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentClassProperty_EquipmentClass_EquipmentClassId",
                        column: x => x.EquipmentClassId,
                        principalTable: "EquipmentClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialClassProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialClassId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialClassProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialClassProperty_MaterialClass_MaterialClassId",
                        column: x => x.MaterialClassId,
                        principalTable: "MaterialClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialDefinition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PrimaryUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssemblyType = table.Column<int>(type: "int", nullable: false),
                    AssemblyRelationship = table.Column<int>(type: "int", nullable: false),
                    MaterialClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialDefinition_MaterialClass_MaterialClassId",
                        column: x => x.MaterialClassId,
                        principalTable: "MaterialClass",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialDefinition_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonStatus = table.Column<int>(type: "int", nullable: false),
                    PersonnelClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Person_PersonnelClass_PersonnelClassId",
                        column: x => x.PersonnelClassId,
                        principalTable: "PersonnelClass",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Person_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelClassProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonClassId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelClassProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelClassProperty_PersonnelClass_PersonClassId",
                        column: x => x.PersonClassId,
                        principalTable: "PersonnelClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessSegment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SegmentType = table.Column<int>(type: "int", nullable: false),
                    OperationsType = table.Column<int>(type: "int", nullable: false),
                    HierarchyScope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionParameterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessSegment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessSegment_ProductionParameter_ProductionParameterId",
                        column: x => x.ProductionParameterId,
                        principalTable: "ProductionParameter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcessSegment_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionParameterProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionParameterId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionParameterProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionParameterProperty_ProductionParameter_ProductionParameterId",
                        column: x => x.ProductionParameterId,
                        principalTable: "ProductionParameter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityStandardProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QualityStandardId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityStandardProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityStandardProperty_QualityStandard_QualityStandardId",
                        column: x => x.QualityStandardId,
                        principalTable: "QualityStandard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceNetworkConnectionProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConnectionId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceNetworkConnectionProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceNetworkConnectionProperty_ResourceNetworkConnection_ConnectionId",
                        column: x => x.ConnectionId,
                        principalTable: "ResourceNetworkConnection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Areas_HierarchyModel_Id",
                        column: x => x.Id,
                        principalTable: "HierarchyModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Areas_Sites_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Sites",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Downtimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<DateTime>(type: "datetime2", nullable: false),
                    To = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DowntimeTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Downtimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Downtimes_EquipmentDowntimeType_DowntimeTypeId",
                        column: x => x.DowntimeTypeId,
                        principalTable: "EquipmentDowntimeType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Downtimes_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCapability",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HierarchyScope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapabilityType = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCapability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentCapability_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentCapability_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentProperty_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillOfMaterial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialUse = table.Column<int>(type: "int", nullable: false),
                    AssemblyType = table.Column<int>(type: "int", nullable: false),
                    AssemblyRelationship = table.Column<int>(type: "int", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillOfMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillOfMaterial_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillOfMaterial_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialDefinitionProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialDefinitionProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialDefinitionProperty_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialDefinitionQualityStandard",
                columns: table => new
                {
                    MaterialDefinitionsId = table.Column<int>(type: "int", nullable: false),
                    QualityStandardsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialDefinitionQualityStandard", x => new { x.MaterialDefinitionsId, x.QualityStandardsId });
                    table.ForeignKey(
                        name: "FK_MaterialDefinitionQualityStandard_MaterialDefinition_MaterialDefinitionsId",
                        column: x => x.MaterialDefinitionsId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialDefinitionQualityStandard_QualityStandard_QualityStandardsId",
                        column: x => x.QualityStandardsId,
                        principalTable: "QualityStandard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialUnit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ConversionValueToPrimaryUnit = table.Column<decimal>(type: "decimal(30,8)", precision: 30, scale: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialUnit_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OperationsDefinition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OperationsType = table.Column<int>(type: "int", nullable: false),
                    HierarchyScope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkDefinitionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationsDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationsDefinition_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OperationsDefinition_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelCapability",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HierarchyScope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapabilityType = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelCapability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelCapability_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonnelCapability_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonProperty_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSegmentSpecification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EquipmentUse = table.Column<int>(type: "int", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessSegmentId = table.Column<int>(type: "int", nullable: false),
                    EquipmentClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSegmentSpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentSegmentSpecification_EquipmentClass_EquipmentClassId",
                        column: x => x.EquipmentClassId,
                        principalTable: "EquipmentClass",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentSegmentSpecification_ProcessSegment_ProcessSegmentId",
                        column: x => x.ProcessSegmentId,
                        principalTable: "ProcessSegment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentSegmentSpecification_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialSegmentSpecification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssemblyType = table.Column<int>(type: "int", nullable: false),
                    AssemblyRelationship = table.Column<int>(type: "int", nullable: false),
                    MaterialUse = table.Column<int>(type: "int", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessSegmentId = table.Column<int>(type: "int", nullable: false),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialSegmentSpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialSegmentSpecification_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialSegmentSpecification_ProcessSegment_ProcessSegmentId",
                        column: x => x.ProcessSegmentId,
                        principalTable: "ProcessSegment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialSegmentSpecification_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelSegmentSpecification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PersonnelUse = table.Column<int>(type: "int", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessSegmentId = table.Column<int>(type: "int", nullable: false),
                    PersonnelClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelSegmentSpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelSegmentSpecification_PersonnelClass_PersonnelClassId",
                        column: x => x.PersonnelClassId,
                        principalTable: "PersonnelClass",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonnelSegmentSpecification_ProcessSegment_ProcessSegmentId",
                        column: x => x.ProcessSegmentId,
                        principalTable: "ProcessSegment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonnelSegmentSpecification_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkCenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    WorkCenterType = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkCenters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkCenters_Areas_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Areas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkCenters_HierarchyModel_Id",
                        column: x => x.Id,
                        principalTable: "HierarchyModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCapabilityProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentCapabilityId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCapabilityProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentCapabilityProperty_EquipmentCapability_EquipmentCapabilityId",
                        column: x => x.EquipmentCapabilityId,
                        principalTable: "EquipmentCapability",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialLot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssemblyType = table.Column<int>(type: "int", nullable: false),
                    AssemblyRelationship = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StorageLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,8)", precision: 30, scale: 8, nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialLot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialLot_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialLot_MaterialUnit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MaterialUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialLot_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManufacturingOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ManufacturingOrderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AvailableDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanedEnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OperationsDefinitionId = table.Column<int>(type: "int", nullable: false),
                    QualityStandardId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManufacturingOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManufacturingOrder_BusinessPartner_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "BusinessPartner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManufacturingOrder_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManufacturingOrder_OperationsDefinition_OperationsDefinitionId",
                        column: x => x.OperationsDefinitionId,
                        principalTable: "OperationsDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManufacturingOrder_Person_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManufacturingOrder_QualityStandard_QualityStandardId",
                        column: x => x.QualityStandardId,
                        principalTable: "QualityStandard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OperationsMaterialBill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationsDefinitionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationsMaterialBill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationsMaterialBill_OperationsDefinition_OperationsDefinitionId",
                        column: x => x.OperationsDefinitionId,
                        principalTable: "OperationsDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationsSegment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SegmentId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductionParameterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OperationsType = table.Column<int>(type: "int", nullable: false),
                    SegmentType = table.Column<int>(type: "int", nullable: false),
                    HierarchyScope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkDefinitionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationsDefinitionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationsSegment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationsSegment_OperationsDefinition_OperationsDefinitionId",
                        column: x => x.OperationsDefinitionId,
                        principalTable: "OperationsDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationsSegmentDependency",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromSegmentId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ToSegmentId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DependencyType = table.Column<int>(type: "int", nullable: false),
                    DependencyFactor_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DependencyFactor_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OperationsDefinitionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationsSegmentDependency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationsSegmentDependency_OperationsDefinition_OperationsDefinitionId",
                        column: x => x.OperationsDefinitionId,
                        principalTable: "OperationsDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelCapabilityProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CapabilityId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelCapabilityProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelCapabilityProperty_PersonnelCapability_CapabilityId",
                        column: x => x.CapabilityId,
                        principalTable: "PersonnelCapability",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSegmentSpecificationProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentSegmentSpecificationId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSegmentSpecificationProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentSegmentSpecificationProperty_EquipmentSegmentSpecification_EquipmentSegmentSpecificationId",
                        column: x => x.EquipmentSegmentSpecificationId,
                        principalTable: "EquipmentSegmentSpecification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialSegmentSpecificationProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialSegmentSpecificationId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialSegmentSpecificationProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialSegmentSpecificationProperty_MaterialSegmentSpecification_MaterialSegmentSpecificationId",
                        column: x => x.MaterialSegmentSpecificationId,
                        principalTable: "MaterialSegmentSpecification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelSegmentSpecificationProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelSegmentSpecificationId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelSegmentSpecificationProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelSegmentSpecificationProperty_PersonnelSegmentSpecification_PersonnelSegmentSpecificationId",
                        column: x => x.PersonnelSegmentSpecificationId,
                        principalTable: "PersonnelSegmentSpecification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkCenterOutputs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    WorkCenterId = table.Column<int>(type: "int", nullable: false),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Output = table.Column<decimal>(type: "decimal(30,8)", precision: 30, scale: 8, nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkCenterOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkCenterOutputs_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkCenterOutputs_MaterialUnit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MaterialUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkCenterOutputs_WorkCenters_WorkCenterId",
                        column: x => x.WorkCenterId,
                        principalTable: "WorkCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkUnits_HierarchyModel_Id",
                        column: x => x.Id,
                        principalTable: "HierarchyModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkUnits_WorkCenters_ParentId",
                        column: x => x.ParentId,
                        principalTable: "WorkCenters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaterialLotProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialLotId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialLotProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialLotProperty_MaterialLot_MaterialLotId",
                        column: x => x.MaterialLotId,
                        principalTable: "MaterialLot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialSubLot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssemblyType = table.Column<int>(type: "int", nullable: false),
                    AssemblyRelationship = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StorageLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,8)", precision: 30, scale: 8, nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false),
                    ParentMaterialLotId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialSubLot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialSubLot_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialSubLot_MaterialLot_ParentMaterialLotId",
                        column: x => x.ParentMaterialLotId,
                        principalTable: "MaterialLot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialSubLot_MaterialUnit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MaterialUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialSubLot_Resources_Id",
                        column: x => x.Id,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationsMaterialBillItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialDefinitionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialUse = table.Column<int>(type: "int", nullable: false),
                    AssemblyType = table.Column<int>(type: "int", nullable: false),
                    AssemblyRelationship = table.Column<int>(type: "int", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationsMaterialBillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationsMaterialBillItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperationsMaterialBillItem_OperationsMaterialBill_OperationsMaterialBillId",
                        column: x => x.OperationsMaterialBillId,
                        principalTable: "OperationsMaterialBill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSpecification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentSegmentSpecificationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EquipmentClassId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EquipmentUse = table.Column<int>(type: "int", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationsSegmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentSpecification_OperationsSegment_OperationsSegmentId",
                        column: x => x.OperationsSegmentId,
                        principalTable: "OperationsSegment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialSpecification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialSegmentSpecificationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaterialDefinitionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AssemblyType = table.Column<int>(type: "int", nullable: false),
                    AssemblyRelationship = table.Column<int>(type: "int", nullable: false),
                    MaterialUse = table.Column<int>(type: "int", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationsSegmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialSpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialSpecification_OperationsSegment_OperationsSegmentId",
                        column: x => x.OperationsSegmentId,
                        principalTable: "OperationsSegment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelSpecification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelSegmentSpecificationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PersonnelClassId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PersonnelUse = table.Column<int>(type: "int", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationsSegmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelSpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelSpecification_OperationsSegment_OperationsSegmentId",
                        column: x => x.OperationsSegmentId,
                        principalTable: "OperationsSegment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    WorkOrderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SegmentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationsSegmentId = table.Column<int>(type: "int", nullable: false),
                    WorkCenterId = table.Column<int>(type: "int", nullable: true),
                    SequenceNumber = table.Column<int>(type: "int", nullable: true),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false),
                    ProductionTarget = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<double>(type: "float", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActuallyStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActuallyEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkOrderStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    QualityStandardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetupTime = table.Column<double>(type: "float", nullable: false),
                    TeardownTime = table.Column<double>(type: "float", nullable: false),
                    RunTime = table.Column<double>(type: "float", nullable: false),
                    ManufacturingOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrder_ManufacturingOrder_ManufacturingOrderId",
                        column: x => x.ManufacturingOrderId,
                        principalTable: "ManufacturingOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrder_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrder_OperationsSegment_OperationsSegmentId",
                        column: x => x.OperationsSegmentId,
                        principalTable: "OperationsSegment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrder_WorkCenters_WorkCenterId",
                        column: x => x.WorkCenterId,
                        principalTable: "WorkCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentSpecificationProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentSpecificationId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentSpecificationProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentSpecificationProperty_EquipmentSpecification_EquipmentSpecificationId",
                        column: x => x.EquipmentSpecificationId,
                        principalTable: "EquipmentSpecification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialSpecificationProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialSpecificationId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialSpecificationProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialSpecificationProperty_MaterialSpecification_MaterialSpecificationId",
                        column: x => x.MaterialSpecificationId,
                        principalTable: "MaterialSpecification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonnelSpecificationProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelSpecificationId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonnelSpecificationProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonnelSpecificationProperty_PersonnelSpecification_PersonnelSpecificationId",
                        column: x => x.PersonnelSpecificationId,
                        principalTable: "PersonnelSpecification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentWorkOrders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentWorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentWorkOrders_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EquipmentWorkOrders_WorkOrder_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrder",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ManufacturingRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    WorkOrderId = table.Column<int>(type: "int", nullable: false),
                    OutputMaterialDefinitionId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Output = table.Column<decimal>(type: "decimal(30,8)", precision: 30, scale: 8, nullable: false),
                    Defects = table.Column<decimal>(type: "decimal(30,8)", precision: 30, scale: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManufacturingRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManufacturingRecords_MaterialDefinition_OutputMaterialDefinitionId",
                        column: x => x.OutputMaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ManufacturingRecords_WorkOrder_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrder",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MaterialWorkOrders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaterialDefinitionId = table.Column<int>(type: "int", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity_ValueType = table.Column<int>(type: "int", nullable: true),
                    ValueUnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialWorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialWorkOrders_MaterialDefinition_MaterialDefinitionId",
                        column: x => x.MaterialDefinitionId,
                        principalTable: "MaterialDefinition",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaterialWorkOrders_WorkOrder_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrder",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PersonWorkOrders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonWorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonWorkOrders_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonWorkOrders_WorkOrder_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrder",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EquipmentManufacturingRecord",
                columns: table => new
                {
                    EquipmentsId = table.Column<int>(type: "int", nullable: false),
                    ManufacturingRecordId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentManufacturingRecord", x => new { x.EquipmentsId, x.ManufacturingRecordId });
                    table.ForeignKey(
                        name: "FK_EquipmentManufacturingRecord_Equipment_EquipmentsId",
                        column: x => x.EquipmentsId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentManufacturingRecord_ManufacturingRecords_ManufacturingRecordId",
                        column: x => x.ManufacturingRecordId,
                        principalTable: "ManufacturingRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonWorkOrderShift",
                columns: table => new
                {
                    PersonWorkOrdersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShiftsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonWorkOrderShift", x => new { x.PersonWorkOrdersId, x.ShiftsId });
                    table.ForeignKey(
                        name: "FK_PersonWorkOrderShift_PersonWorkOrders_PersonWorkOrdersId",
                        column: x => x.PersonWorkOrdersId,
                        principalTable: "PersonWorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonWorkOrderShift_Shift_ShiftsId",
                        column: x => x.ShiftsId,
                        principalTable: "Shift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_ParentId",
                table: "Areas",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_BillOfMaterial_MaterialDefinitionId",
                table: "BillOfMaterial",
                column: "MaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPartnerProperty_BusinessPartnerId_PropertyId",
                table: "BusinessPartnerProperty",
                columns: new[] { "BusinessPartnerId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Downtimes_DowntimeTypeId",
                table: "Downtimes",
                column: "DowntimeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Downtimes_EquipmentId",
                table: "Downtimes",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Enterprises_ParentId",
                table: "Enterprises",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentClassId",
                table: "Equipment",
                column: "EquipmentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_HierarchyModelId",
                table: "Equipment",
                column: "HierarchyModelId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCapability_EquipmentId",
                table: "EquipmentCapability",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCapabilityProperty_EquipmentCapabilityId_PropertyId",
                table: "EquipmentCapabilityProperty",
                columns: new[] { "EquipmentCapabilityId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentClassProperty_EquipmentClassId_PropertyId",
                table: "EquipmentClassProperty",
                columns: new[] { "EquipmentClassId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentManufacturingRecord_ManufacturingRecordId",
                table: "EquipmentManufacturingRecord",
                column: "ManufacturingRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentProperty_EquipmentId_PropertyId",
                table: "EquipmentProperty",
                columns: new[] { "EquipmentId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSegmentSpecification_EquipmentClassId",
                table: "EquipmentSegmentSpecification",
                column: "EquipmentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSegmentSpecification_ProcessSegmentId",
                table: "EquipmentSegmentSpecification",
                column: "ProcessSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSegmentSpecificationProperty_EquipmentSegmentSpecificationId_PropertyId",
                table: "EquipmentSegmentSpecificationProperty",
                columns: new[] { "EquipmentSegmentSpecificationId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecification_OperationsSegmentId",
                table: "EquipmentSpecification",
                column: "OperationsSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentSpecificationProperty_EquipmentSpecificationId_PropertyId",
                table: "EquipmentSpecificationProperty",
                columns: new[] { "EquipmentSpecificationId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentWorkOrders_EquipmentId",
                table: "EquipmentWorkOrders",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentWorkOrders_WorkOrderId",
                table: "EquipmentWorkOrders",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_HierarchyModel_HierarchyModelId",
                table: "HierarchyModel",
                column: "HierarchyModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturingOrder_CreatedById",
                table: "ManufacturingOrder",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturingOrder_CustomerId",
                table: "ManufacturingOrder",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturingOrder_ManufacturingOrderId",
                table: "ManufacturingOrder",
                column: "ManufacturingOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturingOrder_MaterialDefinitionId",
                table: "ManufacturingOrder",
                column: "MaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturingOrder_OperationsDefinitionId",
                table: "ManufacturingOrder",
                column: "OperationsDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturingOrder_QualityStandardId",
                table: "ManufacturingOrder",
                column: "QualityStandardId");

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturingRecords_OutputMaterialDefinitionId",
                table: "ManufacturingRecords",
                column: "OutputMaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturingRecords_WorkOrderId_StartTime",
                table: "ManufacturingRecords",
                columns: new[] { "WorkOrderId", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialClassProperty_MaterialClassId_PropertyId",
                table: "MaterialClassProperty",
                columns: new[] { "MaterialClassId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDefinition_MaterialClassId",
                table: "MaterialDefinition",
                column: "MaterialClassId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDefinitionProperty_MaterialDefinitionId_PropertyId",
                table: "MaterialDefinitionProperty",
                columns: new[] { "MaterialDefinitionId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDefinitionQualityStandard_QualityStandardsId",
                table: "MaterialDefinitionQualityStandard",
                column: "QualityStandardsId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialLot_MaterialDefinitionId",
                table: "MaterialLot",
                column: "MaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialLot_UnitId",
                table: "MaterialLot",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialLotProperty_MaterialLotId_PropertyId",
                table: "MaterialLotProperty",
                columns: new[] { "MaterialLotId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSegmentSpecification_MaterialDefinitionId",
                table: "MaterialSegmentSpecification",
                column: "MaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSegmentSpecification_ProcessSegmentId",
                table: "MaterialSegmentSpecification",
                column: "ProcessSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSegmentSpecificationProperty_MaterialSegmentSpecificationId_PropertyId",
                table: "MaterialSegmentSpecificationProperty",
                columns: new[] { "MaterialSegmentSpecificationId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSpecification_OperationsSegmentId",
                table: "MaterialSpecification",
                column: "OperationsSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSpecificationProperty_MaterialSpecificationId_PropertyId",
                table: "MaterialSpecificationProperty",
                columns: new[] { "MaterialSpecificationId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSubLot_MaterialDefinitionId",
                table: "MaterialSubLot",
                column: "MaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSubLot_ParentMaterialLotId",
                table: "MaterialSubLot",
                column: "ParentMaterialLotId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSubLot_UnitId",
                table: "MaterialSubLot",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialUnit_MaterialDefinitionId",
                table: "MaterialUnit",
                column: "MaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialUnit_UnitId_MaterialDefinitionId",
                table: "MaterialUnit",
                columns: new[] { "UnitId", "MaterialDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialWorkOrders_MaterialDefinitionId",
                table: "MaterialWorkOrders",
                column: "MaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialWorkOrders_WorkOrderId",
                table: "MaterialWorkOrders",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Operation_OperationId_MaterialDefinitionId",
                table: "Operation",
                columns: new[] { "OperationId", "MaterialDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationOperation_PrerequisiteOperationId",
                table: "OperationOperation",
                column: "PrerequisiteOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationsDefinition_MaterialDefinitionId",
                table: "OperationsDefinition",
                column: "MaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationsMaterialBill_OperationsDefinitionId",
                table: "OperationsMaterialBill",
                column: "OperationsDefinitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationsMaterialBillItem_OperationsMaterialBillId",
                table: "OperationsMaterialBillItem",
                column: "OperationsMaterialBillId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationsSegment_OperationsDefinitionId",
                table: "OperationsSegment",
                column: "OperationsDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_OperationsSegmentDependency_OperationsDefinitionId",
                table: "OperationsSegmentDependency",
                column: "OperationsDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_PersonnelClassId",
                table: "Person",
                column: "PersonnelClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelCapability_PersonId",
                table: "PersonnelCapability",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelCapabilityProperty_CapabilityId_PropertyId",
                table: "PersonnelCapabilityProperty",
                columns: new[] { "CapabilityId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelClassProperty_PersonClassId_PropertyId",
                table: "PersonnelClassProperty",
                columns: new[] { "PersonClassId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSegmentSpecification_PersonnelClassId",
                table: "PersonnelSegmentSpecification",
                column: "PersonnelClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSegmentSpecification_ProcessSegmentId",
                table: "PersonnelSegmentSpecification",
                column: "ProcessSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSegmentSpecificationProperty_PersonnelSegmentSpecificationId_PropertyId",
                table: "PersonnelSegmentSpecificationProperty",
                columns: new[] { "PersonnelSegmentSpecificationId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSpecification_OperationsSegmentId",
                table: "PersonnelSpecification",
                column: "OperationsSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonnelSpecificationProperty_PersonnelSpecificationId_PropertyId",
                table: "PersonnelSpecificationProperty",
                columns: new[] { "PersonnelSpecificationId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonProperty_PersonId_PropertyId",
                table: "PersonProperty",
                columns: new[] { "PersonId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonWorkOrders_PersonId",
                table: "PersonWorkOrders",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonWorkOrders_WorkOrderId",
                table: "PersonWorkOrders",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonWorkOrderShift_ShiftsId",
                table: "PersonWorkOrderShift",
                column: "ShiftsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessSegment_ProductionParameterId",
                table: "ProcessSegment",
                column: "ProductionParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionParameterProperty_ProductionParameterId_PropertyId",
                table: "ProductionParameterProperty",
                columns: new[] { "ProductionParameterId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QualityStandardProperty_QualityStandardId_PropertyId",
                table: "QualityStandardProperty",
                columns: new[] { "QualityStandardId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceNetworkConnection_ConnectionId",
                table: "ResourceNetworkConnection",
                column: "ConnectionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceNetworkConnection_FromResourceId",
                table: "ResourceNetworkConnection",
                column: "FromResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceNetworkConnection_ResourceRelationshipNetworkId",
                table: "ResourceNetworkConnection",
                column: "ResourceRelationshipNetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceNetworkConnection_ToResourceId",
                table: "ResourceNetworkConnection",
                column: "ToResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceNetworkConnectionProperty_ConnectionId_PropertyId",
                table: "ResourceNetworkConnectionProperty",
                columns: new[] { "ConnectionId", "PropertyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceRelationshipNetworks_ResourceRelationshipNetworkId",
                table: "ResourceRelationshipNetworks",
                column: "ResourceRelationshipNetworkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ResourceId",
                table: "Resources",
                column: "ResourceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shift_ScheduleId_ShiftName",
                table: "Shift",
                columns: new[] { "ScheduleId", "ShiftName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_ParentId",
                table: "Sites",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_User_PersonId",
                table: "User",
                column: "PersonId",
                unique: true,
                filter: "[PersonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCenterOutputs_MaterialDefinitionId",
                table: "WorkCenterOutputs",
                column: "MaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCenterOutputs_UnitId",
                table: "WorkCenterOutputs",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkCenterOutputs_WorkCenterId_MaterialDefinitionId",
                table: "WorkCenterOutputs",
                columns: new[] { "WorkCenterId", "MaterialDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkCenters_ParentId",
                table: "WorkCenters",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_ManufacturingOrderId",
                table: "WorkOrder",
                column: "ManufacturingOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_MaterialDefinitionId",
                table: "WorkOrder",
                column: "MaterialDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_OperationsSegmentId",
                table: "WorkOrder",
                column: "OperationsSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_WorkCenterId",
                table: "WorkOrder",
                column: "WorkCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_WorkOrderId",
                table: "WorkOrder",
                column: "WorkOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkUnits_ParentId",
                table: "WorkUnits",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillOfMaterial");

            migrationBuilder.DropTable(
                name: "BusinessPartnerProperty");

            migrationBuilder.DropTable(
                name: "Downtimes");

            migrationBuilder.DropTable(
                name: "EquipmentCapabilityProperty");

            migrationBuilder.DropTable(
                name: "EquipmentClassProperty");

            migrationBuilder.DropTable(
                name: "EquipmentManufacturingRecord");

            migrationBuilder.DropTable(
                name: "EquipmentProperty");

            migrationBuilder.DropTable(
                name: "EquipmentSegmentSpecificationProperty");

            migrationBuilder.DropTable(
                name: "EquipmentSpecificationProperty");

            migrationBuilder.DropTable(
                name: "EquipmentWorkOrders");

            migrationBuilder.DropTable(
                name: "MaterialClassProperty");

            migrationBuilder.DropTable(
                name: "MaterialDefinitionProperty");

            migrationBuilder.DropTable(
                name: "MaterialDefinitionQualityStandard");

            migrationBuilder.DropTable(
                name: "MaterialLotProperty");

            migrationBuilder.DropTable(
                name: "MaterialSegmentSpecificationProperty");

            migrationBuilder.DropTable(
                name: "MaterialSpecificationProperty");

            migrationBuilder.DropTable(
                name: "MaterialSubLot");

            migrationBuilder.DropTable(
                name: "MaterialWorkOrders");

            migrationBuilder.DropTable(
                name: "OperationOperation");

            migrationBuilder.DropTable(
                name: "OperationsMaterialBillItem");

            migrationBuilder.DropTable(
                name: "OperationsSegmentDependency");

            migrationBuilder.DropTable(
                name: "PersonnelCapabilityProperty");

            migrationBuilder.DropTable(
                name: "PersonnelClassProperty");

            migrationBuilder.DropTable(
                name: "PersonnelSegmentSpecificationProperty");

            migrationBuilder.DropTable(
                name: "PersonnelSpecificationProperty");

            migrationBuilder.DropTable(
                name: "PersonProperty");

            migrationBuilder.DropTable(
                name: "PersonWorkOrderShift");

            migrationBuilder.DropTable(
                name: "ProcessSegmentDependency");

            migrationBuilder.DropTable(
                name: "ProductionParameterProperty");

            migrationBuilder.DropTable(
                name: "QualityStandardProperty");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ResourceNetworkConnectionProperty");

            migrationBuilder.DropTable(
                name: "WorkCenterOutputs");

            migrationBuilder.DropTable(
                name: "WorkOrderErrorRecord");

            migrationBuilder.DropTable(
                name: "WorkOrderProgressRecord");

            migrationBuilder.DropTable(
                name: "WorkUnits");

            migrationBuilder.DropTable(
                name: "EquipmentDowntimeType");

            migrationBuilder.DropTable(
                name: "EquipmentCapability");

            migrationBuilder.DropTable(
                name: "ManufacturingRecords");

            migrationBuilder.DropTable(
                name: "EquipmentSegmentSpecification");

            migrationBuilder.DropTable(
                name: "EquipmentSpecification");

            migrationBuilder.DropTable(
                name: "MaterialSegmentSpecification");

            migrationBuilder.DropTable(
                name: "MaterialSpecification");

            migrationBuilder.DropTable(
                name: "MaterialLot");

            migrationBuilder.DropTable(
                name: "Operation");

            migrationBuilder.DropTable(
                name: "OperationsMaterialBill");

            migrationBuilder.DropTable(
                name: "PersonnelCapability");

            migrationBuilder.DropTable(
                name: "PersonnelSegmentSpecification");

            migrationBuilder.DropTable(
                name: "PersonnelSpecification");

            migrationBuilder.DropTable(
                name: "PersonWorkOrders");

            migrationBuilder.DropTable(
                name: "Shift");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "ResourceNetworkConnection");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "MaterialUnit");

            migrationBuilder.DropTable(
                name: "ProcessSegment");

            migrationBuilder.DropTable(
                name: "WorkOrder");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "ResourceRelationshipNetworks");

            migrationBuilder.DropTable(
                name: "EquipmentClass");

            migrationBuilder.DropTable(
                name: "ProductionParameter");

            migrationBuilder.DropTable(
                name: "ManufacturingOrder");

            migrationBuilder.DropTable(
                name: "OperationsSegment");

            migrationBuilder.DropTable(
                name: "WorkCenters");

            migrationBuilder.DropTable(
                name: "BusinessPartner");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "QualityStandard");

            migrationBuilder.DropTable(
                name: "OperationsDefinition");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "PersonnelClass");

            migrationBuilder.DropTable(
                name: "MaterialDefinition");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "MaterialClass");

            migrationBuilder.DropTable(
                name: "Enterprises");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "HierarchyModel");

            migrationBuilder.DropSequence(
                name: "areaeq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "enterpriseeq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "hierarchymodeleq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "manufacturingorderseq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "manufacturingrecordeq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "materialuniteq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "operationeq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "resourcenetworkconnectioneq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "resourcerelationshipnetworkeq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "siteeq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "workcentereq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "workcenteroutputeq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "workorderseq",
                schema: "application");

            migrationBuilder.DropSequence(
                name: "workuniteq",
                schema: "application");
        }
    }
}
