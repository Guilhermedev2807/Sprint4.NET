using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCareAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(160)", maxLength: 160, nullable: false),
                    SenhaHash = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PET",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Especie = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Raca = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Peso = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PET", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PET_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CONSULTA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PetId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DataHora = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Veterinario = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Motivo = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONSULTA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CONSULTA_PET_PetId",
                        column: x => x.PetId,
                        principalTable: "PET",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VACINA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PetId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    DataAplicacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ProximaDose = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VACINA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VACINA_PET_PetId",
                        column: x => x.PetId,
                        principalTable: "PET",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_PetId",
                table: "CONSULTA",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_PET_UsuarioId",
                table: "PET",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_Email",
                table: "USUARIO",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VACINA_PetId",
                table: "VACINA",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONSULTA");

            migrationBuilder.DropTable(
                name: "VACINA");

            migrationBuilder.DropTable(
                name: "PET");

            migrationBuilder.DropTable(
                name: "USUARIO");
        }
    }
}
