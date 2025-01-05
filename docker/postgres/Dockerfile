# TimescaleDB 공식 이미지 사용
FROM timescale/timescaledb:latest-pg17

# 환경 변수 설정
ENV POSTGRES_USER=root
ENV POSTGRES_PASSWORD=vaporcloud
ENV POSTGRES_DB=msd_db

# 초기화 스크립트 복사
COPY init.sql /docker-entrypoint-initdb.d/