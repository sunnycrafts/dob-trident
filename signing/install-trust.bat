@echo off
REM Run this ONCE on the target PC, in the same folder as DOBNowTrident-publisher.cer
REM It tells Windows to trust the publisher of DOBNowTrident.exe so it launches clean.
REM No admin needed (installs into the current user's store). Click YES if a prompt appears.

echo Installing the DOB NOW Trident Host publisher certificate...
echo.
certutil -user -addstore -f Root "%~dp0DOBNowTrident-publisher.cer"
certutil -user -addstore -f TrustedPublisher "%~dp0DOBNowTrident-publisher.cer"
echo.
echo Done. If a security dialog appeared and you clicked Yes, the certificate
echo is now trusted and DOBNowTrident.exe will run without the "unknown
echo publisher" warning.
echo.
pause
