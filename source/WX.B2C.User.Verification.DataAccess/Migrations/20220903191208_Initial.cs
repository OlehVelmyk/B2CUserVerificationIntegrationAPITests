using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WX.B2C.User.Verification.DataAccess.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Policy");

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    PolicyId = table.Column<Guid>(nullable: false),
                    ProductType = table.Column<string>(maxLength: 50, nullable: false),
                    State = table.Column<string>(maxLength: 10, nullable: false),
                    PreviousState = table.Column<string>(maxLength: 10, nullable: true),
                    DecisionReasons = table.Column<string>(nullable: true),
                    IsAutomating = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    EntryKey = table.Column<Guid>(nullable: false),
                    EntryType = table.Column<string>(maxLength: 40, nullable: false),
                    EventType = table.Column<string>(maxLength: 40, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: false),
                    Initiator = table.Column<string>(maxLength: 385, nullable: false),
                    Reason = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BridgerCredentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<string>(maxLength: 20, nullable: false),
                    EncryptedPassword = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BridgerCredentials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Checks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    VariantId = table.Column<Guid>(nullable: false),
                    Provider = table.Column<string>(maxLength: 20, nullable: false),
                    State = table.Column<string>(maxLength: 10, nullable: false),
                    ExternalId = table.Column<string>(maxLength: 50, nullable: true),
                    ExternalData = table.Column<string>(nullable: true),
                    InputData = table.Column<string>(nullable: true),
                    OutputData = table.Column<string>(nullable: true),
                    Result = table.Column<string>(maxLength: 10, nullable: true),
                    Decision = table.Column<string>(maxLength: 30, nullable: true),
                    Errors = table.Column<string>(nullable: true),
                    StartedAt = table.Column<DateTime>(nullable: true),
                    PerformedAt = table.Column<DateTime>(nullable: true),
                    CompletedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    XPath = table.Column<string>(maxLength: 100, nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    IsReviewNeeded = table.Column<bool>(nullable: false),
                    State = table.Column<string>(maxLength: 10, nullable: false),
                    ReviewResult = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionSteps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    Category = table.Column<string>(maxLength: 20, nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Provider = table.Column<string>(maxLength: 20, nullable: false),
                    ExternalId = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalProfiles", x => new { x.UserId, x.Provider });
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    FileName = table.Column<string>(maxLength: 250, nullable: false),
                    Crc32Checksum = table.Column<long>(nullable: true),
                    Status = table.Column<string>(maxLength: 10, nullable: false),
                    ExternalId = table.Column<string>(maxLength: 50, nullable: true),
                    Provider = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonalDetails",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 150, nullable: true),
                    LastName = table.Column<string>(maxLength: 150, nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    Nationality = table.Column<string>(maxLength: 2, nullable: true),
                    Email = table.Column<string>(maxLength: 385, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalDetails", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Triggers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    VariantId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ApplicationId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: false),
                    ScheduleDate = table.Column<DateTime>(nullable: false),
                    UnscheduleDate = table.Column<DateTime>(nullable: true),
                    FiringDate = table.Column<DateTime>(nullable: true),
                    Context = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triggers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerificationDetails",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    IpAddress = table.Column<string>(maxLength: 40, nullable: true),
                    TaxResidence = table.Column<string>(nullable: true),
                    RiskLevel = table.Column<string>(maxLength: 10, nullable: true),
                    IdDocumentNumber = table.Column<string>(maxLength: 50, nullable: true),
                    IdDocumentNumberType = table.Column<string>(maxLength: 50, nullable: true),
                    Tin = table.Column<string>(maxLength: 150, nullable: true),
                    Nationality = table.Column<string>(maxLength: 2, nullable: true),
                    IsPep = table.Column<bool>(nullable: true),
                    IsSanctioned = table.Column<bool>(nullable: true),
                    IsAdverseMedia = table.Column<bool>(nullable: true),
                    Turnover = table.Column<decimal>(nullable: true),
                    PoiIssuingCountry = table.Column<string>(maxLength: 2, nullable: true),
                    PlaceOfBirth = table.Column<string>(maxLength: 2, nullable: true),
                    ComprehensiveIndex = table.Column<int>(nullable: true),
                    IsIpMatched = table.Column<bool>(nullable: true),
                    ResolvedCountryCode = table.Column<string>(maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationDetails", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "VerificationTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    VariantId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(maxLength: 30, nullable: false),
                    State = table.Column<string>(maxLength: 10, nullable: false),
                    Result = table.Column<string>(maxLength: 10, nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    IsExpired = table.Column<bool>(nullable: false),
                    ExpiredAt = table.Column<DateTime>(nullable: true),
                    ExpirationReason = table.Column<string>(maxLength: 20, nullable: true),
                    AcceptanceCheckIds = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecksVariants",
                schema: "Policy",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    Provider = table.Column<string>(maxLength: 20, nullable: false),
                    Config = table.Column<string>(nullable: true),
                    FailResultType = table.Column<string>(maxLength: 30, nullable: true),
                    FailResult = table.Column<string>(nullable: true),
                    FailResultCondition = table.Column<string>(nullable: true),
                    RunPolicy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecksVariants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monitoring",
                schema: "Policy",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    RegionType = table.Column<int>(maxLength: 10, nullable: false),
                    Region = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monitoring", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "Policy",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Type = table.Column<string>(maxLength: 30, nullable: false),
                    CollectionSteps = table.Column<string>(nullable: true),
                    AutoCompletePolicy = table.Column<string>(maxLength: 10, nullable: false, defaultValue: "None")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Triggers",
                schema: "Policy",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PolicyId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Iterative = table.Column<bool>(nullable: false),
                    Schedule = table.Column<string>(nullable: true),
                    Preconditions = table.Column<string>(nullable: true),
                    Conditions = table.Column<string>(nullable: true),
                    Commands = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triggers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValidationPolicies",
                schema: "Policy",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RegionType = table.Column<int>(maxLength: 10, nullable: false),
                    Region = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValidationRules",
                schema: "Policy",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RuleType = table.Column<string>(maxLength: 50, nullable: false),
                    RuleSubject = table.Column<string>(maxLength: 50, nullable: true),
                    Validation = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Verifications",
                schema: "Policy",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    RegionType = table.Column<int>(maxLength: 10, nullable: false),
                    Region = table.Column<string>(maxLength: 50, nullable: false),
                    RejectionPolicy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationStateChangelog",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(nullable: false),
                    FirstApprovedDate = table.Column<DateTime>(nullable: true),
                    LastApprovedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStateChangelog", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_ApplicationStateChangelog_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    FileId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentFiles_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResidenceAddresses",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Line1 = table.Column<string>(maxLength: 250, nullable: true),
                    Line2 = table.Column<string>(maxLength: 250, nullable: true),
                    City = table.Column<string>(maxLength: 100, nullable: true),
                    State = table.Column<string>(maxLength: 100, nullable: true),
                    Country = table.Column<string>(maxLength: 2, nullable: false),
                    ZipCode = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidenceAddresses", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_ResidenceAddresses_PersonalDetails_UserId",
                        column: x => x.UserId,
                        principalTable: "PersonalDetails",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationTasks",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationTasks", x => new { x.ApplicationId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_ApplicationTasks_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationTasks_VerificationTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "VerificationTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskChecks",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(nullable: false),
                    CheckId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskChecks", x => new { x.TaskId, x.CheckId });
                    table.ForeignKey(
                        name: "FK_TaskChecks_Checks_CheckId",
                        column: x => x.CheckId,
                        principalTable: "Checks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskChecks_VerificationTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "VerificationTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskCollectionSteps",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(nullable: false),
                    StepId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCollectionSteps", x => new { x.TaskId, x.StepId });
                    table.ForeignKey(
                        name: "FK_TaskCollectionSteps_CollectionSteps_StepId",
                        column: x => x.StepId,
                        principalTable: "CollectionSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskCollectionSteps_VerificationTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "VerificationTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskCheckVariants",
                schema: "Policy",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(nullable: false),
                    CheckVariantId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCheckVariants", x => new { x.TaskId, x.CheckVariantId });
                    table.ForeignKey(
                        name: "FK_TaskCheckVariants_ChecksVariants_CheckVariantId",
                        column: x => x.CheckVariantId,
                        principalSchema: "Policy",
                        principalTable: "ChecksVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskCheckVariants_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "Policy",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyValidationRules",
                schema: "Policy",
                columns: table => new
                {
                    ValidationRuleId = table.Column<Guid>(nullable: false),
                    ValidationPolicyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyValidationRules", x => new { x.ValidationPolicyId, x.ValidationRuleId });
                    table.ForeignKey(
                        name: "FK_PolicyValidationRules_ValidationPolicies_ValidationPolicyId",
                        column: x => x.ValidationPolicyId,
                        principalSchema: "Policy",
                        principalTable: "ValidationPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PolicyValidationRules_ValidationRules_ValidationRuleId",
                        column: x => x.ValidationRuleId,
                        principalSchema: "Policy",
                        principalTable: "ValidationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyTasks",
                schema: "Policy",
                columns: table => new
                {
                    PolicyId = table.Column<Guid>(nullable: false),
                    TaskVariantId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyTasks", x => new { x.PolicyId, x.TaskVariantId });
                    table.ForeignKey(
                        name: "FK_PolicyTasks_Verifications_PolicyId",
                        column: x => x.PolicyId,
                        principalSchema: "Policy",
                        principalTable: "Verifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PolicyTasks_Tasks_TaskVariantId",
                        column: x => x.TaskVariantId,
                        principalSchema: "Policy",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "ChecksVariants",
                columns: new[] { "Id", "Config", "FailResult", "FailResultCondition", "FailResultType", "Name", "Provider", "RunPolicy", "Type" },
                values: new object[,]
                {
                    { new Guid("29aac87b-3ad4-40e0-b34f-3685ca64805d"), "{\"reportName\":\"facial_similarity_photo\",\"isVideoRequired\":false}", null, null, null, "Onfido facial confirmation by photo", "Onfido", null, "FacialSimilarity" },
                    { new Guid("ca5d8609-897e-48e1-b9e3-82ba4f40e6ff"), "{\"matchType\":\"ByState\"}", "{\"xPath\":\"Documents.ProofOfAddress\",\"isReviewNeeded\":true,\"isRequired\":true}", null, "AddCollectionStep", "Check state by IP match", "System", null, "IpMatch" },
                    { new Guid("6431dd13-ef54-4d99-978e-e94067913b43"), "{\"matchType\":\"ByCountry\"}", "{\"xPath\":\"Documents.ProofOfAddress\",\"isReviewNeeded\":true,\"isRequired\":true}", null, "AddCollectionStep", "Check country by IP match with assigning PoA on fail", "System", null, "IpMatch" },
                    { new Guid("fe22ebbc-da71-43a9-883c-b15f0124f0e3"), "{\"matchType\":\"ByCountry\",\"extractOnly\":true}", null, null, null, "Check country by IP match and just store result", "System", null, "IpMatch" },
                    { new Guid("779e3ed5-68e1-4c95-8e15-e6d8957820bc"), "{\"matchType\":\"ByRegion\"}", "{\"xPath\":\"Documents.ProofOfAddress\",\"isReviewNeeded\":true,\"isRequired\":true}", null, "AddCollectionStep", "Check region by IP match", "System", null, "IpMatch" },
                    { new Guid("0822efcf-5f04-4d10-a144-460e2040968e"), "{\"reportName\":\"known_faces\",\"isVideoRequired\":false}", "{\"xPath\":\"Documents.Selfie.Photo\",\"isReviewNeeded\":false,\"isRequired\":true}", "{\"type\":\"MatchDecision\",\"value\":\"Resubmit\"}", "ResubmitCollectionStep", "Onfido known face check by photo", "Onfido", "{\"MaxAttempts\":5}", "FaceDuplication" },
                    { new Guid("0e3b0522-8330-487f-a35c-38cc6022812a"), "{\"reportName\":\"known_faces\",\"isVideoRequired\":true}", "{\"xPath\":\"Documents.Selfie.Video\",\"isReviewNeeded\":false,\"isRequired\":true}", "{\"type\":\"MatchDecision\",\"value\":\"Resubmit\"}", "ResubmitCollectionStep", "Onfido known face check by video", "Onfido", "{\"MaxAttempts\":5}", "FaceDuplication" },
                    { new Guid("a9e0048b-0f6b-44f0-8a22-703dd86ba05e"), "{\"countries\":[\"US\"]}", "{\"xPath\":\"Documents.Taxation.W9Form\",\"isReviewNeeded\":true,\"isRequired\":true}", "{\"type\":\"MatchCountry\",\"value\":\"US\"}", "AddCollectionStep", "Tax residence check", "System", null, "TaxResidence" },
                    { new Guid("372929f1-8597-4e50-a5ce-5881377af295"), "{\"matchByFirstLastName\":true,\"matchByDob\":true}", null, null, null, "Name and date of birth duplication", "System", null, "NameAndDoBDuplication" },
                    { new Guid("6d930727-5bcc-4014-a9c1-b08b85491d34"), "{\"reportName\":\"facial_similarity_video\",\"isVideoRequired\":true}", "{\"xPath\":\"Documents.Selfie.Video\",\"isReviewNeeded\":false,\"isRequired\":true}", "{\"type\":\"MatchDecision\",\"value\":\"Resubmit\"}", "ResubmitCollectionStep", "Onfido facial similarity check by video", "Onfido", "{\"MaxAttempts\":5}", "FacialSimilarity" },
                    { new Guid("7147ade3-a665-4b03-9b89-99008218c12f"), null, null, null, null, "LN RDP fraud screening check", "LexisNexis", null, "FraudScreening" },
                    { new Guid("63eaa27a-a6fc-43a8-93e4-0d7bfbd7cf23"), "{\"reportName\":\"identity_enhanced\"}", "{\"xPath\":\"Documents.ProofOfAddress\",\"isReviewNeeded\":true,\"isRequired\":true}", null, "AddCollectionStep", "Onfido identity check", "Onfido", "{\"MaxAttempts\":5}", "IdentityEnhanced" },
                    { new Guid("d6f2daf9-3f8f-4335-a7b8-4fc383471a1d"), "{\"reportName\":\"document\"}", "{\"xPath\":\"Documents.ProofOfIdentity\",\"isReviewNeeded\":false,\"isRequired\":true}", "{\"type\":\"MatchDecision\",\"value\":\"Resubmit\"}", "ResubmitCollectionStep", "Onfido document check", "Onfido", "{\"MaxAttempts\":5}", "IdentityDocument" },
                    { new Guid("bb30dacb-f8a0-477c-941a-fb0c71c0297a"), null, null, null, null, "Pep, adverse media, sanction check", "PassFort", null, "RiskListsScreening" },
                    { new Guid("759de7bc-0d76-4c53-8fe3-702c2f6dd2ce"), null, "{\"xPath\":\"Survey.CA6B7FB1-413D-449B-9038-32AB5B4914B6\",\"isReviewNeeded\":true,\"isRequired\":true}", "{\"type\":\"IsPep\",\"value\":true}", "AddCollectionStep", "UK Pep, adverse media, sanction check", "PassFort", null, "RiskListsScreening" },
                    { new Guid("34a33df0-b9b5-4205-9cc6-1f90be10d313"), null, null, null, null, "US PEP, adverse media, sanction check", "LexisNexis", null, "RiskListsScreening" },
                    { new Guid("23714f13-cbf6-41a4-85c6-719991e6c3f3"), "{\"reportName\":\"facial_similarity_video\",\"isVideoRequired\":true}", null, null, null, "Onfido facial confirmation by video", "Onfido", null, "FacialSimilarity" },
                    { new Guid("3b43f013-5ac2-4bd9-96de-20a683331bcf"), "{\"reportName\":\"facial_similarity_photo\",\"isVideoRequired\":false}", "{\"xPath\":\"Documents.Selfie.Photo\",\"isReviewNeeded\":false,\"isRequired\":true}", "{\"type\":\"MatchDecision\",\"value\":\"Resubmit\"}", "ResubmitCollectionStep", "Onfido facial similarity check by photo", "Onfido", "{\"MaxAttempts\":5}", "FacialSimilarity" },
                    { new Guid("62ba1cc3-8802-4c13-b2ba-bb6efbdbee1f"), null, null, null, null, "Id document number duplication", "System", null, "IdDocNumberDuplication" }
                });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "Monitoring",
                columns: new[] { "Id", "Name", "Region", "RegionType" },
                values: new object[,]
                {
                    { new Guid("c8e4aff9-c3ac-429f-907e-61fd2519cadd"), "APAC monitoring policy", "APAC", 2 },
                    { new Guid("cdee5676-5323-4a1a-8b37-fc24ec2deb73"), "Philippines monitoring policy", "PH", 3 },
                    { new Guid("de887152-ca43-499d-8efd-6bc955b78b80"), "GB monitoring policy", "GB", 3 },
                    { new Guid("7ba96008-c7b3-469b-9f3c-307224abbe83"), "EEA monitoring policy", "EEA", 2 },
                    { new Guid("caed84f7-fc93-431c-8b34-56612ce52dfd"), "ROW monitoring policy", "RoW", 2 }
                });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "Tasks",
                columns: new[] { "Id", "AutoCompletePolicy", "CollectionSteps", "Name", "Type" },
                values: new object[,]
                {
                    { new Guid("91ddffdb-4788-4783-a004-025b2357a9ed"), "EveryTime", null, "Identity task", "Identity" },
                    { new Guid("782bdf03-5c44-45ad-a267-8a26934066a8"), "EveryTime", null, "Tax residence", "TaxResidence" },
                    { new Guid("d4d1e3d0-8fb8-46a8-a5a8-bfae7cf9e471"), "EveryTime", null, "User duplication screening", "DuplicationScreening" },
                    { new Guid("7fc98096-1cb1-47fb-91bd-1f600beb82a0"), "EveryTime", null, "User address", "Address" },
                    { new Guid("1dd5ae17-d87c-4b9f-b61f-a084b29abb4a"), "EveryTime", "[{\"XPath\":\"VerificationDetails.IsPep\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsAdverseMedia\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsSanctioned\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "Risk lists screening", "RiskListsScreening" },
                    { new Guid("325704db-6db2-4dab-9c07-ef700e341dbe"), "EveryTime", null, "Identity task", "Identity" },
                    { new Guid("02f9e694-49f2-4669-a5c8-bea234d92e03"), "EveryTime", null, "Tax residence", "TaxResidence" },
                    { new Guid("cbeed7f8-943b-482f-81a5-4648f4f1fa04"), "EveryTime", null, "User duplication screening", "DuplicationScreening" },
                    { new Guid("2796b0cb-42cb-49b7-a30e-84824d603799"), "EveryTime", null, "User address", "Address" },
                    { new Guid("caefe21a-e362-4ff5-93b5-943158102c31"), "EveryTime", null, "Identity task", "Identity" },
                    { new Guid("fbca7efc-39f1-4d26-8e5c-4feb4a7fd34f"), "EveryTime", "[{\"XPath\":\"VerificationDetails.TaxResidence\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "Tax residence", "TaxResidence" },
                    { new Guid("9486f8ef-1730-45a7-a724-434d4e89c7c1"), "EveryTime", null, "User duplication screening", "DuplicationScreening" },
                    { new Guid("828c3d69-d9b1-46d5-9498-45f3dd74b278"), "EveryTime", null, "User address", "Address" },
                    { new Guid("886609cb-ed63-4aef-aee9-b51b75c2a829"), "EveryTime", null, "Additional identity proofing task", "FraudScreening" },
                    { new Guid("c2badff2-b73f-4fb0-b447-a3eb48964b36"), "EveryTime", "[{\"XPath\":\"VerificationDetails.IsPep\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsAdverseMedia\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsSanctioned\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "Risk lists screening", "RiskListsScreening" },
                    { new Guid("72df5230-bb5a-4857-bcaf-9e27f0cd8a36"), "EveryTime", "[{\"XPath\":\"Survey.DE532CA0-C21E-4F7B-AD09-647EAA0C4E00\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "CDD questionnaire", "FinancialCondition" },
                    { new Guid("c2d32093-ad0f-45ad-9377-4dd12550a221"), "EveryTime", null, "EDD questionnaire", "UserRiskScreening" },
                    { new Guid("1673241e-bc0a-4007-a353-a2c39880bbef"), "EveryTime", null, "Proof of funds/wealth assessment", "ProofOfFunds" },
                    { new Guid("aa49652a-74c8-4667-a20f-a92fe59cbf2b"), "EveryTime", "[{\"XPath\":\"VerificationDetails.IsPep\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsAdverseMedia\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsSanctioned\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "Risk lists screening", "RiskListsScreening" },
                    { new Guid("cf3d8afc-cbb4-4531-bea5-ddf2eb990cfe"), "EveryTime", "[{\"XPath\":\"VerificationDetails.IsPep\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsAdverseMedia\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsSanctioned\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "Risk lists screening", "RiskListsScreening" },
                    { new Guid("bc3341a5-e1b3-4f72-93a3-1a5324a92ff9"), "EveryTime", "[{\"XPath\":\"Documents.ProofOfAddress\",\"IsRequired\":true,\"IsReviewNeeded\":true}]", "User address verification", "Address" },
                    { new Guid("543ec1a7-6141-4491-9ca6-3691bbddb7ee"), "EveryTime", null, "User address", "Address" },
                    { new Guid("b5160ff6-c9e6-4492-9422-96eb6b8f42ef"), "EveryTime", null, "Tax residence", "TaxResidence" },
                    { new Guid("689c42ce-e4ac-4712-96a6-c2fcda404283"), "EveryTime", null, "Identity task", "Identity" },
                    { new Guid("805be539-a68e-4e21-9b9e-b9f16dd91c8b"), "EveryTime", null, "Tax residence", "TaxResidence" },
                    { new Guid("fd2abd3f-3348-431c-a856-1a37f650398c"), "EveryTime", null, "User duplication screening", "DuplicationScreening" },
                    { new Guid("3b349c17-154a-4dc9-a683-045aa985836d"), "EveryTime", null, "User address verification", "Address" },
                    { new Guid("563af1e6-dc53-40a7-8604-78945f88b6a3"), "EveryTime", null, "User duplication screening", "DuplicationScreening" },
                    { new Guid("2661697c-e75a-4b87-96cb-3261e9a460d7"), "EveryTime", "[{\"XPath\":\"Survey.C5E7A138-2E36-43D0-BD76-43A606068F49\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "Identity task", "Identity" },
                    { new Guid("2f2e1614-b199-4d7a-a8f0-e9aa810e29ec"), "EveryTime", null, "Tax residence", "TaxResidence" },
                    { new Guid("28016523-2197-4c87-adc8-86bfa48a68fd"), "EveryTime", null, "User duplication screening", "DuplicationScreening" },
                    { new Guid("a8d1ca2b-1abb-49d7-a5ae-74e5dd3d9f5e"), "EveryTime", null, "User address", "Address" },
                    { new Guid("fae04e22-af37-46a6-875a-c93a2ea8c9a3"), "EveryTime", "[{\"XPath\":\"VerificationDetails.IsPep\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsAdverseMedia\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsSanctioned\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "Risk lists screening", "RiskListsScreening" },
                    { new Guid("2772ddb3-1d85-4625-8479-67677d9622be"), "EveryTime", "[{\"XPath\":\"VerificationDetails.IsPep\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsAdverseMedia\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsSanctioned\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "Risk lists screening", "RiskListsScreening" },
                    { new Guid("41f23381-f8bb-42c7-8b58-c344a4ad011e"), "EveryTime", null, "Financial stability assessment", "FinancialCondition" },
                    { new Guid("d06a5360-bd42-490e-82eb-c910ba66acff"), "EveryTime", "[{\"XPath\":\"Survey.C5E7A138-2E36-43D0-BD76-43A606068F49\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "Identity task", "Identity" },
                    { new Guid("f8743f21-a511-45a3-ab03-e833ab639afa"), "EveryTime", null, "Tax residence", "TaxResidence" },
                    { new Guid("62ba1278-c7f7-42d1-a901-0548fdb1a4db"), "EveryTime", null, "User duplication screening", "DuplicationScreening" },
                    { new Guid("9a1c60fb-6f96-431b-abdb-11b5fc9c5ca5"), "EveryTime", "[{\"XPath\":\"VerificationDetails.IsPep\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsAdverseMedia\",\"IsRequired\":true,\"IsReviewNeeded\":false},{\"XPath\":\"VerificationDetails.IsSanctioned\",\"IsRequired\":true,\"IsReviewNeeded\":false}]", "Risk lists screening", "RiskListsScreening" },
                    { new Guid("c1b826b7-6e0a-4ab3-813d-4393e0c0e095"), "EveryTime", null, "Proof of funds/wealth assessment", "ProofOfFunds" },
                    { new Guid("cf9f0ca8-6535-4f4a-b2d3-71ab075ec841"), "EveryTime", null, "Financial stability assessment", "FinancialCondition" },
                    { new Guid("94538731-6f81-48c7-8664-13e70238e5c3"), "EveryTime", null, "Identity task", "Identity" },
                    { new Guid("9e202236-42b4-4105-a6a4-1356a82911a2"), "EveryTime", null, "Proof of funds/wealth assessment", "ProofOfFunds" }
                });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "Triggers",
                columns: new[] { "Id", "Commands", "Conditions", "Iterative", "Name", "PolicyId", "Preconditions", "Schedule" },
                values: new object[,]
                {
                    { new Guid("f623d69c-f636-4dac-923a-5be2c3779d7b"), "[{\"Type\":4,\"Config\":\"{\\\"reason\\\":\\\"second-threshold-reached\\\"}\"},{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"C1B826B7-6E0A-4AB3-813D-4393E0C0E095\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.0FB7492B-7DC5-4277-A7FF-F3D07376FF66\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":true}}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfFunds\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":75000}},{\"Type\":4,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":60000}}]", false, "GB second threshold", new Guid("de887152-ca43-499d-8efd-6bc955b78b80"), null, null },
                    { new Guid("6f762b87-112e-43c5-b311-c32cdf723d63"), "[{\"Type\":4,\"Config\":\"{\\\"reason\\\":\\\"repeating-turnover-threshold-reached\\\"}\"}]", "[{\"Type\":5,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":75000,\"step\":75000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":60000,\"step\":60000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"High\",\"threshold\":15000,\"step\":30000}}]", true, "GB repeating threshold", new Guid("de887152-ca43-499d-8efd-6bc955b78b80"), null, null },
                    { new Guid("1f50accd-f4f2-4744-8d89-9f4a27df17b1"), "[{\"Type\":4,\"Config\":\"{\\\"reason\\\":\\\"pep-monthly-review-reminder\\\"}\"}]", null, true, "Pep user review reminder", new Guid("de887152-ca43-499d-8efd-6bc955b78b80"), "[{\"Type\":2,\"Value\":true}]", "{\"Type\":2,\"Offset\":\"30.00:00:00\",\"Value\":\"{\\\"unit\\\":\\\"Month\\\",\\\"value\\\":1}\"}" },
                    { new Guid("139c0711-ae5d-42a5-9ce4-3de2877d4d86"), "[{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":false}}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"High\",\"threshold\":400}}]", false, "Request PoA on first turnover threshold", new Guid("cdee5676-5323-4a1a-8b37-fc24ec2deb73"), null, null },
                    { new Guid("13fa5763-cd13-4c38-9586-81f6c80183cc"), "[{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"High\",\"threshold\":550}}]", false, "Require PoA on second turnover threshold", new Guid("cdee5676-5323-4a1a-8b37-fc24ec2deb73"), null, null },
                    { new Guid("c8c1ecf5-c784-41bb-8fad-c30cfac68918"), "[{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"1673241E-BC0A-4007-A353-A2C39880BBEF\\\",\\\"addCompleted\\\":true}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfFunds\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":false}}\"},{\"Type\":4,\"Config\":\"{\\\"reason\\\":\\\"first-threshold-reached\\\"}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":100000}},{\"Type\":4,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":75000}},{\"Type\":4,\"Value\":{\"riskLevel\":\"High\",\"threshold\":50000}}]", false, "Request PoF on third turnover threshold", new Guid("caed84f7-fc93-431c-8b34-56612ce52dfd"), null, null },
                    { new Guid("f7aef22c-0638-4322-97fd-e5da05918d1f"), "[{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":false}}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"High\",\"threshold\":4000}}]", false, "Request PoA on first turnover threshold", new Guid("caed84f7-fc93-431c-8b34-56612ce52dfd"), null, null },
                    { new Guid("47eaa9b2-0fe0-41e0-a1e8-b502f48a9c23"), "[{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"High\",\"threshold\":5000}}]", false, "Require PoA on second turnover threshold", new Guid("caed84f7-fc93-431c-8b34-56612ce52dfd"), null, null },
                    { new Guid("82cc6921-9263-442a-b022-00a8d1d5a4bf"), "[{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"1673241E-BC0A-4007-A353-A2C39880BBEF\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfFunds\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"},{\"Type\":4,\"Config\":\"{\\\"reason\\\":\\\"second-threshold-reached\\\"}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":200000}},{\"Type\":4,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":150000}},{\"Type\":4,\"Value\":{\"riskLevel\":\"High\",\"threshold\":100000}}]", false, "Require PoF on fourth turnover threshold", new Guid("caed84f7-fc93-431c-8b34-56612ce52dfd"), null, null },
                    { new Guid("044529b5-d983-4f57-8dba-16febf264052"), "[{\"Type\":4,\"Config\":\"{\\\"reason\\\":\\\"first-threshold-reached\\\"}\"},{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"CF9F0CA8-6535-4F4A-B2D3-71AB075EC841\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"FinancialCondition\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":60000}},{\"Type\":4,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":30000}}]", false, "GB first threshold", new Guid("de887152-ca43-499d-8efd-6bc955b78b80"), null, null },
                    { new Guid("651f41fe-29f9-4f2a-979d-f425377db305"), "[{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":6,\"Value\":{\"riskLevel\":\"Low\",\"limit\":\"3\",\"unit\":\"Year\"}},{\"Type\":6,\"Value\":{\"riskLevel\":\"Medium\",\"limit\":\"2\",\"unit\":\"Year\"}},{\"Type\":6,\"Value\":{\"riskLevel\":\"High\",\"limit\":\"1\",\"unit\":\"Year\"}}]", false, "Account age tracking", new Guid("caed84f7-fc93-431c-8b34-56612ce52dfd"), null, "{\"Type\":2,\"Offset\":\"01:00:00\",\"Value\":\"{\\\"unit\\\":\\\"Day\\\",\\\"value\\\":7}\"}" },
                    { new Guid("e1ffb995-72cb-429e-99b2-db4282da6526"), "[{\"Type\":4,\"Config\":\"{\\\"reason\\\":\\\"repeating-turnover-threshold-reached\\\"}\"}]", "[{\"Type\":5,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":75000,\"step\":75000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":60000,\"step\":60000}},{\"Type\":5,\"Value\":{\"riskLevel\":\"High\",\"threshold\":15000,\"step\":30000}}]", true, "EEA repeating threshold", new Guid("7ba96008-c7b3-469b-9f3c-307224abbe83"), null, null },
                    { new Guid("64e483bc-20f6-4eec-9f1a-2f2363cfc861"), "[{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"CF9F0CA8-6535-4F4A-B2D3-71AB075EC841\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"FinancialCondition\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":true}}\"},{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"C1B826B7-6E0A-4AB3-813D-4393E0C0E095\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.0FB7492B-7DC5-4277-A7FF-F3D07376FF66\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":true}}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfFunds\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":0,\"Value\":\"High\"},{\"Type\":0,\"Value\":\"ExtraHigh\"}]", false, "GB high risk user", new Guid("de887152-ca43-499d-8efd-6bc955b78b80"), null, null },
                    { new Guid("25540161-4ccb-4c03-9458-62cd8dad0bfa"), "[{\"Type\":4,\"Config\":\"{\\\"reason\\\":\\\"first-threshold-reached\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":false}}\"},{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"41F23381-F8BB-42C7-8B58-C344A4AD011E\\\",\\\"addCompleted\\\":true}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"FinancialCondition\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":false}}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":60000}},{\"Type\":4,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":30000}}]", false, "Request PoA on first turnover threshold", new Guid("7ba96008-c7b3-469b-9f3c-307224abbe83"), null, null },
                    { new Guid("15cf6520-6990-4f6d-a6a2-e4416dd55906"), "[{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"9E202236-42B4-4105-A6A4-1356A82911A2\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfFunds\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.0FB7492B-7DC5-4277-A7FF-F3D07376FF66\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":0,\"Value\":\"High\"}]", false, "Proof of funds/wealth assessment", new Guid("0eaae368-8acb-410b-8ec0-3ae404f49d5e"), null, null },
                    { new Guid("86107529-fe66-4817-8261-3ab8e860885e"), "[{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"41F23381-F8BB-42C7-8B58-C344A4AD011E\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"FinancialCondition\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":0,\"Value\":\"High\"}]", false, "Financial stability assessment", new Guid("0eaae368-8acb-410b-8ec0-3ae404f49d5e"), null, null },
                    { new Guid("90abd5ec-84ff-4e9c-a3db-4b5586116d3c"), "[{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":0,\"Value\":\"High\"}]", false, "Request POA documents for HighRisk", new Guid("0eaae368-8acb-410b-8ec0-3ae404f49d5e"), null, null },
                    { new Guid("af4321cb-e246-479a-9f13-84ac8942c078"), "[{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"CF9F0CA8-6535-4F4A-B2D3-71AB075EC841\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"FinancialCondition\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":0,\"Value\":\"High\"}]", false, "Financial stability assessment", new Guid("dc658b4f-a0eb-4c20-b296-e0d57e8da6db"), null, null },
                    { new Guid("d064bf48-8b8c-44c7-851c-59a69e78c37a"), "[{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":0,\"Value\":\"High\"}]", false, "Require POA documents for HighRisk", new Guid("dc658b4f-a0eb-4c20-b296-e0d57e8da6db"), null, null },
                    { new Guid("222e9d58-cacb-4094-9399-2aa50aa2d766"), "[{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"C1B826B7-6E0A-4AB3-813D-4393E0C0E095\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfFunds\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.0FB7492B-7DC5-4277-A7FF-F3D07376FF66\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":0,\"Value\":\"High\"}]", false, "Proof of funds/wealth assessment", new Guid("dc658b4f-a0eb-4c20-b296-e0d57e8da6db"), null, null },
                    { new Guid("2b6467db-ba15-42e5-9b3d-a4df4fae2475"), "[{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":false}}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"High\",\"threshold\":2300}}]", false, "Request PoA on first turnover threshold", new Guid("c8e4aff9-c3ac-429f-907e-61fd2519cadd"), null, null },
                    { new Guid("e8671add-b3f4-4c61-abb8-310b31af9c2e"), "[{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"High\",\"threshold\":2900}}]", false, "Require PoA on second turnover threshold", new Guid("c8e4aff9-c3ac-429f-907e-61fd2519cadd"), null, null },
                    { new Guid("b62b049c-4b5c-40e5-8927-d83674e2a245"), "[{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":false}}\"},{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"41F23381-F8BB-42C7-8B58-C344A4AD011E\\\",\\\"addCompleted\\\":true}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"FinancialCondition\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":false}}\"}]", "[{\"Type\":6,\"Value\":{\"riskLevel\":\"Low\",\"limit\":\"3\",\"unit\":\"Year\"}},{\"Type\":6,\"Value\":{\"riskLevel\":\"Medium\",\"limit\":\"2\",\"unit\":\"Year\"}},{\"Type\":6,\"Value\":{\"riskLevel\":\"High\",\"limit\":\"1\",\"unit\":\"Year\"}},{\"Type\":6,\"Value\":{\"riskLevel\":\"ExtraHigh\",\"limit\":\"1\",\"unit\":\"Year\"}}]", false, "Account age tracking", new Guid("7ba96008-c7b3-469b-9f3c-307224abbe83"), null, "{\"Type\":2,\"Offset\":\"01:00:00\",\"Value\":\"{\\\"unit\\\":\\\"Day\\\",\\\"value\\\":7}\"}" },
                    { new Guid("14fa6b41-8751-48fa-a862-ec7486a7b06c"), "[{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"41F23381-F8BB-42C7-8B58-C344A4AD011E\\\",\\\"addCompleted\\\":true}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"FinancialCondition\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.F9A2A3AC-6E98-43C9-BAB2-794E8E6DF686\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":false}}\"},{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"9E202236-42B4-4105-A6A4-1356A82911A2\\\",\\\"addCompleted\\\":true}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.0FB7492B-7DC5-4277-A7FF-F3D07376FF66\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":false}}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfFunds\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":false}}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"Address\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfAddress\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":false}}\"}]", "[{\"Type\":0,\"Value\":\"High\"},{\"Type\":0,\"Value\":\"ExtraHigh\"}]", false, "EEA high risk user", new Guid("7ba96008-c7b3-469b-9f3c-307224abbe83"), null, null },
                    { new Guid("9bd82eb1-4649-460a-98b3-491a2c79a509"), "[{\"Type\":4,\"Config\":\"{\\\"reason\\\":\\\"second-threshold-reached\\\"}\"},{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"9E202236-42B4-4105-A6A4-1356A82911A2\\\",\\\"addCompleted\\\":true}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.0FB7492B-7DC5-4277-A7FF-F3D07376FF66\\\",\\\"isReviewNeeded\\\":false,\\\"isRequired\\\":false}}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"ProofOfFunds\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Documents.ProofOfFunds\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":false}}\"}]", "[{\"Type\":4,\"Value\":{\"riskLevel\":\"Low\",\"threshold\":75000}},{\"Type\":4,\"Value\":{\"riskLevel\":\"Medium\",\"threshold\":60000}}]", false, "EEA second threshold", new Guid("7ba96008-c7b3-469b-9f3c-307224abbe83"), null, null },
                    { new Guid("44fe4c45-7f6d-48a8-8690-e5d92c97f61b"), "[{\"Type\":1,\"Config\":\"{\\\"variantId\\\":\\\"C2D32093-AD0F-45AD-9377-4DD12550A221\\\"}\"},{\"Type\":2,\"Config\":\"{\\\"taskType\\\":\\\"UserRiskScreening\\\",\\\"collectionStep\\\":{\\\"xPath\\\":\\\"Survey.EDDACA4C-C4A6-40C6-8FF3-D63A5D435783\\\",\\\"isReviewNeeded\\\":true,\\\"isRequired\\\":true}}\"}]", "[{\"Type\":1,\"Value\":\"ITIN\"},{\"Type\":2,\"Value\":true}]", false, "EDD questionnaire", new Guid("4b6271bd-fde5-40f7-8701-29aa66865568"), null, null }
                });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "ValidationPolicies",
                columns: new[] { "Id", "Region", "RegionType" },
                values: new object[,]
                {
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), "IN", 3 },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), "NZ", 3 },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), "AU", 3 },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), "DK", 3 },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), "KR", 3 },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), "LT", 3 },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), "UA", 3 },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), "NO", 3 },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), "US", 3 },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), "GB", 3 },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), "RU", 3 },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), "APAC", 2 },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), "PH", 3 },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), "RoW", 2 },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), "EEA", 2 },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), "Global", 1 }
                });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "ValidationRules",
                columns: new[] { "Id", "RuleSubject", "RuleType", "Validation" },
                values: new object[,]
                {
                    { new Guid("bbe234db-476a-4a66-ac23-dbca57db4e45"), "InvestCertificates", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("31474269-ffdb-44d3-9d3f-2b3bb5c00f6c"), "CompanyBankStatement", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("5d2cda7c-8b21-454a-999d-4f146ac2e5a2"), "DividendContract", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("ab90f2f2-3b5d-4e74-b05f-d443f23fdcb9"), "CompanyAccounts", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("af4b82da-22e0-4fa8-b74e-42fa285f5a43"), "LetterSolicitor", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("6c50922d-febc-4416-af3d-81f771a389dd"), "LetterRegulatedAccountant", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("21f3d70e-f290-4fce-8d0b-318a16fb19a9"), "CashInStatement", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("f6ede1e2-b69f-41b2-956d-db45d8dbb226"), "AuditedBankStatement", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("e46e49e8-7bea-4d6a-80c5-c6f556c410b0"), "BusinessSale", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("75da945b-94c9-4975-8587-9aae38f53216"), "InvestBankStatement", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("68746bcc-737c-43a1-a84a-6d5414dd1e6f"), "PensionLetterRegulatedAccountant", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("5a2d668f-6333-4f84-88a1-75eb88897a17"), "ScreenshotSourceWallet", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("5e519b39-a458-4703-be2f-fe7f5948daa5"), "PensionStatement", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("f4346a26-b2f6-4efb-84de-ca4b9ac5b52e"), "PensionLetterAnnuityProvider", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("16340711-72e0-4b4f-8e89-e650a695810e"), "PensionBankStatement", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("0ef1a99b-2ac7-4423-a204-accc38774b13"), "PensionSavingsStatement", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("f65fe169-e581-42c3-b31f-d6041cdb29c0"), "DepositStatement", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("624f33b9-21e9-4e0d-81b8-f115c0e4c323"), "DepositEvidence", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("eb29ee6b-608f-4414-85a1-69a9146074f0"), "LetterDonor", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("ada3aafc-5a8b-4e47-853b-fb3324cd71e8"), "InvestmentContractNotes", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("f4478c30-5ecd-410f-ba72-575ba268682c"), "LetterSalary", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("5caf831e-7018-4e7e-836e-6c61fe31dc7b"), "InvestLetterRegulatedAccountant", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("4f77702a-df34-4684-af5d-8c5fd9ec1b36"), "Payslip", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("1b42e59a-8302-481d-92c4-f70ab448c422"), "PostalIdentityCard", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"postalIdentityCard\",\"minFileQuantity\":1,\"maxFileQuantity\":1}" },
                    { new Guid("00c6b6f4-71dd-47cb-b636-813c4ccd55c1"), "Other", "ProofOfAddress", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":3,\"maxFileSize\":10485760,\"descriptionCode\":\"poaOther\"}" },
                    { new Guid("5cbecb0b-86d7-4aa3-9361-5a0414200910"), "SSN", "Tin", "{\"regex\":\"^(?!666|000|9\\\\d{2})\\\\d{3}(?!00)\\\\d{2}(?!0{4})\\\\d{4}$\"}" },
                    { new Guid("b512e3ad-764e-4e78-83cc-d466354746be"), "ITIN", "Tin", "{\"regex\":\"^(9\\\\d{2})((5[0-9]|6[0-5]|8[3-8]|9[0-2]|9[4-9])){1}\\\\d{4}$\"}" },
                    { new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270"), null, "TaxResidence", "{\"allowedCountries\":[\"AF\",\"AX\",\"AL\",\"DZ\",\"AS\",\"AD\",\"AO\",\"AI\",\"AQ\",\"AG\",\"AR\",\"AM\",\"AW\",\"AC\",\"AU\",\"AT\",\"AZ\",\"BS\",\"BH\",\"BD\",\"BB\",\"BY\",\"BE\",\"BZ\",\"BJ\",\"BM\",\"BT\",\"BO\",\"BQ\",\"BA\",\"BW\",\"BV\",\"BR\",\"IO\",\"BN\",\"BG\",\"BF\",\"BI\",\"KH\",\"CM\",\"CA\",\"CV\",\"KY\",\"CF\",\"TD\",\"CL\",\"CN\",\"CX\",\"CC\",\"CO\",\"KM\",\"CG\",\"CD\",\"CK\",\"CR\",\"CI\",\"HR\",\"CU\",\"CW\",\"CY\",\"CZ\",\"DK\",\"DJ\",\"DM\",\"DO\",\"EC\",\"EG\",\"SV\",\"GQ\",\"ER\",\"EE\",\"SZ\",\"ET\",\"FK\",\"FO\",\"FJ\",\"FI\",\"FR\",\"GF\",\"PF\",\"TF\",\"GA\",\"GM\",\"GE\",\"DE\",\"GH\",\"GI\",\"GR\",\"GL\",\"GD\",\"GP\",\"GU\",\"GT\",\"GG\",\"GN\",\"GW\",\"GY\",\"HT\",\"HM\",\"VA\",\"HN\",\"HK\",\"HU\",\"IS\",\"IN\",\"ID\",\"IR\",\"IQ\",\"IE\",\"IM\",\"IL\",\"IT\",\"JM\",\"JP\",\"JE\",\"JO\",\"KZ\",\"KE\",\"KI\",\"KR\",\"XK\",\"KW\",\"KG\",\"LA\",\"LV\",\"LB\",\"LS\",\"LR\",\"LY\",\"LI\",\"LT\",\"LU\",\"MO\",\"MK\",\"MG\",\"MW\",\"MY\",\"MV\",\"ML\",\"MT\",\"MH\",\"MQ\",\"MR\",\"MU\",\"YT\",\"MX\",\"FM\",\"MD\",\"MC\",\"MN\",\"ME\",\"MS\",\"MA\",\"MZ\",\"MM\",\"NA\",\"NR\",\"NP\",\"NL\",\"NC\",\"NZ\",\"NI\",\"NE\",\"NG\",\"NU\",\"NF\",\"KP\",\"MP\",\"NO\",\"OM\",\"PK\",\"PW\",\"PS\",\"PA\",\"PG\",\"PY\",\"PE\",\"PH\",\"PN\",\"PL\",\"PT\",\"PR\",\"QA\",\"RE\",\"RO\",\"RU\",\"RW\",\"BL\",\"SH\",\"KN\",\"LC\",\"MF\",\"PM\",\"VC\",\"WS\",\"SM\",\"ST\",\"SA\",\"SN\",\"RS\",\"SC\",\"SL\",\"SG\",\"SX\",\"SK\",\"SI\",\"SB\",\"SO\",\"ZA\",\"GS\",\"SS\",\"ES\",\"LK\",\"SD\",\"SR\",\"SJ\",\"SE\",\"CH\",\"SY\",\"TW\",\"TJ\",\"TZ\",\"TH\",\"TL\",\"TG\",\"TK\",\"TO\",\"TT\",\"TA\",\"TN\",\"TR\",\"TM\",\"TC\",\"TV\",\"UG\",\"UA\",\"AE\",\"GB\",\"UM\",\"US\",\"UY\",\"UZ\",\"VU\",\"VE\",\"VN\",\"VG\",\"VI\",\"WF\",\"EH\",\"YE\",\"ZM\",\"ZW\"]}" },
                    { new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c"), "W9Form", "Taxation", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"maxFileSize\":10485760,\"descriptionCode\":\"w9form\",\"documentSide\":\"Front\"}" },
                    { new Guid("ffc18c02-804c-4684-adea-2444ea3e9a62"), "Photo", "Selfie", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"selfie\",\"minFileQuantity\":1,\"maxFileQuantity\":1}" },
                    { new Guid("bf6fa364-0215-4a0e-9c03-be9edd4aa150"), "Video", "Selfie", "{\"fileFormats\":[\"mp4\"],\"maxFileSize\":1431121,\"descriptionCode\":\"selfie\",\"minFileQuantity\":1,\"maxFileQuantity\":1}" },
                    { new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861"), "DriverLicense", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"drivingLicense\",\"minFileQuantity\":1,\"maxFileQuantity\":2}" },
                    { new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af"), "Other", "ProofOfFunds", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"minFileQuantity\":1,\"maxFileQuantity\":7,\"maxFileSize\":10485760,\"descriptionCode\":\"sourceOfFunds\"}" },
                    { new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c"), "IdentityCard", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"nationalIdentityCard\",\"minFileQuantity\":1,\"maxFileQuantity\":2}" },
                    { new Guid("14b35fbd-71ba-4ee4-b43c-a91a9b627a1e"), "SocialSecurityCard", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"socialSecurityCard\",\"minFileQuantity\":1,\"maxFileQuantity\":1}" },
                    { new Guid("6da77929-3d09-473c-b707-23c0512d4c8d"), "InternationalPassport", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"internationalPassport\",\"minFileQuantity\":1,\"maxFileQuantity\":1}" },
                    { new Guid("30f5b20e-b307-4409-a79a-dcb0d2e8894c"), "PassportCard", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"passport\",\"minFileQuantity\":2,\"maxFileQuantity\":2}" },
                    { new Guid("ea2f0788-3e72-4452-b524-45fa8404206d"), "CertificateOfResidency", "ProofOfAddress", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"maxFileSize\":10485760,\"descriptionCode\":\"poaCertificateOfResidency\",\"documentSide\":\"Front\"}" },
                    { new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055"), "Passport", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"passport\",\"minFileQuantity\":1,\"maxFileQuantity\":2}" },
                    { new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921"), "TaxReturn", "ProofOfAddress", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"maxFileSize\":10485760,\"descriptionCode\":\"taxReturn\",\"documentSide\":\"Front\"}" },
                    { new Guid("657e332d-4c1d-467d-9491-8bc55548a73e"), "UtilityBill", "ProofOfAddress", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"maxFileSize\":10485760,\"descriptionCode\":\"utilityBill\",\"documentSide\":\"Front\"}" },
                    { new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d"), "CouncilTax", "ProofOfAddress", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"maxFileSize\":10485760,\"descriptionCode\":\"councilTax\",\"documentSide\":\"Front\"}" },
                    { new Guid("3201df75-b356-4d0e-9b48-76aa8d12f4ce"), "WorkPermit", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"workPermit\",\"minFileQuantity\":2,\"maxFileQuantity\":2}" },
                    { new Guid("bd991802-3a91-443c-bdf6-9af3e6efbe12"), "VoterId", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"voterId\",\"minFileQuantity\":1,\"maxFileQuantity\":1}" },
                    { new Guid("6f108a0d-4c86-4509-9b76-1036d2014538"), "Visa", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"visa\",\"minFileQuantity\":1,\"maxFileQuantity\":1}" },
                    { new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de"), "ResidencePermit", "ProofOfIdentity", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\"],\"maxFileSize\":10485760,\"descriptionCode\":\"residencePermit\",\"minFileQuantity\":1,\"maxFileQuantity\":2}" },
                    { new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c"), "BankStatement", "ProofOfAddress", "{\"fileFormats\":[\"jpg\",\"jpeg\",\"png\",\"pdf\"],\"maxFileSize\":10485760,\"descriptionCode\":\"bankStatement\",\"documentSide\":\"Front\"}" }
                });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "Verifications",
                columns: new[] { "Id", "Name", "Region", "RegionType", "RejectionPolicy" },
                values: new object[,]
                {
                    { new Guid("67a2b2c8-beab-4c3e-a772-19ce9380cb0e"), "RU verification policy", "RU", 3, null },
                    { new Guid("37c6ad01-067c-4b80-976d-30a568e7b0cd"), "APAC verification policy", "APAC", 2, null },
                    { new Guid("0eaae368-8acb-410b-8ec0-3ae404f49d5e"), "EEA verification policy", "EEA", 2, "{\"Conditions\":[{\"Type\":0,\"Value\":\"ExtraHigh\"}]}" },
                    { new Guid("dc658b4f-a0eb-4c20-b296-e0d57e8da6db"), "GB verification policy", "GB", 3, "{\"Conditions\":[{\"Type\":0,\"Value\":\"ExtraHigh\"}]}" },
                    { new Guid("5dece2a9-cdd3-4d0d-b1bc-8a164b745051"), "Global verification policy", "Global", 1, null },
                    { new Guid("d5b5997e-ffc1-495d-9e98-60ccbdd6f43b"), "RoW verification policy", "RoW", 2, null },
                    { new Guid("4b6271bd-fde5-40f7-8701-29aa66865568"), "US verification policy", "US", 3, null }
                });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "PolicyTasks",
                columns: new[] { "PolicyId", "TaskVariantId" },
                values: new object[,]
                {
                    { new Guid("4b6271bd-fde5-40f7-8701-29aa66865568"), new Guid("72df5230-bb5a-4857-bcaf-9e27f0cd8a36") },
                    { new Guid("dc658b4f-a0eb-4c20-b296-e0d57e8da6db"), new Guid("9a1c60fb-6f96-431b-abdb-11b5fc9c5ca5") },
                    { new Guid("dc658b4f-a0eb-4c20-b296-e0d57e8da6db"), new Guid("543ec1a7-6141-4491-9ca6-3691bbddb7ee") },
                    { new Guid("dc658b4f-a0eb-4c20-b296-e0d57e8da6db"), new Guid("62ba1278-c7f7-42d1-a901-0548fdb1a4db") },
                    { new Guid("dc658b4f-a0eb-4c20-b296-e0d57e8da6db"), new Guid("f8743f21-a511-45a3-ab03-e833ab639afa") },
                    { new Guid("dc658b4f-a0eb-4c20-b296-e0d57e8da6db"), new Guid("d06a5360-bd42-490e-82eb-c910ba66acff") },
                    { new Guid("0eaae368-8acb-410b-8ec0-3ae404f49d5e"), new Guid("fae04e22-af37-46a6-875a-c93a2ea8c9a3") },
                    { new Guid("0eaae368-8acb-410b-8ec0-3ae404f49d5e"), new Guid("a8d1ca2b-1abb-49d7-a5ae-74e5dd3d9f5e") },
                    { new Guid("0eaae368-8acb-410b-8ec0-3ae404f49d5e"), new Guid("28016523-2197-4c87-adc8-86bfa48a68fd") },
                    { new Guid("0eaae368-8acb-410b-8ec0-3ae404f49d5e"), new Guid("2f2e1614-b199-4d7a-a8f0-e9aa810e29ec") },
                    { new Guid("0eaae368-8acb-410b-8ec0-3ae404f49d5e"), new Guid("2661697c-e75a-4b87-96cb-3261e9a460d7") },
                    { new Guid("37c6ad01-067c-4b80-976d-30a568e7b0cd"), new Guid("2772ddb3-1d85-4625-8479-67677d9622be") },
                    { new Guid("37c6ad01-067c-4b80-976d-30a568e7b0cd"), new Guid("3b349c17-154a-4dc9-a683-045aa985836d") },
                    { new Guid("37c6ad01-067c-4b80-976d-30a568e7b0cd"), new Guid("fd2abd3f-3348-431c-a856-1a37f650398c") },
                    { new Guid("37c6ad01-067c-4b80-976d-30a568e7b0cd"), new Guid("805be539-a68e-4e21-9b9e-b9f16dd91c8b") },
                    { new Guid("37c6ad01-067c-4b80-976d-30a568e7b0cd"), new Guid("689c42ce-e4ac-4712-96a6-c2fcda404283") },
                    { new Guid("5dece2a9-cdd3-4d0d-b1bc-8a164b745051"), new Guid("94538731-6f81-48c7-8664-13e70238e5c3") },
                    { new Guid("5dece2a9-cdd3-4d0d-b1bc-8a164b745051"), new Guid("b5160ff6-c9e6-4492-9422-96eb6b8f42ef") },
                    { new Guid("4b6271bd-fde5-40f7-8701-29aa66865568"), new Guid("c2badff2-b73f-4fb0-b447-a3eb48964b36") },
                    { new Guid("5dece2a9-cdd3-4d0d-b1bc-8a164b745051"), new Guid("bc3341a5-e1b3-4f72-93a3-1a5324a92ff9") },
                    { new Guid("5dece2a9-cdd3-4d0d-b1bc-8a164b745051"), new Guid("563af1e6-dc53-40a7-8604-78945f88b6a3") },
                    { new Guid("4b6271bd-fde5-40f7-8701-29aa66865568"), new Guid("886609cb-ed63-4aef-aee9-b51b75c2a829") },
                    { new Guid("4b6271bd-fde5-40f7-8701-29aa66865568"), new Guid("9486f8ef-1730-45a7-a724-434d4e89c7c1") },
                    { new Guid("4b6271bd-fde5-40f7-8701-29aa66865568"), new Guid("fbca7efc-39f1-4d26-8e5c-4feb4a7fd34f") },
                    { new Guid("4b6271bd-fde5-40f7-8701-29aa66865568"), new Guid("caefe21a-e362-4ff5-93b5-943158102c31") },
                    { new Guid("67a2b2c8-beab-4c3e-a772-19ce9380cb0e"), new Guid("cf3d8afc-cbb4-4531-bea5-ddf2eb990cfe") },
                    { new Guid("67a2b2c8-beab-4c3e-a772-19ce9380cb0e"), new Guid("2796b0cb-42cb-49b7-a30e-84824d603799") },
                    { new Guid("67a2b2c8-beab-4c3e-a772-19ce9380cb0e"), new Guid("cbeed7f8-943b-482f-81a5-4648f4f1fa04") },
                    { new Guid("4b6271bd-fde5-40f7-8701-29aa66865568"), new Guid("828c3d69-d9b1-46d5-9498-45f3dd74b278") },
                    { new Guid("67a2b2c8-beab-4c3e-a772-19ce9380cb0e"), new Guid("325704db-6db2-4dab-9c07-ef700e341dbe") },
                    { new Guid("d5b5997e-ffc1-495d-9e98-60ccbdd6f43b"), new Guid("1dd5ae17-d87c-4b9f-b61f-a084b29abb4a") },
                    { new Guid("d5b5997e-ffc1-495d-9e98-60ccbdd6f43b"), new Guid("7fc98096-1cb1-47fb-91bd-1f600beb82a0") },
                    { new Guid("d5b5997e-ffc1-495d-9e98-60ccbdd6f43b"), new Guid("d4d1e3d0-8fb8-46a8-a5a8-bfae7cf9e471") },
                    { new Guid("d5b5997e-ffc1-495d-9e98-60ccbdd6f43b"), new Guid("782bdf03-5c44-45ad-a267-8a26934066a8") },
                    { new Guid("d5b5997e-ffc1-495d-9e98-60ccbdd6f43b"), new Guid("91ddffdb-4788-4783-a004-025b2357a9ed") },
                    { new Guid("67a2b2c8-beab-4c3e-a772-19ce9380cb0e"), new Guid("02f9e694-49f2-4669-a5c8-bea234d92e03") },
                    { new Guid("5dece2a9-cdd3-4d0d-b1bc-8a164b745051"), new Guid("aa49652a-74c8-4667-a20f-a92fe59cbf2b") }
                });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "PolicyValidationRules",
                columns: new[] { "ValidationPolicyId", "ValidationRuleId" },
                values: new object[,]
                {
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("56d55a44-fe48-472b-9879-08679cdf1f4d") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("201a30c0-2216-4c09-a1d5-06a5b3b5e921") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("657e332d-4c1d-467d-9491-8bc55548a73e") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("31474269-ffdb-44d3-9d3f-2b3bb5c00f6c") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("bbe234db-476a-4a66-ac23-dbca57db4e45") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("bbe234db-476a-4a66-ac23-dbca57db4e45") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("75da945b-94c9-4975-8587-9aae38f53216") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("75da945b-94c9-4975-8587-9aae38f53216") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("5caf831e-7018-4e7e-836e-6c61fe31dc7b") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("5caf831e-7018-4e7e-836e-6c61fe31dc7b") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("5a2d668f-6333-4f84-88a1-75eb88897a17") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("5a2d668f-6333-4f84-88a1-75eb88897a17") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("5e519b39-a458-4703-be2f-fe7f5948daa5") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("5e519b39-a458-4703-be2f-fe7f5948daa5") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("68746bcc-737c-43a1-a84a-6d5414dd1e6f") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("68746bcc-737c-43a1-a84a-6d5414dd1e6f") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("f4346a26-b2f6-4efb-84de-ca4b9ac5b52e") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("f4346a26-b2f6-4efb-84de-ca4b9ac5b52e") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("16340711-72e0-4b4f-8e89-e650a695810e") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("16340711-72e0-4b4f-8e89-e650a695810e") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("0ef1a99b-2ac7-4423-a204-accc38774b13") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("0ef1a99b-2ac7-4423-a204-accc38774b13") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("f65fe169-e581-42c3-b31f-d6041cdb29c0") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("f65fe169-e581-42c3-b31f-d6041cdb29c0") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("624f33b9-21e9-4e0d-81b8-f115c0e4c323") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("624f33b9-21e9-4e0d-81b8-f115c0e4c323") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("eb29ee6b-608f-4414-85a1-69a9146074f0") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("eb29ee6b-608f-4414-85a1-69a9146074f0") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("ada3aafc-5a8b-4e47-853b-fb3324cd71e8") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("ada3aafc-5a8b-4e47-853b-fb3324cd71e8") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("21f3d70e-f290-4fce-8d0b-318a16fb19a9") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("21f3d70e-f290-4fce-8d0b-318a16fb19a9") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("31474269-ffdb-44d3-9d3f-2b3bb5c00f6c") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("5d2cda7c-8b21-454a-999d-4f146ac2e5a2") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("5d2cda7c-8b21-454a-999d-4f146ac2e5a2") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("e46e49e8-7bea-4d6a-80c5-c6f556c410b0") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("ea2f0788-3e72-4452-b524-45fa8404206d") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("4f77702a-df34-4684-af5d-8c5fd9ec1b36") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("4f77702a-df34-4684-af5d-8c5fd9ec1b36") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("f4478c30-5ecd-410f-ba72-575ba268682c") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("f4478c30-5ecd-410f-ba72-575ba268682c") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("f6ede1e2-b69f-41b2-956d-db45d8dbb226") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("f6ede1e2-b69f-41b2-956d-db45d8dbb226") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("ab90f2f2-3b5d-4e74-b05f-d443f23fdcb9") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("ab90f2f2-3b5d-4e74-b05f-d443f23fdcb9") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("6c50922d-febc-4416-af3d-81f771a389dd") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("6c50922d-febc-4416-af3d-81f771a389dd") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("af4b82da-22e0-4fa8-b74e-42fa285f5a43") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("af4b82da-22e0-4fa8-b74e-42fa285f5a43") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("e46e49e8-7bea-4d6a-80c5-c6f556c410b0") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("822ded63-f7ef-4277-805a-d200c7a6e9af") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("ffc18c02-804c-4684-adea-2444ea3e9a62") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("ffc18c02-804c-4684-adea-2444ea3e9a62") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("ffc18c02-804c-4684-adea-2444ea3e9a62") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("ffc18c02-804c-4684-adea-2444ea3e9a62") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("ffc18c02-804c-4684-adea-2444ea3e9a62") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("ffc18c02-804c-4684-adea-2444ea3e9a62") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("ffc18c02-804c-4684-adea-2444ea3e9a62") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("ffc18c02-804c-4684-adea-2444ea3e9a62") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("bf6fa364-0215-4a0e-9c03-be9edd4aa150") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("bf6fa364-0215-4a0e-9c03-be9edd4aa150") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("bf6fa364-0215-4a0e-9c03-be9edd4aa150") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("bf6fa364-0215-4a0e-9c03-be9edd4aa150") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("bf6fa364-0215-4a0e-9c03-be9edd4aa150") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("bf6fa364-0215-4a0e-9c03-be9edd4aa150") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("bf6fa364-0215-4a0e-9c03-be9edd4aa150") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("ffc18c02-804c-4684-adea-2444ea3e9a62") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("5cbecb0b-86d7-4aa3-9361-5a0414200910") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("b512e3ad-764e-4e78-83cc-d466354746be") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("9e5498a6-a9d0-461b-96d4-6d92a01ec270") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("52a55d8b-5f61-4149-b52e-c901024f7f2c") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("30f5b20e-b307-4409-a79a-dcb0d2e8894c") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("1b42e59a-8302-481d-92c4-f70ab448c422") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("6f108a0d-4c86-4509-9b76-1036d2014538") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("6f108a0d-4c86-4509-9b76-1036d2014538") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("bd991802-3a91-443c-bdf6-9af3e6efbe12") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("bd991802-3a91-443c-bdf6-9af3e6efbe12") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("3201df75-b356-4d0e-9b48-76aa8d12f4ce") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("bf01fd37-22b1-4baf-aa72-453daa550b9c") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("5b6b1683-b74a-4335-adec-6138ac8b96de") },
                    { new Guid("69233213-ec1c-49e2-ae25-a9bd3016fbed"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("dcfe96d5-326e-4489-850f-03af13388200"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("e691c2f3-cde4-40aa-93bd-9f9b8a867b27"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("150159b9-1656-4c85-903f-8f90e4cc4acc"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("79d98c07-27ca-4168-8434-ef536b40c8de"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("4e18c9e3-a215-4527-b626-df1b9f7bdab2"), new Guid("6da77929-3d09-473c-b707-23c0512d4c8d") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("6da77929-3d09-473c-b707-23c0512d4c8d") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c") },
                    { new Guid("84d8f664-332a-4880-b942-ef1fa2707622"), new Guid("c2c64d69-2b83-49e2-912d-4d83027f3861") },
                    { new Guid("a0b5d0ad-2c34-413a-87ec-91c2d343b994"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c") },
                    { new Guid("b886b233-9e03-4afe-a0aa-4580adc01ef1"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("3accc8ef-788e-4b73-beb0-57e39bde57a6"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("24adc526-8cf1-44e3-8980-5f845411d5f3"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("0735076b-99d2-4f9f-a82d-370cf9456b12"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("b04a7a3c-9153-4507-8278-8c831cdc4a7f"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("aa8c8f46-aaf5-4ea5-bbb2-f8f811da1f5c") },
                    { new Guid("ee49481d-b3d6-47b0-afa2-db4de028ef97"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("f19aa3f4-9d10-4fe2-8d11-214b1de1027b"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") },
                    { new Guid("f20795f5-36ca-4209-a20b-2ddac7563f9a"), new Guid("bf74a96c-b036-4d70-a61f-928f7c75b055") }
                });

            migrationBuilder.InsertData(
                schema: "Policy",
                table: "TaskCheckVariants",
                columns: new[] { "TaskId", "CheckVariantId" },
                values: new object[,]
                {
                    { new Guid("f8743f21-a511-45a3-ab03-e833ab639afa"), new Guid("a9e0048b-0f6b-44f0-8a22-703dd86ba05e") },
                    { new Guid("94538731-6f81-48c7-8664-13e70238e5c3"), new Guid("3b43f013-5ac2-4bd9-96de-20a683331bcf") },
                    { new Guid("d06a5360-bd42-490e-82eb-c910ba66acff"), new Guid("d6f2daf9-3f8f-4335-a7b8-4fc383471a1d") },
                    { new Guid("d06a5360-bd42-490e-82eb-c910ba66acff"), new Guid("3b43f013-5ac2-4bd9-96de-20a683331bcf") },
                    { new Guid("62ba1278-c7f7-42d1-a901-0548fdb1a4db"), new Guid("372929f1-8597-4e50-a5ce-5881377af295") },
                    { new Guid("94538731-6f81-48c7-8664-13e70238e5c3"), new Guid("d6f2daf9-3f8f-4335-a7b8-4fc383471a1d") },
                    { new Guid("62ba1278-c7f7-42d1-a901-0548fdb1a4db"), new Guid("62ba1cc3-8802-4c13-b2ba-bb6efbdbee1f") },
                    { new Guid("543ec1a7-6141-4491-9ca6-3691bbddb7ee"), new Guid("fe22ebbc-da71-43a9-883c-b15f0124f0e3") },
                    { new Guid("543ec1a7-6141-4491-9ca6-3691bbddb7ee"), new Guid("63eaa27a-a6fc-43a8-93e4-0d7bfbd7cf23") },
                    { new Guid("9a1c60fb-6f96-431b-abdb-11b5fc9c5ca5"), new Guid("bb30dacb-f8a0-477c-941a-fb0c71c0297a") },
                    { new Guid("fae04e22-af37-46a6-875a-c93a2ea8c9a3"), new Guid("bb30dacb-f8a0-477c-941a-fb0c71c0297a") },
                    { new Guid("62ba1278-c7f7-42d1-a901-0548fdb1a4db"), new Guid("0822efcf-5f04-4d10-a144-460e2040968e") },
                    { new Guid("a8d1ca2b-1abb-49d7-a5ae-74e5dd3d9f5e"), new Guid("fe22ebbc-da71-43a9-883c-b15f0124f0e3") },
                    { new Guid("2661697c-e75a-4b87-96cb-3261e9a460d7"), new Guid("d6f2daf9-3f8f-4335-a7b8-4fc383471a1d") },
                    { new Guid("28016523-2197-4c87-adc8-86bfa48a68fd"), new Guid("0822efcf-5f04-4d10-a144-460e2040968e") },
                    { new Guid("28016523-2197-4c87-adc8-86bfa48a68fd"), new Guid("372929f1-8597-4e50-a5ce-5881377af295") },
                    { new Guid("2f2e1614-b199-4d7a-a8f0-e9aa810e29ec"), new Guid("a9e0048b-0f6b-44f0-8a22-703dd86ba05e") },
                    { new Guid("2661697c-e75a-4b87-96cb-3261e9a460d7"), new Guid("3b43f013-5ac2-4bd9-96de-20a683331bcf") },
                    { new Guid("2772ddb3-1d85-4625-8479-67677d9622be"), new Guid("bb30dacb-f8a0-477c-941a-fb0c71c0297a") },
                    { new Guid("3b349c17-154a-4dc9-a683-045aa985836d"), new Guid("6431dd13-ef54-4d99-978e-e94067913b43") },
                    { new Guid("fd2abd3f-3348-431c-a856-1a37f650398c"), new Guid("62ba1cc3-8802-4c13-b2ba-bb6efbdbee1f") },
                    { new Guid("fd2abd3f-3348-431c-a856-1a37f650398c"), new Guid("0e3b0522-8330-487f-a35c-38cc6022812a") },
                    { new Guid("fd2abd3f-3348-431c-a856-1a37f650398c"), new Guid("372929f1-8597-4e50-a5ce-5881377af295") },
                    { new Guid("805be539-a68e-4e21-9b9e-b9f16dd91c8b"), new Guid("a9e0048b-0f6b-44f0-8a22-703dd86ba05e") },
                    { new Guid("689c42ce-e4ac-4712-96a6-c2fcda404283"), new Guid("6d930727-5bcc-4014-a9c1-b08b85491d34") },
                    { new Guid("28016523-2197-4c87-adc8-86bfa48a68fd"), new Guid("62ba1cc3-8802-4c13-b2ba-bb6efbdbee1f") },
                    { new Guid("b5160ff6-c9e6-4492-9422-96eb6b8f42ef"), new Guid("a9e0048b-0f6b-44f0-8a22-703dd86ba05e") },
                    { new Guid("c2badff2-b73f-4fb0-b447-a3eb48964b36"), new Guid("34a33df0-b9b5-4205-9cc6-1f90be10d313") },
                    { new Guid("563af1e6-dc53-40a7-8604-78945f88b6a3"), new Guid("0822efcf-5f04-4d10-a144-460e2040968e") },
                    { new Guid("886609cb-ed63-4aef-aee9-b51b75c2a829"), new Guid("7147ade3-a665-4b03-9b89-99008218c12f") },
                    { new Guid("828c3d69-d9b1-46d5-9498-45f3dd74b278"), new Guid("6431dd13-ef54-4d99-978e-e94067913b43") },
                    { new Guid("9486f8ef-1730-45a7-a724-434d4e89c7c1"), new Guid("62ba1cc3-8802-4c13-b2ba-bb6efbdbee1f") },
                    { new Guid("9486f8ef-1730-45a7-a724-434d4e89c7c1"), new Guid("0e3b0522-8330-487f-a35c-38cc6022812a") },
                    { new Guid("9486f8ef-1730-45a7-a724-434d4e89c7c1"), new Guid("372929f1-8597-4e50-a5ce-5881377af295") },
                    { new Guid("caefe21a-e362-4ff5-93b5-943158102c31"), new Guid("6d930727-5bcc-4014-a9c1-b08b85491d34") },
                    { new Guid("caefe21a-e362-4ff5-93b5-943158102c31"), new Guid("d6f2daf9-3f8f-4335-a7b8-4fc383471a1d") },
                    { new Guid("cf3d8afc-cbb4-4531-bea5-ddf2eb990cfe"), new Guid("bb30dacb-f8a0-477c-941a-fb0c71c0297a") },
                    { new Guid("2796b0cb-42cb-49b7-a30e-84824d603799"), new Guid("6431dd13-ef54-4d99-978e-e94067913b43") },
                    { new Guid("cbeed7f8-943b-482f-81a5-4648f4f1fa04"), new Guid("62ba1cc3-8802-4c13-b2ba-bb6efbdbee1f") },
                    { new Guid("cbeed7f8-943b-482f-81a5-4648f4f1fa04"), new Guid("0e3b0522-8330-487f-a35c-38cc6022812a") },
                    { new Guid("cbeed7f8-943b-482f-81a5-4648f4f1fa04"), new Guid("372929f1-8597-4e50-a5ce-5881377af295") },
                    { new Guid("02f9e694-49f2-4669-a5c8-bea234d92e03"), new Guid("a9e0048b-0f6b-44f0-8a22-703dd86ba05e") },
                    { new Guid("325704db-6db2-4dab-9c07-ef700e341dbe"), new Guid("6d930727-5bcc-4014-a9c1-b08b85491d34") },
                    { new Guid("325704db-6db2-4dab-9c07-ef700e341dbe"), new Guid("d6f2daf9-3f8f-4335-a7b8-4fc383471a1d") },
                    { new Guid("1dd5ae17-d87c-4b9f-b61f-a084b29abb4a"), new Guid("bb30dacb-f8a0-477c-941a-fb0c71c0297a") },
                    { new Guid("7fc98096-1cb1-47fb-91bd-1f600beb82a0"), new Guid("6431dd13-ef54-4d99-978e-e94067913b43") },
                    { new Guid("d4d1e3d0-8fb8-46a8-a5a8-bfae7cf9e471"), new Guid("62ba1cc3-8802-4c13-b2ba-bb6efbdbee1f") },
                    { new Guid("d4d1e3d0-8fb8-46a8-a5a8-bfae7cf9e471"), new Guid("0822efcf-5f04-4d10-a144-460e2040968e") },
                    { new Guid("d4d1e3d0-8fb8-46a8-a5a8-bfae7cf9e471"), new Guid("372929f1-8597-4e50-a5ce-5881377af295") },
                    { new Guid("782bdf03-5c44-45ad-a267-8a26934066a8"), new Guid("a9e0048b-0f6b-44f0-8a22-703dd86ba05e") },
                    { new Guid("91ddffdb-4788-4783-a004-025b2357a9ed"), new Guid("3b43f013-5ac2-4bd9-96de-20a683331bcf") },
                    { new Guid("91ddffdb-4788-4783-a004-025b2357a9ed"), new Guid("d6f2daf9-3f8f-4335-a7b8-4fc383471a1d") },
                    { new Guid("aa49652a-74c8-4667-a20f-a92fe59cbf2b"), new Guid("bb30dacb-f8a0-477c-941a-fb0c71c0297a") },
                    { new Guid("563af1e6-dc53-40a7-8604-78945f88b6a3"), new Guid("62ba1cc3-8802-4c13-b2ba-bb6efbdbee1f") },
                    { new Guid("563af1e6-dc53-40a7-8604-78945f88b6a3"), new Guid("372929f1-8597-4e50-a5ce-5881377af295") },
                    { new Guid("689c42ce-e4ac-4712-96a6-c2fcda404283"), new Guid("d6f2daf9-3f8f-4335-a7b8-4fc383471a1d") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_UserId_ProductType",
                table: "Applications",
                columns: new[] { "UserId", "ProductType" });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStateChangelog_ApplicationId_LastApprovedDate",
                table: "ApplicationStateChangelog",
                columns: new[] { "ApplicationId", "LastApprovedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationTasks_TaskId",
                table: "ApplicationTasks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntries_CreatedAt",
                table: "AuditEntries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntries_UserId",
                table: "AuditEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntries_UserId_EntryType",
                table: "AuditEntries",
                columns: new[] { "UserId", "EntryType" });

            migrationBuilder.CreateIndex(
                name: "IX_Checks_UserId",
                table: "Checks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Checks_VariantId",
                table: "Checks",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Checks_Provider_ExternalId",
                table: "Checks",
                columns: new[] { "Provider", "ExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSteps_UserId_XPath",
                table: "CollectionSteps",
                columns: new[] { "UserId", "XPath" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_DocumentId",
                table: "DocumentFiles",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFiles_FileId",
                table: "DocumentFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UserId_Category",
                table: "Documents",
                columns: new[] { "UserId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_Files_UserId_Crc32Checksum",
                table: "Files",
                columns: new[] { "UserId", "Crc32Checksum" });

            migrationBuilder.CreateIndex(
                name: "IX_ResidenceAddresses_Country",
                table: "ResidenceAddresses",
                column: "Country")
                .Annotation("SqlServer:Online", true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskChecks_CheckId",
                table: "TaskChecks",
                column: "CheckId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCollectionSteps_StepId",
                table: "TaskCollectionSteps",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_ApplicationId",
                table: "Triggers",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_VariantId_ApplicationId_State",
                table: "Triggers",
                columns: new[] { "VariantId", "ApplicationId", "State" });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTasks_UserId_Type",
                table: "VerificationTasks",
                columns: new[] { "UserId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTasks_UserId_VariantId",
                table: "VerificationTasks",
                columns: new[] { "UserId", "VariantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Monitoring_RegionType_Region",
                schema: "Policy",
                table: "Monitoring",
                columns: new[] { "RegionType", "Region" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyTasks_TaskVariantId",
                schema: "Policy",
                table: "PolicyTasks",
                column: "TaskVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyValidationRules_ValidationRuleId",
                schema: "Policy",
                table: "PolicyValidationRules",
                column: "ValidationRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheckVariants_CheckVariantId",
                schema: "Policy",
                table: "TaskCheckVariants",
                column: "CheckVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_PolicyId",
                schema: "Policy",
                table: "Triggers",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_RegionType_Region",
                schema: "Policy",
                table: "Verifications",
                columns: new[] { "RegionType", "Region" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationStateChangelog");

            migrationBuilder.DropTable(
                name: "ApplicationTasks");

            migrationBuilder.DropTable(
                name: "AuditEntries");

            migrationBuilder.DropTable(
                name: "BridgerCredentials");

            migrationBuilder.DropTable(
                name: "DocumentFiles");

            migrationBuilder.DropTable(
                name: "ExternalProfiles");

            migrationBuilder.DropTable(
                name: "ResidenceAddresses");

            migrationBuilder.DropTable(
                name: "TaskChecks");

            migrationBuilder.DropTable(
                name: "TaskCollectionSteps");

            migrationBuilder.DropTable(
                name: "Triggers");

            migrationBuilder.DropTable(
                name: "VerificationDetails");

            migrationBuilder.DropTable(
                name: "Monitoring",
                schema: "Policy");

            migrationBuilder.DropTable(
                name: "PolicyTasks",
                schema: "Policy");

            migrationBuilder.DropTable(
                name: "PolicyValidationRules",
                schema: "Policy");

            migrationBuilder.DropTable(
                name: "TaskCheckVariants",
                schema: "Policy");

            migrationBuilder.DropTable(
                name: "Triggers",
                schema: "Policy");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "PersonalDetails");

            migrationBuilder.DropTable(
                name: "Checks");

            migrationBuilder.DropTable(
                name: "CollectionSteps");

            migrationBuilder.DropTable(
                name: "VerificationTasks");

            migrationBuilder.DropTable(
                name: "Verifications",
                schema: "Policy");

            migrationBuilder.DropTable(
                name: "ValidationPolicies",
                schema: "Policy");

            migrationBuilder.DropTable(
                name: "ValidationRules",
                schema: "Policy");

            migrationBuilder.DropTable(
                name: "ChecksVariants",
                schema: "Policy");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "Policy");
        }
    }
}
