
/****** Object:  StoredProcedure [dbo].[AddOrUpdateCalendars]    Script Date: 05/05/2022 17:20:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE   PROCEDURE [dbo].[AddOrUpdateCalendars]
@uuidHash nvarchar(max),
@dueDate datetime,
@dueDateHash nvarchar(max),
@dateCreated dateTime,
@locale nvarchar(max)
AS

BEGIN

SET NOCOUNT ON
IF(SELECT COUNT(CalendarId) FROM Calendar WHERE DueDateHash = @dueDateHash AND Locale = @locale) > 0
BEGIN
--UPDATE
BEGIN TRANSACTION T1;
	UPDATE Calendar
	SET UuidHash = @uuidHash,
	DueDate= @dueDate,
	DateCreated= @dateCreated
	WHERE DueDateHash = @dueDateHash AND Locale = @locale;
COMMIT TRANSACTION T1;
END
ELSE
BEGIN	
	DECLARE @calenderId as NVARCHAR(Max);
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
/****** Object:  StoredProcedure [dbo].[AddOrUpdateEvents]    Script Date: 05/05/2022 17:20:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE   PROCEDURE [dbo].[AddOrUpdateEvents] 
@contentId nvarchar(max),
@lastCreated DateTime,
@title nvarchar(max),
@description nvarchar(max),
@period nvarchar(max),
@number int,
@url nvarchar(max),
@type nvarchar(max),
@locale nvarchar(max)
AS

BEGIN

SET NOCOUNT ON
IF(SELECT COUNT(EventId) FROM Events WHERE ContentId = @contentId AND Locale = @locale) > 0
BEGIN
--UPDATE
BEGIN TRANSACTION T1;
	UPDATE Events
	SET LastCreated = @lastCreated,
	Title= @title,
	Description = @description,
	Period= @period,
	Number = @number,
	URL =@url,
	Type = @type
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
		LastCreated,
		Title,
		Description,
		Period,
		Number,
		URL,
		Locale,
		Type
	)
	VALUES
	(	
		NEWID(),
		@contentId,
		@lastCreated,
		@title,
		@description,
		@period,
		@number,
		@url,
		@locale,
		@type
		);
	COMMIT TRANSACTION T2;
END


END;
GO
/****** Object:  StoredProcedure [dbo].[AddOrUpdateUserCalendar]    Script Date: 05/05/2022 17:20:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE   PROCEDURE [dbo].[AddOrUpdateUserCalendar] 
@uuidHash nvarchar(max),
@dueDateHash nvarchar(max),
@calenderId nvarchar(max),
@locale nvarchar(max)
AS

BEGIN

SET NOCOUNT ON
IF(SELECT COUNT(UserCalendarId) FROM UserCalendar WHERE UuidHash = @uuidHash AND DueDateHash = @dueDateHash AND Locale = @locale) > 0
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
/****** Object:  StoredProcedure [dbo].[DeleteCalendar]    Script Date: 05/05/2022 17:20:15 ******/
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
/****** Object:  StoredProcedure [dbo].[DeleteEvent]    Script Date: 05/05/2022 17:20:15 ******/
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
/****** Object:  StoredProcedure [dbo].[GetAllCalendars]    Script Date: 05/05/2022 17:20:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[GetAllCalendars] @locale nvarchar(max)
AS

BEGIN

IF (@locale IS NULL OR @locale = '')
BEGIN
SELECT * FROM Calendar
END
ELSE 
BEGIN
SELECT * FROM Calendar WHERE Locale = @locale
END


END;
GO
/****** Object:  StoredProcedure [dbo].[GetCalendars]    Script Date: 05/05/2022 17:20:15 ******/
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
/****** Object:  StoredProcedure [dbo].[GetEvents]    Script Date: 05/05/2022 17:20:15 ******/
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
/****** Object:  StoredProcedure [dbo].[GetUserCalendars]    Script Date: 05/05/2022 17:20:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--exec GetUserCalendars 'en-us','e12b0498-c0a2-11ec-9d64-0242ac1200992',2,'DESC';
--SELECT * FROM Calendar
--ORDER BY DUEDATE desc
/*
SELECT * FROM Calendar WHERE Locale = 'en-US' and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = 'e12b0498-c0a2-11ec-9d64-0242ac120002')
ORDER BY DUEDATE ;
*/


CREATE PROCEDURE [dbo].[GetUserCalendars] @locale nvarchar(max), @uuidHash nvarchar(max),@limit int, @sorting nvarchar(max)
AS

BEGIN

If (@limit IS NULL OR @limit = 0)
BEGIN
--DECLARE @SQL VARCHAR(MAX);
--SET @SQL = 'SELECT * FROM Calendar WHERE Locale = '+@locale+' and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = '+@uuidHash+')'
--+'ORDER BY DUEDATE '+ @sorting;
--EXECUTE sp_executesql @SQL;
--SELECT @SQL;
SELECT * FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
--GROUP BY DueDate,DateCreated,Locale,CalendarId,UuidHash,DueDateHash
ORDER BY 

 CASE WHEN @sorting='ASC' THEN DueDate END ASC,
 CASE WHEN @sorting = 'DESC' THEN DueDate END DESC
END

If (@sorting IS NULL OR @sorting = '')
BEGIN
SELECT TOP (@limit) * FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
END

IF ((@sorting IS NULL OR @sorting = '')AND (@limit IS NULL OR @limit = 0) )
BEGIN
SELECT * FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
END

If (@sorting IS not NULL AND @limit IS not NULL)
BEGIN
SELECT TOP (@limit) * FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
ORDER BY 

 CASE WHEN @sorting='ASC' THEN DueDate END ASC,
 CASE WHEN @sorting = 'DESC' THEN DueDate END DESC
END

END;
GO
