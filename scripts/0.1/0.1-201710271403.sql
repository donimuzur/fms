/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_MST_MODUL
	(
	MST_MODUL_ID int NOT NULL IDENTITY (1, 1),
	MODUL_NAME nvarchar(50) NULL,
	MODUL_URL nvarchar(100) NULL,
	MENU_NAME nvarchar(50) NULL,
	PARENT_MODUL_ID int NULL,
	CREATED_BY nvarchar(50) NOT NULL,
	CREATED_DATE datetime NOT NULL,
	MODIFIED_BY nvarchar(50) NULL,
	MODIFIED_DATE datetime NULL,
	IS_ACTIVE bit NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_MST_MODUL SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_MST_MODUL ON
GO
IF EXISTS(SELECT * FROM dbo.MST_MODUL)
	 EXEC('INSERT INTO dbo.Tmp_MST_MODUL (MST_MODUL_ID, MODUL_NAME, MODUL_URL, MENU_NAME, PARENT_MODUL_ID, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, IS_ACTIVE)
		SELECT MST_MODUL_ID, MODUL_NAME, MODUL_URL, MENU_NAME, PARENT_MODUL_ID, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, IS_ACTIVE FROM dbo.MST_MODUL WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_MST_MODUL OFF
GO
ALTER TABLE dbo.MST_MODUL
	DROP CONSTRAINT FK_MST_MODUL_MST_MODUL
GO
ALTER TABLE dbo.TRA_WORKFLOW_HISTORY
	DROP CONSTRAINT FK_TRA_WORKFLOW_HISTORY_MST_MODUL
GO
ALTER TABLE dbo.TRA_CHANGES_HISTORY
	DROP CONSTRAINT FK_TRA_CHANGES_HISTORY_MST_MODUL
GO
DROP TABLE dbo.MST_MODUL
GO
EXECUTE sp_rename N'dbo.Tmp_MST_MODUL', N'MST_MODUL', 'OBJECT' 
GO
ALTER TABLE dbo.MST_MODUL ADD CONSTRAINT
	PK_MST_MODUL PRIMARY KEY CLUSTERED 
	(
	MST_MODUL_ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.MST_MODUL ADD CONSTRAINT
	FK_MST_MODUL_MST_MODUL FOREIGN KEY
	(
	PARENT_MODUL_ID
	) REFERENCES dbo.MST_MODUL
	(
	MST_MODUL_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.TRA_CHANGES_HISTORY ADD CONSTRAINT
	FK_TRA_CHANGES_HISTORY_MST_MODUL FOREIGN KEY
	(
	MODUL_ID
	) REFERENCES dbo.MST_MODUL
	(
	MST_MODUL_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.TRA_CHANGES_HISTORY SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.TRA_WORKFLOW_HISTORY ADD CONSTRAINT
	FK_TRA_WORKFLOW_HISTORY_MST_MODUL FOREIGN KEY
	(
	MODUL_ID
	) REFERENCES dbo.MST_MODUL
	(
	MST_MODUL_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.TRA_WORKFLOW_HISTORY SET (LOCK_ESCALATION = TABLE)
GO
COMMIT