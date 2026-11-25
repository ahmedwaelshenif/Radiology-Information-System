using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedModalityPrices2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Prices.Name was previously stored as the modality name string.
            // Convert the seeded names to their Modality enum integer values before changing the column type.
            migrationBuilder.Sql(@"
                UPDATE [Prices]
                SET [Name] = CASE [Name]
                    WHEN N'MRI' THEN N'0'
                    WHEN N'CT' THEN N'1'
                    WHEN N'XRay' THEN N'2'
                    WHEN N'Ultrasound' THEN N'3'
                    WHEN N'PET' THEN N'4'
                    WHEN N'Mammography' THEN N'5'
                    WHEN N'Fluoroscopy' THEN N'6'
                    ELSE [Name]
                END;");

            migrationBuilder.AlterColumn<int>(
                name: "Name",
                table: "Prices",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Prices",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "MRI");

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "CT");

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "XRay");

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Ultrasound");

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "PET");

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Mammography");

            migrationBuilder.UpdateData(
                table: "Prices",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "Fluoroscopy");
        }
    }
}
