using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailVerfication.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyToPayRoll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			// Add the foreign key constraint between PayRolls and Users tables
			migrationBuilder.AddForeignKey(
				name: "FK_PayRolls_Users_Id", // Foreign key name
				table: "PayRolls", // Table that holds the foreign key
				column: "Id", // Column in PayRolls table that references the foreign key
				principalTable: "Users", // Principal table (Users) to which the foreign key references
				principalColumn: "Id", // Column in Users table to which the foreign key refers
				onDelete: ReferentialAction.Cascade);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			// Remove the foreign key constraint
			migrationBuilder.DropForeignKey(
				name: "FK_PayRolls_Users_Id", // Name of the foreign key constraint to drop
				table: "PayRolls");
		}
    }
}
