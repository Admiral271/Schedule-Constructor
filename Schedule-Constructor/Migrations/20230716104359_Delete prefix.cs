using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedule_Constructor.Migrations
{
    /// <inheritdoc />
    public partial class Deleteprefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupSubjects");

            migrationBuilder.RenameColumn(
                name: "Name_Subject",
                table: "Subjects",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Id_Subject",
                table: "Subjects",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Name_Group",
                table: "Groups",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Id_Group",
                table: "Groups",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Subjects",
                newName: "Name_Subject");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Subjects",
                newName: "Id_Subject");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Groups",
                newName: "Name_Group");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Groups",
                newName: "Id_Group");

            migrationBuilder.CreateTable(
                name: "GroupSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupSubjects_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id_Group",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id_Subject",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupSubjects_GroupId",
                table: "GroupSubjects",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSubjects_SubjectId",
                table: "GroupSubjects",
                column: "SubjectId");
        }
    }
}
