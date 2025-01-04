# MESCADAS Crux
MESCADAS Crux는 ***MESCADAS 프로젝트*** 의 중앙 서버 웹애플리케이션으로, MESCADAS  핵심 서비스의 관리와 조율을 위한 중심 허브 역할을 수행하도록 설계됩니다. 강력하고 고성능인 .NET Core를 기반으로 구축된 MESCADAS Crux는 현대 애플리케이션을 위한 확장 가능하고 신뢰할 수 있는 기반을 제공합니다.
> 이하, MSD Crux


# ■ MSD Crux 소스 코드 관리
## 컨벤션
* [환경별 서버 구성과 Git 브랜치 전략](./docs/CONVELTIONS.md)
* [DB 네이밍 및 C# 코딩 컨벤션](./docs/CONVENTIONS-CODE.md)



# ■ 로컬 데이터베이스 Dockerfile
MSD Crux 서버앱에서 사용하는 RDBMS는 TimescaleDB 플러그인이 활성화된 PostgreSQL(포스트그레스)입니다.

본 레포지토리에는 PostgreSQL을 실행할 수 있도록 도커파일이 준비되어있습니다.

개발 중 개인 PC에 포스트그레스를 별도 설치,설정할 필요 없이 로컬개발에 필요한 DB/스키마, 테이블을 생성하는 도커 컨테이너를 실행할 수 있습니다.

[DOCKER.md](./docs/DOCKER.md)(로컬 PC 개발환경에서 데이터베이스 사용) 내용을 확인하세요.

