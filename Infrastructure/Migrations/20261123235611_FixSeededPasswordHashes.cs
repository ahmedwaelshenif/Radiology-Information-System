using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSeededPasswordHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE [Users]
SET [PasswordHash] = N'AQAAAAIAAYagAAAAEMDl9X2F0YzU5Jj/+P4blXtqXAkUTupAme6VxMUnV84OVPnJgThcfDBbexEb+rEl3g=='
WHERE [NormalizedEmail] IN (
    N'AAA@AAA',
    N'DEMO.RADIOLOGIST@EXAMPLE.TEST',
    N'DEMO.RECEPTIONIST@EXAMPLE.TEST',
    N'DEMO.PATIENT1@EXAMPLE.TEST',
    N'DEMO.PATIENT2@EXAMPLE.TEST',
    N'DEMO.PATIENT3@EXAMPLE.TEST',
    N'DEMO.PATIENT4@EXAMPLE.TEST'
)
AND [PasswordHash] = N'AQAAAAEAAYag2kki+IMP+XClS2R9KZILW22Qp8E7CGhgvFZmLnLAiXLZEfd4k5RjzrP5HCfj3czu'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
