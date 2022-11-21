if exist F#-Hopac-counters.exe del F#-Hopac-counters.exe

fsc -r:Hopac.dll -r:Hopac.Core.dll F#-Hopac-counters.fs
pause

F#-Hopac-counters.exe
pause
