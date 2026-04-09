using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniCinema.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBaoCaoThongKe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaoCaoThongKes",
                columns: table => new
                {
                    MaBaoCao = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenBaoCao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongDoanhThu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongSoVe = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoCaoThongKes", x => x.MaBaoCao);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaoCaoThongKes");
        }
    }
}
