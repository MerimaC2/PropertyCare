using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRefreshTokenFingerprint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fingerprint",
                table: "RefreshTokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Fingerprint",
                table: "RefreshTokens",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
