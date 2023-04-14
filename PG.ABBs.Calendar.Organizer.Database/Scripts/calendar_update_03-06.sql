
/****** Object:  StoredProcedure [dbo].[AddOrUpdateCalendars]    Script Date: 02/06/2022 16:14:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








ALTER   PROCEDURE [dbo].[AddOrUpdateCalendars]
@uuidHash nvarchar(100),
@dueDate datetime,
@dueDateHash nvarchar(100),
@dateCreated dateTime,
@locale nvarchar(12)
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


-----------------------


/****** Object:  StoredProcedure [dbo].[AddOrUpdateCalendars]    Script Date: 02/06/2022 16:14:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








ALTER   PROCEDURE [dbo].[AddOrUpdateCalendars]
@uuidHash nvarchar(100),
@dueDate datetime,
@dueDateHash nvarchar(100),
@dateCreated dateTime,
@locale nvarchar(12)
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


-----------------------------------------


/****** Object:  StoredProcedure [dbo].[AddOrUpdateUserCalendar]    Script Date: 02/06/2022 16:19:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






ALTER   PROCEDURE [dbo].[AddOrUpdateUserCalendar] 
@uuidHash nvarchar(50),
@dueDateHash nvarchar(256),
@calenderId nvarchar(256),
@locale nvarchar(12)
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


----------------------------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[DeleteCalendar]    Script Date: 02/06/2022 16:21:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





ALTER PROCEDURE [dbo].[DeleteCalendar] @dueDateHash nvarchar(100), @locale nvarchar(12)
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

---------------------------------------------



/****** Object:  StoredProcedure [dbo].[DeleteEvent]    Script Date: 02/06/2022 16:21:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER PROCEDURE [dbo].[DeleteEvent] @contentId nvarchar(100)
AS

BEGIN

DELETE FROM Events
WHERE ContentId = @contentId

END;
GO

----------------------------------------------------



/****** Object:  StoredProcedure [dbo].[GetAllCalendars]    Script Date: 02/06/2022 16:22:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





ALTER PROCEDURE [dbo].[GetAllCalendars] @locale nvarchar(12)
AS

BEGIN

IF (@locale IS NULL OR @locale = '')
BEGIN
SELECT CalendarId,UuidHash,DueDate,DueDateHash,DateCreated,Locale FROM Calendar
END
ELSE 
BEGIN
SELECT CalendarId,UuidHash,DueDate,DueDateHash,DateCreated,Locale FROM Calendar WHERE Locale = @locale
END


END;
GO


--------------------------------------------


/****** Object:  StoredProcedure [dbo].[GetCalendars]    Script Date: 02/06/2022 16:25:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER PROCEDURE [dbo].[GetCalendars] @locale nvarchar(12), @dueDateHash nvarchar(100),@limit int, @sorting nvarchar(5)
AS

BEGIN
If (@limit IS NULL OR @limit = '')
BEGIN
SELECT CalendarId,UuidHash,DueDate,DueDateHash,DateCreated,Locale FROM Calendar WHERE Locale = @locale and DueDateHash = @dueDateHash
ORDER BY 
CASE WHEN @sorting = 'ASC' THEN DueDate END ASC,
CASE WHEN @sorting = 'DESC' THEN DueDate END DESC
END

If (@sorting IS NULL OR @sorting = '')
BEGIN
SELECT TOP (@limit) CalendarId,UuidHash,DueDate,DueDateHash,DateCreated,Locale FROM Calendar WHERE Locale = @locale and DueDateHash = @dueDateHash
END

IF ((@sorting IS NULL OR @sorting = '')AND (@limit IS NULL OR @limit = '') )
BEGIN
SELECT CalendarId,UuidHash,DueDate,DueDateHash,DateCreated,Locale FROM Calendar WHERE Locale = @locale and DueDateHash = @dueDateHash
END


END;
GO


------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[GetEvents]    Script Date: 02/06/2022 16:27:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER PROCEDURE [dbo].[GetEvents] @locale nvarchar(12)
AS

BEGIN

IF (@locale IS NULL OR @locale = '')
BEGIN
SELECT EventId,ContentId,LastCreated,Title,Description,Period,Number,URL,Locale,Type FROM Events
END
ELSE 
BEGIN
SELECT EventId,ContentId,LastCreated,Title,Description,Period,Number,URL,Locale,Type FROM Events WHERE Locale = @locale
END


END;
GO


--------------------------------------


/****** Object:  StoredProcedure [dbo].[GetUserCalendars]    Script Date: 02/06/2022 16:31:04 ******/
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


ALTER PROCEDURE [dbo].[GetUserCalendars] @locale nvarchar(12), @uuidHash nvarchar(100),@limit int, @sorting nvarchar(5)
AS

BEGIN

If (@limit IS NULL OR @limit = 0)
BEGIN
--DECLARE @SQL VARCHAR(MAX);
--SET @SQL = 'SELECT * FROM Calendar WHERE Locale = '+@locale+' and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = '+@uuidHash+')'
--+'ORDER BY DUEDATE '+ @sorting;
--EXECUTE sp_executesql @SQL;
--SELECT @SQL;
SELECT CalendarId,UuidHash,DueDate,DueDateHash,DateCreated,Locale FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
--GROUP BY DueDate,DateCreated,Locale,CalendarId,UuidHash,DueDateHash
ORDER BY 

 CASE WHEN @sorting='ASC' THEN DueDate END ASC,
 CASE WHEN @sorting = 'DESC' THEN DueDate END DESC,
 CASE WHEN @sorting = '' THEN DueDate END ASC
END

If (@sorting IS NULL OR @sorting = '')
BEGIN
SELECT TOP (@limit) CalendarId,UuidHash,DueDate,DueDateHash,DateCreated,Locale FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
order by DueDate ASC
END

IF ((@sorting IS NULL OR @sorting = '')AND (@limit IS NULL OR @limit = 0) )
BEGIN
SELECT CalendarId,UuidHash,DueDate,DueDateHash,DateCreated,Locale FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
order by DueDate ASC
END

If (@sorting IS not NULL AND @limit IS not NULL)
BEGIN
SELECT TOP (@limit) CalendarId,UuidHash,DueDate,DueDateHash,DateCreated,Locale FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
ORDER BY 

 CASE WHEN @sorting='ASC' THEN DueDate END ASC,
 CASE WHEN @sorting = 'DESC' THEN DueDate END DESC,
 CASE WHEN @sorting = '' THEN DueDate END ASC
END

END;
GO


--EOF

--Indexes

CREATE INDEX DueDatehash_Locale
ON calendar (DueDateHash , Locale);


CREATE INDEX ContentId_Locale
ON Events (ContentId , Locale);

CREATE INDEX UUIDHash_DueDatehash_Locale
ON usercalendar (uuidhash,DueDateHash , Locale);



------------
--Clean up db

Delete from Calendar;
Delete from UserCalendar;
