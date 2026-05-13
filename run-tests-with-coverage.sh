#!/bin/bash

echo "Limpando resultados anteriores..."
rm -rf ./test-results
mkdir -p ./test-results/coverage-report

echo "Rodando testes com cobertura de código (ignorando Migrations)..."
# Adicionado o /p:Exclude="[*]*Migrations.*"
dotnet test /p:CollectCoverage=true \
            /p:CoverletOutputFormat=cobertura \
            /p:Exclude="[*]*Migrations.*" \
            --logger "console;verbosity=detailed"

echo "Gerando relatório HTML..."
reportgenerator -reports:"./**/*coverage.cobertura.xml" -targetdir:"./test-results/coverage-report" -reporttypes:Html

echo "Relatório gerado em: ./test-results/coverage-report/index.html"