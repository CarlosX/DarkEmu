/****** Object:  Table [dbo].[pets]    Script Date: 08/08/2011 17:04:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[pets](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[playerid] [int] NULL,
	[pet_type] [tinyint] NULL,
	[pet_name] [varchar](50) NULL,
	[pet_state] [tinyint] NULL,
	[pet_hp] [int] NULL CONSTRAINT [DF_pets_pet_hp]  DEFAULT ((0)),
	[pet_hgp] [int] NULL CONSTRAINT [DF_pets_pet_hgp]  DEFAULT ((0)),
	[pet_itemid] [int] NULL,
	[pet_slots] [tinyint] NULL CONSTRAINT [DF_pets_pet_slots]  DEFAULT ((28)),
	[pet_time] [datetime] NULL,
	[pet_active] [tinyint] NULL CONSTRAINT [DF_pets_pet_active]  DEFAULT ((0)),
	[pet_unique] [int] NULL,
	[pet_check] [varchar](50) NULL CONSTRAINT [DF_pets_pet_check]  DEFAULT ((0)),
	[pet_level] [tinyint] NULL CONSTRAINT [DF_pets_pet_level]  DEFAULT ((1)),
	[pet_experience] [bigint] NULL CONSTRAINT [DF_pets_pet_experience]  DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF