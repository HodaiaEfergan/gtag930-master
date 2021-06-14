using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class default_unit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UnitInfos",
                columns: new[] { "Id", "HexRgbColor", "Name" },
                values: new object[] { "DEFAULT_UNIT_INFO_ID", "FF0000", "Default Unit" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UnitInfos",
                keyColumn: "Id",
                keyValue: "DEFAULT_UNIT_INFO_ID");
        }
    }
}
