using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedStudyUIDs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyUIDs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Modality = table.Column<int>(type: "int", nullable: false),
                    StudyInstanceUID = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyUIDs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "StudyUIDs",
                columns: new[] { "Id", "Modality", "StudyInstanceUID" },
                values: new object[,]
                {
                    { 1, 2, "1.3.6.1.4.1.14519.5.2.1.9999.103.2003757953720308493720544525327" },
                    { 2, 2, "1.2.392.200036.9125.0.199302241758.16" },
                    { 3, 1, "1.3.6.1.4.1.14519.5.2.1.6919.4624.107720852982298931479046337475" },
                    { 4, 1, "1.3.6.1.4.1.14519.5.2.1.6919.4624.508771521556732938904058410751" },
                    { 5, 5, "1.3.6.1.4.1.14519.5.2.1.1239.1759.860502043886043938075747571015" },
                    { 6, 0, "1.2.840.4892943.343.20240528040031.5710.1656205" },
                    { 7, 0, "1.3.46.670589.11.84425.5.0.14744.2025051016220214026" },
                    { 8, 0, "1.2.840.4892943.343.20250413070448.1553.2122473" },
                    { 9, 3, "1.3.6.1.4.1.14519.5.2.1.1.23350688958676093611472873878291718902" },
                    { 10, 3, "1.3.6.1.4.1.14519.5.2.1.205603453180402812780236219545985083918" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyUIDs");
        }
    }
}
