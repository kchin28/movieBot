FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-amd64 as moviebot-build

ARG TARGETPLATFORM

COPY ./ /source/
WORKDIR /source
RUN if [ "$TARGETPLATFORM" = "linux/amd64" ]; then \
        RID=linux-x64 ; \
    elif [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
        RID=linux-arm64 ; \
    elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then \
        RID=linux-arm ; \
    fi \
    && dotnet publish -c Release -r $RID -o /publish

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim as moviebot
COPY --from=moviebot-build /publish /dbot
ENTRYPOINT ["/dbot/dbot"]
