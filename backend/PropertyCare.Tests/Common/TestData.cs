using PropertyCare.Domain.Entities.Facilities;
using PropertyCare.Domain.Entities.Identity;
using PropertyCare.Infrastructure.Database;

namespace PropertyCare.Tests.Common;

/// <summary>Helpers for inserting common dynamic test data (users, buildings...).</summary>
public static class TestData
{
    public static AppUserEntity AddUser(
        DatabaseContext ctx, int roleId, string email, bool isActive = true)
    {
        var user = new AppUserEntity
        {
            TenantId = 1,
            RoleId = roleId,
            FirstName = "Test",
            LastName = "User",
            Email = email,
            PasswordHash = "hash",
            IsActive = isActive
        };
        ctx.Users.Add(user);
        ctx.SaveChanges();
        return user;
    }

    public static BuildingEntity AddBuilding(DatabaseContext ctx, string name = "Test Building")
    {
        var building = new BuildingEntity
        {
            TenantId = 1,
            BuildingTypeId = 1,
            Name = name
        };
        ctx.Buildings.Add(building);
        ctx.SaveChanges();
        return building;
    }
}
