using PropertyManagement.Api.Common;
using PropertyManagement.Api.DTOs.Shared;
using PropertyManagement.Api.DTOs.Units;

namespace PropertyManagement.Api.Services.Units
{
    public interface IUnitService
    {
        Task<PagedResponse<UnitResponse>> GetAllUnitsAsync(QueryFilter filter, CancellationToken cancellationToken = default);
    }
}
