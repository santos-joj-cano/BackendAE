using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAE.Migrations
{
    /// <inheritdoc />
    public partial class IntialMigrationT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CajaSesiones_Usuarios_UsuarioId",
                table: "CajaSesiones");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "CajaSesiones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CajaSesiones_Usuarios_UsuarioId",
                table: "CajaSesiones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CajaSesiones_Usuarios_UsuarioId",
                table: "CajaSesiones");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "CajaSesiones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CajaSesiones_Usuarios_UsuarioId",
                table: "CajaSesiones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId");
        }
    }
}
