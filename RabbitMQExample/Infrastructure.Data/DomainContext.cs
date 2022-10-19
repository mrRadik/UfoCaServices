using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data;

public class DomainContext : DbContext
{
    public DomainContext(DbContextOptions<DomainContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<LogEntity>(ConfigureLogs);
        builder.Entity<CertificateEntity>(ConfigureCertificates);
    }
    private void ConfigureLogs(EntityTypeBuilder<LogEntity> builder)
    {
        builder.ToTable("Logs");
        builder.HasKey(x => x.Id);
    }
    
    private void ConfigureCertificates(EntityTypeBuilder<CertificateEntity> builder)
    {
        builder.ToTable("Certificates");
        builder.HasKey(x => x.Id);
    }
}