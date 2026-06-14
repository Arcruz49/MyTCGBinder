using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTCGBinder.Migrations
{
    /// <inheritdoc />
    public partial class tcgCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tcg_cards",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    set_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    set_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    series = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rarity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    image_small = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    image_large = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    synced_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tcg_cards", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_cards_tcg_card_id",
                table: "user_cards",
                column: "tcg_card_id");

            migrationBuilder.CreateIndex(
                name: "IX_tcg_cards_name",
                table: "tcg_cards",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_tcg_cards_set_id",
                table: "tcg_cards",
                column: "set_id");

            migrationBuilder.CreateIndex(
                name: "IX_tcg_cards_set_id_number",
                table: "tcg_cards",
                columns: new[] { "set_id", "number" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_cards_tcg_cards_tcg_card_id",
                table: "user_cards",
                column: "tcg_card_id",
                principalTable: "tcg_cards",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_cards_tcg_cards_tcg_card_id",
                table: "user_cards");

            migrationBuilder.DropTable(
                name: "tcg_cards");

            migrationBuilder.DropIndex(
                name: "IX_user_cards_tcg_card_id",
                table: "user_cards");
        }
    }
}
