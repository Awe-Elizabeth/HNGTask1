using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HNGTask1.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sample_size",
                table: "Profiles",
                newName: "sample_size");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Profiles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Gender_probability",
                table: "Profiles",
                newName: "gender_probability");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Profiles",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "Created_at",
                table: "Profiles",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Country_probability",
                table: "Profiles",
                newName: "country_probability");

            migrationBuilder.RenameColumn(
                name: "Country_name",
                table: "Profiles",
                newName: "country_name");

            migrationBuilder.RenameColumn(
                name: "Country_id",
                table: "Profiles",
                newName: "country_id");

            migrationBuilder.RenameColumn(
                name: "Age_group",
                table: "Profiles",
                newName: "age_group");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "Profiles",
                newName: "age");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Profiles",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Profiles_Name",
                table: "Profiles",
                newName: "IX_Profiles_name");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    github_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    avatar_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_github_id",
                table: "Users",
                column: "github_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.RenameColumn(
                name: "sample_size",
                table: "Profiles",
                newName: "Sample_size");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Profiles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "gender_probability",
                table: "Profiles",
                newName: "Gender_probability");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "Profiles",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Profiles",
                newName: "Created_at");

            migrationBuilder.RenameColumn(
                name: "country_probability",
                table: "Profiles",
                newName: "Country_probability");

            migrationBuilder.RenameColumn(
                name: "country_name",
                table: "Profiles",
                newName: "Country_name");

            migrationBuilder.RenameColumn(
                name: "country_id",
                table: "Profiles",
                newName: "Country_id");

            migrationBuilder.RenameColumn(
                name: "age_group",
                table: "Profiles",
                newName: "Age_group");

            migrationBuilder.RenameColumn(
                name: "age",
                table: "Profiles",
                newName: "Age");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Profiles",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Profiles_name",
                table: "Profiles",
                newName: "IX_Profiles_Name");
        }
    }
}
