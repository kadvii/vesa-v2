using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vesa.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRestrictionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRestricted",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RestrictionReason",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRestricted",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RestrictionReason",
                table: "AspNetUsers");
        }
    }
}
