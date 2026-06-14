using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTCGBinder.Migrations
{
    /// <inheritdoc />
    public partial class corectuserCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_url",
                table: "user_cards");

            migrationBuilder.DropColumn(
                name: "image_url_large",
                table: "user_cards");

            migrationBuilder.DropColumn(
                name: "name",
                table: "user_cards");

            migrationBuilder.DropColumn(
                name: "number",
                table: "user_cards");

            migrationBuilder.DropColumn(
                name: "rarity",
                table: "user_cards");

            migrationBuilder.DropColumn(
                name: "set_id",
                table: "user_cards");

            migrationBuilder.DropColumn(
                name: "set_name",
                table: "user_cards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "user_cards",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "image_url_large",
                table: "user_cards",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "user_cards",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "number",
                table: "user_cards",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "rarity",
                table: "user_cards",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "set_id",
                table: "user_cards",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "set_name",
                table: "user_cards",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
