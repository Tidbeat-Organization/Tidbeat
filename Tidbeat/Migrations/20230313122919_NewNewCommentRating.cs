using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tidbeat.Migrations
{
    public partial class NewNewCommentRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentRating",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentRating", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_CommentRating_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentRating_Comment_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comment",
                        principalColumn: "CommentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentRating_CommentId",
                table: "CommentRating",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentRating_UserId",
                table: "CommentRating",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentRating");
        }
    }
}
