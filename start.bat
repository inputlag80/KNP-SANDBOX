@echo off
chcp 65001 >nul
cd /d "%~dp0"

echo ============================================
echo   KNP Sandbox - запуск
echo ============================================
echo.

REM --- Python ---
python --version >nul 2>&1
if errorlevel 1 goto no_python

REM --- venv ---
if not exist "venv\Scripts\activate.bat" goto create_venv
echo [1/5] venv найден.
goto after_venv

:create_venv
echo [1/5] Создаю виртуальное окружение...
python -m venv venv
if errorlevel 1 goto venv_failed

:after_venv
call venv\Scripts\activate.bat

echo [2/5] Проверяю Python-зависимости...
pip install -q -r requirements.txt
if errorlevel 1 goto pip_failed

echo [3/5] Проверяю .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 goto no_dotnet

echo [4/5] Проверяю сборку C#...
if exist "ui\KNP.Server\bin\Release\net10.0\KNP.Server.dll" goto cs_built
echo [4/5] Первая сборка C# (1-2 минуты)...
pushd ui\KNP.Server
dotnet build -c Release -v quiet
popd
if errorlevel 1 goto cs_failed
goto run

:cs_built
echo [4/5] C#-ядро уже собрано.

:run
echo [5/5] Запускаю Flask-фасад...
echo.
echo ============================================
echo   http://127.0.0.1:5000
echo   Закрой это окно, чтобы остановить всё.
echo ============================================
echo.
python launcher\app.py
pause
exit /b 0

:no_python
echo [ОШИБКА] Python не найден в PATH.
echo Установи Python 3.10+ и добавь в PATH.
pause
exit /b 1

:venv_failed
echo [ОШИБКА] Не удалось создать venv.
pause
exit /b 1

:pip_failed
echo [ОШИБКА] Не удалось установить Python-зависимости.
pause
exit /b 1

:no_dotnet
echo [ОШИБКА] .NET SDK не найден.
echo Скачай: https://dotnet.microsoft.com/download/dotnet
pause
exit /b 1

:cs_failed
echo [ОШИБКА] Сборка C# провалилась.
pause
exit /b 1