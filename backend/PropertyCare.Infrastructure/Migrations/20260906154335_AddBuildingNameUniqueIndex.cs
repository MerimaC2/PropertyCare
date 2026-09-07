using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildingNameUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Buildings_TenantId",
                table: "Buildings");

            migrationBuilder.AddColumn<string>(
                name: "NameNormalized",
                table: "Buildings",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            // Backfill before the unique index is created: without this every existing row would
            // carry the empty default and the index could not be built. Mirrors
            // BuildingEntity.NormalizeName.
            migrationBuilder.Sql(
                "UPDATE [Buildings] SET [NameNormalized] = UPPER(LTRIM(RTRIM([Name])));");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_TenantId_NameNormalized",
                table: "Buildings",
                columns: new[] { "TenantId", "NameNormalized" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Buildings_TenantId_NameNormalized",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "NameNormalized",
                table: "Buildings");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_TenantId",
                table: "Buildings",
                column: "TenantId");
        }
    }
}
