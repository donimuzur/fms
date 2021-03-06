CREATE TABLE [dbo].[VEHICLE_REPORT_DATA](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[POLICE_NUMBER] [nvarchar](50) NULL,
	[MANUFACTURER] [nvarchar](50) NULL,
	[MODEL] [nvarchar](50) NULL,
	[SERIES] [nvarchar](50) NULL,
	[BODY_TYPE] [nvarchar](50) NULL,
	[EMPLOYEE_ID] [nvarchar](9) NULL,
	[EMPLOYEE_NAME] [nvarchar](100) NULL,
	[VEHICLE_TYPE] [nvarchar](50) NULL,
	[COST_CENTER] [char](10) NULL,
	[START_RENT] [date] NULL,
	[END_RENT] [date] NULL,
	[SUPPLY_METHOD] [nvarchar](50) NULL,
	[VENDOR] [nvarchar](100) NULL,
	[FUNCTION] [nvarchar](50) NULL,
	[REGIONAL] [nvarchar](50) NULL,
	[CITY] [nvarchar](50) NULL,
	[TRANSMISSION] [nvarchar](50) NULL,
	[FUEL_TYPE] [nvarchar](50) NULL,
	[BRANDING] [nvarchar](50) NULL,
	[COLOR] [nvarchar](50) NULL,
	[AIRBAG] [bit] NULL,
	[ABS] [bit] NULL,
	[CHASIS_NUMBER] [nvarchar](50) NULL,
	[ENGINE_NUMBER] [nvarchar](50) NULL,
	[VEHICLE_STATUS] [bit] NULL,
	[ASSET_NUMBER] [nvarchar](20) NULL,
	[CURRENT_LOCATION] [nvarchar](50) NULL,
	[TERMINATION_DATE] [date] NULL,
	[RESTITUTION] [bit] NULL,
	[MONTHLY_INSTALLMENT] [decimal](18,2) NULL,
	[VAT] [decimal](18,2) NULL,
	[TOTAL_MONTHLY_CHARGE] [decimal](18,2) NULL,
	[PO_NUMBER] [nvarchar](50) NULL,
	[PO_LINE] [nvarchar](50) NULL,
	[REPORT_MONTH] [int] NULL,
	[REPORT_YEAR] [int] NULL,
	[CREATED_DATE] [datetime] NOT NULL,
CONSTRAINT [PK_VEHICLE_REPORT_DATA] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO