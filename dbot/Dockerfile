FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine as moviebot-build

COPY ./ /source/
WORKDIR /source
RUN dotnet publish -c Release -r linux-arm -o /publish

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim-arm32v7 as moviebot
COPY --from=moviebot-build /publish /dbot
ENTRYPOINT ["/dbot/dbot"]
