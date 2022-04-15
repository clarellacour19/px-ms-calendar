-- Create

CREATE TABLE Events(
EventId UNIQUEIDENTIFIER,
ContentId VARCHAR(100),
LastUpdated DATETIME,
Title VARCHAR(64),
Description VARCHAR(MAX),
Period VARCHAR(16),
Number INT,
URL VARCHAR(256),
CONSTRAINT pk_events PRIMARY KEY (EventId));


CREATE TABLE Calendar(
CalendarId UNIQUEIDENTIFIER,
UuidHash VARCHAR(50),
DueDate DATETIME,
DueDateHash VARCHAR(50),
DateCreated DATETIME,
CONSTRAINT pk_caldendar PRIMARY KEY (CalendarId));

ALTER TABLE Calendar
ADD Locale VARCHAR(12);
ALTER TABLE Events
ADD Locale VARCHAR(12);


ALTER TABLE	Events
ADD Type varchar(16);

CREATE TABLE UserCalendar
(UserCalendarId VARCHAR(50),
UuidHash VARCHAR(50),
DueDateHash VARCHAR(50),
CalendarId VARCHAR(50),
Locale VARCHAR(12),
CONSTRAINT pk_UserCalendar	PRIMARY KEY (UserCalendarId));