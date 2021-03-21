using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogAPI.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TopicUser",
                columns: table => new
                {
                    InterestedUserId = table.Column<int>(type: "int", nullable: false),
                    InterestsName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicUser", x => new { x.InterestedUserId, x.InterestsName });
                    table.ForeignKey(
                        name: "FK_TopicUser_Topics_InterestsName",
                        column: x => x.InterestsName,
                        principalTable: "Topics",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicUser_Users_InterestedUserId",
                        column: x => x.InterestedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopicUser_InterestsName",
                table: "TopicUser",
                column: "InterestsName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopicUser");
        }
    }
}
