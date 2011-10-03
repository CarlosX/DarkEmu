@echo off
CLS
TITLE SREMU BACKUP

echo ===================== SREMU BACKUP SCRIPT =====================

FOR /f "tokens=1-4 delims=/ " %%a IN ('date/t') DO (
SET dw=%%a
SET mm=%%b
SET dd=%%c
SET yy=%%d
)

SET database=sremu
SET mysqluser=root
SET mysqlpass=sremu
SET backupdir=dbfiles
SET filename=%database%.%yy%-%mm%-%dd%.sql

IF EXIST %backupdir%\%filename% goto file_exists

:dump
ECHO [+] Dumping database.
mysqldump -u %mysqluser% -p%mysqlpass% %database% > %backupdir%\%filename%

IF NOT EXIST %backupdir%\%filename% goto dump_fail
ECHO [+] DONE!
GOTO end

:dump_fail
ECHO [-] Unable to dump.
GOTO end

:file_exists
ECHO [+] A file with the same date is found, it will now be deleted.
DEL %backupdir%\%filename%
goto dump

:end
echo ===============================================================
pause