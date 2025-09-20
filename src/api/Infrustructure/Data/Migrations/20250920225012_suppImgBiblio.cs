using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class suppImgBiblio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bibliothecaires_Fichier_Photo",
                table: "Bibliothecaires");

            migrationBuilder.DropIndex(
                name: "IX_Bibliothecaires_Photo",
                table: "Bibliothecaires");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Bibliothecaires");

            migrationBuilder.AddColumn<string>(
                name: "BibliothecaireId",
                table: "Fichier",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_emp",
                table: "Emprunts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 22, 50, 11, 901, DateTimeKind.Utc).AddTicks(932),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 9, 41, 29, 759, DateTimeKind.Utc).AddTicks(5631));

            migrationBuilder.CreateIndex(
                name: "IX_Fichier_BibliothecaireId",
                table: "Fichier",
                column: "BibliothecaireId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fichier_Bibliothecaires_BibliothecaireId",
                table: "Fichier",
                column: "BibliothecaireId",
                principalTable: "Bibliothecaires",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fichier_Bibliothecaires_BibliothecaireId",
                table: "Fichier");

            migrationBuilder.DropIndex(
                name: "IX_Fichier_BibliothecaireId",
                table: "Fichier");

            migrationBuilder.DropColumn(
                name: "BibliothecaireId",
                table: "Fichier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_emp",
                table: "Emprunts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 9, 41, 29, 759, DateTimeKind.Utc).AddTicks(5631),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 22, 50, 11, 901, DateTimeKind.Utc).AddTicks(932));

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "Bibliothecaires",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bibliothecaires_Photo",
                table: "Bibliothecaires",
                column: "Photo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bibliothecaires_Fichier_Photo",
                table: "Bibliothecaires",
                column: "Photo",
                principalTable: "Fichier",
                principalColumn: "IdFichier");
        }
    }
}
