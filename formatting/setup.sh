#!/bin/bash

echo "$(uname): .git에  pre-commit hook 추가..."

# 플랫폼 확인 (Linux 또는 macOS)
if [[ "$(uname)" == "Linux" || "$(uname)" == "Darwin" ]]; then
    # Linux/macOS 용 pre-commit 파일 복사
    cp ./formatting/hooks/bash/pre-commit .git/hooks/pre-commit
    chmod +x .git/hooks/pre-commit
    echo "성공 Pre-commit hook이 .git에 추가되었습니다."
else
    echo "실패! Pre-commit hook을 .git에 추가하지 못했습니다. 플랫폼과 경로가 맞는지 확인해보세요."
    exit 1
fi
