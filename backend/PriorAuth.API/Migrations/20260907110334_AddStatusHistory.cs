using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PriorAuth.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "diagnosis_codes",
                columns: table => new
                {
                    diagnosis_code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diagnosis_codes", x => x.diagnosis_code);
                });

            migrationBuilder.CreateTable(
                name: "health_plans",
                columns: table => new
                {
                    health_plan_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    ins_plan_code = table.Column<string>(type: "text", nullable: true),
                    line_of_business = table.Column<string>(type: "text", nullable: true),
                    plan_type = table.Column<string>(type: "text", nullable: true),
                    entity = table.Column<string>(type: "text", nullable: true),
                    plan_code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_health_plans", x => x.health_plan_id);
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    patient_id = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    gender = table.Column<string>(type: "text", nullable: true),
                    language_code = table.Column<string>(type: "text", nullable: true),
                    address_line1 = table.Column<string>(type: "text", nullable: true),
                    address_line2 = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<string>(type: "text", nullable: true),
                    zip_code = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    email_address = table.Column<string>(type: "text", nullable: true),
                    member_code = table.Column<string>(type: "text", nullable: true),
                    group_number = table.Column<string>(type: "text", nullable: true),
                    ipa_code = table.Column<string>(type: "text", nullable: true),
                    plan_code = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => x.patient_id);
                });

            migrationBuilder.CreateTable(
                name: "procedure_codes",
                columns: table => new
                {
                    procedure_code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_procedure_codes", x => x.procedure_code);
                });

            migrationBuilder.CreateTable(
                name: "providers",
                columns: table => new
                {
                    physician_id = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    npi = table.Column<string>(type: "text", nullable: true),
                    tin = table.Column<string>(type: "text", nullable: true),
                    specialty1 = table.Column<string>(type: "text", nullable: true),
                    specialty2 = table.Column<string>(type: "text", nullable: true),
                    address_line1 = table.Column<string>(type: "text", nullable: true),
                    address_line2 = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<string>(type: "text", nullable: true),
                    zip_code = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    fax = table.Column<string>(type: "text", nullable: true),
                    email_address = table.Column<string>(type: "text", nullable: true),
                    cell_phone = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_providers", x => x.physician_id);
                });

            migrationBuilder.CreateTable(
                name: "sites",
                columns: table => new
                {
                    site_id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    npi = table.Column<string>(type: "text", nullable: true),
                    tin = table.Column<string>(type: "text", nullable: true),
                    specialty1 = table.Column<string>(type: "text", nullable: true),
                    specialty2 = table.Column<string>(type: "text", nullable: true),
                    address_line1 = table.Column<string>(type: "text", nullable: true),
                    address_line2 = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<string>(type: "text", nullable: true),
                    zip_code = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    fax = table.Column<string>(type: "text", nullable: true),
                    participating = table.Column<bool>(type: "boolean", nullable: false),
                    steerage_flag = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sites", x => x.site_id);
                });

            migrationBuilder.CreateTable(
                name: "authorizations",
                columns: table => new
                {
                    authorization_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reference_number = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    program = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    patient_id = table.Column<string>(type: "text", nullable: true),
                    physician_id = table.Column<string>(type: "text", nullable: true),
                    health_plan_id = table.Column<int>(type: "integer", nullable: true),
                    site_id = table.Column<string>(type: "text", nullable: true),
                    primary_diagnosis = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authorizations", x => x.authorization_id);
                    table.ForeignKey(
                        name: "FK_authorizations_diagnosis_codes_primary_diagnosis",
                        column: x => x.primary_diagnosis,
                        principalTable: "diagnosis_codes",
                        principalColumn: "diagnosis_code",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_authorizations_health_plans_health_plan_id",
                        column: x => x.health_plan_id,
                        principalTable: "health_plans",
                        principalColumn: "health_plan_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_authorizations_members_patient_id",
                        column: x => x.patient_id,
                        principalTable: "members",
                        principalColumn: "patient_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_authorizations_providers_physician_id",
                        column: x => x.physician_id,
                        principalTable: "providers",
                        principalColumn: "physician_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_authorizations_sites_site_id",
                        column: x => x.site_id,
                        principalTable: "sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "authorization_diagnoses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    authorization_id = table.Column<int>(type: "integer", nullable: false),
                    diagnosis_code = table.Column<string>(type: "text", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authorization_diagnoses", x => x.id);
                    table.ForeignKey(
                        name: "FK_authorization_diagnoses_authorizations_authorization_id",
                        column: x => x.authorization_id,
                        principalTable: "authorizations",
                        principalColumn: "authorization_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_authorization_diagnoses_diagnosis_codes_diagnosis_code",
                        column: x => x.diagnosis_code,
                        principalTable: "diagnosis_codes",
                        principalColumn: "diagnosis_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "authorization_procedures",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    authorization_id = table.Column<int>(type: "integer", nullable: false),
                    procedure_code = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authorization_procedures", x => x.id);
                    table.ForeignKey(
                        name: "FK_authorization_procedures_authorizations_authorization_id",
                        column: x => x.authorization_id,
                        principalTable: "authorizations",
                        principalColumn: "authorization_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_authorization_procedures_procedure_codes_procedure_code",
                        column: x => x.procedure_code,
                        principalTable: "procedure_codes",
                        principalColumn: "procedure_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "status_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    authorization_id = table.Column<int>(type: "integer", nullable: false),
                    previous_status = table.Column<string>(type: "text", nullable: true),
                    new_status = table.Column<string>(type: "text", nullable: false),
                    changed_by = table.Column<string>(type: "text", nullable: true),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_status_history_authorizations_authorization_id",
                        column: x => x.authorization_id,
                        principalTable: "authorizations",
                        principalColumn: "authorization_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_authorization_diagnoses_authorization_id",
                table: "authorization_diagnoses",
                column: "authorization_id");

            migrationBuilder.CreateIndex(
                name: "IX_authorization_diagnoses_diagnosis_code",
                table: "authorization_diagnoses",
                column: "diagnosis_code");

            migrationBuilder.CreateIndex(
                name: "IX_authorization_procedures_authorization_id",
                table: "authorization_procedures",
                column: "authorization_id");

            migrationBuilder.CreateIndex(
                name: "IX_authorization_procedures_procedure_code",
                table: "authorization_procedures",
                column: "procedure_code");

            migrationBuilder.CreateIndex(
                name: "IX_authorizations_health_plan_id",
                table: "authorizations",
                column: "health_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_authorizations_patient_id",
                table: "authorizations",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_authorizations_physician_id",
                table: "authorizations",
                column: "physician_id");

            migrationBuilder.CreateIndex(
                name: "IX_authorizations_primary_diagnosis",
                table: "authorizations",
                column: "primary_diagnosis");

            migrationBuilder.CreateIndex(
                name: "IX_authorizations_site_id",
                table: "authorizations",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_status_history_authorization_id",
                table: "status_history",
                column: "authorization_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authorization_diagnoses");

            migrationBuilder.DropTable(
                name: "authorization_procedures");

            migrationBuilder.DropTable(
                name: "status_history");

            migrationBuilder.DropTable(
                name: "procedure_codes");

            migrationBuilder.DropTable(
                name: "authorizations");

            migrationBuilder.DropTable(
                name: "diagnosis_codes");

            migrationBuilder.DropTable(
                name: "health_plans");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "providers");

            migrationBuilder.DropTable(
                name: "sites");
        }
    }
}
