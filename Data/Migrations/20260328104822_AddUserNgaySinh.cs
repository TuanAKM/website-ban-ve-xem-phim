using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniCinema.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNgaySinh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgaySinh",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgaySinh",
                table: "AspNetUsers");
        }
    }
}
