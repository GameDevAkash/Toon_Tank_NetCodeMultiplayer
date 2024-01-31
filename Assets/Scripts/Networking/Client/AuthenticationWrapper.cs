using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxretries = 5)
    {
        if(AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }

        if(AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already Authenticating");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxretries);

        return AuthState;

    }

    private static async Task<AuthState> Authenticating()
    {
        while(AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }
        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxretries)
    {
        AuthState = AuthState.Authenticating;

        int tries = 0;
        while (AuthState == AuthState.Authenticating && tries < maxretries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch(AuthenticationException authException)
            {
                Debug.LogError(authException);
                AuthState = AuthState.Error;
            }
            catch(RequestFailedException requestException)
            {
                Debug.LogError(requestException);
                AuthState = AuthState.Error;
            }

            tries++;
            await Task.Delay(1000);
        }

        if (AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successfully after {tries} tries");
            AuthState = AuthState.TimeOut;
        }
    }
}
public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}
