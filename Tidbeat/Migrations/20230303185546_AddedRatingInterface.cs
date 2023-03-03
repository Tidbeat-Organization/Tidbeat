using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tidbeat.Migrations
{
    public partial class AddedRatingInterface : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostRatingId",
                table: "PostRating",
                newName: "RatingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RatingId",
                table: "PostRating",
                newName: "PostRatingId");
        }
    }
}
