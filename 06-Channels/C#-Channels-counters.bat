if exist C#-Channels-counters.exe del C#-Channels-counters.exe

csc  -r:mscorlib.dll,netstandard.dll,System.Threading.Channels.dll,System.Threading.Tasks.Extensions.dll,System.Runtime.CompilerServices.Unsafe.dll C#-Channels-counters.cs
pause

C#-Channels-counters.exe
pause
