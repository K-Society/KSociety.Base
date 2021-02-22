@setlocal
@pushd %~dp0
@set _C=Release
@set _P=%~dp0build\%_C%\publish


msbuild -t:restore -p:Configuration=%_C% || exit /b

msbuild -p:Configuration=%_C% || exit /b

msbuild -t:Pack -p:Configuration=%_C% src\00\KSociety.Base.InstallAction || exit /b

@popd
@endlocal