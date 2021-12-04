@call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat" -no_ext -winsdk=none %*
@setlocal
@pushd %~dp0
@set _C=Release
@set _P=%~dp0build\%_C%\publish


msbuild -t:restore -p:Configuration=%_C% || exit /b

msbuild -p:Configuration=%_C% || exit /b

REM msbuild src\00\KSociety.Base.InstallAction\KSociety.Base.InstallAction.csproj -p:Configuration=%_C% -p:TargetFramework="net461" -p:OutputPath="%_P%\KSociety.Base.InstallAction\%_C%\net461" || exit /b

REM msbuild src\00\KSociety.Base.InstallAction\KSociety.Base.InstallAction.csproj -p:Configuration=%_C% -p:OutputPath="%_P%\KSociety.Base.InstallAction" || exit /b

REM msbuild src\00\KSociety.Base.InstallAction -target:Pack -p:Configuration=%_C% || exit /b

@popd
@endlocal