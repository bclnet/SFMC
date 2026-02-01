NODE_TLS_REJECT_UNAUTHORIZED=0

sf org list auth
sf org login web --instance-url https://localhost:7019 -a sf
sf org logout --target-org me@my.org

START ngrok:
ngrok http 5019

START Redis:
docker run -p 6379:6379 redis


https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/authorization-code.html
https://developer.salesforce.com/docs/marketing/marketing-cloud/references/mc_rest_auth?meta=getAccessToken


REDIS
https://redis.io/tutorials/develop/dotnet/aspnetcore/caching/basic-api-caching/