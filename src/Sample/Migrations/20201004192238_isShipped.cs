using Microsoft.EntityFrameworkCore.Migrations;

namespace Sample.Migrations
{
    public partial class isShipped : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsShipped",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsShipped",
                table: "Orders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "0");
        }
    }
}
