up:
	docker-compose up --build
down:
	docker-compose stop && docker-compose down
test:
	dotnet test Money.Test/
	