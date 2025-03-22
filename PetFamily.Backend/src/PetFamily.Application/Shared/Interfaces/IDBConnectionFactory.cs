using System.Data;

namespace PetFamily.Application.Shared.Interfaces;

public interface IDBConnectionFactory
{
    public IDbConnection Create();
}
