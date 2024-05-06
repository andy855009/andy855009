USE [FlowDB]
GO

/****** Object:  Table [dbo].[FM7T_SIRTEC_G_MPR_D_POContent]    Script Date: 2024/5/31 ¤U¤È 03:01:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FM7T_SIRTEC_G_MPR_D_POContent](
	[AutoCounter] [bigint] NOT NULL,
	[SelectFlag] [nvarchar](1) NULL,
	[Num] [int] NULL,
	[RequisitionID] [nvarchar](50) NULL,
	[Product_Name] [nvarchar](100) NULL,
	[Product_Specification] [nvarchar](100) NULL,
	[Customer] [nvarchar](100) NULL,
	[ItemName] [nvarchar](100) NULL,
	[ItemSpec] [nvarchar](100) NULL,
	[ItemCount] [decimal](18, 2) NULL,
	[ItemPrice] [decimal](18, 2) NULL,
	[ItemAmount] [decimal](18, 2) NULL,
	[TaxRate] [decimal](18, 2) NULL,
	[TotalTax] [decimal](18, 2) NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[Comment] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


