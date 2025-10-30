1. Para rodar a api � necess�rio ter instalado a vers�o 7.0 do DotNet framework

2. Criar o bando de dados DesafioBackend no PostgreSQL

3. Rodar os scripts de Create para as tabelas abaixo

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

-- Tabela de Loca��es
CREATE TABLE "Locacoes" (
    "Id" SERIAL PRIMARY KEY,
    "Identificador" VARCHAR(150) NOT NULL UNIQUE,
    "EntregadorId" VARCHAR(150) NOT NULL REFERENCES "Entregadores"("Identificador") ON DELETE CASCADE,
    "MotoId" VARCHAR(100) NOT NULL REFERENCES "Motos"("Identificador") ON DELETE CASCADE,
    "DataInicio" DATE NOT NULL,
    "DataPrevistaFim" DATE NOT NULL,
    "DataFim" DATE, -- pode ser NULL at� o entregador devolver
    "PlanoDias" INT NOT NULL,
    "ValorDiaria" DECIMAL(10,2) NOT NULL,
    "Multa" DECIMAL(10,2) DEFAULT 0,
    "ValorTotal" DECIMAL(10,2) DEFAULT 0
);

CREATE TABLE "MotosNotificadas" (
    "Id" SERIAL PRIMARY KEY,
    "Identificador" VARCHAR(100) NOT NULL,
    "Ano" INT NOT NULL,
    "Modelo" VARCHAR(100) NOT NULL,
    "Placa" VARCHAR(20) NOT NULL,
    "DataNotificacao" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

4. Ap�s configurar connection com banco, abra o cmd a raiz do projeto e rode docker-compose up -d, comando ir� rodar o rabbitMQ na url http://localhost:15672/ (Caso seja necess�rio, pode alterar o arquivo docker-compose.yml para alterar a senha do rabbitMQ)

5. Ap�s rodar o rabbitMQ, rode o projeto da api no Visual Studio que ir� abrir a tela do swagger
