using Microsoft.EntityFrameworkCore.Migrations;

namespace dbot.Migrations
{
    public partial class UserTablePrincipalKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyNominations_Users_UserID",
                table: "WeeklyNominations");

            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyVotes_Users_UserID",
                table: "WeeklyVotes");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyVotes_UserID",
                table: "WeeklyVotes");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyNominations_UserID",
                table: "WeeklyNominations");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "WeeklyVotes");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "WeeklyNominations");

            migrationBuilder.AddColumn<string>(
                name: "UserKey",
                table: "WeeklyVotes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserKey",
                table: "WeeklyNominations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_Key",
                table: "Users",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyVotes_UserKey",
                table: "WeeklyVotes",
                column: "UserKey");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyNominations_UserKey",
                table: "WeeklyNominations",
                column: "UserKey");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Key",
                table: "Users",
                column: "Key",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyNominations_Users_UserKey",
                table: "WeeklyNominations",
                column: "UserKey",
                principalTable: "Users",
                principalColumn: "Key",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyVotes_Users_UserKey",
                table: "WeeklyVotes",
                column: "UserKey",
                principalTable: "Users",
                principalColumn: "Key",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyNominations_Users_UserKey",
                table: "WeeklyNominations");

            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyVotes_Users_UserKey",
                table: "WeeklyVotes");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyVotes_UserKey",
                table: "WeeklyVotes");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyNominations_UserKey",
                table: "WeeklyNominations");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_Key",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Key",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserKey",
                table: "WeeklyVotes");

            migrationBuilder.DropColumn(
                name: "UserKey",
                table: "WeeklyNominations");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "WeeklyVotes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "WeeklyNominations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyVotes_UserID",
                table: "WeeklyVotes",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyNominations_UserID",
                table: "WeeklyNominations",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyNominations_Users_UserID",
                table: "WeeklyNominations",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyVotes_Users_UserID",
                table: "WeeklyVotes",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
