using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatisticService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class editDefaultYearStatisticModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<int>>(
                name: "Colspan",
                table: "DefaultYearStatisticEntities",
                type: "integer[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Colspan",
                table: "DefaultYearStatisticEntities");
        }
    }
}
