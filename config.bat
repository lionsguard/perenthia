@set Config=%1
@set ProjectDir=%2
@set TargetDir=%3

echo Config = %Config%
echo ProjectDir = %ProjectDir%
echo TargetDir = %TargetDir%

echo Copying %ProjectDir%\AppSettings_%Config%.config to %TargetDir%\AppSettings.config
copy %ProjectDir%\AppSettings_%Config%.config "%TargetDir%\AppSettings.config"

echo Copying %ProjectDir%\ConnectionStrings_%Config%.config to %TargetDir%\ConnectionStrings.config
copy %ProjectDir%\ConnectionStrings_%Config%.config "%TargetDir%\ConnectionStrings.config"