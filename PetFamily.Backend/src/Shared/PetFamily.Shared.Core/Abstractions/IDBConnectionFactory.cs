using System.Data;

namespace PetFamily.Shared.Core.Abstractions;

public interface IDBConnectionFactory
{
    public IDbConnection Create();
}
