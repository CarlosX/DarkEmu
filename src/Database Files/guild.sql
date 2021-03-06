/****** Object:  Table [dbo].[guild]    Script Date: 08/08/2011 17:03:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[guild](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[guild_name] [varchar](50) NULL,
	[guild_level] [tinyint] NULL,
	[guild_points] [int] NULL,
	[guild_news_t] [varchar](50) NULL,
	[guild_news_m] [varchar](50) NULL,
	[guild_members_t] [tinyint] NULL,
	[guild_storage_slots] [int] NULL CONSTRAINT [DF_guild_guild_storage_slots]  DEFAULT ((0)),
	[guild_war_gold_r] [int] NULL CONSTRAINT [DF_guild_guild_war_gold_r]  DEFAULT ((0)),
	[guild_master_id] [int] NULL,
	[guild_icon] [varchar](50) NULL,
	[guild_storage_gold] [bigint] NULL CONSTRAINT [DF_guild_guild_storage_gold]  DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF