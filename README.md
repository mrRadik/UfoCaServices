# UfoCaServices

Для работы необходимо установить RabbitMq и PostgreDB. Можно развернуть в докере. 

docker run -it -d --rm --name rabbitmq -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=user -e RABBITMQ_DEFAULT_PASS=password  rabbitmq:3.10-management
docker run --name postgres-ca -e POSTGRES_PASSWORD=1qaz@WSX -p 5432:5432 -d postgres
