# Variables
API_PROJECT := Elevator.WebApi/Elevator.WebApi.csproj
TEST_PROJECT := Elevator.WebApi.Tests/Elevator.WebApi.Tests.csproj

deps: 
	dotnet tool install -g dotnet-reportgenerator-globaltool || true
 
build:
	dotnet build $(API_PROJECT)

run:
	dotnet run --project $(API_PROJECT)

test: deps
	dotnet test $(TEST_PROJECT) --collect:"XPlat Code Coverage" 
	reportgenerator \
    	-reports:"**/coverage.cobertura.xml" \
    	-targetdir:"coveragereport" \
    	-reporttypes:Html \

clean:
	rm -rf ./publish
	rm -rf ./coveragereport	

# this is just an example to demonstrate docker containerization
deploy: build
	dotnet publish $(API_PROJECT) -c Release -o ./publish
	docker build -t elevator-api ./publish
	docker tag elevator-api:latest registry.heroku.com/elevator-api/web
	docker push registry.heroku.com/elevator-api/web
	heroku container:release web -a elevator-api

# this is just an example to demonstrate a possible load test
load: 
	docker run -it --rm --net=host -v $(PWD)/loadtest:/var/loadtest --entrypoint /bin/sh direvius/yandex-tank

