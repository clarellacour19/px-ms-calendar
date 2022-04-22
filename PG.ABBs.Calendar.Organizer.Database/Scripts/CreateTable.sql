
/****** Object:  Table [dbo].[Calendar]    Script Date: 20/04/2022 21:23:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Calendar](
	[CalendarId] [uniqueidentifier] NOT NULL,
	[UuidHash] [nvarchar](100) NULL,
	[DueDate] [datetime] NULL,
	[DueDateHash] [nvarchar](100) NULL,
	[DateCreated] [datetime] NULL,
	[Locale] [varchar](12) NULL,
 CONSTRAINT [pk_caldendar] PRIMARY KEY CLUSTERED 
(
	[CalendarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Events]    Script Date: 20/04/2022 21:23:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Events](
	[EventId] [uniqueidentifier] NOT NULL,
	[ContentId] [varchar](100) NULL,
	[LastCreated] [datetime] NULL,
	[Title] [varchar](64) NULL,
	[Description] [varchar](max) NULL,
	[Period] [varchar](16) NULL,
	[Number] [int] NULL,
	[URL] [varchar](256) NULL,
	[Locale] [varchar](12) NULL,
	[Type] [varchar](16) NULL,
 CONSTRAINT [pk_events] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserCalendar]    Script Date: 20/04/2022 21:23:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserCalendar](
	[UserCalendarId] [uniqueidentifier] NOT NULL,
	[UuidHash] [varchar](50) NULL,
	[DueDateHash] [nvarchar](256) NULL,
	[CalendarId] [nvarchar](256) NULL,
	[Locale] [varchar](12) NULL,
 CONSTRAINT [pk_UserCalendaer] PRIMARY KEY CLUSTERED 
(
	[UserCalendarId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
