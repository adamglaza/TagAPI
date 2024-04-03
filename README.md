# TagAPI

TagAPI is a C# web API that downloads tags from StackOverflow API and stores them locally on an MSSQL database.

Both the web API and the MSSQL database are containerized using docker.

This repository also contains several unit tests and integration tests.
## Usage

Use docker [docker](https://www.docker.com/products/docker-desktop/) to run the containers.

```bash
docker compose up
```
## Config

This app can be configured with by modifying the following files: docker-compose.yml and TagAPI/appsettings.json .

## License

[MIT](https://choosealicense.com/licenses/mit/)