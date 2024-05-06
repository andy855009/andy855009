USE [FlowDB]
GO

/****** Object:  Table [dbo].[zSYS_SIRTEC_G_MPR_POMember]    Script Date: 2024/5/31 ¤U¤È 03:05:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[zSYS_SIRTEC_G_MPR_POMember](
	[AutoCounter] [bigint] IDENTITY(1,1) NOT NULL,
	[Company_code] [nvarchar](10) NULL,
	[Category] [nvarchar](50) NULL,
	[DeptID] [nvarchar](50) NULL,
	[DeptName] [nvarchar](50) NULL,
	[Account] [nvarchar](50) NULL,
	[MemberName] [nvarchar](50) NULL,
	[Combi_ID] [nvarchar](50) NULL,
	[Combi_Name] [nvarchar](50) NULL,
	[Enabled] [int] NULL,
	[WhoCreated] [nvarchar](50) NULL,
	[WhenCreated] [datetime] NULL,
	[WhoChanged] [nvarchar](50) NULL,
	[WhenChanged] [datetime] NULL,
 CONSTRAINT [PK_SIRTEC_G_PL_POMember] PRIMARY KEY CLUSTERED 
(
	[AutoCounter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


