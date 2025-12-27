using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProcurementManager.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddGoodsReceiptTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoodsReceipts",
                columns: table => new
                {
                    GRNID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    POID = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceiptDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceivedByUserID = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceipts", x => x.GRNID);
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_PurchaseOrders_POID",
                        column: x => x.POID,
                        principalTable: "PurchaseOrders",
                        principalColumn: "POID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceipts_Users_ReceivedByUserID",
                        column: x => x.ReceivedByUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptItems",
                columns: table => new
                {
                    GRItemID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GRNID = table.Column<int>(type: "INTEGER", nullable: false),
                    POItemID = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "TEXT", nullable: false),
                    QualityStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptItems", x => x.GRItemID);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItems_GoodsReceipts_GRNID",
                        column: x => x.GRNID,
                        principalTable: "GoodsReceipts",
                        principalColumn: "GRNID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItems_PurchaseOrderItems_POItemID",
                        column: x => x.POItemID,
                        principalTable: "PurchaseOrderItems",
                        principalColumn: "POItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_GRNID",
                table: "GoodsReceiptItems",
                column: "GRNID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_POItemID",
                table: "GoodsReceiptItems",
                column: "POItemID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_POID",
                table: "GoodsReceipts",
                column: "POID");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_ReceivedByUserID",
                table: "GoodsReceipts",
                column: "ReceivedByUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsReceiptItems");

            migrationBuilder.DropTable(
                name: "GoodsReceipts");
        }
    }
}
