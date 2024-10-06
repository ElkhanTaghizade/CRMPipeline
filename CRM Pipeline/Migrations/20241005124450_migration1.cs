using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRMPipeline.Migrations
{
    /// <inheritdoc />
    public partial class migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lead_Stage_History",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeadId = table.Column<Guid>(name: "Lead_Id", type: "uniqueidentifier", nullable: false),
                    Probability = table.Column<double>(type: "float", nullable: false),
                    ExpectedClosingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(name: "User_Id", type: "int", nullable: false),
                    ExtraInformation = table.Column<string>(name: "Extra_Information", type: "nvarchar(max)", nullable: true),
                    InternalNotes = table.Column<string>(name: "Internal_Notes", type: "nvarchar(max)", nullable: true),
                    ChangedAt = table.Column<DateTime>(name: "Changed_At", type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lead_Stage_History", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalRevenue = table.Column<double>(name: "Total_Revenue", type: "float", nullable: false),
                    IsActive = table.Column<bool>(name: "Is_Active", type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Email",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsResponse = table.Column<bool>(type: "bit", nullable: false),
                    SentByUserId = table.Column<int>(type: "int", nullable: false),
                    SentToCustomerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Email_Customers_SentToCustomerId",
                        column: x => x.SentToCustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Email_Users_SentByUserId",
                        column: x => x.SentByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExpectedRevenue = table.Column<double>(type: "float", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StageId = table.Column<Guid>(name: "Stage_Id", type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(name: "Product_Id", type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leads_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CreatedByUserId",
                table: "Customers",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Email_SentByUserId",
                table: "Email",
                column: "SentByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Email_SentToCustomerId",
                table: "Email",
                column: "SentToCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_CustomerId",
                table: "Leads",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Email");

            migrationBuilder.DropTable(
                name: "Lead_Stage_History");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Stages");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
