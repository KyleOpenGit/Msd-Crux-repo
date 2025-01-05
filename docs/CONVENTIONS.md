# 환경별 서버 구성과 Git 브랜치 전략

## ASP.NET Core의 구성 파일

MSD Crux 서버 애플리케이션의 구성 설정은 다음 세 가지 서버에 배포되는 시나리오를 갖춥니다.
* 개발 서버(Dev)
* 프로덕션 서버(Prod) :라이브 서버

각 서버별 구분을 위해 환경변수를 사용하며 환경변수마다 다른 설정을 로드합니다.

### 환경변수
* `Local` : 로컬 컴퓨터 환경
* `Development`: Dev 서버
* `Production`: Prod 서버
> ASP.NET 프레임워크에서 기본지원하는 `Development`, `Stage`, `Produciton` 중 `Stage`는 생략

### appsettings.json
 ASP.NET Core의 구성 파일은 appsettings.`{환경}`.json 형식의  네이밍으로 이루어집니다.
 > 여기에서 {환경}이란, ASP.NET Core 환경변수 : `ASPNETCORE_ENVIRONMENT`의 값을 말합니다. [ASP.NET Core에서 여러 환경 사용(MS 공식문서)](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/environments?view=aspnetcore-8.0) 참고

본 레포지토리(MSD Crux)에서는 개발 컴퓨터(로컬환경)을 개발서버(Dev)와 구분하기 위해 `appsettings.Local.json` 구성파일이 추가됩니다.

구성 파일에 대한 git 트래킹은 다음 두 파일에대해서만 이루어지며:
* appsettings.json - 일반 공통 설정 (비밀정보 미포함)
* appsettings.Local.json - 로컬 전용. github에 공개가능한 참고용 
비밀정보 설정 예제

> 중요: `appsettings.Local.json`에 작성될 수 있는 비밀정보는 오직 개인 PC 로컬환경 전용이므로 이곳에 개발/프로덕션 서버의 비밀정보를 넣어서는 안됩니다 (이 파일은 git으로 트래킹되므로 인터넷에 공개됩니다.)

서버에 배포시 각 환경별 서버에 배포되어야할 Development, Stage, Production 구성파일은 서버환경에 맞는 구성파일을 직접 포함해서 배포하거나, 각각의 환경변수를 직접 설정해야합니다.


## Git 브랜치 전략 및 컨벤션
MSD Crux 소스코드 레포지토리는 환경별 서버 배포 시나리오에 맞춰, [gitlab flow](https://about.gitlab.com/blog/2023/07/27/gitlab-flow-duo/)와 같은 브랜치 전략을 갖습니다.

* `main`(master): 개발 통합 브랜치. 배포예정 브랜치.
* `feat`과 `fix` : main에 병합할 PR용 브랜치
* `dev` : 개발 서버에 배포된 코드
* `prod` : 프로덕션 서버에 배포된 코드

### 브랜치 PR 컨벤션
PR용 브랜치는 약속된 구분플래그로 구조화된 브랜치명을 사용합니다.  **{구분플래그}/{MyBranch}** 형식.

* 구분 플래그:
  * feat: 기능추가
  * fix: 오류수정, hot-fix
* 브랜치명 예시:
  * `feat/로그인API엔드포인트_추가`
  * `feat/로그인화면_추가`
  * `fix/메모리릭_수정`

orgin(github)에 올라온 각 feat,fix 브랜치는 main 브랜치에 병합되고도 개발, 스테이지서버에서 테스트를 마친 뒤 prod 브랜치에 병합 되면 삭제됩니다.
