namespace MesMicroservice.Infrastructure.EntityConfigurations.ScheduleConfigurations
{
    public class ScheduleEntityTypeConfigration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable(nameof(Schedule));

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.ActiveDays)
                .IsRequired()
                .HasConversion<int>();

            builder.HasMany<Shift>("_shifts")
                .WithOne()
                .HasForeignKey(s => s.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(s => s.Shifts);
        }
    }
}
