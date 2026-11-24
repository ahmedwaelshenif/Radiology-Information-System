using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [Email] = N'aaa@aaa')
BEGIN
    DECLARE @AdminId int = ISNULL((SELECT MAX([Id]) FROM [Users]), 0) + 1;
    SET IDENTITY_INSERT [Users] ON;
    INSERT INTO [Users]
        ([Id], [FullName], [Role], [IsActive], [UserName], [NormalizedUserName],
         [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp],
         [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled],
         [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
    VALUES
        (@AdminId, N'Admin', 0, 1, N'aaa@aaa', N'AAA@AAA', N'aaa@aaa', N'AAA@AAA', 1,
         N'AQAAAAIAAYagAAAAEMDl9X2F0YzU5Jj/+P4blXtqXAkUTupAme6VxMUnV84OVPnJgThcfDBbexEb+rEl3g==',
         N'8f86d4a8-0171-42c9-89df-3a87cbe278ae', N'3c7e3c09-9f65-459c-af3d-3c01f88f7830',
         NULL, 0, 0, NULL, 0, 0);
    SET IDENTITY_INSERT [Users] OFF;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM [Users]
WHERE [Email] = N'aaa@aaa'
  AND [NormalizedEmail] = N'AAA@AAA'
  AND [Role] = 0");
        }
    }
}
