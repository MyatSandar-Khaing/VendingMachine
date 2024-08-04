using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VendingMachine.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "ID", "Email", "Name", "Password", "Role" },
                values: new object[,]
                {
                    { 1, "admin1@gmail.com", "Admin1", "P@ssw0rd", "admin" },
                    { 2, "user1@gmail.com", "User1", "P@ssw0rd", "user" },
                    { 3, "user2@gmail.com", "User2", "P@ssw0rd", "user" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "ID",
                keyValue: 3);
        }
    }
}
