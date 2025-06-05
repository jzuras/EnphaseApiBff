# Backend for Frontend (BFF) for Enphase API

This is a hybrid (Server and WASM) Blazor Web App that uses a Backend For Frontend (BFF) 
pattern to interact with the Enphase API, which is used to monitor solar energy systems.
This app reads and displays data from various endpoints of the Enphase API. Enphase also 
provides a way to get data directly from the Enphase Envoy installed on the same network 
as the app; I plan to experiment with that in a future project.

Unlike my previous repos, this one might prove useful to anyone who wants to learn about their own 
Enphase system, or to see an example of a BFF for WASM.

There is a backstory to explain why I wanted/needed to learn how to use the Enphase API; 
[click here](BackStory.md) to learn more. (Short version: Enphase seems unable to monitor their own 
systems properly, so I need to create software to do so myself.) I also [wrote about](History.md) 
how the data shows some of the history of my system, using a couple of screenshots from my app.

I documented some lessons learned with this project and others on my 
[LinkedIn page.](https://www.linkedin.com/in/jimzuras)

This project is Blazor Hybrid because my recent work has been with Blazor Server, and I wanted to 
explore Blazor WASM again. I also wanted to remember how to use the InteractiveAuto rendering mode,
which first uses server-side rendering, then switches to client-side rendering after the initial load.

(Note: at the time of this writing, the Interactive part is not actually needed or used, since
the "buttons" are all href links.)


## Why this project does not use the standard OIDC authentication middleware

The ASP.NET Core's authentication middleware expects a standard OIDC API, but the Enphase API is 
non-standard, though it is OAuth compatible. This required creating the app with the 
template setting for Auth set to None and implementing some stuff manually 
(with the help of Claude AI).

Some of the non-standard aspects for the Enphase API: OIDC discovery, token exchange, 
URI redirect handling, and how it expects the API Key and client credentials. 
These are handled by the custom "/login" and /signin-enphase" minimal APIs.

Also handled manually is the token refresh. I ran into a timing / concurrency issue with the
Claude-generated code for refreshing the token - see 
[RefreshTokenIssue.md](RefreshTokenIssue.md) for details.


## BFF requirement

The app uses InteractiveAuto as a rendering mode, which requires extra work to get the 
token from the cookie (see RemoteAuthenticationStateProvider in the Client project). 
But that wasn't enough, because the initial login was done in the Server project, so the 
Client (WASM) was never authenticated. This, combined with not having secrets
in Client-side code, is why the BFF pattern is necessary.

For this project, I implemented BFF endpoints using a controller in the Server that called the 
same EnphaseBffService that the web app code used directly. This server-side service implemented
an Interface accessible to both projects and used for DI. On the client side, the implementation of 
the Interface called the BFF endpoints in the Server project.

An alternative approach would have been to use the same Service in both client and server,
which called the local endpoints, and the endpoints would call the Enphase API. Either way, 
the Razor code is the same, and does not know or care if it is running on the Server or Client side.

## Inconsistent Method and Responses

Astute readers will notice that the GetMicroinvertersSummaryAsync method in the Server-side
EnphaseBffService.cs file is not consistent with the other methods, which are more succinct.
This is due to an error in the Enphase API documentation for that endpoint, which required
me to take a closer look at the API response. The documentation incorrectly states that 
the last report date is a string, not a long like the other endpoints. There was also an
issue with the Enphase API returning a list which as far as I can tell is always a single item,
so I changed the response class to ignore the list.

Another endpoint, Energy Lifetime, returns lists of different lengths for overall production
vs meter or micro production, so the Razor code finds the max length and uses default values 
for the shorter lists.
