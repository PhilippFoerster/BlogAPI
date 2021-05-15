using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogAPI.Migrations
{
    public partial class _9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_CreatedById",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTopic_Articles_ArticlesId",
                table: "ArticleTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTopic_Topics_TopicsName",
                table: "ArticleTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicUser_AspNetUsers_InterestedUserId",
                table: "TopicUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicUser_Topics_InterestsName",
                table: "TopicUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_CreatedById",
                table: "Articles",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTopic_Articles_ArticlesId",
                table: "ArticleTopic",
                column: "ArticlesId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTopic_Topics_TopicsName",
                table: "ArticleTopic",
                column: "TopicsName",
                principalTable: "Topics",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicUser_AspNetUsers_InterestedUserId",
                table: "TopicUser",
                column: "InterestedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicUser_Topics_InterestsName",
                table: "TopicUser",
                column: "InterestsName",
                principalTable: "Topics",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_CreatedById",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTopic_Articles_ArticlesId",
                table: "ArticleTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTopic_Topics_TopicsName",
                table: "ArticleTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicUser_AspNetUsers_InterestedUserId",
                table: "TopicUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicUser_Topics_InterestsName",
                table: "TopicUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_CreatedById",
                table: "Articles",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTopic_Articles_ArticlesId",
                table: "ArticleTopic",
                column: "ArticlesId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTopic_Topics_TopicsName",
                table: "ArticleTopic",
                column: "TopicsName",
                principalTable: "Topics",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_AspNetUsers_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicUser_AspNetUsers_InterestedUserId",
                table: "TopicUser",
                column: "InterestedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicUser_Topics_InterestsName",
                table: "TopicUser",
                column: "InterestsName",
                principalTable: "Topics",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
