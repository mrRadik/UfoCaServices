using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgre;

public class CertificatePostgreRepository : GenericPostgreRepository<CertificateEntity>, ICertificateRepository
{
    public CertificatePostgreRepository(PostgreContext context) : base(context)
    {
    }
}