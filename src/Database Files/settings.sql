/****** Object:  Table [dbo].[settings]    Script Date: 08/08/2011 17:04:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[settings](
	[setname] [varchar](64) NOT NULL,
	[setvalue] [varchar](2048) NULL,
 CONSTRAINT [PK_settings] PRIMARY KEY CLUSTERED 
(
	[setname] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF