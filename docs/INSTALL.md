# 설치 도우미

본 레포지토리의 `install` 디렉토리 하위에는 몇가지 설치 도우미가 있습니다.


# Docker와 Docker Compose 설치
* [install_docker_ubuntu.sh](../install/install_docker_ubuntu.sh) 파일 : 우분투 24.04 서버에 Docker와 Docker Compose를 설치하는 셸스크립트.


`install_docker_ubuntu.sh` 파일을 서버에 업로드한뒤 실행하거나, 원하는 위치에서 비슷한 이름의 파일을 만들고 내용을 복사해서 해당파일을 실행하면 Docker와 Docker Compose를 설치하는 여러 명령어들이 순차적으로 실행됩니다.


1) 파일 생성
현재위치에 스크립트 파일 만들기 (ssh 접속후)
    ~~~
    nano install_docker_ubuntu.sh
    ~~~
    그러면 빈내용의 파일이 Nano 편집기에서 열립니다. 그곳에 내용을 복사해넣고 저장(control+x, y, 엔터)합니다.


2) 실행권한 부여

    스크립트 파일이 준비되면 이 스크립트파일에대한 실행권한을 부여합니다:
    ~~~
    chmod +x install_docker_ubuntu.sh
    ~~~
3) 실행
    파일을 실행합니다 (현재위치의 .sh 파일 실행):
    ~~~
   ./install_docker_ubuntu.sh
    ~~~

    그러면 여러명령어들이 순차적으로 실행되고 완료되면 Docker와 Docker Compose 버전을 확인할 수 있습니다.

4) ubuntu 유저 로그아웃/재로그인

    docker 명령에대한 권한 변경 사항(sudo 없이 docker 명령어 사용)을 적용하려면 ssh 접속을 끝냈다가 다시 접속하거나 다음 명령어를 입력합니다:
    ~~~
    newgrp docker
    ~~~

추가로, `docker run hello-world` 를 실행해서 Docker가 잘 실행되는지 확인해볼 수 있습니다.



