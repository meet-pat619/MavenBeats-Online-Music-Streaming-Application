using Microsoft.EntityFrameworkCore.Migrations;

namespace DemoMusicMVC.Data.Migrations
{
    public partial class music_test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "songs",
                columns: table => new
                {
                    songId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    songName = table.Column<string>(nullable: true),
                    photoPath = table.Column<string>(nullable: true),
                    songPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_songs", x => x.songId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "songs");
        }
    }
}
