

/****** Object:  StoredProcedure [dbo].[[GetCalendars]]    Script Date: 14/04/2022 11:17:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[GetCalendars] @locale nvarchar(max), @dueDateHash nvarchar(max),@limit int, @sorting nvarchar(max)
AS

BEGIN
If (@limit IS NULL OR @limit = '')
BEGIN
SELECT * FROM Calendar WHERE Locale = @locale and DueDateHash = @dueDateHash
ORDER BY 
CASE WHEN @sorting = 'ASC' THEN DueDate END ASC,
CASE WHEN @sorting = 'DESC' THEN DueDate END DESC
END

If (@sorting IS NULL OR @sorting = '')
BEGIN
SELECT TOP (@limit) * FROM Calendar WHERE Locale = @locale and DueDateHash = @dueDateHash
END

IF ((@sorting IS NULL OR @sorting = '')AND (@limit IS NULL OR @limit = '') )
BEGIN
SELECT * FROM Calendar WHERE Locale = @locale and DueDateHash = @dueDateHash
END


END;
GO



/****** Object:  StoredProcedure [dbo].[GetEvents]    Script Date: 14/04/2022 11:17:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[GetEvents] @locale nvarchar(max)
AS

BEGIN

IF (@locale IS NULL OR @locale = '')
BEGIN
SELECT * FROM Events
END
ELSE 
BEGIN
SELECT * FROM Events WHERE Locale = @locale
END


END;
GO




/****** Object:  StoredProcedure [dbo].[DeleteEvents]    Script Date: 14/04/2022 11:17:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[DeleteEvent] @contentId nvarchar(max)
AS

BEGIN

DELETE FROM Events
WHERE ContentId = @contentId

END;
GO




/****** Object:  StoredProcedure [dbo].[[AddOrUpdateEvents]]    Script Date: 14/04/2022 11:17:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[AddOrUpdateEvents] 
@contentId nvarchar(max),
@lastUpdated DateTime,
@title nvarchar(max),
@description nvarchar(max),
@period nvarchar(max),
@number int,
@url nvarchar(max),
@locale nvarchar(max)
AS

BEGIN

SET NOCOUNT ON
IF(SELECT COUNT(*) FROM Events WHERE ContentId = @contentId AND Locale = @locale) > 0
BEGIN
--UPDATE
BEGIN TRANSACTION T1;
	UPDATE Events
	SET LastUpdated = @lastUpdated,
	Title= @title,
	Description = @description,
	Period= @period,
	Number = @number,
	URL =@url
	WHERE ContentId = @contentId AND Locale = @locale;
COMMIT TRANSACTION T1;
END
ELSE
BEGIN
	BEGIN TRANSACTION T2;
	INSERT INTO Events
	(
		EventId,
		ContentId,
		LastUpdated,
		Title,
		Description,
		Period,
		Number,
		URL,
		Locale
	)
	VALUES
	(	
		NEWID(),
		@contentId,
		@lastUpdated,
		@title,
		@description,
		@period,
		@number,
		@url,
		@locale
		);
	COMMIT TRANSACTION T2;
END


END;
GO






/****** Object:  StoredProcedure [dbo].[AddOrUpdateCalendars]    Script Date: 15/04/2022 14:20:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[AddOrUpdateCalendars]
@uuidHash nvarchar(max),
@dueDate datetime,
@dueDateHash nvarchar(max),
@dateCreated dateTime,
@locale nvarchar(max)
AS

BEGIN

SET NOCOUNT ON
IF(SELECT COUNT(*) FROM Calendar WHERE DueDateHash = @dueDateHash AND Locale = @locale) > 0
BEGIN
--UPDATE
BEGIN TRANSACTION T1;
	UPDATE Calendar
	SET UuidHash = @uuidHash,
	DueDate= @dueDate,
	DateCreated= @dateCreated
	WHERE CalendarId = @dueDateHash AND Locale = @locale;
COMMIT TRANSACTION T1;
END
ELSE
BEGIN	
	DECLARE @calenderId as VARCHAR;
	SET @calenderId = NEWID();
	BEGIN TRANSACTION T2;
	INSERT INTO Calendar
	(
		CalendarId,
		UuidHash,
		DueDate,
		DueDateHash,
		DateCreated,
		Locale
	)
	VALUES
	(	
		@calenderId,
		@uuidHash,
		@dueDate,
		@dueDateHash,
		@dateCreated,
		@locale
		);
	COMMIT TRANSACTION T2;
	BEGIN TRANSACTION T3
		INSERT INTO UserCalendar
		(
		UserCalendarId,
		UuidHash,
		DueDateHash,
		CalendarId,Locale
		)
		VALUES(
		NEWID(),
		@uuidHash,
		@dueDateHash,
		@calenderId,
		@locale
		)
	COMMIT TRANSACTION T3;
END


END;
GO






/****** Object:  StoredProcedure [dbo].[[GetUserCalendars]]    Script Date: 15/04/2022 14:10:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[GetUserCalendars] @locale nvarchar(max), @uuidHash nvarchar(max),@limit int, @sorting nvarchar(max)
AS

BEGIN
If (@limit IS NULL OR @limit = '')
BEGIN
SELECT * FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
ORDER BY 
CASE WHEN @sorting = 'ASC' THEN DueDate END ASC,
CASE WHEN @sorting = 'DESC' THEN DueDate END DESC
END

If (@sorting IS NULL OR @sorting = '')
BEGIN
SELECT TOP (@limit) * FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
END

IF ((@sorting IS NULL OR @sorting = '')AND (@limit IS NULL OR @limit = '') )
BEGIN
SELECT * FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
END


END;
GO


/****** Object:  StoredProcedure [dbo].[[DeleteCalendar]]    Script Date: 15/04/2022 14:16:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[DeleteCalendar] @dueDateHash nvarchar(max), @locale nvarchar(max)
AS

BEGIN

BEGIN TRANSACTION T1
DELETE FROM Calendar
WHERE DueDateHash = @dueDateHash AND Locale = @locale;
COMMIT TRANSACTION T1;

BEGIN TRANSACTION T2
DELETE FROM UserCalendar
WHERE DueDateHash = @dueDateHash AND Locale = @locale;
COMMIT TRANSACTION T2;

END;
GO

/****** Object:  StoredProcedure [dbo].[[AddOrUpdateUserCalendar]]    Script Date: 15/04/2022 15:19:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[AddOrUpdateUserCalendar] 
@uuidHash nvarchar(max),
@dueDateHash nvarchar(max),
@calenderId nvarchar(max),
@locale nvarchar(max)
AS

BEGIN

SET NOCOUNT ON
IF(SELECT COUNT(*) FROM UserCalendar WHERE UuidHash = @uuidHash AND DueDateHash = @dueDateHash AND Locale = @locale) > 0
BEGIN
--UPDATE
BEGIN TRANSACTION T1;
	UPDATE UserCalendar
	SET UuidHash = @uuidHash,
	DueDateHash= @dueDateHash,
	CalendarId = @calenderId
	WHERE   UuidHash = @uuidHash AND DueDateHash = @dueDateHash AND Locale = @locale;
COMMIT TRANSACTION T1;
END
ELSE
BEGIN
	BEGIN TRANSACTION T2;
	INSERT INTO UserCalendar
	(
		UserCalendarId,
		UuidHash,
		DueDateHash,
		CalendarId,
		Locale
	)
	VALUES
	(	
		NEWID(),
		@uuidHash,
		@dueDateHash,
		@calenderId,
		@locale
		);
	COMMIT TRANSACTION T2;
END


END;
GO
