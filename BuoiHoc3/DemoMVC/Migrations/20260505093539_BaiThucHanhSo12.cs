using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMVC.Migrations
{
    /// <inheritdoc />
    public partial class BaiThucHanhSo12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentCode",
                table: "Students",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DeviceCategories",
                columns: table => new
                {
                    DeviceCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceCategories", x => x.DeviceCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "ExportReceipts",
                columns: table => new
                {
                    ExportReceiptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportReceipts", x => x.ExportReceiptId);
                });

            migrationBuilder.CreateTable(
                name: "ImportReceipts",
                columns: table => new
                {
                    ImportReceiptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportReceipts", x => x.ImportReceiptId);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    DeviceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeviceCategoryId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.DeviceId);
                    table.ForeignKey(
                        name: "FK_Devices_DeviceCategories_DeviceCategoryId",
                        column: x => x.DeviceCategoryId,
                        principalTable: "DeviceCategories",
                        principalColumn: "DeviceCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devices_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExportReceiptDetails",
                columns: table => new
                {
                    ExportReceiptDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExportReceiptId = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ExportPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportReceiptDetails", x => x.ExportReceiptDetailId);
                    table.ForeignKey(
                        name: "FK_ExportReceiptDetails_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExportReceiptDetails_ExportReceipts_ExportReceiptId",
                        column: x => x.ExportReceiptId,
                        principalTable: "ExportReceipts",
                        principalColumn: "ExportReceiptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportReceiptDetails",
                columns: table => new
                {
                    ImportReceiptDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportReceiptId = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ImportPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportReceiptDetails", x => x.ImportReceiptDetailId);
                    table.ForeignKey(
                        name: "FK_ImportReceiptDetails_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImportReceiptDetails_ImportReceipts_ImportReceiptId",
                        column: x => x.ImportReceiptId,
                        principalTable: "ImportReceipts",
                        principalColumn: "ImportReceiptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceCategoryId",
                table: "Devices",
                column: "DeviceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SupplierId",
                table: "Devices",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportReceiptDetails_DeviceId",
                table: "ExportReceiptDetails",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportReceiptDetails_ExportReceiptId",
                table: "ExportReceiptDetails",
                column: "ExportReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportReceiptDetails_DeviceId",
                table: "ImportReceiptDetails",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportReceiptDetails_ImportReceiptId",
                table: "ImportReceiptDetails",
                column: "ImportReceiptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportReceiptDetails");

            migrationBuilder.DropTable(
                name: "ImportReceiptDetails");

            migrationBuilder.DropTable(
                name: "ExportReceipts");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "ImportReceipts");

            migrationBuilder.DropTable(
                name: "DeviceCategories");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropColumn(
                name: "StudentCode",
                table: "Students");
        }
    }
}
