using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tidbeat.Migrations
{
    public partial class CorrectedFavorite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Song_FavoriteSongSongId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FavoriteSongSongId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FavoriteSongSongId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "FavoriteSongId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavoriteSongId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "FavoriteSongSongId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FavoriteSongSongId",
                table: "AspNetUsers",
                column: "FavoriteSongSongId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Song_FavoriteSongSongId",
                table: "AspNetUsers",
                column: "FavoriteSongSongId",
                principalTable: "Song",
                principalColumn: "SongId");
        }
    }
}
