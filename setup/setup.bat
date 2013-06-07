@echo off
if exist svnrepo rd /s /q svnrepo
svnadmin create svnrepo
"C:\Program Files\Inno Setup 5\Compil32.exe" /cc setup.iss
