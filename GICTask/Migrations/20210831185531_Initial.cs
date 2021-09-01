using Microsoft.EntityFrameworkCore.Migrations;

namespace GICTask.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actuals",
                columns: table => new
                {
                    State = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualPopulation = table.Column<double>(type: "REAL", nullable: false),
                    ActualHouseholds = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actuals", x => x.State);
                });

            migrationBuilder.CreateTable(
                name: "Estimates",
                columns: table => new
                {
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Districts = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimatesPopulation = table.Column<float>(type: "REAL", nullable: false),
                    EstimatesHouseholds = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estimates", x => new { x.State, x.Districts });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actuals");

            migrationBuilder.DropTable(
                name: "Estimates");
        }
    }
}
