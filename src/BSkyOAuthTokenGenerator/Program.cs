// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using BSkyOauthTokenGenerator;
using ConsoleAppFramework;
using FishyFlip;
using FishyFlip.Models;
using Microsoft.Extensions.Logging.Debug;

var app = ConsoleApp.Create();
app.Add<OAuthCommands>();
app.Run(args);

/// <summary>
/// OAuth Commands.
/// </summary>
#pragma warning disable SA1649 // File name should match first type name
public class OAuthCommands
#pragma warning restore SA1649 // File name should match first type name
{
    /// <summary>
    /// Refresh session from saved json file.
    /// </summary>
    /// <param name="outputName">-o, Output Name.</param>
    /// <param name="instanceUrl">-i, Instance URL.</param>
    /// <param name="verbose">-v, Verbose logging.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    [Command("refresh")]
    public async Task RefreshSessionAsync(string outputName = "session.json", string instanceUrl = "https://bsky.social", bool verbose = false, CancellationToken cancellationToken = default)
    {
        var consoleLog = new ConsoleLog(verbose);
        if (File.Exists(outputName))
        {
            consoleLog.LogError("Session file does not exist");
            return;
        }
        var protocol = this.GenerateProtocol(new Uri(instanceUrl));
        var sessionJson = File.ReadAllText(outputName);
        var oauthSession = OAuthSession.FromString(sessionJson);
        if (oauthSession is null)
        {
            consoleLog.LogError("Failed to read session");
            return;
        }

        consoleLog.Log($"Starting OAuth2 Refresh");
        var session = await protocol.AuthenticateWithOAuth2SessionAsync(oauthSession, "http://localhost");
        if (session is null)
        {
            consoleLog.LogError("Failed to refresh session, session is null");
            return;
        }

        consoleLog.Log($"Refreshed session");
        consoleLog.Log($"Did: {session.Did}");
        consoleLog.Log($"Access Token: {session.AccessJwt}");
        consoleLog.Log($"Refresh Token: {session.RefreshJwt}");

        var savedSession = await protocol.SaveOAuthSessionAsync();
        if (savedSession is null)
        {
            consoleLog.LogError("OAuth Session is null");
            return;
        }

        consoleLog.LogDebug($"OAuth Session: {savedSession}");
        File.WriteAllText(outputName, savedSession.ToString());
        consoleLog.Log($"Session saved to {outputName}");
    }

    /// <summary>
    /// Start new session.
    /// </summary>
    /// <param name="clientId">-c, Client ID.</param>
    /// <param name="redirectUrl">-r, Redirect URL.</param>
    /// <param name="instanceUrl">-i, Instance URL.</param>
    /// <param name="port">-p, Port.</param>
    /// <param name="scopes">-s, Scopes.</param>
    /// <param name="outputName">-o, Output Name.</param>
    /// <param name="verbose">-v, Verbose logging.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    [Command("start")]
    public async Task StartSessionAsync(string clientId = "http://localhost", string? redirectUrl = default, string instanceUrl = "https://bsky.social", int port = 0, string scopes = "atproto", string outputName = "session.json", bool verbose = false, CancellationToken cancellationToken = default)
    {
        var consoleLog = new ConsoleLog(verbose);
        Uri.TryCreate(instanceUrl, UriKind.Absolute, out var iUrl);
        // If outputName does not end in ".json", add it.
        if (!outputName.EndsWith(".json"))
        {
            outputName += ".json";
        }

        if (iUrl is null)
        {
            consoleLog.LogError("Invalid instance URL");
            return;
        }

        if (string.IsNullOrEmpty(clientId))
        {
            consoleLog.LogError("Invalid Client ID");
            return;
        }

        var browser = new SystemBrowser(port);
        if (string.IsNullOrEmpty(redirectUrl))
        {
            redirectUrl = $"http://127.0.0.1:{browser.Port}";
        }
        var scopeList = scopes.Split(',');
        var protocol = this.GenerateProtocol(iUrl);
        consoleLog.Log($"Starting OAuth2 Authentication for {instanceUrl}");
        var url = await protocol.GenerateOAuth2AuthenticationUrlAsync(clientId, redirectUrl, scopeList, instanceUrl.ToString(), cancellationToken);
        var result = await browser.InvokeAsync(url, cancellationToken);
        if (result.IsError)
        {
            consoleLog.LogError(result.Error);
            return;
        }

        consoleLog.Log($"Got session, finishing OAuth2 Authentication for {instanceUrl}");

        var session = await protocol.AuthenticateWithOAuth2CallbackAsync(result.Response, cancellationToken);
        if (session is null)
        {
            consoleLog.LogError("Failed to authenticate, session is null");
            return;
        }

        consoleLog.Log($"Authenticated with {instanceUrl}");
        consoleLog.Log($"Did: {session.Did}");
        consoleLog.Log($"Access Token: {session.AccessJwt}");
        consoleLog.Log($"Refresh Token: {session.RefreshJwt}");

        var savedSession = await protocol.SaveOAuthSessionAsync();
        if (savedSession is null)
        {
            consoleLog.LogError("OAuth Session is null");
            return;
        }

        consoleLog.LogDebug($"OAuth Session: {savedSession}");
        File.WriteAllText(outputName, savedSession.ToString());
        consoleLog.Log($"Session saved to {outputName}");
    }

    private ATProtocol GenerateProtocol(Uri instanceUrl)
    {
        var debugLogger = new DebugLoggerProvider();
        var builder = new ATProtocolBuilder();
        builder.WithLogger(debugLogger.CreateLogger("ATProtocol"));
        builder.WithInstanceUrl(instanceUrl);
        return builder.Build();
    }
}