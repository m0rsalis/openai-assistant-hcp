## How to run

- Create `api-key.txt` file to root folder with your api key in it
- Run the app
- Send your request to `https://localhost:7296/chat` (launches swagger with model definitions)
- Requests should be in format:

```
{
    "RequestMessage": "{User message}",
    "SessionId": "{empty for first request, then use session ID returned by first response}"
}
```
