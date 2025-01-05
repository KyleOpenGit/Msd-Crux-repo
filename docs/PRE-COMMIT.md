# EditorConfig와 Git pre-commit 훅

[코딩 포매팅 컨벤션](CONVENTIONS-CODE.md)에 따라 원활한 코드 형상관리를 위해서 팀원간 서로다른 코드에디터의 포매팅 규칙을 지양하고 같은 규칙을 적용해야 할 것입니다. [EditorConfig](https://editorconfig.org)는 다양한 코드 에디터에서 공통으로 적용가능한 동일한 규칙을 적용할 수 있습니다.

솔루션폴더에서 dotnet toll 중 하나인 `dotnet format` 명령어를 사용하면 `.editorconfig` 파일의 내용대로 C# 코드가 자동 포매팅 됩니다.
> `dotnet format` 명령어를 위해서는 dotnet-format 명령줄 도구가 설치되어있어야합니다.


본 레포지토리의 닷넷 솔루션 폴더에는 `.editorconfig` 파일이 준비되어있습니다.

또한, Git의 pre-commit hook(커밋 전 실행하는 스크립트)로 커밋때마다 자동으로 `dotnet format` 명령이 실행되도록이 설정할 수 있습니다.
> 참고: [Git Hooks](https://git-scm.com/book/ko/v2/Git맞춤-Git-Hooks)


본 레포지토리에는  `dotnet format`을 실행하는 코드가 작성된 pre-commit 파일을 로컬 레포지토리에 추가해주는 스크립트 파일이 준비되어있습니다.
다음 안내에 따라 dotnet-format 도구와 pre-commit을 추가하고 나면 git commit 할 때마다 자동으로 포매팅이 실행된 후 커밋 됩니다.


## Prerequisites

pre-commit이 실행하는 `dotnet format` 명령어가 실행되려면 먼저 컴퓨터에 dotnet-format CLI 도구가 설치되어야 합니다.

* dotnet format 설치:
  > `dotnet tool install -g dotnet-format`

전역 설치: 위 명령어는 도구를 전역으로 설치하므로, 시스템 어디에서나 사용할 수 있습니다.


* 최신 버전 업데이트:
> dotnet tool update -g dotnet-format


dotnet tool로 설치하는 dotnet-format 명령어 도구는 컴퓨터에 설치되는 것이므로 한번만 설치해두면 됩니다.


## 준비된 pre-commit 설치하기
> **중요**: git hooks는 .git 레포지토리 폴더에 추가되긴 하지만 git으로 트래킹되지 않으므로 원격레포지토리에 push 되지 않습니다. 그러므로 본 레포지토리를 Github에서 새롭게 클론 받는다면 아래 내용대로 다시
설정해주어야합니다.


레포지토리 루트에는 `formatting` 폴더가 다음 구조로 준비되어 있습니다.
~~~
formatting/
├── setup.sh                 # Linux/macOS 용 pre-commit 추가 스크립트
├── setup.bat                # Windows CMD 용 pre-commit 추가 스크립트
├── hooks/
│   ├── sh/
│   │   └── pre-commit       # Linux/macOS 용 Git Hook
│   └── bat/
│       └── pre-commit       # Windows CMD 용 Git Hook
~~~

사용자 컴퓨터 OS에 따라 스크립트 파일을 실행하면 각 OS에 해당하는 pre-commit 파일이 로컬 레포지토리에 추가됩니다.
**레포지토리 루트에서** 다음 명령어를 실행하세요.

* Windows 사용자:
  * `formatting\setup.bat`
* macOS/Linux 사용자:
  * `bash formatting/setup.sh`

> 참고:
> 파일 실행시 권한 문제가 있을 수 있습니다. setup 파일과 pre-commit 파일에대해 실행권한을 추가해야 할 수 있습니다.

위 setup 파일을 실행하면 로컬레포지토리의 `.git/hooks` 폴더 하위에 `pre-commit` 파일이 복사되었을 것입니다.

이제 git 으로 커밋 할 때마다 터미널 또는 git log에서 포매팅 실행여부에대한 메시지를 볼 수 있을 것입니다.

