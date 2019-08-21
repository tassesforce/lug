# Lug

Framework developed for LIVE. Anyone is free to use it, it would be our pleasure.

Be assured that we would be glad to hear about you, where do you work and why you chosed to use our lib :)
You can contact me anytime here : florian.c@learn-virtual.com

main features :
- ApiErrorResponse : Return class of a web api in case of error
- TokenHandler : Handler to manage a JWT (OAuth2)
- HttpHelper : Helper which can create a basic RestRequest whith the common headers (AUthorization, etc.), URI, etc.
- CorrelationId : Offer Middleware and context to add correlationId to your services
- RemoteIpMiddleware : Add a property Address to the LogContext (contains the remoteIp)
- RequestResponseLoggingMiddleware : Logs of the HTTP request/response for a web api
- SerilogMiddleware
- StringEncrypterHelper : helper d'encryption/decryption de string