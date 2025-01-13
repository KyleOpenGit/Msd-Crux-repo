#!/bin/bash

echo "Docker 및 Docker Compose 설치 스크립트 실행 중..."

# 1. Docker가 설치되어 있다면 제거
sudo apt-get remove -y docker docker-engine docker.io containerd runc

# 2. 패키지 업데이트 및 필수 패키지 설치
sudo apt-get update -y
sudo apt-get install -y apt-transport-https ca-certificates curl software-properties-common

# 3. Docker GPG 키 추가
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg

# 4. Docker 저장소 추가
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# 5. Docker 설치
sudo apt-get update -y
sudo apt-get install -y docker-ce docker-ce-cli containerd.io

# 6. Docker Compose 설치
DOCKER_COMPOSE_VERSION="v2.20.2"
sudo curl -L "https://github.com/docker/compose/releases/download/$DOCKER_COMPOSE_VERSION/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# 7. 현재 사용자를 Docker 그룹에 추가
sudo usermod -aG docker $USER

# 8. 설치 완료 메시지 출력 및 버전 확인
echo "Docker 및 Docker Compose 설치가 완료되었습니다."
docker --version
docker-compose --version

echo "사용자 권한이 변경되었습니다. 변경 사항을 적용하려면 로그아웃 후 다시 로그인하세요."
