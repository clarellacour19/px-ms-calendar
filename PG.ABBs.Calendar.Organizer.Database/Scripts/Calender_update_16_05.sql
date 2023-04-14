-- update column
ALTER TABLE EVENTS
ALTER COLUMN TITLE NVARCHAR(64) COLLATE Latin1_General_100_CI_AI_SC_UTF8;

ALTER TABLE EVENTS
ALTER COLUMN DESCRIPTION NVARCHAR(MAX)  COLLATE Latin1_General_100_CI_AI_SC_UTF8;


--update SP

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


ALTER PROCEDURE [dbo].[GetUserCalendars] @locale nvarchar(max), @uuidHash nvarchar(max),@limit int, @sorting nvarchar(max)
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
order by DueDate ASC
END

IF ((@sorting IS NULL OR @sorting = '')AND (@limit IS NULL OR @limit = 0) )
BEGIN
SELECT * FROM Calendar WHERE Locale = @locale and DueDateHash IN (select DueDateHash from UserCalendar where UuidHash = @uuidHash)
order by DueDate ASC
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


