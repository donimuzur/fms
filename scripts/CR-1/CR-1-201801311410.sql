ALTER TABLE TRA_CSF ADD DATE_SEND_VENDOR DATETIME NULL
GO

ALTER TABLE TRA_CRF ADD DATE_SEND_VENDOR DATETIME NULL
GO

ALTER TABLE TRA_CTF ADD DATE_SEND_VENDOR DATETIME NULL
GO

ALTER TABLE TRA_TEMPORARY ADD DATE_SEND_VENDOR DATETIME NULL
GO

ALTER TABLE MST_FLEET ADD SALES_CODE NVARCHAR(255) NULL
GO

ALTER TABLE MST_FLEET ADD DOCUMENT_NUMBER CHAR(15) NULL
GO

CREATE TABLE [dbo].[LOCATION_CHANGE](
	[LOCATION_CHANGE_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[EMPLOYEE_ID] [nvarchar](9) NULL,
	[FORMAL_NAME] [nvarchar](100) NULL,
	[BASETOWN_OLD] [nvarchar](255) NULL,
	[BASETOWN_NEW] [nvarchar](255) NULL,
	[ADDRESS_OLD] [nvarchar](1000) NULL,
	[ADDRESS_NEW] [nvarchar](1000) NULL,
	[CITY_OLD] [nvarchar](255) NULL,
	[CITY_NEW] [nvarchar](255) NULL,
	[CHANGE_DATE] [datetime] NULL,
	[DATE_SEND] [datetime] NULL,
 CONSTRAINT [PK_LOCATION_CHANGE] PRIMARY KEY CLUSTERED 
(
	[LOCATION_CHANGE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[FUNCTION_CHANGE](
	[FUNCTION_CHANGE_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[EMPLOYEE_ID] [nvarchar](9) NULL,
	[FORMAL_NAME] [nvarchar](100) NULL,
	[COST_CENTER] [nvarchar](255) NULL,
	[FUNCTION_OLD] [nvarchar](255) NULL,
	[FUNCTION_NEW] [nvarchar](255) NULL,
	[CHANGE_DATE] [datetime] NULL,
	[DATE_SEND] [datetime] NULL,
 CONSTRAINT [PK_FUNCTION_CHANGE] PRIMARY KEY CLUSTERED 
(
	[FUNCTION_CHANGE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[FLEET_CHANGE](
	[FLEET_CHANGE_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FLEET_ID] [bigint] NULL,
	[POLICE_NUMBER] [nvarchar](9) NULL,
	[CHASIS_NUMBER] [nvarchar](9) NULL,
	[EMPLOYEE_ID] [nvarchar](9) NULL,
	[EMPLOYEE_NAME] [nvarchar](100) NULL,
	[FIELD_NAME] [nvarchar](50) NULL,
	[DATE_BEFORE] [nvarchar](255) NULL,
	[DATE_AFTER] [nvarchar](255) NULL,
	[CHANGE_DATE] [datetime] NULL,
	[DATE_SEND] [datetime] NULL,
 CONSTRAINT [PK_FLEET_CHANGE] PRIMARY KEY CLUSTERED 
(
	[FLEET_CHANGE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO