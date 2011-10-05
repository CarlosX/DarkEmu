/****** Object:  Table [dbo].[character_quests]    Script Date: 08/08/2011 17:03:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[character_quests](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[quest_owner] [int] NULL,
	[quest_id] [int] NULL,
	[quest_status] [tinyint] NULL,
	[quest_step] [tinyint] NULL,
	[quest_reward_id] [int] NULL
) ON [PRIMARY]
