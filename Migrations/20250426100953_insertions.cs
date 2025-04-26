using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DCCR_SERVER.Migrations
{
    /// <inheritdoc />
    public partial class insertions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(File.ReadAllText("donnees.sql")); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
