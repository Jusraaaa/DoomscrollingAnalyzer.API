# Doomscrolling Analyzer

## Docker database setup

Start the SQL Server 2022 database container:

```bash
docker compose up -d
```

Stop and remove the Compose-managed container:

```bash
docker compose down
```

Start the existing SQL Server container:

```bash
docker start doomscrolling-sql
```

Stop the existing SQL Server container:

```bash
docker stop doomscrolling-sql
```
