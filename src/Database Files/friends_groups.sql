/****** Object:  Table [dbo].[friends_groups]    Script Date: 08/08/2011 17:03:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[friends_groups](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[playerid] [int] NULL,
	[groupname] [varchar](50) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF