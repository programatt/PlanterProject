using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanterApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastReceivedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AirTemperatureCelsius = table.Column<int>(type: "integer", nullable: false),
                    AirHumidityPercentage = table.Column<int>(type: "integer", nullable: false),
                    SoilTemperatureCelsius = table.Column<int>(type: "integer", nullable: false),
                    SoilMoisturePercentage = table.Column<int>(type: "integer", nullable: false),
                    WaterPumpOn = table.Column<bool>(type: "boolean", nullable: false),
                    LightSourceOn = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceMessages_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceMessages_DeviceId",
                table: "DeviceMessages",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceMessages");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
