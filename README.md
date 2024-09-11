# BSkyOAuthTokenGenerator

BSkyOAuthTokenGenerator is a simple .NET application for creating an OAuth token with [Bluesky](https://bsky.app) and the [ATProtocol](https://atproto.com/guides/overview). It can be useful for:

- Getting an OAuth token for use with other services
- Testing an existing OAuth Client ID (Ex. `client-metadata.json`)
- Doing mock OAuth sessions to force redirect actions to happen on other, installed, Bluesky applications.

It is built on top of [FishyFlip](https://github.com/drasticactions/fishyflip) and .NET NativeAOT. Once published, the outputted builds can be run independent of .NET itself.

## Usage

For those just wanting to test OAuth in general, the default values should be enough to complete the flow and get a token.

 **NOTE:** For those on Linux, `xdg-utils` is required to start the browser session. If no browsers are installed, you can still take the URL given and use it in your own browser session and it _should_ work.


```
Usage: [command] [-h|--help] [--version]

Commands:
  refresh    Refresh session from saved json file.
  start      Start new session.
```

```
Usage: start [options...] [-h|--help] [--version]

Start new session.

Options:
  -c|--client-id <string>        Client ID. (Default: "http://localhost")
  -r|--redirect-url <string?>    Redirect URL. (Default: null)
  -i|--instance-url <string>     Instance URL. (Default: "https://bsky.social")
  -p|--port <int>                Port. (Default: 0)
  -s|--scopes <string>           Scopes. (Default: "atproto")
  -o|--output-name <string>      Output Name. (Default: "session.json")
  -v|--verbose                   Verbose logging. (Optional)
```

```
Usage: refresh [options...] [-h|--help] [--version]

Refresh session from saved json file.

Options:
  -o|--output-name <string>     Output Name. (Default: "session.json")
  -i|--instance-url <string>    Instance URL. (Default: "https://bsky.social")
  -v|--verbose                  Verbose logging. (Optional)
```

## Build

- Be sure to check out the submodules!
- `dotnet build` on the project or solution file should be enough to build the project.

## Third Party Libraries

- [ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework)