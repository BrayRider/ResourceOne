 
CREATE TABLE [dbo].[Lubrizol_Employees] (
    [EmployeeID]            NCHAR (8)     NOT NULL,
    [Initials]              NVARCHAR (30) NOT NULL,
    [Name]                  NVARCHAR (50) NOT NULL,
    [ReportingLocation]     NVARCHAR (4)  NULL,
    [ReportingLocationName] NVARCHAR (30) NULL,
    [PhysicalLocation]      NVARCHAR (4)  NULL,
    [PhysicalLocationName]  NVARCHAR (30) NULL,
    [EmployeeStatus]        NCHAR (1)     NULL,
    [EmployeeStatusDesc]    NVARCHAR (40) NULL,
    [Department]            NVARCHAR (10) NULL,
    [DepartmentName]        NVARCHAR (40) NULL,
    [EmployeeClassDesc]     NVARCHAR (20) NULL,
    [Division]              NVARCHAR (40) NULL,
    [LegalEntity]           NVARCHAR (4)  NULL,
    [Company]               NVARCHAR (30) NULL,
    [JobDescr]              NVARCHAR (40) NULL,
    [SupervisorID]          NCHAR (8)     NULL,
    [SupervisorInitials]    NVARCHAR (30) NULL,
    [SupervisorName]        NVARCHAR (50) NULL,
    [FirstName]             NVARCHAR (40) NULL,
    [MiddleName]            NVARCHAR (40) NULL,
    [LastName]              NVARCHAR (40) NULL,
    [Country]               NVARCHAR (40) NULL,
    [LastLoadDate]          DATETIME      NULL,
    [LastUpdated]           DATETIME      CONSTRAINT [DF_Lubrizol_Employees_LastUpdated] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Lubrizol_Employees] PRIMARY KEY CLUSTERED ([EmployeeID] ASC)
);


 
GO
 


CREATE UNIQUE INDEX [IX_Lubrizol_Employees_EmployeeID] ON [dbo].[Lubrizol_Employees] ([EmployeeID])
