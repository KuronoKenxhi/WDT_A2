using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace s3901335_a2.Migrations
{
    /// <inheritdoc />
    public partial class _12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_DestinationAccountNumber",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "DestinationAccountNumber",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_DestinationAccountNumber",
                table: "Transactions",
                column: "DestinationAccountNumber",
                principalTable: "Accounts",
                principalColumn: "AccountNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_DestinationAccountNumber",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "DestinationAccountNumber",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_DestinationAccountNumber",
                table: "Transactions",
                column: "DestinationAccountNumber",
                principalTable: "Accounts",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
