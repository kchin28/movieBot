FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine as moviebot-build

COPY ./ /source/
WORKDIR /source
RUN dotnet publish -c Release -r linux-arm -o /publish

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim-arm32v7 as moviebot
COPY --from=moviebot-build /publish /dbot
ENTRYPOINT ["/dbot/dbot"]

# TODO: Update serialization location in config files to point to volume

# TODO: Create instructions for deployment
# 1. Build image - docker build --platform linux/arm/v7 .
# 2. Create volume - docker volume create volumeName
# 3. Populate volume - docker run -it --rm -v volumeName:/config -v /host/path/to/config:/hostConfig --entrypoint=/bin/sh moviebot
#     a. Copy the configuration from /hostConfig to /config
# 4. Run moviebot docker run -d -v volumeName:/config moviebot /config/secrets.json
