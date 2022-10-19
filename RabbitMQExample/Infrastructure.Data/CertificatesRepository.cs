using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Data;

public class CertificatesRepository : GenericRepository<CertificateEntity>, ICertificateRepository
{
    public CertificatesRepository(DomainContext context) : base(context)
    {
    }
}