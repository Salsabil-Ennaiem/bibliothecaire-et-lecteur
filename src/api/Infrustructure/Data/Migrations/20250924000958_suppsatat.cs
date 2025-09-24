using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class suppsatat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statistiques");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_emp",
                table: "Emprunts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 24, 0, 9, 55, 954, DateTimeKind.Utc).AddTicks(2184),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 22, 50, 11, 901, DateTimeKind.Utc).AddTicks(932));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_emp",
                table: "Emprunts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 22, 50, 11, 901, DateTimeKind.Utc).AddTicks(932),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 24, 0, 9, 55, 954, DateTimeKind.Utc).AddTicks(2184));

            migrationBuilder.CreateTable(
                name: "Statistiques",
                columns: table => new
                {
                    id_stat = table.Column<string>(type: "text", nullable: false),
                    id_param = table.Column<string>(type: "text", nullable: true),
                    Emprunt_Par_Membre = table.Column<double>(type: "double precision", nullable: false),
                    Nombre_Sanction_Emises = table.Column<int>(type: "integer", nullable: false),
                    Période_en_jour = table.Column<int>(type: "integer", nullable: false),
                    Somme_Amende_Collectées = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Taux_Emprunt_En_Perte = table.Column<double>(type: "double precision", nullable: false),
                    Taux_Emprunt_En_Retard = table.Column<double>(type: "double precision", nullable: false),
                    date_stat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistiques", x => x.id_stat);
                    table.ForeignKey(
                        name: "FK_Statistiques_Parametres_id_param",
                        column: x => x.id_param,
                        principalTable: "Parametres",
                        principalColumn: "id_param");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Statistiques_id_param",
                table: "Statistiques",
                column: "id_param",
                unique: true);
        }
    }
}
