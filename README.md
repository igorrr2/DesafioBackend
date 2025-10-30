1. Para rodar a api é necessário ter instalado a versão 7.0 do DotNet framework

2. Abrir o Cmd na raiz do projeto e rodar o comando dotnet ef database update que irá criar o banco de dados e suas tabelas correspondentes (caso o comando falhe, criar o banco DesafioBackend e rodar os scripts na mão)

-- Tabela de Motos

    CREATE TABLE "Motos" (
    "Id" SERIAL PRIMARY KEY,
    "Identificador" VARCHAR(100) NOT NULL UNIQUE,
    "Modelo" VARCHAR(100) NOT NULL,
    "Ano" INT NOT NULL,
    "Placa" VARCHAR(20) NOT NULL UNIQUE
    );


-- Tabela de Entregadores

    CREATE TABLE "Entregadores" (
    "Id" SERIAL PRIMARY KEY,
    "Identificador" VARCHAR(150) NOT NULL UNIQUE,
    "Nome" VARCHAR(150) NOT NULL,
    "Cnpj" VARCHAR(20) NOT NULL UNIQUE,
    "DataNascimento" DATE NOT NULL,
    "NumeroCNH" VARCHAR(20) NOT NULL UNIQUE,
    "TipoCNH" VARCHAR(10) NOT NULL, -- A, B ou A+B
    "ImagemCNH" VARCHAR(255) -- caminho do arquivo no storage
    );

-- Tabela de Locações

    CREATE TABLE "Locacoes" (
    "Id" SERIAL PRIMARY KEY,
    "Identificador" VARCHAR(150) NOT NULL UNIQUE,
    "EntregadorId" VARCHAR(150) NOT NULL REFERENCES "Entregadores"("Identificador") ON DELETE CASCADE,
    "MotoId" VARCHAR(100) NOT NULL REFERENCES "Motos"("Identificador") ON DELETE CASCADE,
    "DataInicio" DATE NOT NULL,
    "DataPrevistaFim" DATE NOT NULL,
    "DataFim" DATE, -- pode ser NULL até o entregador devolver
    "PlanoDias" INT NOT NULL,
    "ValorDiaria" DECIMAL(10,2) NOT NULL,
    "Multa" DECIMAL(10,2) DEFAULT 0,
    "ValorTotal" DECIMAL(10,2) DEFAULT 0
    );


-- Tabela MotosNotificadas 

    CREATE TABLE "MotosNotificadas" (
    "Id" SERIAL PRIMARY KEY,
    "Identificador" VARCHAR(100) NOT NULL,
    "Ano" INT NOT NULL,
    "Modelo" VARCHAR(100) NOT NULL,
    "Placa" VARCHAR(20) NOT NULL,
    "DataNotificacao" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
    );


4. Após configurar connection com banco, rode docker-compose up -d, comando irá rodar o rabbitMQ na url http://localhost:15672/ (Caso seja necessário, pode alterar o arquivo docker-compose.yml para alterar a senha do rabbitMQ)

5. Após rodar o rabbitMQ, rode o projeto da api no Visual Studio que irá abrir a tela do swagger
