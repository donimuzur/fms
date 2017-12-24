USE [FMS]
GO

/****** Object:  Table [dbo].[ROLE_CONFIG]    Script Date: 28/10/2017 19:49:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ROLE_CONFIG](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ROLE_NAME] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ROLE_CONFIG] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO