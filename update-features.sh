#!/bin/bash

# Traer los últimos cambios de origin
git fetch origin

# Recorrer todas las ramas feature/*
for branch in $(git branch --list "feature/*" | sed 's/* //'); do
    echo "======================================"
    echo "Actualizando $branch desde develop..."
    echo "======================================"

    git checkout $branch || continue

    # Hacer merge con mensaje custom
    git merge origin/develop -m "Merge develop into $branch [AUTO]"
done

# Volver a develop al final
git checkout develop