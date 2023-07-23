copy ..\StarDust.Emitter\build\*.dll .\build\
copy ..\StarDust.Emitter\build\*.pdb .\build\
copy ..\StarDust.Interpreter\build\*.dll .\build\
copy ..\StarDust.Interpreter\build\*.pdb .\build\

robocopy .\Data .\build\Data\ /E
if %errorlevel% leq 1 exit 0 else exit %errorlevel%