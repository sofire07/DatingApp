using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class m2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    UserLikingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserBeingLikedId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => new { x.UserLikingId, x.UserBeingLikedId });
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_UserBeingLikedId",
                        column: x => x.UserBeingLikedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Likes_AspNetUsers_UserLikingId",
                        column: x => x.UserLikingId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserBeingLikedId",
                table: "Likes",
                column: "UserBeingLikedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Likes");
        }
    }
}
