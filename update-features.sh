#!/bin/bash

# Traer los últimos cambios
git fetch origin

# Lista de ramas feature/
for branch in $(git branch --list "feature/*" | sed 's/* //'); do
    echo "======================================"
    echo "Actualizando $branch desde develop..."
    echo "======================================"

    git checkout $branch || continue
    git merge origin/develop
done

# Volver a develop al final
git checkout develop