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
ALTER TABLE dbo.TRA_CAF_PROGRESS
	DROP CONSTRAINT FK_TRA_CAF_PROGRESS_TRA_CAF
GO
ALTER TABLE dbo.TRA_CAF SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.TRA_CAF', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.TRA_CAF', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.TRA_CAF', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_TRA_CAF_PROGRESS
	(
	TRA_CAF_PROGRESS_ID bigint NOT NULL IDENTITY (1, 1),
	TRA_CAF_ID bigint NOT NULL,
	STATUS_ID int NULL,
	PROGRESS_DATE date NULL,
	REMARK text NULL,
	ESTIMATION datetime NULL,
	ACTUAL datetime NULL,
	CREATED_BY nvarchar(50) NOT NULL,
	CREATED_DATE datetime NOT NULL,
	MODIFIED_BY nvarchar(50) NULL,
	MODIFIED_DATE datetime NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_TRA_CAF_PROGRESS SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_TRA_CAF_PROGRESS ON
GO
IF EXISTS(SELECT * FROM dbo.TRA_CAF_PROGRESS)
	 EXEC('INSERT INTO dbo.Tmp_TRA_CAF_PROGRESS (TRA_CAF_PROGRESS_ID, TRA_CAF_ID, STATUS_ID, PROGRESS_DATE, REMARK, ESTIMATION, ACTUAL, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE)
		SELECT TRA_CAF_PROGRESS_ID, TRA_CAF_ID, STATUS_ID, PROGRESS_DATE, REMARK, CONVERT(datetime, ESTIMATION), CONVERT(datetime, ACTUAL), CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE FROM dbo.TRA_CAF_PROGRESS WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_TRA_CAF_PROGRESS OFF
GO
DROP TABLE dbo.TRA_CAF_PROGRESS
GO
EXECUTE sp_rename N'dbo.Tmp_TRA_CAF_PROGRESS', N'TRA_CAF_PROGRESS', 'OBJECT' 
GO
ALTER TABLE dbo.TRA_CAF_PROGRESS ADD CONSTRAINT
	PK_TRA_CAF_PROGRESS PRIMARY KEY CLUSTERED 
	(
	TRA_CAF_PROGRESS_ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.TRA_CAF_PROGRESS ADD CONSTRAINT
	FK_TRA_CAF_PROGRESS_TRA_CAF FOREIGN KEY
	(
	TRA_CAF_ID
	) REFERENCES dbo.TRA_CAF
	(
	TRA_CAF_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT