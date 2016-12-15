net stop r1sm
InstallUtil /u RSMService.exe
copy bin\RSMSupport.dll .
copy /y ..\RSMService\bin\Debug\RSMService.exe .
InstallUtil /LogToConsole=true RSMService.exe
net start r1sm