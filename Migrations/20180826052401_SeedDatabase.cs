using Microsoft.EntityFrameworkCore.Migrations;

namespace ShiftScheduler.Migrations
{
    public partial class SeedDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Engineers (Name) VALUES ('David')");
            migrationBuilder.Sql("INSERT INTO Engineers (Name) VALUES ('John')");
            migrationBuilder.Sql("INSERT INTO Engineers (Name) VALUES ('Carol')");
            migrationBuilder.Sql("INSERT INTO Engineers (Name) VALUES ('Michael')");
            migrationBuilder.Sql("INSERT INTO Engineers (Name) VALUES ('Ann')");
            migrationBuilder.Sql("INSERT INTO Engineers (Name) VALUES ('Mark')");
            migrationBuilder.Sql("INSERT INTO Engineers (Name) VALUES ('Andrew')");
            migrationBuilder.Sql("INSERT INTO Engineers (Name) VALUES ('Sarah')");
            migrationBuilder.Sql("INSERT INTO Engineers (Name) VALUES ('Robert')");
            migrationBuilder.Sql("INSERT INTO Engineers (Name) VALUES ('Helen')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Engineers WHERE Name IN ('David', 'John', 'Carol', 'Michael', 'Ann', 'Mark', 'Andrew', 'Sarah', 'Robert', 'Helen')");
        }
    }
}
