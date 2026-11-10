global using MesMicroservice.Infrastructure.EntityConfigurations.ProductionParameterConfigurations;
global using MesMicroservice.Infrastructure.EntityConfigurations.QualityStandardConfigurations;
global using MesMicroservice.Infrastructure.EntityConfigurations.BomModelConfigurations;
global using MesMicroservice.Infrastructure.EntityConfigurations.OperationModelConfigurations;
global using MesMicroservice.Infrastructure.EntityConfigurations.SegmentModelConfigurations;
global using MesMicroservice.Infrastructure.EntityConfigurations.BusinessPartnerConfigurations;

namespace MesMicroservice.Infrastructure;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public const string DEFAULT_SCHEMA = "application";

    private IDbContextTransaction? _currentTransaction;
    private readonly IMediator _mediator;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ApplicationDbContext(DbContextOptions options) : base(options) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public ApplicationDbContext(DbContextOptions options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction != null;

    public DbSet<Enterprise> Enterprises { get; set; }

    public DbSet<EquipmentClass> EquipmentClasses { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<EquipmentCapability> EquipmentCapabilities { get; set; }

    public DbSet<WorkCenterOutput> WorkCenterOutputs { get; set; }

    public DbSet<ManufacturingOrder> ManufacturingOrders { get; set; }
    public DbSet<ManufacturingRecord> ManufacturingRecords { get; set; }

    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<WorkOrderProgressRecord> WorkOrderProgressRecords { get; set; }
    public DbSet<WorkOrderErrorRecord> WorkOrderErrorRecords { get; set; }
    public DbSet<EquipmentWorkOrder> EquipmentWorkOrders { get; set; }
    public DbSet<PersonWorkOrder> PersonWorkOrders { get; set; }
    public DbSet<MaterialWorkOrder> MaterialWorkOrders { get; set; }

    public DbSet<MaterialClass> MaterialClasses { get; set; }
    public DbSet<MaterialDefinition> MaterialDefinitions { get; set; }
    public DbSet<MaterialLot> MaterialLots { get; set; }
    public DbSet<MaterialSubLot> MaterialSubLots { get; set; }

    public DbSet<Resource> Resources { get; set; }
    public DbSet<ResourceRelationshipNetwork> ResourceRelationshipNetworks { get; set; }

    public DbSet<Schedule> Schedules { get; set; }

    public DbSet<Downtime> Downtimes { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    public DbSet<Person> Persons { get; set; }
    public DbSet<PersonnelCapability> PersonnelCapabilities { get; set; }
    public DbSet<PersonnelClass> PersonnelClasses { get; set; }

    public DbSet<ProductionParameter> ProductionParameters { get; set; }
    public DbSet<QualityStandard> QualityStandards { get; set; }

    public DbSet<ProcessSegment> ProcessSegments { get; set; }
    public DbSet<ProcessSegmentDependency> ProcessSegmentDependencies { get; set; }
    public DbSet<PersonnelSegmentSpecification> PersonnelSegmentSpecifications { get; set; }
    public DbSet<MaterialSegmentSpecification> MaterialSegmentSpecifications { get; set; }
    public DbSet<EquipmentSegmentSpecification> EquipmentSegmentSpecifications { get; set; }

    public DbSet<BillOfMaterial> BillOfMaterials { get; set; }

    public DbSet<OperationsDefinition> OperationsDefinitions { get; set; }
    public DbSet<OperationsMaterialBill> OperationsMaterialBills { get; set; }
    public DbSet<OperationsMaterialBillItem> OperationsMaterialBillItems { get; set; }
    public DbSet<OperationsSegment> OperationsSegments { get; set; }
    public DbSet<OperationsSegmentDependency> OperationsSegmentDependencies { get; set; }
    public DbSet<EquipmentSpecification> EquipmentSpecifications { get; set; }
    public DbSet<PersonnelSpecification> PersonnelSpecifications { get; set; }
    public DbSet<MaterialSpecification> MaterialSpecifications { get; set; }

    public DbSet<BusinessPartner> BusinessPartners { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ResourceEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ResourceRelationshipNetworkEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new HierarchyModelEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EnterpriseEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SiteEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AreaEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new WorkCenterEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new WorkUnitEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new WorkCenterOutputEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new EquipmentClassEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentCapabilityEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new ManufacturingOrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ManufacturingRecordEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new WorkOrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new WorkOrderProgressRecordEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new WorkOrderErrorRecordEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentWorkOrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PersonWorkOrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MaterialWorkOrderEntityTypeConfiguration());


        modelBuilder.ApplyConfiguration(new MaterialClassEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MaterialDefinitionEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MaterialUnitEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MaterialLotEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MaterialSubLotEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new Old_OperationEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TokenEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new PersonEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PersonnelClassEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PersonnelCapabilityEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new ScheduleEntityTypeConfigration());
        modelBuilder.ApplyConfiguration(new ShiftEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new ProductionParameterEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new QualityStandardEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new ProcessSegmentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProcessSegmentDependencyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PersonnelSegmentSpecificationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MaterialSegmentSpecificationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentSegmentSpecificationEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new BomEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new OperationsDefinitionEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OperationsMaterialBillEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OperationsMaterialBillItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OperationsSegmentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OperationsSegmentDependencyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EquipmentSpecificationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PersonnelSpecificationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MaterialSpecificationEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new BusinessPartnerEntityTypeConfiguration());

    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEventsAsync(this);
        await base.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_currentTransaction != null) return null;

        _currentTransaction = await Database.BeginTransactionAsync();

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            transaction.Commit();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}