namespace MesMicroservice.Infrastructure.EntityConfigurations.ScheduleConfigurations
{
    public class ShiftEntityTypeConfiguration : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            builder.ToTable(nameof(Shift));

            builder.HasKey(s => s.Id);

            builder.Property(s => s.ScheduleId)
                .IsRequired();

            builder.Property(s => s.ShiftName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.StartTime)
                .IsRequired()
                .HasConversion(
                    v => v.TotalSeconds,               
                    v => TimeSpan.FromSeconds(v)
                );

            builder.Property(s => s.EndTime)
                .IsRequired()
                .HasConversion(
                    v => v.TotalSeconds,
                    v => TimeSpan.FromSeconds(v)
                );

            builder.HasIndex(s => new { s.ScheduleId, s.ShiftName }).IsUnique();
        }
    }
}
