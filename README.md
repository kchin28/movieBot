# movieBot

## Build
To build moviebot, run
```
cd dbot/
docker build . -t <id>/moviebot:<version>
```

To build moviebot for a specific platform, run
```
cd dbot/
docker build --platform linux/arm/v7 . -t <id>/moviebot:<version>
```

To build moviebot for multiple platforms, run
```
cd dbot/
docker buildx build --push --platform linux/amd64,linux/arm/v7 --tag <id>/moviebot:<version> .
```
Up to date builds can be found at https://hub.docker.com/r/dktrotti/moviebot

## Deployment
1. Create volume a volume for the configuration data
```
docker volume create <volumeName>
```

2. Mount the volume in a temporary container and copy the secrets file from `/hostConfig` to `/config`. Ensure that serialization output is configured to be output into `/config`, otherwise the data will be lost when the container stops.
```
docker run -it --rm -v <volumeName>:/config -v /host/path/to/config:/hostConfig --entrypoint=/bin/sh dktrotti/moviebot
```

3. Run moviebot
```
docker run -d -v volumeName:/config --name <name> --restart=unless-stopped dktrotti/moviebot /config/secrets.json
```
