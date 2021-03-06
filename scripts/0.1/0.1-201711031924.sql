
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[AUTO_GR](
	[AUTO_GR_ID] [int] NOT NULL,
	[PO_NUMBER] [varchar](20) NULL,
	[PO_DATE] [datetime] NULL,
	[CREATED_DATE] [datetime] NULL,
	[IS_POSTED] [bit] NULL,
 CONSTRAINT [PK_AUTO_GR] PRIMARY KEY CLUSTERED 
(
	[AUTO_GR_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



/****** Object:  Table [dbo].[AUTO_GR_DETAIL]    Script Date: 11/3/2017 7:24:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AUTO_GR_DETAIL](
	[AUTO_GR_DETAIL_ID] [int] NOT NULL,
	[AUTO_GR_ID] [int] NOT NULL,
	[LINE_ITEM] [int] NULL,
	[QTY_ITEM] [decimal](18, 2) NULL,
 CONSTRAINT [PK_AUTO_GR_DETAIL] PRIMARY KEY CLUSTERED 
(
	[AUTO_GR_DETAIL_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

