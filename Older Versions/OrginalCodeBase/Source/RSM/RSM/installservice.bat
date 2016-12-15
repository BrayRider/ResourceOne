net stop r1sm
InstallUtil /u RSMService.exe
InstallUtil /LogToConsole=true RSMService.exe
net start r1sm