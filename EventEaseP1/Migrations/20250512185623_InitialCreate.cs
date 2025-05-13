using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEaseP1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    // Change this line to add the Identity specification
                    VenueID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // Add this line
                    Name = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Location = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Venues__3C57E5D2C536C794", x => x.VenueID);
                });

            migrationBuilder.CreateTable(
                name: "Eventss",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EventDate = table.Column<DateOnly>(type: "date", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Eventss__7944C87065698F4E", x => x.EventID);
                    table.ForeignKey(
                        name: "FK__Eventss__VenueID__398D8EEE",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueID");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bookings__73951ACD5AD8848B", x => x.BookingID);
                    table.ForeignKey(
                        name: "FK__Bookings__EventI__3C69FB99",
                        column: x => x.EventId,
                        principalTable: "Eventss",
                        principalColumn: "EventID");
                    table.ForeignKey(
                        name: "FK__Bookings__VenueI__47DBAE45",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EventId",
                table: "Bookings",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VenueId",
                table: "Bookings",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventss_VenueId",
                table: "Eventss",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Eventss");

            migrationBuilder.DropTable(
                name: "Venues");
        }
    }
}
