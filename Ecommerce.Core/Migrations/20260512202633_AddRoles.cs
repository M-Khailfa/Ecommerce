using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        private const string AdminRoleId = "f02916b5-9c70-4adb-9770-11c173a25f57";
        private const string UserRoleId = "9b14a171-6d6d-487b-bd43-8eeeca2c1d1e";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    {AdminRoleId, "Admin", "ADMIN", AdminRoleId },
                    {UserRoleId, "User", "USER", UserRoleId }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValues: new object[] { AdminRoleId, UserRoleId });
        }

    }
}
