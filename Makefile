# Variables
API_PROJECT := Elevator.WebApi/Elevator.WebApi.csproj
TEST_PROJECT := Elevator.WebApi.Tests/Elevator.WebApi.Tests.csproj

# Targets
build:
	dotnet build $(API_PROJECT)

run:
	dotnet run --project $(API_PROJECT)

test:
	dotnet test $(TEST_PROJECT)
