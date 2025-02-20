using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyShop.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Content of Message",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Receiver of Message");

            migrationBuilder.AddColumn<string>(
                name: "Recipient",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "Recipient of Message");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Recipient",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Receiver of Message",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Content of Message");
        }
    }
}
