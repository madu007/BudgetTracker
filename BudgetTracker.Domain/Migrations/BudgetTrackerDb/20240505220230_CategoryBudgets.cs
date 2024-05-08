using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTracker.Domain.Migrations.BudgetTrackerDb
{
    /// <inheritdoc />
    public partial class CategoryBudgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryLimits_Categories_CategoryId1",
                table: "CategoryLimits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryLimits",
                table: "CategoryLimits");

            migrationBuilder.DropIndex(
                name: "IX_CategoryLimits_CategoryId1",
                table: "CategoryLimits");

            migrationBuilder.RenameColumn(
                name: "CategoryId1",
                table: "CategoryLimits",
                newName: "CategorysId");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CategoryLimits",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "CategorysId",
                table: "CategoryLimits",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryLimits",
                table: "CategoryLimits",
                column: "CategorysId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryLimits_CategoryId",
                table: "CategoryLimits",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryLimits_Categories_CategoryId",
                table: "CategoryLimits",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryLimits_Categories_CategoryId",
                table: "CategoryLimits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryLimits",
                table: "CategoryLimits");

            migrationBuilder.DropIndex(
                name: "IX_CategoryLimits_CategoryId",
                table: "CategoryLimits");

            migrationBuilder.RenameColumn(
                name: "CategorysId",
                table: "CategoryLimits",
                newName: "CategoryId1");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CategoryLimits",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId1",
                table: "CategoryLimits",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryLimits",
                table: "CategoryLimits",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryLimits_CategoryId1",
                table: "CategoryLimits",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryLimits_Categories_CategoryId1",
                table: "CategoryLimits",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
