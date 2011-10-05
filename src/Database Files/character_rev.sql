/****** Object:  Table [dbo].[character_rev]    Script Date: 08/08/2011 17:03:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[character_rev](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[charname] [varchar](50) NULL,
	[revxsec] [tinyint] NULL,
	[revysec] [tinyint] NULL,
	[revx] [int] NULL,
	[revy] [int] NULL,
	[revz] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF