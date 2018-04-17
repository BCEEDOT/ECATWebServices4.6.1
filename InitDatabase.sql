DECLARE @CurrentMigration [nvarchar](max)

IF object_id('[dbo].[__MigrationHistory]') IS NOT NULL
    SELECT @CurrentMigration =
        (SELECT TOP (1) 
        [Project1].[MigrationId] AS [MigrationId]
        FROM ( SELECT 
        [Extent1].[MigrationId] AS [MigrationId]
        FROM [dbo].[__MigrationHistory] AS [Extent1]
        WHERE [Extent1].[ContextKey] = N'Ecat.Shared.DbMgr.Context.EcatContext+EcatCtxConfig'
        )  AS [Project1]
        ORDER BY [Project1].[MigrationId] DESC)

IF @CurrentMigration IS NULL
    SET @CurrentMigration = '0'

IF @CurrentMigration < '201607111345570_AutomaticMigration'
BEGIN
    CREATE TABLE [dbo].[ActionItem] (
        [Id] [int] NOT NULL IDENTITY,
        [MeetingId] [int] NOT NULL,
        [Todo] [nvarchar](50),
        [Opr] [nvarchar](50),
        [ActionStatus] [nvarchar](50),
        [Resolution] [nvarchar](50),
        [DueDate] [datetime2](7),
        [ResolutionDate] [datetime2](7),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.ActionItem] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[Meeting] (
        [Id] [int] NOT NULL IDENTITY,
        [MeetingReason] [nvarchar](50),
        [Abstract] [nvarchar](50),
        [Background] [nvarchar](50),
        [Purpose] [nvarchar](50),
        [MeetingDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.Meeting] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[MeetingAttendee] (
        [AttendeeId] [int] NOT NULL,
        [MeetingId] [int] NOT NULL,
        [IsOrganizer] [int] NOT NULL,
        CONSTRAINT [PK_dbo.MeetingAttendee] PRIMARY KEY ([AttendeeId], [MeetingId])
    )
    CREATE TABLE [dbo].[ProfileStaff] (
        [PersonId] [int] NOT NULL,
        [Bio] [nvarchar](max),
        [HomeStation] [nvarchar](50),
        CONSTRAINT [PK_dbo.ProfileStaff] PRIMARY KEY ([PersonId])
    )
    CREATE TABLE [dbo].[Person] (
        [PersonId] [int] NOT NULL IDENTITY,
        [IsActive] [bit] NOT NULL,
        [BbUserId] [nvarchar](50),
        [BbUserName] [nvarchar](50),
        [LastName] [nvarchar](50) NOT NULL,
        [FirstName] [nvarchar](50) NOT NULL,
        [AvatarLocation] [nvarchar](50),
        [GoByName] [nvarchar](50),
        [Gender] [nvarchar](50) NOT NULL,
        [Affiliation] [nvarchar](50) NOT NULL,
        [Paygrade] [nvarchar](50) NOT NULL,
        [Component] [nvarchar](50) NOT NULL,
        [Email] [nvarchar](80) NOT NULL,
        [RegistrationComplete] [bit] NOT NULL,
        [InstituteRole] [nvarchar](50) NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.Person] PRIMARY KEY ([PersonId])
    )
    CREATE TABLE [dbo].[ProfileDesigner] (
        [PersonId] [int] NOT NULL,
        [Bio] [nvarchar](max),
        [HomeStation] [nvarchar](50),
        [AssociatedAcademyId] [nvarchar](50),
        CONSTRAINT [PK_dbo.ProfileDesigner] PRIMARY KEY ([PersonId])
    )
    CREATE TABLE [dbo].[ProfileExternal] (
        [PersonId] [int] NOT NULL,
        [Bio] [nvarchar](max),
        [HomeStation] [nvarchar](50),
        CONSTRAINT [PK_dbo.ProfileExternal] PRIMARY KEY ([PersonId])
    )
    CREATE TABLE [dbo].[ProfileFaculty] (
        [PersonId] [int] NOT NULL,
        [Bio] [nvarchar](max),
        [HomeStation] [nvarchar](50),
        [IsCourseAdmin] [bit] NOT NULL,
        [IsReportViewer] [bit] NOT NULL,
        [AcademyId] [nvarchar](50),
        CONSTRAINT [PK_dbo.ProfileFaculty] PRIMARY KEY ([PersonId])
    )
    CREATE TABLE [dbo].[FacultyInCourse] (
        [FacultyPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [BbCourseMemId] [nvarchar](50),
        [IsDeleted] [bit] NOT NULL,
        [DeletedById] [int],
        [DeletedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.FacultyInCourse] PRIMARY KEY ([FacultyPersonId], [CourseId])
    )
    CREATE TABLE [dbo].[Course] (
        [Id] [int] NOT NULL IDENTITY,
        [AcademyId] [nvarchar](50),
        [BbCourseId] [nvarchar](60),
        [Name] [nvarchar](50),
        [ClassNumber] [nvarchar](50),
        [Term] [nvarchar](50),
        [GradReportPublished] [bit] NOT NULL,
        [StartDate] [datetime2](7) NOT NULL,
        [GradDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.Course] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[SpResponse] (
        [AssessorPersonId] [int] NOT NULL,
        [AssesseePersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [InventoryItemId] [int] NOT NULL,
        [ItemResponse] [nvarchar](50),
        [ItemModelScore] [int] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.SpResponse] PRIMARY KEY ([AssessorPersonId], [AssesseePersonId], [CourseId], [WorkGroupId], [InventoryItemId])
    )
    CREATE TABLE [dbo].[CrseStudentInGroup] (
        [StudentId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [HasAcknowledged] [bit] NOT NULL,
        [BbCrseStudGroupId] [nvarchar](50),
        [IsDeleted] [bit] NOT NULL,
        [DeletedById] [int],
        [DeletedDate] [datetime2](7),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.CrseStudentInGroup] PRIMARY KEY ([StudentId], [CourseId], [WorkGroupId])
    )
    CREATE TABLE [dbo].[StratResponse] (
        [AssessorPersonId] [int] NOT NULL,
        [AssesseePersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [StratPosition] [int] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.StratResponse] PRIMARY KEY ([AssessorPersonId], [AssesseePersonId], [CourseId], [WorkGroupId])
    )
    CREATE TABLE [dbo].[StudSpComment] (
        [AuthorPersonId] [int] NOT NULL,
        [RecipientPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [FacultyPersonId] [int],
        [RequestAnonymity] [bit] NOT NULL,
        [CommentText] [nvarchar](max),
        [CreatedDate] [datetime2](7) NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.StudSpComment] PRIMARY KEY ([AuthorPersonId], [RecipientPersonId], [CourseId], [WorkGroupId])
    )
    CREATE TABLE [dbo].[StudSpCommentFlag] (
        [AuthorPersonId] [int] NOT NULL,
        [RecipientPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [Author] [nvarchar](50),
        [Recipient] [nvarchar](50),
        [Faculty] [nvarchar](50),
        [FlaggedByFacultyId] [int],
        CONSTRAINT [PK_dbo.StudSpCommentFlag] PRIMARY KEY ([AuthorPersonId], [RecipientPersonId], [CourseId], [WorkGroupId])
    )
    CREATE TABLE [dbo].[WorkGroup] (
        [WorkGroupId] [int] NOT NULL IDENTITY,
        [CourseId] [int] NOT NULL,
        [WgModelId] [int] NOT NULL,
        [Category] [nvarchar](4),
        [GroupNumber] [nvarchar](6),
        [AssignedSpInstrId] [int],
        [AssignedKcInstrId] [int],
        [CustomName] [nvarchar](50),
        [BbGroupId] [nvarchar](50),
        [DefaultName] [nvarchar](50),
        [SpStatus] [nvarchar](50),
        [IsPrimary] [bit] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.WorkGroup] PRIMARY KEY ([WorkGroupId])
    )
    CREATE TABLE [dbo].[KcInstrument] (
        [Id] [int] NOT NULL IDENTITY,
        [ModifiedById] [int],
        [Instructions] [nvarchar](max),
        [Version] [nvarchar](50),
        [IsActive] [bit] NOT NULL,
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.KcInstrument] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[KcResult] (
        [StudentId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [Version] [int] NOT NULL,
        [InventoryId] [int] NOT NULL,
        [InstrumentId] [int] NOT NULL,
        [NumberCorrect] [int] NOT NULL,
        [Score] [real] NOT NULL,
        CONSTRAINT [PK_dbo.KcResult] PRIMARY KEY ([StudentId], [CourseId], [Version])
    )
    CREATE TABLE [dbo].[KcResponse] (
        [StudentId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [InventoryId] [int] NOT NULL,
        [Version] [int] NOT NULL,
        [ResultId] [int],
        [IsCorrect] [bit] NOT NULL,
        [AllowNewAttempt] [bit] NOT NULL,
        CONSTRAINT [PK_dbo.KcResponse] PRIMARY KEY ([StudentId], [CourseId], [InventoryId], [Version])
    )
    CREATE TABLE [dbo].[KcInventory] (
        [Id] [int] NOT NULL IDENTITY,
        [ModifiedById] [int],
        [InstrumentId] [int] NOT NULL,
        [DisplayOrder] [int] NOT NULL,
        [IsScored] [bit] NOT NULL,
        [IsDisplayed] [bit] NOT NULL,
        [KnowledgeArea] [nvarchar](50),
        [QuestionText] [nvarchar](50),
        [ItemWeight] [real] NOT NULL,
        [Answer] [nvarchar](50),
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.KcInventory] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[StudentInCourse] (
        [StudentPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [BbCourseMemId] [nvarchar](50),
        [IsDeleted] [bit] NOT NULL,
        [DeletedById] [int],
        [DeletedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.StudentInCourse] PRIMARY KEY ([StudentPersonId], [CourseId])
    )
    CREATE TABLE [dbo].[ProfileStudent] (
        [PersonId] [int] NOT NULL,
        [Bio] [nvarchar](max),
        [HomeStation] [nvarchar](50),
        [ContactNumber] [nvarchar](50),
        [Commander] [nvarchar](50),
        [Shirt] [nvarchar](50),
        [CommanderEmail] [nvarchar](50),
        [ShirtEmail] [nvarchar](50),
        CONSTRAINT [PK_dbo.ProfileStudent] PRIMARY KEY ([PersonId])
    )
    CREATE TABLE [dbo].[SpInstrument] (
        [Id] [int] NOT NULL IDENTITY,
        [Name] [nvarchar](50),
        [IsActive] [bit] NOT NULL,
        [Version] [nvarchar](50),
        [StudentInstructions] [nvarchar](max),
        [FacultyInstructions] [nvarchar](max),
        [ModifiedDate] [datetime2](7),
        [ModifiedById] [int],
        CONSTRAINT [PK_dbo.SpInstrument] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[SpInventory] (
        [Id] [int] NOT NULL IDENTITY,
        [InstrumentId] [int] NOT NULL,
        [DisplayOrder] [int] NOT NULL,
        [IsScored] [bit] NOT NULL,
        [IsDisplayed] [bit] NOT NULL,
        [Behavior] [nvarchar](max),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.SpInventory] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[FacSpComment] (
        [RecipientPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [FacultyPersonId] [int] NOT NULL,
        [CreatedDate] [datetime2](7) NOT NULL,
        [CommentText] [nvarchar](max),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.FacSpComment] PRIMARY KEY ([RecipientPersonId], [CourseId], [WorkGroupId])
    )
    CREATE TABLE [dbo].[FacSpCommentFlag] (
        [RecipientPersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [FacultyId] [int] NOT NULL,
        [Author] [nvarchar](50),
        [Recipient] [nvarchar](50),
        CONSTRAINT [PK_dbo.FacSpCommentFlag] PRIMARY KEY ([RecipientPersonId], [CourseId], [WorkGroupId])
    )
    CREATE TABLE [dbo].[FacSpResponse] (
        [AssesseePersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [InventoryItemId] [int] NOT NULL,
        [FacultyPersonId] [int] NOT NULL,
        [ItemResponse] [nvarchar](50),
        [ItemModelScore] [real] NOT NULL,
        [IsDeleted] [bit] NOT NULL,
        [DeletedById] [int],
        [DeletedDate] [datetime2](7),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        [CrseStudentInGroup_StudentId] [int],
        [CrseStudentInGroup_CourseId] [int],
        [CrseStudentInGroup_WorkGroupId] [int],
        CONSTRAINT [PK_dbo.FacSpResponse] PRIMARY KEY ([AssesseePersonId], [CourseId], [WorkGroupId], [InventoryItemId])
    )
    CREATE TABLE [dbo].[FacStratResponse] (
        [AssesseePersonId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [StratPosition] [int] NOT NULL,
        [StratResultId] [int],
        [FacultyPersonId] [int] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.FacStratResponse] PRIMARY KEY ([AssesseePersonId], [CourseId], [WorkGroupId])
    )
    CREATE TABLE [dbo].[SpResult] (
        [StudentId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [AssignedInstrumentId] [int] NOT NULL,
        [AssessResult] [nvarchar](50),
        [CompositeScore] [int] NOT NULL,
        [BreakOut_IneffA] [int] NOT NULL,
        [BreakOut_IneffU] [int] NOT NULL,
        [BreakOut_EffA] [int] NOT NULL,
        [BreakOut_EffU] [int] NOT NULL,
        [BreakOut_HighEffU] [int] NOT NULL,
        [BreakOut_HighEffA] [int] NOT NULL,
        [BreakOut_NotDisplay] [int] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.SpResult] PRIMARY KEY ([StudentId], [CourseId], [WorkGroupId])
    )
    CREATE TABLE [dbo].[StratResult] (
        [StudentId] [int] NOT NULL,
        [CourseId] [int] NOT NULL,
        [WorkGroupId] [int] NOT NULL,
        [OriginalStratPosition] [int] NOT NULL,
        [FinalStratPosition] [int] NOT NULL,
        [StratCummScore] [decimal](18, 2) NOT NULL,
        [StudStratAwardedScore] [decimal](18, 3) NOT NULL,
        [FacStratAwardedScore] [decimal](18, 3) NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.StratResult] PRIMARY KEY ([StudentId], [CourseId], [WorkGroupId])
    )
    CREATE TABLE [dbo].[WorkGroupModel] (
        [Id] [int] NOT NULL IDENTITY,
        [Name] [nvarchar](50),
        [AssignedSpInstrId] [int],
        [AssignedKcInstrId] [int],
        [EdLevel] [nvarchar](50),
        [WgCategory] [nvarchar](50),
        [MaxStratStudent] [decimal](18, 2) NOT NULL,
        [MaxStratFaculty] [decimal](18, 2) NOT NULL,
        [IsActive] [bit] NOT NULL,
        [StratDivisor] [int] NOT NULL,
        [StudStratCol] [nvarchar](50),
        [FacStratCol] [nvarchar](50),
        CONSTRAINT [PK_dbo.WorkGroupModel] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[Security] (
        [PersonId] [int] NOT NULL,
        [BadPasswordCount] [int] NOT NULL,
        [PasswordHash] [nvarchar](400),
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.Security] PRIMARY KEY ([PersonId])
    )
    CREATE TABLE [dbo].[Decision] (
        [Id] [int] NOT NULL IDENTITY,
        [MeetingId] [int] NOT NULL,
        [Status] [nvarchar](50),
        [DecisionItem] [nvarchar](50),
        [ApprovalAuthority] [nvarchar](50),
        [ApprovedDate] [datetime2](7) NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_dbo.Decision] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[Discussion] (
        [Id] [int] NOT NULL IDENTITY,
        [MeetingId] [int] NOT NULL,
        [DiscussionItem] [nvarchar](50),
        CONSTRAINT [PK_dbo.Discussion] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[CogInstrument] (
        [Id] [int] NOT NULL IDENTITY,
        [ModifiedById] [int],
        [Version] [nvarchar](50),
        [IsActive] [bit] NOT NULL,
        [CogInstructions] [nvarchar](max),
        [CogInstrumentType] [nvarchar](50),
        [CogResultRange] [nvarchar](50),
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.CogInstrument] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[CogInventory] (
        [Id] [int] NOT NULL IDENTITY,
        [InstrumentId] [int] NOT NULL,
        [DisplayOrder] [int] NOT NULL,
        [IsScored] [bit] NOT NULL,
        [IsDisplayed] [bit] NOT NULL,
        [AdaptiveDescription] [nvarchar](50),
        [InnovativeDescription] [nvarchar](50),
        [ItemDescription] [nvarchar](50),
        [IsReversed] [bit] NOT NULL,
        [ModifiedById] [int],
        [ModifiedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.CogInventory] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[CogResponse] (
        [Id] [int] NOT NULL IDENTITY,
        [CogInventoryItem] [int] NOT NULL,
        [ItemScore] [real] NOT NULL,
        [IsDeleted] [bit] NOT NULL,
        [DeletedById] [int],
        [DeletedDate] [datetime2](7),
        CONSTRAINT [PK_dbo.CogResponse] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[CogResult] (
        [Id] [int] NOT NULL IDENTITY,
        [CogOutcome] [nvarchar](50),
        [CogScore] [real] NOT NULL,
        CONSTRAINT [PK_dbo.CogResult] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_MeetingId] ON [dbo].[ActionItem]([MeetingId])
    CREATE INDEX [IX_AttendeeId] ON [dbo].[MeetingAttendee]([AttendeeId])
    CREATE INDEX [IX_MeetingId] ON [dbo].[MeetingAttendee]([MeetingId])
    CREATE INDEX [IX_PersonId] ON [dbo].[ProfileStaff]([PersonId])
    CREATE UNIQUE INDEX [IX_UniqueEmailAddress] ON [dbo].[Person]([Email])
    CREATE INDEX [IX_PersonId] ON [dbo].[ProfileDesigner]([PersonId])
    CREATE INDEX [IX_PersonId] ON [dbo].[ProfileExternal]([PersonId])
    CREATE INDEX [IX_PersonId] ON [dbo].[ProfileFaculty]([PersonId])
    CREATE INDEX [IX_FacultyPersonId] ON [dbo].[FacultyInCourse]([FacultyPersonId])
    CREATE INDEX [IX_CourseId] ON [dbo].[FacultyInCourse]([CourseId])
    CREATE UNIQUE INDEX [IX_UniqueBbCourseId] ON [dbo].[Course]([BbCourseId])
    CREATE INDEX [IX_AssessorPersonId_CourseId_WorkGroupId] ON [dbo].[SpResponse]([AssessorPersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[SpResponse]([AssesseePersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_InventoryItemId] ON [dbo].[SpResponse]([InventoryItemId])
    CREATE INDEX [IX_StudentId_CourseId] ON [dbo].[CrseStudentInGroup]([StudentId], [CourseId])
    CREATE INDEX [IX_CourseId] ON [dbo].[CrseStudentInGroup]([CourseId])
    CREATE INDEX [IX_WorkGroupId] ON [dbo].[CrseStudentInGroup]([WorkGroupId])
    CREATE INDEX [IX_AssessorPersonId_CourseId_WorkGroupId] ON [dbo].[StratResponse]([AssessorPersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[StratResponse]([AssesseePersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_AuthorPersonId_CourseId_WorkGroupId] ON [dbo].[StudSpComment]([AuthorPersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[StudSpComment]([RecipientPersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_AuthorPersonId_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[StudSpCommentFlag]([AuthorPersonId], [RecipientPersonId], [CourseId], [WorkGroupId])
    CREATE UNIQUE INDEX [IX_UniqueCourseGroup] ON [dbo].[WorkGroup]([CourseId], [GroupNumber], [Category])
    CREATE INDEX [IX_WgModelId] ON [dbo].[WorkGroup]([WgModelId])
    CREATE INDEX [IX_AssignedSpInstrId] ON [dbo].[WorkGroup]([AssignedSpInstrId])
    CREATE INDEX [IX_AssignedKcInstrId] ON [dbo].[WorkGroup]([AssignedKcInstrId])
    CREATE INDEX [IX_InstrumentId] ON [dbo].[KcResult]([InstrumentId])
    CREATE INDEX [IX_StudentId_CourseId] ON [dbo].[KcResponse]([StudentId], [CourseId])
    CREATE INDEX [IX_StudentId_CourseId_Version] ON [dbo].[KcResponse]([StudentId], [CourseId], [Version])
    CREATE INDEX [IX_InventoryId] ON [dbo].[KcResponse]([InventoryId])
    CREATE INDEX [IX_InstrumentId] ON [dbo].[KcInventory]([InstrumentId])
    CREATE INDEX [IX_StudentPersonId] ON [dbo].[StudentInCourse]([StudentPersonId])
    CREATE INDEX [IX_CourseId] ON [dbo].[StudentInCourse]([CourseId])
    CREATE INDEX [IX_PersonId] ON [dbo].[ProfileStudent]([PersonId])
    CREATE INDEX [IX_InstrumentId] ON [dbo].[SpInventory]([InstrumentId])
    CREATE INDEX [IX_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[FacSpComment]([RecipientPersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_CourseId] ON [dbo].[FacSpComment]([CourseId])
    CREATE INDEX [IX_FacultyPersonId_CourseId] ON [dbo].[FacSpComment]([FacultyPersonId], [CourseId])
    CREATE INDEX [IX_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[FacSpCommentFlag]([RecipientPersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[FacSpResponse]([AssesseePersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_FacultyPersonId_CourseId] ON [dbo].[FacSpResponse]([FacultyPersonId], [CourseId])
    CREATE INDEX [IX_InventoryItemId] ON [dbo].[FacSpResponse]([InventoryItemId])
    CREATE INDEX [IX_CrseStudentInGroup_StudentId_CrseStudentInGroup_CourseId_CrseStudentInGroup_WorkGroupId] ON [dbo].[FacSpResponse]([CrseStudentInGroup_StudentId], [CrseStudentInGroup_CourseId], [CrseStudentInGroup_WorkGroupId])
    CREATE INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[FacStratResponse]([AssesseePersonId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_FacultyPersonId_CourseId] ON [dbo].[FacStratResponse]([FacultyPersonId], [CourseId])
    CREATE INDEX [IX_WorkGroupId] ON [dbo].[FacStratResponse]([WorkGroupId])
    CREATE INDEX [IX_StudentId_CourseId_WorkGroupId] ON [dbo].[SpResult]([StudentId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_CourseId] ON [dbo].[SpResult]([CourseId])
    CREATE INDEX [IX_WorkGroupId] ON [dbo].[SpResult]([WorkGroupId])
    CREATE INDEX [IX_AssignedInstrumentId] ON [dbo].[SpResult]([AssignedInstrumentId])
    CREATE INDEX [IX_StudentId_CourseId_WorkGroupId] ON [dbo].[StratResult]([StudentId], [CourseId], [WorkGroupId])
    CREATE INDEX [IX_CourseId] ON [dbo].[StratResult]([CourseId])
    CREATE INDEX [IX_WorkGroupId] ON [dbo].[StratResult]([WorkGroupId])
    CREATE INDEX [IX_AssignedSpInstrId] ON [dbo].[WorkGroupModel]([AssignedSpInstrId])
    CREATE INDEX [IX_AssignedKcInstrId] ON [dbo].[WorkGroupModel]([AssignedKcInstrId])
    CREATE INDEX [IX_PersonId] ON [dbo].[Security]([PersonId])
    CREATE INDEX [IX_MeetingId] ON [dbo].[Decision]([MeetingId])
    CREATE INDEX [IX_MeetingId] ON [dbo].[Discussion]([MeetingId])
    CREATE INDEX [IX_InstrumentId] ON [dbo].[CogInventory]([InstrumentId])
    ALTER TABLE [dbo].[ActionItem] ADD CONSTRAINT [FK_dbo.ActionItem_dbo.Meeting_MeetingId] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meeting] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[MeetingAttendee] ADD CONSTRAINT [FK_dbo.MeetingAttendee_dbo.ProfileStaff_AttendeeId] FOREIGN KEY ([AttendeeId]) REFERENCES [dbo].[ProfileStaff] ([PersonId])
    ALTER TABLE [dbo].[MeetingAttendee] ADD CONSTRAINT [FK_dbo.MeetingAttendee_dbo.Meeting_MeetingId] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meeting] ([Id])
    ALTER TABLE [dbo].[ProfileStaff] ADD CONSTRAINT [FK_dbo.ProfileStaff_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[ProfileDesigner] ADD CONSTRAINT [FK_dbo.ProfileDesigner_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[ProfileExternal] ADD CONSTRAINT [FK_dbo.ProfileExternal_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[ProfileFaculty] ADD CONSTRAINT [FK_dbo.ProfileFaculty_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[FacultyInCourse] ADD CONSTRAINT [FK_dbo.FacultyInCourse_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[FacultyInCourse] ADD CONSTRAINT [FK_dbo.FacultyInCourse_dbo.ProfileFaculty_FacultyPersonId] FOREIGN KEY ([FacultyPersonId]) REFERENCES [dbo].[ProfileFaculty] ([PersonId])
    ALTER TABLE [dbo].[SpResponse] ADD CONSTRAINT [FK_dbo.SpResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssesseePersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[SpResponse] ADD CONSTRAINT [FK_dbo.SpResponse_dbo.CrseStudentInGroup_AssessorPersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssessorPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[SpResponse] ADD CONSTRAINT [FK_dbo.SpResponse_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[SpResponse] ADD CONSTRAINT [FK_dbo.SpResponse_dbo.SpInventory_InventoryItemId] FOREIGN KEY ([InventoryItemId]) REFERENCES [dbo].[SpInventory] ([Id])
    ALTER TABLE [dbo].[SpResponse] ADD CONSTRAINT [FK_dbo.SpResponse_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[CrseStudentInGroup] ADD CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[CrseStudentInGroup] ADD CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.StudentInCourse_StudentId_CourseId] FOREIGN KEY ([StudentId], [CourseId]) REFERENCES [dbo].[StudentInCourse] ([StudentPersonId], [CourseId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CrseStudentInGroup] ADD CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.ProfileStudent_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[ProfileStudent] ([PersonId])
    ALTER TABLE [dbo].[CrseStudentInGroup] ADD CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[StratResponse] ADD CONSTRAINT [FK_dbo.StratResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssesseePersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[StratResponse] ADD CONSTRAINT [FK_dbo.StratResponse_dbo.CrseStudentInGroup_AssessorPersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssessorPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[StratResponse] ADD CONSTRAINT [FK_dbo.StratResponse_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[StudSpComment] ADD CONSTRAINT [FK_dbo.StudSpComment_dbo.CrseStudentInGroup_AuthorPersonId_CourseId_WorkGroupId] FOREIGN KEY ([AuthorPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[StudSpComment] ADD CONSTRAINT [FK_dbo.StudSpComment_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[StudSpComment] ADD CONSTRAINT [FK_dbo.StudSpComment_dbo.CrseStudentInGroup_RecipientPersonId_CourseId_WorkGroupId] FOREIGN KEY ([RecipientPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[StudSpComment] ADD CONSTRAINT [FK_dbo.StudSpComment_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[StudSpCommentFlag] ADD CONSTRAINT [FK_dbo.StudSpCommentFlag_dbo.StudSpComment_AuthorPersonId_RecipientPersonId_CourseId_WorkGroupId] FOREIGN KEY ([AuthorPersonId], [RecipientPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[StudSpComment] ([AuthorPersonId], [RecipientPersonId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[WorkGroup] ADD CONSTRAINT [FK_dbo.WorkGroup_dbo.KcInstrument_AssignedKcInstrId] FOREIGN KEY ([AssignedKcInstrId]) REFERENCES [dbo].[KcInstrument] ([Id])
    ALTER TABLE [dbo].[WorkGroup] ADD CONSTRAINT [FK_dbo.WorkGroup_dbo.SpInstrument_AssignedSpInstrId] FOREIGN KEY ([AssignedSpInstrId]) REFERENCES [dbo].[SpInstrument] ([Id])
    ALTER TABLE [dbo].[WorkGroup] ADD CONSTRAINT [FK_dbo.WorkGroup_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[WorkGroup] ADD CONSTRAINT [FK_dbo.WorkGroup_dbo.WorkGroupModel_WgModelId] FOREIGN KEY ([WgModelId]) REFERENCES [dbo].[WorkGroupModel] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[KcResult] ADD CONSTRAINT [FK_dbo.KcResult_dbo.KcInstrument_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [dbo].[KcInstrument] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[KcResponse] ADD CONSTRAINT [FK_dbo.KcResponse_dbo.KcInventory_InventoryId] FOREIGN KEY ([InventoryId]) REFERENCES [dbo].[KcInventory] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[KcResponse] ADD CONSTRAINT [FK_dbo.KcResponse_dbo.StudentInCourse_StudentId_CourseId] FOREIGN KEY ([StudentId], [CourseId]) REFERENCES [dbo].[StudentInCourse] ([StudentPersonId], [CourseId])
    ALTER TABLE [dbo].[KcResponse] ADD CONSTRAINT [FK_dbo.KcResponse_dbo.KcResult_StudentId_CourseId_Version] FOREIGN KEY ([StudentId], [CourseId], [Version]) REFERENCES [dbo].[KcResult] ([StudentId], [CourseId], [Version])
    ALTER TABLE [dbo].[KcInventory] ADD CONSTRAINT [FK_dbo.KcInventory_dbo.KcInstrument_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [dbo].[KcInstrument] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[StudentInCourse] ADD CONSTRAINT [FK_dbo.StudentInCourse_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[StudentInCourse] ADD CONSTRAINT [FK_dbo.StudentInCourse_dbo.ProfileStudent_StudentPersonId] FOREIGN KEY ([StudentPersonId]) REFERENCES [dbo].[ProfileStudent] ([PersonId])
    ALTER TABLE [dbo].[ProfileStudent] ADD CONSTRAINT [FK_dbo.ProfileStudent_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[SpInventory] ADD CONSTRAINT [FK_dbo.SpInventory_dbo.SpInstrument_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [dbo].[SpInstrument] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[FacSpComment] ADD CONSTRAINT [FK_dbo.FacSpComment_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[FacSpComment] ADD CONSTRAINT [FK_dbo.FacSpComment_dbo.FacultyInCourse_FacultyPersonId_CourseId] FOREIGN KEY ([FacultyPersonId], [CourseId]) REFERENCES [dbo].[FacultyInCourse] ([FacultyPersonId], [CourseId])
    ALTER TABLE [dbo].[FacSpComment] ADD CONSTRAINT [FK_dbo.FacSpComment_dbo.CrseStudentInGroup_RecipientPersonId_CourseId_WorkGroupId] FOREIGN KEY ([RecipientPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[FacSpComment] ADD CONSTRAINT [FK_dbo.FacSpComment_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[FacSpCommentFlag] ADD CONSTRAINT [FK_dbo.FacSpCommentFlag_dbo.FacSpComment_RecipientPersonId_CourseId_WorkGroupId] FOREIGN KEY ([RecipientPersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[FacSpComment] ([RecipientPersonId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[FacSpResponse] ADD CONSTRAINT [FK_dbo.FacSpResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssesseePersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[FacSpResponse] ADD CONSTRAINT [FK_dbo.FacSpResponse_dbo.FacultyInCourse_FacultyPersonId_CourseId] FOREIGN KEY ([FacultyPersonId], [CourseId]) REFERENCES [dbo].[FacultyInCourse] ([FacultyPersonId], [CourseId])
    ALTER TABLE [dbo].[FacSpResponse] ADD CONSTRAINT [FK_dbo.FacSpResponse_dbo.SpInventory_InventoryItemId] FOREIGN KEY ([InventoryItemId]) REFERENCES [dbo].[SpInventory] ([Id])
    ALTER TABLE [dbo].[FacSpResponse] ADD CONSTRAINT [FK_dbo.FacSpResponse_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[FacSpResponse] ADD CONSTRAINT [FK_dbo.FacSpResponse_dbo.CrseStudentInGroup_CrseStudentInGroup_StudentId_CrseStudentInGroup_CourseId_CrseStudentInGroup_WorkGrou] FOREIGN KEY ([CrseStudentInGroup_StudentId], [CrseStudentInGroup_CourseId], [CrseStudentInGroup_WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[FacStratResponse] ADD CONSTRAINT [FK_dbo.FacStratResponse_dbo.FacultyInCourse_FacultyPersonId_CourseId] FOREIGN KEY ([FacultyPersonId], [CourseId]) REFERENCES [dbo].[FacultyInCourse] ([FacultyPersonId], [CourseId])
    ALTER TABLE [dbo].[FacStratResponse] ADD CONSTRAINT [FK_dbo.FacStratResponse_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[FacStratResponse] ADD CONSTRAINT [FK_dbo.FacStratResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId] FOREIGN KEY ([AssesseePersonId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[SpResult] ADD CONSTRAINT [FK_dbo.SpResult_dbo.SpInstrument_AssignedInstrumentId] FOREIGN KEY ([AssignedInstrumentId]) REFERENCES [dbo].[SpInstrument] ([Id])
    ALTER TABLE [dbo].[SpResult] ADD CONSTRAINT [FK_dbo.SpResult_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[SpResult] ADD CONSTRAINT [FK_dbo.SpResult_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId])
    ALTER TABLE [dbo].[SpResult] ADD CONSTRAINT [FK_dbo.SpResult_dbo.CrseStudentInGroup_StudentId_CourseId_WorkGroupId] FOREIGN KEY ([StudentId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[StratResult] ADD CONSTRAINT [FK_dbo.StratResult_dbo.Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
    ALTER TABLE [dbo].[StratResult] ADD CONSTRAINT [FK_dbo.StratResult_dbo.WorkGroup_WorkGroupId] FOREIGN KEY ([WorkGroupId]) REFERENCES [dbo].[WorkGroup] ([WorkGroupId]) ON DELETE CASCADE
    ALTER TABLE [dbo].[StratResult] ADD CONSTRAINT [FK_dbo.StratResult_dbo.CrseStudentInGroup_StudentId_CourseId_WorkGroupId] FOREIGN KEY ([StudentId], [CourseId], [WorkGroupId]) REFERENCES [dbo].[CrseStudentInGroup] ([StudentId], [CourseId], [WorkGroupId])
    ALTER TABLE [dbo].[WorkGroupModel] ADD CONSTRAINT [FK_dbo.WorkGroupModel_dbo.KcInstrument_AssignedKcInstrId] FOREIGN KEY ([AssignedKcInstrId]) REFERENCES [dbo].[KcInstrument] ([Id])
    ALTER TABLE [dbo].[WorkGroupModel] ADD CONSTRAINT [FK_dbo.WorkGroupModel_dbo.SpInstrument_AssignedSpInstrId] FOREIGN KEY ([AssignedSpInstrId]) REFERENCES [dbo].[SpInstrument] ([Id])
    ALTER TABLE [dbo].[Security] ADD CONSTRAINT [FK_dbo.Security_dbo.Person_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId])
    ALTER TABLE [dbo].[Decision] ADD CONSTRAINT [FK_dbo.Decision_dbo.Meeting_MeetingId] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meeting] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[Discussion] ADD CONSTRAINT [FK_dbo.Discussion_dbo.Meeting_MeetingId] FOREIGN KEY ([MeetingId]) REFERENCES [dbo].[Meeting] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[CogInventory] ADD CONSTRAINT [FK_dbo.CogInventory_dbo.CogInstrument_InstrumentId] FOREIGN KEY ([InstrumentId]) REFERENCES [dbo].[CogInstrument] ([Id]) ON DELETE CASCADE
    CREATE TABLE [dbo].[__MigrationHistory] (
        [MigrationId] [nvarchar](150) NOT NULL,
        [ContextKey] [nvarchar](300) NOT NULL,
        [Model] [varbinary](max) NOT NULL,
        [ProductVersion] [nvarchar](32) NOT NULL,
        CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
    )
    INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
    VALUES (N'201607111345570_AutomaticMigration', N'Ecat.Shared.DbMgr.Context.EcatContext+EcatCtxConfig',  0x1F8B0800000000000400ED7DDB721CB98EE0FB46EC3F28F4B4BB7146B2DD67CE9EE9B0674296D46D475B9646729FDE3745AA8A2A65B8AAB23A334BB66663BE6C1FF693F6179679E705E02D99CCAC72C589E8631591200882200882C0FFFB3FFFF7EDBF7D5F2D8F9E499AC5C9FADDF1EB9357C747643D4BE6F17AF1EE789B3FFED3DF8FFFED5FFFEB7F797B395F7D3FFA4703F7530147BF5C67EF8E9FF27CF3F3E969367B22AB283B59C5B334C992C7FC6496AC4EA37972FAE6D5AB7F397DFDFA945014C714D7D1D1DBDBED3A8F57A4FC83FE799EAC6764936FA3E5553227CBACFE9DB6DC95588F3E472B926DA21979777C398BF293BBA72825F3938B87AB457A42BFCEC9F7FCF8E86C194794A03BB27C3C3E8AD6EB248F724AEECFBF67E42E4F93F5E26E437F88965F5E3684C23D46CB8CD4C3F8B903371DD1AB37C5884EBB0F1B54B36D96272B4B84AF7FAA59742A7EEEC4E8E39685257B579B25F95E0CBBE42465D1E69664DB65FE3E25D1D7EB2DE59DD8EBCFE7CBB4F88067F879929293728E3EC50F6994BE9C7C2251BA26E98988F12F47E2777F69C5874A59F1BFBF1C9DD30FB62979B726DB3C8D967F39BAD93E2CE3D96FE4E54BF295ACDFADB7CB253B123A969B34D990347FA907F2714D1E1FCF8E8F2A523FAEF39FDE1C1F7DA69F450F4BD24EF1A91EC7EFFD705CF6A6E2B2370D1FE2C593372C3D47F339C92FE26CB38C5E6CF0BC3D654495FDFD92AA8BFC8511E0B35921A41F73B27217DDBB3C7A7C3CB92224A7CAEE4BF4954A71877628F9A56DDC0F0CEB6EC96323917389BFA7E2879214CF759C66387497D301FD4AE8C28D7232BF89F29CA4EB02072919AD9BDD9A67FA2ED568BE24F3A4C1401534C5787C74157DFF44D68BFCE9DDF13FD33DE697F83B99373FD4487F5FC77483A2DFE4E956DBC7F5261DBA8BAB4D2536549EF26D36746F54CD26CB6DB5310CDBD3C5965C50F168BA29FEFD856EDAE604BA7C4D5768FC1893F9FB1749BACC3E843BD549E6E7E8395E946B0316F5E3A35BB22CDBB3A77853D9188DEAB8EFB44646E8FCFF9226ABDB64D97DCC03DC7F89D205A15BEE97440575976CD399A41F3B45A8548F2DD51E7563FDC74131EA15E32D89B2E1D7E7D94346993BCB87EEE77D34FBBA4893ED7A3E744F37DB7493646470855DCD92674D2168017FFAA2D1046A7DD168156372A968AFE7444D6A0374DF2A1489581104536F121CA4E054045F90595C681998E0A615A2546C93489400AC698B33AA6915D4B5ED207D52AB4CA10CE26383682665888DA2C13DEA86D110216F1C203863D8DA6E336C4F7D8C624FB6F5C7EC3A5D44EBF83F486A8948AB318C1446078C6B8CF61F888693016D959C81F5E6A2E274F48AABD86989D2A13CC64B522E30F7F5F97B4657248B6AD4E578433BA25B96C3F2EABEECB32ADEC7AA03E7DF5EBDF2615E7C485605AB0739A2E924BD12C1683D33DCD7AD96A96E6797D6B329F1D5E48204574DF71FFEACD7414726DF22112734F7DA2D1BFAFA2DC212C99E2F3F7F47A98F5961DF3EB716FAFB844E6BB4B65FF20F05F73F0E7E76A9FA29FE3D744F9FA22CF7D38F252B7F89D3917A3E7B8EF228FD94CC86D1AA426FBF26EF5F424CE4D5E6D7425F7A7050DADA989BB3476A10C49EB869DDFB4DF4B248A3797839BADA14170CC99AAAA0E07D5FAEA278A9E8F5EF83F47A4B1671E1262A66BABA5CE99C1EAE2AF56AF3719D511DBECD49B1D7869FC589F9882F48162FD6C53AC60D980E46B2609A26E95821B6DB9E7E2EBF175B6EB454D1D5C14874354D185D6DBB2D5DBF44336AE0BCA8C86A4124AAEA168CA8A6D996A6D6C0EC638322348926AA294D7764B64D6384514DE37D639C7654094D92692CB6DB9AEB77F9765E6A509C572D88C4ABBA05E355D3ECE3E8DCAD391FA7E706DB9E5BF07B7E80160DCB2C4B66717140399B518364F532C041A1C7B1D764D7400EBED2AED2672575BB848F95D4603BACA45D5E493DC4DAC4E840C45A324AFA88756B65F890EA1AD941A87759A82527D43915B38C9CCD57F1BAEFB1E963764B36499AFF2326DFBA43BF2BB611F6AB8A17B063B996FE8FEB0AA8B1C3EBB5C12E7435A4B4EE35E0FE5DCDFAA307A29AC4938993661286DBE36276F644254B9C9751B5533375889202BFA9C876D168526F7D145B47462FF5F850E1B922ABE11DD21FB30B52787BE67D354D8DC6DADB527F671405A8D139462AA701C5554DFD7FE2D1130173706BDC15FEC5153DBFA24AB28568D4054835020529470CD4563196A86E49B6A114E33ABE0369BAA32728420F51A934020C101E040AED328EC2DFD9204447C202E906A38205C7A3FCC06148DC5E3AE4F6AB5B1BC86EEDB4D779DAE226B0B34D3D2E75386311D9E1943DFDCD474F212EE8CE9751967DDEAE1E7CDCD169DE42907435F8BD661ACDAB234829F6D9537FDB809EC3D2DCED3A0520CEF3BD8CEA9EC1D978D09D4F1CB75FDDDECBEC9032A952A3ECF297209C08A4235590475B31E2BA269834A6DDFE36A2DA6B71DA3A00883CA955A6500671BC32A15242BFFE354DB61B98D6A2B9852DE10092512089721CD2710018879BD1618B098600380D82D912FB47927E5530B96D060815DB241225805EC7FD6E5D7A7A825BE21A37E4BA3673AD8EF9D5478478F10D80C0EDBC19C27F5C3F53494CD297E281855340B8C4873EAE039941E3BB333896F67B812DF0BA5700FDA6C0D22DAB81BD2CB4AF7235DECDE832EB49F9C4E25A1A99D319241D1C689234CDD2D91282B17EC6D49ED84D68E44FF640B39EC6C43E0E47E1C3B234EB14D4393AADB8A5A72351000629E5605404F380B674B7BA4747330308D2DBEDE70A5A3BA07E0E0FC91CEBEFFC90508EBAF937A484DDC0F59B3143D71EED9B1FA2EC6CF6759D7C5B92F9A2FF61FEFD43234D0275872B8ADDD9AC2D1C09A6FBB6C29920EDEDD6E46A7DF5BC2F1DA41984408FF15E284F526B46EB8D0F2DA31D2E0F5A729D182DD00C421831DA85F26DFE94A4D78FCA2BB5425B755760D527A2E3416C07DD0E12902DB90A2BAFA75747344CF4FE1F4B0F693D72FD9DE52D99C59B5808EC8521D477951D98E365986EF5013C82BE54B25FFEC0642A80AF1CA7A55C47B6A3ABBE311A57016A33A212DE762CED4C5B2D6450D01010CD727617B5C6336D3A051DBC92FDADC3DB80F52DACF5DB81CE676D4C3EFB897A048C43DC64100CB8FD38388FB0F95884CF34E3E1DDCE4663E23F711C97EA4E1FEF15B8D6D7025B0C4A75B9EF7E2607FA03CFE62A389341783AAB0BF6526FE73C8BEEE09FF77CBC3FF8DA2D6EECA920DE2459CCC64FFF08FE6AD763A4E416549F36FD38AE5D8F6226C4221E6C0BCDC898573E3423836E5CCD581EFCAC545C6B5C4E55310A43EAB3DC81B1EE915E54878BEB38F3E79664F9D93A59BFACCA70BC7EEECC7A317C29B39AA32E58FA4F1F316E29895017E83436043B87913F3791AC4C15BE241F97811C7E3888466CD710E9EA175A4670D2340E7B0175DFEE03089D2C90FAA4CE41DA9ED51987815FE7829ABFB213C2D30526D70B7C8709836808F6763A1226CEB31D50A03CD802075B601C5BE06AD3EC104327806254D1D05DB5D1D0C37654ACDC4561023481D11A4B40B151E297123DB721A58A04372C2725C9A8ED9E511E2DA65175622FED63B138FDBD7BF1A45716E59CF4D72AE774108B2455ADC1BF7A79E741D9AC7DB6F2373FE9538A242374F11419C1524BABBFF9FAB799CBD7E7E5920AF10CE8FD43A078980BF218D1B51B624C579BBB4D9882251FB39B345E4569EF53F104DD8DACF8821B55DDB6AD4E8CF507CDD3846EAB528049672615AC437008BB7E913816A32128C080B8167F43509CA7ADDE79883601FA10C4E212DFE6393878C68321D4A115F249D08A629B27E038CD8AC859F8D9773FAA592F374E38E70B476987A1F4AFBCDD47507E75458AFDDA3898C5FEEAD4E06D98FB103492DEC39DA10EEEE843B171F89E7908B92280AF27A5FA979738955C2BFEFEB20F85062B90412F81437A598452686809D43A1AA34580F219EAAD0256135FC018915E025A7B11ABA3899AE212A49B5E986411087FF82841F67A01C99A33EEE7E426DB2167701D52420C614D57FC2D6B4FA9CE0E7E6EA7EAE91AFE8CE2A7D880A77B2CC1C81EE65C21EA24933388F96D0CAE487F9BD5FB0DBBEE5992A566E00824C3F454424DDC63DF5B8C06D3BEBEC26A17E44EBFC012D44ADF57CBBD9F3F3752DC1753E5F4A3D29692AE2EA4639C18FB9EF98E6ADD651F5F09BBD6FDE804597DE17AC3426D290CD8B607D070955B7112D586AA9DD6F2149FDAE1DA57CDC52DD71F46D37953525E546625FFB60667262834E794B9CB65F2ED33F956D4295B6DECD1E91FD1A3B65ADD8EA90E0000F4FBCA503E6B37743A00AADF20B7C20A8E07E9A9E018CE7A391AD6D80E27C3E14E863EEC988B38DB2CA397EB746E5DBE54521EA521D3FB85F9C7ACA6A93FAADF9A07FB672989863ED9FE7B11984A8542134BEA2D17CE1F245E3CE5B676A3A8A8D7D9B7E1532EFA39AC6B2DDB4E6B63C62D04011ECF0130DF26AED356A5A2D593AD2B3DE8EB195123E09B82D1EB256CD0D83C9D52E0DE212FBACFBCE8CED918A1B03800CCF1A9AB11AD80DD898068A95559A04681D997EB34592EF14B4DEFCF8E0D6E68B197CA4E3AF5A6290B5EF3DC4F8DF112D9A10A0E1E77B7735570CE93751ECDF23089B08B9BFDC84F555C8D83F3294E073789DBD1E8AABFFA1BD2303D6936223684856A485B8579E392D0C05C5DDE38160B52153CEAB173E9B21E8B3B9C29B937DADA46FAB29C486D23154916C98EBD5EF5B3E80E0E1DD5AD508078625F37E8812EFCDB25173096A1AD1410B04F2B1F87B31FB04F248387F0623971ABBF4886D691714ECF026456995BC830D47E1E18028C8D56B8839C15AF3F3F3A83EDA0760FEEF0BEA8DE9327BAF8948F2DFDEAC291B213681CC5CE0A04D27E1E1CC56C127D6D84B47D627045A4349C41DC49F1B10F23DC355FBD7173EF3546D57C537F69BEDFAFC3FD1616F5987E2564DA989195A9BA2CA82A890AD00C153DEC9F42852FEF3944355025D970E15063EAB104306C1FD8C37B0C46F96E6DC0EC2FCE298C950C1E2AF58BF3AB4025B59EF2BE8833E677531D3DEBCB61639DC0C67AC8DEE29815A5976E56690F7F2951B847BF9E94C7240297F7B680DD9EA6C1F55A72CEEB89600AF5EB1CE3F6762AB4665A071A65EE623E550294BB188680957AFFDCC542417B039AA52F30D20540CD084468FFD5EFF8FE503F97024C338421CBE0F5C8D6A126DAE369C24F9A7DD6209858A6FD8927CDDFD3FDDD639A7B264183E5D6E0D72E9858FA2BD34D804B92A3D90754B0A04A527EE018A8AADC8A3D565E32885F022B35F9DB1B7A2544D24E87AF54C36D55252F05D2F7F86DFEA1422AB4C795D11FFEEEE2AF36957E688472F0F8D14DB189110FE5C5DFA724FA7ABDED6866D347758DD3D8966C238CB4B7EBF5D37B081E48972583C1E5A11158DF15C28B6E90FAE05D134EA1E32550F5F52FC8DEEEA1009E4904AF582CCFCFEE679D350DE5ADE772637E36BA0ED961AF0354CF7EEE75D769BC88D7D1D2E3D1E717CFF84A54E7DBD58ADBCE2EC82C5E45CBE3A39B94FEAB14CDE3D77F3F3EBA9B450566976EB6F3B2ABB36F513A27739BDE7E72F1FE86EB6CBA71176C954FE8C9A1D88AD62A0BBA6339D53C357A7922974875DA18F8B48B1EC277798487085E455F211E4E8C9942FF6A7349697F26833F4CBBDAFCB130287EE0A7AFE87BB9F8DAB74E036D324D3F425D15EFFDF87A5953127B113FC759171AE1BA8BD7DBEB7932B8E4349BEB105DF5CAEC2FA4C3953E4153EC0A90786E6018DCA1CEA836C53FD265FB89762C35A4E9581A70E7DB2E4D8A66B784C71AE281D4C86E273D32DBA6E5E6D3EBBD7D83E6C77E691FD14D3DCBBE25E99C5A8C9DB277C3D6A0FA10654F0A65F3572FEFF74736E46FF0B7C28D64DD37308C15CF37C926BCD0DE6BA15CD4FB678F5C3F79F4F8787245484EE7F04BF4952E9B06E9C1EE558958C5B1FE77B021EA0B35335A85640C6CAC6F3669F21C2DAB00D378F86A7A5587D32ECFEB7C835C8B19A8849A59BD6F813A2D24B6496A4802E8A787E28C2A20FF9AA8457BD045C3EBA28EDB83E80937196F6902A55C6A95E55C06E925E9E7C9C26B860E0EDF41CA8750CD3B5620A3958850F92F369C0C16BD0D7F65BFA8FCBEB7D17A31786FE6A7013B2DE02F5D048BEEA003547D1DF2451885F4CCA34DA18AA880CDD278132201DE473AC7CF51E84EA99D12B2BBEC963CD3A5D87F82A69D6783554748A20D044432BF30B8BE3658FF70788A641D17F27AC2A03B285F9D61C23DEEE8A53629861FEB4195A57CF70A7312A57BEC30A7A9CB7669045F6FF35912A2AE38EDCB5EF431F139CBB26416975CE00FD4F767E509A25868457E229EA6CBF5FCA852D12074A7C8BB38D8F6007E458524DE50B1A0D4BC3B7E7D2C4EFFF5BA5A1347154A2AD051368BE63263E888E67664B561251D591D8048D9FF903AA40249D24222A2E57952EC4651BCCE65E98DD7B378132D4D18247C6C28FCC5D8DB6EC4960BB221EB426E4D3861D23FE31F92C9687B13A646C7AAB7A78CE0A9E5B1BEE9688E3BA8240A70900C36F72D414450A40710BE9B2A6D6E3B3481B0572727AFFD0821C21B93E9C76E20AD8410E145DFFEC349DFE5F7624788C4DD4B9AED066E2AD2D7D2834B5F3BB4A1A54FE44D68E91379B103D2C7BFF76ADFCD61930E8343B228BC0AD6ED7CDA2E00F192633665D1F7225E6A924C665911460FC2E381F420B83AB188B91CABA7D78412FD83E900E31D69DD24B8E900830FBE6E12D0281875DD24567BF30EAF9BC4CA086ABED9FF75B39D7729EDEA2C5FB848CBC0F09A61E0ECD60CD0C1B82B062768AFD60B3EB146ABA5FCE2C75A2BF5831C335116DFE7785F2B62BD2B66ADD40F87063C8478585A3CFD01BC24AAF9E927BDE16591CB4C68242F709E42442AABACA406674593CE0009552E8121143944D0606ACE2001EA64B702484CF69C51C1F7912EE3A999A60772297BDF4DE444CCE31B5F124D7B6B7F49337C583DCCEAA95F6D81D58D30E9567C03AD2216DC72E353F504AC252657C620775106030F606B1930C5F04C2EBECC1D4902EB57E88C94E0F220C1C212D7DCE09BAB6C083520622A611EFF14A0184410C14467C7A8772EBC70346D08845EA9D4932A0C8B179B3662D54628D525D976472E55E308A433F199DA41E9EC2A5F99C84E0B6D299B035DAEC2A481526D7E57D05B30252605964B89136662D9C4048E79FE224C7160034F9E0C8E9DBE1848DBF317D089853FCFDB910B2723905B0E67F68E38E6D8013439534C275E2C0B3D8C7849B5A9A5209136D74B103913E83199672F81221AEE5BB818468E1BE9F61DADC4C9A0E889C4FAD21BC08D387C1582EC49C6705ABC4CABB5F2B13B95801314D6E515F6306D621F8AA0364769B5D746C23C8E6D870D7002BECEF659AC8FF3B693E538DE0043D80D8AA2EDE8AE6E52C19D51BB1B67B7A649FDF7806E4D838187B055F54CB1716B32E90A4793400B5792AE76362F770EAE244DD56D43C11EDF95A41E472039DD79575297145C735C17012189B4D04F0AC43B146C83D11E40FAB0F9D801DB14AA7B8D4987B2087627202C988DF0A9EA67EF80FC29C80F20828AB9D93529E48B9A1B490B52E1DCB34CC2C5D1F95ECA52C4C31EFAF53499CCB854652CDCE15F3F75E30C20B0A01B8534EA2B2FC342EE10D0A8ADDD6CB89E061073D768C669440C392D0CD7B8C4690C39F49EA18F3084C16DF70A8B2534910843354913F0490DB1780EF185E64BA73B281A493550DCCAB39925D7C63239D60EB15C244A46936789EB3B2468E64FBD75F5C8054173B9BDD3D4321F4D43FF084FBDD5D36BE847FE219E7A2BEBD51B2E1FB410B2F7558455501EE764AC236B170EC7BA49DCEFF3713B7A3EB79E993072DF0C28F47C3FC88D0D1AC13980B8830485F23CAAD96F76FFC27C3811F933348D5565CEBDCBDDF8D6314ECA78C6C40EDBC75C1E0F9BAD5EF5212680AE995EB4FD8DBAE79B50B603DBBEC984EEF5CECF31C04CF9C29F0C2EFEA36B613535A115B17A16764317B7B5D99B6025A30021F41B384AC8FE09A4AA0BEB38214F91695A8A02989D06AC37F428085F8F2F80BAC73A3C9C77411BEB5D0EDC7F4851DAB9B8899672FD6629837A179C5137459C8A909B21CEE5DDD80419B237DCAE8E4768E39F5885250EF4AA55411DF86AC7CA40EC19A788F12BA4C0EAF9B31B82DBD06EB27F4AA0AAA4A8B6CA50461EFE812B464288BD1465EE0E6CA7D062A003B1D37DC50753D57C256D0ABD6724ECDEB41ECBA9B1751ECB99DDD07842A1792173905E64C1CF94825B7EE1F49C00EECB32638ABFDCFD667405D096669361738E1D3B711432A2DA2D612B95F56741A4B2E9CBD2A932B8540A748D2795C264D848E5D8EFFE8411B57F1AECECE217F6B238F4FE2E5138C26B552DBBC28BADCC10A37D7D517E3D91C4A9066E1D187EC0C4A9E37A79D4A40475F5A839BF1B46A41CE4A83B3DA35F4022A78FA154C91DDE55E823B5969200EA4DCBF81D38600363A82FABBB580E7C4F36FADA4D0C35EF3ACC7AC6DE49193A14FDF0B138CFBA70B0F8CE8D778E8C2B3B4458A672C3FA33B80DB961B2AE762402DB703A0C4DFB1F22141B60597B496621F5EDC5508825D676069E65613FDFA0CB4A1CFD7E2F2991FDD31FED48EB88F13ADB483773C510643531FDD9F9CE875D53321BA62F68BD96953C0FD31FF0582B8BCF3E6925ED42BAD000A73AB14F977C9A1E7D6CFE5628382C0BA11D2312D95C26C2AEBE91D7529D33D86129D55F865C494D9763E53E36A6CC4424BC6441369E9E9E423D9694EA3DC5AA8F82C8E6A88E63137A46B66F76D4856CF3C0DFF075BFDB3BB9A9BDEBFF311EF51F5EF4BBAD12550D6A1976C0553276F9699C9E7D5E258752ED9A5562F20886851C64858CF91406A2C06416FD49EACE5D2F5AA78EB0C91BD15B982690316202E922F627578455A208D32C11BDA56CFC4758137893BC179921D8E40906A9B90170E419BC2A2B83E6153CD449E8DD514D460015A666F60EEC93E2009A84111A17A7FAB381854DE80CF76CD68081840F262B985BD36C4A4CC8D1260D09229955F7CD305049E4C120C9AB200285620BE458CBA6BF7B629831C1C4116644DFEEC3C9DD873FEFF2E8F151277735D854E4AE214775DB538C6A68A913D8125AEA0436EC80D4DD91D936A573715F4B0D6ADEF370E0B9A106B12D7A266086440891683F0706B8FF60A28370760764A72158530995079B8AC672AECDEB5D6739541FF5A9B33C17050E20775784E4F17A7196E7454FE4BEF9072A81D807902C0AB036E705B41BCB5DD19378E9C80926683AF61B5D3FD4DF4C46E8EABF8D65AEFE7B60916B7A0124AE21388CB0098404708968186E4241FDC9683276416671511C572B5C222024550D8C8D3849786DE4C8E36EED248218F101640F9B8F9D10BA389B6D3333B1934041C16BA1AC444FC6BD53C287921F42FCD079D905013C4F16562591117830809001B50A1D44BA00EF1916532E8BAC194900E1D44C971105A133A35ED26F8A8290EB9C7E41D29A8CCB599417BF91EF74A2E982CB9355B45E2779F9F9CFBF67E47C99169291BD3BA6E4CAC79102E71DC91B8BB61480E2FA383B3EAA1A19B1EA5A0131E511D5EB17C2D2AA4B33148DEDA440D519A91A94EC0107C2C71F8074C848B25912084BED393023E68294B96052053D0D8816E5E5F79CA4EB68A9C0D5806871B53E790C530B6086A8B98B816893EEC33428714C8608D867F5321236D84087888B050429022206B5D40939460112F9A7E00654B6393960745C0E141B7445414E2DCAAAECAA062D937E4846C7846868D0B0B9D2204C7C2E352DB226D72184A879DE6882049DCAAED56868D5AE1523A818CBC25472F1C5243D2E3343A9D4ACB5FB52BB02D473C8671E3340A6E21A175FA65766CA85C4577AB440862D23B978B1095295BC09B9370CD0E994919C9AC244FD22ABAA7B886FAC25314CEC0B649DEEA9F25B29354F9DC54C4756757582885A7B1DA5C1D21CDE21249D134587A43D8281689823B176BF5DA8D7A370E03041A75A91FC11498F4D259C4CB3212A449CDA46090D63B04BE6EB3D635217F431B0BC112B008AE709C9BDCAC3B707CD7648AC0D2E9D4E74D89A3325838D3B1A88E7429E0306DCA9EF793ACB56E68B08828F41808478D118EB0A4E8858001EC826BB2F46746639CA8816443B8406B21F235A2C00239883466F0E705B47F7560E600402898F04FE00628B6C722B1884A005F8241D0F06E15602AE1F04D26E5809B89A7A732B0197D710DC6293119E6DF3278C57009C6A483238C827E1DCA5E4128072041E35A6BE8E4750E0B5624042E0B50F1E0961D60CCAF644E3973785F97DDFDAF61A0671C0864362BFD1B2AA3E3098B28BC38DC8153E0DBDC5EA96DAAC9BD884710CA8A924B45F7895AF0E6B9065C8BA25DADCD20D7699672A707C848AAF20DE09AE1405EB548801F6B1AE1E0F9CABB31FB14710886332986A401234CCA1F6C0A0E48E8C0CE08A92DD8E2205DC6121D2A4BBED92E65B71DD250C8A39EB69644871BF1588518C51A3E6139298151D929C8AB53797E41CABA268C2F699A33A274CAE23B59D0040AAD5AEFC01A6CB09E7B3D4687300EDE0F602DB67EBEFD4B3098C58550E480C5EF5C528310A55C6EB4D6B576702059700288D9AE58051956D706E01B0A90C017F42D4EE161A552441196C401A2564B59985D43E1B2BE34805AE5807F857F021D8D43852211EDA38622E3734360002A91E96990D20DCC0685865660328D9EFC0A8EE4134BEB54930F8404450882DF844AB510DBA85B1174E0A564060F81000688821C2ED9A822710C2706CA9E3150CB9C3431B8E89FBC823AF78BC00CBE4700DAFBCD339515058B3F1E95C28C095AB21E7740E14E57CF41538A5FB0481341408A5F3C451C802BB4EB8BE9940110DAB90EC24F8B0E40C25FD5925E72519CA12E0EEFD95F72C08A4664446F72C6274828E4D23DDB2F07DD72A5175D9A2FEC07490C2775E1928E20EA8FA91EC535A362A925529060A67ACF2C142385395A99DDB9B7B7ACDA64BBCA4189B5EB7B9702CAC76E32E1A0D17ADF21BF530559F625C34BF36D5F6106E01736468A51081B618A956167B70319C44B6E54E9A13BFEEF88D83ABCECBE857F041DCC4C3A3421AE024DE76AFF0310B200663517895ED9812C48FDC74A65C6D009401FDCAD565C98B70AB092A4E8E3B4715D0267E16F1232FCE1B0929B894D4BAAD47C0906641C950FA881EDDB2E2E3730D4284422D2FACD2BDA92895B076735E7CE2598C4A944A218218DF875D60397625D3E02F0CC6A9ACFCCE31B08DDC36619FB2B2FD9057CC9A02F0E63C044BC69B0C562C1AEF8987627DF8102E7AA95CB99E7D586D73D51081EAE63D990654231FCCFCCCE18AD9DAF82B236302FEC26BFC5538DB02AFF60C30CBB0343437307D7168736F9621E241F74FB37AC566CCD3153AD68D5751EAD81F5315558D8DDD247EF95CD633B6E3B05C02D974D85C1164FF5CE54A1EDB1CF5FDB0B47B7166C44EB8CCAD6EAC52A15B7F6C94CADA9A9CE73CB18E7D6267C63DACB8A976944079538F3C048A990E69521B14A734E5A7A2A4A57ED470514B9F7C854B58423D0CBA3F09E50AAD980B2600371CB99805DC3B6BC5CCDF720703715469529A97E2D30D536B5CF660616067B0C10B37AB6B57C33B57D3AB9BD16E5BA1226026CC51BFD152540BF3C19C60CFB3A4DA4F6AD69879C4C11251FDD812CC2F6E7897EC70916C738BECC0A1E0F7C7A697C79637C7A6D7C60E2C0A7A610C5592812F35B53567C44B4755D5198B0B5B13B443873DAA4AA218704B554445393CA48C8A2FEE21855364F4FE1EF9B70865AEA90A7C70C3404A7C30649BBFF0170B730C39F6A684043E76B0C80444B65866C269EC62710816099F25AEBF1616CA19400A5855F180D79548CD0356ED321968545A17A971A065A7BB10281EFAA8F2F643F3A778E0632E04615E3AA1A9E1F164309A34F2DC58B489E49951C93915F5A961F0D4F103AE192CB3B901CBC084C1CA818979837D314CCC192CE3F5C12A294137C02375126F6E10681A6F867A263B95821D68E2EE81F820E78C8638A1C92CCD8F00CF2DCD72834DB3A5E2079E4D7A188E60498C217F8D49BE63DE9DA2C978CC99A30BD3F75FBA24C7225697EBE5228D7181A54DA5DBB6BD3DBD9B3D915554FFF0F69482CCC826DF46CBEAAEB769B88A369B6292BA2FEB5F8EEE36D1AC18C73FDD1D1F7D5F2DD7D9BBE3A73CDFFC7C7A9A95A8B393553C4B932C79CC4F66C9EA349A27A76F5EBDFA97D3D7AF4F57158ED3197705F656A0B6ED89F2285A10A1B5E0CB9CFC12A7597E11E5D143549C69CEE72B098C4D1CCC73AEE571D3939C1B589EC0E29322595AF34DF1EF2E43F1C9DD539492F9C9C5C3D5223DA9BB65F3090B083B86FE42C7584C70395CC24CBFEA63FAF9DD2C5A466993B699C91A7D9E2CB7AB359E451AFF9AC949CE2251A42AC7717D49E6098FA6FAC51CC3F526E511943F588C665371906ED9F936E351F12DE638E9319E6229BEE5F1B1BF9B63BBD8122AC28447D5FEE842958C4E6CB3E05F328F1F63327FFF220904D7628F51A6926F9131BE3D15568BB83E4FA5052A284B71CD1B698476ABF2A30E90FDD84017A05F0EAA086E499489922E3499E33C7B2852C1CF726121B6BF9A637A1FCDBE2ED264BB16C6C8FE6E8EED669B6E924C90C7F6476B9E01B2CD364C4DB43BBBDFAB8863A73A7351C731A002C614ABE2444C51C44A3B9D5EF6C28FD975BA88D6F17F106147E31A26231AFC51D78F5C708507EC8542FD39BAB4DB3A6ADCDA46ABABE198DEC7823153FE60FEFD876455102F1B0E5CC37424A07230799A7AD0E36732E9C887C34FF7C7AC30109F89B85A9B5F2D04E7E1F78CA4224DDDAFB6988A7F43B8AADFCDB17D8AB25CC6D5FD6A8EA93C89C9A8989F2D6C84677AA24B3F253360A5886DE6587F4DDEBFC80476BFDA1C297E2DB612418B37BF591D4D1EA93E8BA161B20D36186FA297455AD634E2A4BFFDD506D779B2DA24EBD293C222637E36C776B98AE2258FA7FEC9E680B3880B3BB1E04A414451CB493CE6401036632E5C2D71BECD49E1A711163EDF743842D91B135D4671AFF604963BDDDCA4C031ECAF5581AADFDAA148E6674575B49524B520C06444ADCBD9EE55C6B0B4F4E6328663D85F191B4906F0F8821E12802035170014C1FECE3F6E5A57E12967F355BC16ED6BAEC906E72DD92469FE8F987C938FD87C9B85364474E004359F1427E447FCC5AA75F6F2AFC5809E2AEAE025701D488DE6935AD12122EC7EB5398F555F5D91957CBCE39A6C04B9AA532A3A6FBB9F2D2E17AA4F64D3936BB0C6075C58B00D9359127E5782F302B095FB7E7E7B1795A5936F4CB8EDB0C9E76FDBB3F7F932CAB2CFDBD583A8E1B9068B0B4A92AE840BCAF2170BBF023D58579BCBCDF66119674FE2BA0501CCF1D3DD35CDE5F5C6FC6C47AB8CAAFB7532EB56FD4CD665ED32B500EDD7AFEA63C531AA7C3E016F5E72ABD501AD7CB5A2C2CCB68EB12FB631EC2232AEC1624F64DF1448EA516CB4F2F8D06FDAD9E5D1722D16B4D2EFCA389ABB1915231927DB76F022D96805E1D591A75D5D7A30E7B2C31B20C1357CF5DD5CD4F0EDCFFBB0823F44D9D9ECEB3AF9B624F385B8474A8D56164ACD7B905AA0F9C7B6C40FFA4561756893A839991E5CD16007EB43FDFDC10009A1BECA49B849B258F681094D872567B9A53369937C2D3806A9D382537E8F2E8BB29A25B22884369B5BCF3AC93C8C1868DE87E53694E7EF96FCB925597EB64ED62FABB878F9C733536CB5E165292E5F8A4073F1BEBC6BB0C0979208DCDFB98683B271553655598C01144E81B8AFD281711C14CFF08AE76AD3944F96B9688787296602F2CD0E5BFB52195084568A954AD6A258EBCDA58CA85B81F6C92C6065764B97858BA6813058B08A6F4309ABC7655465F994E8EA7EB68A1CA37A7F91A482B476BFDA78AC2953207F3FD760757461D3B9026717B1D91E779D5317C3CD345BCCF436CB93957C7FC2FE6EE333417C250E3278411E23AA2864D2B8061BE9298EE1F263ADEE571BFFCD4D1AAF225110999F0FF69385FAE59345FBD1C02C4E0725ACFE7C98FB55FF92508DA17C8D28483DDF628EF11FD428931C15ED8F63C4C0EFA0B4E329F0DD241DCE076A24E5D8A7BB75A7E04D2ADB0B3FF426D061FDAD0086F12D16910EA565729EA429111F3B0A4D162E48F93A11BD451C73CD78759E77185DD78DA5DB7C9A2BC7B7C4FB5A89955E927D03CDAF7621A1C06A617EB6B0C497CBE4DB67F2AD78F4B9DA888F8DC5C609AD1E367F883FFBAA961047F30AFD7AB7AC2B9FDAFD22CE36CBE8E53A955E81F12D36D25FAA72E9A2B9F9D5EACEBAA201B8B5EE1ACCF1FDD6DCCA9FA524E2310A4DE638FFBDF0B2534D233BCBF916BB989B3F48BC7812D507F3BB85FE5867520879F3DB1E5BBC528A727FDE7106ADA36F5C8941B399C31E6CA9F110403EDDB095719784F7040860FA478395A04380B11D5E012EA23FB5A74465AEAF590EC684F34D7677B991FCC49BF9D9E2D4F614A7C2A654FFE4400FF08C5A6CB3A40CC0C8FE3E9D85B8F1EF7B64713A458587F73DCA6E765BFFBA3F8F9EAF3364BBBB634E5010C03A9604C70F0284B0F3FC9F81465C9DBE4FAE1D4E9793ABF2EB61D6E6E19CD9E79CF99E3C45CFB11878D1FD7AB8A9B37B0AEC3DAA92C5E9F608D821A6F2C70C421AECDDB3E79842DF318F8795CB2D139F218A22DE9E2BD82E40F1875EC5C8FADDA5B0C431D782EF0B450EA9EB2A707D8DF363BD9919EAD1EE50DBE3EE3C06DE470FEFC128502AC2215E268A781DD5619FF7893F96461CE2152153B259BEE1E29A26A0610F6BB92B55EECB4DE61C56887FAA73D54E2B38CAF72A6D22F571871E0C6165CE97FAADA9292F6BBEA6C5EA30BE29D407012C0DB10DC45A65CCFD2EFAE552127DBDDE52526CA4B1FD0AE8083717C9E3E399E0FFABF1DC378D10E54A84BFAB10FE6E87F012A7EFD29ABA4B9CB64B6BCA3EC48B2705C2AED9012936E4AED906E9E724AFFDC3085A160094D35341500FBB8F4B8A0B9F5B5087B2477A8BC34624E1BB4EE345BC8E960AB31101B130F5341D40ED96C6E9F976B58282D78536BB1BE3F2EBB36F513A27731039086265026BBA80210EC6B0CD83DEEA7DA9F7E7BC25D63E6F7A1104530DEAD8D567AD579BCBF927F24CC4D210CD8F3698FE58C0CF8ED9DF2DF045DFCBC5DDD672E65693D8688F177CCF2F358E11D6535270113FC79978DDC0B738A86B8A0CD1D2658BBD729610720D9351734CA9724F16575DD4DCC5DC423FC598ED3176349ADF4459F62D49E7D4BC125795DC6A8EB9F9F043943D0974722D87DDD9426CBBFAD77EA4B6C1E720B5F8A7C36CC93E0BFF81D90CAC7319341C284B010BF72A6C8BC5C6BED9A4C973B4AC6E91A5645840B32D6E48D6F996C37AB4598F4C05764F2BB2C5E8B226151F4F7F5576D4032B4A689B8C0408C5E0FD080187D4A9BA81F2FB8144C1BB3A985ECA8C96B150D0BAD4687356E2A6AC90110477D76C4575E5CDBB8DD60B1931D7B6C7CABAE4A1EF987806A98BEDAFFE7C98857A888A971B2CECA879B429D4C605C96669BC012A85420036B3B3A6369EB20704C42E040AC72E36DAF0FA963C53FD2CB3BAFBFD605FDAA92CDF11460C4A377D651F57D44F5DB12A52360CE556BB658004011EE2FF8E46167A8FB7A12D426781B7BA09ED694617A6E0F5369F25526649E6774B7C6000CA62B45C554DBD5ABABDC8B32FD56DB9AF9DEF6CE4B9631D191095553C3915C1B9F486C7A09BFBBB649BCE20956B739B0E74A5BC60B7BA5E9771AB6EDC2DEFDB01ECCA2BF842845A46FB9A832F51BA20901618281CD6577483DFD886219E0628A78BAEC77919A770F431FBBC5D2EDF1D3F464B319CDF4EB49D71E26C7646A960F7DB5350D531308DAA2C3363C46B928A20AD2EAE7F69FFCE9A1F0AFD152D48756FDF7D77377B22ABA89CDD6C13CD4A119B935FE2342BAA14460F111D4809727C4427EE39A62734AA775E323AC32705C0C9DD9FCBF365F500A901B88AD6F123C9F22FC957B27E77FCE6D5EB37C74767CB38CA8A6BACE5E3F1D1F7D5729DFD3C2B932047F49C52A50F7977FC94E79B9F4F4FB3B2C7EC6415CFD2244B1EF313BA9F9D46F3E494E2FAE9F4F5EB53325F9D8A9FD7688DB0BCFA97064B96CDB98001C67A685678E9A929CD467EDA7E23921037C27D4B1E8FB07DFDEDA9F8E15BC036280878771C177C2DB79E5F099DF6E2B1E74D941705C90B2852927A7C54C861F4B024AD2C9E2AD1337E57A617919D3F7F5CCFC9F777C7FFBBFCEAE7A38FFFEBBEFDF02F47E559FDE7A35747FF69DDFD97649E343DAF9FA3744677E5E3A3ABE8FB27B25EE44FEF8EFFF9158B334FB75A94D79BD433C66AD29BEB1EAFA8E94E43554525B15E115F6C4965A45758E7F4DF79BC226F8A9559DF35BD3BFE9FEEE40E809C3FA333D2E886C69242405C59A355A915EAB5B05F2AE196449977B13C7BC8F2349AE59ED1BE8F665F1774435DCF3D23BED9A69B24239EB1D61C0E2FA145BE54AAC9898BA436DF8296900C8E5EE8E9E59BEDC96E53EABEECB52B8DBC297ECCAED305B59BFE83A4881EEC230A374DFAB9E8F1D1450EB0E38B7E5EBB2FEDB8DA7CD78BA9658A3B7E15FFB755F4FDBFDBAE5D2EC55D0FAD603E5FE5E8273053BEF799EE3AB5EAE421D6C93830AB0FBF6724FDE85BED57688B7F7B46FC29CA724BB4467C280F6943203E7BA607BBF45332EB2FF132F25F93F72F0330F957522578F4CC8947AA36635B3618A1BE895E166934F73E79E593C175791EF78BB8CE2C292235D0E5BFAFE33FB7A4FCFE6C3E4F4996FDE5E86356FDFAF3D1173ABB856E67A8FBBB3D75B764111766664142F5CE2CEFA5648A4BEE38DFE6E436597A9FA3DD3DF5D486C405291F0FA413D8A17E545B025056B51391CCCF6654B1AC5E7A6E91B64271F9BD300EA2E5412876C0C0ACA6AC79B07298B1E92CE3A2724A71FB70365FC5EB5E5B58764B36499AFF2326DF3AD3C80553607DD2E6F585CA069849A736D709F00D76E9A39768A9373BC1163EEF25DFDD20EC6868BEEBB7B8F8CA079E97451B23E32EC85C688CB3DDC505C4F470871B2F08F775304157B31F65820B1F84D8F890D221D11C51FE664DDF0087DEF36594654D9904AF98BF9074E5FB844E4FBBD58674B37D58C6D953BFA54C37DF34EF7FF281E90C78A2C282880C1DF4657C89AA503CFA912A28C562770481D5413B809A52478E185C1F487CB0BC4410BE6FF743366EA3D7DE2833DD85C2EE7BFF14BA9A0E7694BD3EFA4F67BEBF761815278A430EEC4D8F81BD71B93112D78CDDE084CFFB5D5E714956FD9A7D42AE55D3ABB17D72BAC9D1642E5B852A4E6E3875AF57DD0C5D7612DC7EE8E7F0E2F3E46446656075E66B9FF8106567B3AF4D0DC95E461C35B76BD916C6743834FE0831548A0CC4FB6AFE1E4CD983297B30652513804B53F823DA7865462DB8CE92A12A2CD3BD58E93483FA2DE36A4261489622CF7DED5FC700DCB323504230213D68C13B5E0B5A0E2AB41AB418D79B3EE372D182AA5B24DB90FD3F8B7AEC67EB64FDB22A7DF4EED62F5712CCC3C52957B2CCAFE7765CEDDFE37E8753FE7289AEC306E0630308A06E7B6F09FD487651A69E768800EAD19FDAB724F6279723525DE1CDAB3783A9F5E6156F9BB6D62F56AAC7168532660AE719EA6463C5D94E978BC2ECA59A3069F47D5BEEBA3EABABEBEAEB92CE4E4B4037DBB66BB1CAFFEDE0836C3EEC67D6B689A19D2FFA21CEFCA4B9F3FFABFD9537458FDDCFF722F58D2E3CC12178544C356EED61E01160336C430D939CDC8D9A16411F6ACECBFE0688DF78FF308CDBFB823C4654E50E40715187648027D11FB39B345E4569AF93CAEE3A7F6A29051271EE6C9497A7C9E053657A387EB689413DCB6FFF676DE18F9DBFCDA0A455A3DF2623B95B5D6F927D185C963804217341D18567F4A284CFD5691B21D27DDBCB5CAB2CA0F324A5229CF7190D1703929268E94F01F7B9851C702D7062107EEDF48EC25002DED7D48EE2E256C765D8D1EDE278C116B855E856BF7509A92927CE490C71F1EF74F56EDDAD854C50324E8F5D96CBE4DB67F2ADC82BB1DA58A0B232F7C06CCE076B6F02BB159F8CBAC7D6DBA6A1EEF3848BC93EED8EE6B72658EB8C6E9A9E4DDF7F2F6E9AE8944017441E623FFF20F1E229B7D8F38105BDCEBE797F2031CE25511901EAFE1AA846E1E5B6C7D8A470BD0B113E9F4C6CE7E1559C124BB80571D32435AA8AF939AC076C211CDE30F77D9856E4299DE5833C4D2BAEC923CB642F46AECDA738F5BD83B5B422C953FA13DC1FB1C54BB5BDF3520EE021F7E10A1CC649D91A109E5DAAED737DCF78BD99589ECE2B560B65BFCE778783D93007B3F7E4297A8EE540915EEB65E7C2DFCA520A3D429FA71EC6B6CF91C21AA7AF10D3AAF3B54E3B7A78E41860FB4C329E1E460E17B1EB3BB8782F54A06B00F0410D8EA7060F8A4B13E4B9EB51B3762BB9FF9BD63DCCCE727830FADADA369AF41BD11E77DEA3A72C99A2A1153C8D8AD375DA1E5E394CDF9084AC72552534CB439C0217D4D82A06759DB35E31D7AAA26CD31ADD6B2FA3EBA1B8871EE01BDD00AD8C233F393F269EBE631F6D1D4B5F8EB56914D2D6417819D41AF296F5A259527DC3E7A66813EDEEA38EF21438BDE8FAA9E46A1B5D9DD9872BEF84DE32A4DB4561358FEAFADC0642383C24A3AA979AF7E88D4DA19E49EFDC93EFE949EBEBF536A74758F2F878E60FD5EF5E505DFAA2E9D217451FE2C5936F647E86F839C9EBABDF3EE8A676BEB44ED977D8D80E1BDBDE6C6CD769BC88D7D1D29B45FE8B576C25A2F3ED6AC5ED4373BAB65785FF8E59E5AFFF7E7C74378B0ACC6F1CBAD9CECBAECEBE459497739BDE7E7271C086EB6C67D56D2BE1A5F3764FA2C98AFF7A36D50EE92B706A2EE79FC833F11D7DFCC702CFC2D2EF654DF4BD540C4D5CFF50EAAEE947C87EE4BD1F1F01C125A117F1739C25BD421F5B157F9EF89687469FF7C56C6E8992D9368D7FACE27511D5BE59F62D49E7D406EAD6868B2834883E44D99372C2FEFACAF971DCCE6DB71735C63DD96847AE693F48AAA0668A8A1B6DDF46C4669326CFD1B20A438ABD67C4ABF0EF63EED33E7EF28B38A312795875BE565DC7CFDE4BC4A240E162EFDE86795A51D34D38D54E9AC7375B9C2014F8BCBBE41795F3F1365A2F7C230F6F6F94EC3A3C158BDA00BFC353310CCDD93CDA142BFE8264B334DE0C51789A4ECD7334701F744B1A107B764B9EA9BEEDC7E99D3DBD54CAD139B06A82CA84D58FAC35E3B496E9F787E8D7FEF2E578F3374DE9BADEE6B3C47F5DE664E12B55E1599625B3B8DC30F9E3C5FD5969B615529D914C9891CBF5FCE8365976D00D257764F978D2FE7645E732DE2CE319EDF8DDF1EB637116AFD7958C1D557D1589B7B3A27AB73C93B4438C828E4E8E08F6679E8EFF21A1A75244D26292A3E57952D80811950759E4E2F52CDE444B61E4029CA17016836A318A2D176443D685D4C94334E98D3920CA9DB6B8050EEB78F0F6941115B50455BEC87BBA0F17970C292A3B151C376BCD4F4124E7A64AF0D3D2C91122B6F114BD3A3979ED478CEA119BCCAB2222D95C96C481F5ED389C3C5D7E2FF47124DE524E4E9E5A3A0179EADAF64C9EE0C999A63C714F04DA406C54AC80F2C8ECCC42CDA2B88933DB21E79F2BB078859641F6305DE56764FE145156203C1E670582ABDF8F9A8BA6E22908D2B3FEED4780F18DB402127CA33EAC801F6C05A8EABCEDCB0A604AE2D505BCC6927FB6322B2FFF5CCB41FE9B59B6907FB4EA2DD2B3AECAE13E4A3F98F09791FEAA9993F8FAA72066F5C80B04600E32AFC1855511BF1E5C8A8AAC42F77092B5DE5369AC3ECBD44628DAAA75A88357205D63908569B21A58CE3CB57F9C09AEBDBBFC4307F3E587375F7E9835C096B46BC3D94B92F0BB0AAE0C1E2BA97C83C106D1E1EC2AC4B208995F07117BBCA01F32613D0511A9838B74063C4F184946AAEBC47B3476ACA76C0C64F0B675F3783A9A1FF742A2E0DA80585F5CCCCF68FAA60E14D83D71EA62C04452DADFF744A89068B7DD90ABC651A7DEC12CA772481D25BB6CD99F071329AB29F6A0A6CCBDA7EA728AA1CE06A4ABE2E4D1B7A33E02307D4A8700AE6D0F9C34EC7876C44DC38A045CD048BAC616DE47B2B7D86DD32EC9C88DA29C1332715E2EC85D24465F482D98FD5C5D11EA64A6C7F4AAA466AC5D669039B3D60D43ED52FEBC1F618F7126F691C5C9698A8267752C1AD88B85D47ADE0DC9B41D51887D7863EFB2E26AA2714A75B3632E2BBCBA1B3259A3B8AC98FC1EA3C988859BC1493C86BAA6DDC06753EEF7BD102DB4FE1AD6DDF86E8676354CFDB27F1CE514EEFC68A79646B6BCD8AA4153171CAEC81B4B05DFB0E3E28397B2DB0109AA33216904A92DF7094894D4666ED18F291F22D926132725CB0E779CB41632FFB406164DA3E02527093215492972496E1C2A70C97ABAA71164E024CEC61148D31863680D3D722C91D30AF3B710F63B9868479779E8258018E8EE4794A91A25762791D1A47022E23191E7827CF54749620E8FA5FA8AE6E1B9A07605D4D6BEF6D5E09007B9D196C02E1EE5AC647A77CF72AD80F2297D3CFAABA72B96D62EE9D062A5A9F13B8A9C8437F546938F118D3D2B3199C65EC7BD8E9FC07687BE90971B0F9B9EAE0A68307AC710D6E02A6D6CD11C4FB1594BD8F8BAAD29190954C9F31D62A08804DD00315ADD8F7B12216011C605973D1C574082848D8F20080103C56D4460EC08F166DA836E1F63CCFF38DB85952C8CBF4D74C142771B6E9BC3E314ADC5421C97A74C2163A6921A49B676D10EE98A7C067AA0D4151585A4622FF61BAC702AD2DDC85B0EA46328E9BBA261C24BD2B8DA650737AFB2A2A558E9709CDC1055714D506EEBA6BD787EAD2A268AF43895541188D4D4E7C6719E678C2435A33DD4B0979AB15F6B0852D3FE69B08BD94EEE5E85DB3B4C79D0B0FB3F1625611349F015F6503E6A3AAFB18C1CDB345BE39B3972284E909354EFE0A69D3B5739C63C8D7CBC02C4A3BEE0EA2E64F15DAAF72C6B4C9C4314DB94EEDC0159B11E2BA68DFA6098A4926D5651712E1E2F04D4F9D6F5102C3FE005EF8F120E0A290CF08623580E5A9B9BA4C31218E0D26ADC318D24F3986F3474E5140B67F441F887F17BFF98F2AF4892E52B37D8205EAFA99C62772CCDD8E84B7E6449AF3308864D9C3815591D2B7DA27FA11B4B8A82FA71A72236E378741D8566FC6D76320F4D11FFDCC139D7FF4C75702868A57EB45A7C07A99F8AD4FF0045F83AA90F166E3F866C870DB9B7B8F79840D0FD880F96C79287C04F9577F29DF2588F94C79289119F65EC52DCBCF00E37C8B6D1E359F3CE6D202EEF9CC77F01CC8944F38CD9CC55554343AEAAB669974445A03D98936A9007F241E4A7EAFE1EE4182B2F25182F27F54F413CF6CEF2EAEFC2AB1E6F3099F22CCBE164E9C39F7779F4F8387559AA88049DF465C39EC9113027D394A23B32DBA694E7F710A3DCC44861F0D69DF1E66EFBE3BEC8403BA21D98FF5A8B68AB944D458BD85EF6EDAE1479BE020C204B5784E4F17A7196E7454FE4BEF987FE0AD7666F50291881020EA7D436ECC5AD0FFD6F2C2BE2D88C1CBF35F064C4A5FE1B9596A61D98D4DD921170A0C82C8D2018F537A3C9C50599C545E130BF0231C8B6D490CA91D0FDB8EBD2D38E6427C426CE66DB6C5704A72596171DE6E79D179E6E2CBB203EE7C9C2AA185C090FBE46175AC2C45832D4CBC40C5B108E1F6F00C9E2C664D45FE85C6F97F49BA2F4D23AA75F90B415B139F9254EB3FC22CAA38708B84B28BEBA237963A1950251DE521E556DCCC4B28D77B327B28ADE1DCF1F123AF9D1C352F858B285F87E5AD52775D2B6403DB48D66E83BA30AEBA6835074D70169BAE5CF10529F7C33D4210FA1EBAD3E0DCBFDD40D600F759BD9482E48998721C507D34128C6D30199757BF93D27E93A5AE2DD76108A6E3B20B36E5B0F33D66B0BA0E8B485D1F429DD97489D4A1050AF1290A65BB437552786B8D95B64093FDB08F5C1B6EBC6008480C9E30180C0B10170BA71F20F30E5A1F2EDE06879106D875C6606A043AE1DEE9003B1E9B02A4EA6EEB482D1765C81693A67021BA44E9936A833A659D3099FF549EA876F86BAE221B4BD35AFB5809E9A26B897A6D5A4074C20D946B41733512C86DDDA7620DBDA568C6B2D80811012F6E91028821C0426801C90E9265E3B77F16DBC06506EE4358C56732AA5916F86B5A7853472F1646067CA39E400F4BB9C4A71F1CDC8FE66AEB6E4928ACA1E71A5254399F4AC5881423BDAA7F13A94F300807DEA772219CA6497877559D784EEF026BA8C7BDE8AEEAE783F2C80E95E53E7E4C2379C1A40B9EBD430BAE1B5B78FF2D8DA2670606DABA687CE1D28F5D035413D74ADBA1E18AF91DC07D308F6C2B46BCDD58552330AEDB0F1BAB0D08DBCF302EE4FA91D79087D6F8A05CCB5227D991BCD252CBCA49836BC1B683931BE08E9F07DDFB9038A5444470C2C7F061700455709E0436DC7867A04B8EF64A746F9A9C25D71CA0FCC60D0F5A57677D295872B82E004F327FB9258F8DC0E5D6D8A67F6EA73CD41DC79C0DD191B1D700B32D080456F013B60D405603F606E97EC1EEF01E34620F181E007DA722CFA232A870DB40B4A44EA8DDE0F4B1250F611C87D64099BD9F16C9B3F610C01E0C2B103F027D4EC50B909FAB2A33981E9D80107D90281EF2C0BC0D3DD1486CDD59ED78D1D2F54EF6328380EF6B824E301CF407DA5A1AB45AE1508A46CF95E2C11D691D4E6A5AEB30C038C5181E3C381DC5DE568546E2C0E83E4952B3FC71D6E2E8CA82B46B014410C90C1861CB8E8BBABBF462C62B7F9072E8191A9D75D177B1FBC741A6A1160879C7E2CE892976A38806439ED4BBE3CF7B2D1A0F097BA2945D2F927359B240039C4364964BF6BABFC88C299DA7FF8ADCB543F7E244E5511C32B1F10100FED880CE9844BC50B006A9001045F0EAD7AD7280209CA93060FBFFE3756FBBF0A5C2101C0054335FD8A8B83C0FB3F73AFA0D90811C821870FDC99B408FC6D845DD1247C1790607CEBFFE1679ABD5E518C1402F33D58E882AAFC5679F1D473C875D086E1C87968E550A088926634AA3091D159A23B23A3B09E87816290CEC7DA2BC29E32A23C1D2390A10EC7A32C1A4601691882269B705772E30DDFC8E38C40069507D052525F6AF76549ADD5548E67F5078194E908ACE1330E6919A34850D4D70C9A043BF4CA43978FC7AFF608C602EEDEC570BD28BF09B364D0FB1A6DB88A0736698505811E4E5E42B2A3AD157E2695AD87CF6238F8B00732E0508F8618F56083C21F27807877C4851AA252E075B5E3BD09FAD0C355543D571EB3D535D2FD0D3FF8B5BD46BA3535BCBD789AA5403F6EC0C3CDFBB6A8446D38EB5BA96AB5F7390FC400B862B28A0D06359687BA67E42232799EC27196DE18D3140536670C58467828076C78C630756FF53CC18AE4FA194C689F740E176AD5462C0CBE59068B4FC00B91024C30AC5AEA61EFE8EDC1F0C30AA0E8A6195B74D53AF7C25DA3ABAF68C72AB924E3C04C0A79EC52D5DC33631356CF6DB818A9810D764D493643AEE085BE060F371DD6AC33A8D865CA226595AA01020926A2BC85B24F56DC0213F07283F4137C321156294D1FF35A48DEEC9F31D862781933E64D0CB2AF7BDDD4A1622E266C0819E01E960D1A17215E02C4979330DC50B5F74A235C2A056681DE5B1AE66220C4B0918204F0BD88B674810771EF71C9E461F842F27D0336A8D2F543D682908386B51690CC3223B247C82D8F3F0144729CFB7A0168CF34F7C136C9CFF1C182E9D1FD0D96CB2EC55B9350D628075527E4E786B49C2A85B787A18A4FE42BC5863D7F779E4B457CB52A49B5CFB9B43D1B380C16CD918C3FC9D6E453EE2F961C06245BDB1DF3BE1B4DC1D69F1DED0B723D379054961E9EA887658194EE1618BB3A25AE97418B692ACA0FD114140EC394D3B34203D52471F53354295F4635583C1386C3A91D49270A1DD84D328F0AD61A9088A336DA14F935641C90A1AFCC998133A2484F5AE068935EB66D6F4FAB3C16F50FF44F8A3A5A90F24A292B7F7D7B7ABBA55FAF48F5579319A1F982E25C9332414487B481F9B87E4C9A8C9F02450D48D3DCEA9C3C9A47797496E6F163342B9C47337A222EA5E61FD1724B412E570F45ECCCF536DF6CF3E2C0BC7A5872CC287286AAFA7F7B2AD1FCF67A53FC95F918022533A64320D7EBF7DB78396FE9FE255A8AB62486A24846FA2BA1BF5773593843C9E2A5C5F439591B22AAD9D7E650FD42569B2545965DAFEFA267E242DBEF19F94416D1ACB0D49FE37991220343A29F089EED6F2FE2689146ABACC6D17D4FFFA4323C5F7DFFD7FF0FB0CD77EC5E090400 , N'6.1.3-40302')
END

