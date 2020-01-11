using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Routine.APi.Migrations
{
    public partial class AddEmployeeData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[,]
                {
                    { new Guid("ca268a19-0f39-4d8b-b8d6-5bace54f8027"), new Guid("bbdee09c-089b-4d30-bece-44df5923716c"), new DateTime(1955, 10, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "M001", "William", 1, "Gates" },
                    { new Guid("265348d2-1276-4ada-ae33-4c1b8348edce"), new Guid("bbdee09c-089b-4d30-bece-44df5923716c"), new DateTime(1998, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "M024", "Kent", 1, "Back" },
                    { new Guid("47b70abc-98b8-4fdc-b9fa-5dd6716f6e6b"), new Guid("6fb600c1-9011-4fd7-9234-881379716440"), new DateTime(1986, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "G003", "Mary", 0, "King" },
                    { new Guid("059e2fcb-e5a4-4188-9b46-06184bcb111b"), new Guid("6fb600c1-9011-4fd7-9234-881379716440"), new DateTime(1977, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "G007", "Kevin", 1, "Richardson" },
                    { new Guid("a868ff18-3398-4598-b420-4878974a517a"), new Guid("5efc910b-2f45-43df-afee-620d40542853"), new DateTime(1964, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "A001", "Jack", 1, "Ma" },
                    { new Guid("2c3bb40c-5907-4eb7-bb2c-7d62edb430c9"), new Guid("5efc910b-2f45-43df-afee-620d40542853"), new DateTime(1997, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "A201", "Lorraine", 0, "Shaw" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("059e2fcb-e5a4-4188-9b46-06184bcb111b"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("265348d2-1276-4ada-ae33-4c1b8348edce"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("2c3bb40c-5907-4eb7-bb2c-7d62edb430c9"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("47b70abc-98b8-4fdc-b9fa-5dd6716f6e6b"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("a868ff18-3398-4598-b420-4878974a517a"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("ca268a19-0f39-4d8b-b8d6-5bace54f8027"));
        }
    }
}
