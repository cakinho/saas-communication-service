#!/bin/bash
set -e

echo "=========================================="
echo "Communication Service - Iniciando..."
echo "=========================================="

# Aguardar banco de dados PostgreSQL
echo "Aguardando banco de dados..."
until nc -z ${DB_HOST:-communication-db} ${DB_PORT:-5432}; do
    echo "Banco de dados não disponível, aguardando..."
    sleep 2
done
echo "Banco de dados disponível!"

echo "=========================================="
echo "Iniciando aplicação..."
echo "=========================================="

exec dotnet CommunicationService.API.dll
