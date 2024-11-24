using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GraphQlDotNetCore.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Owners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Owners",
                columns: new[] { "Id", "Address", "Name" },
                values: new object[,]
                {
                    { new Guid("5e6455ff-12c0-4b6b-80f8-e2ff35d68151"), "John Doe's address", "John Doe" },
                    { new Guid("8a5d6e67-2802-4c7a-a337-dbd755e744bc"), "Jane Doe's address", "Jane Doe" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Description", "OwnerId", "Type" },
                values: new object[,]
                {
                    { new Guid("1e703cc7-2847-428f-a6b6-411e93e6a649"), "Income account for our users", new Guid("8a5d6e67-2802-4c7a-a337-dbd755e744bc"), 3 },
                    { new Guid("7ac048f1-9aba-47e2-a8d8-b88bdfd271c7"), "Cash account for our users", new Guid("5e6455ff-12c0-4b6b-80f8-e2ff35d68151"), 0 },
                    { new Guid("b8462ef1-077d-451c-8669-212e32d6fa9a"), "Savings account for our users", new Guid("8a5d6e67-2802-4c7a-a337-dbd755e744bc"), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_OwnerId",
                table: "Accounts",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Owners");
        }
    }
}
