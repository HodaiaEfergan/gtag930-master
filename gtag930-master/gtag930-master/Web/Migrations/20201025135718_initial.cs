using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "AspNetRoles",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_AspNetRoles", x => x.Id); });

            migrationBuilder.CreateTable(
                "AspNetUsers",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_AspNetUsers", x => x.Id); });

            migrationBuilder.CreateTable(
                "DeviceCodes",
                table => new
                {
                    UserCode = table.Column<string>(maxLength: 200, nullable: false),
                    DeviceCode = table.Column<string>(maxLength: 200, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(maxLength: 50000, nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_DeviceCodes", x => x.UserCode); });

            migrationBuilder.CreateTable(
                "PersistedGrants",
                table => new
                {
                    Key = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Data = table.Column<string>(maxLength: 50000, nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_PersistedGrants", x => x.Key); });

            migrationBuilder.CreateTable(
                "UnitInfos",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    HexRgbColor = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_UnitInfos", x => x.Id); });

            migrationBuilder.CreateTable(
                "AspNetRoleClaims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        x => x.RoleId,
                        "AspNetRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserClaims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetUserClaims_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserLogins",
                table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new {x.LoginProvider, x.ProviderKey});
                    table.ForeignKey(
                        "FK_AspNetUserLogins_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserRoles",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new {x.UserId, x.RoleId});
                    table.ForeignKey(
                        "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        x => x.RoleId,
                        "AspNetRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AspNetUserRoles_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserTokens",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new {x.UserId, x.LoginProvider, x.Name});
                    table.ForeignKey(
                        "FK_AspNetUserTokens_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "ApplicationUserUnitInfos",
                table => new
                {
                    ApplicationUserId = table.Column<string>(nullable: false),
                    UnitInfoId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserUnitInfos", x => new {x.ApplicationUserId, x.UnitInfoId});
                    table.ForeignKey(
                        "FK_ApplicationUserUnitInfos_AspNetUsers_ApplicationUserId",
                        x => x.ApplicationUserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_ApplicationUserUnitInfos_UnitInfos_UnitInfoId",
                        x => x.UnitInfoId,
                        "UnitInfos",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "UnitScans",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UnitInfoId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageJpeg = table.Column<byte[]>(nullable: true),
                    BatteryVolt = table.Column<decimal>(nullable: true),
                    Connection = table.Column<bool>(nullable: false),
                    RSSI = table.Column<decimal>(nullable: true),
                    NetworkConnection = table.Column<string>(nullable: true),
                    TemperatureSensor1Celsius = table.Column<decimal>(nullable: true),
                    TemperatureSensor2Celsius = table.Column<decimal>(nullable: true),
                    GpsLatitude = table.Column<decimal>(nullable: true),
                    GpsLongitude = table.Column<decimal>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitScans", x => x.Id);
                    table.ForeignKey(
                        "FK_UnitScans_AspNetUsers_OwnerId",
                        x => x.OwnerId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_UnitScans_UnitInfos_UnitInfoId",
                        x => x.UnitInfoId,
                        "UnitInfos",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "TagScans",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TagId = table.Column<string>(nullable: true),
                    Rssi = table.Column<decimal>(nullable: true),
                    UnitScanId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagScans", x => x.Id);
                    table.ForeignKey(
                        "FK_TagScans_UnitScans_UnitScanId",
                        x => x.UnitScanId,
                        "UnitScans",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_ApplicationUserUnitInfos_UnitInfoId",
                "ApplicationUserUnitInfos",
                "UnitInfoId");

            migrationBuilder.CreateIndex(
                "IX_AspNetRoleClaims_RoleId",
                "AspNetRoleClaims",
                "RoleId");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "AspNetRoles",
                "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_AspNetUserClaims_UserId",
                "AspNetUserClaims",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserLogins_UserId",
                "AspNetUserLogins",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserRoles_RoleId",
                "AspNetUserRoles",
                "RoleId");

            migrationBuilder.CreateIndex(
                "EmailIndex",
                "AspNetUsers",
                "NormalizedEmail");

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                "AspNetUsers",
                "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_DeviceCodes_DeviceCode",
                "DeviceCodes",
                "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_DeviceCodes_Expiration",
                "DeviceCodes",
                "Expiration");

            migrationBuilder.CreateIndex(
                "IX_PersistedGrants_Expiration",
                "PersistedGrants",
                "Expiration");

            migrationBuilder.CreateIndex(
                "IX_PersistedGrants_SubjectId_ClientId_Type",
                "PersistedGrants",
                new[] {"SubjectId", "ClientId", "Type"});

            migrationBuilder.CreateIndex(
                "IX_TagScans_UnitScanId",
                "TagScans",
                "UnitScanId");

            migrationBuilder.CreateIndex(
                "IX_UnitScans_OwnerId",
                "UnitScans",
                "OwnerId");

            migrationBuilder.CreateIndex(
                "IX_UnitScans_UnitInfoId",
                "UnitScans",
                "UnitInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "ApplicationUserUnitInfos");

            migrationBuilder.DropTable(
                "AspNetRoleClaims");

            migrationBuilder.DropTable(
                "AspNetUserClaims");

            migrationBuilder.DropTable(
                "AspNetUserLogins");

            migrationBuilder.DropTable(
                "AspNetUserRoles");

            migrationBuilder.DropTable(
                "AspNetUserTokens");

            migrationBuilder.DropTable(
                "DeviceCodes");

            migrationBuilder.DropTable(
                "PersistedGrants");

            migrationBuilder.DropTable(
                "TagScans");

            migrationBuilder.DropTable(
                "AspNetRoles");

            migrationBuilder.DropTable(
                "UnitScans");

            migrationBuilder.DropTable(
                "AspNetUsers");

            migrationBuilder.DropTable(
                "UnitInfos");
        }
    }
}