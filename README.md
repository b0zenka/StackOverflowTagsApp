# StackOverflow Tags App

Projekt oparty o **.NET 9 (WebAPI)**, **Angular**, **PostgreSQL** oraz **Docker**.  
Celem projektu jest:
- Pobranie co najmniej 1000 tagów z publicznego API StackOverflow i zapisanie ich w lokalnej bazie danych.
- Obliczenie procentowego udziału każdego tagu w całej pobranej populacji.
- Udostępnienie API do pobierania tagów ze stronicowaniem i sortowaniem.
- Udostępnienie metody API do ręcznego odświeżenia danych z API StackOverflow.
- Wygenerowanie dokumentacji OpenAPI (Swagger).
- Stworzenie frontendowej aplikacji w Angularze do wyświetlania danych w tabeli z paginacją i sortowaniem.
- Przygotowanie podstawowych testów jednostkowych i integracyjnych.
- Uruchamianie całego rozwiązania za pomocą jednego polecenia:  
```bash
docker compose up
```

## Technologie
- **Backend**: ASP.NET Core 9 WebAPI (C#)
- **Frontend**: Angular
- **Baza danych**: PostgreSQL
- **Konteneryzacja**: Docker + Docker Compose
- **ORM**: Entity Framework Core
- **Dokumentacja API**: Swagger / OpenAPI

## Struktura projektu
- /Backend      -> WebAPI + logika aplikacyjna + EF Core
- /Frontend     -> Angular app
/docker-compose.yml

## Jak uruchomić?
1. Wymagania
- Zainstalowany Docker i Docker Compose
- Porty:
    - 5000 – backend (WebAPI)
    - 4200 – frontend (Angular)
    - 5432 – baza danych PostgreSQL

2. Uruchomienie projektu
### W głównym katalogu projektu wykonaj:
```bash
docker compose up
```
lub w tle
```bash
docker compose up -d
```
### Docker automatycznie:
- postawi bazę danych PostgreSQL,
- zbuduje i uruchomi backend (ASP.NET Core WebAPI),
- zbuduje i uruchomi frontend (Angular).

3. Adresy usług
- Swagger / OpenAPI (backend API): http://localhost:5000/swagger/index.html
- Frontend Angular: http://localhost:4200

## API
- GET /api/tags?page=1&size=20&sortBy=name&order=asc – pobiera tagi (stronicowanie + sortowanie)
- POST /api/tags/refresh – wymusza ponowne pobranie tagów z API StackOverflow

## Testy
- Testy jednostkowe: backend (xUnit / NUnit)
- Testy integracyjne: backend API
Uruchamiane lokalnie np.:
```bash
dotnet test
```

## Deployment w Dockerze
Całość działa od razu po wykonaniu polecenia:
```bash
docker compose up
```

## Uwagi
- Pierwsze pobranie tagów może chwilę potrwać, ponieważ dane są pobierane z zewnętrznego API StackOverflow.
- W przypadku błędów 429 Too Many Requests mechanizm pozwala na ponowne pobranie później.