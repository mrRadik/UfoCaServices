using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain;

public class DomainContext : DbContext
{
    public DomainContext(DbContextOptions<DomainContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<LogEntity>(ConfigureLog);
    }
    private void ConfigureLog(EntityTypeBuilder<LogEntity> builder)
    {
        builder.ToTable("Logs");
        builder.HasKey(x => x.Id);
    }
}