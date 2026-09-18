using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceOrders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustarRelacionamentoComprador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Compradores_CompradorId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_CompradorId",
                table: "Pedidos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_CompradorId",
                table: "Pedidos",
                column: "CompradorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Compradores_CompradorId",
                table: "Pedidos",
                column: "CompradorId",
                principalTable: "Compradores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
