@echo off
echo 윈도우환경 pre-commit hook 설정...

:: Windows용 pre-commit 파일 복사
copy formatting\hooks\windows\pre-commit .git\hooks\pre-commit >nul

:: 성공 메시지 출력
if %ERRORLEVEL% EQU 0 (
    echo Pre-commit hook이 .git에 추가되었습니다.
) else (
    echo 실패! Pre-commit hook을 .git에 추가하지 못했습니다. 경로가 맞는지 확인해보세요.
)
