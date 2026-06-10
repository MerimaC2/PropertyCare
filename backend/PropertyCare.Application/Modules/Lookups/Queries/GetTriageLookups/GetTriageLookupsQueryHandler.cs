using PropertyCare.Application.Abstractions;
using PropertyCare.Domain.Entities.Identity;

namespace PropertyCare.Application.Modules.Lookups.Queries.GetTriageLookups;

public sealed class GetTriageLookupsQueryHandler
    : IRequestHandler<GetTriageLookupsQuery, GetTriageLookupsQueryDto>
{
    private readonly IAppDbContext _ctx;

    public GetTriageLookupsQueryHandler(IAppDbContext ctx) => _ctx = ctx;

    public async Task<GetTriageLookupsQueryDto> Handle(
        GetTriageLookupsQuery request,
        CancellationToken ct)
    {
        var technicians = await _ctx.Users.AsNoTracking()
            .Where(u => u.Role.Name == UserRoleEntity.Names.Technician && u.IsActive && !u.IsDeleted)
            .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
            .Select(u => new LookupItemDto { Id = u.Id, Name = u.FirstName + " " + u.LastName })
            .ToListAsync(ct);

        var statuses = await _ctx.RequestStatuses.AsNoTracking()
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.Id)
            .Select(s => new LookupItemDto { Id = s.Id, Name = s.Name })
            .ToListAsync(ct);

        var priorities = await _ctx.RequestPriorities.AsNoTracking()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Id)
            .Select(p => new LookupItemDto { Id = p.Id, Name = p.Name })
            .ToListAsync(ct);

        var buildings = await _ctx.Buildings.AsNoTracking()
            .Where(b => !b.IsDeleted)
            .OrderBy(b => b.Name)
            .Select(b => new LookupItemDto { Id = b.Id, Name = b.Name })
            .ToListAsync(ct);

        return new GetTriageLookupsQueryDto
        {
            Technicians = technicians,
            Statuses = statuses,
            Priorities = priorities,
            Buildings = buildings
        };
    }
}
