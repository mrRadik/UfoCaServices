using Domain.Entities;

namespace Domain.Repositories.Implementations;

public class CertificatesRepository : GenericRepository<CertificateEntity>, ICertificateRepository
{
    public CertificatesRepository(DomainContext context) : base(context)
    {
    }
}