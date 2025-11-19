# Projet Secret (GameCollect API)

Bienvenue sur l'API GameCollect ! Il s'agit d'une API RESTful construite avec .NET 8 et Entity Framework Core 8 pour gérer une collection de jeux vidéo.

Le projet est entièrement conteneurisé avec Docker, incluant une base de données PostgreSQL, l'authentification JWT et une politique CORS flexible pour le développement.

## Stack Technique

- **Framework :** .NET 8 Web API
- **Base de Données :** PostgreSQL 16
- **ORM :** Entity Framework Core 8
- **Authentification :** JSON Web Tokens (JWT)
- **Conteneurisation :** Docker & Docker Compose
- **Test & Documentation :** Swagger (OpenAPI)
- **Génération de données :** Bogus

---

## Prérequis

Pour lancer ce projet, vous avez **seulement** besoin des outils suivants installés sur votre machine locale :

- [Docker](https://www.docker.com/products/docker-desktop/)Desktop)

**Vous n'avez PAS besoin d'installer le SDK .NET ou PostgreSQL localement**, car ils sont tous les deux gérés à l'intérieur des conteneurs Docker.

---

## Installation (Première fois)

1.  **Créer le fichier d'environnement**
    Ce projet nécessite un fichier `.env` à la racine pour stocker les secrets. Créez un fichier nommé `.env` et copiez-y le contenu suivant :

    ```.env
    # Configuration de la base de données
    POSTGRES_USER=postgres
    POSTGRES_PASSWORD=postgres
    POSTGRES_DB=projetsecret
    CONNECTIONSTRINGS__DEFAULT=Host=projetsecret-db;Port=5432;Database=projetsecret;Username=postgres;Password=postgres

    # Configuration JWT
    JWT_SECRET=VOICI_LA_CLE_SECRETE_PERSONNALISEE_DE_MINIMUM_32_CARACTERES
    JWT_ISSUER=http://localhost:8080
    JWT_AUDIENCE=http://localhost:8080
    ```

    > **Important :** Remplacez `VOTRE_CLE_SECRETE_PERSONNALISEE...` par une vraie chaîne de caractères longue et aléatoire.

2.  **Construire les conteneurs**
    La première fois, vous devez construire les images Docker (en particulier l'image de l'API) :
    ```bash
    docker compose build
    ```

---

## Démarrage (Développement)

Pour lancer l'environnement de développement complet (API + Base de données) :

```bash
docker compose up
```

L'API est accessible sur http://localhost:8080 (qui affiche directement l'interface Swagger).

La base de données Postgres est exposée sur le port 5432 (si vous avez besoin d'y accéder avec un client BDD comme DBeaver).

Hot Reload
Le service api est configuré pour utiliser dotnet watch. Cela signifie que toute modification que vous effectuez dans le code source (fichiers .cs, Program.cs, etc.) déclenchera automatiquement une recompilation et un redémarrage du serveur, sans que vous ayez besoin de relancer docker compose.

---

## Gestion de la Base de Données

Les commandes relatives à la base de données (migrations, seeding) doivent être exécutées à l'intérieur du conteneur de l'API.

```Bash
docker compose exec -it api bash
```

Vous serez alors à l'intérieur du conteneur (root@...:/src#).

**_Pour utiliser les outils de dotnet, il faudra problablement lancer la commande de restauration :_**

```bash
dotnet ef restore
```

**Migrations (Mise à jour du schéma)**
Chaque fois que vous modifiez les fichiers dans /Models ou le GameCollectDbContext, vous devez créer une nouvelle migration.

```Bash
# 1. Créer la migration
dotnet ef migrations add NomDeVotreMigration

# 2. Appliquer la migration à la BDD
dotnet ef database update
```

**Seeding (Peupler la BDD)**

```Bash
dotnet run seed
```

Ce script va :

- Créer 1 Admin

- Email : admin@gamecollect.com

- Mot de passe : Password123!

- Créer 100 Utilisateurs (Mot de passe : Password123!)

- Créer 500 Jeux

- Créer ~1000 Entrées de collection

- Créer ~500 Avis

---

## Tester l'API avec Swagger

L'API est protégée par JWT. Voici le flux de travail pour tester les endpoints sécurisés :

Rendez-vous sur http://localhost:8080.

Créez un compte via POST /api/users/register.

Connectez-vous avec ce compte via POST /api/users/login.

Copiez le token reçu dans la réponse.

Cliquez sur le bouton Authorize en haut à droite de Swagger.

Dans la pop-up, écrivez Bearer (avec un espace) suivi de votre token. Exemple : Bearer eyJhbGciOi...

Cliquez sur "Authorize".

Vous êtes maintenant authentifié et pouvez tester tous les endpoints protégés (ex: GET /api/games, POST /api/collection, etc.).

---
