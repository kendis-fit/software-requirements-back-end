using Microsoft.EntityFrameworkCore.Migrations;

namespace SoftwareRequirements.Migrations
{
    public partial class FieldModify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Write",
                table: "Requirements",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Write",
                table: "Requirements");
        }
    }
}
