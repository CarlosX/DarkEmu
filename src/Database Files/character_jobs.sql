/****** Object:  Table [dbo].[character_jobs]    Script Date: 08/08/2011 17:03:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[character_jobs](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[character_name] [varchar](50) NULL,
	[job_alias] [varchar](50) NULL CONSTRAINT [DF_character_jobs_job_alias]  DEFAULT ((0)),
	[job_type] [tinyint] NULL,
	[job_experience] [int] NULL CONSTRAINT [DF_character_jobs_job_experience]  DEFAULT ((0)),
	[job_rank] [tinyint] NULL CONSTRAINT [DF_character_jobs_job_rank]  DEFAULT ((0)),
	[job_state] [int] NULL CONSTRAINT [DF_character_jobs_job_state]  DEFAULT ((0)),
	[job_level] [tinyint] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF